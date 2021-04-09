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
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Mass;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Framework.Logging;
using PTV.Framework.ServiceManager;
using ServiceCollection = PTV.Database.Model.Models.ServiceCollection;

namespace PTV.Database.DataAccess.Services.V2
{
    [Framework.RegisterService(typeof(IMassService), RegisterType.Transient)]
    internal class MassService : ServiceBase, IMassService
    {
        private IContextManager contextManager;
        private readonly ITypesCache typesCache;
        private readonly ILockingManager lockingManager;
        private readonly MassToolConfiguration massToolConfiguration;
        private ICommonServiceInternal commonService;
        private readonly IChannelServiceInternal channelService;
        private readonly IGeneralDescriptionServiceInternal generalDescriptionService;
        private readonly IServiceServiceInternal serviceService;
        private readonly IOrganizationServiceInternal organizationService;

        private readonly ILogger logger;
        private readonly IExpirationTimeCache expirationTimeCache;
        private readonly IExpirationService expirationService;
        private readonly IServiceUtilities utilities;
        private const string MassToolFinnishedSuccessfully = "MassTool.{0}.Success";
        private const string MassToolError = "MassTool.{0}.Error";

        public MassService(
            IContextManager contextManager,
            ICacheManager cacheManager,
            ILockingManager lockingManager,
            IVersioningManager versioningManager,
            ICommonServiceInternal commonService,
            ITranslationEntity translationEntToVm,
            ITranslationViewModel translationVmtoEnt,
            IOptions<MassToolConfiguration> massToolConfiguration,
            ILogger<MassService> logger,
            IUserOrganizationChecker userOrganizationChecker,
            IChannelServiceInternal channelService,
            IGeneralDescriptionServiceInternal generalDescriptionService,
            IServiceServiceInternal serviceService,
            IExpirationTimeCache expirationTimeCache,
            IOrganizationServiceInternal organizationService,
            IExpirationService expirationService,
            IServiceUtilities utilities) : base(translationEntToVm, translationVmtoEnt, cacheManager.PublishingStatusCache, userOrganizationChecker, versioningManager)
        {
            this.typesCache = cacheManager.TypesCache;
            this.contextManager = contextManager;
            this.commonService = commonService;
            this.logger = logger;
            this.lockingManager = lockingManager;
            this.massToolConfiguration = massToolConfiguration.Value;
            this.channelService = channelService;
            this.generalDescriptionService = generalDescriptionService;
            this.serviceService = serviceService;
            this.organizationService = organizationService;
            this.expirationTimeCache = expirationTimeCache;
            this.expirationService = expirationService;
            this.utilities = utilities;
        }

        /// <summary>
        /// Publish entities by language availabilities
        /// </summary>
        /// <param name="model"></param>
        /// <param name="result"></param>
        public IMessage PublishEntities(VmMassDataModel<VmPublishingModel> model, IServiceResultWrap result)
        {
            PublishEntitiesValidation(model, maxLanguageVersionCount: massToolConfiguration.MaxPublishLanguageVersions);

            return contextManager.ExecuteWriter((unitOfWork) =>
            {
                if (model.PublishAt.HasValue)
                {
                    SaveMassLanguageAvailabilities(unitOfWork, model, true);
                }
                else if (!model.PublishAt.HasValue && model.ArchiveAt.HasValue)
                {
                    //Can't change order of methods, modified date will set badly
                    var messages = PublishMassLanguageAvailabilities(unitOfWork, commonService, model, result);
                    SaveMassLanguageAvailabilities(unitOfWork, model, false);

                    return messages;
                }
                else //immediately publishing
                {
                    return PublishMassLanguageAvailabilities(unitOfWork, commonService, model, result);
                }

                return null;
            });
        }

        private void PublishEntitiesValidation<T>(IVmMassDataModel<T> model, int maxLanguageVersionCount) where T : class, IVmLocalizedEntityModel
        {
            var publishedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());

