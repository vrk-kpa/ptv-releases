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
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Interfaces;

namespace PTV.Database.DataAccess.Services.Validation
{
    [RegisterService(typeof(IBaseValidationChecker<ServiceVersioned>), RegisterType.Transient)]
    internal class ServiceValidationChecker : BaseValidationChecker<ServiceVersioned>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private IVersioningManager VersioningManager;

        public ServiceValidationChecker(ICacheManager cacheManager, IResolveManager resolveManager) : base(cacheManager, resolveManager)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
            this.VersioningManager = resolveManager.Resolve<IVersioningManager>();
        }

        public override ServiceVersioned FetchEntity(Guid id, IUnitOfWork unitOfWork)
        {
            return GetEntity<ServiceVersioned>(id, unitOfWork,
                q => q.Include(i => i.ServiceNames)
                        .Include(i => i.OrganizationServices)
                        .Include(i => i.Organization).ThenInclude(i => i.Versions)
                        .Include(i => i.ServiceDescriptions)
                        .Include(i => i.ServiceLanguages)
                        .Include(i => i.LanguageAvailabilities)
                        .Include(i => i.ServiceTargetGroups).ThenInclude(x=>x.TargetGroup)
                        .Include(i => i.ServiceServiceClasses)
                        .Include(i => i.ServiceOntologyTerms)
                        .Include(x => x.ServiceProducers).ThenInclude(x => x.Organizations)
                        .Include(x => x.ServiceProducers).ThenInclude(x => x.AdditionalInformations)
                        .Include(x => x.ServiceLaws).ThenInclude(x => x.Law).ThenInclude(x => x.WebPages).ThenInclude(x => x.WebPage)
                        .Include(x => x.ServiceLaws).ThenInclude(x => x.Law).ThenInclude(x => x.Names)
                        .Include(x => x.ServiceWebPages).ThenInclude(x => x.WebPage)
                        .Include(x => x.Areas)
                        .Include(x => x.AreaMunicipalities)
                        .Include(x => x.StatutoryServiceGeneralDescription).ThenInclude(x=>x.Versions).ThenInclude(x=>x.TargetGroups).ThenInclude(x=>x.TargetGroup)

                );
        }

        public override Dictionary<Guid, List<ValidationMessage>> ValidateEntity(Guid? language)
        {
            var generalDescriptionId = entity.StatutoryServiceGeneralDescriptionId;

            foreach (var entitylanguageId in entityOrPublishedLanguagesAvailabilityIds)
            {
                SetValidationLanguage(entitylanguageId);

                var organizationStatus = VersioningManager.ApplyPublishingStatusFilterFallback(entity.Organization.Versions)?.PublishingStatusId;
                NotBeTrue("organization", x=> organizationStatus != typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString()));

                NotEmptyGuid("fundingType", x => x.FundingTypeId);
                NotEmptyString("name", x => x.ServiceNames
                    .Where(y => y.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()) && y.LocalizationId == entitylanguageId)
                    .Select(y => y.Name)
                    .FirstOrDefault());
                NotEmptyList("languages", x => x.ServiceLanguages);

                NotEmptyString("shortDescription",
                    x => x.ServiceDescriptions
                        .Where(y => y.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString()) && y.LocalizationId == entitylanguageId)
                        .Select(y => y.Description)
                        .FirstOrDefault());

                if (!generalDescriptionId.IsAssigned())
                {
                    NotEmptyGuid("serviceType", x => x.TypeId);
                    NotEmptyTextEditorString("description",
                        x => x.ServiceDescriptions
                            .Where(y => y.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()) && y.LocalizationId == entitylanguageId)
                            .Select(y => y.Description)
                            .FirstOrDefault());
                    NotEmptyList("targetGroups", x => x.ServiceTargetGroups.Where(y => y.TargetGroup.ParentId == null));
                    NotEmptyList("serviceClasses", x => x.ServiceServiceClasses);
                    NotEmptyList("ontologyTerms", x => x.ServiceOntologyTerms);
                }
                else
                {
                    var gdTargetGroup = VersioningManager.ApplyPublishingStatusFilterFallback(entity.StatutoryServiceGeneralDescription.Versions).TargetGroups.Where(x=>x.TargetGroup.ParentId == null);
                    var serviceOverrideTargetGroup = entity.ServiceTargetGroups.Where(y => y.Override && y.TargetGroup.ParentId == null);
                    var gdTargetGroupsChecked = gdTargetGroup.Where(x => !serviceOverrideTargetGroup.Any(y => y.TargetGroupId == x.TargetGroupId));
                    var serviceTargetGroupsChecked = entity.ServiceTargetGroups.Where(y => !y.Override && y.TargetGroup.ParentId == null);
                    NotBeTrue("targetGroups", z => !gdTargetGroupsChecked.Any() && !serviceTargetGroupsChecked.Any());
                }

                //Service producers
                if (!NotEmptyList("serviceProducers", x => x.ServiceProducers))
                {
                    foreach (var serviceProducer in entity.ServiceProducers)
                    {
                        CheckEntityWithMergeResult(serviceProducer);
                    }
                }
                //NotBeTrue("serviceProducers", x => !x.ServiceProducers.Any() || entity.ServiceProducers.Any(y => y.ProvisionTypeId == typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString()) && !y.Organizations.Any()));

                //Service vouchers
                foreach (var serviceWebPage in entity.ServiceWebPages.Where(x => x.WebPage?.LocalizationId == entitylanguageId))
                {
                    CheckEntityWithMergeResult(serviceWebPage.WebPage);
                }

                //Laws
                foreach (var serviceLaw in entity.ServiceLaws)
                {
                    CheckEntityWithMergeResult(serviceLaw.Law);
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

            return validationMessagesDictionary;
        }
    }
}
