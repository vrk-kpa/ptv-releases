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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using PTV.Framework;

namespace PTV.Database.DataAccess.DirectRaw
{
    internal class DatabaseRawAccessor : IDatabaseRawAccessor
    {
        private readonly DatabaseRawContext.DatabaseRawAccessorOptions options;
        private readonly IHttpContextAccessor httpContextAccessor;

        internal DatabaseRawAccessor(IHttpContextAccessor httpContextAccessor, DatabaseRawContext.DatabaseRawAccessorOptions options)
        {
            this.options = options;
            this.httpContextAccessor = httpContextAccessor;
        }
        
        public IList<T> SelectList<T>(string query, object paramObject)
        {
            return this.options.DbConnection.Query<T>(query, paramObject, this.options.DbTransaction).ToList();
        }

        public IList<T> SelectListWithInclude<T, U, V, W, X, Y>(string query, object paramObject,
            Func<T, U, V, W, X, Y, T> connectFunc, string splitOn)
        {
            return this.options.DbConnection.Query(query, connectFunc, paramObject, splitOn: splitOn).Distinct().ToList();
        }
        
        public async Task<List<T>> SelectListAsync<T>(string query, object paramObject)
        {
            return await this.options.DbConnection.QueryAsync<T>(new CommandDefinition(query, paramObject, this.options.DbTransaction, cancellationToken: httpContextAccessor.HttpContext?.RequestAborted ?? new CancellationToken(false))).ContinueWith(i => i.Result.InclusiveToList());
        }

        public async Task<IList<T>> SelectListWithIncludeAsync<T, U, V, W, X, Y>(string query, object paramObject,
            Func<T, U, V, W, X, Y, T> connectFunc, string splitOn)
        {
            return await this.options.DbConnection
                .QueryAsync(new CommandDefinition(query, paramObject, this.options.DbTransaction, cancellationToken: httpContextAccessor.HttpContext?.RequestAborted ?? new CancellationToken(false)), connectFunc, splitOn)
                .ContinueWith(i => i.Result.Distinct().InclusiveToList());
        }

        public T SelectOne<T>(string query, object paramObject)
        {
            return this.options.DbConnection.QueryFirstOrDefault<T>(query, paramObject, this.options.DbTransaction);
        }
        
        public async Task<T> SelectOneAsync<T>(string query, object paramObject)
        {
            return await this.options.DbConnection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(query, paramObject, this.options.DbTransaction, cancellationToken: httpContextAccessor.HttpContext?.RequestAborted ?? new CancellationToken(false)));
        }

        public void Command(string command, object paramObject)
        {
            this.options.DbConnection.Execute(command, paramObject, this.options.DbTransaction);
        }
        
        public async void CommandAsync(string command, object paramObject)
        {
            await this.options.DbConnection.ExecuteAsync(new CommandDefinition(command, paramObject, this.options.DbTransaction, cancellationToken: httpContextAccessor.HttpContext?.RequestAborted ?? new CancellationToken(false)));
        }

        public void Save()
        {
            //this.options.DbTransaction.Save(Guid.NewGuid().ToString()); //TODO test and return 
            this.options.Saved = true;
        }
    }
}
