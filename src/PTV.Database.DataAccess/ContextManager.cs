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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

namespace PTV.Database.DataAccess
{
    public class DataContextOptions
    {
        public string RetriesOnDeadlock { get; set; }
    }


    /// <summary>
    /// Context Manager provides set of funcionality for working with DB Context in scope /reader/writer/
    /// </summary>
    [RegisterService(typeof(IContextManager), RegisterType.Transient)]
    internal class ContextManager : IContextManager
    {
        // Prefered type of isolation level of transaction for Read operations
        const IsolationLevel ReaderIsolationLevel = IsolationLevel.ReadCommitted;

        // Prefered type of isolation level of transaction for Write operations
        const IsolationLevel WriterIsolationLevel = IsolationLevel.Serializable;

        private readonly int RetriesOnDeadlock = 3;

        const string PostgreDeadLockCode = "40P01";
        const string PostgreSerializationFailure = "40001";

        private readonly ILogger logger;
        private bool contextInvoked = false;
        private readonly IServiceProvider serviceProvider;
        private readonly IUserIdentification userIdentification;

        public ContextManager(ILoggerFactory loggerFactory, IServiceProvider serviceProvider, IUserIdentification userIdentification, IOptions<DataContextOptions> dataContextOptions)
        {
            this.serviceProvider = serviceProvider;
            this.userIdentification = userIdentification;
            this.logger = loggerFactory.CreateLogger(this.GetType().Name);
            var retriesOnDeadlockSetting = dataContextOptions.Value?.RetriesOnDeadlock;
            if (retriesOnDeadlockSetting == null)
            {
                RetriesOnDeadlock = 0;
            }
            else
            {
                var deadlockRetriesSetting = retriesOnDeadlockSetting?.ParseToInt();
                if (deadlockRetriesSetting != null)
                {
                    RetriesOnDeadlock = deadlockRetriesSetting.Value;
                }
            }
        }

//
//        public void ExecuteTransactionlessReader(Action<IUnitOfWork> action)
//        {
//            Perform<IUnitOfWork>(null, action);
//        }
//
//        public TResult ExecuteTransactionlessReader<TResult>(Func<IUnitOfWork, TResult> action)
//        {
//            TResult result = default(TResult);
//            Perform<IUnitOfWork>(null, (unitOfWork) =>
//            {
//                result = action(unitOfWork);
//            });
//            return result;
//        }
//
//        public void ExecuteTransactionlessWriter(Action<IUnitOfWorkWritable> action)
//        {
//            Perform(null, action);
//        }
//
//        public TResult ExecuteTransactionlessWriter<TResult>(Func<IUnitOfWorkWritable, TResult> action)
//        {
//            TResult result = default(TResult);
//            Perform<IUnitOfWorkWritable>(null, (unitOfWork) =>
//            {
//                result = action(unitOfWork);
//            });
//            return result;
//        }

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
            var dbException = e as DbUpdateException;
            if (dbException?.InnerException == null) return false;
            var postgresException = dbException.InnerException as PostgresException;
            switch (postgresException?.SqlState?.ToUpper())
            {
                case PostgreSerializationFailure: return true;
                case PostgreDeadLockCode: return true;
                default: return false;
            }
        }

        private void CloseContext(PtvDbContext context)
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
                logger.LogError(CoreMessages.RollbackFailed+Environment.NewLine+e.Message);
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
        private void Perform<TUnitOfWork>(IsolationLevel? isolationLevel, Action<TUnitOfWork> action)
        {
            lock (this)
            {
                if (contextInvoked)
                {
                    throw new Exception(CoreMessages.DbContextInUseAlready);
                }
                contextInvoked = true;
                var userName = userIdentification.UserName;
                try
                {
                    int operationRetries = 0;
                    while (true)
                    {
                        using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                        {
                            PtvDbContext dbContext = serviceScope.ServiceProvider.GetService<PtvDbContext>();
                            TUnitOfWork unitOfWorkWritable = serviceScope.ServiceProvider.GetService<TUnitOfWork>();
                            IThreadUserInterface threadUserInterface = serviceScope.ServiceProvider.GetService<IUserIdentification>() as IThreadUserInterface;
                            threadUserInterface?.SetUserName(userName);
                            IDbContextTransaction transaction = isolationLevel != null ? dbContext.Database.BeginTransaction(isolationLevel.Value) : null;
                            try
                            {
                                logger.LogDebug($"DBContext called, thread ID {Thread.CurrentThread.ManagedThreadId}");
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
                            catch (Exception e)
                            {
                                RollBackTransaction(transaction);
                                var ioException = e as IOException;
                                if (HasDeadLockOccured(e) || (ioException != null))
                                {
                                    if (operationRetries++ < RetriesOnDeadlock)
                                    {
                                        CloseContext(dbContext);
                                        logger.LogInformation(e.Message + Environment.NewLine + "Repeating DBContext operation...");
                                        Random randomGen = new Random(Thread.CurrentThread.ManagedThreadId);
                                        Thread.Sleep((randomGen.Next() % 500) + 10); // random wait
                                        continue;
                                    }
                                }
                                if (e is PtvAppException)
                                {
                                    throw;
                                }
                                logger.LogError(CoreExtensions.ExtractAllInnerExceptions(e));
                                throw;
                            }
                            CloseContext(dbContext);
                            break;
                        }
                    }
                }
                finally
                {
                    contextInvoked = false;
                }
            }
        }
    }
}
