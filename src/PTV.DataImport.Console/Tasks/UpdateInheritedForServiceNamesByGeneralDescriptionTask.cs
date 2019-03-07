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
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PTV.Framework;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Enums;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    [RegisterService(typeof(UpdateInheritedForServiceNamesByGeneralDescriptionTask), RegisterType.Transient)]
    public class UpdateInheritedForServiceNamesByGeneralDescriptionTask
    {
        private IServiceProvider _serviceProvider;
        private ILogger _logger;

        public UpdateInheritedForServiceNamesByGeneralDescriptionTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<CreateServiceDataForGeneralDescriptionsJsonTask>();

            _logger.LogDebug("UpdateInheritedForServiceNamesByGeneralDescriptionTask .ctor");
        }

        public void UpdateData()
        {
            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    Console.WriteLine("Update inherited field of service names starting...");
                    var updatedItemCount = 0;
                    var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    var gdRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                    var psPublishedType = unitOfWork.CreateRepository<IPublishingStatusTypeRepository>().All().FirstOrDefault(m => m.Code == PublishingStatus.Published.ToString());

                    var services = unitOfWork.ApplyIncludes(serviceRep.All().Where(x => x.StatutoryServiceGeneralDescriptionId != null), i => i.Include(x => x.ServiceNames)).ToList();
                    var generalDescriptions = unitOfWork.ApplyIncludes(gdRep.All(), i => i.Include(x => x.Names).Include(x => x.PublishingStatus).Include(x => x.LanguageAvailabilities)).ToList();

                    foreach (var service in services)
                    {
                        var gd = generalDescriptions.Where(x => x.Id == service.StatutoryServiceGeneralDescriptionId)
                            .OrderBy(x => x.PublishingStatus.PriorityFallback).FirstOrDefault();
                        
                        if (gd != null)
                        {

                            var publishedLanguagesIds = gd.LanguageAvailabilities.Where(x => x.StatusId == psPublishedType?.Id)
                                .Select(x => x.LanguageId).ToList();

                            gd.Names = gd.Names.Where(x => publishedLanguagesIds.Contains(x.LocalizationId)).ToList();

                            service.ServiceNames.ForEach(sn =>
                            {
                                sn.Inherited = gd.Names.Any(y => y.LocalizationId == sn.LocalizationId && y.Name?.Trim() == sn.Name?.Trim());
                                if (sn.Inherited)
                                {
                                    updatedItemCount++;
                                }
                            });
                        }
                    }

                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                    Console.WriteLine($@"Update inherited field of service names is done. Result - updated {updatedItemCount} names.");
                    Console.WriteLine("Done.");
                });
            }
        }
    }
}
