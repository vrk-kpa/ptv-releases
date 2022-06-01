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
using PTV.Framework.Extensions;

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
            var exists = vModel.VersionId.IsAssigned();
            if (exists)
            {
                var entityId = vModel.VersionId.Value;
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
                .UseDataContextUpdate(i => exists, i => o => i.VersionId.Value == o.Id)
                .UseVersioning<ServiceVersioned, Service>(o => o);

            // Set available languages
            var languages = GetAvailableLanguages(vModel);
            if (languages.Count > 0)
            {
                definitions.AddCollectionWithRemove(i => languages, o => o.LanguageAvailabilities, x => true);
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
                definitions.AddSimple(i => typesCache.Get<ServiceType>(i.Type.GetEnumValueByOpenApiEnumValue<ServiceTypeEnum>()), o => o.TypeId);
            }

            // Service funding type
            if (!string.IsNullOrEmpty(vModel.FundingType))
            {
                definitions.AddSimple(i => typesCache.Get<ServiceFundingType>(i.FundingType), o => o.FundingTypeId);
            }

            // Service names
            if (vModel.ServiceNames?.Count > 0)
            {
                definitions.AddCollectionWithRemove(i => i.ServiceNames, o => o.ServiceNames, x => true);
            }

            // Descriptions
            if (vModel.ServiceDescriptions != null && vModel.ServiceDescriptions.Count > 0)
            {
                definitions.AddCollectionWithRemove(i => i.ServiceDescriptions, o => o.ServiceDescriptions, x => true);
            }

            // Requirements
            if (vModel.Requirements?.Count > 0)
            {
                definitions.AddCollectionWithRemove(i => i.Requirements, o => o.ServiceRequirements, x => true);
            }

            // set chargeType
            if (vModel.DeleteServiceChargeType || !string.IsNullOrEmpty(vModel.ServiceChargeType))
            {
                var chargeType = string.IsNullOrEmpty(vModel.ServiceChargeType) ? null : vModel.ServiceChargeType.GetEnumValueByOpenApiEnumValue<ServiceChargeTypeEnum>();
                if (string.Compare(chargeType, ServiceChargeTypeEnum.Other.ToString(), StringComparison.CurrentCultureIgnoreCase) == 0)
                {
                    chargeType = null;
                }
                definitions.AddSimple(i => (chargeType == null) ? (Guid?)null : typesCache.Get<ServiceChargeType>(chargeType), o => o.ChargeTypeId);
            }

            // Languages
            if (vModel.Languages?.Count > 0)
            {
                var languageList = new List<VmOpenApiStringItem>();
                // Append ordering number for each item
                var index = 1;
                vModel.Languages.ForEach(l => languageList.Add(new VmOpenApiStringItem { Value = l, OwnerReferenceId = vModel.VersionId, Order = index++ }));
                definitions.AddCollectionWithRemove(i => languageList, o => o.ServiceLanguages, x => true);
            }

            // Area type
            if (!exists || !vModel.AreaType.IsNullOrEmpty())
            {
                //Let's use default value for area type if no are type is given
                var typeID = vModel.AreaType.IsNullOrEmpty() ? typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString())
                    : typesCache.Get<AreaInformationType>(vModel.AreaType.GetEnumValueByOpenApiEnumValue<AreaInformationTypeEnum>());
                definitions.AddSimple(i => typeID, o => o.AreaInformationTypeId);
            }

            // Set areas
            if (vModel.Areas?.Count > 0)
            {
                var municipalityAreas = vModel.Areas.Where(a => a.Type == AreaTypeEnum.Municipality.ToString()).ToList();
                var otherAreas = vModel.Areas.Where(a => a.Type != AreaTypeEnum.Municipality.ToString()).ToList();
                if (municipalityAreas.Count > 0)
                {
                    var municipalities = new List<VmOpenApiStringItem>();
                    municipalityAreas.ForEach(area =>
                    {
                        area.AreaCodes.ForEach(m => municipalities.Add(new VmOpenApiStringItem { Value = m, OwnerReferenceId = vModel.VersionId }));
                    });
                    definitions.AddCollectionWithRemove(i => municipalities, o => o.AreaMunicipalities, x => true); // Update municipalities
                    if (otherAreas.Count == 0)
                    {
                        definitions.AddCollectionWithRemove(i => new List<VmOpenApiArea>(), o => o.Areas, x => true); // Remove possible old areas
                    }
                }
                if (otherAreas.Count > 0)
                {
                    var areas = new List<VmOpenApiArea>();
                    otherAreas.ForEach(area =>
                    {
                        area.AreaCodes.ForEach(a => areas.Add(new VmOpenApiArea { Type = area.Type, Code = a, OwnerReferenceId = vModel.VersionId }));
                    });
                    definitions.AddCollectionWithRemove(i => areas, o => o.Areas, x => true); // Update areas
                    if (municipalityAreas.Count == 0)
                    {
                        definitions.AddCollectionWithRemove(i => new List<VmOpenApiStringItem>(), o => o.AreaMunicipalities, x => true); // Remove possible old municipalities
                    }
                }
            }
            else if (exists && !vModel.AreaType.IsNullOrEmpty() && vModel.AreaType != AreaInformationTypeEnum.AreaType.GetOpenApiValue())
            {
                // Are type has been changed into WholeCountry or WholeCountryExceptAlandIslands so we need to remove possible old municipalities and areas.
                definitions.AddCollectionWithRemove(i => new List<VmOpenApiStringItem>(), o => o.AreaMunicipalities, x => true);
                definitions.AddCollectionWithRemove(i => new List<VmOpenApiArea>(), o => o.Areas, x => true);
            }

            if (vModel.ServiceClasses?.Count > 0)
            {
                var classes = new List<VmOpenApiStringItem>();
                vModel.ServiceClasses.ForEach(o => classes.Add(new VmOpenApiStringItem
                    {Value = o, OwnerReferenceId = vModel.VersionId}));
                definitions.AddCollectionWithRemove(i => classes, o => o.ServiceServiceClasses, x => true);
            }

            if (vModel.OntologyTerms?.Count > 0)
            {
                var terms = new List<VmOpenApiStringItem>();
                vModel.OntologyTerms.ForEach(o => terms.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.VersionId }));
                definitions.AddCollectionWithRemove(i => terms, o => o.ServiceOntologyTerms, x => true);
            }

            if (vModel.TargetGroups?.Count > 0)
            {
                var groups = new List<VmOpenApiStringItem>();
                vModel.TargetGroups.ForEach(o => groups.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.VersionId }));
                definitions.AddCollectionWithRemove(i => groups, o => o.ServiceTargetGroups, x => true);
            }

            if (vModel.DeleteAllLifeEvents || vModel.LifeEvents?.Count > 0)
            {
                var events = new List<VmOpenApiStringItem>();
                vModel.LifeEvents.ForEach(o => events.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.VersionId }));
                definitions.AddCollectionWithRemove(i => events, o => o.ServiceLifeEvents, x => true);
            }

            if (vModel.DeleteAllIndustrialClasses || vModel.IndustrialClasses?.Count > 0)
            {
                var classes = new List<VmOpenApiStringItem>();
                vModel.IndustrialClasses.ForEach(o => classes.Add(new VmOpenApiStringItem { Value = o, OwnerReferenceId = vModel.VersionId }));
                definitions.AddCollectionWithRemove(i => classes, o => o.ServiceIndustrialClasses, x => true);
            }

            if (vModel.DeleteAllLaws || vModel.Legislation?.Count > 0)
            {
                var order = 1;
                vModel.Legislation.ForEach(l => l.OrderNumber = order++);
                definitions.AddCollectionWithRemove(i => i.Legislation, o => o.ServiceLaws, x => true);
            }

            if (vModel.DeleteAllKeywords || vModel.Keywords?.Count > 0)
            {
                definitions.AddCollectionWithRemove(i => i.Keywords, o => o.ServiceKeywords, x => true);
            }

            if (vModel.OtherResponsibleOrganizations?.Count > 0)
            {
                definitions.AddCollectionWithRemove(i => i.OtherResponsibleOrganizations.Select( oId => new V7VmOpenApiOrganizationServiceIn{OrganizationId = oId, OwnerReferenceId = vModel.VersionId }), o => o.OrganizationServices, TranslationPolicy.FetchData, r => true);
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
                    definitions.AddSimple(i => typesCache.Get<VoucherType>(VoucherTypeEnum.NotUsed.ToString()), o => o.VoucherTypeId);
                }
                else
                {
                    // The actual validity (correctly formed url) is checked elsewhere
                    var notValidUrlsCount = vModel.ServiceVouchers.Count(x => string.IsNullOrWhiteSpace(x.Url));
                    if (notValidUrlsCount == vModel.ServiceVouchers.Count)
                    {
                        // OpenAPI caller can provide n vouchers. Some of them might have urls and some don't. The VoucherType is at the
                        // service level so it is caller's responsibility to provide valid urls for each voucher. With the current logic
                        // it is not possible to give voucher information that does not have urls because those are not saved
                        // (scroll down to see AddCollectionWithRemove() call)
                        definitions.AddSimple(i => false, o => o.WebPageInUse);
                        definitions.AddSimple(i => typesCache.Get<VoucherType>(VoucherTypeEnum.NoUrl.ToString()), o => o.VoucherTypeId);
                    }
                    else
                    {
                        definitions.AddSimple(i => true, o => o.WebPageInUse);
                        definitions.AddSimple(i => typesCache.Get<VoucherType>(VoucherTypeEnum.Url.ToString()), o => o.VoucherTypeId);
                    }
                }
                
                // Is the order numbers already set for vouchers (PTV-3705)
                if (!vModel.ServiceVouchers.Any(v => v.OrderNumber != 0))
                {
                    var order = 1;
                    vModel.ServiceVouchers.ForEach(l => l.OrderNumber = order++);
                }
                
                definitions.AddCollectionWithRemove(
                    i => i.ServiceVouchers.Where(x => !x.Url.IsNullOrWhitespace()),
                    o => o.ServiceWebPages,
                    TranslationPolicy.FetchData,
                    r => true);
            }

            if (vModel.ServiceProducers?.Count > 0)
            {
                // Is the order numbers already set for producers (PTV-3705)
                if (!vModel.ServiceProducers.Any(v => v.OrderNumber != 0))
                {
                    var order = 1;
                    vModel.ServiceProducers.ForEach(l => l.OrderNumber = order++);
                }
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
            vModel.AvailableLanguages.ForEach(lang =>
            {
                languages.Add(new VmOpenApiLanguageAvailability
                {
                    Language = lang,
                    OwnerReferenceId = vModel.VersionId,
                    PublishingStatus = vModel.PublishingStatus,
                    PublishAt = vModel.ValidFrom,
                    ArchiveAt = vModel.ValidTo,
                    ReviewedBy = vModel.UserName});
            });

            return languages;
        }
    }
}
