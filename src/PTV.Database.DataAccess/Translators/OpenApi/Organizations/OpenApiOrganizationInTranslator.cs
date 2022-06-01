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
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationVersioned, IVmOpenApiOrganizationInVersionBase>), RegisterType.Transient)]
    internal class OpenApiOrganizationInTranslator : OpenApiOrganizationBaseTranslator<IVmOpenApiOrganizationInVersionBase>
    {
        public OpenApiOrganizationInTranslator(
            IResolveManager resolveManager, 
            ITranslationPrimitives translationPrimitives, 
            ICacheManager cacheManager) 
            : base(resolveManager, translationPrimitives, cacheManager)
        {
        }

        public override IVmOpenApiOrganizationInVersionBase TranslateEntityToVm(OrganizationVersioned entity)
        {
            throw new NotImplementedException();
        }

        public override OrganizationVersioned TranslateVmToEntity(IVmOpenApiOrganizationInVersionBase vModel)
        {
            var definition = CreateBaseVmEntityDefinitions(vModel)
                .DisableAutoTranslation();

            var exists = vModel.VersionId.IsAssigned();

            if (exists) // We are updating the items within certain version
            {
                var entityId = vModel.VersionId.Value;
                vModel.OrganizationDescriptions.ForEach(d => d.OwnerReferenceId = entityId);
                vModel.Emails.ForEach(e => e.OwnerReferenceId = entityId);
                vModel.PhoneNumbers.ForEach(p => p.OwnerReferenceId = entityId);
                vModel.WebPages.ForEach(w => w.OwnerReferenceId = entityId);
                vModel.Addresses.ForEach(a => a.OwnerReferenceId = entityId);
                vModel.ElectronicInvoicings.ForEach(e => e.OwnerReferenceId = entityId);
            }

            // Set order number (PTV-3705)
            var order = 1;
            vModel.Emails.ForEach(e => e.OrderNumber = order++);
            order = 1;
            vModel.PhoneNumbers.ForEach(p => p.OrderNumber = order++);
            // Check if order number has already been set. In older versions (< 8) user could set the order number for web page.
            if (vModel.WebPages?.Count > 0 && !vModel.WebPages.Any(w => w.OrderNumber != 0))
            {
                order = 1;
                vModel.WebPages.ForEach(w => w.OrderNumber = order++);
            }
            order = 1;
            vModel.Addresses.ForEach(a => a.OrderNumber = order++);

            // Set available languages
            var languages = new List<VmOpenApiLanguageAvailability>();
            vModel.AvailableLanguages.ForEach(lang =>
            {
                if (!languages.Select(l => l.Language).ToList().Contains(lang))
                {
                    languages.Add(new VmOpenApiLanguageAvailability
                    { Language = lang, OwnerReferenceId = vModel.VersionId, PublishingStatus = vModel.PublishingStatus,
                        PublishAt = vModel.ValidFrom, ArchiveAt = vModel.ValidTo, ReviewedBy = vModel.UserName });
                }
            });
            if (languages.Count > 0)
            {
                definition.AddCollectionWithRemove(i => languages, o => o.LanguageAvailabilities, x => true);
            }

            // Municipality
            if (!string.IsNullOrEmpty(vModel.Municipality))
            {
                definition.AddNavigation(i => i.Municipality, o => o.Municipality);
                // We need to remove possible areas if municipality has been set and we are updating existing item
                if (exists)
                {
                    definition.AddCollectionWithRemove(i => new List<VmOpenApiArea>(), o => o.OrganizationAreas, x => true); // Remove possible old areas
                }
            }

            // Descriptions
            if (vModel.OrganizationDescriptions?.Count > 0)
            {
                definition.AddCollectionWithRemove(i => i.OrganizationDescriptions, o => o.OrganizationDescriptions, x => true);
            }

            // Areas
            if (!vModel.SubAreaType.IsNullOrEmpty() && vModel.Areas?.Count > 0)
            {
                var subAreaType = (AreaTypeEnum)vModel.SubAreaType.GetEnumByOpenApiEnumValue(typeof(AreaTypeEnum));
                if (subAreaType == AreaTypeEnum.Municipality)
                {
                    var municipalities = new List<VmOpenApiStringItem>();
                    vModel.Areas.ForEach(m => municipalities.Add(new VmOpenApiStringItem { Value = m, OwnerReferenceId = vModel.VersionId }));
                    definition.AddCollectionWithRemove(i => municipalities, o => o.OrganizationAreaMunicipalities, x => true); // Update municipalities
                    definition.AddCollectionWithRemove(i => new List<VmOpenApiArea>(), o => o.OrganizationAreas, x => true); // Remove possible old areas
                }
                else
                {
                    var areas = new List<VmOpenApiArea>();
                    vModel.Areas.ForEach(a => areas.Add(new VmOpenApiArea { Type = vModel.SubAreaType, Code = a, OwnerReferenceId = vModel.VersionId }));
                    definition.AddCollectionWithRemove(i => areas, o => o.OrganizationAreas, x => true); // Update areas
                    definition.AddCollectionWithRemove(i => new List<VmOpenApiStringItem>(), o => o.OrganizationAreaMunicipalities, x => true); // Remove possible old municipalities
                }
            }
            else if (exists && !vModel.AreaType.IsNullOrEmpty() && vModel.AreaType != AreaInformationTypeEnum.AreaType.GetOpenApiValue())
            {
                // Area type has been changed into WholeCountry or WholeCountryExceptAlandIslands so we need to remove possible old municipalities and areas.
                definition.AddCollectionWithRemove(i => new List<VmOpenApiStringItem>(), o => o.OrganizationAreaMunicipalities, x => true);
                definition.AddCollectionWithRemove(i => new List<VmOpenApiArea>(), o => o.OrganizationAreas, x => true);
            }

            // Emails
            if (vModel.DeleteAllEmails || vModel.Emails?.Count > 0)
            {
                definition.AddCollectionWithRemove(i => i.Emails.Select(e => new VmEmailData
                {
                    Id = e.Id,
                    OwnerReferenceId = e.OwnerReferenceId ?? Guid.Empty,
                    Email = e.Value,
                    AdditionalInformation = e.Description,
                    LanguageId = e.Language != null ? languageCache.Get(e.Language) : Guid.Empty,
                    OrderNumber = e.OrderNumber
                }), o => o.OrganizationEmails, x => true);
            }

            // Phones
            if (vModel.DeleteAllPhones || vModel.PhoneNumbers?.Count > 0)
            {
                definition.AddCollectionWithRemove(i => i.PhoneNumbers, o => o.OrganizationPhones, x => true);
            }

            // Web pages
            if (vModel.DeleteAllWebPages || vModel.WebPages?.Count > 0)
            {
                definition.AddCollectionWithRemove(
                    i => i.WebPages.Where(x => !x.Url.IsNullOrWhitespace()),
                    o => o.OrganizationWebAddress,
                    x => true);
            }

            // Addresses
            if (vModel.DeleteAllAddresses || vModel.Addresses?.Count > 0)
            {
                var list = vModel.Addresses
                    // Postal addresses have to handled separately, after saving the main entity. See AddressService
                    .Where(x => x.SubType.Parse<AddressTypeEnum>() != AddressTypeEnum.PostOfficeBox)
                    .ToList();
                
                definition.AddCollectionWithRemove(i => list, o => o.OrganizationAddresses, x => true);
            }

            // Electronic invoicing addresses
            if (vModel.DeleteAllElectronicInvoicings || vModel.ElectronicInvoicings?.Count > 0)
            {
                definition.AddCollectionWithRemove(
                    i => vModel.DeleteAllElectronicInvoicings
                        ? new List<VmOpenApiOrganizationEInvoicing>()
                        : i.ElectronicInvoicings,
                    o => o.OrganizationEInvoicings,
                    def => true);
            }

            if (!string.IsNullOrEmpty(vModel.ParentOrganizationId))
            {
                definition.AddSimple(i => Guid.Parse(i.ParentOrganizationId), o => o.ParentId);
            }

/* SOTE has been disabled (SFIPTV-1177)
            if (!string.IsNullOrEmpty(vModel.ResponsibleOrganizationId))
            {
                definition.AddSimple(i => Guid.Parse(i.ResponsibleOrganizationId), o => o.ResponsibleOrganizationRegionId);
            }
            else if(vModel.OrganizationType != OrganizationTypeEnum.SotePublic.ToString() && vModel.OrganizationType != OrganizationTypeEnum.SotePrivate.ToString())
            {
                // We need to empty possible responsible organization since responsible organization can only be defined for organizations with type SotePublic or SotePrivate (SFIPTV-601)
                definition.AddSimple(i => (Guid?)null, o => o.ResponsibleOrganizationRegionId);
            }
*/
            return definition.GetFinal();
        }
    }
}
