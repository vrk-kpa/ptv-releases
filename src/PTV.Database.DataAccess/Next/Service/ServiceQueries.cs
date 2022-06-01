using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.DataAccess.Next.Organization;
using PTV.Database.DataAccess.Next.ServiceChannels;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Next.Model;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common.Connections;

namespace PTV.Database.DataAccess.Next.Service
{
    [RegisterService(typeof(IServiceQueries), RegisterType.Transient)]
    internal class ServiceQueries : IServiceQueries
    {
        private readonly IContextManager contextManager;
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;
        private readonly IServiceService service;
        private readonly IChannelsConnectedToServiceQuery channelQuery;
        private readonly IEntityHistoryService entityHistoryService;
        private readonly IGeneralDescriptionQueries generalDescriptionQueries;
        private readonly IOrganizationMapper organizationMapper;
        private readonly TranslationAvailabilityMapper translationAvailabilityMapper;

        public ServiceQueries(IContextManager contextManager, 
            ILanguageCache languageCache, 
            ITypesCache typesCache, 
            IServiceService service,
            IChannelsConnectedToServiceQuery channelQuery,
            IEntityHistoryService entityHistoryService,
            IGeneralDescriptionQueries generalDescriptionQueries,
            IValidationManager validationManager,
            IOrganizationMapper organizationMapper)
        {
            this.contextManager = contextManager;
            this.languageCache = languageCache;
            this.typesCache = typesCache;
            this.service = service;
            this.channelQuery = channelQuery;
            this.entityHistoryService = entityHistoryService;
            this.generalDescriptionQueries = generalDescriptionQueries;
            this.organizationMapper = organizationMapper;
            translationAvailabilityMapper = new TranslationAvailabilityMapper(languageCache, typesCache, validationManager);
        }

        public ServiceModel GetModel(Guid versionedId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceVersionedRepo = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var basicInfo = serviceVersionedRepo.All()
                    .Include(x => x.UnificRoot)
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x => x.ServiceNames)
                    .Include(x => x.ServiceDescriptions)
                    .Include(x => x.ServiceLanguages)
                    .Include(x => x.ServiceRequirements)
                    .Include(x => x.OrganizationServices)
                    .Include(x => x.ServiceKeywords).ThenInclude(x => x.Keyword)
                    .Include(x => x.Versioning)
                    .FirstOrDefault(x => x.Id == versionedId);

                if (basicInfo == null)
                {
                    return null;
                }

                var serviceWithAreas = serviceVersionedRepo.All()
                    .Include(x => x.AreaMunicipalities)
                    .Include(x => x.Areas).ThenInclude(x => x.Area)
                    .FirstOrDefault(x => x.Id == versionedId);

                var serviceWithClassifications = serviceVersionedRepo.All()
                    .Include(x => x.ServiceTargetGroups).ThenInclude(x => x.TargetGroup)
                    .Include(x => x.ServiceIndustrialClasses)
                    .Include(x => x.ServiceServiceClasses)
                    .Include(x => x.ServiceLifeEvents)
                    .Include(x => x.ServiceOntologyTerms).ThenInclude(x => x.OntologyTerm).ThenInclude(x => x.Names)
                    .FirstOrDefault(x => x.Id == versionedId);

                var lawRepo = unitOfWork.CreateRepository<IServiceLawRepository>();
                var laws = lawRepo.All()
                    .Include(x => x.Law).ThenInclude(x => x.Names)
                    .Include(x => x.Law).ThenInclude(x => x.WebPages).ThenInclude(x => x.WebPage)
                    .Where(x => x.ServiceVersionedId == versionedId)
                    .Select(x => x.Law)
                    .Distinct()
                    .ToList();

                var voucherRepo = unitOfWork.CreateRepository<IServiceWebPageRepository>();
                var vouchers = voucherRepo.All()
                    .Include(x => x.WebPage)
                    .Where(x => x.ServiceVersionedId == versionedId)
                    .Distinct()
                    .ToList();
                
                var serviceProducerRepo = unitOfWork.CreateRepository<IServiceProducerRepository>();
                var serviceProducers = serviceProducerRepo.All()
                    .Include(x => x.Organizations)
                    .Include(x => x.AdditionalInformations)
                    .Where(x => x.ServiceVersionedId == versionedId)
                    .Distinct()
                    .ToList();

                var translationOrderRepo = unitOfWork.CreateRepository<ITranslationOrderStateRepository>();
                var latestTranslationOrders = translationOrderRepo.All()
                    .Include(x => x.TranslationOrder)
                    .Where(x => x.TranslationOrder.ServiceTranslationOrders.Any(y =>
                        y.ServiceId == basicInfo.UnificRootId) && x.Last)
                    .ToList();

                var result = basicInfo.ToModel(languageCache, typesCache, organizationMapper)
                    .FillInCategorization(serviceWithClassifications, languageCache)
                    .FillInAreas(serviceWithAreas, typesCache)
                    .FillInServiceProviders(serviceProducers, typesCache, languageCache, organizationMapper)
                    .FillInAdditionalInfo(laws, vouchers, languageCache);

