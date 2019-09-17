/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Npgsql;
using PTV.Framework;
using PTV.Framework.Logging;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.DirectRaw
{
    [RegisterService(typeof(IDatabaseRawContext), RegisterType.Transient)]
    internal class DatabaseRawContext : IDatabaseRawContext
    {
        internal class DatabaseRawAccessorOptions
        {
            internal NpgsqlConnection DbConnection { get; set; }
            internal NpgsqlTransaction DbTransaction { get; set; }
            internal bool Saved { get; set; } = false;
        }

        private const int MaxRetries = 3;
        private const IsolationLevel ReaderIsolation = IsolationLevel.ReadCommitted;
        private const IsolationLevel WriterIsolation = IsolationLevel.Serializable;

        const string PostgreDeadLockCode = "40P01";
        const string PostgreSerializationFailure = "40001";
        const string PostgreTooManyConnections = "53300";
        const string PostgreOutOfMemory = "53200";
        const string PostgreDiskIsFull = "53100";
        const string PostgreInsufficientResources = "53000";
        private const string DbOperationFailedFatal = "Fatal DB error occured, operation failed.";
        private const string DbOperationFailedUnknown = "DB operation failed.";
        private const string DbFailedRetriesExceeded = "Retries of operation exceeded limit";

        readonly string[] temporaryPostgreErrors = { PostgreTooManyConnections, PostgreOutOfMemory, PostgreInsufficientResources };
        private readonly string connectionString;
        private IHttpContextAccessor httpContextAccessor;
        private ILogger logger;

        public DatabaseRawContext(MainConnectionString connectionString, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory)
        {
            this.connectionString = connectionString.ConnectionString;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = loggerFactory.CreateLogger(this.GetType().Name);
        }

        
        private bool HasDeadLockOccured(Exception e)
        {
            switch (GetPostgreErrorCode(e))
            {
                case PostgreSerializationFailure: return true;
                case PostgreDeadLockCode: return true;
                default: return false;
            }
        }
        
        private bool HasCriticalkOccured(Exception e)
        {
            switch (GetPostgreErrorCode(e))
            {
                case PostgreDiskIsFull: return true;
                default: return false;
            }
        }

        private string GetPostgreErrorCode(Exception e)
        {
            return ((e as PostgresException) ?? e?.InnerException as PostgresException)?.SqlState?.ToUpper();
        }

        private bool HasTemporaryOccured(Exception e)
        {
            var postgreErrorCode = GetPostgreErrorCode(e);
            bool isTemporary = !string.IsNullOrEmpty(postgreErrorCode) && temporaryPostgreErrors.Any(i => i == postgreErrorCode);
            var npgsqlException = e as NpgsqlException ?? e.InnerException as NpgsqlException;
            var postgresException = (e as PostgresException) ?? e?.InnerException as PostgresException;
            isTemporary |= npgsqlException?.IsTransient ?? false;
            isTemporary |= postgresException?.IsTransient ?? false;
            isTemporary |= (((npgsqlException)?.InnerException as IOException)?.InnerException as SocketException) != null;
            return isTemporary;
        }

        private void RollbackTransaction(NpgsqlTransaction transaction)
        {
            try
            {
                transaction.Rollback();
            }
            catch (Exception ex)
            {
                logger.LogDatabaseException(ex);
            }
        }
        

        private T CreateConnection<T>(Func<DatabaseRawAccessor,T> accessor, IsolationLevel isolationLevel)
        {
            var retries = 0;
            while (retries++ < MaxRetries)
            {
                using (NpgsqlConnection dbConnection = new NpgsqlConnection(connectionString))
                {
                    try
                    {
                        dbConnection.Open();
                        using (NpgsqlTransaction transaction = dbConnection.BeginTransaction(isolationLevel))
                        {
                            var dbOptions = new DatabaseRawAccessorOptions()
                                {DbConnection = dbConnection, DbTransaction = transaction};
                            try
                            {
                                var response = accessor(new DatabaseRawAccessor(httpContextAccessor, dbOptions));
                                if (isolationLevel == WriterIsolation && dbOptions.Saved)
                                {
                                    transaction.Commit();
                                }
                                else
                                {
                                    RollbackTransaction(transaction);
                                }

                                return response;
                            }
                            catch (AggregateException e) when (e.InnerException is TaskCanceledException)
                            {
                                logger.LogDatabaseException(e);
                                throw new PtvActionCancelledException();
                            }
                            catch (AggregateException e) when (e.InnerException is PtvActionCancelledException)
                            {
                                logger.LogDatabaseException(e);
                                throw e.InnerException;
                            }
                            catch (TaskCanceledException ex)
                            {
                                logger.LogDatabaseException(ex);
                                RollbackTransaction(transaction);
                                throw new PtvActionCancelledException();
                            }
                            catch (Exception e)
                            {
                                logger.LogDatabaseException(e);
                                RollbackTransaction(transaction);
                                if (HasDeadLockOccured(e) || HasTemporaryOccured(e))
                                {
                                    logger.LogDatabaseRetry(DatabaseLogEvents.Retry, e,
                                        $"{e.Message} Repeating DBContext operation.");
                                    var randomGen = new Random(Thread.CurrentThread.ManagedThreadId);
                                    Task.Delay((randomGen.Next() % 500) + 10).GetAwaiter().GetResult(); // random wait
                                    continue;
                                }

                                if (HasCriticalkOccured(e))
                                {
                                    throw new Exception(DbOperationFailedFatal, e);
                                }

                                throw new Exception(DbOperationFailedUnknown, e);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogDatabaseException(ex);
                        continue;
                    }
                    finally
                    {
                        dbConnection.Close();
                    }
                }
            }
            throw new Exception(DbFailedRetriesExceeded);
        }
        
        
        private async Task<T> CreateConnectionAsync<T>(Func<DatabaseRawAccessor, Task<T>> accessor, IsolationLevel isolationLevel)
        {
            var retries = 0;
            while (retries++ < MaxRetries)
            {
                using (NpgsqlConnection dbConnection = new NpgsqlConnection(connectionString))
                {
                    try
                    {
                        dbConnection.Open();
                        using (NpgsqlTransaction transaction = dbConnection.BeginTransaction(isolationLevel))
                        {
                            var dbOptions = new DatabaseRawAccessorOptions() {DbConnection = dbConnection, DbTransaction = transaction};
                            try
                            {
                                var response = await accessor(new DatabaseRawAccessor(httpContextAccessor, dbOptions));
                                if (isolationLevel == WriterIsolation && dbOptions.Saved)
                                {
                                    transaction.Commit();
                                }
                                else
                                {
                                    RollbackTransaction(transaction);
                                }

                                return response;
                            }
                            catch (AggregateException e) when (e.InnerException is TaskCanceledException)
                            {
                                logger.LogDatabaseException(e);
                                RollbackTransaction(transaction);
                                throw new PtvActionCancelledException();
                            }
                            catch (Exception e)
                            {
                                logger.LogDatabaseException(e);
                                RollbackTransaction(transaction);
                                if (HasDeadLockOccured(e) || HasTemporaryOccured(e))
                                {
                                    logger.LogDatabaseRetry(DatabaseLogEvents.Retry, e, $"{e.Message} Repeating DBContext operation.");
                                    var randomGen = new Random(Thread.CurrentThread.ManagedThreadId);
                                    await Task.Delay((randomGen.Next()%500) + 10); // random wait
                                    continue;
                                }

                                if (HasCriticalkOccured(e))
                                {
                                    throw new Exception(DbOperationFailedFatal, e);
                                }

                                throw new Exception(DbOperationFailedUnknown, e);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogDatabaseException(ex);
                        continue;
                    }
                    finally
                    {
                        dbConnection.Close();
                    }
                }
            }
            throw new Exception(DbFailedRetriesExceeded);
        }
        
        public void ExecuteReader(Action<IDatabaseRawAccessor> dbAccess)
        {
            CreateConnection<object>((i) =>
            {
                dbAccess(i);
                return null;
            }, ReaderIsolation);
        }
        

        public void ExecuteWriter(Action<IDatabaseRawAccessor> dbAccess)
        {
            CreateConnection<object>((i) =>
            {
                dbAccess(i);
                return null;
            }, WriterIsolation);
        }

        public T ExecuteReader<T>(Func<IDatabaseRawAccessor, T> dbAccess)
        {
            return CreateConnection(dbAccess, ReaderIsolation);
        }
        
        public async Task<T> ExecuteReaderAsync<T>(Func<IDatabaseRawAccessor, Task<T>> dbAccess)
        {
            return await CreateConnectionAsync(dbAccess, ReaderIsolation);
        }

        public T ExecuteWriter<T>(Func<IDatabaseRawAccessor, T> dbAccess)
        {
            return CreateConnection(dbAccess, WriterIsolation);
        }
    }
}