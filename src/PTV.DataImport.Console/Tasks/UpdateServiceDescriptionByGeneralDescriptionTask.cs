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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Cloud;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.DataImport.Console.Tasks
{
    public class UpdateServiceDescriptionByGeneralDescriptionTask
    {
        private readonly IServiceProvider serviceProvider;
        private const string DefaultCreatedBy = "UpdateServiceDescriptionByGeneralDescriptionTask";

        public UpdateServiceDescriptionByGeneralDescriptionTask(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            var logger = this.serviceProvider.GetService<ILoggerFactory>().CreateLogger<UpdateServiceDescriptionByGeneralDescriptionTask>();
            logger.LogDebug("UpdateBackgroundGeneralDescriptionTask .ctor");
        }

        public void CheckAndUpdateServiceDescriptionByGeneralDescription()
        {
            var defaultLanguageCode = DomainConstants.DefaultLanguage;

            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                var commonService = serviceScope.ServiceProvider.GetService<ICommonService>();
                var textManager = serviceScope.ServiceProvider.GetService<ITextManager>();
                var storage = serviceScope.ServiceProvider.GetService<IStorageService>();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    System.Console.WriteLine("UPDATE and ADD service description by general description data is running...");

                    var serviceVersionedRepository = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    var statutoryServiceDescriptionRepository = unitOfWork.CreateRepository<IStatutoryServiceDescriptionRepository>();
                    var descriptionTypeCode = DescriptionTypeEnum.Description.ToString();
                    var backgroundTypeCode = DescriptionTypeEnum.BackgroundDescription.ToString();
                    var descriptionTypeId = commonService.GetDescriptionTypeId(DescriptionTypeEnum.Description.ToString());
                    var backgroundDescriptionTypeId = commonService.GetDescriptionTypeId(DescriptionTypeEnum.BackgroundDescription.ToString());
                    var updatedOrAddedServiceVersionsIds = new HashSet<Guid>();
                    var updatedGeneralDescriptionsIds = new HashSet<Guid>();
                    var updateCounter = 0;
                    var addNewCounter = 0;

                    var servicesWithGeneralDescriptionAndEmptyDescription = serviceVersionedRepository
                                .All()
                                .Where(x => x.StatutoryServiceGeneralDescriptionId != null
                                && (x.ServiceDescriptions.Any(y => y.TypeId == descriptionTypeId) || !x.ServiceDescriptions.Any())
                        );

                    var services = unitOfWork.ApplyIncludes(servicesWithGeneralDescriptionAndEmptyDescription, q => q
                         .Include(i => i.StatutoryServiceGeneralDescription).ThenInclude(i => i.Versions).ThenInclude(i => i.PublishingStatus)
                         .Include(i => i.StatutoryServiceGeneralDescription).ThenInclude(i => i.Versions).ThenInclude(i => i.Descriptions)
                         .Include(i => i.StatutoryServiceGeneralDescription).ThenInclude(i => i.Versions).ThenInclude(i => i.Descriptions).ThenInclude(i => i.Type)
                         .Include(i => i.ServiceDescriptions)
                         .Include(i => i.LanguageAvailabilities)
                       ).ToList();

                    foreach (var service in services)
                    {
                        var statutoryServiceGeneralDescriptionVersionedPublished = GetStatutoryServiceGeneralDescriptionPublished(service.StatutoryServiceGeneralDescription.Versions);
                        var statutoryServiceDescriptions = statutoryServiceGeneralDescriptionVersionedPublished?.Descriptions
                                                            .Where(y => y.Type.Code == descriptionTypeCode
                                                                     || y.Type.Code == backgroundTypeCode
                                                                  )
                                                            .ToList();

                        if (statutoryServiceDescriptions != null)
                        {
                            if (service.ServiceDescriptions.Any())
                            {
                                foreach (var serviceLanguage in service.LanguageAvailabilities)
                                {
                                    // Add service description which not exist for some languages
                                    if (!service.ServiceDescriptions.Any(x => x.TypeId == descriptionTypeId && x.LocalizationId == serviceLanguage.LanguageId))
                                    {
                                        var newDescription = GetLocalizedStatutoryServiceDescription(statutoryServiceDescriptions, serviceLanguage.LanguageId, commonService.GetLocalizationId(defaultLanguageCode), textManager);
                                        if (!String.IsNullOrWhiteSpace(newDescription))
                                        {
                                            service.ServiceDescriptions.Add(GetNewServiceDescription(newDescription, descriptionTypeId, service.Id, serviceLanguage.LanguageId));

                                            updatedOrAddedServiceVersionsIds.Add(service.Id);
                                            addNewCounter++;
                                        }

                                    }
                                    //Update service description which exist and are empty
                                    else
                                    {
                                        service.ServiceDescriptions
                                            .Where(x => x.TypeId == descriptionTypeId
                                                            && (string.IsNullOrWhiteSpace(x.Description)
                                                                    || ((!string.IsNullOrWhiteSpace(x.Description) && string.IsNullOrEmpty(textManager.ConvertToPureText(x.Description))))))
                                            .ForEach(serviceDescription =>
                                            {
                                                var copiedDescription = GetLocalizedStatutoryServiceDescription(statutoryServiceDescriptions, serviceDescription.LocalizationId, commonService.GetLocalizationId(defaultLanguageCode), textManager);
                                                if (!String.IsNullOrWhiteSpace(copiedDescription))
                                                {
                                                    serviceDescription.Description = copiedDescription;

                                                    updatedOrAddedServiceVersionsIds.Add(service.Id);
                                                    updateCounter++;
                                                }
                                            });
                                    }
                                }
                            }
                            else //Add service description which not exist
                            {
                                foreach (var serviceLanguage in service.LanguageAvailabilities)
                                {
                                    var newDescription = GetLocalizedStatutoryServiceDescription(statutoryServiceDescriptions, serviceLanguage.LanguageId, commonService.GetLocalizationId(defaultLanguageCode), textManager);
                                    if (!String.IsNullOrWhiteSpace(newDescription))
                                    {
                                        service.ServiceDescriptions.Add(GetNewServiceDescription(newDescription, descriptionTypeId, service.Id, serviceLanguage.LanguageId));

                                        updatedOrAddedServiceVersionsIds.Add(service.Id);
                                        addNewCounter++;
                                    }
                                }
                            }

                        }
                    }
                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                    if (updatedOrAddedServiceVersionsIds.Any())
                    {
                        var content = String.Join("\n",
                            updatedOrAddedServiceVersionsIds.Select(id => id.ToString()).ToArray());
                        using (var client = storage.GetClient())
                        {
                            client.SaveFile(nameof(UpdateServiceDescriptionByGeneralDescriptionTask),
                                "MigrationData_UpdateAndNewServiceDescription_ServiceVersionedIds.txt", content);
                        }
                    }

                    System.Console.WriteLine($"ADDED: '{addNewCounter}' service descriptions.");
                    System.Console.WriteLine($"UPDATED: '{updateCounter}' service descriptions.");
                    System.Console.WriteLine("UPDATE and ADD service description by general description data is done.");


                    //Update GD description -> GD background
                    System.Console.WriteLine("General Description UPDATE - description -> background is running...");
                    var excludedExistingBackgroundDescriptionIds = statutoryServiceDescriptionRepository.All()
                                                                        .Where(x => x.TypeId == backgroundDescriptionTypeId)
                                                                        .Select(x => x.StatutoryServiceGeneralDescriptionVersionedId)
                                                                        .ToList();

                    var generalDescriptions = statutoryServiceDescriptionRepository.All()
                                .Where(x => x.TypeId == descriptionTypeId
                                && !excludedExistingBackgroundDescriptionIds.Contains(x.StatutoryServiceGeneralDescriptionVersionedId)).ToList();

                    generalDescriptions.ForEach(gd =>
                    {
                        statutoryServiceDescriptionRepository.Remove(gd);
                        statutoryServiceDescriptionRepository.Add(new StatutoryServiceDescription
                        {
                                                                    Description = gd.Description,
                                                                    StatutoryServiceGeneralDescriptionVersionedId = gd.StatutoryServiceGeneralDescriptionVersionedId,
                                                                    LocalizationId = gd.LocalizationId,
                                                                    TypeId = backgroundDescriptionTypeId,
                                                                    Created = DateTime.UtcNow,
                                                                    CreatedBy = UpdateServiceDescriptionByGeneralDescriptionTask.DefaultCreatedBy,
                        });

                        updatedGeneralDescriptionsIds.Add(gd.StatutoryServiceGeneralDescriptionVersionedId);
                    });

                    unitOfWork.Save(SaveMode.NonTrackedDataMigration);
                    if (updatedGeneralDescriptionsIds.Any())
                    {
                        var content = String.Join("\n",
                            updatedGeneralDescriptionsIds.Select(id => id.ToString()).ToArray());

                        using (var client = storage.GetClient())
                        {
                            client.SaveFile(nameof(UpdateServiceDescriptionByGeneralDescriptionTask),
                                "MigrationData_UpdateGeneralDescriptionToBackground_StatutoryServiceDescriptionVersionedIds.txt",
                                content);
                        }
                    }

                    System.Console.WriteLine($"UPDATED: '{generalDescriptions.Count}' statutory service descriptions of description type -> background description type.");
                    System.Console.WriteLine("General Description UPDATE - description -> background update is done.");
                });
            }
        }

        private ServiceDescription GetNewServiceDescription(string description, Guid descriptionTypeId, Guid serviceId, Guid languageId)
        {
            return new ServiceDescription
            {
                Description = description,
                TypeId = descriptionTypeId,
                ServiceVersionedId = serviceId,
                LocalizationId = languageId,
                Created = DateTime.UtcNow,
                CreatedBy = UpdateServiceDescriptionByGeneralDescriptionTask.DefaultCreatedBy,
            };
        }

        private string GetLocalizedStatutoryServiceDescription(List<StatutoryServiceDescription> statutoryServiceDescriptions, Guid localizedId, Guid defaultLocalizedId, ITextManager textManager)
        {
            var descriptionTypeCode = DescriptionTypeEnum.Description.ToString();
            var backgroundTypeCode = DescriptionTypeEnum.BackgroundDescription.ToString();

            return CheckGeneralDescription(statutoryServiceDescriptions?.FirstOrDefault(x => x.LocalizationId == localizedId && x.Type.Code == descriptionTypeCode)?.Description, textManager) ??
                   CheckGeneralDescription(statutoryServiceDescriptions?.FirstOrDefault(x => x.LocalizationId == localizedId && x.Type.Code == backgroundTypeCode)?.Description, textManager) ??
                   CheckGeneralDescription(statutoryServiceDescriptions?.FirstOrDefault(x => x.LocalizationId == defaultLocalizedId && x.Type.Code == descriptionTypeCode)?.Description, textManager) ??
                   CheckGeneralDescription(statutoryServiceDescriptions?.FirstOrDefault(x => x.LocalizationId == defaultLocalizedId && x.Type.Code == backgroundTypeCode)?.Description, textManager);
        }

        private string CheckGeneralDescription(string gdDescription, ITextManager textManager)
        {
            return !string.IsNullOrWhiteSpace(gdDescription) && !string.IsNullOrWhiteSpace(textManager.ConvertToPureText(gdDescription))
                ? gdDescription
                : null;
        }

        private StatutoryServiceGeneralDescriptionVersioned GetStatutoryServiceGeneralDescriptionPublished(IEnumerable<StatutoryServiceGeneralDescriptionVersioned> versions)
        {
            var now = DateTime.UtcNow;
            var published = PublishingStatus.Published.ToString();
            return  versions.FirstOrDefault(i => i.PublishingStatus.Code == published &&
                    ((i.ValidFrom <= now && i.ValidTo >= now) ||
                    (i.ValidFrom == null && i.ValidTo == null)));
        }
    }
}
