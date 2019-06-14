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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Npgsql;
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Framework;
using PTV.Framework.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PTV.Framework.ServiceManager;
using PTV.ToolUtilities;
using PTV.Framework.Logging;
// ReSharper disable EmptyGeneralCatchClause

namespace PTV.Database.DataAccess
{
    [RegisterService(typeof(IContextManager), RegisterType.Transient)]
    internal class ContextManager : ContextManagerBase<PtvDbContext>
    {
        public ContextManager(ILoggerFactory loggerFactory, IServiceProvider serviceProvider, IOptions<DataContextOptions> dataContextOptions) : base(loggerFactory, serviceProvider, dataContextOptions, false)
        {
        }
    }
    
    
    /// <summary>
    /// Context Manager provides set of functionality for working with DB Context in scope /reader/writer/
    /// </summary>
    public class ContextManagerBase<TDbContext> : IContextManager where TDbContext : DbContext
    {
        // Prefered type of isolation level of transaction for Read operations
        const IsolationLevel ReaderIsolationLevel = IsolationLevel.ReadCommitted;

        // Prefered type of isolation level of transaction for Write operations
        const IsolationLevel WriterIsolationLevel = IsolationLevel.Serializable;


        const IsolationLevel TesterIsolationLevel = IsolationLevel.ReadUncommitted;

        private readonly int retriesOnTemporaryFail = 3;

        const string PostgreDeadLockCode = "40P01";
        const string PostgreSerializationFailure = "40001";
        const string PostgreTooManyConnections = "53300";
        const string PostgreOutOfMemory = "53200";
        const string PostgreDiskIsFull = "53100";
        const string PostgreInsufficientResources = "53000";

        readonly string[] temporaryPostgreErrors = { PostgreTooManyConnections, PostgreOutOfMemory, PostgreInsufficientResources };
        readonly string[] permanentPostgreErrors = { PostgreDiskIsFull };

        private readonly ILogger logger;
        private bool contextInvoked = false;
        private readonly IServiceProvider serviceProvider;
        private readonly bool nonUserContext;
        private readonly bool logContextCalls;

        protected ContextManagerBase(ILoggerFactory loggerFactory, IServiceProvider serviceProvider, IOptions<DataContextOptions> dataContextOptions, bool nonUserContext = false)
        {
            this.nonUserContext = nonUserContext;
            this.serviceProvider = serviceProvider;
            this.logger = loggerFactory.CreateLogger(this.GetType().Name);
            var retriesOnDeadlockSetting = dataContextOptions.Value?.RetriesOnDeadlock;
            logContextCalls = dataContextOptions.Value?.LogContextCalls == true;
            if (retriesOnDeadlockSetting == null)
            {
                retriesOnTemporaryFail = 0;
            }
            else
            {
                var deadlockRetriesSetting = retriesOnDeadlockSetting?.ParseToInt();
                if (deadlockRetriesSetting != null)
                {
                    retriesOnTemporaryFail = deadlockRetriesSetting.Value;
                }
            }
        }


