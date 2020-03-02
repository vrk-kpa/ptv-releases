/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using PTV.Database.DataAccess.Interfaces.DbContext;
using System;
using PTV.ToolUtilities;

namespace PTV.Database.DataAccess.Tests.Services
{
    public class TestContextManager : IContextManager
    {
        private IUnitOfWork unitOfWork;
        private IUnitOfWorkWritable unitOfWorkWritable;

        public TestContextManager(IUnitOfWork unitOfWork, IUnitOfWorkWritable unitOfWorkWritable)
        {
            this.unitOfWork = unitOfWork;
            this.unitOfWorkWritable = unitOfWorkWritable;
        }

        public void ExecuteReader(Action<IUnitOfWork> action)
        {
            action(unitOfWork);
        }

        public void ExecuteWriter(Action<IUnitOfWorkWritable> action)
        {
            action(unitOfWorkWritable);
        }

        public TResult ExecuteReader<TResult>(Func<IUnitOfWork, TResult> action)
        {
            return action(unitOfWork);
        }

        public TResult ExecuteWriter<TResult>(Func<IUnitOfWorkWritable, TResult> action)
        {
            return action(unitOfWorkWritable);
        }

        public HealthCheckResult VerifyDatabaseConnection()
        {
            return HealthCheckResult.NotTested;
        }

        public TResult ExecuteIsolatedReader<TResult>(Func<IUnitOfWork, TResult> action)
        {
            return action(unitOfWork);
        }

        public void ExecuteIsolatedReader(Action<IUnitOfWork> action)
        {
            action(unitOfWork);
        }

        public void ExecuteTransactionlessReader(Action<IUnitOfWork> action)
        {
            action(unitOfWork);
        }

        public TResult ExecuteTransactionlessReader<TResult>(Func<IUnitOfWork, TResult> action)
        {
            return action(unitOfWork);
        }

        public void ExecuteTransactionlessWriter(Action<IUnitOfWorkWritable> action)
        {
            action(unitOfWorkWritable);
        }

        public TResult ExecuteTransactionlessWriter<TResult>(Func<IUnitOfWorkWritable, TResult> action)
        {
            return action(unitOfWorkWritable);
        }

        public void ExecuteTransactionless(Action<IUnitOfWork> action)
        {
            action(unitOfWork);
        }

        public TResult ExecuteTransactionless<TResult>(Func<IUnitOfWork, TResult> action)
        {
            return action(unitOfWork);
        }
    }
}
