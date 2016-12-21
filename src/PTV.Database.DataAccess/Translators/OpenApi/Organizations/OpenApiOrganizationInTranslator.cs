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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    [RegisterService(typeof(ITranslator<Organization, IV2VmOpenApiOrganizationInBase>), RegisterType.Transient)]
    internal class OpenApiOrganizationInTranslator : OpenApiOrganizationBaseTranslator<IV2VmOpenApiOrganizationInBase>
    {

        private readonly ILanguageCache languageCache;

        public OpenApiOrganizationInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override IV2VmOpenApiOrganizationInBase TranslateEntityToVm(Organization entity)
        {
            throw new NotImplementedException();
        }

        public override Organization TranslateVmToEntity(IV2VmOpenApiOrganizationInBase vModel)
        {
            var definition = CreateBaseVmEntityDefinitions(vModel)
                .DisableAutoTranslation();

            if (vModel.Id.HasValue) // We are updating the items
            {
                vModel.OrganizationDescriptions.ForEach(d => d.OwnerReferenceId = vModel.Id);
                vModel.EmailAddresses.ForEach(e => e.OwnerReferenceId = vModel.Id);
                vModel.PhoneNumbers.ForEach(p => p.OwnerReferenceId = vModel.Id);
                vModel.WebPages.ForEach(p => p.OwnerReferenceId = vModel.Id);
                vModel.Addresses.ForEach(p => p.OwnerReferenceId = vModel.Id);
            }

            if (vModel.OrganizationDescriptions?.Count > 0)
            {
                definition.AddCollection(i => i.OrganizationDescriptions, o => o.OrganizationDescriptions);
            }

            if (vModel.EmailAddresses != null && vModel.EmailAddresses.Count > 0)
            {
                definition.AddCollection(i => i.EmailAddresses.Select(e => new VmEmailData
                {
                    Id = e.Id,
                    OwnerReferenceId = e.OwnerReferenceId ?? Guid.Empty,
                    Email = e.Value,
                    AdditionalInformation = e.Description,
                    LanguageId = e.Language != null ? languageCache.Get(e.Language) : Guid.Empty
                }), o => o.OrganizationEmails);
            }

            if (vModel.PhoneNumbers != null && vModel.PhoneNumbers.Count > 0)
            {
                definition.AddCollection(i => i.PhoneNumbers, o => o.OrganizationPhones);
            }

            if (vModel.WebPages != null && vModel.WebPages.Count > 0)
            {
                definition.AddCollection(i => i.WebPages, o => o.OrganizationWebAddress);
            }

            if(vModel.Addresses != null && vModel.Addresses.Count > 0)
            {
                definition.AddCollection(i => i.Addresses, o => o.OrganizationAddresses);
            }

            if (!string.IsNullOrEmpty(vModel.ParentOrganizationId))
            {
                definition.AddSimple(i => Guid.Parse(i.ParentOrganizationId), o => o.ParentId);
            }

            if (!string.IsNullOrEmpty(vModel.DisplayNameType))
            {
                definition.AddNavigation(i => vModel.DisplayNameType, o => o.DisplayNameType);
            }

            return definition.GetFinal();
        }
    }
}
