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
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V5;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationVersioned, IVmOpenApiOrganizationInVersionBase>), RegisterType.Transient)]
    internal class OpenApiOrganizationInTranslator : OpenApiOrganizationBaseTranslator<IVmOpenApiOrganizationInVersionBase>
    {

        private readonly ILanguageCache languageCache;

        public OpenApiOrganizationInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override IVmOpenApiOrganizationInVersionBase TranslateEntityToVm(OrganizationVersioned entity)
        {
            throw new NotImplementedException();
        }

        public override OrganizationVersioned TranslateVmToEntity(IVmOpenApiOrganizationInVersionBase vModel)
        {
            var definition = CreateBaseVmEntityDefinitions(vModel)
                .DisableAutoTranslation();

            var exists = vModel.Id.IsAssigned();

            if (exists) // We are updating the items within certain version
            {
                var entityId = vModel.Id.Value;
                vModel.OrganizationDescriptions.ForEach(d => d.OwnerReferenceId = entityId);
                vModel.EmailAddresses.ForEach(e => e.OwnerReferenceId = entityId);
                vModel.PhoneNumbers.ForEach(p => p.OwnerReferenceId = entityId);
                vModel.WebPages.ForEach(p => p.OwnerReferenceId = entityId);
                vModel.Addresses.ForEach(p => p.OwnerReferenceId = entityId);
            }

            // Set available languages
            var languages = new List<VmOpenApiLanguageAvailability>();
            var currentPublishingStatus = !string.IsNullOrEmpty(vModel.CurrentPublishingStatus) ? vModel.CurrentPublishingStatus : PublishingStatus.Draft.ToString();
            vModel.AvailableLanguages.ForEach(lang =>
            {
                if (!languages.Select(l => l.Language).ToList().Contains(lang))
                {
                    languages.Add(new VmOpenApiLanguageAvailability() { Language = lang, OwnerReferenceId = vModel.Id, PublishingStatus = currentPublishingStatus });
                }
            });
            if (languages.Count > 0)
            {
                definition.AddCollection(i => languages, o => o.LanguageAvailabilities, true);
            }

            if (!string.IsNullOrEmpty(vModel.Municipality))
            {
                definition.AddNavigation(i => i.Municipality, o => o.Municipality);
            }

            if (vModel.OrganizationDescriptions?.Count > 0)
            {
                definition.AddCollection(i => i.OrganizationDescriptions, o => o.OrganizationDescriptions, true);
            }

            // Map areas
            if (!vModel.SubAreaType.IsNullOrEmpty() && vModel.Areas?.Count > 0)
            {
                if (vModel.SubAreaType == AreaTypeEnum.Municipality.ToString())
                {
                    var municipalities = new List<VmOpenApiStringItem>();
                    vModel.Areas.ForEach(m => municipalities.Add(new VmOpenApiStringItem { Value = m, OwnerReferenceId = vModel.Id }));
                    definition.AddCollection(i => municipalities, o => o.OrganizationAreaMunicipalities, false); // Update municipalities
                    definition.AddCollection(i => new List<VmOpenApiArea>(), o => o.OrganizationAreas, false); // Remove possible old areas
                }
                else
                {
                    var areas = new List<VmOpenApiArea>();
                    vModel.Areas.ForEach(a => areas.Add(new VmOpenApiArea { Type = vModel.SubAreaType, Code = a, OwnerReferenceId = vModel.Id }));
                    definition.AddCollection(i => areas, o => o.OrganizationAreas, false); // Update areas
                    definition.AddCollection(i => new List<VmOpenApiStringItem>(), o => o.OrganizationAreaMunicipalities, false); // Remove possible old municipalities
                }
            }
            else if (exists && !vModel.AreaType.IsNullOrEmpty() && vModel.AreaType != AreaInformationTypeEnum.AreaType.ToString())
            {
                // Area type has been changed into WholeCountry or WholeCountryExceptAlandIslands so we need to remove possible old municipalities and areas.
                definition.AddCollection(i => new List<VmOpenApiStringItem>(), o => o.OrganizationAreaMunicipalities, false);
                definition.AddCollection(i => new List<VmOpenApiArea>(), o => o.OrganizationAreas, false);
            }

            if (vModel.DeleteAllEmails || vModel.EmailAddresses?.Count > 0)
            {
                definition.AddCollection(i => i.EmailAddresses.Select(e => new VmEmailData
                {
                    Id = e.Id,
                    OwnerReferenceId = e.OwnerReferenceId ?? Guid.Empty,
                    Email = e.Value,
                    AdditionalInformation = e.Description,
                    LanguageId = e.Language != null ? languageCache.Get(e.Language) : Guid.Empty
                }), o => o.OrganizationEmails, false);
            }

            if (vModel.DeleteAllPhones || vModel.PhoneNumbers?.Count > 0)
            {
                definition.AddCollection(i => i.PhoneNumbers, o => o.OrganizationPhones, false);
            }

            if (vModel.DeleteAllWebPages || vModel.WebPages?.Count > 0)
            {
                definition.AddCollection(i => i.WebPages, o => o.OrganizationWebAddress, false);
            }

            if (vModel.DeleteAllAddresses || vModel.Addresses?.Count > 0)
            {
                definition.AddCollection(i => i.Addresses, o => o.OrganizationAddresses, false);
            }

            if (!string.IsNullOrEmpty(vModel.ParentOrganizationId))
            {
                definition.AddSimple(i => Guid.Parse(i.ParentOrganizationId), o => o.ParentId);
            }

            return definition.GetFinal();
        }
    }
}
