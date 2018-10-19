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
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Mass;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Framework.Logging;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services.V2
{
    [Framework.RegisterService(typeof(IMassService), RegisterType.Transient)]
    internal class MassService : ServiceBase, IMassService
    {
        private IContextManager contextManager;
        private readonly ITypesCache typesCache;
        private readonly IResolveManager _resolveManager;
        private readonly ILockingManager lockingManager;
        private readonly IVersioningManager versioningManager;
        private readonly MassToolConfiguration massToolConfiguration;
        private ICommonServiceInternal commonService;
        private readonly IChannelServiceInternal channelService;

        private readonly ILogger logger;
        private const string MessageEntitiesPublishedSuccessfully = "MassTool.Publish.Success";
        private const string MessageEntitiesArchivedSuccessfully = "MassTool.Archive.Success";

        public MassService(
            IContextManager contextManager,
            ICacheManager cacheManager,
            IResolveManager resolveManager,
            ILockingManager lockingManager,
            IVersioningManager versioningManager,
            ICommonServiceInternal commonService,
            ITranslationEntity translationEntToVm,
            ITranslationViewModel translationVmtoEnt,
            IOptions<MassToolConfiguration> massToolConfiguration,
            ILogger<MassService> logger,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            IChannelServiceInternal channelService) : base(translationEntToVm, translationVmtoEnt,
            publishingStatusCache, userOrganizationChecker)
        {
            this.typesCache = cacheManager.TypesCache;
            this._resolveManager = resolveManager;
            this.contextManager = contextManager;
            this.versioningManager = versioningManager;
            this.commonService = commonService;
            this.logger = logger;
            this.lockingManager = lockingManager;
            this.massToolConfiguration = massToolConfiguration.Value;
            this.channelService = channelService;
        }

        /// <summary>
        /// Publish entities by language availabilities
        /// </summary>
        /// <param name="resolveManager"></param>
        /// <param name="model"></param>
        public IMessage PublishEntities(IResolveManager resolveManager, VmMassDataModel<VmPublishingModel> model)
        {
            var context = resolveManager.Resolve<IContextManager>();
            var commonServiceInternal = resolveManager.Resolve<ICommonServiceInternal>();
            
            BaseValidation(model, maxEntityCount: massToolConfiguration.MaxPublishEntities, maxLanguageVersionCount: massToolConfiguration.MaxPublishLanguageVersions);
            
            return context.ExecuteWriter((unitOfWork) =>
            {
                if (model.PublishAt.HasValue)
                {
                    SaveMassLanguageAvailabilities(unitOfWork, model);
                }
                else if (!model.PublishAt.HasValue && model.ArchiveAt.HasValue)
                {
                    //Can't change order of methods, modified date will set badly
                    var messages = PublishMassLanguageAvailabilities(unitOfWork, commonServiceInternal, model);
                    SaveMassLanguageAvailabilities(unitOfWork, model);

                    return messages;
                }
                else //immediately publishing 
                {
                    return PublishMassLanguageAvailabilities(unitOfWork, commonServiceInternal, model);
                }

                return null;
            });
        }

        private void BaseValidation<T>(IVmMassDataModel<T> model, int? maxEntityCount = null, int? maxLanguageVersionCount = null) where T : class, IVmLocalizedEntityModel
        {
            if (maxLanguageVersionCount.HasValue)
            {
                var languageVersionCount = model.Services.SelectMany(x => x.LanguagesAvailabilities).Count()
                                           + model.Channels.SelectMany(x => x.LanguagesAvailabilities).Count()
                                           + model.GeneralDescriptions.SelectMany(x => x.LanguagesAvailabilities).Count()
                                           + model.Organizations.SelectMany(x => x.LanguagesAvailabilities).Count()
                                           + model.ServiceCollections.SelectMany(x => x.LanguagesAvailabilities).Count();
                
                if (languageVersionCount > maxLanguageVersionCount)
                {
                    throw new PtvMaxCountLanguageVersionsValidationException("", maxLanguageVersionCount.ToString());

                }
            }
            
            if (maxEntityCount.HasValue)
            {
                var entityCount = model.Services.Select(x => x.Id).Count()
                                  + model.Channels.Select(x => x.Id).Count()
                                  + model.GeneralDescriptions.Select(x => x.Id).Count()
                                  + model.Organizations.Select(x => x.Id).Count()
                                  + model.ServiceCollections.Select(x => x.Id).Count();
                
                if (entityCount > maxEntityCount)
                {
                    throw new PtvMaxCountEntitiesValidationException("", maxEntityCount.ToString());
                }
            }
        }

        private void FilterPublishingEntities(IUnitOfWorkWritable unitOfWork, IVmMassDataModel<VmPublishingModel> model)
        {
            var excludedServiceIds = GetExcludedEntityIds<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, model.Services).ToList();
            model.Services = model.Services.Where(x =>!excludedServiceIds.Contains(x.Id)).ToList();
            
            var excludedChannelIds = GetExcludedEntityIds<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, model.Channels);
            model.Channels = model.Channels.Where(x =>!excludedChannelIds.ToList().Contains(x.Id)).ToList();
            
            var excludedGeneralDescriptionIds = GetExcludedEntityIds<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(unitOfWork,model.GeneralDescriptions).ToList();
            model.GeneralDescriptions = model.GeneralDescriptions.Where(x => !excludedGeneralDescriptionIds.Contains(x.Id)).ToList();
            
            var excludedOrganizationIds = GetExcludedEntityIds<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWork,model.Organizations).ToList();
            model.Organizations = model.Organizations.Where(x => !excludedOrganizationIds.Contains(x.Id)).ToList();
            
            var excludedServiceCollectionIds = GetExcludedEntityIds<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(unitOfWork,model.ServiceCollections).ToList();
            model.ServiceCollections = model.ServiceCollections.Where(x =>!excludedServiceCollectionIds.Contains(x.Id)).ToList();
        }
        
        private IReadOnlyList<Guid> GetExcludedEntityIds<TEntity, TLanguageAvail>(IUnitOfWork unitOfWork, IReadOnlyList<VmPublishingModel> massEntityModels)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability
        {
            var excludeEntityIds = new List<Guid>();
            
            //Locked filter
            excludeEntityIds.AddRange(GetLockedEntityIds<TEntity>(unitOfWork, massEntityModels.Select(x => x.Id).ToList()));
           
            return excludeEntityIds;
        }

        private IMessage PublishMassLanguageAvailabilities(IUnitOfWorkWritable unitOfWork, ICommonServiceInternal commonServiceInternal, IVmMassDataModel<VmPublishingModel> model, bool allowAnonymous = false)
        {
            var allEntities = model.Services.Concat(model.Channels.Concat(model.Organizations.Concat(model.GeneralDescriptions.Concat(model.ServiceCollections)))).ToDictionary(x => x.Id, x => x.LanguagesAvailabilities);
            FilterPublishingEntities(unitOfWork, model);
			var filteredEntities = model.Services.Concat(model.Channels.Concat(model.Organizations.Concat(model.GeneralDescriptions.Concat(model.ServiceCollections)))).ToDictionary(x => x.Id, x => x.LanguagesAvailabilities);
            
            //Publishing
            var channelsResults =
                commonServiceInternal
                    .ExecutePublishEntities<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork,
                        model.Channels);
            
            var affectedEntities = commonServiceInternal.ExecutePublishEntities<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork,model.Services)
                    .Concat(channelsResults)
                    .Concat(commonServiceInternal.ExecutePublishEntities<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(unitOfWork,model.GeneralDescriptions))
                    .Concat(commonServiceInternal.ExecutePublishEntities<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWork,model.Organizations))
                    .Concat(commonServiceInternal.ExecutePublishEntities<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(unitOfWork,model.ServiceCollections));
            
            channelService.RemoveNotCommonConnections(channelsResults.Select(x=>x.Id), unitOfWork);
            if (allowAnonymous)
            {
                AddPreSaveEntityProcessing<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork);
                AddPreSaveEntityProcessing<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork);
                AddPreSaveEntityProcessing<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(unitOfWork);
                AddPreSaveEntityProcessing<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWork);
                AddPreSaveEntityProcessing<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous);
            }
            else
            {
                unitOfWork.Save();
            }

            var diffEntities = allEntities.Keys.Except(filteredEntities.Keys).ToList();
            
            var messages = affectedEntities.OfType<PublishingResultWithValidationMessages>()
                .Where(x => x.ValidationMessages.Any())
                .ToList();

            var unpublishedEntityIds = new List<Guid>();
            if (messages.Any())
            {
                unpublishedEntityIds.AddRange(messages.Select(x => x.Id));
            }

            if (diffEntities.Any())
            {
                unpublishedEntityIds.AddRange(diffEntities);
            }

            logger.LogInformation(
                $"MASS TOOL - Published entity ids: {string.Join(", ", filteredEntities.Select(x => x.Key).ToList().Except(unpublishedEntityIds).Select(x => x.ToString()))}.");
            if (unpublishedEntityIds.Count > 0)
            {
                logger.LogWarning($"MASS TOOL WARNING - Non-Published(filtered, mandatory fields) entity Ids: {string.Join(", ", unpublishedEntityIds.Select(x => x.ToString()))}.");
            }

            var publishId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            return new Message(MessageEntitiesPublishedSuccessfully,
                filteredEntities.Keys.Count + ";" + filteredEntities.SelectMany(x => x.Value).Count(x => x.StatusId == publishId) + ";" +
                unpublishedEntityIds.Count);
        }
                
        private void SaveMassLanguageAvailabilities<T>(IUnitOfWorkWritable unitOfWork, IVmMassDataModel<T> model) where T : class, IVmLocalizedEntityModel
        {
            var userName = unitOfWork.GetUserNameForAuditing();
            var publishAtEpoch = model.PublishAt.FromEpochTime();
            var archiveAtEpoch = model.ArchiveAt.FromEpochTime();

            TranslationManagerToEntity.TranslateAll<VmMassLanguageAvailabilityModel, ServiceLanguageAvailability>(
                SetMassLanguageAvailabilityModel(model.Services, userName, publishAtEpoch, archiveAtEpoch), unitOfWork);
            
            TranslationManagerToEntity.TranslateAll<VmMassLanguageAvailabilityModel, ServiceChannelLanguageAvailability>(
                SetMassLanguageAvailabilityModel(model.Channels, userName, publishAtEpoch,archiveAtEpoch), unitOfWork);
            
            TranslationManagerToEntity.TranslateAll<VmMassLanguageAvailabilityModel, GeneralDescriptionLanguageAvailability>(
                SetMassLanguageAvailabilityModel(model.GeneralDescriptions, userName, publishAtEpoch, archiveAtEpoch), unitOfWork);
            
            TranslationManagerToEntity.TranslateAll<VmMassLanguageAvailabilityModel, OrganizationLanguageAvailability>(
                SetMassLanguageAvailabilityModel(model.Organizations, userName, publishAtEpoch, archiveAtEpoch), unitOfWork);
            
            TranslationManagerToEntity.TranslateAll<VmMassLanguageAvailabilityModel, ServiceCollectionLanguageAvailability>(
                SetMassLanguageAvailabilityModel(model.ServiceCollections, userName, publishAtEpoch, archiveAtEpoch), unitOfWork);

            unitOfWork.Save(preSaveAction: PreSaveAction.DoNotSetAudits); 
        }
        
        private IEnumerable<VmMassLanguageAvailabilityModel> SetMassLanguageAvailabilityModel<T>(
            IEnumerable<T> publishingModelList, string userName, DateTime? validFrom, DateTime? validTo) where T : class, IVmLocalizedEntityModel
        {
            //Set data
            var psId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var languageAvailabilityMassModel = new List<VmMassLanguageAvailabilityModel>();
            publishingModelList?.ForEach(publishingModel =>
            {
                publishingModel.UserName = userName;
                publishingModel.LanguagesAvailabilities.Where(la => typeof(T) != typeof(VmPublishingModel) || la.StatusId == psId).ForEach(y =>
                {
                    y.ValidFrom = validFrom?.ToEpochTime();
                    y.ValidTo = validTo?.ToEpochTime();
                });
                languageAvailabilityMassModel.AddRange(TranslationManagerToVm.Translate<T, VmMassLanguageAvailabilityModelList>(publishingModel).LanguageAvailabilities);
            });
            
            return languageAvailabilityMassModel;
        }

        /// <summary>
        /// Archive entities by entity ids
        /// </summary>
        /// <param name="resolveManager"></param>
        /// <param name="model"></param>
        public IMessage ArchiveEntities(IResolveManager resolveManager, VmMassDataModel<VmArchivingModel> model)
        {
            var context = resolveManager.Resolve<IContextManager>();
            var commonServiceInternal = resolveManager.Resolve<ICommonServiceInternal>();
            
            BaseValidation(model, maxEntityCount: massToolConfiguration.MaxArchiveEntities);
            
            return context.ExecuteWriter((unitOfWork) =>
            {
                
                if (model.ArchiveAt.HasValue)
                {
                    SetLanguageAvailabilitiesOfEntities(unitOfWork, model);
                    SaveMassLanguageAvailabilities(unitOfWork, model);
                    return (IMessage)null;
                }
                else //immediatelly archiving
                {
                    ArchiveEntities(unitOfWork, commonServiceInternal, model);
                    return new Message(MessageEntitiesArchivedSuccessfully, null, null);
                }
            });
        }
        
        private void FilterArchivingEntities(IUnitOfWorkWritable unitOfWork, IVmMassDataModel<VmArchivingModel> model)
        {
            var lockedServiceIds = GetLockedEntityIds<ServiceVersioned>(unitOfWork, model.Services.Select(x => x.Id).ToList());
            model.Services = model.Services.Where(x => !lockedServiceIds.Contains(x.Id)).ToList();
            
            var lockedChannelIds = GetLockedEntityIds<ServiceChannelVersioned>(unitOfWork, model.Channels.Select(x => x.Id).ToList());
            model.Channels = model.Channels.Where(x => !lockedChannelIds.Contains(x.Id)).ToList();
            
            var lockedGeneralDescriptionIds = GetLockedEntityIds<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, model.GeneralDescriptions.Select(x => x.Id).ToList());
            model.GeneralDescriptions = model.GeneralDescriptions.Where(x => !lockedGeneralDescriptionIds.Contains(x.Id)).ToList();
            
            var lockedOrganizationIds = GetLockedEntityIds<OrganizationVersioned>(unitOfWork, model.Organizations.Select(x => x.Id).ToList());
            model.Organizations = model.Organizations.Where(x => !lockedOrganizationIds.Contains(x.Id)).ToList();
            
            var lockedServiceCollectionIds = GetLockedEntityIds<ServiceCollectionVersioned>(unitOfWork, model.ServiceCollections.Select(x => x.Id).ToList());
            model.ServiceCollections = model.ServiceCollections.Where(x => !lockedServiceCollectionIds.Contains(x.Id)).ToList();
        }
        
        private void ArchiveEntities(IUnitOfWorkWritable unitOfWork, ICommonServiceInternal commonServiceInternal, IVmMassDataModel<VmArchivingModel> model)
        {
            FilterArchivingEntities(unitOfWork, model);
            
            commonServiceInternal.ExecuteArchiveEntities<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, model.Services.Select(x => x.Id).ToList());
            commonServiceInternal.ExecuteArchiveEntities<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, model.Channels.Select(x => x.Id).ToList());
            commonServiceInternal.ExecuteArchiveEntities<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(unitOfWork, model.GeneralDescriptions.Select(x => x.Id).ToList());
            commonServiceInternal.ExecuteArchiveEntities<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWork, model.Organizations.Select(x => x.Id).ToList());
            commonServiceInternal.ExecuteArchiveEntities<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(unitOfWork, model.ServiceCollections.Select(x => x.Id).ToList());
            unitOfWork.Save();
        }
        
        private IReadOnlyList<Guid> GetLockedEntityIds<TEntity>(IUnitOfWork unitOfWork, IReadOnlyList<Guid> sourceEntityIds)
            where TEntity : class, IEntityIdentifier, IVersionedVolume
        {
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entities = repository.All().Where(x => sourceEntityIds.Contains(x.Id)).ToList();
            
            var lockedDictionary = lockingManager.IsLocked(unitOfWork, entities.Select(x => x.UnificRootId).ToList());
            var lockedentityIds = entities
                .Where(x => lockedDictionary.ContainsKey(x.UnificRootId) && lockedDictionary[x.UnificRootId] == true)
                .Select(y => y.Id).ToList();

            if (lockedentityIds.Any())
            {
                var logText = $"MASS TOOL WARNING - Filtered locked entities - of entity type: [{typeof(TEntity).Name}] are these ids: {string.Join(",", lockedentityIds)}";
                logger.LogWarning(logText);
            }

            return lockedentityIds;
        }
        
        private void SetLanguageAvailabilitiesOfEntities<T>(IUnitOfWork unitOfWork, IVmMassDataModel<T> model) where T : class, IVmLocalizedEntityModel
        {
            model.Services = TranslationManagerToVm.TranslateAll<IBaseInformation, T>(
                GetEntititiesWithLanguageAvailabilites<ServiceVersioned, ServiceLanguageAvailability>(
                    unitOfWork, model.Services.Select(x => x.Id).ToList()));
            
            model.Channels = TranslationManagerToVm.TranslateAll<IBaseInformation, T>(
                GetEntititiesWithLanguageAvailabilites<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                    unitOfWork, model.Channels.Select(x => x.Id).ToList()));
            
            model.GeneralDescriptions = TranslationManagerToVm.TranslateAll<IBaseInformation, T>(
                GetEntititiesWithLanguageAvailabilites<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(
                    unitOfWork, model.GeneralDescriptions.Select(x => x.Id).ToList()));
            
            model.Organizations = TranslationManagerToVm.TranslateAll<IBaseInformation, T>(
                GetEntititiesWithLanguageAvailabilites<OrganizationVersioned, OrganizationLanguageAvailability>(
                    unitOfWork, model.Organizations.Select(x => x.Id).ToList()));
            
            model.ServiceCollections = TranslationManagerToVm.TranslateAll<IBaseInformation, T>(
                GetEntititiesWithLanguageAvailabilites<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(
                    unitOfWork, model.ServiceCollections.Select(x => x.Id).ToList()));
        }

        private IEnumerable<TEntity> GetEntititiesWithLanguageAvailabilites<TEntity, TLanguageAvailEntity>(IUnitOfWork unitOfWork, List<Guid> entityIds)
            where TLanguageAvailEntity : LanguageAvailability, ILanguageAvailabilityBase, new()
            where TEntity : class, IBaseInformation, IMultilanguagedEntity<TLanguageAvailEntity>
        {
            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            
            var entities = unitOfWork.ApplyIncludes(repository.All().Where(x => entityIds.Contains(x.Id)), q => q
                .Include(i => i.LanguageAvailabilities)
            ).ToList();

            return entities;
        }
        
        //Archive language availability in scheduler
        private void ArchiveMassLanguageAvailabilities(IUnitOfWorkWritable unitOfWork, ICommonServiceInternal commonServiceInternal, IVmMassDataModel<VmArchivingModel> model, bool allowAnonymous = false)
        {
            FilterArchivingEntities(unitOfWork, model);
             
            //Archiving
            commonServiceInternal.ExecuteArchiveEntityLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, model.Services);
            commonServiceInternal.ExecuteArchiveEntityLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, model.Channels);
            commonServiceInternal.ExecuteArchiveEntityLanguageVersions<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(unitOfWork, model.GeneralDescriptions);
            commonServiceInternal.ExecuteArchiveEntityLanguageVersions<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWork, model.Organizations);
            commonServiceInternal.ExecuteArchiveEntityLanguageVersions<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(unitOfWork, model.ServiceCollections);

            if (allowAnonymous)
            {
                AddPreSaveEntityProcessing<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork);
                AddPreSaveEntityProcessing<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork);
                AddPreSaveEntityProcessing<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(unitOfWork);
                AddPreSaveEntityProcessing<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWork);
                AddPreSaveEntityProcessing<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous);
            }
            else
            {
                unitOfWork.Save();
            }
        }
        
        /// <summary>
        /// Copying selected entities by ids 
        /// </summary>
        /// <param name="resolveManager"></param>
        /// <param name="model"></param>
        public IMessage CopyEntities(IResolveManager resolveManager, VmMassDataModel<VmCopyingModel> model)
        {
            var context = resolveManager.Resolve<IContextManager>();
            var commonServiceInternal = resolveManager.Resolve<ICommonServiceInternal>();
            
            CopyValidation(model);
            
            return context.ExecuteWriter((unitOfWork) =>
            {
                ExecuteCopyServices(unitOfWork, commonServiceInternal, model.Services, model.OrganizationId);
                commonServiceInternal.ExecuteCopyEntities<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(unitOfWork, model.Channels.Select(x => x.Id).ToList(), model.OrganizationId);
                ExecuteCopyServiceCollectionWithConnectedServices(unitOfWork, commonServiceInternal, model.ServiceCollections.Select(x => x.Id).ToList(), model.OrganizationId);

                unitOfWork.Save();
                
                return (IMessage)null;
            });
        }

        private void CopyValidation(VmMassDataModel<VmCopyingModel> model)
        {
            if (!model.OrganizationId.IsAssigned())
            {
                throw new PtvMandatoryOrganizationValidationException("");
            }
                
            BaseValidation(model, maxEntityCount: massToolConfiguration.MaxCopyEntities);
        }
        
        /// <summary>
        /// Execute copy of services, If exist self service provider is organization changed by selected organization 
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="commonServiceInternal"></param>
        /// <param name="entities"></param>
        /// <param name="organizationId"></param>
        private void ExecuteCopyServices(IUnitOfWorkWritable unitOfWork, ICommonServiceInternal commonServiceInternal, IReadOnlyList<VmCopyingModel> entities, Guid organizationId)
        {
            foreach (var unificRootId in entities.Select(x => x.UnificRootId).Distinct())
            {
                //GetLastVersion 
                var lastEntityVersion =
                    versioningManager.GetLastPublishedDraftVersion<ServiceVersioned>(unitOfWork, unificRootId);
                if (lastEntityVersion != null)
                {
                    var entityRep = unitOfWork.CreateRepository<IRepository<ServiceVersioned>>();
                    var lastPublishedDraftEntity = entityRep.All().Single(x => x.Id == lastEntityVersion.EntityId);
                    var copiedEntity = versioningManager.CreateCopyVersion<ServiceVersioned, Service, ServiceLanguageAvailability>
                        (
                            unitOfWork,
                            lastPublishedDraftEntity,
                            PublishingStatus.Draft
                        );

                    var selfProducedId = typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString());
                    if (copiedEntity.ServiceProducers.Any(x =>
                        x.ProvisionTypeId == selfProducedId &&
                        x.Organizations.Any(y => y.OrganizationId == lastPublishedDraftEntity.OrganizationId)
                        ))
                    {
                        var serviceOrganizationProducers = copiedEntity.ServiceProducers
                            .Where(f => f.ProvisionTypeId == selfProducedId).SelectMany(x =>
                                x.Organizations
                                    .Where(spo => spo.OrganizationId == lastPublishedDraftEntity.OrganizationId)
                                    .Select(y => y));

                        serviceOrganizationProducers.ForEach(x => x.OrganizationId = organizationId);
                    }

                    //copy selected
                    if (lastPublishedDraftEntity != null)
                    {
                        commonServiceInternal.FinalizeCopyEntity<ServiceVersioned, ServiceLanguageAvailability>(
                            copiedEntity, lastPublishedDraftEntity.Id, organizationId);
                    }
                }

            }
        }
        
        /// <summary>
        /// Execute copy of service collections with connected services 
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="commonServiceInternal"></param>
        /// <param name="entityIds"></param>
        /// <param name="organizationId"></param>
        private void ExecuteCopyServiceCollectionWithConnectedServices(IUnitOfWorkWritable unitOfWork, ICommonServiceInternal commonServiceInternal, IReadOnlyList<Guid> entityIds, Guid organizationId)
        {
            foreach (var entityVersionedId in entityIds)
            {
                var unificRootId = versioningManager.GetUnificRootId<ServiceCollectionVersioned>(unitOfWork, entityVersionedId);

                if (unificRootId.HasValue)
                {
                    //GetLastVersion 
                    var lastEntityVersion = versioningManager.GetLastPublishedDraftVersion<ServiceCollectionVersioned>(unitOfWork, unificRootId.Value);
                    if (lastEntityVersion != null)
                    {
                        var entityRep = unitOfWork.CreateRepository<IRepository<ServiceCollectionVersioned>>();
                        var lastPublishedDraftEntity = unitOfWork
                            .ApplyIncludes(entityRep.All(), q =>
                               q.Include(i => i.UnificRoot)
                                .ThenInclude(i => i.ServiceCollectionServices))
                            .SingleOrDefault(x => x.Id == lastEntityVersion.EntityId);

                        var copiedEntity = versioningManager.CreateCopyVersion<ServiceCollectionVersioned, ServiceCollection, ServiceCollectionLanguageAvailability>(unitOfWork, lastPublishedDraftEntity, PublishingStatus.Draft);

                        //copy selected
                        if (lastPublishedDraftEntity != null)
                        {
                            copiedEntity.UnificRoot.ServiceCollectionServices = CopyServiceCollectionServices(unitOfWork, lastPublishedDraftEntity.UnificRoot?.ServiceCollectionServices,
                                    copiedEntity.UnificRootId).ToList();
                            
                            commonServiceInternal.FinalizeCopyEntity<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(copiedEntity, lastPublishedDraftEntity.Id, organizationId);
                        }
                    }
                }
            }
        }
        
        private IReadOnlyList<PTV.Database.Model.Models.ServiceCollectionService> CopyServiceCollectionServices(IUnitOfWorkWritable unitOfWork, IEnumerable<PTV.Database.Model.Models.ServiceCollectionService> oldEntities, Guid newServiceCollectionId)
        {
            var serviceCollectionServiceRep = unitOfWork.CreateRepository<IRepository<PTV.Database.Model.Models.ServiceCollectionService>>();
            var result = new List<PTV.Database.Model.Models.ServiceCollectionService>();

            if (oldEntities == null || !oldEntities.Any())
            {
                return result;
            }

            foreach (var sourceEntity in oldEntities)
            {
                var serviceCollectionService = new PTV.Database.Model.Models.ServiceCollectionService()
                {
                    ServiceId = sourceEntity.ServiceId,
                    OrderNumber = sourceEntity.OrderNumber,
                    ServiceCollectionId = newServiceCollectionId
                };

                serviceCollectionServiceRep.Add(serviceCollectionService);
                result.Add(serviceCollectionService);
            }

            return result;
        }

        #region TaskScheduler

        public void PublishScheduledLanguageVersions(DateTime dateTime)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var model = GetLanguageVersionsForPublish(unitOfWork, dateTime);
                try
                {
                    logger.LogInformation($"MASS Publishing date: {dateTime.Date:u} {dateTime:u}, services: {model.Services.Count}, channels: {model.Channels.Count}, organization: {model.Organizations.Count}.");
					PublishMassLanguageAvailabilities(unitOfWork, commonService, model, true);
                }
                catch (PtvEntitiesValidationException e)
                {
                    logger.LogWarning($"MASS TOOL WARNING - Scheduled Publishing not published entities: {string.Join(", ", e.NotPublishedEntities.Select(x => x.ToString()))}");
                }
                catch (Exception e)
                {
                    logger.LogError($"MASS TOOL ERROR - Scheduled Publishing failed with exception:{e.GetType()} message:{e.Message} details:{CoreExtensions.ExtractAllInnerExceptions(e)}");
                }
            });    
        }

        private IVmMassDataModel<VmPublishingModel> GetLanguageVersionsForPublish(IUnitOfWork unitOfWork, DateTime startDate)
        {
            var model = new VmMassDataModel<VmPublishingModel>()
            {
                Services = TranslationManagerToVm.TranslateAll<IBaseInformation, VmPublishingModel>(
                    GetReviewedEntitities<ServiceLanguageAvailability, ServiceVersioned>(unitOfWork, startDate)),
                Channels = TranslationManagerToVm.TranslateAll<IBaseInformation, VmPublishingModel>(
                    GetReviewedEntitities<ServiceChannelLanguageAvailability, ServiceChannelVersioned>(unitOfWork, startDate)),
                GeneralDescriptions = TranslationManagerToVm.TranslateAll<IBaseInformation, VmPublishingModel>(
                    GetReviewedEntitities<GeneralDescriptionLanguageAvailability, StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, startDate)),
                Organizations = TranslationManagerToVm.TranslateAll<IBaseInformation, VmPublishingModel>(
                    GetReviewedEntitities<OrganizationLanguageAvailability, OrganizationVersioned>(unitOfWork, startDate)),
                ServiceCollections = TranslationManagerToVm.TranslateAll<IBaseInformation, VmPublishingModel>(
                    GetReviewedEntitities<ServiceCollectionLanguageAvailability, ServiceCollectionVersioned>(unitOfWork, startDate))
            };

            return model;
        }

        private IEnumerable<TOutEntity> GetReviewedEntitities<TLanguageAvailEntity, TOutEntity>(IUnitOfWork unitOfWork,
            DateTime? startDate)
            where TLanguageAvailEntity : LanguageAvailability, ILanguageAvailabilityBase, new()
            where TOutEntity : class, IBaseInformation, IMultilanguagedEntity<TLanguageAvailEntity>
        {
            var repository = unitOfWork.CreateRepository<IRepository<TLanguageAvailEntity>>();

            var allVersions = repository.All()
                .Where(x => x.PublishAt.Value.Date <= startDate.Value.Date)
                .ToList();

            //            var reviewedVersions = allVersions
            //                .Where(x => x.Reviewed >= x.Modified)
            //                .ToList();

            //            var modifiedVersionIds = allVersions.Select(x => x.Id).Distinct().ToList();
            //            if (modifiedVersionIds.Any())
            //            {
            //                logger.LogWarning($"MASS TOOL WARNING - Publishing entity: {typeof(TOutEntity)}, modified not reviewed ids: {string.Join(", ", modifiedVersionIds.Select(x => x.ToString()))}");
            //            }

            var instanceList = Activator.CreateInstance<List<TOutEntity>>();
            allVersions.GroupBy(x => x.Id).ForEach(x =>
            {
                var instance = Activator.CreateInstance<TOutEntity>();
                instance.Id = x.Key;
                instance.LanguageAvailabilities = x.ToList();
                instanceList.Add(instance);
            });

            return instanceList;
        }
        
        #region Archiving 

        /// <summary>
        /// Archive scheduled language versions
        /// </summary>
        /// <param name="dateTime">Input for archiving language version.</param>
        /// <param name="jobName"></param>
        public void ArchiveScheduledLanguageVersions(DateTime dateTime)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var model = GetLanguageVersionsForArchiving(unitOfWork, dateTime);
                try
                {
                    ArchiveMassLanguageAvailabilities(unitOfWork, commonService, model, true); 
                }
                catch (Exception e)
                {
                    logger.LogError($"MASS TOOL ERROR - Scheduled Archiving failed with exception:{e.GetType()} message:{e.Message} details:{CoreExtensions.ExtractAllInnerExceptions(e)}");
                }
            }); 
        }
        
        private VmMassDataModel<VmArchivingModel> GetLanguageVersionsForArchiving(IUnitOfWork unitOfWork, DateTime? archivingDate)
        {
            archivingDate = archivingDate ?? DateTime.UtcNow;
            
            var model = new VmMassDataModel<VmArchivingModel>()
            {
                Services = TranslationManagerToVm.TranslateAll<IBaseInformation, VmArchivingModel>(
                    GetEntititiesToArchiving<ServiceLanguageAvailability, ServiceVersioned>(unitOfWork, archivingDate)),
                Channels = TranslationManagerToVm.TranslateAll<IBaseInformation, VmArchivingModel>(
                    GetEntititiesToArchiving<ServiceChannelLanguageAvailability, ServiceChannelVersioned>(unitOfWork, archivingDate)),
                GeneralDescriptions = TranslationManagerToVm.TranslateAll<IBaseInformation, VmArchivingModel>(
                    GetEntititiesToArchiving<GeneralDescriptionLanguageAvailability, StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, archivingDate)),
                Organizations = TranslationManagerToVm.TranslateAll<IBaseInformation, VmArchivingModel>(
                    GetEntititiesToArchiving<OrganizationLanguageAvailability, OrganizationVersioned>(unitOfWork, archivingDate)),
                ServiceCollections = TranslationManagerToVm.TranslateAll<IBaseInformation, VmArchivingModel>(
                    GetEntititiesToArchiving<ServiceCollectionLanguageAvailability, ServiceCollectionVersioned>(unitOfWork, archivingDate))
            };

            return model;
        }
        
        private IEnumerable<TOutEntity> GetEntititiesToArchiving<TLanguageAvailEntity, TOutEntity>(IUnitOfWork unitOfWork,
            DateTime? archivedDate)
            where TLanguageAvailEntity : LanguageAvailability, ILanguageAvailabilityBase, new()
            where TOutEntity : class, IBaseInformation, IMultilanguagedEntity<TLanguageAvailEntity>
        {
            var repository = unitOfWork.CreateRepository<IRepository<TLanguageAvailEntity>>();
            
            var allArchiveVersions = repository.All()
                .Where(x => x.ArchiveAt.Value.Date <= archivedDate.Value.Date)
                .ToList();

            var instanceList = Activator.CreateInstance<List<TOutEntity>>();
            allArchiveVersions.GroupBy(x => x.Id).ForEach(x =>
            {
                var instance = Activator.CreateInstance<TOutEntity>();
                instance.Id = x.Key;
                instance.LanguageAvailabilities = x.ToList();
                instanceList.Add(instance);
            });

            return instanceList;
        }
        
        #endregion Archiving
        
        #endregion TaskScheduler

        private static void AddPreSaveEntityProcessing<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork)
            where TEntity : class, IMultilanguagedEntity<TLanguageAvail>
            where TLanguageAvail : class, ILanguageAvailability
        {
            unitOfWork.AddPreSaveEntityProcessing<TLanguageAvail>((langEntity, entityState) =>
            {
                langEntity.ModifiedBy = langEntity.ReviewedBy;
            });
            unitOfWork.AddPreSaveEntityProcessing<TEntity>((entity, entityState) =>
            {
                if (!(entity is IAuditing auditingEntity)) return;
                var reviewer = entity.LanguageAvailabilities
                    .Where(la => !string.IsNullOrEmpty(la.ReviewedBy))
                    .OrderByDescending(la => la.Reviewed)
                    .FirstOrDefault()?.ReviewedBy;
                auditingEntity.ModifiedBy = reviewer;
            });
        }
    }
}