        public HealthCheckResult VerifyDatabaseConnection()
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                TDbContext dbContext = serviceScope.ServiceProvider.GetService<TDbContext>();
                IDbContextTransaction transaction = null;
                try
                {
                    dbContext.Database.OpenConnection();
                    transaction = dbContext.Database.BeginTransaction(TesterIsolationLevel);
                    var testResult = dbContext.Database.ExecuteSqlCommand("SELECT 1;");
                    if (testResult < -1 || testResult > 0)
                    {
                        throw new Exception("Database ping command failed!");
                    }
                    return HealthCheckResult.Ok;
                }
                catch (Exception) { }
                finally
                {
                    RollBackTransaction(transaction);
                    CloseContext(dbContext);
                }
            }
            return HealthCheckResult.Failed;
        }

        /// <summary>
        /// Creates new unit of work for reading operation
        /// </summary>
        /// <param name="action">Set of operations which will be performed on context</param>
        public void ExecuteReader(Action<IUnitOfWork> action)
        {
            Perform<IUnitOfWork>(ReaderIsolationLevel, action);
        }

        /// <summary>
        /// Creates new unit of work for writing operation
        /// </summary>
        /// <param name="action">Set of operations which will be performed on context</param>
        public void ExecuteWriter(Action<IUnitOfWorkWritable> action)
        {
            Perform<IUnitOfWorkWritable>(WriterIsolationLevel, action);
        }


        /// <summary>
        /// Creates new unit of work for reading operation, user http context is not used
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public TResult ExecuteIsolatedReader<TResult>(Func<IUnitOfWork, TResult> action)
        {
            TResult result = default(TResult);
            Perform<IUnitOfWork>(ReaderIsolationLevel, (unitOfWork) =>
            {
                result = action(unitOfWork);
            }, true);
            return result;
        }

        /// <summary>
        /// Creates new unit of work for reading operation, user http context is not used
        /// </summary>
        /// <param name="action"></param>
        public void ExecuteIsolatedReader(Action<IUnitOfWork> action)
        {
            Perform<IUnitOfWork>(ReaderIsolationLevel, action);
        }

        public TResult ExecuteReader<TResult>(Func<IUnitOfWork, TResult> action)
        {
            TResult result = default(TResult);
            Perform<IUnitOfWork>(ReaderIsolationLevel, (unitOfWork) =>
            {
                result = action(unitOfWork);
            });
            return result;
        }

        public TResult ExecuteWriter<TResult>(Func<IUnitOfWorkWritable, TResult> action)
        {
            TResult result = default(TResult);
            Perform<IUnitOfWorkWritable>(WriterIsolationLevel, (unitOfWork) =>
            {
                result = action(unitOfWork);
            });
            return result;
        }

        private bool HasDeadLockOccured(Exception e)
        {
            if (e is DbUpdateConcurrencyException || e?.InnerException is DbUpdateConcurrencyException) return true;
            switch (GetPostgreErrorCode( e))
            {
                case PostgreSerializationFailure: return true;
                case PostgreDeadLockCode: return true;
                default: return false;
            }
        }

        private T IterateSpecificException<T>(Exception e) where T : Exception
        {
            if (e == null)
            {
                return null;
            }
            if (e is T result)
            {
                return result;
            }

            return IterateSpecificException<T>(e.InnerException);
        }

        private string GetPostgreErrorCode(Exception e)
        {
            return (IterateSpecificException<PostgresException>(e))?.SqlState?.ToUpper();
        }

        private bool HasTemporaryOccured(Exception e)
        {
            var postgreErrorCode = GetPostgreErrorCode(e);
            bool isTemporary = !string.IsNullOrEmpty(postgreErrorCode) && temporaryPostgreErrors.Any(i => i == postgreErrorCode);
            var npgsqlException = IterateSpecificException<NpgsqlException>(e);
            var postgresException = IterateSpecificException<PostgresException>(e);
            isTemporary |= npgsqlException?.IsTransient ?? false;
            isTemporary |= postgresException?.IsTransient ?? false;
            isTemporary |= (((npgsqlException)?.InnerException as IOException)?.InnerException as SocketException) != null;
            return isTemporary;
        }

        private void CloseContext(TDbContext context)
        {
            if (context == null) return;
            try { context.Database.CloseConnection(); } catch { }
            try { context.Dispose(); } catch {}
        }

        private void RollBackTransaction(IDbContextTransaction transaction)
        {
            try
            {
                transaction?.Rollback();
            }
            catch (Exception e )
            {
                logger.LogDebug(CoreMessages.RollbackFailed+Environment.NewLine+e.Message);
            }
            try
            {
                transaction?.Dispose();
            }
            catch {}
        }

        private void CommitTransaction(IDbContextTransaction transaction)
        {
            try
            {
                transaction?.Commit();
            }
            catch (Exception e)
            {
                logger.LogError(CoreMessages.CommitFailed + Environment.NewLine + e.Message);
            }
            try
            {
                transaction?.Dispose();
            }
            catch { }
        }

        /// <summary>
        /// Invoke operation on context in transaction according to selected unit of work
        /// </summary>
        /// <param name="isolationLevel">Desired isolation level of transaction</param>
        /// <param name="action">Set of operations invoked on DB Context</param>
        /// <param name="nonUserLocalContext">Do not copy http context related stuff</param>
        private void Perform<TUnitOfWork>(IsolationLevel? isolationLevel, Action<TUnitOfWork> action, bool nonUserLocalContext = false)
        {
            lock (this)
            {
                if (contextInvoked)
                {
                    throw new Exception(CoreMessages.DbContextInUseAlready);
                }
                contextInvoked = true;
                Stopwatch watchTime = null;
                string methodName = null;
                int threadId = 0;
                if (logContextCalls)
                {
                    watchTime = new Stopwatch();
                    methodName = string.Join('/', new StackTrace().GetFrames().Skip(1).Take(3).Select(i => i?.GetMethod()).Where(i => i != null).Select(i => $"{i.DeclaringType?.Name ?? "unknown"}.{i.Name}").Reverse());
                    threadId = Thread.CurrentThread.ManagedThreadId;
                    logger.LogInformation($"*** DB context operation '{methodName}' thread ID {threadId} started.");
                }
                try
                {
                    watchTime.SafeCall(w => w.Start());
                    int operationRetries = 0;
                    while (true)
                    {
                        var contextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                        using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                        {
                            TDbContext dbContext = serviceScope.ServiceProvider.GetService<TDbContext>();
                            TUnitOfWork unitOfWorkWritable = serviceScope.ServiceProvider.GetService<TUnitOfWork>();
                            ((IUnitOfWorkContextInitializer)unitOfWorkWritable).InitContext(dbContext);
                            if (!nonUserLocalContext && !this.nonUserContext)
                            {
                                IThreadUserInterface threadUserInterface = serviceScope.ServiceProvider.GetService<IUserIdentification>() as IThreadUserInterface;
                                threadUserInterface?.CopyBearerToken(contextAccessor);
                            }
                            var performanceMonitor = serviceScope.ServiceProvider.CopyPerformanceMonitoring(serviceProvider);
                            performanceMonitor.AddIgnoredClass(this.GetType());
                            performanceMonitor.AddIgnoredClass(this.GetType().BaseType);
                            Guid measuringId = performanceMonitor.StartMeasuring();
                            IDbContextTransaction transaction = isolationLevel != null ? dbContext.Database.BeginTransaction(isolationLevel.Value) : null;
                            bool operationFailedWithException = false;
                            try
                            {
                                dbContext.Database.OpenConnection();
                                action(unitOfWorkWritable);
                                if (isolationLevel == WriterIsolationLevel)
                                {
                                    CommitTransaction(transaction);
                                }
                                else
                                {
                                    RollBackTransaction(transaction);
                                }
                            }
                            catch (AggregateException e) when (e.InnerException is TaskCanceledException)
                            {
                                throw new PtvActionCancelledException();
                            }
                            catch (AggregateException e) when (e.InnerException is PtvActionCancelledException)
                            {
                                throw e.InnerException;
                            }
                            catch (TaskCanceledException)
                            {
                                throw new PtvActionCancelledException();
                            }
                            catch (PtvActionCancelledException)
                            {
                                throw;
                            }
                            catch (Exception e)
                            {
                                operationFailedWithException = true;
                                RollBackTransaction(transaction);

                                PrintDbException(e);
                                var isDeadlock = HasDeadLockOccured(e);
                                var isTemporaryIssue = HasTemporaryOccured(e);
                                
                                if (isDeadlock || (e is IOException) || isTemporaryIssue)
                                {
                                    if (operationRetries++ < retriesOnTemporaryFail)
                                    {
                                        CloseContext(dbContext);
                                        performanceMonitor.StopMeasuring(measuringId, $" (DB - TERMINATED - {(isDeadlock ? "DEADLOCK" : isTemporaryIssue ? "TRANSIENT ISSUE" : e.Message)})");
                                        logger.LogWarning(DatabaseLogEvents.Retry, e, $"{e.Message} Repeating DBContext operation.");

                                        Random randomGen = new Random(Thread.CurrentThread.ManagedThreadId);
                                        Thread.Sleep((randomGen.Next()%500) + 10); // random wait
                                        continue;
                                    }
                                    else
                                    {
                                        var errorCode = GetPostgreErrorCode(e);
                                        if (errorCode == PostgreTooManyConnections)
                                        {
                                            throw new PtvDbTooManyConnectionsException();
                                        }
                                    }
                                }

                                if (e is PtvAppException)
                                {
                                    throw;
                                }

                                throw;
                            }
                            finally
                            {
                                CloseContext(dbContext);
                                performanceMonitor.StopMeasuring(measuringId, operationFailedWithException ? " (DB - FAILED WITH EXCEPTION)" : " (DB Context)");
                            }
                            break;
                        }
                    }
                }
                finally
                {
                    contextInvoked = false;
                    if (logContextCalls && watchTime != null)
                    {
                        watchTime.Stop();
                        logger.LogInformation($"*** DB context operation '{methodName}' thread ID {threadId} done in {watchTime.ElapsedMilliseconds}ms");
                    }
                }
            }
        }

        private void PrintDbException(Exception exception)
        {
            if (exception == null)
            {
                return;
            }

            var exceptionType = exception.GetType().Name ?? string.Empty;
            var innerType = exception.InnerException?.GetType()?.Name ?? string.Empty;
            var postgreException = exception.InnerException as PostgresException;
            var code = (exception as PostgresException)?.SqlState;

            if (string.IsNullOrEmpty(code))
            {
                code = postgreException?.SqlState ?? string.Empty;
            }

            var message = (exception.Message ?? string.Empty) + Environment.NewLine + (exception.InnerException?.Message ?? string.Empty);

            var details = postgreException == null ? string.Empty : $"{Environment.NewLine}Details:{Environment.NewLine} - Message:{postgreException.Message}{Environment.NewLine}" +
                          $" - Column:{postgreException.ColumnName ?? string.Empty}{Environment.NewLine}" +
                          $" - Constraint:{postgreException.ConstraintName ?? string.Empty}{Environment.NewLine}" +
                          $" - Detail:{postgreException.Detail ?? string.Empty}{Environment.NewLine}";

            string logMsgInnerType = string.IsNullOrWhiteSpace(innerType) ? string.Empty : $" with inner type '{innerType}'";
            string logMsgCode = string.IsNullOrWhiteSpace(code) ? string.Empty : $" and Code is '{code}'";

            logger.LogError(DatabaseLogEvents.Error, exception, $"DB exception occured. Type '{exceptionType}'{logMsgInnerType}{logMsgCode}. Message:'{message}'{details}");
        }
    }
}
