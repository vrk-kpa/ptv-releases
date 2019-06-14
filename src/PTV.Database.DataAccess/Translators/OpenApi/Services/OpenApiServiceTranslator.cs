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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Framework.Extensions;
using PTV.Domain.Model.Models.OpenApi.Extensions;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<ServiceVersioned, VmOpenApiServiceVersionBase>), RegisterType.Transient)]
    internal class OpenApiServiceTranslator : Translator<ServiceVersioned, VmOpenApiServiceVersionBase>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        public OpenApiServiceTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmOpenApiServiceVersionBase TranslateEntityToVm(ServiceVersioned entity)
        {
            // Property value changes (PTV-2184)
            var type = entity.TypeId.HasValue ? typesCache.GetByValue<ServiceType>(entity.TypeId.Value) : null;
            var chargeType = entity.ChargeTypeId.HasValue ? typesCache.GetByValue<ServiceChargeType>(entity.ChargeTypeId.Value) : null;
            var areaType = typesCache.GetByValue<AreaInformationType>(entity.AreaInformationTypeId);
            var definition = CreateEntityViewModelDefinition<VmOpenApiServiceVersionBase>(entity)
                // We have to use unique root id as id!
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddSimple(i => i.Id, o => o.VersionId)
                .AddNavigation(i => string.IsNullOrEmpty(type) ? null : type.GetOpenApiEnumValue<ServiceTypeEnum>(), o => o.Type)
                .AddNavigation(i => i.FundingTypeId.HasValue ? typesCache.GetByValue<ServiceFundingType>(i.FundingTypeId.Value) : null, o => o.FundingType)
                .AddCollection(i => i.ServiceNames, o => o.ServiceNames)
                .AddNavigation(i => string.IsNullOrEmpty(chargeType) ? null : chargeType.GetOpenApiEnumValue<ServiceChargeTypeEnum>(), o => o.ServiceChargeType)
                .AddNavigation(i => string.IsNullOrEmpty(areaType) ? null : areaType.GetOpenApiEnumValue<AreaInformationTypeEnum>(), o => o.AreaType)
                .AddCollection(i => i.ServiceDescriptions, o => o.ServiceDescriptions)
                .AddCollection(i => i.ServiceLanguages.OrderBy(j => j.OrderNumber).Select(j => j.Language.Code).ToList(), o => o.Languages)
                .AddCollection(i => i.ServiceLaws.OrderBy(x => x.Law.OrderNumber).Select(j => j.Law), o => o.Legislation)
                .AddCollection(i => i.ServiceKeywords, o => o.Keywords)
                .AddCollection(i => i.ServiceRequirements, o => o.Requirements)
                .AddNavigation(i => typesCache.GetByValue<PublishingStatusType>(i.PublishingStatusId), o => o.PublishingStatus)
                .AddCollection(i => i.LanguageAvailabilities.Select(l => languageCache.GetByValue(l.LanguageId)).ToList(), o => o.AvailableLanguages)
                .AddSimple(i => i.StatutoryServiceGeneralDescriptionId, o => o.GeneralDescriptionId)
                .AddCollection(i => i.ServiceServiceClasses.Where(x => x.ServiceClass.IsValid == true).Select(j => j.ServiceClass).ToList(), o => o.ServiceClasses)// PTV-4317
                .AddCollection(i => i.Areas.Select(a => a.Area), o => o.Areas)
                .AddCollection(i => i.ServiceOntologyTerms.Where(x => x.OntologyTerm.IsValid == true).Select(j => j.OntologyTerm).ToList(), o => o.OntologyTerms)// PTV-4317
                .AddCollection(i => i.ServiceTargetGroups.Where(x => x.TargetGroup.IsValid == true).Select(j => j.TargetGroup).ToList(), o => o.TargetGroups)// PTV-4317
                .AddCollection(i => i.ServiceLifeEvents.Where(x => x.LifeEvent.IsValid == true).Select(j => j.LifeEvent).ToList(), o => o.LifeEvents)// PTV-4317
                .AddCollection(i => i.ServiceIndustrialClasses.Where(x => x.IndustrialClass.IsValid == true).Select(j => j.IndustrialClass).ToList(), o => o.IndustrialClasses)// PTV-4317
                .AddCollection(i => i.AreaMunicipalities.Select(m => m.Municipality), o => o.Municipalities)
                .AddCollection(i => i.UnificRoot?.ServiceServiceChannels, o => o.ServiceChannels)
                .AddCollection(i => i.OrganizationServices, o => o.Organizations)
                .AddSimple(i => i.ServiceWebPages?.Count > 0 || i.WebPageInUse ? true : false, o => o.ServiceVouchersInUse)
                .AddCollection(i => i.ServiceWebPages.OrderByDescending(x => x.WebPage.LocalizationId).ThenBy(x => x.WebPage.OrderNumber), o => o.ServiceVouchers)
                .AddCollection(i => i.ServiceProducers.OrderBy(x => x.OrderNumber).ThenBy(x => x.Modified), output => output.ServiceProducers)
                .AddCollection(i => i.UnificRoot?.ServiceCollectionServices, o => o.ServiceCollections)
                .AddNavigation(i => i.Organization, o => o.MainOrganization)
                .AddSimple(i => i.Modified, o => o.Modified);

            var vm = definition.GetFinal();

            if (vm.Municipalities?.Count > 0)
            {
                if (vm.Areas == null)
                {
                    vm.Areas = new List<VmOpenApiArea>();
                }
                vm.Areas.Add(new VmOpenApiArea { Type = AreaTypeEnum.Municipality.ToString(), Municipalities = vm.Municipalities.ToList() });
            }

            // Main responsible organization
            if (vm.MainOrganization != null)
            {
                var responsible = CommonConsts.RESPONSIBLE;

                // Check if there already exists the main organization within OtherResponsible organizations
                var otherResponsibleOrganization = vm.Organizations.Where(o => o.Organization != null && o.Organization.Id == vm.MainOrganization.Id).FirstOrDefault();
                if (otherResponsibleOrganization != null)
                {
                    otherResponsibleOrganization.RoleType = responsible;
                }
                else
                {
                    vm.Organizations.Add(new V6VmOpenApiServiceOrganization
                    {
                        RoleType = responsible,
                        Organization = vm.MainOrganization
                    });
                }                
            }

            // Producers
            if (vm.ServiceProducers?.Count > 0)
            {
                vm.ServiceProducers.ForEach(serviceProducer =>
                {
                    if (serviceProducer.Organizations?.Count > 0)
                    {
                        serviceProducer.Organizations.ForEach(o =>
                        vm.Organizations.Add(new V6VmOpenApiServiceOrganization
                        {
                            RoleType = CommonConsts.PRODUCER,
                            ProvisionType = serviceProducer.ProvisionType,
                            Organization = o,
                            AdditionalInformation = serviceProducer.AdditionalInformation
                        }));
                    }
                    else
                    {
                        vm.Organizations.Add(new V6VmOpenApiServiceOrganization
                        {
                            RoleType = CommonConsts.PRODUCER,
                            ProvisionType = serviceProducer.ProvisionType,
                            AdditionalInformation = serviceProducer.AdditionalInformation
                        });
                    }
                });
            }
                
            return vm;
        }

        public override ServiceVersioned TranslateVmToEntity(VmOpenApiServiceVersionBase vModel)
        {
            throw new NotImplementedException();
        }
    }
}
