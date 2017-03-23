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
using PTV.Domain.Model.Models.OpenApi.V1;
using PTV.Database.DataAccess.Interfaces;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using VmGeneralDescription = PTV.Domain.Model.Models.VmGeneralDescription;
using VmGeneralDescriptionListItem = PTV.Domain.Model.Models.VmGeneralDescriptionListItem;
using VmGeneralDescriptions = PTV.Domain.Model.Models.VmGeneralDescriptions;

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
        private ICommonService commonService;
        private ILanguageCache languageCache;
        private IVersioningManager versioningManager;

        public GeneralDescriptionService(
            IContextManager contextManager,
            IUserIdentification userIdentification,
            ITranslationEntity translationManager,
            ITranslationViewModel translationManagerVModel,
            ILogger<GeneralDescriptionService> logger,
            ServiceUtilities utilities,
            ICommonService commonService,
            IPublishingStatusCache publishingStatusCache,
            IUserOrganizationChecker userOrganizationChecker,
            ILanguageCache languageCache,
            IVersioningManager versioningManager)
            : base(translationManager, translationManagerVModel, publishingStatusCache, userOrganizationChecker)
        {
            this.translationManager = translationManager;
            this.contextManager = contextManager;
            this.translationManagerVModel = translationManagerVModel;
            this.logger = logger;
            this.utilities = utilities;
            this.commonService = commonService;
            this.languageCache = languageCache;
            this.versioningManager = versioningManager;
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
                    searchData.Name = searchData.Name.ToLower();
                    descriptions = descriptions.Where(i => i.Names.Select(j => j.Name.ToLower()).Any(m => m.Contains(searchData.Name)));
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

        private Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptions FilteredSearchGeneralDescriptions_V2(VmGeneralDescriptionSearchForm searchData)
        {
            var result = new Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptions();
            var count = 0;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var psPublished = PublishingStatusCache.Get(PublishingStatus.Published);
                var descriptions = statutoryGeneralDescriptionRep.All().Where(i => i.PublishingStatusId == psPublished);
                if (!string.IsNullOrEmpty(searchData.Name))
                {
                    searchData.Name = searchData.Name.ToLower();
                    descriptions = descriptions.Where(i => i.Names.Select(j => j.Name.ToLower()).Any(m => m.Contains(searchData.Name)));
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

                count = descriptions.Count();
                descriptions = descriptions.OrderBy(i => i.Id);
                descriptions = searchData.PageNumber > 0
                    ? descriptions.Skip(searchData.PageNumber * CoreConstants.MaximumNumberOfAllItems).Take(CoreConstants.MaximumNumberOfAllItems)
                    : descriptions.Take(CoreConstants.MaximumNumberOfAllItems);

                descriptions = unitOfWork.ApplyIncludes(descriptions, i => i
                    .Include(j => j.Names)
                    .Include(j => j.ServiceClasses)
                    .Include(j => j.Descriptions)
                    .Include(j => j.LanguageAvailabilities).ThenInclude(j => j.Language)
                    .Include(j => j.Versioning)
                );
                result.PageNumber = ++searchData.PageNumber;
                result.GeneralDescriptions = translationManager.TranslateAll<StatutoryServiceGeneralDescriptionVersioned, Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptionListItem>(descriptions);
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
                    generalDescriptionNames = generalDescriptionNames
                        .Where(x =>
                            x.Name.ToLower().Contains(vm.Name.ToLower()) &&
                            languagesIds.Contains(x.LocalizationId)
                        );
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
            contextManager.ExecuteReader(unitOfWork =>
            {
                var generalDescriptionNameRepository = unitOfWork.CreateRepository<IStatutoryServiceNameRepository>();

                var languagesIds = vm.Languages.Select(language => languageCache.Get(language.ToString()));
                var generalDescriptionNames = generalDescriptionNameRepository.All();

                if (!string.IsNullOrEmpty(vm.Name))
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

        public Domain.Model.Models.V2.GeneralDescriptions.VmGeneralDescriptions SearchGeneralDescriptions_V2(VmGeneralDescriptionSearchForm searchData)
        {
            var result = searchData.Languages?.Count > 1
                ? FullTextSearchGeneralDescriptions_V2(searchData)
                : FilteredSearchGeneralDescriptions_V2(searchData);
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

                description = unitOfWork.ApplyIncludes(description, i => i
                    .Include(j => j.Names)
                    .Include(j => j.ServiceClasses)
                    .Include(j => j.TargetGroups)
                    .Include(j => j.IndustrialClasses).ThenInclude(j => j.IndustrialClass)
                    .Include(j => j.LifeEvents).ThenInclude(j => j.LifeEvent)
                    .Include(j => j.OntologyTerms).ThenInclude(j => j.OntologyTerm)
                    .Include(j => j.Type)
                    .Include(j => j.ChargeType)
                    .Include(j => j.StatutoryServiceRequirements)
                    .Include(j => j.Descriptions)
                    .Include(j => j.StatutoryServiceLaws)
                );

                result = translationManager.TranslateFirst<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescription>(description);
                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("Laws", commonService.GetLaws(unitOfWork, result.Laws))
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

        public IVmOpenApiGuidPageVersionBase GetGeneralDescriptions(DateTime? date, int pageNumber = 1, int pageSize = 1000)
        {
            return GetGeneralDescriptions(new VmOpenApiGuidPage(pageNumber, pageSize), date);
        }

        public IVmOpenApiGuidPageVersionBase V3GetGeneralDescriptions(DateTime? date, int pageNumber = 1, int pageSize = 1000)
        {
            return GetGeneralDescriptions(new V3VmOpenApiGuidPage(pageNumber, pageSize), date);
        }

        private IVmOpenApiGuidPageVersionBase GetGeneralDescriptions(IVmOpenApiGuidPageVersionBase vm, DateTime? date)
        {
            contextManager.ExecuteReader(unitOfWork =>
            {
                var filters = new List<Expression<Func<StatutoryServiceGeneralDescriptionVersioned, bool>>>() { PublishedFilter<StatutoryServiceGeneralDescriptionVersioned>() };
                SetItemPage(vm, date, unitOfWork, filters, q => q.Include(i => i.Names));
            });

            return vm;
        }

        public IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionVersionBase(Guid id, int openApiVersion)
        {
            IVmOpenApiGeneralDescriptionVersionBase result = null;
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    // Get the right version id
                    Guid? entityId = versioningManager.GetVersionId<StatutoryServiceGeneralDescriptionVersioned>(unitOfWork, id, PublishingStatus.Published);

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

        private IVmOpenApiGeneralDescriptionVersionBase GetGeneralDescriptionWithDetails(IUnitOfWork unitOfWork, Guid versionId, int openApiVersion)
        {
            IVmOpenApiGeneralDescriptionVersionBase result = null;
            var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();

            var query = unitOfWork.ApplyIncludes(statutoryGeneralDescriptionRep.All()
                .Where(s => s.Id == versionId)
                .Where(PublishedFilter<StatutoryServiceGeneralDescriptionVersioned>()), q =>
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
                .Include(i => i.StatutoryServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.WebPages).ThenInclude(i => i.WebPage));

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

        public IVmOpenApiGeneralDescriptionVersionBase AddGeneralDescription(IVmOpenApiGeneralDescriptionIn vm, bool allowAnonymous)
        {
            IVmOpenApiGeneralDescriptionVersionBase result = new VmOpenApiGeneralDescriptionVersionBase();
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = utilities.GetRelationIdForExternalSource();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                var generalDescription = TranslationManagerToEntity.Translate<IVmOpenApiGeneralDescriptionIn, StatutoryServiceGeneralDescriptionVersioned>(vm, unitOfWork);
                if (vm.PublishingStatus == PublishingStatus.Published.ToString())
                {
                    versioningManager.PublishVersion(unitOfWork, generalDescription);
                }
                var generalDescriptionRepository = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                generalDescriptionRepository.Add(generalDescription);

                unitOfWork.Save(saveMode);
                result = GetGeneralDescriptionWithDetails(unitOfWork, generalDescription.Id, 4);
            });

            return result;
        }
    }
}
