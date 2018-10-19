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
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.DataAccess.Services.V2;
using PTV.Database.DataAccess.Utils;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.OpenApi.V6;
using System.Linq.Expressions;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Logic;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Domain.Model.Models.V2.TranslationOrder;
using PTV.Framework.Extensions;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IGeneralDescriptionService), RegisterType.Transient)]
    internal class GeneralDescriptionService : EntityServiceBase<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>, IGeneralDescriptionService
    {
        private readonly ITranslationEntity translationManager;
        private ITranslationViewModel translationManagerVModel;
        private readonly IContextManager contextManager;
        private readonly ILogger logger;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private IVersioningManager versioningManager;
        private DataUtils dataUtils;
        private ServiceUtilities utilities;
        private ITranslationService translationService;
        private ICommonServiceInternal commonService;
        private ILanguageOrderCache languageOrderCache;
        private IPahaTokenProcessor pahaTokenProcessor;
        private IRestrictionFilterManager restrictionFilterManager;

        public GeneralDescriptionService(
            IContextManager contextManager,
            ITranslationEntity translationManager,
            ITranslationViewModel translationManagerVModel,
            ILogger<GeneralDescriptionService> logger,
            ServiceUtilities utilities,
            ICommonServiceInternal commonService,
            ITranslationService translationService,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            ILanguageCache languageCache,
            ITypesCache typesCache,
            IVersioningManager versioningManager,
            DataUtils dataUtils,
            IValidationManager validationManager,
            ILanguageOrderCache languageOrderCache,
            IRestrictionFilterManager restrictionFilterManager,
            IPahaTokenProcessor pahaTokenProcessor)
            : base(translationManager, translationManagerVModel, publishingStatusCache, userOrganizationChecker, contextManager, utilities, commonService, validationManager)
        {
            this.translationManager = translationManager;
            this.contextManager = contextManager;
            this.translationManagerVModel = translationManagerVModel;
            this.logger = logger;
            this.typesCache = typesCache;
            this.languageCache = languageCache;
            this.versioningManager = versioningManager;
            this.dataUtils = dataUtils;
            this.utilities = utilities;
            this.translationService = translationService;
            this.commonService = commonService;
            this.languageOrderCache = languageOrderCache;
            this.pahaTokenProcessor = pahaTokenProcessor;
            this.restrictionFilterManager = restrictionFilterManager;
        }

        public VmTargetGroups GetSubTargetGroups(Guid targetGroupId)
        {
            VmTargetGroups result = new VmTargetGroups();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var targetGroupRep = unitOfWork.CreateRepository<ITargetGroupRepository>();
                var targetGroups = targetGroupRep.All().Where(i => i.ParentId == targetGroupId).OrderBy(i => i.Label);
                result.TargetGroups = translationManager.TranslateAll<TargetGroup, VmListItem>(targetGroups);
            });
            return result;
        }
        
        public Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptions SearchGeneralDescriptions(VmGeneralDescriptionSearchForm searchData)
        {
            searchData.Name = searchData.Name != null ? Regex.Replace(searchData.Name.Trim(), @"\s+", " ") : searchData.Name;
            IReadOnlyList<PTV.Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionListItem> result = new List<PTV.Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionListItem>();
            bool moreAvailable = false;
            int count = 0;
            int safePageNumber = searchData.PageNumber.PositiveOrZero();

            var gdTypes = typesCache.GetCacheData<GeneralDescriptionType>();

            var restrictedTypes = pahaTokenProcessor.UserRole == UserRoleEnum.Eeva ? new List<Guid>() : restrictionFilterManager.SetAccessForGuidTypes<GeneralDescriptionType>(pahaTokenProcessor.ActiveOrganization, gdTypes).Where(i => i.Access == EVmRestrictionFilterType.Denied).Select(i => i.Id).ToList();
            
            var languagesIds = searchData.Languages.Select(language => languageCache.Get(language.ToString())).ToList();
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var publishedStatusId = PublishingStatusCache.Get(PublishingStatus.Published);
            
            contextManager.ExecuteReader(unitOfWork =>
            {
                var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var query = generalDescriptionRep.All();
                if (restrictedTypes.Any())
                {
                    query = query.Where(i => !restrictedTypes.Contains(i.GeneralDescriptionTypeId));
                }

                var resultTemp = searchData.OnlyPublished ? query.Where(x => x.PublishingStatusId == publishedStatusId) : query;

                #region SearchByFilterParam

                if (searchData.ServiceClassId.HasValue)
                {
                    resultTemp = resultTemp.Where(x => x.ServiceClasses.Any(s => s.ServiceClassId == searchData.ServiceClassId.Value));
                }

                if (searchData.ServiceTypeId.HasValue)
                {
                    resultTemp = resultTemp.Where(x => x.TypeId == searchData.ServiceTypeId.Value);
                }
                
                if (searchData.GeneralDescriptionTypeId.HasValue)
                {
                    resultTemp = resultTemp.Where(x => x.GeneralDescriptionTypeId == searchData.GeneralDescriptionTypeId.Value);
                }
                
                if (searchData.TargetGroupId.IsAssigned())
                {
                    resultTemp = resultTemp.Where(x => x.TargetGroups.Any(s => s.TargetGroupId == searchData.TargetGroupId.Value));
                }

                if (!string.IsNullOrEmpty(searchData.Name))
                {
                    var rootId = GetRootIdFromString(searchData.Name);
                    if (!rootId.HasValue)
                    {
                        var searchText = searchData.Name.ToLower();
                        resultTemp =
                            resultTemp.Where(x => x.Names.Any(y =>
                                (y.Name.ToLower().Contains(searchText) || y.CreatedBy.ToLower().Contains(searchText) || y.ModifiedBy.ToLower().Contains(searchText))
                                && languagesIds.Contains(y.LocalizationId)));
                    }
                    else
                    {
                        resultTemp = resultTemp
                            .Where(service =>
                                service.UnificRootId == rootId
                            );
                    }
                }
                else
                {
                    resultTemp =
                        resultTemp.Where(
                            x =>
                                x.Names.Any(
                                    y =>
                                        languagesIds.Contains(y.LocalizationId) &&
                                        !string.IsNullOrEmpty(y.Name)));
                }

                if (searchData.SelectedPublishingStatuses != null)
                {
                    CommonService.ExtendPublishingStatusesByEquivalents(searchData.SelectedPublishingStatuses);
                    resultTemp = resultTemp.WherePublishingStatusIn(searchData.SelectedPublishingStatuses);
                }

                #endregion SearchByFilterParam

                count = resultTemp.Count();
                moreAvailable = count.MoreResultsAvailable(safePageNumber);

                var nameType = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
                var gdTypeInfo = typesCache.Get<DescriptionType>(DescriptionTypeEnum.GeneralDescriptionTypeAdditionalInformation.ToString());

                var resultTempData = resultTemp.Select(i => new
                {
                    Id = i.Id,
                    PublishingStatusId = i.PublishingStatusId,
                    UnificRootId = i.UnificRootId,
                    Name = i.Names.OrderBy(x=>x.Localization.OrderNumber).FirstOrDefault(x => languagesIds.Contains(x.LocalizationId) && x.TypeId == nameTypeId).Name,
                    AllNames = i.Names.Where(x => x.TypeId == nameType).Select(x => new { x.LocalizationId, x.Name }),
                    GeneralDescriptionTypeAdditionalInformation = i.Descriptions.Where(x => x.TypeId == gdTypeInfo).OrderBy(x=>x.Localization.OrderNumber).FirstOrDefault(x => languagesIds.Contains(x.LocalizationId)).Description,
                    AllGeneralDescriptionTypeAdditionalInformation = i.Descriptions.Where(x => x.TypeId == gdTypeInfo).Select(x => new { x.LocalizationId, x.Description }),
                    TypeId = i.TypeId,
                    GeneralDescriptionTypeId = i.GeneralDescriptionTypeId,
                    TypeName = i.Type.Names.OrderBy(x => x.Localization.OrderNumber).FirstOrDefault(x => languagesIds.Contains(x.LocalizationId)).Name,
                    LanguageAvailabilities = i.LanguageAvailabilities.OrderBy(x => x.Language.OrderNumber),
                    VersionMajor = i.Versioning.VersionMajor,
                    VersionMinor = i.Versioning.VersionMinor,
                    Modified = i.Modified,
                    ModifiedBy = i.ModifiedBy,
                })
                .ApplySortingByVersions(searchData.SortData, new VmSortParam() { Column = "Modified", SortDirection = SortDirectionEnum.Desc })
                    .Select(i => new
                    {
                        Id = i.Id,
                        UnificRootId = i.UnificRootId,
                        PublishingStatusId = i.PublishingStatusId,
                        VersionMajor = i.VersionMajor,
                        VersionMinor = i.VersionMinor,
                        TypeId = i.TypeId,
                        GeneralDescriptionTypeId = i.GeneralDescriptionTypeId,
                        Modified = i.Modified
                    })
                .ApplyPagination(safePageNumber)
                .ToList();

                var generalDescriptionIds = resultTempData.Select(i => i.Id).ToList();
                var generalDescriptionNameRep = unitOfWork.CreateRepository<IStatutoryServiceNameRepository>();
                var generalDescriptionNames =
                generalDescriptionNameRep.All().Where(x => generalDescriptionIds.Contains(x.StatutoryServiceGeneralDescriptionVersionedId) && languagesIds.Contains(x.LocalizationId) && x.TypeId == nameTypeId).Select(i => new { i.StatutoryServiceGeneralDescriptionVersionedId, i.Name, i.LocalizationId }).ToList().GroupBy(i => i.StatutoryServiceGeneralDescriptionVersionedId)
                    .ToDictionary(i => i.Key, i => i.OrderBy(j => languageOrderCache.Get(j.LocalizationId)).ToDictionary(y => languageCache.GetByValue(y.LocalizationId), m => m.Name));

                var generalDescriptionShortDescRep = unitOfWork.CreateRepository<IStatutoryServiceDescriptionRepository>();
                var generalDescriptionTypeDescs =
                    generalDescriptionShortDescRep.All().Where(x => x.TypeId == gdTypeInfo && generalDescriptionIds.Contains(x.StatutoryServiceGeneralDescriptionVersionedId) && languagesIds.Contains(x.LocalizationId)).Select(i => new { i.StatutoryServiceGeneralDescriptionVersionedId, i.Description, i.LocalizationId }).ToList().GroupBy(i => i.StatutoryServiceGeneralDescriptionVersionedId)
                        .ToDictionary(i => i.Key, i => i.OrderBy(j => languageOrderCache.Get(j.LocalizationId)).ToDictionary(y => languageCache.GetByValue(y.LocalizationId), m => m.Description));

                var generalDescriptionLangAvailabilitiesRep = unitOfWork.CreateRepository<IGeneralDescriptionLanguageAvailabilityRepository>();
                var generalDescriptionLangAvailabilities = generalDescriptionLangAvailabilitiesRep.All().Where(x => generalDescriptionIds.Contains(x.StatutoryServiceGeneralDescriptionVersionedId)).ToList()
                    .GroupBy(i => i.StatutoryServiceGeneralDescriptionVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(j => languageOrderCache.Get(j.LanguageId)).ToList());

                var generalDescriptionServiceClassesRep = unitOfWork.CreateRepository<IStatutoryServiceServiceClassRepository>();
                var generalDescriptionServiceClasses = generalDescriptionServiceClassesRep.All()
                    .Where(x => generalDescriptionIds.Contains(x.StatutoryServiceGeneralDescriptionVersionedId)).ToList()
                    .GroupBy(j => j.StatutoryServiceGeneralDescriptionVersionedId).ToDictionary(j => j.Key, j => j.Select(k => k.ServiceClassId).ToList());
                
                result = resultTempData.Select(i => new PTV.Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionListItem
                {
                    Id = searchData.OnlyPublished ? i.UnificRootId : i.Id,
                    PublishingStatusId = i.PublishingStatusId,
                    UnificRootId = i.UnificRootId,
                    ServiceTypeId = i.TypeId,
                    GeneralDescriptionTypeId = i.GeneralDescriptionTypeId,
                    Name = generalDescriptionNames.TryGetOrDefault(i.Id, new Dictionary<string, string>()), 
                    GeneralDescriptionTypeAdditionalInformation = generalDescriptionTypeDescs.TryGetOrDefault(i.Id, new Dictionary<string, string>()), 
                    ServiceClasses = generalDescriptionServiceClasses.TryGetOrDefault(i.Id, new List<Guid>()),
                    LanguagesAvailabilities = translationManager.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(generalDescriptionLangAvailabilities.TryGetOrDefault(i.Id, new List<GeneralDescriptionLanguageAvailability>())),
                    Version = new VmVersion() { Major = i.VersionMajor, Minor = i.VersionMinor },
                })
                .ToList();

                return result;
            });

            return new Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptions() {
                SearchResult = result,
                PageNumber = ++safePageNumber,
                MoreAvailable = moreAvailable,
                Count = count
            };
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

            description = unitOfWork.ApplyIncludes(description, i => i
                .Include(j => j.Names)
                .Include(x => x.ServiceClasses).ThenInclude(x => x.ServiceClass).ThenInclude(x => x.Names)
                .Include(x => x.OntologyTerms).ThenInclude(x => x.OntologyTerm).ThenInclude(x => x.Names)
                .Include(x => x.LifeEvents).ThenInclude(x => x.LifeEvent).ThenInclude(x => x.Names)
                .Include(x => x.IndustrialClasses).ThenInclude(x => x.IndustrialClass).ThenInclude(x => x.Names)
                .Include(x => x.TargetGroups)
                .Include(j => j.Descriptions)
                .Include(x => x.LanguageAvailabilities)
                .Include(x => x.Versioning)
                .Include(j => j.StatutoryServiceRequirements)
                .Include(j => j.StatutoryServiceLaws).ThenInclude(j => j.Law).ThenInclude(j => j.Names)
                .Include(j => j.StatutoryServiceLaws).ThenInclude(j => j.Law).ThenInclude(j => j.WebPages).ThenInclude(j => j.WebPage)
            );
          
            var result = translationManager.TranslateFirst<StatutoryServiceGeneralDescriptionVersioned, Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionOutput>(description);
            result.PreviousInfo = result.Id.HasValue ? Utilities.CheckIsEntityEditable<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(result.Id.Value, unitOfWork) : null;
            result.TranslationAvailability = translationService.GetGeneralDescriptionTranslationAvailabilities(unitOfWork, result.Id.Value, result.UnificRootId);
            
            //add connections
            var gdChannelRep = unitOfWork.CreateRepository<IGeneralDescriptionServiceChannelRepository>();
            var gdChannels = gdChannelRep.All()
                .Where(x => x.StatutoryServiceGeneralDescriptionId == result.UnificRootId
                            && x.ServiceChannel.Versions.Any(v =>
                                v.PublishingStatusId == psPublished || v.PublishingStatusId == psModified ||
                                v.PublishingStatusId == psDraft))
                .Include(j => j.ServiceChannel).ThenInclude(j => j.Versions).ThenInclude(j => j.ServiceChannelNames)
                .Include(j => j.ServiceChannel).ThenInclude(j => j.Versions).ThenInclude(j => j.LanguageAvailabilities);
                
            result.Connections = translationManager.TranslateAll<GeneralDescriptionServiceChannel, VmConnectionOutput>(gdChannels).ToList();

            
            if (model.OnlyPublished)
            {
                result.Id = result.UnificRootId;
            }
            
            // set IsSoteAndUsedInService
            result.IsSoteAndUsedInService = IsSoteAndUsedInService(unitOfWork, result.GeneralDescriptionType, result.UnificRootId);
            
            return result;
        }

        public IVmOpenApiGuidPageVersionBase GetGeneralDescriptions(DateTime? date, int pageNumber = 1, int pageSize = 1000, DateTime? dateBefore = null)
        {
            var vm = new V3VmOpenApiGuidPage(pageNumber, pageSize);

            if (pageNumber <= 0) return vm;

            List<StatutoryServiceGeneralDescriptionVersioned> gdList = null;

            contextManager.ExecuteReader(unitOfWork =>
            {
                gdList = GetPublishedEntities<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(vm, date, dateBefore, unitOfWork, q => q.Include(i => i.Names));
            });

            if (gdList?.Count > 0)
            {
                vm.ItemList = TranslationManagerToVm.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiItem>(gdList).ToList();
            }
            return vm;
        }

        public IList<IVmOpenApiGeneralDescriptionVersionBase> GetGeneralDescriptions(List<Guid> idList, int openApiVersion)
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
                    result = GetGeneralDescriptionsWithDetails(unitOfWork, filters, openApiVersion);
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

        public IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionVersionBase(Guid id, int openApiVersion, bool getOnlyPublished = true, bool checkRestrictions = false)
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
                        entityId = versioningManager.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, id, PublishingStatus.Published);
                    }
                    else
                    {
                        entityId = versioningManager.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, id, null, false);
                    }

                    if (entityId.HasValue)
                    {
                        result = GetGeneralDescriptionWithDetails(unitOfWork, entityId.Value, openApiVersion, getOnlyPublished);
                        // Check the restictions
                        if (checkRestrictions)
                        {
                            var gdTypes = typesCache.GetCacheData<GeneralDescriptionType>();
                            var restrictedTypes = pahaTokenProcessor.UserRole == UserRoleEnum.Eeva ? new List<Guid>() : restrictionFilterManager.SetAccessForGuidTypes<GeneralDescriptionType>(pahaTokenProcessor.ActiveOrganization, gdTypes).Where(i => i.Access == EVmRestrictionFilterType.Denied).Select(i => i.Id).ToList();

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
            Guid? entityId = versioningManager.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, id, PublishingStatus.Published);

            if (entityId.HasValue)
            {
                var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();

                var query = unitOfWork.ApplyIncludes(statutoryGeneralDescriptionRep.All()
                    .Where(s => s.Id == entityId.Value), q =>
                    q.Include(i => i.ServiceClasses).ThenInclude(i => i.ServiceClass).ThenInclude(i => i.Names)
                    .Include(i => i.ServiceClasses).ThenInclude(i => i.ServiceClass).ThenInclude(i => i.Descriptions)
                    .Include(i => i.OntologyTerms).ThenInclude(i => i.OntologyTerm).ThenInclude(i => i.Names)
                    .Include(i => i.TargetGroups).ThenInclude(i => i.TargetGroup).ThenInclude(i => i.Names)
                    .Include(i => i.LifeEvents).ThenInclude(i => i.LifeEvent).ThenInclude(i => i.Names)
                    .Include(i => i.IndustrialClasses).ThenInclude(i => i.IndustrialClass).ThenInclude(i => i.Names));

                return translationManager.TranslateFirst<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(query);
            }

            return null;
        }
        
        private IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionWithDetails(Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiGeneralDescriptionVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetGeneralDescriptionWithDetails(unitOfWork, versionId, openApiVersion, getOnlyPublished);
            });
            return result;
        }

        private IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionWithDetails(IUnitOfWork unitOfWork, Guid versionId, int openApiVersion, bool getOnlyPublished = true)
        {
            IVmOpenApiGeneralDescriptionVersionBase result = null;
            var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var query = unitOfWork.ApplyIncludes(statutoryGeneralDescriptionRep.All()
                .Where(s => s.Id == versionId), q =>
                q.Include(i => i.Languages).ThenInclude(i => i.Language)
                .Include(i => i.Names)
                .Include(i => i.Descriptions)
                .Include(i => i.ServiceClasses).ThenInclude(i => i.ServiceClass).ThenInclude(i => i.Names)
                .Include(i => i.ServiceClasses).ThenInclude(i => i.ServiceClass).ThenInclude(i => i.Descriptions)
                .Include(i => i.OntologyTerms).ThenInclude(i => i.OntologyTerm).ThenInclude(i => i.Names)
                .Include(i => i.TargetGroups).ThenInclude(i => i.TargetGroup).ThenInclude(i => i.Names)
                .Include(i => i.LifeEvents).ThenInclude(i => i.LifeEvent).ThenInclude(i => i.Names)
                .Include(i => i.IndustrialClasses).ThenInclude(i => i.IndustrialClass).ThenInclude(i => i.Names)
                .Include(i => i.StatutoryServiceRequirements)
                .Include(i => i.Type)
                .Include(i => i.ChargeType)
                .Include(i => i.StatutoryServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.Names)
                .Include(i => i.StatutoryServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.WebPages).ThenInclude(i => i.WebPage)
                .Include(i => i.LanguageAvailabilities)
                .Include(i => i.UnificRoot));

            var gd = query.FirstOrDefault();
            if (gd == null) return null;

            if (getOnlyPublished)
            {
                FilterOutNotPublishedLanguageVersions(gd, publishedId);
            }

            result = translationManager.Translate<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(gd);

            // Set connection related data outside of translator
            var connectionRep = unitOfWork.CreateRepository<IGeneralDescriptionServiceChannelRepository>();
            var connectionQuery = connectionRep.All().Where(c => c.StatutoryServiceGeneralDescriptionId == gd.UnificRootId && c.ServiceChannel.Versions.Any(v => v.PublishingStatusId == publishedId && v.LanguageAvailabilities.Any(l => l.StatusId == publishedId)));
            var gdConnections = unitOfWork.ApplyIncludes(connectionQuery, q =>
                q.Include(i => i.ServiceChannel)
                .Include(i => i.GeneralDescriptionServiceChannelDescriptions)
                .Include(i => i.GeneralDescriptionServiceChannelDigitalAuthorizations).ThenInclude(i => i.DigitalAuthorization).ThenInclude(i => i.Names)
                ).ToList();
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


            if (result == null) return null;

            // Get the right open api view model version
            return GetEntityByOpenApiVersion(result as IVmOpenApiGeneralDescriptionVersionBase, openApiVersion);
        }

        private IList<IVmOpenApiGeneralDescriptionVersionBase> GetGeneralDescriptionsWithDetails(IUnitOfWork unitOfWork, IList<Expression<Func<StatutoryServiceGeneralDescriptionVersioned, bool>>> filters, int openApiVersion)
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
                .Include(i => i.ServiceClasses).ThenInclude(i => i.ServiceClass).ThenInclude(i => i.Names)
                .Include(i => i.ServiceClasses).ThenInclude(i => i.ServiceClass).ThenInclude(i => i.Descriptions)
                .Include(i => i.OntologyTerms).ThenInclude(i => i.OntologyTerm).ThenInclude(i => i.Names)
                .Include(i => i.TargetGroups).ThenInclude(i => i.TargetGroup).ThenInclude(i => i.Names)
                .Include(i => i.LifeEvents).ThenInclude(i => i.LifeEvent).ThenInclude(i => i.Names)
                .Include(i => i.IndustrialClasses).ThenInclude(i => i.IndustrialClass).ThenInclude(i => i.Names)
                .Include(i => i.StatutoryServiceRequirements)
                .Include(i => i.Type)
                .Include(i => i.ChargeType)
                .Include(i => i.StatutoryServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.Names)
                .Include(i => i.StatutoryServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.WebPages).ThenInclude(i => i.WebPage)
                .Include(i => i.LanguageAvailabilities)
                .Include(i => i.UnificRoot))
                .ToList();

            gds.ForEach(gd =>
            {
                FilterOutNotPublishedLanguageVersions(gd, publishedId);
            });

            result = translationManager.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(gds).ToList();

            // Set connection related data outside of translator
            List<Guid> rootIds = result.Where(c => c.Id != null).Select(c => c.Id.Value).Distinct().ToList();
            var connectionRep = unitOfWork.CreateRepository<IGeneralDescriptionServiceChannelRepository>();
            var connectionQuery = connectionRep.All().Where(c => rootIds.Contains(c.StatutoryServiceGeneralDescriptionId) && c.ServiceChannel.Versions.Any(v => v.PublishingStatusId == publishedId && v.LanguageAvailabilities.Any(l => l.StatusId == publishedId)));
            var gdConnections = unitOfWork.ApplyIncludes(connectionQuery, q =>
                q.Include(i => i.ServiceChannel)
                .Include(i => i.GeneralDescriptionServiceChannelDescriptions)
                .Include(i => i.GeneralDescriptionServiceChannelDigitalAuthorizations).ThenInclude(i => i.DigitalAuthorization).ThenInclude(i => i.Names)
                ).ToList();
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


            if (result == null) return null;

            // Get the right open api view model version
            var versionList = new List<IVmOpenApiGeneralDescriptionVersionBase>();
            result.ForEach(gd => versionList.Add(GetEntityByOpenApiVersion(gd as IVmOpenApiGeneralDescriptionVersionBase, openApiVersion)));
            return versionList;
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
            // Connection description
            vm.Description = GetDescriptions(gdsc.GeneralDescriptionServiceChannelDescriptions, typesCache, languageCache);
            // Digital authorizations
            if (gdsc.GeneralDescriptionServiceChannelDigitalAuthorizations?.Count > 0)
            {
                gdsc.GeneralDescriptionServiceChannelDigitalAuthorizations.ForEach(da =>
                {
                    if (da != null && da.DigitalAuthorization != null)
                    {
                        vm.DigitalAuthorizations.Add(da.DigitalAuthorization.GetOpenApiModel(languageCache));
                    }
                });
            }

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
                    law.Law.WebPages = law.Law.WebPages.Where(i => !notPublishedLanguageVersions.Contains(i.WebPage.LocalizationId)).ToList();
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
                var affected = CommonService.PublishEntity<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(unitOfWork, model);

                //Update service names
                UpdateRelatedServiceNames(unitOfWork, model, affected);
                return affected;
            });
            return ContextManager.ExecuteReader(unitOfWork => GetGeneralDescriptionHeader(result.Id, unitOfWork));
        }

        public VmEntityHeaderBase ScheduleGeneralDescription(IVmLocalizedEntityModel model)
        {
            if (!model.Id.IsAssigned()) return null; 
            Guid? gdId = model.Id;
            var result = ContextManager.ExecuteWriter(unitOfWork =>
            {
                //Validate mandatory values
                if (model.PublishAction == PublishActionTypeEnum.SchedulePublish)
                {
                    var validationMessages =
                        ValidationManager.CheckEntity<StatutoryServiceGeneralDescriptionVersioned>(gdId.Value, unitOfWork, model);
                    if (validationMessages.Any())
                    {
                        throw new SchedulePublishException();
                    }
                }

                //Schedule publish/archive
                return CommonService.SchedulePublishArchiveEntity<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(unitOfWork, model);
            });
            return ContextManager.ExecuteReader(unitOfWork => GetGeneralDescriptionHeader(result.Id, unitOfWork));
        }

        private void UpdateRelatedServiceNames(IUnitOfWorkWritable unitOfWork, IVmLocalizedEntityModel model, PublishingResult affected)
        {
            var unificRootId =
                versioningManager.GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, affected.Id);
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

            var lastPublishedGeneralDescription = unitOfWork.ApplyIncludes(gdRep.All() //Set as param
                        .Where(x => x.Id == entityId),
                    i => i.Include(x => x.Names))
                .FirstOrDefault();

            var serviceRootIds = serviceRep.All()
                .Where(x => x.StatutoryServiceGeneralDescriptionId == generalDescriptionRootId &&
                            x.ServiceNames.Any(y => y.Inherited)).Select(y => y.UnificRootId)
                            .Distinct().ToList();

            var serviceVersionedIds = serviceRootIds
                .Select(x => versioningManager.GetLastPublishedModifiedDraftVersion<ServiceVersioned>(unitOfWork, x)?.EntityId)
                .WhereNotNull()
                .ToList();
            
            var services = serviceRep.All()
                .Include(x => x.ServiceNames)
                .Include(x => x.LanguageAvailabilities)
                .Where(x => serviceVersionedIds.Contains(x.Id)).ToList();
            
            //Change names by inherited, publishedLanguages
            foreach (var service in services)
            {
                service.ServiceNames.Where(y => updatedPublishedLanguageIds.Contains(y.LocalizationId) && y.Inherited).ToList().ForEach(sn =>
                    {
                        if (lastPublishedGeneralDescription != null)
                        {
                            sn.Name = lastPublishedGeneralDescription.Names.Where(x => x.LocalizationId == sn.LocalizationId).Select(y => y.Name).FirstOrDefault() ?? string.Empty;
                        }
                    }
                );
                
                var vmNamesToUpdate = TranslationManagerToVm.Translate<IBaseInformation, VmEntityHeaderBase>(service).Name;
                TranslationManagerToEntity.Translate<VmEntityUpdateName, ServiceVersioned>(new VmEntityUpdateName() { Id = service.Id, Name = vmNamesToUpdate }, unitOfWork);
            }

            unitOfWork.Save();
        }

        private bool CheckAndDistributeGeneralDescriptionNameToRelatedServices(IUnitOfWorkWritable unitOfWork, Guid unificRootId, Guid entityId)
        {
            var gdRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();

            var gdLastOldPublishedId = versioningManager.GetLastOldPublishedVersion<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, unificRootId)?.EntityId;
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

        public VmGeneralDescriptionHeader DeleteGeneralDescription(Guid entityId)
        {
            return ExecuteDelete(entityId, GetGeneralDescriptionHeader, unitOfWork => OnDeletingGeneralDescription(unitOfWork, entityId));
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
            return new VmEntityStatusBase() { PublishingStatusId = result.Id };
        }

        public IVmEntityBase LockGeneralDescription(Guid id, bool isLockDisAllowedForArchived = false)
        {
            return Utilities.LockEntityVersioned<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(id, isLockDisAllowedForArchived);
        }

        public IVmEntityBase UnLockGeneralDescription(Guid id)
        {
            return Utilities.UnLockEntityVersioned<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(id);
        }

        public IVmOpenApiGeneralDescriptionVersionBase AddGeneralDescription(IVmOpenApiGeneralDescriptionInVersionBase vm, bool allowAnonymous, int openApiVersion)
        {
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = Utilities.GetRelationIdForExternalSource();
            StatutoryServiceGeneralDescriptionVersioned generalDescription = null;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                generalDescription = TranslationManagerToEntity.Translate<IVmOpenApiGeneralDescriptionInVersionBase, StatutoryServiceGeneralDescriptionVersioned>(vm, unitOfWork);
                var generalDescriptionRepository = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                generalDescriptionRepository.Add(generalDescription);
                commonService.AddHistoryMetaData<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(generalDescription);
                unitOfWork.Save(saveMode);
            });

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = CommonService.PublishAllAvailableLanguageVersions<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(generalDescription.Id, i => i.StatutoryServiceGeneralDescriptionVersionedId == generalDescription.Id);
            }

            return GetGeneralDescriptionWithDetails(generalDescription.Id, openApiVersion, false);
        }

        public IVmOpenApiGeneralDescriptionVersionBase SaveGeneralDescription(IVmOpenApiGeneralDescriptionInVersionBase vm, int openApiVersion)
        {
            StatutoryServiceGeneralDescriptionVersioned generalDescription = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var generalDescriptionRepository = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var rootId = vm.Id;

                // Set the current version id
                vm.Id = vm.CurrentVersionId.HasValue ? vm.CurrentVersionId : versioningManager.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, vm.Id.Value, null, false);

                if (vm.PublishingStatus == PublishingStatus.Deleted.ToString())
                {
                    // Check if any services are attached into general description
                    var serviceVersionedRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                    var relatedServices = serviceVersionedRep.All()
                        .Where(x => x.StatutoryServiceGeneralDescriptionId == rootId).ToList();
                    if (relatedServices?.Count > 0)
                    {
                        throw new Exception($"There are services attached into general description {rootId}. You cannot delete/archive this general description!");
                    }
                    var publishStatus = TranslationManagerToEntity.Translate<String, PublishingStatusType>(PublishingStatus.Deleted.ToString(), unitOfWork);
                    generalDescription = generalDescriptionRepository.All().Single(i => i.Id == vm.Id.Value);
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
                            var publishingResult = CommonService.RestoreArchivedEntity<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(unitOfWork, vm.Id.Value);
                        }
                    }

                    generalDescription = TranslationManagerToEntity.Translate<IVmOpenApiGeneralDescriptionInVersionBase, StatutoryServiceGeneralDescriptionVersioned>(vm, unitOfWork);

                    if (vm.CurrentPublishingStatus == PublishingStatus.Draft.ToString())
                    {
                        // We need to manually remove items from collections!
                        if (vm.Languages?.Count > 0)
                        {
                            generalDescription.Languages = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, generalDescription.Languages,
                                query => query.StatutoryServiceGeneralDescriptionVersionedId == generalDescription.Id, language => language.LanguageId);
                        }
                        if (vm.ServiceClasses?.Count > 0)
                        {
                            generalDescription.ServiceClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, generalDescription.ServiceClasses,
                                query => query.StatutoryServiceGeneralDescriptionVersionedId == generalDescription.Id, serviceClass => serviceClass.ServiceClass != null ? serviceClass.ServiceClass.Id : serviceClass.ServiceClassId);
                        }
                        if (vm.TargetGroups?.Count > 0)
                        {
                            generalDescription.TargetGroups = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, generalDescription.TargetGroups,
                                query => query.StatutoryServiceGeneralDescriptionVersionedId == generalDescription.Id, targetGroup => targetGroup.TargetGroup != null ? targetGroup.TargetGroup.Id : targetGroup.TargetGroupId);
                        }
                        if (vm.DeleteAllLifeEvents || (vm.LifeEvents?.Count > 0))
                        {
                            var updatedEvents = generalDescription.LifeEvents.Select(l => l.LifeEventId).ToList();
                            var rep = unitOfWork.CreateRepository<IStatutoryServiceLifeEventRepository>();
                            var currentItems = rep.All().Where(s => s.StatutoryServiceGeneralDescriptionVersionedId == generalDescription.Id).ToList();
                            var toRemove = currentItems.Where(e => !updatedEvents.Contains(e.LifeEventId));
                            toRemove.ForEach(i => rep.Remove(i));
                        }
                        if (vm.DeleteAllIndustrialClasses || (vm.IndustrialClasses?.Count > 0))
                        {
                            var updatedClasses = generalDescription.IndustrialClasses.Select(l => l.IndustrialClassId).ToList();
                            var rep = unitOfWork.CreateRepository<IStatutoryServiceIndustrialClassRepository>();
                            var currentItems = rep.All().Where(s => s.StatutoryServiceGeneralDescriptionVersionedId == generalDescription.Id).ToList();
                            var toRemove = currentItems.Where(e => !updatedClasses.Contains(e.IndustrialClassId));
                            toRemove.ForEach(i => rep.Remove(i));
                        }
                        if (vm.OntologyTerms?.Count > 0)
                        {
                            generalDescription.OntologyTerms = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, generalDescription.OntologyTerms,
                                query => query.StatutoryServiceGeneralDescriptionVersionedId == generalDescription.Id, term => term.OntologyTerm != null ? term.OntologyTerm.Id : term.OntologyTermId);
                        }
                        if (vm.DeleteAllLaws || vm.Legislation?.Count > 0)
                        {
                            // Delete all law related names and web pages that were not included in vm
                            List<Guid> updatedGDLaws = generalDescription.StatutoryServiceLaws.Select(l => l.LawId).ToList();
                            List<Law> updatedLaws = generalDescription.StatutoryServiceLaws.Select(l => l.Law).ToList();
                            var rep = unitOfWork.CreateRepository<IStatutoryServiceLawRepository>();
                            var lawRep = unitOfWork.CreateRepository<ILawRepository>();
                            var lawNameRep = unitOfWork.CreateRepository<ILawNameRepository>();
                            var webPageRep = unitOfWork.CreateRepository<IWebPageRepository>();
                            var currentGDLaws = unitOfWork.ApplyIncludes(rep.All().Where(s => s.StatutoryServiceGeneralDescriptionVersionedId == generalDescription.Id), q => q.Include(i => i.Law)).ToList();
                            currentGDLaws.ForEach(l =>
                            {
                                if (updatedGDLaws.Contains(l.LawId))
                                {
                                    // Check names and webPages lists for removed items
                                    var updatedLaw = updatedLaws.Where(s => s.Id == l.LawId).FirstOrDefault();
                                    var updatedWebPages = updatedLaw.WebPages.Select(w => w.WebPageId).ToList();

                                    var updatedNames = updatedLaw.Names.Select(n => new { n.LawId, n.LocalizationId }).ToList();
                                    var currentLaw = unitOfWork.ApplyIncludes(lawRep.All().Where(w => w.Id == l.LawId), q => q.Include(i => i.Names).Include(i => i.WebPages)).FirstOrDefault();
                                    // Delete the web pages that were not included in updated webpages
                                    currentLaw.WebPages.Where(w => !updatedWebPages.Contains(w.WebPageId)).ForEach(w => webPageRep.Remove(w.WebPage));
                                    // Delete all names that were not included in updated names

                                    currentLaw.Names.Where(n => !updatedNames.Any(un => un.LawId == n.LawId && un.LocalizationId == n.LocalizationId)).ForEach(n => lawNameRep.Remove(n));
                                }
                                else
                                {
                                    // The item was removed from service laws so let's remove all webPages and names also.
                                    l.Law.WebPages.ForEach(w => webPageRep.Remove(w.WebPage));
                                    l.Law.Names.ForEach(n => lawNameRep.Remove(n));
                                    lawRep.Remove(l.Law);
                                }
                            });
                        }
                    }
                }
                commonService.AddHistoryMetaData<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(generalDescription, setByEntity: true);
                unitOfWork.Save(parentEntity: generalDescription);
            });

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = CommonService.PublishAllAvailableLanguageVersions<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(generalDescription.Id, i => i.StatutoryServiceGeneralDescriptionVersionedId == generalDescription.Id);
            }

            return GetGeneralDescriptionWithDetails(generalDescription.Id, openApiVersion, false);
        }

        public VmGeneralDescriptionOutput SaveGeneralDescription(VmGeneralDescriptionInput model)
        {
            return ExecuteSave
            (
                unitofWork => SaveGeneralDescription(unitofWork, model),
                (unitOfWork, entity) => GetGeneralDescription(unitOfWork, new VmGeneralDescriptionGet() { Id = entity.Id })
            );
        }

        private StatutoryServiceGeneralDescriptionVersioned SaveGeneralDescription(IUnitOfWorkWritable unitOfWork, VmGeneralDescriptionInput model)
        {
            CheckDescriptionType(unitOfWork, model);
            
            var entity = TranslationManagerToEntity.Translate<VmGeneralDescriptionInput, StatutoryServiceGeneralDescriptionVersioned>(model, unitOfWork);
            
            if (model.Id.IsAssigned())
            {
                foreach (var language in model.LanguagesAvailabilities)
                {
                    translationService.ConfirmGeneralDescriptionDeliveredTranslation(unitOfWork, model.Id.Value, language.LanguageId);
                }
            }
            commonService.AddHistoryMetaData<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(entity);

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
            result.PreviousInfo = generalDescriptionId.HasValue ? Utilities.CheckIsEntityEditable<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(generalDescriptionId.Value, unitOfWork) : null;
            if (generalDescriptionId.HasValue)
            {
                var unificRootId = versioningManager.GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, generalDescriptionId.Value);
                if (unificRootId.HasValue)
                {
                    result.TranslationAvailability = translationService.GetGeneralDescriptionTranslationAvailabilities(unitOfWork, generalDescriptionId.Value, unificRootId.Value);
                }
            }

            return result;
        }

        public VmGeneralDescriptionOutput SaveAndValidateGeneralDescription(VmGeneralDescriptionInput model)
        {
            var result = ExecuteSaveAndValidate
            (
                model,
                unitOfWork => SaveGeneralDescription(unitOfWork, model),
                (unitOfWork, entity) => GetGeneralDescription(unitOfWork, new VmGeneralDescriptionGet() { Id = entity.Id })
            );

            return result;
        }

        private StatutoryServiceGeneralDescriptionVersioned OnDeletingGeneralDescription(IUnitOfWorkWritable unitOfWork, Guid? entityId)
        {
            var psPublishedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            var generalDescription = generalDescriptionRep.All()
                .Include(x => x.TargetGroups)
                .Include(x => x.ServiceClasses)
                .Include(x => x.LifeEvents)
                .Include(x => x.OntologyTerms)
                .Include(x => x.IndustrialClasses)
                .Single(x => x.Id == entityId.Value);

            var originalPublishingStatusId = generalDescription.PublishingStatusId;
//            generalDescription =
//                CommonService.ChangeEntityToDeleted<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork,
//                    entityId.Value);

            if(originalPublishingStatusId == psPublishedId)
            {
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
                    TryWithdrawEntity(unitOfWork, x);
                    x.TypeId = generalDescription.TypeId;
                    x.ChargeTypeId = generalDescription.ChargeTypeId;
                    CopyClassificationData(unitOfWork, x, generalDescription);
                    x.StatutoryServiceGeneralDescriptionId = null;
                });
            }
            return generalDescription;
        }

        private bool TryWithdrawEntity<TEntity>(IUnitOfWorkWritable unitOfWork, TEntity entity ) where TEntity : class, IEntityIdentifier, IVersionedVolume
        {
            try
            {
                versioningManager.ChangeToModified(unitOfWork, entity, new List<PublishingStatus>() { PublishingStatus.Published });
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
                .ForEach(x => dest.ServiceTargetGroups.Add(new ServiceTargetGroup() { ServiceVersionedId = dest.Id, TargetGroupId = x.TargetGroupId }));

            var stgRep = unitOfWork.CreateRepository<IServiceTargetGroupRepository>();
            overideTargetGroup.ForEach(tg => stgRep.Remove(tg));

            var serviceClassesIds = dest.ServiceServiceClasses.Select(x => x.ServiceClassId);
            source.ServiceClasses
                .Where(x => !serviceClassesIds.Contains(x.ServiceClassId))
                .ForEach(x => dest.ServiceServiceClasses.Add(new ServiceServiceClass() { ServiceVersionedId = dest.Id, ServiceClassId = x.ServiceClassId }));

            var ontologyTermsIds = dest.ServiceOntologyTerms.Select(x => x.OntologyTermId);
            source.OntologyTerms
                .Where(x => !ontologyTermsIds.Contains(x.OntologyTermId))
                .ForEach(x => dest.ServiceOntologyTerms.Add(new ServiceOntologyTerm() { ServiceVersionedId = dest.Id, OntologyTermId = x.OntologyTermId }));

            var lifeEventsIds = dest.ServiceLifeEvents.Select(x => x.LifeEventId);
            source.LifeEvents
                .Where(x => !lifeEventsIds.Contains(x.LifeEventId))
                .ForEach(x => dest.ServiceLifeEvents.Add(new ServiceLifeEvent() { ServiceVersionedId = dest.Id, LifeEventId = x.LifeEventId }));

            var industrialClassesIds = dest.ServiceIndustrialClasses.Select(x => x.IndustrialClassId);
            source.IndustrialClasses
                .Where(x => !industrialClassesIds.Contains(x.IndustrialClassId))
                .ForEach(x => dest.ServiceIndustrialClasses.Add(new ServiceIndustrialClass() { ServiceVersionedId = dest.Id, IndustrialClassId = x.IndustrialClassId }));
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
        public VmConnectionsOutput SaveRelations(VmConnectionsInput model)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                utilities.CheckIsEntityConnectable<StatutoryServiceGeneralDescriptionVersioned>(model.Id, unitOfWork);
                SaveRelations(unitOfWork, model);
                unitOfWork.Save();
            });
            return GetRelations(model);
        }

        private void SaveRelations(IUnitOfWorkWritable unitOfWork, VmConnectionsInput model)
        {
            var unificRootId = versioningManager.GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, model.Id);
            if (unificRootId.HasValue)
            {
                model.UnificRootId = unificRootId.Value;
                TranslationManagerToEntity.Translate<VmConnectionsInput, StatutoryServiceGeneralDescription>(model, unitOfWork);
            }
         }

        private VmConnectionsOutput GetRelations(VmConnectionsInput model)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                return GetRelations(unitOfWork, model);
            });
        }

        private VmConnectionsOutput GetRelations(IUnitOfWork unitOfWork, VmConnectionsInput model)
        {
            var gdLangAvailabilitiesRep = unitOfWork.CreateRepository<IGeneralDescriptionLanguageAvailabilityRepository>();
            var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionRepository>();
            var unificRootId = versioningManager.GetUnificRootId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, model.Id);
            if (unificRootId.HasValue)
            {
                var generalDescription = generalDescriptionRep.All()
                                    .Include(j => j.StatutoryServiceGeneralDescriptionServiceChannels).ThenInclude(j => j.ServiceChannel).ThenInclude(j => j.Versions).ThenInclude(j => j.ServiceChannelNames)
                                    .Include(j => j.StatutoryServiceGeneralDescriptionServiceChannels).ThenInclude(j => j.ServiceChannel).ThenInclude(j => j.Versions).ThenInclude(j => j.LanguageAvailabilities)
                                    .Include(j => j.StatutoryServiceGeneralDescriptionServiceChannels).ThenInclude(j => j.GeneralDescriptionServiceChannelDescriptions)
                                    .Include(j => j.StatutoryServiceGeneralDescriptionServiceChannels).ThenInclude(j => j.GeneralDescriptionServiceChannelDigitalAuthorizations).ThenInclude(j => j.DigitalAuthorization)
                                    .Single(x => x.Id == unificRootId.Value);
                var result = TranslationManagerToVm.Translate<StatutoryServiceGeneralDescription, VmConnectionsOutput>(generalDescription);
                result.Id = model.Id;
                result.LanguagesAvailabilities = TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(gdLangAvailabilitiesRep.All().Where(x => model.Id == x.StatutoryServiceGeneralDescriptionVersionedId).OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());
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
                GeneralDescriptions = new List<VmGeneralDescriptionOutput>() { GetGeneralDescription(unitOfWork, new VmGeneralDescriptionGet() { Id = entityId })},
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
    }
}
