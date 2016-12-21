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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi.V2;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IGeneralDescriptionService), RegisterType.Transient)]
    public class GeneralDescriptionService : ServiceBase, IGeneralDescriptionService
    {
        private readonly ITranslationEntity translationManager;
        private ITranslationViewModel translationManagerVModel;
        private readonly IContextManager contextManager;
        private readonly ILogger logger;
        private readonly ServiceUtilities utilities;
        private ICommonService commonService;

        public GeneralDescriptionService(
            IContextManager contextManager,
            IUserIdentification userIdentification,
            ITranslationEntity translationManager,
            ITranslationViewModel translationManagerVModel,
            ILogger<GeneralDescriptionService> logger,
            ServiceUtilities utilities,
            ICommonService commonService,
             IPublishingStatusCache publishingStatusCache) : base(translationManager, translationManagerVModel, publishingStatusCache)
        {
            this.translationManager = translationManager;
            this.contextManager = contextManager;
            this.translationManagerVModel = translationManagerVModel;
            this.logger = logger;
            this.utilities = utilities;
            this.commonService = commonService;
        }

        public VmGeneralDescriptionSearchForm GetGeneralDescriptionSearchForm()
        {
            VmGeneralDescriptionSearchForm result = new VmGeneralDescriptionSearchForm();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var targetGroupRep = unitOfWork.CreateRepository<ITargetGroupRepository>();
                var serviceClassesRep = unitOfWork.CreateRepository<IServiceClassRepository>();

                FillEnumEntities(result,
                    () => GetEnumEntityCollectionModel("TargetGroups", CreateTree<VmExpandedVmTreeItem>(LoadFintoTree(targetGroupRep.All().OrderBy(x => x.Label)), x => x.Code)),
                    () => GetEnumEntityCollectionModel("ServiceClasses", CreateTree<VmTreeItem>(serviceClassesRep.All(), x => x.Name))
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

        public VmGeneralDescriptions SearchGeneralDescriptions(VmGeneralDescriptionSearchForm searchData, bool takeAll = false)
        {
            VmGeneralDescriptions result = new VmGeneralDescriptions();
            var count = 0;
            contextManager.ExecuteReader(unitOfWork =>
            {
                var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionRepository>();
                var descriptions = statutoryGeneralDescriptionRep.All();
                if (!string.IsNullOrEmpty(searchData.GeneralDescriptionName))
                {
                    searchData.GeneralDescriptionName = searchData.GeneralDescriptionName.ToLower();
                    descriptions = descriptions.Where(i => i.Names.Select(j => j.Name.ToLower()).Any(m => m.Contains(searchData.GeneralDescriptionName)));
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
                descriptions = !takeAll ? searchData.PageNumber > 0
                    ? descriptions.Skip(searchData.PageNumber * CoreConstants.MaximumNumberOfAllItems).Take(CoreConstants.MaximumNumberOfAllItems)
                    : descriptions.Take(CoreConstants.MaximumNumberOfAllItems) : descriptions;

                descriptions = unitOfWork.ApplyIncludes(descriptions, i => i
                    .Include(j => j.Names)
                    .Include(j => j.ServiceClasses)
                    .Include(j => j.TargetGroups)
                    .Include(j => j.Descriptions)
                );
                result.Count = count;
                result.PageNumber = ++searchData.PageNumber;
                result.GeneralDescriptions = translationManager.TranslateAll<StatutoryServiceGeneralDescription, VmGeneralDescriptionResults>(descriptions);
            });
            return result;
        }

        public VmGeneralDescription GetGeneralDescriptionById(Guid id)
        {
            VmGeneralDescription result = new VmGeneralDescription();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionRepository>();
                var description = statutoryGeneralDescriptionRep.All().Where(x => x.Id == id);

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
                );

                result = translationManager.TranslateFirst<StatutoryServiceGeneralDescription, VmGeneralDescription>(description);
            });
            return result;
        }

        public IVmOpenApiGuidPage GetGeneralDescriptions(DateTime? date, int pageNumber = 1, int pageSize = 1000)
        {
            var pagingVm = new VmOpenApiGuidPage(pageNumber, pageSize);

            contextManager.ExecuteReader(unitOfWork =>
            {
                var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionRepository>();
                var query = statutoryGeneralDescriptionRep.All();
                if (date.HasValue)
                {
                    query = query.Where(g => g.Modified > date.Value);
                }
                pagingVm.SetPageCount(query.Count());
                if (pagingVm.IsValidPageNumber())
                {
                    if (pagingVm.PageCount > 1)
                    {
                        query = query.OrderBy(o => o.Created).Skip(pagingVm.GetSkipSize()).Take(pagingVm.GetTakeSize());
                    }
                    pagingVm.GuidList = query.ToList().Select(o => o.Id).ToList();
                }
            });

            return pagingVm;
        }

        public IVmOpenApiGeneralDescription GetGeneralDescription(Guid id)
        {
            return TranslationManagerToVm.Translate<V2VmOpenApiGeneralDescription, VmOpenApiGeneralDescription>(V2GetGeneralDescription(id) as V2VmOpenApiGeneralDescription);
        }

        public IVmOpenApiGeneralDescription V2GetGeneralDescription(Guid id)
        {
            var result = new V2VmOpenApiGeneralDescription();
            try
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var statutoryGeneralDescriptionRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionRepository>();

                    var query = unitOfWork.ApplyIncludes(statutoryGeneralDescriptionRep.All().Where(s => s.Id == id), q =>
                        q.Include(i => i.Languages).ThenInclude(i => i.Language)
                        .Include(i => i.Names).ThenInclude(i => i.Type)
                        .Include(i => i.Descriptions).ThenInclude(i => i.Type)
                        .Include(i => i.ServiceClasses).ThenInclude(i => i.ServiceClass)
                        .Include(i => i.OntologyTerms).ThenInclude(i => i.OntologyTerm)
                        .Include(i => i.TargetGroups).ThenInclude(i => i.TargetGroup)
                        .Include(i => i.LifeEvents).ThenInclude(i => i.LifeEvent))
                        .Include(i => i.IndustrialClasses).ThenInclude(i => i.IndustrialClass)
                        .Include(i => i.StatutoryServiceRequirements)
                        .Include(i => i.Type)
                        .Include(i => i.ChargeType)
                        .Include(i => i.StatutoryServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.Names)
                        .Include(i => i.StatutoryServiceLaws).ThenInclude(i => i.Law).ThenInclude(i => i.WebPages).ThenInclude(i => i.WebPage);

                    result = translationManager.TranslateFirst<StatutoryServiceGeneralDescription, V2VmOpenApiGeneralDescription>(query);
                });

            }
            catch(Exception ex)
            {
                var errorMsg = string.Format("Error occured while getting a general description with id {0}. {1}", id, ex.Message);
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }

            return result;
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

        public IVmOpenApiGeneralDescription AddGeneralDescription(IVmOpenApiGeneralDescriptionIn vm, bool allowAnonymous)
        {
            var result = new V2VmOpenApiGeneralDescription();
            var saveMode = allowAnonymous ? SaveMode.AllowAnonymous : SaveMode.Normal;
            var userId = utilities.GetRelationIdForExternalSource();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                var generalDescription = TranslationManagerToEntity.Translate<IVmOpenApiGeneralDescriptionIn, StatutoryServiceGeneralDescription>(vm, unitOfWork);
                var generalDescriptionRepository = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionRepository>();
                generalDescriptionRepository.Add(generalDescription);

                unitOfWork.Save(saveMode);
                result = TranslationManagerToVm.Translate<StatutoryServiceGeneralDescription, V2VmOpenApiGeneralDescription>(generalDescription);
            });

            return result;
        }
    }
}
