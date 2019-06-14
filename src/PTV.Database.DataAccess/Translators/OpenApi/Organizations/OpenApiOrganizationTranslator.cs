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
using PTV.Domain.Model.Enums;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V10;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmOpenApiOrganizationVersionBase>), RegisterType.Transient)]
    internal class OpenApiOrganizationTranslator : OpenApiOrganizationBaseTranslator<VmOpenApiOrganizationVersionBase>
    {
        private readonly ITypesCache typesCache;
        public OpenApiOrganizationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiOrganizationVersionBase TranslateEntityToVm(OrganizationVersioned entity)
        {
            if (entity == null) return null;

            var definitions = CreateBaseEntityVmDefinitions(entity)
                // We have to use unique root id as id!
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddSimple(i => i.Id, o => o.VersionId)
                .AddNavigation(i => i.Municipality, o => o.Municipality)
                .AddCollection(i => i.OrganizationDescriptions, o => o.OrganizationDescriptions)
                .AddCollection(i => i.OrganizationEmails.OrderBy(e => e.Email.OrderNumber).ThenBy(e => e.Email.Modified).Select(e => e.Email), o => o.Emails)
                .AddCollection(i => i.OrganizationPhones.OrderBy(p => p.Phone.OrderNumber).ThenBy(p => p.Phone.Modified), o => o.PhoneNumbers)
                .AddCollection(i => i.OrganizationWebAddress.OrderBy(w => w.WebPage.OrderNumber).ThenBy(w => w.WebPage.Modified), o => o.WebPages)
                .AddCollection(i => i.OrganizationAddresses.OrderBy(x => x.CharacterId).ThenBy(x => x.Address.OrderNumber).ThenBy(x => x.Address.Modified), o => o.Addresses)
                .AddCollection(i => i.OrganizationEInvoicings, o => o.ElectronicInvoicings)
                .AddCollection(i => i.UnificRoot?.OrganizationServices, o => o.Services)
                .AddCollection(i => i.UnificRoot?.OrganizationServicesVersioned, o => o.ResponsibleOrganizationServices)
                .AddCollection(i => i.UnificRoot?.ServiceProducerOrganizations, o => o.ProducerOrganizationServices)
                .AddSimple(i => i.ParentId, o => o.ParentOrganizationId)
                .AddSimple(i => i.Modified, o => o.Modified)
                .AddCollection(i => i.UnificRoot?.Children, o => o.SubOrganizations)
                .AddSimple(i => i.ResponsibleOrganizationRegionId, o => o.ResponsibleOrganizationId)
                .AddNavigation(i => typesCache.GetByValue<PublishingStatusType>(i.PublishingStatusId), o => o.PublishingStatus);

            if (entity.OrganizationAreaMunicipalities?.Count > 0)
            {
                definitions.AddCollection(i => i.OrganizationAreaMunicipalities.Select(a => a.Municipality), o => o.AreaMunicipalities);
            }
            else
            {
                if (entity.OrganizationAreas?.Count > 0)
                {
                    definitions.AddCollection(i => i.OrganizationAreas.Select(a => a.Area), o => o.Areas);
                }                
            }

            var vm = definitions.GetFinal();
            if (vm.AreaMunicipalities?.Count > 0)
            {
                if (vm.Areas == null)
                {
                    vm.Areas = new List<VmOpenApiArea>();
                }
                vm.Areas.Add(new VmOpenApiArea { Type = AreaTypeEnum.Municipality.ToString(), Municipalities = vm.AreaMunicipalities });
            }

            // Map main responsible organizations and producers
            if (vm.ResponsibleOrganizationServices?.Count > 0 || vm.ProducerOrganizationServices?.Count > 0)
            {
                var responsible = CommonConsts.RESPONSIBLE;
                if (vm.Services == null)
                {
                    vm.Services = new List<V10VmOpenApiOrganizationService>();
                }
                vm.ResponsibleOrganizationServices.ForEach(service =>
                {
                    // Check if there already exists the main responsible organization within OtherResponsible organizations
                    var otherResponsible = vm.Services.Where(s => s.Service.Id == service.Id && s.RoleType == "OtherResponsible").FirstOrDefault();
                    if (otherResponsible != null)
                    {
                        otherResponsible.RoleType = responsible;
                    }
                    else
                    {
                        vm.Services.Add(new V10VmOpenApiOrganizationService()
                        {
                            RoleType = responsible,
                            OrganizationId = vm.Id.ToString(),
                            Service = service,
                            AdditionalInformation = new List<VmOpenApiLanguageItem>(),
                        });
                    }
                });
                vm.ProducerOrganizationServices.ForEach(service => vm.Services.Add(service));
            }            

            return vm;
        }

        public override OrganizationVersioned TranslateVmToEntity(VmOpenApiOrganizationVersionBase vModel)
        {
            throw new NotImplementedException();
        }
    }
}