                if (basicInfo.StatutoryServiceGeneralDescriptionId.HasValue)
                {
                    result.GeneralDescription = generalDescriptionQueries.GetPublishedByUnificRootId(basicInfo
                        .StatutoryServiceGeneralDescriptionId.Value);
                }

                result = translationAvailabilityMapper.FillInTranslationOrders<ServiceVersioned>(result,
                    latestTranslationOrders, unitOfWork);

                result.ConnectedChannels = channelQuery.GetChannelsConnectedToService(basicInfo.UnificRootId);
                FillInOtherVersionInfo(result, unitOfWork);
                return result;
            });
        }

        private void FillInOtherVersionInfo(ServiceModel input, IUnitOfWork unitOfWork)
        {
            if (!input.Id.IsAssigned() || !input.UnificRootId.IsAssigned())
            {
                return;
            }

            var repo = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var publishedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());

            if (input.Status != PublishingStatus.Published)
            {
                var data = repo.All()
                    .Where(x => x.UnificRootId == input.UnificRootId && x.PublishingStatusId == publishedStatusId)
                    .Select(x => new {x.Id, x.Modified})
                    .FirstOrDefault();
                input.OtherPublishedVersion =
                    data == null ? null : new OtherVersionModel {Id = data.Id, Modified = data.Modified};
            }

            if (input.Status != PublishingStatus.Draft && input.Status != PublishingStatus.Modified)
            {
                var data = repo.All()
                    .Where(x => x.UnificRootId == input.UnificRootId && (x.PublishingStatusId == modifiedStatusId ||
                                                                         x.PublishingStatusId == draftStatusId))
                    .Select(x => new {x.Id, x.Modified}).FirstOrDefault();
                input.OtherModifiedVersion =
                    data == null ? null : new OtherVersionModel {Id = data.Id, Modified = data.Modified};
            }
        }

        public ValidationModel<ServiceModel> Validate(Guid id)
        {
            var validationResult = service.GetValidatedEntitySimple(id);
            return new ValidationModel<ServiceModel>
            {
                ValidatedFields = validationResult.ToDictionary(x => x.Key.ToEnum<LanguageEnum>(), x => x.Value),
                Entity = GetModel(id)
            };
        }

        public InfiniteModel<EntityHistoryModel> GetEditHistory(Guid id, int page)
        {
            var versioningId = contextManager.ExecuteReader(unitOfWork =>
                unitOfWork.CreateRepository<IServiceVersionedRepository>().All().First(x => x.Id == id)?.VersioningId);
                
            var oldModel = entityHistoryService.GetServiceEntityHistory(new VmHistorySearch
            {
                Id = versioningId,
                PageNumber = page
            }) as VmSearchResult<VmEntityOperation>;
            return oldModel.ToNewModel(typesCache);
        }

        public InfiniteModel<ConnectionHistoryModel> GetConnectionHistory(Guid id, int page)
        {
            var unificRootId = contextManager.ExecuteReader(unitOfWork =>
                unitOfWork.CreateRepository<IServiceVersionedRepository>().All().First(x => x.Id == id)?.UnificRootId);

            var oldModel = entityHistoryService.GetServiceConnectionHistory(new VmHistorySearch
            {
                Id = unificRootId,
                PageNumber = page
            }) as VmSearchResult<VmConnectionOperation>;
            return oldModel.ToNewModel(typesCache);
        }

        public PublishingStatus? GetLanguageVersionPublishingStatus(Guid id, LanguageEnum lang)
        {
            var status = contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceRepository = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var versioned = serviceRepository.All().Include(x => x.LanguageAvailabilities)
                    .ThenInclude(y => y.Language)
                    .Include(x => x.LanguageAvailabilities)
                    .ThenInclude(y => y.Status)
                    .First(serviceVersioned => serviceVersioned.Id == id);
                return versioned.LanguageAvailabilities.First(x => x.Language.Code == lang.ToString())?.StatusId;
            });

            return status.HasValue ? typesCache.GetByValue<PublishingStatusType>(status.Value).ToEnum<PublishingStatus>() : null as PublishingStatus?;
        }

        public PublishingStatus? GetServicePublishingStatus(Guid id)
        {
            var status = contextManager.ExecuteReader(unitOfWork =>
            {
                var serviceRepository = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var versioned = serviceRepository.All().Include(x => x.LanguageAvailabilities)
                    .ThenInclude(y => y.Language)
                    .Include(x => x.LanguageAvailabilities)
                    .ThenInclude(y => y.Status)
                    .FirstOrDefault(serviceVersioned => serviceVersioned.Id == id);
                return versioned?.PublishingStatusId;
            });

            return status.HasValue ? typesCache.GetByValue<PublishingStatusType>(status.Value).ToEnum<PublishingStatus>() : null as PublishingStatus?;
        }
    }
}
