﻿/**
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
* THE SOFTWARE.C:\Projects\PTV_TEST\src\PTV.Database.DataAccess\Services\Security\
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Repositories;

namespace PTV.Database.DataAccess.Services.Validation
{
    [RegisterService(typeof(ILoadingValidationChecker<ServiceVersioned>), RegisterType.Transient)]
    internal class ServiceValidationChecker : BaseLoadingValidationChecker<ServiceVersioned>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly IVersioningManager versioningManager;
        private IUnitOfWork unitOfWork;
        private readonly IRestrictionFilterCache restrictionFilterCache;
        private ITextManager textManager;
        
        public ServiceValidationChecker(ICacheManager cacheManager, IResolveManager resolveManager, IRestrictionFilterCache restrictionFilterCache) : base(cacheManager, resolveManager)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
            this.restrictionFilterCache = restrictionFilterCache;
            this.versioningManager = resolveManager.Resolve<IVersioningManager>();
            this.textManager = resolveManager.Resolve<ITextManager>();
        }

        protected override ServiceVersioned FetchEntity(Guid id, IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            return GetEntity<ServiceVersioned>(id, unitOfWork,
                q => q.Include(i => i.ServiceNames)
                        .Include(i => i.OrganizationServices)
                        .Include(i => i.ServiceDescriptions)
                        .Include(i => i.ServiceLanguages)
                        .Include(i => i.LanguageAvailabilities)
                        .Include(i => i.ServiceTargetGroups).ThenInclude(x => x.TargetGroup)
                        .Include(i => i.ServiceServiceClasses)
                        .Include(i => i.ServiceOntologyTerms)
                        .Include(x => x.ServiceProducers)
                        .Include(x => x.ServiceWebPages).ThenInclude(x => x.WebPage)
                        .Include(x => x.Areas)
                        .Include(x => x.AreaMunicipalities)                       
                );
        }

        private string GetName(ServiceVersioned entity, Guid entitylanguageId)
        {
            return entity.ServiceNames
                .Where(y => y.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()) &&
                            y.LocalizationId == entitylanguageId)
                .Select(y => y.Name)
                .FirstOrDefault();
        }        
        
        private string GetShortDescription(ServiceVersioned entity, Guid entitylanguageId)
        {
            return entity.ServiceDescriptions
                .Where(y => y.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString()) && y.LocalizationId == entitylanguageId)
                .Select(y => y.Description)
                .FirstOrDefault();
        }

        protected override void ValidateEntityInternal(Guid? language)
        {
            var generalDescriptionId = entity.StatutoryServiceGeneralDescriptionId;

            foreach (var entitylanguageId in entityOrPublishedLanguagesAvailabilityIds)
            {
                SetValidationLanguage(entitylanguageId);

                CheckEntityWithMergeResult<Organization>(entity.OrganizationId, unitOfWork);
                
                NotEmptyGuid("fundingType", x => x.FundingTypeId);
                bool nameOrShortDescriptionEmpty = NotEmptyString("name", x => GetName(x, entitylanguageId));
                NotEmptyList("languages", x => x.ServiceLanguages);

                nameOrShortDescriptionEmpty |= NotEmptyString("shortDescription", x => GetShortDescription(x, entitylanguageId));
                
                if(!nameOrShortDescriptionEmpty)
                {
                    NotBeTrue
                    (
                        "shortDescription",
                        x => GetName(x, entitylanguageId) == GetShortDescription(x, entitylanguageId),
                        ValidationErrorTypeEnum.ValueIsSame
                    );
                }
                 

                if (!generalDescriptionId.IsAssigned())
                {
                    NotEmptyGuid("serviceType", x => x.TypeId);
                    NotEmptyTextEditorString("description",
                        x => x.ServiceDescriptions
                            .Where(y => y.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()) && y.LocalizationId == entitylanguageId)
                            .Select(y => y.Description)
                            .FirstOrDefault());
                    NotEmptyList("targetGroups", x => x.ServiceTargetGroups.Where(y => y.TargetGroup.ParentId == null));
                    CheckTargetGroups(entity.ServiceTargetGroups.ToList());
                    bool emptySc = NotEmptyList("serviceClasses", x => x.ServiceServiceClasses);
                    if (!emptySc)
                    {
                        NotBeTrue("serviceClasses", x => x.ServiceServiceClasses.Count > 4,
                            ValidationErrorTypeEnum.MaxLimit);
                    }
                    bool emptyOt = NotEmptyList("ontologyTerms", x => x.ServiceOntologyTerms);
                    if (!emptyOt)
                    {
                        NotBeTrue("ontologyTerms", x => x.ServiceOntologyTerms.Count > 10, ValidationErrorTypeEnum.MaxLimit);
                    }
                }
                else
                {
                    var gdRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                    var gd = gdRep.All().Where(x => x.UnificRootId == entity.StatutoryServiceGeneralDescriptionId)
                        .Include(x => x.TargetGroups).ThenInclude(x => x.TargetGroup)
                        .Include(x => x.Descriptions)
                        .Include(x => x.ServiceClasses)
                        .Include(x => x.OntologyTerms);
                    var publishedGd = versioningManager.ApplyPublishingStatusFilterFallback(gd);
                    var gdTargetGroup = publishedGd.TargetGroups.Where(x=>x.TargetGroup.ParentId == null);
                    var serviceOverrideTargetGroup = entity.ServiceTargetGroups.Where(y => y.Override && y.TargetGroup.ParentId == null);
                    var gdTargetGroupsChecked = gdTargetGroup.Where(x => serviceOverrideTargetGroup.All(y => y.TargetGroupId != x.TargetGroupId));
                    var serviceTargetGroupsChecked = entity.ServiceTargetGroups.Where(y => !y.Override && y.TargetGroup.ParentId == null);
                    NotBeTrue("targetGroups", z => !gdTargetGroupsChecked.Any() && !serviceTargetGroupsChecked.Any());
                    CheckTargetGroups(
                        entity.ServiceTargetGroups.ToList(),
                        publishedGd.TargetGroups.ToList());
                    
                    var gdDescriptionProperty = publishedGd.Descriptions.Where(y => y.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()) && y.LocalizationId == entitylanguageId)
                        .Select(y => y.Description)
                        .FirstOrDefault();
                    if (string.IsNullOrEmpty(textManager.ConvertToPureText(gdDescriptionProperty)))
                    {
                        NotEmptyTextEditorString("description",
                            x => x.ServiceDescriptions
                                .Where(y => y.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()) && y.LocalizationId == entitylanguageId)
                                .Select(y => y.Description)
                                .FirstOrDefault());
                    }
                    NotBeTrue("serviceClasses", x => x.ServiceServiceClasses.Select(sc => sc.ServiceClassId).Concat(publishedGd.ServiceClasses.Select(sc => sc.ServiceClassId)).Distinct().Count() > 4, ValidationErrorTypeEnum.MaxLimit);
                    NotBeTrue("ontologyTerms", x => x.ServiceOntologyTerms.Select(ot => ot.OntologyTermId).Concat(publishedGd.OntologyTerms.Select(ot => ot.OntologyTermId)).Distinct().Count() > 10, ValidationErrorTypeEnum.MaxLimit);
                }
                
                // validate published Organization language
                var publishedOrganizationLanguageExists = false; 
                var publishedOrgInfo = versioningManager.GetLastPublishedVersion<OrganizationVersioned>(unitOfWork, entity.OrganizationId);
                if (publishedOrgInfo != null)
                {
                    var orgRep = unitOfWork.CreateRepository<IOrganizationLanguageAvailabilityRepository>();
                    publishedOrganizationLanguageExists = orgRep.All()
                        .Any(x => x.OrganizationVersionedId == publishedOrgInfo.EntityId
                                  && x.StatusId ==
                                  typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString())
                                  && x.LanguageId == entitylanguageId);
                }
                NotBeTrue("organization", x => !publishedOrganizationLanguageExists, ValidationErrorTypeEnum.PublishedOrganizationLanguageMandatoryField);                    
                

                //Organization services
                foreach (var organizationService in entity.OrganizationServices)
                {
                    var result = CheckEntity<Organization>(organizationService.OrganizationId, unitOfWork);
                    NotBeTrue("responsibleOrganizations", x => result.Count > 0, ValidationErrorTypeEnum.PublishedMandatoryField);
                }
                
                //Service producers
                if (!NotEmptyList("serviceProducers", x => x.ServiceProducers))
                {
                    foreach (var serviceProducer in entity.ServiceProducers)
                    {
                        CheckEntityWithMergeResult<ServiceProducer>(serviceProducer.Id, unitOfWork);                        
                    }
                }

                //Service vouchers
                foreach (var serviceWebPage in entity.ServiceWebPages.Where(x => x.WebPage?.LocalizationId == entitylanguageId))
                {
                    if (!string.IsNullOrEmpty(serviceWebPage.WebPage.Name) || !string.IsNullOrEmpty(serviceWebPage.WebPage.Url))
                    {
                        CheckEntityWithMergeResult(serviceWebPage.WebPage);
                    }
                }

                if (!NotEmptyGuid("areaInformationType", x => x.AreaInformationTypeId))
                {
                    if (entity.AreaInformationTypeId ==
                        typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString()))
                    {
                        NotBeTrue("areaType", x => !(x.Areas.Any() || x.AreaMunicipalities.Any()));
                    }
                }
            }
        }
        
        private const string TgKr2 = "KR2";

        private void CheckTargetGroups(List<ServiceTargetGroup> serviceTargetGroups,
            List<StatutoryServiceTargetGroup> generalTargetGroups = null)
        {
            var serviceTg = serviceTargetGroups.Where(x => !x.Override).Select(x => x.TargetGroup).ToList();
            var overServiceTg = serviceTargetGroups.Where(x => x.Override).Select(x => x.TargetGroupId);
            if (generalTargetGroups != null && generalTargetGroups.Any())
            {
                serviceTg.AddRange(generalTargetGroups.Where(x => !overServiceTg.Contains(x.TargetGroupId))
                    .Select(x => x.TargetGroup));
            }

            var parents = serviceTg.Where(x => x.Code== TgKr2).Select(x => x.Id);
            var subParents = serviceTg.Where(x => x.ParentId.HasValue).Select(x => x.ParentId).Distinct();
            NotBeTrue("targetGroups", z => parents.Any(pId => !subParents.Contains(pId)));
        }
    }
}