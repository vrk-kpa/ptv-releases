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
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace PTV.DataImport.ConsoleApp.Tasks
{
    public class UpdateServiceDescriptionByGeneralDescriptionTask
    {
        private IServiceProvider _serviceProvider;
        private ILogger _logger;
        private const string DefaultCreatedBy = "UpdateServiceDescriptionByGeneralDescriptionTask";

        public UpdateServiceDescriptionByGeneralDescriptionTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _logger = _serviceProvider.GetService<ILoggerFactory>().CreateLogger<UpdateServiceDescriptionByGeneralDescriptionTask>();
            _logger.LogDebug("UpdateBackgroundGeneralDescriptionTask .ctor");
        }

        public void CheckAndUpdateServiceDescriptionByGeneralDescription()
        {
            string defaultlangCode = LanguageCode.fi.ToString();

            using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var scopedCtxMgr = serviceScope.ServiceProvider.GetService<IContextManager>();
                var commonService = serviceScope.ServiceProvider.GetService<ICommonService>();
                var textManager = serviceScope.ServiceProvider.GetService<ITextManager>();

                scopedCtxMgr.ExecuteWriter(unitOfWork =>
                {
                    Console.WriteLine($"UPDATE and ADD service description by general description data is running...");
                    
                    var serviceDescriptionRepository = unitOfWork.CreateRepository<IServiceDescriptionRepository>();
                    var serviceVersionedRepository = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    var statutoryServiceDescriptionRepository = unitOfWork.CreateRepository<IStatutoryServiceDescriptionRepository>();
                    var descriptionTypeCode = DescriptionTypeEnum.Description.ToString();
                    var backgroundTypeCode = DescriptionTypeEnum.BackgroundDescription.ToString();
                    var descriptionTypeId = commonService.GetDescriptionTypeId(DescriptionTypeEnum.Description.ToString());
                    var backgroundDescriptionTypeId = commonService.GetDescriptionTypeId(DescriptionTypeEnum.BackgroundDescription.ToString());
                    var updatedOrAddedServiceVersionsIds = new HashSet<Guid>();
                    var updatedGeneralDescriptionsIds = new HashSet<Guid>();
                    int updateCounter = 0;
                    int addNewCounter = 0;

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
                                                                  );

                        if (statutoryServiceDescriptions != null)
                        {
                            if (service.ServiceDescriptions.Any())
                            {
                                foreach (var servicelanguage in service.LanguageAvailabilities)
                                {
                                    // Add service description which not exist for some languages 
                                    if (!service.ServiceDescriptions.Any(x => x.TypeId == descriptionTypeId && x.LocalizationId == servicelanguage.LanguageId))
                                    {
                                        var newDescription = GetLocalizedStatutoryServiceDescription(statutoryServiceDescriptions, servicelanguage.LanguageId, commonService.GetLocalizationId(defaultlangCode), textManager);
                                        if (!String.IsNullOrWhiteSpace(newDescription))
                                        {
                                            service.ServiceDescriptions.Add(GetNewServiceDescription(newDescription, descriptionTypeId, service.Id, servicelanguage.LanguageId));

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
                                                var copiedDescription = GetLocalizedStatutoryServiceDescription(statutoryServiceDescriptions, serviceDescription.LocalizationId, commonService.GetLocalizationId(defaultlangCode), textManager);
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
                                foreach (var servicelanguage in service.LanguageAvailabilities)
                                {
                                    var newDescription = GetLocalizedStatutoryServiceDescription(statutoryServiceDescriptions, servicelanguage.LanguageId, commonService.GetLocalizationId(defaultlangCode), textManager);
                                    if (!String.IsNullOrWhiteSpace(newDescription))
                                    {
                                        service.ServiceDescriptions.Add(GetNewServiceDescription(newDescription, descriptionTypeId, service.Id, servicelanguage.LanguageId));

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
                        var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "MigrationData_UpdateAndNewServiceDescription_ServiceVersionedIds.txt");
                        File.WriteAllText(FilePath, String.Join("\n", updatedOrAddedServiceVersionsIds.Select(id => id.ToString()).ToArray()));
                    }

                    Console.WriteLine($"ADDED: '{addNewCounter}' service descriptions.");
                    Console.WriteLine($"UPDATED: '{updateCounter}' service descriptions.");
                    Console.WriteLine($"UPDATE and ADD service description by general description data is done.");


                    //Update GD description -> GD background 
                    Console.WriteLine($"General Description UPDATE - description -> background is running...");
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
                        statutoryServiceDescriptionRepository.Add(new StatutoryServiceDescription()
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
                        var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "MigrationData_UpdateGeneralDescriptionToBackground_StatutoryServiceDecripttionVersionedIds.txt"); //CURRENT folder
                        File.WriteAllText(FilePath, String.Join("\n", updatedGeneralDescriptionsIds.Select(id => id.ToString()).ToArray()));
                    }

                    Console.WriteLine($"UPDATED: '{generalDescriptions.Count}' statutory service descriptions of description type -> background description type.");
                    Console.WriteLine($"General Description UPDATE - description -> background update is done.");                    
                });
            }
        }

        private ServiceDescription GetNewServiceDescription(string description, Guid descriptionTypeId, Guid serviceId, Guid languageId)
        {
            return new ServiceDescription()
            {
                Description = description,
                TypeId = descriptionTypeId,
                ServiceVersionedId = serviceId,
                LocalizationId = languageId,
                Created = DateTime.UtcNow,
                CreatedBy = UpdateServiceDescriptionByGeneralDescriptionTask.DefaultCreatedBy,
            };
        }

        private string GetLocalizedStatutoryServiceDescription(IEnumerable<StatutoryServiceDescription> statutoryServiceDescriptions, Guid localizedId, Guid defaultLocalizedId, ITextManager textManager)
        {
            var descriptionTypeCode = DescriptionTypeEnum.Description.ToString();
            var backgroundTypeCode = DescriptionTypeEnum.BackgroundDescription.ToString();

            return CheckGeneralDescription(statutoryServiceDescriptions?.FirstOrDefault(x => x.LocalizationId == localizedId && x.Type.Code == descriptionTypeCode)?.Description, textManager) ??
                   CheckGeneralDescription(statutoryServiceDescriptions?.FirstOrDefault(x => x.LocalizationId == localizedId && x.Type.Code == backgroundTypeCode)?.Description, textManager) ??
                   CheckGeneralDescription(statutoryServiceDescriptions?.FirstOrDefault(x => x.LocalizationId == defaultLocalizedId && x.Type.Code == descriptionTypeCode)?.Description, textManager) ??
                   CheckGeneralDescription(statutoryServiceDescriptions?.FirstOrDefault(x => x.LocalizationId == defaultLocalizedId && x.Type.Code == backgroundTypeCode)?.Description, textManager) ??
                   null;         
        }

        private string CheckGeneralDescription(string gdDescription, ITextManager textManager)
        {
            var test1 = !string.IsNullOrWhiteSpace(gdDescription);
            var test2 = !string.IsNullOrWhiteSpace(textManager.ConvertToPureText(gdDescription));

            return !string.IsNullOrWhiteSpace(gdDescription) && !string.IsNullOrWhiteSpace(textManager.ConvertToPureText(gdDescription))
                ? gdDescription
                : null;
        }

        private StatutoryServiceGeneralDescriptionVersioned GetStatutoryServiceGeneralDescriptionPublished(IEnumerable<StatutoryServiceGeneralDescriptionVersioned> versions)
        {
            var now = DateTime.Now;
            var published = PublishingStatus.Published.ToString();
            return  versions.FirstOrDefault(i => i.PublishingStatus.Code == published &&
                    ((i.ValidFrom <= now && i.ValidTo >= now) ||
                    (i.ValidFrom == null && i.ValidTo == null)));
        }
    }
}