            var languageVersionCount = model.Services.SelectMany(x => x.LanguagesAvailabilities.Where(y => y.StatusId == publishedStatusId)).Count()
                                       + model.Channels.SelectMany(x => x.LanguagesAvailabilities.Where(y => y.StatusId == publishedStatusId)).Count()
                                       + model.GeneralDescriptions.SelectMany(x => x.LanguagesAvailabilities.Where(y => y.StatusId == publishedStatusId)).Count()
                                       + model.Organizations.SelectMany(x => x.LanguagesAvailabilities.Where(y => y.StatusId == publishedStatusId)).Count()
                                       + model.ServiceCollections.SelectMany(x => x.LanguagesAvailabilities.Where(y => y.StatusId == publishedStatusId)).Count();

            if (languageVersionCount > maxLanguageVersionCount)
            {
                throw new PtvMaxCountLanguageVersionsValidationException("", maxLanguageVersionCount.ToString());
            }
        }

        private void BaseValidation<T>(IVmMassDataModel<T> model, int? maxLanguageVersionCount = null) where T : class, IVmLocalizedEntityModel
        {
            if (maxLanguageVersionCount.HasValue)
            {
                int serviceVersionCount = 0, 
                    channelLanguageVersionCount = 0, 
                    generalDescriptionVersionCount = 0,
                    organizationVersionCount = 0,
                    serviceCollectionVersionCount = 0;
                
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var serviceIds = model.Services.Select(x => x.Id).ToList();
                    if (serviceIds.Any())
                    {
                        var serviceLanguageAvailabilityRep = unitOfWork.CreateRepository<IServiceLanguageAvailabilityRepository>();
                        serviceVersionCount = serviceLanguageAvailabilityRep
                            .All().Count(x => serviceIds.Contains(x.ServiceVersionedId));
                    }

                    var channelIds = model.Channels.Select(x => x.Id).ToList();
                    if (channelIds.Any())
                    {
                        var channelLanguageAvailabilityRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
                        channelLanguageVersionCount = channelLanguageAvailabilityRep
                            .All().Count(x => channelIds.Contains(x.ServiceChannelVersionedId));
                    }

                    var generalDescriptionIds = model.GeneralDescriptions.Select(x => x.Id).ToList();
                    if (generalDescriptionIds.Any())
                    {
                        var generalDescriptionAvailabilityRep = unitOfWork.CreateRepository<IGeneralDescriptionLanguageAvailabilityRepository>();
                        generalDescriptionVersionCount = generalDescriptionAvailabilityRep
                            .All().Count(x => generalDescriptionIds.Contains(x.StatutoryServiceGeneralDescriptionVersionedId));
                    }

                    var organizationIds = model.Organizations.Select(x => x.Id).ToList();
                    if (organizationIds.Any())
                    {
                        var organizationAvailabilityRep = unitOfWork.CreateRepository<IOrganizationLanguageAvailabilityRepository>();
                        organizationVersionCount = organizationAvailabilityRep
                            .All().Count(x => organizationIds.Contains(x.OrganizationVersionedId));
                    }

                    var serviceCollectionIds = model.ServiceCollections.Select(x => x.Id).ToList();
                    if (serviceCollectionIds.Any())
                    {
                        var serviceCollectionRep = unitOfWork.CreateRepository<IServiceCollectionLanguageAvailabilityRepository>();
                        serviceCollectionVersionCount = serviceCollectionRep
                            .All().Count(x => serviceCollectionIds.Contains(x.ServiceCollectionVersionedId));
                    }
                });

                var languageVersionCount = serviceVersionCount + 
                                           channelLanguageVersionCount +
                                           generalDescriptionVersionCount + 
                                           organizationVersionCount +
                                           serviceCollectionVersionCount;
                
                if (languageVersionCount > maxLanguageVersionCount)
                {
                    throw new PtvMaxCountLanguageVersionsValidationException("", maxLanguageVersionCount.ToString());
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

        private IMessage PublishMassLanguageAvailabilities(IUnitOfWorkWritable unitOfWork, ICommonServiceInternal commonServiceInternal, IVmMassDataModel<VmPublishingModel> model, IServiceResultWrap result = null, bool allowAnonymous = false)
        {
            var allEntities = model.Services.Concat(model.Channels.Concat(model.Organizations.Concat(model.GeneralDescriptions.Concat(model.ServiceCollections))))
                .ToDictionary(x => x.Id, x => x.LanguagesAvailabilities);
            FilterPublishingEntities(unitOfWork, model);

            //Publishing
            var channelsResults =
                commonServiceInternal
                    .ExecutePublishEntities<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork,
                        model.Channels, allowAnonymous);
            var servicesResults =
                commonServiceInternal.ExecutePublishEntities<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork,
                    model.Services, allowAnonymous);

