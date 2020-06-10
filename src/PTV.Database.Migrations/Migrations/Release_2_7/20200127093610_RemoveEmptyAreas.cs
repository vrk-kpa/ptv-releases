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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;

namespace PTV.Database.Migrations.Migrations.Release_2_7
{
    public partial class RemoveEmptyAreas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            this.AddMigrationAction(serviceProvider =>
            {
                var contextManager = serviceProvider.GetService<IContextManager>();
                var typesCache = serviceProvider.GetService<ITypesCache>();
                contextManager.ExecuteWriter(unitOfWork => ChangeAreaInformationType(unitOfWork, typesCache));
            });
        }

        private void ChangeAreaInformationType(IUnitOfWorkWritable unitOfWork, ITypesCache typesCache)
        {
            Console.WriteLine($"{DateTime.UtcNow} Removing empty area types from web page channels.");
            var repo = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var webPageTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.WebPage.ToString());
            var limitedAreaTypeId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString());

            var toChange = repo.All()
                .Include(x => x.Areas)
                .Include(x => x.AreaMunicipalities)
                .Where(x => x.TypeId == webPageTypeId && x.AreaInformationTypeId == limitedAreaTypeId)
                .Where(x => x.Areas == null || !x.Areas.Any())
                .Where(x => x.AreaMunicipalities == null || !x.AreaMunicipalities.Any())
                .ToList();
            
            Console.WriteLine($"{DateTime.UtcNow} Changing {toChange.Count} web page channels.");

            foreach (var channel in toChange)
            {
                channel.AreaInformationTypeId = null;
            }

            unitOfWork.Save(SaveMode.AllowAnonymous, PreSaveAction.DoNotSetAudits);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
