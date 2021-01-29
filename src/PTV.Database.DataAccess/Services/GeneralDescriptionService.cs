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
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.DataAccess.Services.V2;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.OpenApi.V6;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Logic;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Framework.Extensions;
using PTV.Database.DataAccess.Utils.OpenApi;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IGeneralDescriptionService), RegisterType.Transient)]
    internal class GeneralDescriptionService : EntityServiceBase<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>, IGeneralDescriptionService
    {
        private readonly ITranslationEntity translationManager;
        private readonly IContextManager contextManager;
        private readonly ILogger logger;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private IServiceUtilities utilities;
        private ITranslationService translationService;
        private ICommonServiceInternal commonService;
        private ILanguageOrderCache languageOrderCache;
        private IPahaTokenAccessor pahaTokenAccessor;
        private readonly IResolveManager resolveManager;
        private IGeneralDescriptionServiceInternal generalDescriptionServiceInternal;
        private readonly IUrlService urlService;
        private readonly ISearchServiceInternal searchService;

        public GeneralDescriptionService(
            IContextManager contextManager,
            ITranslationEntity translationManager,
            ITranslationViewModel translationManagerVModel,
            ILogger<GeneralDescriptionService> logger,
            IServiceUtilities utilities,
            ICommonServiceInternal commonService,
            ITranslationService translationService,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            ILanguageCache languageCache,
            ITypesCache typesCache,
            IVersioningManager versioningManager,
            IValidationManager validationManager,
            ILanguageOrderCache languageOrderCache,
            IResolveManager resolveManager,
            IPahaTokenAccessor pahaTokenAccessor,
            IGeneralDescriptionServiceInternal generalDescriptionServiceInternal,
            IUrlService urlService,
            ISearchServiceInternal searchService)
            : base(translationManager, translationManagerVModel, publishingStatusCache, userOrganizationChecker,
                contextManager, utilities, commonService, validationManager, versioningManager)
        {
            this.translationManager = translationManager;
            this.contextManager = contextManager;
            this.logger = logger;
            this.typesCache = typesCache;
            this.languageCache = languageCache;
            this.utilities = utilities;
            this.translationService = translationService;
            this.commonService = commonService;
            this.languageOrderCache = languageOrderCache;
            this.pahaTokenAccessor = pahaTokenAccessor;
            this.generalDescriptionServiceInternal = generalDescriptionServiceInternal;
            this.resolveManager = resolveManager;
            this.urlService = urlService;
            this.searchService = searchService;
        }

        public IVmSearchBase SearchServiceGeneralDescriptions(VmGeneralDescriptionSearchForm searchData)
        {
            var name = searchData.Name != null ? Regex.Replace(searchData.Name.ToLower().Trim(), @"\s+", " ") : String.Empty;
            var rootId = GetRootIdFromString(name);
            var gdTypes = typesCache.GetCacheData<GeneralDescriptionType>();
            var restrictedTypes = new List<Guid>();

            if (pahaTokenAccessor.UserRole != UserRoleEnum.Eeva)
            {
                using (var scope = resolveManager.CreateScope())
                {
                    var restrictionFilterManager = scope.ServiceProvider.GetService<IRestrictionFilterManager>();
                    restrictedTypes = restrictionFilterManager
                        .SetAccessForGuidTypes<GeneralDescriptionType>(pahaTokenAccessor.ActiveOrganizationId, gdTypes)
                        .Where(i => i.Access == EVmRestrictionFilterType.Denied).Select(i => i.Id).ToList();
                }
            }
            
            var publishedStatusId = PublishingStatusCache.Get(PublishingStatus.Published);
            
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var rs = searchService.SearchServiceGeneralDescriptionEntities(new VmEntitySearch
                {
                    SortData = searchData.SortData,
                    MaxPageCount = searchData.MaxPageCount,
                    PageNumber = searchData.PageNumber.PositiveOrZero(),
                    Skip = searchData.Skip,
                    Name = name,
                    Id = rootId, 
                    RestrictedTypes = restrictedTypes,
                    SelectedPublishingStatuses = new VmListItemsData<Guid>{publishedStatusId},
                    SearchType = SearchTypeEnum.Name,
                    Language = searchData.Languages.First(),
                    ContentTypes = new List<SearchEntityTypeEnum> {SearchEntityTypeEnum.GeneralDescription},
                    ServiceTypes = searchData.ServiceTypeId.HasValue ? new List<Guid>{searchData.ServiceTypeId.Value} : null,
                    GeneralDescriptionTypes = searchData.GeneralDescriptionTypeId.HasValue ? new List<Guid>{searchData.GeneralDescriptionTypeId.Value} : null
                }, unitOfWork) as VmSearchResult<VmServiceGeneralDescriptionListItem>;
        
                return rs;
            });
        }

        public VmGeneralDescriptionOutput GetGeneralDescription(VmGeneralDescriptionGet model)
        {
            return ExecuteGet(model, (unitOfWork, vm) => GetGeneralDescription(unitOfWork, model));
        }

        public VmGeneralDescriptionOutput GetGeneralDescription(IUnitOfWork unitOfWork, VmGeneralDescriptionGet model)
        {
            var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            var psPublished = PublishingStatusCache.Get(PublishingStatus.Published);
            var psDraft = PublishingStatusCache.Get(PublishingStatus.Draft);
            var psModified = PublishingStatusCache.Get(PublishingStatus.Modified);
            var description = model.OnlyPublished ?
                statutoryGeneralDescriptionRep.All().Where(x => x.UnificRootId == model.Id && x.PublishingStatusId == psPublished)
                : statutoryGeneralDescriptionRep.All().Where(x => x.Id == model.Id);

            var generalDescription = unitOfWork.ApplyIncludes(description, i => i
                .Include(j => j.UnificRoot).ThenInclude(j => j.StatutoryServiceGeneralDescriptionServiceChannels)
                .Include(j => j.Names)
                .Include(x => x.TargetGroups)
                .Include(j => j.Descriptions)
                .Include(x => x.LanguageAvailabilities)
                .Include(x => x.Versioning)
                .Include(j => j.StatutoryServiceRequirements)
            ).FirstOrDefault();

            if (generalDescription == null)
            {
                return new VmGeneralDescriptionOutput();
            }

            IncludeClassifications(unitOfWork, generalDescription);
            IncludeLegislation(unitOfWork, generalDescription);
            var result = translationManager.Translate<StatutoryServiceGeneralDescriptionVersioned, Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionOutput>(generalDescription);
            result.PreviousInfo = result.Id.HasValue ? Utilities.GetEntityEditableInfo<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(result.Id.Value, unitOfWork) : null;
            result.TranslationAvailability = translationService.GetGeneralDescriptionTranslationAvailabilities(unitOfWork, result.Id.Value, result.UnificRootId);

            var searchChannelIds = generalDescription.UnificRoot.StatutoryServiceGeneralDescriptionServiceChannels.Select(i => i.ServiceChannelId).ToList();

            var channelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var channelGroups = channelRep.All()
                .Where(i => searchChannelIds.Contains(i.UnificRootId)
                            && (i.PublishingStatusId == psPublished || i.PublishingStatusId == psModified || i.PublishingStatusId == psDraft)).Include(i => i.UnificRoot)
                .Include(j => j.ServiceChannelNames)
                .Include(j => j.DisplayNameTypes)
                .Include(j => j.LanguageAvailabilities)
                .ToList().GroupBy(i => i.UnificRootId);

            generalDescription.UnificRoot.StatutoryServiceGeneralDescriptionServiceChannels.ForEach(i => { i.ServiceChannel = channelGroups.FirstOrDefault(j => j.Key == i.ServiceChannelId)?.FirstOrDefault()?.UnificRoot; });
            generalDescription.UnificRoot.StatutoryServiceGeneralDescriptionServiceChannels = generalDescription.UnificRoot.StatutoryServiceGeneralDescriptionServiceChannels.Where(i => i.ServiceChannel?.Versions != null).ToList();
            result.Connections = translationManager.TranslateAll<GeneralDescriptionServiceChannel, VmConnectionOutput>(generalDescription.UnificRoot.StatutoryServiceGeneralDescriptionServiceChannels).ToList();

            if (model.OnlyPublished)
            {
                result.Id = result.UnificRootId;
            }

            return result;
        }

        private void IncludeLegislation(IUnitOfWork unitOfWork, StatutoryServiceGeneralDescriptionVersioned entity)
        {
            if (unitOfWork == null || entity == null)
            {
                return;
            }

            var gdLawRepo = unitOfWork.CreateRepository<IStatutoryServiceLawRepository>();
            var laws = gdLawRepo.All()
                .Include(j => j.Law).ThenInclude(j => j.Names)
                .Include(j => j.Law).ThenInclude(j => j.WebPages).ThenInclude(j => j.WebPage)
                .Include(j => j.Law).ThenInclude(j => j.WebPages).ThenInclude(j => j.Localization)
                .Where(j => j.StatutoryServiceGeneralDescriptionVersionedId == entity.Id)
                .ToList();

            entity.StatutoryServiceLaws = laws;
        }

        private void IncludeClassifications(IUnitOfWork unitOfWork, StatutoryServiceGeneralDescriptionVersioned entity)
        {
            if (unitOfWork == null || entity == null)
            {
                return;
            }

            var serviceClassRepo = unitOfWork.CreateRepository<IStatutoryServiceServiceClassRepository>();
            var ontologyTermRepo = unitOfWork.CreateRepository<IStatutoryServiceOntologyTermRepository>();
            var lifeEventRepo = unitOfWork.CreateRepository<IStatutoryServiceLifeEventRepository>();
            var industrialClassRepo = unitOfWork.CreateRepository<IStatutoryServiceIndustrialClassRepository>();
            var keywordsRepo = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionKeywordRepository>();

            var serviceClasses = serviceClassRepo.All()
                .Include(x => x.ServiceClass).ThenInclude(x => x.Names)
                .Where(x => x.StatutoryServiceGeneralDescriptionVersionedId == entity.Id)
                .ToList();
            var ontologies = ontologyTermRepo.All()
                .Include(x => x.OntologyTerm).ThenInclude(x => x.Names)
                .Where(x => x.StatutoryServiceGeneralDescriptionVersionedId == entity.Id)
                .ToList();
            var lifeEvents = lifeEventRepo.All()
                .Include(x => x.LifeEvent).ThenInclude(x => x.Names)
                .Where(x => x.StatutoryServiceGeneralDescriptionVersionedId == entity.Id)
                .ToList();
            var industrialClasses = industrialClassRepo.All()
                .Include(x => x.IndustrialClass).ThenInclude(x => x.Names)
                .Where(x => x.StatutoryServiceGeneralDescriptionVersionedId == entity.Id)
                .ToList();
            var keywords = keywordsRepo.All()
                .Include(x => x.Keyword)
                .Where(x => x.StatutoryServiceGeneralDescriptionVersionedId == entity.Id)
                .ToList();

            entity.ServiceClasses = serviceClasses;
            entity.OntologyTerms = ontologies;
            entity.LifeEvents = lifeEvents;
            entity.IndustrialClasses = industrialClasses;
            entity.Keywords = keywords;
        }

        public IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetGeneralDescriptions(DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null)
        {
            var handler = new V3GuidPagingHandler<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, StatutoryServiceName, GeneralDescriptionLanguageAvailability>
                (EntityStatusExtendedEnum.Published, date, dateBefore, PublishingStatusCache, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        public IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetNewGeneralDescriptions(int pageNumber = 1, int pageSize = 1000)
        {
            var handler = new NewGdsPagingHandler(PublishingStatusCache, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        public IList<IVmOpenApiGeneralDescriptionVersionBase> GetGeneralDescriptions(List<Guid> idList, int openApiVersion, bool showHeader = false)
        {
            if (idList == null || idList.Count == 0)
            {
                return null;
            }

            IList<IVmOpenApiGeneralDescriptionVersionBase> result = new List<IVmOpenApiGeneralDescriptionVersionBase>();
            contextManager.ExecuteReader(unitOfWork =>
            {
                try
                {
                    IList<Expression<Func<StatutoryServiceGeneralDescriptionVersioned, bool>>> filters = new List<Expression<Func<StatutoryServiceGeneralDescriptionVersioned, bool>>>
                    {
                        c => idList.Contains(c.UnificRootId)
                    };
                    result = GetGeneralDescriptionsWithDetails(unitOfWork, filters, openApiVersion, showHeader);
                }
                catch (Exception ex)
                {
                    var errorMsg = string.Format("Error occured while getting general descriptions. {0}", ex.Message);
                    logger.LogError(errorMsg + " " + ex.StackTrace);
                    throw new Exception(errorMsg);
                }
            });
            return result;
        }

        public IList<VmOpenApiGeneralDescriptionVersionBase> GetGeneralDescriptionsSimple(List<Guid> idList)
        {
            if (idList == null || idList.Count == 0)
            {
                return null;
            }

            IList<VmOpenApiGeneralDescriptionVersionBase> result = new List<VmOpenApiGeneralDescriptionVersionBase>();
            contextManager.ExecuteReader(unitOfWork =>
            {
                try
                {
                    var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                    var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
                    // Get only published general description that exist within the id list.
                    var gds = statutoryGeneralDescriptionRep.All()
                        .Where(gd => idList.Contains(gd.UnificRootId) &&
                        gd.PublishingStatusId == publishedId && gd.LanguageAvailabilities.Any(l => l.StatusId == publishedId))
                        .ToList();

                    if (gds?.Count > 0)
                    {
                        IncludeClassification(unitOfWork, gds);

                        gds.ForEach(gd =>
                        {
                            FilterOutNotPublishedLanguageVersions(gd, publishedId);
                        });

                        result = translationManager.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(gds).ToList();
                    }
                }
                catch (Exception ex)
                {
                    var errorMsg = string.Format("Error occured while getting general descriptions. {0}", ex.Message);
                    logger.LogError(errorMsg + " " + ex.StackTrace);
                    throw new Exception(errorMsg);
                }
            });
            return result;
        }

        public IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionVersionBase(Guid id, int openApiVersion, bool getOnlyPublished = true, bool checkRestrictions = false, bool showHeader = false)
        {
            IVmOpenApiGeneralDescriptionVersionBase result = null;

            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    // Get the right version id
                    Guid? entityId = null;
                    if (getOnlyPublished)
                    {
                        entityId = VersioningManager.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, id, PublishingStatus.Published);
                    }
                    else
                    {
                        entityId = VersioningManager.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, id, null, false);
                    }

                    if (entityId.HasValue)
                    {
                        result = GetGeneralDescriptionWithDetails(unitOfWork, entityId.Value, openApiVersion, showHeader, getOnlyPublished);
                        // Check the restictions
                        if (checkRestrictions)
                        {
                            var restrictedTypes = commonService.GetRestrictedDescriptionTypes();
                            if (restrictedTypes.Contains(result.GeneralDescriptionTypeId))
                            {
                                result = null;
                            }
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting a general description with id {0}. {1}", id, ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }

            return result;
        }

        public IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionSimple(IUnitOfWork unitOfWork, Guid id)
        {
            // Get the right version id
            Guid? entityId = VersioningManager.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, id, PublishingStatus.Published);

            if (entityId.HasValue)
            {
                var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();

                var gd = statutoryGeneralDescriptionRep.All()
                    .FirstOrDefault(s => s.Id == entityId.Value);

                IncludeClassification(unitOfWork, new List<StatutoryServiceGeneralDescriptionVersioned> { gd });
                return translationManager.Translate<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(gd);
            }

            return null;
        }

        public IVmOpenApiGeneralDescriptionVersionBase GetPublishedGeneralDescriptionWithDetails(IUnitOfWork unitOfWork, Guid id, bool showHeader)
        {
            // Get the right version id
            Guid? entityId = VersioningManager.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, id, PublishingStatus.Published);

            if (entityId.HasValue)
            {
                // Get the general description with details - do not get the related service channels though.
                return GetGeneralDescriptionWithDetails(unitOfWork, entityId.Value, 0, showHeader,true, false );
            }

            return null;
        }

        public IList<VmOpenApiGeneralDescriptionVersionBase> GetPublishedGeneralDescriptionsWithDetails(List<Guid> idList, int openApiVersion, bool showHeader)
        {
            if (idList == null || idList.Count == 0)
            {
                return null;
            }

            IList<VmOpenApiGeneralDescriptionVersionBase> result = null;

            contextManager.ExecuteReader(unitOfWork =>
            {
                var filters = new List<Expression<Func<StatutoryServiceGeneralDescriptionVersioned, bool>>>
                {
                    gd => idList.Contains(gd.UnificRootId)
                };
                result = GetGeneralDescriptionsWithDetail(unitOfWork, filters, openApiVersion, showHeader);
            });

            return result;
        }

        private void IncludeClassification(IUnitOfWork unitOfWork, IList<StatutoryServiceGeneralDescriptionVersioned> entities)
        {
            if (unitOfWork == null || entities == null || entities.Count == 0)
            {
                return;
            }

            var ids = entities.Select(e => e.Id).ToList();

            var serviceClassRepo = unitOfWork.CreateRepository<IStatutoryServiceServiceClassRepository>();
            var ontologyRepo = unitOfWork.CreateRepository<IStatutoryServiceOntologyTermRepository>();
            var lifeEventRepo = unitOfWork.CreateRepository<IStatutoryServiceLifeEventRepository>();
            var industrialClassRepo = unitOfWork.CreateRepository<IStatutoryServiceIndustrialClassRepository>();
            var targetGroupRepo = unitOfWork.CreateRepository<IStatutoryServiceTargetGroupRepository>();
            var keywordsRepo = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionKeywordRepository>();

            var serviceClasses = serviceClassRepo.All()
                .Include(x => x.ServiceClass).ThenInclude(x => x.Names)
                .Include(x => x.ServiceClass).ThenInclude(x => x.Descriptions)
                .Where(x => ids.Contains(x.StatutoryServiceGeneralDescriptionVersionedId))
                .ToList();
            var ontologies = ontologyRepo.All()
                .Include(x => x.OntologyTerm).ThenInclude(x => x.Names)
                .Where(x => ids.Contains(x.StatutoryServiceGeneralDescriptionVersionedId))
                .ToList();
            var lifeEvents = lifeEventRepo.All()
                .Include(x => x.LifeEvent).ThenInclude(x => x.Names)
                .Where(x => ids.Contains(x.StatutoryServiceGeneralDescriptionVersionedId))
                .ToList();
            var industrialClasses = industrialClassRepo.All()
                .Include(x => x.IndustrialClass).ThenInclude(x => x.Names)
                .Where(x => ids.Contains(x.StatutoryServiceGeneralDescriptionVersionedId))
                .ToList();
            var targetGroups = targetGroupRepo.All()
                .Include(x => x.TargetGroup).ThenInclude(x => x.Names)
                .Where(x => ids.Contains(x.StatutoryServiceGeneralDescriptionVersionedId))
                .ToList();
            var keywords = keywordsRepo.All()
                .Include(x => x.Keyword)
                .Where(x => ids.Contains(x.StatutoryServiceGeneralDescriptionVersionedId))
                .ToList();

            entities.ForEach(entity =>
            {
                entity.ServiceClasses = serviceClasses.Where(k => k.StatutoryServiceGeneralDescriptionVersionedId == entity.Id).ToList();
                entity.OntologyTerms = ontologies.Where(k => k.StatutoryServiceGeneralDescriptionVersionedId == entity.Id).ToList();
                entity.LifeEvents = lifeEvents.Where(k => k.StatutoryServiceGeneralDescriptionVersionedId == entity.Id).ToList();
                entity.IndustrialClasses = industrialClasses.Where(k => k.StatutoryServiceGeneralDescriptionVersionedId == entity.Id).ToList();
                entity.TargetGroups = targetGroups.Where(k => k.StatutoryServiceGeneralDescriptionVersionedId == entity.Id).ToList();
                entity.Keywords = keywords.Where(k => k.StatutoryServiceGeneralDescriptionVersionedId == entity.Id).ToList();
            });
        }

        private void IncludeLegislation(IUnitOfWork unitOfWork, IList<StatutoryServiceGeneralDescriptionVersioned> entities)
        {
            if (unitOfWork == null || entities == null || entities.Count == 0)
            {
                return;
            }

            var ids = entities.Select(e => e.Id).ToList();


            var serviceLawRepo = unitOfWork.CreateRepository<IStatutoryServiceLawRepository>();
            var serviceLaws = serviceLawRepo.All()
                .Include(j => j.Law).ThenInclude(k => k.Names)
                .Include(j => j.Law).ThenInclude(k => k.WebPages).ThenInclude(l => l.Localization)
                .Include(j => j.Law).ThenInclude(k => k.WebPages).ThenInclude(l => l.WebPage)
                .Where(x => ids.Contains(x.StatutoryServiceGeneralDescriptionVersionedId))
                .ToList();

            entities.ForEach(entity => entity.StatutoryServiceLaws = serviceLaws.Where(k => k.StatutoryServiceGeneralDescriptionVersionedId == entity.Id).ToList());
        }

        private IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionWithDetails(Guid versionId, int openApiVersion, bool showHeader, bool getOnlyPublished = true)
        {
            IVmOpenApiGeneralDescriptionVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetGeneralDescriptionWithDetails(unitOfWork, versionId, openApiVersion, showHeader, getOnlyPublished);
            });
            return result;
        }

        private IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionWithDetails(IUnitOfWork unitOfWork, Guid versionId, int openApiVersion, bool showHeader, bool getOnlyPublished = true, bool getProposedChannels = true)
        {
            IVmOpenApiGeneralDescriptionVersionBase result = null;
            var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var query = unitOfWork.ApplyIncludes(statutoryGeneralDescriptionRep.All()
                .Where(s => s.Id == versionId), q =>
                q.Include(i => i.Languages).ThenInclude(i => i.Language)
                .Include(i => i.Names)
                .Include(i => i.Descriptions)
                .Include(i => i.StatutoryServiceRequirements)
                .Include(i => i.Type)
                .Include(i => i.ChargeType)
                .Include(i => i.LanguageAvailabilities)
                .Include(i => i.UnificRoot));

            var gd = query.FirstOrDefault();
            if (gd == null) return null;

            var gdList = new List<StatutoryServiceGeneralDescriptionVersioned> { gd };
            IncludeClassification(unitOfWork, gdList);
            IncludeLegislation(unitOfWork, gdList);

            if (getOnlyPublished)
            {
                FilterOutNotPublishedLanguageVersions(gd, publishedId);
            }

            translationManager.SetValue(openApiVersion, showHeader);
            result = translationManager.Translate<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(gd);

            if (result == null) return null;

            // Set connection related data outside of translator
            if (getProposedChannels)
            {
                var connectionRep = unitOfWork.CreateRepository<IGeneralDescriptionServiceChannelRepository>();
                var connectionQuery = connectionRep.All().Where(c => c.StatutoryServiceGeneralDescriptionId == gd.UnificRootId && c.ServiceChannel.Versions.Any(v => v.PublishingStatusId == publishedId && v.LanguageAvailabilities.Any(l => l.StatusId == publishedId)));
                var gdConnections = unitOfWork.ApplyIncludes(connectionQuery, q =>
                    q.Include(i => i.ServiceChannel)).ToList();
                if (gdConnections?.Count > 0)
                {
                    // Fill with service channel names
                    var channelRootIds = gdConnections.Select(s => s.ServiceChannelId).ToList();

                    var channels = unitOfWork.ApplyIncludes(
                        unitOfWork.CreateRepository<IServiceChannelVersionedRepository>().All().Where(i => channelRootIds.Contains(i.UnificRootId) && i.PublishingStatusId == publishedId),
                        q => q.Include(i => i.UnificRoot).Include(i => i.ServiceChannelNames).Include(i => i.LanguageAvailabilities)).ToList();

                    gdConnections.ForEach(gdsc =>
                    {
                        var channel = channels.Where(i => i.UnificRootId == gdsc.ServiceChannelId).FirstOrDefault();
                        if (channel != null)
                        {
                            gdsc.ServiceChannel = channel.UnificRoot;
                            result.ServiceChannels.Add(GetChannelConnectionData(gdsc, publishedId));
                        }
                    });
                }
            }

            // Get the right open api view model version
            return GetEntityByOpenApiVersion(result as IVmOpenApiGeneralDescriptionVersionBase, openApiVersion);
        }

        private IList<IVmOpenApiGeneralDescriptionVersionBase> GetGeneralDescriptionsWithDetails(IUnitOfWork unitOfWork, IList<Expression<Func<StatutoryServiceGeneralDescriptionVersioned, bool>>> filters, int openApiVersion, bool showHeader = false)
        {
            var result = GetGeneralDescriptionsWithDetail(unitOfWork, filters, openApiVersion, showHeader);

            if (result == null) return null;

            // Get the right open api view model version
            var versionList = new List<IVmOpenApiGeneralDescriptionVersionBase>();
            result.ForEach(gd => versionList.Add(GetEntityByOpenApiVersion(gd as IVmOpenApiGeneralDescriptionVersionBase, openApiVersion)));
            return versionList;
        }

        private IList<VmOpenApiGeneralDescriptionVersionBase> GetGeneralDescriptionsWithDetail(IUnitOfWork unitOfWork, IList<Expression<Func<StatutoryServiceGeneralDescriptionVersioned, bool>>> filters, int openApiVersion, bool showHeader)
        {
            IList<VmOpenApiGeneralDescriptionVersionBase> result = null;
            var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            // Get only published items -  filter out items that do not have any language versions published.
            filters.Add(e => e.PublishingStatusId == publishedId && e.LanguageAvailabilities.Any(l => l.StatusId == publishedId));

            var query = statutoryGeneralDescriptionRep.All();
            filters.ForEach(a => query = query.Where(a));

            var totalCount = query.Count();
            if (totalCount > 100)
            {
                throw new Exception(CoreMessages.OpenApi.TooManyItems);
            }
            if (totalCount == 0)
            {
                return null;
            }

            var gds = unitOfWork.ApplyIncludes(query, q =>
                q.Include(i => i.Languages).ThenInclude(i => i.Language)
                .Include(i => i.Names)
                .Include(i => i.Descriptions)
                .Include(i => i.StatutoryServiceRequirements)
                .Include(i => i.Type)
                .Include(i => i.ChargeType)
                .Include(i => i.LanguageAvailabilities)
                .Include(i => i.UnificRoot))
                .ToList();

            IncludeClassification(unitOfWork, gds);
            IncludeLegislation(unitOfWork, gds);

            gds.ForEach(gd =>
            {
                FilterOutNotPublishedLanguageVersions(gd, publishedId);
            });
            
            translationManager.SetValue(openApiVersion, showHeader);
            result = translationManager.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(gds).ToList();

            // Set connection related data outside of translator
            List<Guid> rootIds = result.Where(c => c.Id != null).Select(c => c.Id.Value).Distinct().ToList();
            var connectionRep = unitOfWork.CreateRepository<IGeneralDescriptionServiceChannelRepository>();
            var connectionQuery = connectionRep.All().Where(c => rootIds.Contains(c.StatutoryServiceGeneralDescriptionId) && c.ServiceChannel.Versions.Any(v => v.PublishingStatusId == publishedId && v.LanguageAvailabilities.Any(l => l.StatusId == publishedId)));
            var gdConnections = unitOfWork.ApplyIncludes(connectionQuery, q =>
                q.Include(i => i.ServiceChannel)).ToList();
            if (gdConnections?.Count > 0)
            {
                // Fill with service channel names
                var channelRootIds = gdConnections.Select(s => s.ServiceChannelId).ToList();

                var channels = unitOfWork.ApplyIncludes(
                    unitOfWork.CreateRepository<IServiceChannelVersionedRepository>().All().Where(i => channelRootIds.Contains(i.UnificRootId) && i.PublishingStatusId == publishedId),
                    q => q.Include(i => i.UnificRoot).Include(i => i.ServiceChannelNames)).ToList();

                gdConnections.ForEach(gdsc =>
                {
                    var channel = channels.Where(i => i.UnificRootId == gdsc.ServiceChannelId).FirstOrDefault();
                    if (channel != null)
                    {
                        var gd = result.Where(g => g.Id == gdsc.StatutoryServiceGeneralDescriptionId).FirstOrDefault();
                        if (gd != null)
                        {
                            gdsc.ServiceChannel = channel.UnificRoot;
                            gd.ServiceChannels.Add(GetChannelConnectionData(gdsc, publishedId));
                        }
                    }
                });
            }

            return result;
        }

        private V6VmOpenApiServiceServiceChannel GetChannelConnectionData(GeneralDescriptionServiceChannel gdsc, Guid publishedId)
        {
            if (gdsc == null) return null;

            var channel = gdsc.ServiceChannel.Versions.FirstOrDefault();
            var vm = new V6VmOpenApiServiceServiceChannel();
            vm.ServiceChannel.Id = channel.UnificRootId;
            // Get published name for service channel (PTV-3689).
            vm.ServiceChannel.Name = GetNameWithFallback(
                channel.ServiceChannelNames,
                channel.LanguageAvailabilities.Where(i => i.StatusId == publishedId).Select(i => i.LanguageId).ToList(),
                typesCache,
                languageCache);
            // ServiceChargeType values changed into new ones (PTV-2184)
            var type = gdsc.ChargeTypeId.IsAssigned() ? typesCache.GetByValue<ServiceChargeType>(gdsc.ChargeTypeId.Value) : null;
            vm.ServiceChargeType = string.IsNullOrEmpty(type) ? null : type.GetOpenApiEnumValue<ServiceChargeTypeEnum>();

            return vm;
        }

        private void FilterOutNotPublishedLanguageVersions(StatutoryServiceGeneralDescriptionVersioned gd, Guid publishedId)
        {
            var notPublishedLanguageVersions = gd.LanguageAvailabilities.Where(i => i.StatusId != publishedId).Select(i => i.LanguageId).ToList();
            if (notPublishedLanguageVersions.Count > 0)
            {
                gd.Names = gd.Names.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                gd.Descriptions = gd.Descriptions.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                gd.StatutoryServiceRequirements = gd.StatutoryServiceRequirements.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                gd.StatutoryServiceLaws.ForEach(law =>
                {
                    law.Law.Names = law.Law.Names.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    law.Law.WebPages = law.Law.WebPages.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                });
            }
        }

        public bool GeneralDescriptionExists(Guid id)
        {
            var result = false;

            contextManager.ExecuteReader(unitOfWork =>
            {
                var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionRepository>();
                if (statutoryGeneralDescriptionRep.All().FirstOrDefault(o => o.Id.Equals(id)) != null)
                {
                    result = true;
                }
            });
            return result;
        }

        public VmEntityHeaderBase PublishGeneralDescription(IVmLocalizedEntityModel model)
        {
            if (!model.Id.IsAssigned()) return null;
            Guid? generalDescriptionId = model.Id;
            var result = ContextManager.ExecuteWriter(unitOfWork =>
            {
                //Validate mandatory values
                var validationMessages = ValidationManager.CheckEntity<StatutoryServiceGeneralDescriptionVersioned>(generalDescriptionId.Value, unitOfWork, model);
                if (validationMessages.Any())
                {
                    throw new PtvValidationException(validationMessages, null);
                }

                //Publishing
                var affected = CommonService.PublishAndScheduleEntity<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(unitOfWork, model);

                //Update service names
                UpdateRelatedServiceNames(unitOfWork, model, affected);

                //update delivered translation
                var publishedStatusId = PublishingStatusCache.Get(PublishingStatus.Published);
                if (affected.Id.IsAssigned())
                {
                    foreach (var language in model.LanguagesAvailabilities.Where(x => x.StatusId == publishedStatusId))
                    {
                        translationService.ConfirmGeneralDescriptionDeliveredTranslation(unitOfWork, affected.Id, language.LanguageId, allowRemoveTrackingOrders: true);
                    }
                }

                return affected;
            });
            return ContextManager.ExecuteReader(unitOfWork => GetGeneralDescriptionHeader(result.Id, unitOfWork));
        }

        public VmEntityHeaderBase ScheduleGeneralDescription(IVmLocalizedEntityModel model)
        {
            return ExecuteScheduleEntity(model, (unitOfWork, result) => GetGeneralDescriptionHeader(result.Id, unitOfWork));
        }

        private void UpdateRelatedServiceNames(IUnitOfWorkWritable unitOfWork, IVmLocalizedEntityModel model, PublishingResult affected)
        {
            var unificRootId =
                VersioningManager.GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, affected.Id);
            if (unificRootId.HasValue)
            {
                if (CheckAndDistributeGeneralDescriptionNameToRelatedServices(unitOfWork, unificRootId.Value, affected.Id)) //TODO check
                {
                    //Select language in published state
                    var updatedPublishedLanguageIds = model.LanguagesAvailabilities
                        .Where(x => x.StatusId == typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString()))
                        .Select(x => x.LanguageId).ToList();

                    FindAndUpdateRelatedServiceNames(unitOfWork, unificRootId.Value, affected.Id, updatedPublishedLanguageIds);
                }
            }
        }

        private void FindAndUpdateRelatedServiceNames(IUnitOfWorkWritable unitOfWork, Guid generalDescriptionRootId, Guid entityId, ICollection<Guid> updatedPublishedLanguageIds)
        {
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var gdRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());

            var lastPublishedGeneralDescription = unitOfWork.ApplyIncludes(gdRep.All() //Set as param
                        .Where(x => x.Id == entityId),
                    i => i.Include(x => x.Names))
                .FirstOrDefault();

            var serviceRootIds = serviceRep.All()
                .Where(x => x.StatutoryServiceGeneralDescriptionId == generalDescriptionRootId &&
                            x.ServiceNames.Any(y => y.Inherited)).Select(y => y.UnificRootId)
                            .Distinct().ToList();

            var serviceVersionedIds = serviceRootIds
                .Select(x => VersioningManager.GetLastPublishedModifiedDraftVersion<ServiceVersioned>(unitOfWork, x)?.EntityId)
                .WhereNotNull()
                .ToList();

            var services = serviceRep.All()
                .Include(x => x.ServiceNames)
                .Include(x => x.LanguageAvailabilities)
                .Where(x => serviceVersionedIds.Contains(x.Id)).ToList();

            //Change names by inherited, publishedLanguages
            foreach (var service in services)
            {
                var serviceNames = service.ServiceNames
                    .Where(x => x.TypeId == nameTypeId)
                    .Select(serviceName => CopyServiceName(serviceName)) //Create new copy of instance, because later versioning make unwanted service name changes  
                    .ToList();
                
                var nameUpdated = false;
                serviceNames.Where(y => updatedPublishedLanguageIds.Contains(y.LocalizationId) && y.Inherited).ToList().ForEach(sn =>
                    {
                        if (lastPublishedGeneralDescription != null)
                        {
                            nameUpdated = true;
                            sn.Name = lastPublishedGeneralDescription.Names.Where(x => x.LocalizationId == sn.LocalizationId).Select(y => y.Name).FirstOrDefault() ?? string.Empty;
                        }
                    }
                );
                
                if (nameUpdated)
                {
                    //Translator isn't use, because it changed (oldPublished)service names    
                    var updatedServiceNames= serviceNames.ToDictionary(x => languageCache.GetByValue(x.LocalizationId), y => y.Name);
                    TranslationManagerToEntity.Translate<VmEntityUpdateName, ServiceVersioned>(new VmEntityUpdateName { Id = service.Id, Name = updatedServiceNames }, unitOfWork);
                }
            }

            unitOfWork.Save();
        }

        private ServiceName CopyServiceName(ServiceName serviceName)
        {
            return new ServiceName()
            {
                ServiceVersionedId = serviceName.ServiceVersionedId,
                TypeId = serviceName.TypeId,
                Name =  serviceName.Name,
                LocalizationId = serviceName.LocalizationId,
                Inherited =  serviceName.Inherited
            };
        }

        private bool CheckAndDistributeGeneralDescriptionNameToRelatedServices(IUnitOfWorkWritable unitOfWork, Guid unificRootId, Guid entityId)
        {
            var gdRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();

            var gdLastOldPublishedId = VersioningManager.GetLastOldPublishedVersion<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, unificRootId)?.EntityId;
            if (gdLastOldPublishedId.IsAssigned())
            {
                //Last Old published
               var gdLastOldPublishedNames = unitOfWork.ApplyIncludes(gdRep.All()
                        .Where(x => x.Id == gdLastOldPublishedId),
                                i => i.Include(x => x.Names))
                        .FirstOrDefault()?.Names ?? new List<StatutoryServiceName>();
                //Last Published
                var gdLastPublishedNames = unitOfWork.ApplyIncludes(gdRep.All()
                                                       .Where(x => x.Id == entityId),
                                                   i => i.Include(x => x.Names))
                                               .FirstOrDefault()?.Names ?? new List<StatutoryServiceName>();

                //Check that names are changed with previos version
                return CheckNamesChanged(gdLastOldPublishedNames, gdLastPublishedNames);
            }

            return false;
        }

        private bool CheckNamesChanged(ICollection<StatutoryServiceName> previousNames, ICollection<StatutoryServiceName> currentNames)
        {   //Add check of different status of languageAvailabilities after reArchiving
            return previousNames.Count != currentNames.Count
                     || previousNames.Any(prvName => !currentNames.Any(x => x.LocalizationId == prvName.LocalizationId && x.Name.Trim() == prvName.Name.Trim()));
        }


        public VmGeneralDescriptionHeader WithdrawGeneralDescription(Guid generalDescriptionId)
        {
            return ExecuteWithdraw(generalDescriptionId, GetGeneralDescriptionHeader);
//            var result =  CommonService.WithdrawEntity<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(generalDescriptionId);
//            UnLockGeneralDescription(result.Id.Value);
//            return GetGeneralDescriptionHeader(result.Id);
        }

        public VmGeneralDescriptionHeader RestoreGeneralDescription(Guid generalDescriptionId)
        {
            return ExecuteRestore(generalDescriptionId, GetGeneralDescriptionHeader);
//            var result = CommonService.RestoreEntity<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(generalDescriptionId);
//            UnLockGeneralDescription(result.Id.Value);
//            return GetGeneralDescriptionHeader(result.Id);
        }

        public VmGeneralDescriptionHeader ArchiveLanguage(VmEntityBasic model)
        {
            return ExecuteArchiveLanguage(model, GetGeneralDescriptionHeader);
//            var entity = CommonService.ArchiveLanguage<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(model);
//            UnLockGeneralDescription(entity.Id);
//            return GetGeneralDescriptionHeader(entity.Id);
        }

        public VmGeneralDescriptionHeader RestoreLanguage(VmEntityBasic model)
        {
            return ExecuteRestoreLanguage(model, GetGeneralDescriptionHeader);
//            var entity = CommonService.RestoreLanguage<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(model);
//            UnLockGeneralDescription(entity.Id);
//            return GetGeneralDescriptionHeader(entity.Id);
        }

        public VmGeneralDescriptionHeader WithdrawLanguage(VmEntityBasic model)
        {
            return ExecuteWithdrawLanguage(model, GetGeneralDescriptionHeader);
//            var entity = CommonService.WithdrawLanguage<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(model);
//            UnLockGeneralDescription(entity.Id);
//            return GetGeneralDescriptionHeader(entity.Id);
        }

        public VmGeneralDescriptionHeader RemoveGeneralDescription(Guid entityId)
        {
            return ExecuteRemove(entityId, GetGeneralDescriptionHeader);
        }

        public VmGeneralDescriptionHeader DeleteGeneralDescription(Guid entityId)
        {
            return ExecuteDelete(entityId, GetGeneralDescriptionHeader, unitOfWork => generalDescriptionServiceInternal.OnDeletingGeneralDescription(unitOfWork, entityId));
//            VmGeneralDescriptionHeader result = null;
//            contextManager.ExecuteWriter(unitOfWork =>
//            {
//                var deletedGD = DeleteGeneralDescription(unitOfWork, entityId);
//                unitOfWork.Save();
//                result = GetGeneralDescriptionHeader(deletedGD.Id, unitOfWork);
//
//            });
//            UnLockGeneralDescription(result.Id.Value);
//            return result;
        }

        public IVmEntityBase GetGeneralDescriptionStatus(Guid? entityId)
        {
            VmPublishingStatus result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = GetGeneralDescriptionStatus(unitOfWork, entityId);
            });
            return new VmEntityStatusBase { PublishingStatusId = result.Id };
        }

        public IVmEntityBase LockGeneralDescription(Guid id, bool isLockDisAllowedForArchived = false)
        {
            return Utilities.LockEntityVersioned<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(id, isLockDisAllowedForArchived);
        }

        public IVmEntityBase UnLockGeneralDescription(Guid id)
        {
            return Utilities.UnLockEntityVersioned<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(id);
        }

        public IVmOpenApiGeneralDescriptionVersionBase AddGeneralDescription(IVmOpenApiGeneralDescriptionInVersionBase vm, int openApiVersion)
        {
            var saveMode = SaveMode.Normal;
            var userId = Utilities.GetRelationIdForExternalSource();
            StatutoryServiceGeneralDescriptionVersioned generalDescription = null;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                generalDescription = TranslationManagerToEntity.Translate<IVmOpenApiGeneralDescriptionInVersionBase, StatutoryServiceGeneralDescriptionVersioned>(vm, unitOfWork);
                var generalDescriptionRepository = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                generalDescriptionRepository.Add(generalDescription);
                commonService.CreateHistoryMetaData<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(generalDescription);
                unitOfWork.Save(saveMode);
            });

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = CommonService.PublishAllAvailableLanguageVersions<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(generalDescription.Id, i => i.StatutoryServiceGeneralDescriptionVersionedId == generalDescription.Id);
            }

            return GetGeneralDescriptionWithDetails(generalDescription.Id, openApiVersion, true, false);
        }

        public IVmOpenApiGeneralDescriptionVersionBase SaveGeneralDescription(IVmOpenApiGeneralDescriptionInVersionBase vm, int openApiVersion)
        {
            StatutoryServiceGeneralDescriptionVersioned generalDescription = null;
            ProcessNewUrls(vm);
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var generalDescriptionRepository = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();

                if (vm.PublishingStatus == PublishingStatus.Deleted.ToString())
                {
                    // Check if any services are attached into general description
                    var serviceVersionedRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    var relatedServices = serviceVersionedRep.All()
                        .Where(x => x.StatutoryServiceGeneralDescriptionId == vm.Id).ToList();
                    if (relatedServices?.Count > 0)
                    {
                        throw new Exception($"There are services attached into general description {vm.Id}. You cannot delete/archive this general description!");
                    }
                    var publishStatus = TranslationManagerToEntity.Translate<String, PublishingStatusType>(PublishingStatus.Deleted.ToString(), unitOfWork);
                    generalDescription = generalDescriptionRepository.All().Single(i => i.Id == vm.VersionId.Value);
                    generalDescription.PublishingStatus = publishStatus;
                }
                else
                {
                    // Entity needs to be restored?
                    if (vm.CurrentPublishingStatus == PublishingStatus.Deleted.ToString())
                    {
                        if (vm.PublishingStatus == PublishingStatus.Modified.ToString() || vm.PublishingStatus == PublishingStatus.Published.ToString())
                        {
                            // We need to restore already archived item
                            var publishingResult = CommonService.RestoreArchivedEntity<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(unitOfWork, vm.VersionId.Value);
                        }
                    }

                    generalDescription = TranslationManagerToEntity.Translate<IVmOpenApiGeneralDescriptionInVersionBase, StatutoryServiceGeneralDescriptionVersioned>(vm, unitOfWork);

                }
                commonService.CreateHistoryMetaData<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(generalDescription, setByEntity: true);
                unitOfWork.Save(parentEntity: generalDescription);
            });

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = CommonService.PublishAllAvailableLanguageVersions<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(generalDescription.Id, i => i.StatutoryServiceGeneralDescriptionVersionedId == generalDescription.Id);
            }

            return GetGeneralDescriptionWithDetails(generalDescription.Id, openApiVersion, true, false);
        }

        private void ProcessNewUrls(IVmOpenApiGeneralDescriptionInVersionBase vm)
        {
            var distinctUrls = vm?.Legislation.Where(x => !x.WebPages.IsNullOrEmpty())
                ?.SelectMany(x => x.WebPages.Select(y => y.Url)).Distinct().ToList();
            contextManager.ExecuteWriter(unitOfWork =>
            {
                urlService.AddNewUrls(unitOfWork, distinctUrls);
                unitOfWork.Save();
            });
        }

        public VmGeneralDescriptionOutput SaveGeneralDescription(VmGeneralDescriptionInput model)
        {
            var preSaveActions = new List<Action<IUnitOfWorkWritable>>
            {
                unitOfWork => urlService.AddNewUrls(unitOfWork,
                    model.Laws.SelectMany(x => x.WebPage.Select(y => y.Value.UrlAddress)))
            };
            
            return ExecuteSave
            (
                model,
                unitOfWork => SaveGeneralDescription(unitOfWork, model),
                (unitOfWork, entity) => GetGeneralDescription(unitOfWork, new VmGeneralDescriptionGet { Id = entity.Id }),
                preSaveActions
            );
        }

        private StatutoryServiceGeneralDescriptionVersioned SaveGeneralDescription(IUnitOfWorkWritable unitOfWork, VmGeneralDescriptionInput model)
        {
// SOTE has been disabled (SFIPTV-1177)
//            CheckDescriptionType(unitOfWork, model);

            var entity = TranslationManagerToEntity.Translate<VmGeneralDescriptionInput, StatutoryServiceGeneralDescriptionVersioned>(model, unitOfWork);

            if (model.Id.IsAssigned())
            {
                foreach (var language in model.LanguagesAvailabilities)
                {
                    translationService.ConfirmGeneralDescriptionDeliveredTranslation(unitOfWork, model.Id.Value, language.LanguageId);
                }
            }
            commonService.CreateHistoryMetaData<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(entity);

            return entity;
        }

        public VmGeneralDescriptionHeader GetGeneralDescriptionHeader(Guid? generalDescriptionId)
        {
            return contextManager.ExecuteReader(unitOfWork => GetGeneralDescriptionHeader(generalDescriptionId, unitOfWork));
        }

        public VmGeneralDescriptionHeader GetGeneralDescriptionHeader(Guid? generalDescriptionId, IUnitOfWork unitOfWork)
        {
            var result = new VmGeneralDescriptionHeader();
            StatutoryServiceGeneralDescriptionVersioned entity;
            result = GetModel<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionHeader>(entity = GetEntity<StatutoryServiceGeneralDescriptionVersioned>(generalDescriptionId, unitOfWork,
                q => q
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x => x.Names)
                    .Include(x => x.Versioning)
            ), unitOfWork);
            result.PreviousInfo = generalDescriptionId.HasValue ? Utilities.GetEntityEditableInfo<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(generalDescriptionId.Value, unitOfWork) : null;
            if (generalDescriptionId.HasValue)
            {
                var unificRootId = VersioningManager.GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, generalDescriptionId.Value);
                if (unificRootId.HasValue)
                {
                    result.TranslationAvailability = translationService.GetGeneralDescriptionTranslationAvailabilities(unitOfWork, generalDescriptionId.Value, unificRootId.Value);
                }
            }

            return result;
        }

        private VmPublishingStatus GetGeneralDescriptionStatus(IUnitOfWorkWritable unitOfWork, Guid? entityId)
        {
            var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            var generalDescription = generalDescriptionRep.All()
                            .Include(x => x.PublishingStatus)
                            .Single(x => x.Id == entityId.Value);

            return TranslationManagerToVm.Translate<PublishingStatusType, VmPublishingStatus>(generalDescription.PublishingStatus);
        }

        public VmGeneralDescriptionHeader GetValidatedEntity(VmEntityBasic model)
        {
            return ExecuteValidate
            (
                () => Utilities.LockEntityVersioned<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(model.Id.Value, true),
                unitOfWork => GetGeneralDescriptionHeader(model.Id, unitOfWork)
            );
        }

        private void ProcessNewGeneralDescriptionConnections(VmConnectionsInput modelInput)
        {
            ContextManager.ExecuteReader(unitOfWork =>
            {
                var unificRootId = VersioningManager
                    .GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, modelInput.Id);
                if (unificRootId.HasValue)
                {
                    modelInput.UnificRootId = unificRootId.Value;
                }
            });
            if (!modelInput.UnificRootId.IsAssigned()) return;
            var order = 1;
            foreach (var child in modelInput.SelectedConnections)
            {
                ContextManager.ExecuteWriter(unitOfWork =>
                {
                    utilities.CheckIsEntityConnectable<StatutoryServiceGeneralDescriptionVersioned>(modelInput.Id, unitOfWork);

                    var model = new VmConnectionInput
                    {
                        MainEntityId = modelInput.UnificRootId,
                        ConnectedEntityId = child.ConnectedEntityId,
                        ServiceOrderNumber = order++
                    };
                    TranslationManagerToEntity.Translate<VmConnectionInput, GeneralDescriptionServiceChannel>(model,
                        unitOfWork);
                    unitOfWork.Save();
                });
            }

        }
        public VmServiceConnectionsOutput SaveRelations(VmConnectionsInput model)
        {
            ProcessNewGeneralDescriptionConnections(model);
            contextManager.ExecuteWriter(unitOfWork =>
            {
                //utilities.CheckIsEntityConnectable<StatutoryServiceGeneralDescriptionVersioned>(model.Id, unitOfWork);
                SaveRelations(unitOfWork, model);
                unitOfWork.Save();
            });
            return GetRelations(model);
        }

        private void SaveRelations(IUnitOfWorkWritable unitOfWork, VmConnectionsInput model)
        {
            var unificRootId = VersioningManager.GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, model.Id);
            if (unificRootId.HasValue)
            {
                model.UnificRootId = unificRootId.Value;
                TranslationManagerToEntity.Translate<VmConnectionsInput, StatutoryServiceGeneralDescription>(model, unitOfWork);
            }
         }

        private VmServiceConnectionsOutput GetRelations(VmConnectionsInput model)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                return GetRelations(unitOfWork, model);
            });
        }

        private VmServiceConnectionsOutput GetRelations(IUnitOfWork unitOfWork, VmConnectionsInput model)
        {
            var gdLangAvailabilitiesRep = unitOfWork.CreateRepository<IGeneralDescriptionLanguageAvailabilityRepository>();
            var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionRepository>();
            var unificRootId = VersioningManager.GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, model.Id);
            if (unificRootId.HasValue)
            {
                var generalDescription = generalDescriptionRep.All()
                    .Include(j => j.StatutoryServiceGeneralDescriptionServiceChannels)
                    .ThenInclude(j => j.ServiceChannel)
                    .ThenInclude(j => j.Versions)
                    .ThenInclude(j => j.ServiceChannelNames)
                    .Include(j => j.StatutoryServiceGeneralDescriptionServiceChannels)
                    .ThenInclude(j => j.ServiceChannel)
                    .ThenInclude(j => j.Versions)
                    .ThenInclude(j => j.LanguageAvailabilities)
                    .Include(j => j.StatutoryServiceGeneralDescriptionServiceChannels)
                    .ThenInclude(j => j.ServiceChannel)
                    .ThenInclude(j => j.Versions)
                    .ThenInclude(j => j.DisplayNameTypes)
                    .Include(j => j.StatutoryServiceGeneralDescriptionServiceChannels)
                    .Single(x => x.Id == unificRootId.Value);
                var result = TranslationManagerToVm.Translate<StatutoryServiceGeneralDescription, VmServiceConnectionsOutput>(generalDescription);
                result.Id = model.Id;
                result.LanguagesAvailabilities =
                    TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                        gdLangAvailabilitiesRep.All()
                            .Where(x => model.Id == x.StatutoryServiceGeneralDescriptionVersionedId)
                            .ToList()
                            .OrderBy(x => languageOrderCache.Get(x.LanguageId))
                            .ToList());
                return result;
            }
            return null;
        }

        public IVmEntityBase IsConnectable(Guid id)
        {
            ContextManager.ExecuteWriter(unitOfWork =>
            {
                utilities.CheckIsEntityConnectable<StatutoryServiceGeneralDescriptionVersioned>(id, unitOfWork);
            });
            return null;
        }


        public VmTranslationOrderStateSaveOutputs SendGeneralDescriptionEntityToTranslation(VmTranslationOrderInput model)
        {
            Guid entityId = Guid.Empty;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                translationService.CheckGeneralDescriptionOrderUpdate(model, unitOfWork);
                entityId = translationService.SaveGeneralDescriptionTranslationOrder(unitOfWork, model);
                unitOfWork.Save();
            });

            return GetGeneralDescriptionTranslationSaveData(entityId, model.SourceLanguage);
        }

        private VmTranslationOrderStateSaveOutputs GetGeneralDescriptionTranslationSaveData(Guid entityId, Guid languageId)
        {
            return ContextManager.ExecuteReader(unitOfWork => new VmTranslationOrderStateSaveOutputs
            {
                Id = entityId,
                GeneralDescriptions = new List<VmGeneralDescriptionOutput> { GetGeneralDescription(unitOfWork, new VmGeneralDescriptionGet { Id = entityId })},
                Translations = new List<VmTranslationOrderStateOutputs>
                {
                    translationService.GetGeneralDescriptionTranslationOrderStates(unitOfWork, entityId, languageId)
                }
            });
        }

        public VmTranslationOrderStateOutputs GetGeneralDescriptionTranslationData(VmTranslationDataInput model)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                return translationService.GetGeneralDescriptionTranslationOrderStates(unitOfWork, model.EntityId, model.SourceLanguage);
            });
        }