            var affectedEntities = servicesResults
                .Concat(channelsResults)
                .Concat(commonServiceInternal.ExecutePublishEntities<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(unitOfWork,model.GeneralDescriptions, allowAnonymous))
                .Concat(commonServiceInternal.ExecutePublishEntities<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWork,model.Organizations, allowAnonymous))
                .Concat(commonServiceInternal.ExecutePublishEntities<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(unitOfWork,model.ServiceCollections, allowAnonymous));

            commonService.RemoveNotCommonConnections(channelsResults.Select(x=>x.Id), unitOfWork);
            if (allowAnonymous)
            {
                AddPreSaveEntityProcessingForPublishing<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork);
                AddPreSaveEntityProcessingForPublishing<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork);
                AddPreSaveEntityProcessingForPublishing<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(unitOfWork);
                AddPreSaveEntityProcessingForPublishing<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWork);
                AddPreSaveEntityProcessingForPublishing<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous);
            }
            else
            {
                unitOfWork.Save();
            }
            
            expirationService.SetExpirationDateForPublishing<ServiceChannelVersioned>(unitOfWork, channelsResults.Select(x=>x.Id), true);
            expirationService.SetExpirationDateForPublishing<ServiceVersioned>(unitOfWork, servicesResults.Select(x=>x.Id), true);

            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            var oldPublishedId = PublishingStatusCache.Get(PublishingStatus.OldPublished);
            var resultEntityIds = affectedEntities.Aggregate
            (
                new {Published = new List<Guid>(), Unpublished = new List<Guid>()},
                (acc, publishingResult) =>
                {
                    if (publishingResult.PublishingStatusNew == publishedId)
                    {
                        Guid? publishEntityId = null;
                        if (publishingResult.PublishingStatusOld == publishedId)
                        {
                            publishEntityId = publishingResult.AffectedEntities
                                .FirstOrDefault(x =>
                                    x.PublishingStatusOld == oldPublishedId && x.PublishingStatusNew == oldPublishedId)
                                ?.Id;
                        }
                        acc.Published.Add(publishEntityId ?? publishingResult.Id);
                    }
                    else
                    {
                        acc.Unpublished.Add(publishingResult.Id);
                    }

                    return acc;
                }
            );

            logger.LogInformation(
                $"MASS TOOL - Published entity ids: {string.Join(", ", resultEntityIds.Published)}.");
            if (resultEntityIds.Unpublished.Count > 0)
            {
                logger.LogWarning($"MASS TOOL WARNING - Non-Published(filtered, mandatory fields) entity Ids: {string.Join(", ", resultEntityIds.Unpublished)}.");
                result?.Messages.Errors.Add
                (
                    new Error
                    (
                        string.Format(MassToolError, MassToolType.Publish),
                        new []
                        {
                            resultEntityIds.Unpublished.SelectMany(x => allEntities.TryGet(x)).Count(x =>  x.StatusId == publishedId).ToString()
                        }
                    )
                );
            }

            var publishId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var counts = resultEntityIds.Published.Aggregate(
                (PublishedLanguages: 0, UnpublishedLanguages: 0),
                (acc, id) =>
                {
                    var languages = allEntities.TryGet(id);
                    var published = languages?.Count(x => x.StatusId == publishId) ?? 0;
                    acc.PublishedLanguages += published;
                    acc.UnpublishedLanguages += (languages?.Count ?? 0) - published;
                    return acc;
                }
            );
            return new VmMassToolProcessMessage
            (
                model.Id,
                string.Format(MassToolFinnishedSuccessfully, MassToolType.Publish),
                new []
                {
                    counts.PublishedLanguages.ToString(),
                    counts.UnpublishedLanguages.ToString()
                }
            );
        }

        private void SaveMassLanguageAvailabilities<T>(IUnitOfWorkWritable unitOfWork, IVmMassDataModel<T> model, bool setExpirationDates) where T : class, IVmLocalizedEntityModel
        {
            var publishAtEpoch = model.PublishAt.FromEpochTime();
            var archiveAtEpoch = model.ArchiveAt.FromEpochTime();

            ValidatePublishDates(unitOfWork, model, publishAtEpoch, archiveAtEpoch);

            var serviceIds = SaveMassLanguageAvailabilities<T, ServiceVersioned, ServiceLanguageAvailability>
                (unitOfWork, model.Services, publishAtEpoch, archiveAtEpoch);
            var channelIds = SaveMassLanguageAvailabilities<T, ServiceChannelVersioned, ServiceChannelLanguageAvailability>
                (unitOfWork, model.Channels, publishAtEpoch, archiveAtEpoch);
            SaveMassLanguageAvailabilities<T, StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>
                (unitOfWork, model.GeneralDescriptions, publishAtEpoch, archiveAtEpoch);
            SaveMassLanguageAvailabilities<T, OrganizationVersioned, OrganizationLanguageAvailability>
                (unitOfWork, model.Organizations, publishAtEpoch, archiveAtEpoch);
            SaveMassLanguageAvailabilities<T, ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>
                (unitOfWork, model.ServiceCollections, publishAtEpoch, archiveAtEpoch);

            unitOfWork.Save(preSaveAction: PreSaveAction.DoNotSetAudits);

            if (setExpirationDates)
            {
                expirationService.SetExpirationDatesForDraft<ServiceVersioned>(unitOfWork, serviceIds, true);
                expirationService.SetExpirationDatesForDraft<ServiceChannelVersioned>(unitOfWork, channelIds, true);
            }
        }

        private IEnumerable<Guid> SaveMassLanguageAvailabilities<TModel, TEntity, TLanguageAvail>(
            IUnitOfWorkWritable unitOfWork,
            IReadOnlyList<TModel> models,
            DateTime? publishAtEpoch,
            DateTime? archiveAtEpoch)
            where TModel : class, IVmLocalizedEntityModel
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IAuditing, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailabilityBase
        {
            var userName = unitOfWork.GetUserNameForAuditing();
            var entityIds = models.Select(x => x.Id).ToList();
            var modifiedDate = DateTime.UtcNow;

            var languageAvailabilityModels =
                SetMassLanguageAvailabilityModel(models, userName, publishAtEpoch, archiveAtEpoch);

            var languageAvailabilities = TranslationManagerToEntity
                .TranslateAll<VmMassLanguageAvailabilityModel, TLanguageAvail>(languageAvailabilityModels, unitOfWork).ToList();

            var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entities = repository.All()
                .Include(x => x.Versioning)
                .Where(x => entityIds.Contains(x.Id))
                .ToList();

            commonService.UpdateHistoryMetaData(entities, languageAvailabilities);
            commonService.UpdateModifiedDates(entities, languageAvailabilities, modifiedDate);
            return entities.Select(x => x.Id);
        }

        private void ValidatePublishDates<T>(IUnitOfWorkWritable unitOfWork, IVmMassDataModel<T> model, DateTime? publishAtEpoch,
            DateTime? archiveAtEpoch) where T : class, IVmLocalizedEntityModel
        {
            DateTime utcNow = DateTime.Now;
            
            var channelExpirationMonths = (int)expirationTimeCache
                .GetExpirationMonths(typeof(ServiceChannel), PublishingStatusCache.Get(PublishingStatus.Published));
            var channelExpiration = utcNow.AddMonths(channelExpirationMonths);
            
            var channelsScheduledLate = model.Channels.Any(c =>
                channelExpiration < publishAtEpoch);

            if (publishAtEpoch > archiveAtEpoch || channelsScheduledLate)
                throw new PtvAppException("Publishing date cannot be scheduled after archiving date.",
                    "Common.ScheduleException.LateDate");
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
        /// <param name="model"></param>
        public IMessage ArchiveEntities(VmMassDataModel<VmArchivingModel> model)
        {
            BaseValidation(model, maxLanguageVersionCount: massToolConfiguration.MaxArchiveLanguageVersions);
            
            return contextManager.ExecuteWriter((unitOfWork) =>
            {
                if (model.ArchiveAt.HasValue)
                {
                    SetLanguageAvailabilitiesOfEntities(unitOfWork, model);
                    SaveMassLanguageAvailabilities(unitOfWork, model, false);
                    return (IMessage)null;
                }
                // immediately archiving
                ArchiveEntities(unitOfWork, commonService, model);
                return new VmMassToolProcessMessage(model.Id, string.Format(MassToolFinnishedSuccessfully, MassToolType.Archive), null, null);

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

            commonServiceInternal.ExecuteArchiveEntities<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, model.Services.Select(x => x.Id).ToList(), HistoryAction.MassArchive, FilterArchiveAction<ServiceVersioned>(serviceService.OnDeletingService));
            commonServiceInternal.ExecuteArchiveEntities<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, model.Channels.Select(x => x.Id).ToList(), HistoryAction.MassArchive, FilterArchiveAction<ServiceChannelVersioned>(channelService.OnDeletingChannel));
            commonServiceInternal.ExecuteArchiveEntities<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(unitOfWork, model.GeneralDescriptions.Select(x => x.Id).ToList(), HistoryAction.MassArchive, FilterArchiveAction<StatutoryServiceGeneralDescriptionVersioned>(generalDescriptionService.OnDeletingGeneralDescription));
            commonServiceInternal.ExecuteArchiveEntities<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWork,model.Organizations.Select(x => x.Id).ToList(), HistoryAction.MassArchive, FilterArchiveAction<OrganizationVersioned>((uOW, id) => organizationService.CascadeDeleteOrganization(uOW, id, HistoryAction.MassArchive)));
            commonServiceInternal.ExecuteArchiveEntities<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(unitOfWork, model.ServiceCollections.Select(x => x.Id).ToList(), HistoryAction.MassArchive);
            unitOfWork.Save();
        }

        private Action<IUnitOfWorkWritable, Guid, TEntity> FilterArchiveAction<TEntity>(Action<IUnitOfWorkWritable, Guid> onAdditionalAction) where TEntity : class, IEntityIdentifier, IVersionedVolume, new()
        {
            return (unitOfWork, id, entity) =>
            {
                if (entity != null && entity.PublishingStatusId != PublishingStatusCache.Get(PublishingStatus.Published))
                {
                    return;
                }
                onAdditionalAction.Invoke(unitOfWork, id);
            };
        }

        private IReadOnlyList<Guid> GetLockedEntityIds<TEntity>(IUnitOfWork unitOfWork, IReadOnlyList<Guid> sourceEntityIds)
            where TEntity : class, IEntityIdentifier, IVersionedVolume
        {
            if (sourceEntityIds.Count == 0)
            {
                return sourceEntityIds;
            }
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
            if (entityIds.Count == 0)
            {
                return new List<TEntity>();
            }

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
            commonServiceInternal.ExecuteArchiveEntityLanguageVersions<ServiceVersioned, ServiceLanguageAvailability>(
                unitOfWork, model.Services, HistoryAction.ArchivedViaScheduling,
                FilterArchiveAction<ServiceVersioned>(serviceService.OnDeletingService));
            commonServiceInternal
                .ExecuteArchiveEntityLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                    unitOfWork, model.Channels, HistoryAction.ArchivedViaScheduling,
                    FilterArchiveAction<ServiceChannelVersioned>(channelService.OnDeletingChannel));
            commonServiceInternal
                .ExecuteArchiveEntityLanguageVersions<StatutoryServiceGeneralDescriptionVersioned,
                    GeneralDescriptionLanguageAvailability>(unitOfWork, model.GeneralDescriptions,
                    HistoryAction.ArchivedViaScheduling,
                    FilterArchiveAction<StatutoryServiceGeneralDescriptionVersioned>(generalDescriptionService
                        .OnDeletingGeneralDescription));
            commonServiceInternal
                .ExecuteArchiveEntityLanguageVersions<OrganizationVersioned, OrganizationLanguageAvailability>(
                    unitOfWork, model.Organizations, HistoryAction.ArchivedViaScheduling,
                    FilterArchiveAction<OrganizationVersioned>((uOW, id) =>
                        organizationService.CascadeDeleteOrganization(uOW, id, HistoryAction.ScheduledArchive)));
            commonServiceInternal
                .ExecuteArchiveEntityLanguageVersions<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>
                    (unitOfWork, model.ServiceCollections, HistoryAction.ArchivedViaScheduling);

            if (allowAnonymous)
            {
                AddPreSaveEntityProcessingForArchiving<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork);
                AddPreSaveEntityProcessingForArchiving<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork);
                AddPreSaveEntityProcessingForArchiving<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(unitOfWork);
                AddPreSaveEntityProcessingForArchiving<OrganizationVersioned, OrganizationLanguageAvailability>(unitOfWork);
                AddPreSaveEntityProcessingForArchiving<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(unitOfWork);
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
        /// <param name="model"></param>
        public IMessage CopyEntities(VmMassDataModel<VmCopyingModel> model)
        {
            CopyValidation(model);

            return contextManager.ExecuteWriter((unitOfWork) =>
            {
                var serviceIds = ExecuteCopyServices(unitOfWork, commonService, model.Services, model.OrganizationId);
                var channelIds = commonService.ExecuteCopyEntities<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(unitOfWork, model.Channels.Select(x => x.Id).ToList(), model.OrganizationId);
                ExecuteCopyServiceCollectionWithConnectedServices(unitOfWork, commonService, model.ServiceCollections.Select(x => x.Id).ToList(), model.OrganizationId);
                unitOfWork.Save();
                
                expirationService.SetExpirationDatesForDraft<ServiceVersioned>(unitOfWork, serviceIds, true);
                expirationService.SetExpirationDatesForDraft<ServiceChannelVersioned>(unitOfWork, channelIds, true);

                return new VmMassToolProcessMessage(model.Id, String.Format(MassToolFinnishedSuccessfully, MassToolType.Copy), null);
            });
        }

        private void CopyValidation(VmMassDataModel<VmCopyingModel> model)
        {
            if (!model.OrganizationId.IsAssigned())
            {
                throw new PtvMandatoryOrganizationValidationException("");
            }
            BaseValidation(model, maxLanguageVersionCount: massToolConfiguration.MaxCopyLanguageVersions);
        }

        /// <summary>
        /// Execute copy of services, If exist self service provider is organization changed by selected organization
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="commonServiceInternal"></param>
        /// <param name="entities"></param>
        /// <param name="organizationId"></param>
        private IEnumerable<Guid> ExecuteCopyServices(IUnitOfWorkWritable unitOfWork, ICommonServiceInternal commonServiceInternal, IReadOnlyList<VmCopyingModel> entities, Guid organizationId)
        {
            foreach (var unificRootId in entities.Select(x => x.UnificRootId).Distinct())
            {
                var rep = unitOfWork.CreateRepository<IRepository<ServiceVersioned>>();
                var versions = rep.All().Where(x => x.UnificRootId == unificRootId);

                var lastPublishedDraftModifiedEntity = VersioningManager.ApplyPublishingStatusFilterFallback(versions);

                if (lastPublishedDraftModifiedEntity != null)
                {
                    var copiedEntity = VersioningManager.CreateCopyVersion<ServiceVersioned, Service, ServiceLanguageAvailability>
                    (
                        unitOfWork,
                        lastPublishedDraftModifiedEntity,
                        PublishingStatus.Draft
                    );

                    var selfProducedId = typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString());
                    if (copiedEntity.ServiceProducers.Any(x =>
                        x.ProvisionTypeId == selfProducedId &&
                        x.Organizations.Any(y => y.OrganizationId == lastPublishedDraftModifiedEntity.OrganizationId)
                    ))
                    {
                        var serviceOrganizationProducers = copiedEntity.ServiceProducers
                            .Where(f => f.ProvisionTypeId == selfProducedId).SelectMany(x =>
                                x.Organizations
                                    .Where(spo => spo.OrganizationId == lastPublishedDraftModifiedEntity.OrganizationId)
                                    .Select(y => y));

                        serviceOrganizationProducers.ForEach(x => x.OrganizationId = organizationId);
                    }

                    var copyTemplate = new VmCopyTemplate
                    {
                        TemplateId = lastPublishedDraftModifiedEntity.Id,
                        TemplateOrganizationId = lastPublishedDraftModifiedEntity.OrganizationId
                    };

                    //copy selected
                    commonServiceInternal.FinalizeCopyEntity<ServiceVersioned, ServiceLanguageAvailability>(
                        copiedEntity, copyTemplate, organizationId);
                    
                    unitOfWork.Save(SaveMode.AllowAnonymous);
                    yield return copiedEntity.Id;
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
                var unificRootId = VersioningManager.GetUnificRootId<ServiceCollectionVersioned>(unitOfWork, entityVersionedId);

                if (unificRootId.HasValue)
                {
                    var rep = unitOfWork.CreateRepository<IRepository<ServiceCollectionVersioned>>();
                    var versions = rep.All().Where(x => x.UnificRootId == unificRootId);

                    var lastPublishedDraftModifiedEntity = VersioningManager.ApplyPublishingStatusFilterFallback(
                        unitOfWork.ApplyIncludes(versions, q =>
                            q.Include(i => i.UnificRoot)
                                .ThenInclude(i => i.ServiceCollectionServices)));

                    if (lastPublishedDraftModifiedEntity != null)
                    {
                        var copiedEntity = VersioningManager.CreateCopyVersion<ServiceCollectionVersioned, ServiceCollection, ServiceCollectionLanguageAvailability>(unitOfWork, lastPublishedDraftModifiedEntity, PublishingStatus.Draft);

                        copiedEntity.UnificRoot.ServiceCollectionServices = CopyServiceCollectionServices(unitOfWork, lastPublishedDraftModifiedEntity.UnificRoot?.ServiceCollectionServices,
                            copiedEntity.UnificRootId).ToList();

                        var copyTemplate = new VmCopyTemplate
                        {
                            TemplateId = lastPublishedDraftModifiedEntity.Id,
                            TemplateOrganizationId = lastPublishedDraftModifiedEntity.OrganizationId
                        };

                        commonServiceInternal.FinalizeCopyEntity<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(copiedEntity, copyTemplate, organizationId);
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
                var serviceCollectionService = new PTV.Database.Model.Models.ServiceCollectionService
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

        /// <summary>
        /// Restoring selected entities by ids
        /// </summary>
        /// <param name="model"></param>
        public IMessage RestoreEntities(VmMassDataModel<VmRestoringModel> model)
        {
            RestoreValidation(model);

            return contextManager.ExecuteWriter((unitOfWork) =>
            {
                var serviceIds = commonService.ExecuteRestoreEntities<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, model.Services.Select(x => x.Id).ToList());
                var channelIds = commonService.ExecuteRestoreEntities<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, model.Channels.Select(x => x.Id).ToList());
                commonService.ExecuteRestoreEntities<ServiceCollectionVersioned, ServiceCollectionLanguageAvailability>(unitOfWork, model.ServiceCollections.Select(x => x.Id).ToList());

                unitOfWork.Save();
                
                expirationService.SetExpirationDatesForDraft<ServiceVersioned>(unitOfWork, serviceIds, true);
                expirationService.SetExpirationDatesForDraft<ServiceChannelVersioned>(unitOfWork, channelIds, true);

                return new VmMassToolProcessMessage(model.Id, String.Format(MassToolFinnishedSuccessfully ,MassToolType.Restore), null);
            });
        }

        private void RestoreValidation(VmMassDataModel<VmRestoringModel> model)
        {
            BaseValidation(model, maxLanguageVersionCount: massToolConfiguration.MaxRestoreLanguageVersions);
        }

        #region TaskScheduler

        public void PublishScheduledLanguageVersions(DateTime dateTime, VmJobLogEntry logInfo)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var model = GetLanguageVersionsForPublish(unitOfWork, dateTime);
                try
                {
                    logger.LogSchedulerInfo(logInfo, $"MASS Publishing date: {dateTime.Date:u} {dateTime:u}, services: {model.Services.Count}, channels: {model.Channels.Count}, organization: {model.Organizations.Count}.");
                    PublishMassLanguageAvailabilities(unitOfWork, commonService, model, allowAnonymous: true);
                }
                catch (PtvEntitiesValidationException e)
                {
                    logger.LogSchedulerWarn(logInfo, $"MASS TOOL WARNING - Scheduled Publishing not published entities: {string.Join(", ", e.NotPublishedEntities.Select(x => x.ToString()))}");
                }
                catch (Exception e)
                {
                    logger.LogSchedulerError(logInfo, $"MASS TOOL ERROR - Scheduled Publishing failed with exception:{e.GetType()} message:{e.Message} details:{CoreExtensions.ExtractAllInnerExceptions(e)}");
                }
            });
        }

        private IVmMassDataModel<VmPublishingModel> GetLanguageVersionsForPublish(IUnitOfWork unitOfWork, DateTime startDate)
        {
            var model = new VmMassDataModel<VmPublishingModel>
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
        public void ArchiveScheduledLanguageVersions(DateTime dateTime, VmJobLogEntry logInfo)
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
                    logger.LogSchedulerError(logInfo, $"MASS TOOL ERROR - Scheduled Archiving failed with exception:{e.GetType()} message:{e.Message} details:{CoreExtensions.ExtractAllInnerExceptions(e)}");
                }
            });
        }

        private VmMassDataModel<VmArchivingModel> GetLanguageVersionsForArchiving(IUnitOfWork unitOfWork, DateTime? archivingDate)
        {
            archivingDate = archivingDate ?? DateTime.UtcNow;

            var model = new VmMassDataModel<VmArchivingModel>
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

        private void AddPreSaveEntityProcessingForPublishing<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork)
            where TEntity : class, IMultilanguagedEntity<TLanguageAvail>, IVersionedVolume, IAuditing
            where TLanguageAvail : class, ILanguageAvailability
        {
            unitOfWork.AddPreSaveEntityProcessing<TLanguageAvail>((langEntity, entityState) =>
            {
                langEntity.ModifiedBy = langEntity.ReviewedBy;
            });
            unitOfWork.AddPreSaveEntityProcessing<TEntity>((entity, entityState) =>
            {
                var reviewer = entity.LanguageAvailabilities
                    .Where(la => !string.IsNullOrEmpty(la.ReviewedBy))
                    .OrderByDescending(la => la.Reviewed)
                    .FirstOrDefault()?.ReviewedBy;
                entity.ModifiedBy = reviewer;
                entity.Versioning.CreatedBy = reviewer;
                entity.Versioning.ModifiedBy = reviewer;
            });
        }

        private static void AddPreSaveEntityProcessingForArchiving<TEntity, TLanguageAvail>(IUnitOfWorkWritable unitOfWork)
            where TEntity : class, IMultilanguagedEntity<TLanguageAvail>, IVersionedVolume, IAuditing
            where TLanguageAvail : class, ILanguageAvailability
        {
            unitOfWork.AddPreSaveEntityProcessing<TLanguageAvail>((langEntity, entityState) =>
            {
                langEntity.ModifiedBy = langEntity.SetForArchivedBy;
            });
            unitOfWork.AddPreSaveEntityProcessing<TEntity>((entity, entityState) =>
            {
                var archiver = entity.LanguageAvailabilities
                    .Where(la => !string.IsNullOrEmpty(la.SetForArchivedBy))
                    .OrderByDescending(la => la.SetForArchived)
                    .FirstOrDefault()?.SetForArchivedBy;
                entity.ModifiedBy = archiver;
                entity.Versioning.CreatedBy = archiver;
                entity.Versioning.ModifiedBy = archiver;
            });
        }
    }
}
