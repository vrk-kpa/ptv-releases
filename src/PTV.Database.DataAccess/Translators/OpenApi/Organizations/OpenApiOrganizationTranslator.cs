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
using PTV.Domain.Model.Models.OpenApi.V2;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    [RegisterService(typeof(ITranslator<Organization, V2VmOpenApiOrganization>), RegisterType.Transient)]
    internal class OpenApiOrganizationTranslator : OpenApiOrganizationBaseTranslator<V2VmOpenApiOrganization>
    {
        public OpenApiOrganizationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
        }

        public override V2VmOpenApiOrganization TranslateEntityToVm(Organization entity)
        {

            return CreateBaseEntityVmDefinitions(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddCollection(i => i.OrganizationDescriptions, o => o.OrganizationDescriptions)
                .AddCollection(i => i.OrganizationEmails.Select(e => e.Email), o => o.EmailAddresses)
                .AddCollection(i => i.OrganizationPhones, o => o.PhoneNumbers)
                .AddCollection(i => i.OrganizationWebAddress, o => o.WebPages)
                .AddCollection(i => i.OrganizationAddresses, o => o.Addresses)
                .AddCollection(i => i.OrganizationServices, o => o.Services)
                .AddSimple(i => i.ParentId, o => o.ParentOrganization)
                .GetFinal();
        }

        public override Organization TranslateVmToEntity(V2VmOpenApiOrganization vModel)
        {
            throw new NotImplementedException();
        }
    }
}
