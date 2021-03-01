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
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;

namespace PTV.Database.Migrations.Migrations.Release_3_0
{
    public partial class SuomiFiRequestFilters : Migration
    {
        private readonly string[] NewIps = new[] {"34.251.90.130", "54.154.222.234", "54.72.0.27"};
        private const int RequestLimit = 64;
        private const string Interface = "openapi";
        private const string AnyUserName = "any";
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            Console.WriteLine($"{DateTime.UtcNow} - Adding new entries to request filter.");
            
            
            this.AddMigrationAction(serviceProvider =>
            {
                var contextManager = serviceProvider.GetService<IContextManager>();
                contextManager.ExecuteWriter(unitOfWork => AddSuomiFiEntries(unitOfWork));
            });

        }

        private void AddSuomiFiEntries(IUnitOfWorkWritable unitOfWork)
        {
            var repo = unitOfWork.CreateRepository<ICFGRequestFilterRepository>();
            var existingIps = repo.All().Select(x => x.IPAddress).Distinct().ToList();

            foreach (var ip in NewIps)
            {
                if (existingIps.Contains(ip))
                {
                    continue;
                }
                
                var record = new CFGRequestFilter
                {
                    Interface = Interface,
                    ConcurrentRequests = RequestLimit,
                    UserName = AnyUserName,
                    IPAddress = ip
                };
                repo.Add(record);
            }

            unitOfWork.Save(SaveMode.AllowAnonymous);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
