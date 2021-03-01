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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.Database.Migrations.Migrations.Release_3_1
{
    public partial class SetMissingInheritedFieldOfServiceNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            this.AddMigrationAction(serviceProvider =>
            {
                var contextManager = serviceProvider.GetService<IContextManager>();

                contextManager.ExecuteWriter(unitOfWork =>
                {
                    Console.WriteLine($"{DateTime.UtcNow} - Update inherited field of service names starting...");
                    var updatedItemCount = 0;
                    var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    var gdRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                    var psPublishedType = unitOfWork.CreateRepository<IPublishingStatusTypeRepository>().All()
                        .FirstOrDefault(m => m.Code == PublishingStatus.Published.ToString());

                    var services = unitOfWork
                        .ApplyIncludes(serviceRep.All().Where(x => x.StatutoryServiceGeneralDescriptionId != null),
                            i => i.Include(x => x.ServiceNames)).ToList();
                    var generalDescriptions = unitOfWork.ApplyIncludes(gdRep.All(),
                        i => i.Include(x => x.Names).Include(x => x.PublishingStatus)
                            .Include(x => x.LanguageAvailabilities)).ToList();

                    foreach (var service in services)
                    {
                        var gd = generalDescriptions
                            .Where(x => x.UnificRootId == service.StatutoryServiceGeneralDescriptionId)
                            .OrderBy(x => x.PublishingStatus.PriorityFallback).FirstOrDefault();

                        if (gd != null)
                        {
                            var publishedLanguagesIds = gd.LanguageAvailabilities
                                .Where(x => x.StatusId == psPublishedType?.Id)
                                .Select(x => x.LanguageId).ToList();

                            gd.Names = gd.Names.Where(x => publishedLanguagesIds.Contains(x.LocalizationId)).ToList();

                            service.ServiceNames.Where(x => !x.Inherited).ForEach(sn =>
                            {
                                var inheritedBefore = sn.Inherited;
                                sn.Inherited = gd.Names.Any(y =>
                                    y.LocalizationId == sn.LocalizationId && y.Name?.Trim() == sn.Name?.Trim());

                                if (!inheritedBefore && sn.Inherited)
                                {
                                    //Console.WriteLine($"ServiceName set Inherited of versionId {sn.ServiceVersionedId}");
                                    updatedItemCount++;
                                }
                            });
                        }
                    }

                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                    Console.WriteLine($"{DateTime.UtcNow} - Update inherited field of service names is done. Result - set Inherited of {updatedItemCount} service names.");
                });
            });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
