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
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.ExternalSources.Resources.Types;
using PTV.Framework;

namespace PTV.Database.DataAccess
{
    internal class MigrationTools
    {
        public static MigrationTools Instance { get; private set; }

        private readonly IServiceProvider serviceProvider;

        public MigrationTools(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            Instance = this;
        }

        public string GenerateSqlDefaultSeedForType<TEntity>(string code)
        {
            var contextInfo = EntityFrameworkEntityTools.DataContextInfo;
            var tableName = contextInfo.EntityTableName[typeof(TEntity)];
            var schema = contextInfo.Schema;
            var id = code.GetGuid<TEntity>();
            var query = $@"INSERT INTO {schema}.""{tableName}"" (""Id"",""Code"",""Created"",""Modified"") VALUES ('{id}','{code}','0001-01-01 01:01:01','0001-01-01 01:01:01');";
            return query;
        }
    }
}
