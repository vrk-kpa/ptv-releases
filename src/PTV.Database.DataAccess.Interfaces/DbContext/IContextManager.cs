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
using PTV.Framework;
using PTV.ToolUtilities;

namespace PTV.Database.DataAccess.Interfaces.DbContext
{
    /// <summary>
    /// Context Manager provides set of funcionality for working with DB Context in scope /reader/writer/
    /// </summary>
    public interface IContextManager
    {
        /// <summary>
        /// Creates new unit of work for reading operation
        /// </summary>
        /// <param name="action">Set of operations which will be performed on context</param>
        void ExecuteReader(Action<IUnitOfWork> action);

        /// <summary>
        /// Creates new unit of work for writing operation
        /// </summary>
        /// <param name="action">Set of operations which will be performed on context</param>
        void ExecuteWriter(Action<IUnitOfWorkWritable> action);

        /// <summary>
        /// Creates new unit of work for reading operation
        /// </summary>
        /// <param name="action">Set of operations which will be performed on context</param>
        TResult ExecuteReader<TResult>(Func<IUnitOfWork, TResult> action);

        /// <summary>
        /// Creates new unit of work for writing operation
        /// </summary>
        /// <param name="action">Set of operations which will be performed on context</param>
        TResult ExecuteWriter<TResult>(Func<IUnitOfWorkWritable, TResult> action);

        /// <summary>
        /// Verify connection to database
        /// </summary>
        /// <returns></returns>
        HealthCheckResult VerifyDatabaseConnection();

        /// <summary>
        /// Creates new unit of work for reading operation, user http context is not used
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        TResult ExecuteIsolatedReader<TResult>(Func<IUnitOfWork, TResult> action);

        /// <summary>
        /// Creates new unit of work for reading operation, user http context is not used
        /// </summary>
        /// <param name="action"></param>
        void ExecuteIsolatedReader(Action<IUnitOfWork> action);
    }
}