/* SOTE has been disabled (SFIPTV-1177)
        private void CheckDescriptionType(IUnitOfWorkWritable unitOfWork, VmGeneralDescriptionInput model)
        {
            var existingEntity = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>()
                .All().SingleOrDefault(gd => gd.Id == model.Id);

            if (existingEntity == null) return;
            if (IsSoteAndUsedInService(unitOfWork, existingEntity.GeneralDescriptionTypeId, existingEntity.UnificRootId) && !IsSoteGeneralDescription(model.GeneralDescriptionType))
            {
                throw new PtvAppException("", "GeneralDescription.Save.GeneralDescriptionType.MessageFailed");
            }
        }

        private bool IsSoteAndUsedInService(IUnitOfWork unitOfWork, Guid? generalDescriptionType, Guid? unificRootId)
        {
            return IsSoteGeneralDescription(generalDescriptionType)
                   && unitOfWork.CreateRepository<IServiceVersionedRepository>().All().Any(s => s.StatutoryServiceGeneralDescriptionId == unificRootId);
        }

        private bool IsSoteGeneralDescription(Guid? generalDescriptionType)
        {
            if (!generalDescriptionType.IsAssigned()) return false;
            return generalDescriptionType == typesCache.Get<GeneralDescriptionType>(GeneralDescriptionTypeEnum.PrescribedByFreedomOfChoiceAct.ToString())
                   || generalDescriptionType == typesCache.Get<GeneralDescriptionType>(GeneralDescriptionTypeEnum.OtherPermissionGrantedSote.ToString());
        }
*/
    }

    [RegisterService(typeof(IGeneralDescriptionServiceInternal), RegisterType.Transient)]
    internal class GeneralDescriptionServiceInternal : IGeneralDescriptionServiceInternal
    {
        private IVersioningManager versioningManager;
        private ICommonServiceInternal commonService;

        public GeneralDescriptionServiceInternal(IVersioningManager versioningManager, ICommonServiceInternal commonService)
        {
            this.versioningManager = versioningManager;
            this.commonService = commonService;
        }

        public void OnDeletingGeneralDescription(IUnitOfWorkWritable unitOfWork, Guid entityId)
        {
            var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            var rootId = versioningManager.GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, entityId);

            if (rootId == null)
            {
                return;
            }

            var lastPublishedVersionInfo = versioningManager
                .GetLastPublishedVersion<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, rootId.Value);

            if (lastPublishedVersionInfo == null)
            {
                return;
            }

            var generalDescription = generalDescriptionRep.All()
                .Include(x => x.TargetGroups)
                .Include(x => x.ServiceClasses)
                .Include(x => x.LifeEvents)
                .Include(x => x.OntologyTerms)
                .Include(x => x.IndustrialClasses)
                .Single(x => x.Id == lastPublishedVersionInfo.EntityId);

            var serviceVersionedRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var relatedServices = serviceVersionedRep.All()
                .Where(x => x.StatutoryServiceGeneralDescriptionId == generalDescription.UnificRootId)
                .Include(x => x.ServiceTargetGroups)
                .Include(x => x.ServiceServiceClasses)
                .Include(x => x.ServiceLifeEvents)
                .Include(x => x.ServiceOntologyTerms)
                .Include(x => x.ServiceIndustrialClasses).ToList();
            relatedServices.ForEach(x =>
            {
                TryWithdrawEntity<ServiceVersioned, ServiceLanguageAvailability>(unitOfWork, x);
                x.TypeId = generalDescription.TypeId;
                x.ChargeTypeId = generalDescription.ChargeTypeId;
                CopyClassificationData(unitOfWork, x, generalDescription);
                x.StatutoryServiceGeneralDescriptionId = null;
            });
        }

        private bool TryWithdrawEntity<TEntity, TLanguage>(IUnitOfWorkWritable unitOfWork, TEntity entity )
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguage>, new()
            where TLanguage: class, ILanguageAvailability, new()
        {
            try
            {
                versioningManager.ChangeStatusOfLanguageVersion<TEntity, TLanguage>(unitOfWork, entity, PublishingStatus.Modified, new List<PublishingStatus> { PublishingStatus.Published });
                versioningManager.ChangeToModified(unitOfWork, entity, new List<PublishingStatus> { PublishingStatus.Published });
                commonService.CreateHistoryMetaData<TEntity, TLanguage>(entity, action: HistoryAction.Withdraw);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private void CopyClassificationData(IUnitOfWorkWritable unitOfWork, ServiceVersioned dest, StatutoryServiceGeneralDescriptionVersioned source)
        {
            var overideTargetGroup = dest.ServiceTargetGroups.Where(x => x.Override);
            var targetGroupIds = dest.ServiceTargetGroups.Select(x => x.TargetGroupId);

            source.TargetGroups
                .Where(x=>!targetGroupIds.Contains(x.TargetGroupId))
                .ForEach(x => dest.ServiceTargetGroups.Add(new ServiceTargetGroup { ServiceVersionedId = dest.Id, TargetGroupId = x.TargetGroupId }));

            var stgRep = unitOfWork.CreateRepository<IServiceTargetGroupRepository>();
            overideTargetGroup.ForEach(tg => stgRep.Remove(tg));

            var serviceClassesIds = dest.ServiceServiceClasses.Select(x => x.ServiceClassId);
            source.ServiceClasses
                .Where(x => !serviceClassesIds.Contains(x.ServiceClassId))
                .ForEach(x => dest.ServiceServiceClasses.Add(new ServiceServiceClass { ServiceVersionedId = dest.Id, ServiceClassId = x.ServiceClassId }));

            var ontologyTermsIds = dest.ServiceOntologyTerms.Select(x => x.OntologyTermId);
            source.OntologyTerms
                .Where(x => !ontologyTermsIds.Contains(x.OntologyTermId))
                .ForEach(x => dest.ServiceOntologyTerms.Add(new ServiceOntologyTerm { ServiceVersionedId = dest.Id, OntologyTermId = x.OntologyTermId }));

            var lifeEventsIds = dest.ServiceLifeEvents.Select(x => x.LifeEventId);
            source.LifeEvents
                .Where(x => !lifeEventsIds.Contains(x.LifeEventId))
                .ForEach(x => dest.ServiceLifeEvents.Add(new ServiceLifeEvent { ServiceVersionedId = dest.Id, LifeEventId = x.LifeEventId }));

            var industrialClassesIds = dest.ServiceIndustrialClasses.Select(x => x.IndustrialClassId);
            source.IndustrialClasses
                .Where(x => !industrialClassesIds.Contains(x.IndustrialClassId))
                .ForEach(x => dest.ServiceIndustrialClasses.Add(new ServiceIndustrialClass { ServiceVersionedId = dest.Id, IndustrialClassId = x.IndustrialClassId }));
        }
    }
}
