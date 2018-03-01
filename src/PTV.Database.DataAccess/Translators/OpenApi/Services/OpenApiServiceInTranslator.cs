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
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi;
using System.Linq;
using PTV.Domain.Model.Models.OpenApi.V7;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<ServiceVersioned, IVmOpenApiServiceInVersionBase>), RegisterType.Transient)]
    internal class OpenApiServiceInTranslator : Translator<ServiceVersioned, IVmOpenApiServiceInVersionBase>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override IVmOpenApiServiceInVersionBase TranslateEntityToVm(ServiceVersioned entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceInTranslator!");
        }
        public override ServiceVersioned TranslateVmToEntity(IVmOpenApiServiceInVersionBase vModel)
        {
            var exists = vModel.Id.IsAssigned();
            if (exists)
            {
                var entityId = vModel.Id.Value;
                vModel.ServiceNames.ForEach(n => n.OwnerReferenceId = entityId);
                vModel.ServiceDescriptions.ForEach(d => d.OwnerReferenceId = entityId);
                vModel.Requirements.ForEach(r => r.OwnerReferenceId = entityId);
                vModel.Keywords.ForEach(k => k.OwnerReferenceId = entityId);
                vModel.Legislation.ForEach(l => l.OwnerReferenceId = entityId);
                vModel.Areas.ForEach(a => a.OwnerReferenceId = entityId);
                vModel.ServiceVouchers.ForEach(sv => sv.OwnerReferenceId = entityId);
            }
            else
            {
                // Set default values for POST operation
                if (vModel.FundingType.IsNullOrEmpty())
                {
                    vModel.FundingType = ServiceFundingTypeEnum.PubliclyFunded.ToString();
                }
            }

            var definitions = CreateViewModelEntityDefinition<ServiceVersioned>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => i.Id.Value == o.Id)
                .UseVersioning<ServiceVersioned, Service>(o => o);

            // Set available languages
            var languages = GetAvailableLanguages(vModel);
            if (languages.Count > 0)
            {
                definitions.AddCollection(i => languages, o => o.LanguageAvailabilities, true);
            }

            // General description
            if (!string.IsNullOrEmpty(vModel.GeneralDescriptionId))
            {
               definitions = definitions.AddSimple(i => vModel.GeneralDescriptionId.ParseToGuidWithExeption(), o => o.StatutoryServiceGeneralDescriptionId)
                    .AddSimple(i => new Guid?(), o => o.TypeId);
            }
            else if (vModel.DeleteGeneralDescriptionId == true)
            {
                definitions = definitions.AddSimple(i => (Guid?)null, o => o.StatutoryServiceGeneralDescriptionId);
            }

            // Service type
            if (!string.IsNullOrEmpty(vModel.Type))
            {
                definitions.AddSimple(i => typesCache.Get<ServiceType>(i.Type), o => o.TypeId);
            }

            // Service funding type
            if (!string.IsNullOrEmpty(vModel.FundingType))
            {
                definitions.AddSimple(i => typesCache.Get<ServiceFundingType>(i.FundingType), o => o.FundingTypeId);
            }

            // Service names
            if (vModel.ServiceNames?.Count > 0)
            {
                definitions.AddCollection(i => i.ServiceNames, o => o.ServiceNames, true);
            }

            // Descriptions
            if (vModel.ServiceDescriptions != null && vModel.ServiceDescriptions.Count > 0)
            {
                definitions.AddCollection(i => i.ServiceDescriptions, o => o.ServiceDescriptions, true);
            }

            // Requirements
            if (vModel.Requirements?.Count > 0)
            {
                definitions.AddCollection(i => i.Requirements, o => o.ServiceRequirements, false);
            }
            
            // set chargeType
            if (vModel.DeleteServiceChargeType || !string.IsNullOrEmpty(vModel.ServiceChargeType))
            {
                var chargeType = (string.Compare(vModel.ServiceChargeType, ServiceChargeTypeEnum.Other.ToString(), StringComparison.CurrentCultureIgnoreCase) == 0)
                        ? null
                        : vModel.ServiceChargeType;
                definitions.AddSimple(i => (chargeType == null) ? (Guid?)null : typesCache.Get<ServiceChargeType>(chargeType), o => o.ChargeTypeId);
            }   

            // Languages
            if (vModel.Languages?.Count > 0)
            {
                var languageList = new List<VmOpenApiStringItem>();
                // Append ordering number for each item
                var index = 1;
                vModel.Languages.ForEach(l => languageList.Add(new VmOpenApiStringItem { Value = l, OwnerReferenceId = vModel.Id, Order = index++ }));
                definitions.AddCollection(i => languageList, o => o.ServiceLanguages, false);
            }         

            // Area type
            if (!exists || !vModel.AreaType.IsNullOrEmpty())
            {
                //Let's use default value for area type if no are type is given
                var typeID = vModel.AreaType.IsNullOrEmpty() ? typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString()) : typesCache.Get<AreaInformationType>(vModel.AreaType);
                definitions.AddSimple(i => typeID, o => o.AreaInformationTypeId);
            }

            // Set areas
            if (vModel.Areas.Count > 0)
            {
                var municipalityAreas = vModel.Areas.Where(a => a.Type == AreaTypeEnum.Municipality.ToString()).ToList();
                var otherAreas = vModel.Areas.Where(a => a.Type != AreaTypeEnum.Municipality.ToString()).ToList();
                if (municipalityAreas.Count > 0)
                {
                    var municipalities = new List<VmOpenApiStringItem>();
                    municipalityAreas.ForEach(area =>
                    {
                        area.AreaCodes.ForEach(m => municipalities.Add(new VmOpenApiStringItem { Value = m, OwnerReferenceId = vModel.Id }));
                    });
                    definitions.AddCollection(i => municipalities, o => o.AreaMunicipalities, false); // Update municipalities
                    if (otherAreas.Count == 0)
                    {
                        definitions.AddCollection(i => new List<VmOpenApiArea>(), o => o.Areas, false); // Remove possible old areas
                    }
                }
                if (otherAreas.Count > 0)
                {
                    var areas = new List<VmOpenApiArea>();
                    otherAreas.ForEach(area =>
                    {
                        area.AreaCodes.ForEach(a => areas.Add(new VmOpenApiArea { Type = area.Type, Code = a, OwnerReferenceId = vModel.Id }));
                    });
                    definitions.AddCollection(i => areas, o => o.Areas, false); // Update areas
                    if (municipalityAreas.Count == 0)
                    {
                        definitions.AddCollection(i => new List<VmOpenApiStringItem>(), o => o.AreaMunicipalities, false); // Remove possible old municipalities
                    }
                }
            }
            else if (exists && !vModel.AreaType.IsNullOrEmpty() && vModel.AreaType != AreaInformationTypeEnum.AreaType.ToString())
            {
                // Are type has been changed into WholeCountry or WholeCountryExceptAlandIslands so we need to remove possible old municipalities and areas.
                definitions.AddCollection(i => new List<VmOpenApiStringItem>(), o => o.AreaMunicipalities, false);
                definitions.AddCollection(i => new List<VmOpenApiArea>(), o => o.Areas, false);
            }

            if (vModel.ServiceClasses?.Count > 0)
            {
                var classes = new List<VmOpenApiStringItem>();
                vModel.ServiceClasses.ForEach(o => classes.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => classes, o => o.ServiceServiceClasses, false);
            }

            if (vModel.OntologyTerms?.Count > 0)
            {
                var terms = new List<VmOpenApiStringItem>();
                vModel.OntologyTerms.ForEach(o => terms.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => terms, o => o.ServiceOntologyTerms, false);
            }

            if (vModel.TargetGroups?.Count > 0)
            {
                var groups = new List<VmOpenApiStringItem>();
                vModel.TargetGroups.ForEach(o => groups.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => groups, o => o.ServiceTargetGroups, false);
            }

            if (vModel.DeleteAllLifeEvents || vModel.LifeEvents?.Count > 0)
            {
                var events = new List<VmOpenApiStringItem>();
                vModel.LifeEvents.ForEach(o => events.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => events, o => o.ServiceLifeEvents, false);
            }

            if (vModel.DeleteAllIndustrialClasses || vModel.IndustrialClasses?.Count > 0)
            {
                var classes = new List<VmOpenApiStringItem>();
                vModel.IndustrialClasses.ForEach(o => classes.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.Id }));
                definitions.AddCollection(i => classes, o => o.ServiceIndustrialClasses, false);
            }

            if (vModel.DeleteAllLaws || vModel.Legislation?.Count > 0)
            {
                definitions.AddCollection(i => i.Legislation, o => o.ServiceLaws, false);
            }

            if (vModel.DeleteAllKeywords || vModel.Keywords?.Count > 0)
            {
                definitions.AddCollection(i => i.Keywords, o => o.ServiceKeywords, false);
            }

            if (vModel.OtherResponsibleOrganizations?.Count > 0)
            {
                definitions.AddCollectionWithRemove(i => i.OtherResponsibleOrganizations.Select( oId => new V7VmOpenApiOrganizationServiceIn{OrganizationId = oId, OwnerReferenceId = vModel.Id}), o => o.OrganizationServices, TranslationPolicy.FetchData, r => true);
            }

            if (vModel.ServiceVouchersInUse)
            {
                definitions.AddSimple(i => i.ServiceVouchersInUse, o => o.WebPageInUse);
            }
            if (vModel.DeleteAllServiceVouchers || vModel.ServiceVouchers?.Count > 0)
            {
                if (vModel.DeleteAllServiceVouchers && (vModel.ServiceVouchers == null || vModel.ServiceVouchers.Count == 0) && !vModel.ServiceVouchersInUse)
                {
                    definitions.AddSimple(i => false, o => o.WebPageInUse);
                }
                else
                {
                    definitions.AddSimple(i => true, o => o.WebPageInUse);
                }                
                definitions.AddCollectionWithRemove(i => i.ServiceVouchers, o => o.ServiceWebPages, TranslationPolicy.FetchData, r => true);
            }            

            if (vModel.ServiceProducers?.Count > 0)
            {
                definitions.AddCollectionWithRemove(i => i.ServiceProducers, o => o.ServiceProducers, TranslationPolicy.FetchData, r => true);
            }

            // Main responsible organization
            if (!string.IsNullOrEmpty(vModel.MainResponsibleOrganization))
            {
                definitions = definitions.AddSimple(i => vModel.MainResponsibleOrganization.ParseToGuidWithExeption(), o => o.OrganizationId);
            }

            return definitions.GetFinal();
        }

        private List<VmOpenApiLanguageAvailability> GetAvailableLanguages(IVmOpenApiServiceInVersionBase vModel)
        {
            var languages = new List<VmOpenApiLanguageAvailability>();
            var currentPublishingStatus = !string.IsNullOrEmpty(vModel.CurrentPublishingStatus) ? vModel.CurrentPublishingStatus : PublishingStatus.Draft.ToString();
            vModel.AvailableLanguages.ForEach(lang =>
            {
                languages.Add(new VmOpenApiLanguageAvailability() { Language = lang, OwnerReferenceId = vModel.Id, PublishingStatus = currentPublishingStatus });
            });
            
            return languages;
        }
    }
}
