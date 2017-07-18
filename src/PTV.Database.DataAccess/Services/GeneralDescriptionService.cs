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
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Linq.Expressions;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Utils;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using VmGeneralDescription = PTV.Domain.Model.Models.VmGeneralDescription;
using VmGeneralDescriptionListItem = PTV.Domain.Model.Models.VmGeneralDescriptionListItem;
using VmGeneralDescriptions = PTV.Domain.Model.Models.VmGeneralDescriptions;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IGeneralDescriptionService), RegisterType.Transient)]
    internal class GeneralDescriptionService : ServiceBase, IGeneralDescriptionService
    {
        private readonly ITranslationEntity translationManager;
        private ITranslationViewModel translationManagerVModel;
        private readonly IContextManager contextManager;
        private readonly ILogger logger;
        private readonly ServiceUtilities utilities;
        private ICommonServiceInternal commonService;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private IVersioningManager versioningManager;
        private DataUtils dataUtils;

        public GeneralDescriptionService(
            IContextManager contextManager,
            IUserIdentification userIdentification,
            ITranslationEntity translationManager,
            ITranslationViewModel translationManagerVModel,
            ILogger<GeneralDescriptionService> logger,
            ServiceUtilities utilities,
            ICommonServiceInternal commonService,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            ILanguageCache languageCache,
            ITypesCache typesCache,
            IVersioningManager versioningManager,
            DataUtils dataUtils)
            : base(translationManager, translationManagerVModel, publishingStatusCache, userOrganizationChecker)
        {
            this.translationManager = translationManager;
            this.contextManager = contextManager;
            this.translationManagerVModel = translationManagerVModel;
            this.logger = logger;
            this.utilities = utilities;
            this.commonService = commonService;
            this.typesCache = typesCache;
            this.languageCache = languageCache;
            this.versioningManager = versioningManager;
            this.dataUtils = dataUtils;
        }

        public VmGeneralDescriptionSearchForm GetGeneralDescriptionSearchForm()
        {
            VmGeneralDescriptionSearchForm result = new VmGeneralDescriptionSearchForm();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var targetGroupRep = unitOfWork.CreateRepository<ITargetGroupRepository>();
                var serviceClassesRep = unitOfWork.CreateRepository<IServiceClassRepository>();

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("TargetGroups", CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<TargetGroup, TargetGroupName>(unitOfWork, targetGroupRep.All().OrderBy(x => x.Label))), x => x.Code)),
                    () => GetEnumEntityCollectionModel("ServiceClasses", CreateList<VmListItem>(GetIncludesForFinto<ServiceClass, ServiceClassName>(unitOfWork, serviceClassesRep.All()), x => x.Name))
                    //() => GetEnumEntityCollectionModel("LifeEvents", CreateTree<VmTreeItem>(lifeEventsRep.All().OrderBy(x => x.Label))),
                    //() => GetEnumEntityCollectionModel("IndustrialClasses", CreateTree<VmTreeItem>(industrialClassesRep.All().OrderBy(x => x.Label)))
                );
            });
            return result;
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

        [Obsolete("Should not be used anymore, after refactoring of adding gd to service")]
        private VmGeneralDescriptions FilteredSearchGeneralDescriptions(VmGeneralDescriptionSearchForm searchData)
        {
            VmGeneralDescriptions result = new VmGeneralDescriptions();
            //var count = 0;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var psPublished = PublishingStatusCache.Get(PublishingStatus.Published);
                var descriptions = statutoryGeneralDescriptionRep.All().Where(i => i.PublishingStatusId == psPublished);
                if (!string.IsNullOrEmpty(searchData.Name))
                {                    
                    var rootId = GetRootIdFromString(searchData.Name);
                    if (!rootId.HasValue)
                    {
                        searchData.Name = searchData.Name.ToLower();
                        descriptions = descriptions.Where(i => i.Names.Select(j => j.Name.ToLower()).Any(m => m.Contains(searchData.Name)));
                    }
                    else
                    {
                        descriptions = descriptions
                            .Where(description =>
                                description.UnificRootId == rootId
                            );
                    }
                }
                if (searchData.ServiceClassId != null)
                {
                    descriptions = descriptions.Where(i => i.ServiceClasses.Any(j => j.ServiceClassId == searchData.ServiceClassId.Value));
                }
                if (searchData.TargetGroupId != null)
                {
                    descriptions = descriptions.Where(i => i.TargetGroups.Any(j => j.TargetGroupId == searchData.TargetGroupId.Value));
                }
                if ((searchData.TargetGroupId != null) && (searchData.SubTargetGroupId != null))
                {
                    descriptions = descriptions.Where(i => i.TargetGroups.Any(j => j.TargetGroupId == searchData.SubTargetGroupId.Value));
                }

                //count = descriptions.Count();
                descriptions = descriptions.OrderBy(i => i.Id);
                descriptions = searchData.PageNumber > 0
                    ? descriptions.Skip(searchData.PageNumber * CoreConstants.MaximumNumberOfAllItems).Take(CoreConstants.MaximumNumberOfAllItems)
                    : descriptions.Take(CoreConstants.MaximumNumberOfAllItems);

                descriptions = unitOfWork.ApplyIncludes(descriptions, i => i
                    .Include(j => j.Names)
                    .Include(j => j.ServiceClasses)
                    .Include(j => j.TargetGroups)
                    .Include(j => j.Descriptions)
                    .Include(j => j.LanguageAvailabilities).ThenInclude(j => j.Language)
                    .Include(j => j.Versioning)
                );
                //result.Count = count;
                result.PageNumber = ++searchData.PageNumber;
                result.GeneralDescriptions = translationManager.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionListItem>(descriptions);
            });
            return result;
        }

        public Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptions SearchGeneralDescriptions_V2(VmGeneralDescriptionSearchForm searchData)
        {
            var result = new Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptions();
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var shortDescriptionTypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString());
            var languagesIds = searchData.Languages.Select(language => languageCache.Get(language.ToString())).ToList();
            bool moreAvailable = false;
            var count = 0;

            contextManager.ExecuteReader(unitOfWork =>
            {
                var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();               
                var descriptions = statutoryGeneralDescriptionRep.All();

                #region FilteringData

                if (!string.IsNullOrEmpty(searchData.Name))
                {
                    var rootId = GetRootIdFromString(searchData.Name);
                    if (!rootId.HasValue)
                    {
                        descriptions = descriptions
                            .Where(i => i.Names.Any(
                                y => y.Name.ToLower().Contains(searchData.Name.ToLower()) &&
                                languagesIds.Contains(y.LocalizationId)
                            ));
                    }
                    else
                    {
                        descriptions = descriptions
                            .Where(description =>
                                description.UnificRootId == rootId
                            );
                    }
                }
                else
                {
                    descriptions =
                        descriptions.Where(
                            x =>
                                x.Names.Any(
                                    y =>
                                        languagesIds.Contains(y.LocalizationId) &&
                                        !string.IsNullOrEmpty(y.Name)));
                }
                if (searchData.ServiceClassId != null)
                {
                    descriptions = descriptions.Where(i => i.ServiceClasses.Any(j => j.ServiceClassId == searchData.ServiceClassId.Value));
                }
                if (searchData.TargetGroupId != null)
                {
                    descriptions = descriptions.Where(i => i.TargetGroups.Any(j => j.TargetGroupId == searchData.TargetGroupId.Value));
                }
                if ((searchData.TargetGroupId != null) && (searchData.SubTargetGroupId != null))
                {
                    descriptions = descriptions.Where(i => i.TargetGroups.Any(j => j.TargetGroupId == searchData.SubTargetGroupId.Value));
                }

                if (searchData.SelectedPublishingStatuses != null)
                {
                    commonService.ExtendPublishingStatusesByEquivalents(searchData.SelectedPublishingStatuses);
                    descriptions = descriptions.WherePublishingStatusIn(searchData.SelectedPublishingStatuses);
                }

                #endregion FilteringData

                count = descriptions.Count();
                moreAvailable = count > (searchData.PageNumber.PositiveOrZero() == 0
                    ? CoreConstants.MaximumNumberOfAllItems
                    : searchData.PageNumber.PositiveOrZero() * CoreConstants.MaximumNumberOfAllItems);

                var descriptionsTemp = descriptions.Select(i => new
                {
                    Id = i.Id,
                    UnificRootId = i.UnificRootId,
                    PublishStatusId = i.PublishingStatusId,                    
                    Names = i.Names.Where(x => x.TypeId == nameTypeId),
                    ShortDescriptions = i.Descriptions.Where(x => x.TypeId == shortDescriptionTypeId),
                    ServiceClasses = i.ServiceClasses.Select(x => x.ServiceClassId),
                    Versioning = i.Versioning,
                    LanguageAvailabilities = i.LanguageAvailabilities.OrderBy(x => x.Language.OrderNumber),
                    //Sort
                    Name = i.Names.OrderBy(x => x.Localization.OrderNumber).FirstOrDefault(x => languagesIds.Contains(x.LocalizationId) && x.TypeId == nameTypeId).Name,
                    ShortDescription = i.Descriptions.OrderBy(x => x.Localization.OrderNumber).FirstOrDefault(x => languagesIds.Contains(x.LocalizationId) && x.TypeId == shortDescriptionTypeId).Description,
                    VersionMajor = i.Versioning.VersionMajor,
                    VersionMinor = i.Versioning.VersionMinor,
                    Modified = i.Modified
                })
                .ApplySortingByVersions(searchData.SortData, new VmSortParam() { Column = "Modified", SortDirection = SortDirectionEnum.Desc })
                .ApplyPagination(searchData.PageNumber)
                .ToList();
                
                result.GeneralDescriptions = descriptionsTemp.Select(i => new Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionListItem
                {
                    Id = i.Id,
                    UnificRootId = i.UnificRootId,
                    PublishingStatusId = i.PublishStatusId,
                    Name = i.Names.ToDictionary(y => languageCache.GetByValueEnum(y.LocalizationId).ToString(), y => y.Name), 
                    ShortDescription = i.ShortDescriptions.ToDictionary(y => languageCache.GetByValueEnum(y.LocalizationId).ToString(), y => y.Description),
                    ServiceClasses = i.ServiceClasses.ToList(),
                    LanguagesAvailabilities = translationManager.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(i.LanguageAvailabilities),
                    Version = TranslationManagerToVm.Translate<Versioning, VmVersion>(i.Versioning),
                })
                .ToList();

                result.PageNumber = ++searchData.PageNumber;
                result.MoreAvailable = moreAvailable;
                result.Count = count;
            });
            return result;
        }

        [Obsolete("Should not be used anymore, after refactoring of adding gd to service")]
        private VmGeneralDescriptions FullTextSearchGeneralDescriptions(VmGeneralDescriptionSearchForm vm)
        {
            var result = new VmGeneralDescriptions();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var generalDescriptionNameRepository = unitOfWork.CreateRepository<IStatutoryServiceNameRepository>();

                var languagesIds = vm.Languages.Select(language => languageCache.Get(language.ToString()));
                var generalDescriptionNames = generalDescriptionNameRepository.All();

                if (!string.IsNullOrEmpty(vm.Name))
                {                   
                    var rootId = GetRootIdFromString(vm.Name);
                    if (!rootId.HasValue)
                    {
                        generalDescriptionNames = generalDescriptionNames
                        .Where(x =>
                            x.Name.ToLower().Contains(vm.Name.ToLower()) &&
                            languagesIds.Contains(x.LocalizationId)
                        );
                    }
                    else
                    {
                        generalDescriptionNames = generalDescriptionNames
                            .Where(generalDescriptionName =>
                                generalDescriptionName.StatutoryServiceGeneralDescriptionVersioned.UnificRootId == rootId
                            );
                    }
                }
                else
                {
                    generalDescriptionNames = generalDescriptionNames
                        .Where(x =>
                            languagesIds.Contains(x.LocalizationId)
                        );
                }

                generalDescriptionNames = unitOfWork.ApplyIncludes(generalDescriptionNames, q =>
                    q.Include(i => i.StatutoryServiceGeneralDescriptionVersioned)
                        .ThenInclude(i => i.Names)
                    .Include(i => i.StatutoryServiceGeneralDescriptionVersioned)
                        .ThenInclude(i => i.ServiceClasses)
                    .Include(i => i.StatutoryServiceGeneralDescriptionVersioned)
                        .ThenInclude(i => i.TargetGroups)
                    .Include(i => i.StatutoryServiceGeneralDescriptionVersioned)
                        .ThenInclude(i => i.Descriptions)
                    .Include(i => i.StatutoryServiceGeneralDescriptionVersioned)
                        .ThenInclude(i => i.LanguageAvailabilities)
                        .ThenInclude(i => i.Language)
                    .Include(i => i.StatutoryServiceGeneralDescriptionVersioned)
                        .ThenInclude(i => i.Versioning)
                );

                if (vm.ServiceClassId != null)
                {
                    generalDescriptionNames = generalDescriptionNames
                        .Where(i => i.StatutoryServiceGeneralDescriptionVersioned.ServiceClasses.Any(j => j.ServiceClassId == vm.ServiceClassId.Value));
                }
                if (vm.TargetGroupId != null)
                {
                    generalDescriptionNames = generalDescriptionNames
                        .Where(i => i.StatutoryServiceGeneralDescriptionVersioned.TargetGroups.Any(j => j.TargetGroupId == vm.TargetGroupId.Value));
                }
                if ((vm.TargetGroupId != null) && (vm.SubTargetGroupId != null))
                {
                    generalDescriptionNames = generalDescriptionNames
                        .Where(i => i.StatutoryServiceGeneralDescriptionVersioned.TargetGroups.Any(j => j.TargetGroupId == vm.SubTargetGroupId.Value));
                }

                var names = generalDescriptionNames
                    .Skip(vm.Skip)
                    .Take(CoreConstants.MaximumNumberOfAllItems * vm.Languages.Count)
                    .ToList();
                var generalDescriptions = names
                    .Select(x => x.StatutoryServiceGeneralDescriptionVersioned)
                    .GroupBy(x => x.Id)
                    .Select(x => x.First())
                    .Take(CoreConstants.MaximumNumberOfAllItems)
                    .ToList();

                generalDescriptions.ForEach(x => { x.Names = x.Names.Intersect(names).ToList(); });

                result.Skip = generalDescriptions.Aggregate(0, (acc, generalDescription) => generalDescription.Names.Count + acc);

                result.GeneralDescriptions = TranslationManagerToVm.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionSearchListItem>(generalDescriptions);
            });
            return result;
        }

        private Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptions FullTextSearchGeneralDescriptions_V2(VmGeneralDescriptionSearchForm vm)
        {
            var result = new Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptions();
            bool moreAvailable = false;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var generalDescriptionNameRepository = unitOfWork.CreateRepository<IStatutoryServiceNameRepository>();

                var languagesIds = vm.Languages.Select(language => languageCache.Get(language.ToString()));
                var generalDescriptionNames = generalDescriptionNameRepository.All();

                if (!string.IsNullOrEmpty(vm.Name))
                {
                    var rootId = GetRootIdFromString(vm.Name);
                    if (!rootId.HasValue)
                    {
                        generalDescriptionNames = generalDescriptionNames
                        .Where(x =>
                            x.Name.ToLower().Contains(vm.Name.ToLower()) &&
                            languagesIds.Contains(x.LocalizationId)
                        );
                    }
                    else
                    {
                        generalDescriptionNames = generalDescriptionNames
                            .Where(generalDescriptionName =>
                                generalDescriptionName.StatutoryServiceGeneralDescriptionVersioned.UnificRootId == rootId
                            );
                    }
                }
                else
                {
                    generalDescriptionNames = generalDescriptionNames
                        .Where(x =>
                            languagesIds.Contains(x.LocalizationId)
                        );
                }

                generalDescriptionNames = unitOfWork.ApplyIncludes(generalDescriptionNames, q =>
                    q.Include(i => i.StatutoryServiceGeneralDescriptionVersioned)
                        .ThenInclude(i => i.Names)
                    .Include(i => i.StatutoryServiceGeneralDescriptionVersioned)
                        .ThenInclude(i => i.ServiceClasses)
                    .Include(i => i.StatutoryServiceGeneralDescriptionVersioned)
                        .ThenInclude(i => i.Descriptions)
                    .Include(i => i.StatutoryServiceGeneralDescriptionVersioned)
                        .ThenInclude(i => i.LanguageAvailabilities)
                        .ThenInclude(i => i.Language)
                    .Include(i => i.StatutoryServiceGeneralDescriptionVersioned)
                        .ThenInclude(i => i.Versioning)
                );

                if (vm.ServiceClassId != null)
                {
                    generalDescriptionNames = generalDescriptionNames
                        .Where(i => i.StatutoryServiceGeneralDescriptionVersioned.ServiceClasses.Any(j => j.ServiceClassId == vm.ServiceClassId.Value));
                }
                if (vm.TargetGroupId != null)
                {
                    generalDescriptionNames = generalDescriptionNames
                        .Where(i => i.StatutoryServiceGeneralDescriptionVersioned.TargetGroups.Any(j => j.TargetGroupId == vm.TargetGroupId.Value));
                }
                if ((vm.TargetGroupId != null) && (vm.SubTargetGroupId != null))
                {
                    generalDescriptionNames = generalDescriptionNames
                        .Where(i => i.StatutoryServiceGeneralDescriptionVersioned.TargetGroups.Any(j => j.TargetGroupId == vm.SubTargetGroupId.Value));
                }
                if (vm.SelectedPublishingStatuses != null)
                {
                    commonService.ExtendPublishingStatusesByEquivalents(vm.SelectedPublishingStatuses);
                    generalDescriptionNames = generalDescriptionNames.Where(x => vm.SelectedPublishingStatuses.Contains(x.StatutoryServiceGeneralDescriptionVersioned.PublishingStatusId));
                }

                var names = generalDescriptionNames
                    .Skip(vm.Skip)
                    .Take(CoreConstants.MaximumNumberOfAllItems * vm.Languages.Count)
                    .ToList();

                var appliedPaging = names.ApplyPaging(name => name.Select(x => x.StatutoryServiceGeneralDescriptionVersioned)
                        .GroupBy(x => x.Id)
                        .Select(x => x.First()));

                var generalDescriptions = appliedPaging.Data;

                moreAvailable = appliedPaging.MoreAvailable;
                
                generalDescriptions.ForEach(x => { x.Names = x.Names.Intersect(names).ToList(); });

                result.Skip = vm.Skip + generalDescriptions.Aggregate(0, (acc, generalDescription) => generalDescription.Names.Count + acc);
                result.MoreAvailable = moreAvailable;
                result.GeneralDescriptions = TranslationManagerToVm.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionListItem>(generalDescriptions);
            });
            return result;
        }

        [Obsolete("Should not be used anymore, after refactoring of adding gd to service")]
        public VmGeneralDescriptions SearchGeneralDescriptions(VmGeneralDescriptionSearchForm searchData)
        {
            var result = searchData.Languages?.Count > 1
                ? FullTextSearchGeneralDescriptions(searchData)
                : FilteredSearchGeneralDescriptions(searchData);
            return result;
        }

        public VmGeneralDescription GetGeneralDescriptionById(Guid id)
        {
            VmGeneralDescription result = new VmGeneralDescription();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var psPublished = PublishingStatusCache.Get(PublishingStatus.Published);
                var description = statutoryGeneralDescriptionRep.All().Where(x => x.UnificRootId == id && x.PublishingStatusId == psPublished);

                var query = unitOfWork.ApplyIncludes(description, i => i
                    .Include(j => j.Names)
                    .Include(j => j.ServiceClasses).ThenInclude(j => j.ServiceClass).ThenInclude(j => j.Names)
                    .Include(j => j.TargetGroups).ThenInclude(j => j.TargetGroup).ThenInclude(j => j.Names)
                    .Include(j => j.IndustrialClasses).ThenInclude(j => j.IndustrialClass).ThenInclude(x => x.Names)
                    .Include(j => j.LifeEvents).ThenInclude(j => j.LifeEvent).ThenInclude(x => x.Names)
                    .Include(j => j.OntologyTerms).ThenInclude(j => j.OntologyTerm).ThenInclude(x => x.Names)
                    .Include(j => j.Type)
                    .Include(j => j.ChargeType)
                    .Include(j => j.StatutoryServiceRequirements)
                    .Include(j => j.Descriptions)
                    .Include(j => j.PublishingStatus)
                    .Include(j => j.StatutoryServiceLaws).ThenInclude(j => j.Law).ThenInclude(j => j.Names)
                    .Include(j => j.StatutoryServiceLaws).ThenInclude(j => j.Law).ThenInclude(j => j.WebPages).ThenInclude(j => j.WebPage)
                );

                result = translationManager.TranslateFirst<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescription>(query);
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Laws", commonService.GetLaws(unitOfWork, result.Laws.Select(x => x.Id.Value).ToList()))
                );
            });
            return result;
        }

        public Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionDialog GetGeneralDescription_V2(VmGeneralDescriptionIn model)
        {
            var result = new Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionDialog();
            SetTranslatorLanguage(model);
            contextManager.ExecuteReader(unitOfWork =>
            {
                var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var psPublished = PublishingStatusCache.Get(PublishingStatus.Published);
                var description = statutoryGeneralDescriptionRep.All().Where(x => x.UnificRootId == model.Id && x.PublishingStatusId == psPublished);

                description = unitOfWork.ApplyIncludes(description, i => i
                    .Include(j => j.Names)
                    .Include(j => j.ServiceClasses)
                    .Include(j => j.TargetGroups)
                    .Include(j => j.Descriptions)
                    .Include(j => j.OntologyTerms).ThenInclude(j => j.OntologyTerm)
                );

                result = translationManager.TranslateFirst<StatutoryServiceGeneralDescriptionVersioned, Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionDialog>(description);
            });
            return result;
        }

        public IVmOpenApiGuidPageVersionBase V3GetGeneralDescriptions(DateTime? date, int pageNumber = 1, int pageSize = 1000)
        {
            return GetGeneralDescriptions(new V3VmOpenApiGuidPage(pageNumber, pageSize), date);
        }

        private IVmOpenApiGuidPageVersionBase GetGeneralDescriptions(IVmOpenApiGuidPageVersionBase vm, DateTime? date)
        {
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetItemPage<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(vm, date, unitOfWork, q => q.Include(i => i.Names), false);
            });

            return vm;
        }

        public IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionVersionBase(Guid id, int openApiVersion, bool getOnlyPublished = true)
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
                        entityId = versioningManager.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, id);
                    }

                    if (entityId.HasValue)
                    {
                        result = GetGeneralDescriptionWithDetails(unitOfWork, entityId.Value, openApiVersion);
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

        private IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionWithDetails(Guid versionId, int openApiVersion)
        {
            IVmOpenApiGeneralDescriptionVersionBase result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetGeneralDescriptionWithDetails(unitOfWork, versionId, openApiVersion);
            });
            return result;
        }

        private IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionWithDetails(IUnitOfWork unitOfWork, Guid versionId, int openApiVersion)
        {
            IVmOpenApiGeneralDescriptionVersionBase result = null;
            var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();

            var query = unitOfWork.ApplyIncludes(statutoryGeneralDescriptionRep.All()
                .Where(s => s.Id == versionId), q =>
                q.Include(i => i.Languages).ThenInclude(i => i.Language)
                .Include(i => i.Names)
                .Include(i => i.Descriptions)
                .Include(i => i.ServiceClasses).ThenInclude(i => i.ServiceClass).ThenInclude(i => i.Names)
                .Include(i => i.OntologyTerms).ThenInclude(i => i.OntologyTerm).ThenInclude(i => i.Names)
                .Include(i => i.TargetGroups).ThenInclude(i => i.TargetGroup).ThenInclude(i => i.Names)
                .Include(i => i.LifeEvents).ThenInclude(i => i.LifeEvent).ThenInclude(i => i.Names)
                .Include(i => i.IndustrialClasses).ThenInclude(i => i.IndustrialClass).ThenInclude(i => i.Names)
                .Include(i => i.StatutoryServiceRequirements)
                .Include(i => i.Type)
                .Include(i => i.ChargeType)
                .Include(i => i.StatutoryServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.Names)
                .Include(i => i.StatutoryServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.WebPages).ThenInclude(i => i.WebPage)
                .Include(i => i.PublishingStatus)
                .Include(i => i.LanguageAvailabilities));

            result = translationManager.TranslateFirst<StatutoryServiceGeneralDescriptionVersioned, VmOpenApiGeneralDescriptionVersionBase>(query);

            if (result == null) return null;

            // Get the right open api view model version
            return GetEntityByOpenApiVersion(result as IVmOpenApiGeneralDescriptionVersionBase, openApiVersion);
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

        public VmPublishingResultModel PublishGeneralDescription(VmPublishingModel model)
        {
            Guid generalDescriptionId = model.Id;
            var affected = commonService.PublishEntity<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(model);
            var result = new VmPublishingResultModel()
            {
                Id = generalDescriptionId,
                PublishingStatusId = affected.AffectedEntities.First(i => i.Id == generalDescriptionId).PublishingStatusNew,
                LanguagesAvailabilities = model.LanguagesAvailabilities,
                Version = affected.Version
            };
            FillEnumEntities(result, () => GetEnumEntityCollectionModel("GeneralDescriptions", affected.AffectedEntities.Select(i => new VmEntityStatusBase() { Id = i.Id, PublishingStatusId = i.PublishingStatusNew }).ToList<IVmBase>()));
            return result;
        }
        
        public VmPublishingResultModel WithdrawGeneralDescription(VmPublishingModel model)
        {
            return commonService.WithdrawEntity<StatutoryServiceGeneralDescriptionVersioned>(model.Id);
        }
        
        public VmPublishingResultModel RestoreGeneralDescription(VmPublishingModel model)
        {
            return commonService.RestoreEntity<StatutoryServiceGeneralDescriptionVersioned>(model.Id);
        }

        public IVmEntityBase DeleteGeneralDescription(Guid? entityId)
        {
            StatutoryServiceGeneralDescriptionVersioned result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = DeleteGeneralDescription(unitOfWork, entityId);
                unitOfWork.Save();

            });
            return new VmEntityStatusBase { Id = result.Id, PublishingStatusId = result.PublishingStatusId };
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

        public IVmEntityBase LockGeneralDescription(Guid id)
        {
            return utilities.LockEntityVersioned<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(id);
        }

        public IVmEntityBase UnLockGeneralDescription(Guid id)
        {
            return utilities.UnLockEntityVersioned<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(id);
        }

        public IVmEntityBase IsGeneralDescriptionLocked(Guid id)
        {
            return utilities.CheckIsEntityLocked<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(id);
        }

        public IVmEntityBase IsGeneralDescriptionEditable(Guid id)
        {
            return utilities.CheckIsEntityEditable<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(id);
        }

        public VmEntityNames GetGeneralDescriptionNames(VmEntityBase model)
        {
            var result = new VmEntityNames();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var generalDescription = unitOfWork.ApplyIncludes(generalDescriptionRep.All(), q =>
                    q.Include(i => i.Names)
                        .Include(i => i.LanguageAvailabilities)).Single(x => x.Id == model.Id.Value);

                result = TranslationManagerToVm.Translate<StatutoryServiceGeneralDescriptionVersioned, VmEntityNames>(generalDescription);

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages())
                );
            });
            return result;
        }

        public IVmOpenApiGeneralDescriptionVersionBase AddGeneralDescription(IVmOpenApiGeneralDescriptionInVersionBase vm, bool allowAnonymous, int openApiVersion)
        {
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = utilities.GetRelationIdForExternalSource();
            StatutoryServiceGeneralDescriptionVersioned generalDescription = null;

            contextManager.ExecuteWriter(unitOfWork =>
            {
                generalDescription = TranslationManagerToEntity.Translate<IVmOpenApiGeneralDescriptionInVersionBase, StatutoryServiceGeneralDescriptionVersioned>(vm, unitOfWork);
                var generalDescriptionRepository = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                generalDescriptionRepository.Add(generalDescription);

                unitOfWork.Save(saveMode);
            });

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = commonService.PublishAllAvailableLanguageVersions<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(generalDescription.Id, i => i.StatutoryServiceGeneralDescriptionVersionedId == generalDescription.Id);
            }

            return GetGeneralDescriptionWithDetails(generalDescription.Id, openApiVersion);
        }
                
        public IVmOpenApiGeneralDescriptionVersionBase SaveGeneralDescription(IVmOpenApiGeneralDescriptionInVersionBase vm, int openApiVersion)
        {
            StatutoryServiceGeneralDescriptionVersioned generalDescription = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var generalDescriptionRepository = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var rootId = vm.Id;

                // Set the current version id
                vm.Id = vm.CurrentVersionId.HasValue ? vm.CurrentVersionId : versioningManager.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, vm.Id.Value);

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
                            var publishingResult = commonService.RestoreArchivedEntity<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, vm.Id.Value);
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
                                    var updatedNames = updatedLaw.Names.Select(n => n.Id).ToList();
                                    var currentLaw = unitOfWork.ApplyIncludes(lawRep.All().Where(w => w.Id == l.LawId), q => q.Include(i => i.Names).Include(i => i.WebPages)).FirstOrDefault();
                                    // Delete the web pages that were not included in updated webpages
                                    currentLaw.WebPages.Where(w => !updatedWebPages.Contains(w.WebPageId)).ForEach(w => webPageRep.Remove(w.WebPage));
                                    // Delete all names that were not included in updated names
                                    currentLaw.Names.Where(n => !updatedNames.Contains(n.Id)).ForEach(n => lawNameRep.Remove(n));
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

                unitOfWork.Save(parentEntity: generalDescription);
            });

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult = commonService.PublishAllAvailableLanguageVersions<StatutoryServiceGeneralDescriptionVersioned, GeneralDescriptionLanguageAvailability>(generalDescription.Id, i => i.StatutoryServiceGeneralDescriptionVersionedId == generalDescription.Id);
            }

            return GetGeneralDescriptionWithDetails(generalDescription.Id, openApiVersion);
        }

        public VmGeneralDescription GetGeneralDescription(VmGeneralDescriptionIn model)
        {
            VmGeneralDescription result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                SetTranslatorLanguage(model);

                var serviceTypes = commonService.GetServiceTypes();
                var entity = GetEntity<StatutoryServiceGeneralDescriptionVersioned>(model.Id, unitOfWork,
                    q => q.Include(x => x.Names)
                         .Include(x => x.Descriptions)
                         .Include(x => x.Languages)
                         .Include(x => x.StatutoryServiceRequirements).Include(x => x.PublishingStatus)
                         .Include(x => x.ServiceClasses).ThenInclude(j => j.ServiceClass)
                         .Include(x => x.TargetGroups)
                         .Include(x => x.IndustrialClasses).ThenInclude(j => j.IndustrialClass)
                         .Include(x => x.LifeEvents).ThenInclude(j => j.LifeEvent)
                         .Include(x => x.OntologyTerms).ThenInclude(j => j.OntologyTerm)
                         .Include(x => x.Type)
                         .Include(x => x.ChargeType)
                         .Include(x => x.StatutoryServiceLaws).ThenInclude(j => j.Law).ThenInclude(k => k.Names)
                         .Include(x => x.StatutoryServiceLaws).ThenInclude(j => j.Law).ThenInclude(k => k.WebPages)
                         .Include(x => x.StatutoryServiceLaws).ThenInclude(j => j.Law).ThenInclude(k => k.WebPages).ThenInclude(l => l.WebPage)
                );
                result = GetModel<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescription>(entity, unitOfWork);
                                
                var serviceClassesRep = unitOfWork.CreateRepository<IServiceClassRepository>();

                var lifeEventRep = unitOfWork.CreateRepository<ILifeEventRepository>();

                var keyWordRep = unitOfWork.CreateRepository<IKeywordRepository>();

                var targetGroupRep = unitOfWork.CreateRepository<ITargetGroupRepository>();

                var industrialClassesRep = unitOfWork.CreateRepository<IIndustrialClassRepository>();

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Languages", commonService.GetLanguages()),
                    () => GetEnumEntityCollectionModel("ChargeTypes", commonService.GetPhoneChargeTypes()),
                    () => GetEnumEntityCollectionModel("ServiceTypes", serviceTypes),
                    () => GetEnumEntityCollectionModel("Laws", commonService.GetLaws(unitOfWork, result.Laws.Select(x => x.Id.Value).ToList())),
                    () => GetEnumEntityCollectionModel("TopTargetGroups", CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(GetIncludesForFinto<TargetGroup, TargetGroupName>(unitOfWork, targetGroupRep.All().OrderBy(x => x.Label)), 1), x => x.Code)),
                    () => GetEnumEntityCollectionModel("TopLifeEvents", CreateTree<VmTreeItem>(LoadFintoTree(GetIncludesForFinto<LifeEvent, LifeEventName>(unitOfWork, lifeEventRep.All()), 1), x => x.Name)),
                    () => GetEnumEntityCollectionModel("TopServiceClasses", CreateTree<VmTreeItem>(LoadFintoTree(GetIncludesForFinto<ServiceClass, ServiceClassName>(unitOfWork, serviceClassesRep.All()), 1), x => x.Name)),
                    () => GetEnumEntityCollectionModel("IndustrialClasses", TranslationManagerToVm.TranslateAll<IFintoItem, VmTreeItem>(GetIncludesForFinto<IndustrialClass, IndustrialClassName>(unitOfWork, industrialClassesRep.All().Where(x => x.Code == "5").OrderBy(x => x.Label))).ToList())
                );
            });

            return result;
        }

        public VmGeneralDescription UpdateGeneralDescription(VmGeneralDescription model)
        {
            Guid? generalDescriptionId = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SetTranslatorLanguage(model);
                var generalDescription = TranslationManagerToEntity.Translate<VmGeneralDescription, StatutoryServiceGeneralDescriptionVersioned>(model, unitOfWork);

                if (model.Id == generalDescription.Id)
                {
                   var id = generalDescription.Id;
                   generalDescription.TargetGroups = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, generalDescription.TargetGroups,
                        query => query.StatutoryServiceGeneralDescriptionVersionedId == id,
                        targetGroup => targetGroup.TargetGroupId);
                    generalDescription.LifeEvents = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, generalDescription.LifeEvents,
                        query => query.StatutoryServiceGeneralDescriptionVersionedId == id,
                        lifeEvent => lifeEvent.LifeEventId);
                    generalDescription.ServiceClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, generalDescription.ServiceClasses,
                        query => query.StatutoryServiceGeneralDescriptionVersionedId == id,
                        serviceClass => serviceClass.ServiceClassId);
                    generalDescription.OntologyTerms = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, generalDescription.OntologyTerms,
                        query => query.StatutoryServiceGeneralDescriptionVersionedId == id,
                        ontologyTerm => ontologyTerm.OntologyTermId);                  
                    generalDescription.IndustrialClasses = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, generalDescription.IndustrialClasses,
                        query => query.StatutoryServiceGeneralDescriptionVersionedId == id,
                        industrialClass => industrialClass.IndustrialClassId);

                    //Removing laws
                    dataUtils.UpdateCollectionWithRemove(unitOfWork,
                        generalDescription.StatutoryServiceLaws.Select(x => x.Law).ToList(),
                        curr => curr.StatutoryServiceLaws.Any(x => x.StatutoryServiceGeneralDescriptionVersionedId == generalDescription.Id));
                }

                unitOfWork.Save(parentEntity: generalDescription);
                generalDescriptionId = generalDescription.Id;
            });
            return GetGeneralDescription(new VmGeneralDescriptionIn { Id = generalDescriptionId, Language = model.Language});
        }

        public IVmEntityBase AddGeneralDescription(VmGeneralDescription model)
        {
            StatutoryServiceGeneralDescriptionVersioned result = null;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                result = AddGeneralDescription(unitOfWork, model);
                unitOfWork.Save();
            });
            return new VmEntityRootStatusBase() { Id = result.Id, PublishingStatusId = commonService.GetDraftStatusId()};
        }

        private StatutoryServiceGeneralDescriptionVersioned AddGeneralDescription(IUnitOfWorkWritable unitOfWork, VmGeneralDescription vm)
        {
            var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            SetTranslatorLanguage(vm);
            var generalDescription = TranslationManagerToEntity.Translate<VmGeneralDescription, StatutoryServiceGeneralDescriptionVersioned>(vm, unitOfWork);
            generalDescriptionRep.Add(generalDescription);
            return generalDescription;
        }

        private StatutoryServiceGeneralDescriptionVersioned DeleteGeneralDescription(IUnitOfWorkWritable unitOfWork, Guid? entityId)
        {            
            var publishStatus = TranslationManagerToEntity.Translate<String, PublishingStatusType>(PublishingStatus.Deleted.ToString(), unitOfWork);
            var generalDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
            var generalDescription = generalDescriptionRep.All()
                .Include(x => x.TargetGroups)
                .Include(x => x.ServiceClasses)
                .Include(x => x.LifeEvents)
                .Include(x => x.OntologyTerms)
                .Include(x => x.IndustrialClasses)
                .Single(x => x.Id == entityId.Value);
                
            var originalPublishingStatusId = generalDescription.PublishingStatusId;
            generalDescription.PublishingStatusId = publishStatus.Id;

            var publishStatusPublished = TranslationManagerToEntity.Translate<String, PublishingStatusType>(PublishingStatus.Published.ToString(), unitOfWork);
            if(originalPublishingStatusId == publishStatusPublished.Id)
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
                    x.ServiceChargeTypeId = generalDescription.ChargeTypeId;
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

    }
}
