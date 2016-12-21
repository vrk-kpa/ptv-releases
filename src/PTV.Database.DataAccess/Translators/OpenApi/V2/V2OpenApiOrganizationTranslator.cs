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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.V2
{
    [RegisterService(typeof(ITranslator<V2VmOpenApiOrganization, VmOpenApiOrganization>), RegisterType.Transient)]
    internal class V2OpenApiOrganizationTranslator : Translator<V2VmOpenApiOrganization, VmOpenApiOrganization>
    {
        public V2OpenApiOrganizationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiOrganization TranslateEntityToVm(V2VmOpenApiOrganization entity)
        {
            if (entity == null)
            {
                return null;
            }

            var vm = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.Oid, o => o.Oid)
                .AddSimple(i => i.ParentOrganization, o => o.ParentOrganization)
                .AddNavigation(i => i.Municipality, o => o.Municipality)
                .AddNavigation(i => i.OrganizationType, o => o.OrganizationType)
                .AddNavigation(i => i.BusinessCode, o => o.BusinessCode)
                .AddNavigation(i => i.BusinessName, o => o.BusinessName)
                .AddNavigation(i => i.DisplayNameType, o => o.DisplayNameType)
                .AddNavigation(i => i.PublishingStatus, o => o.PublishingStatus)
                .AddCollection(i => i.EmailAddresses, o => o.EmailAddresses)
                .AddCollection(i => i.PhoneNumbers, o => o.PhoneNumbers)
                .AddSimple(i => i.BusinessId, o => o.BusinessId)
                .GetFinal();

            vm.OrganizationNames = entity.OrganizationNames;
            vm.OrganizationDescriptions = entity.OrganizationDescriptions;
            vm.WebPages = entity.WebPages;
            vm.Addresses = new List<VmOpenApiAddressWithType>();
            entity.Addresses.ForEach(a => vm.Addresses.Add(a.ConvertToVersion1()));
            vm.Services = entity.Services;

            return vm;
        }

        public override V2VmOpenApiOrganization TranslateVmToEntity(VmOpenApiOrganization vModel)
        {
            throw new NotImplementedException();
        }
    }
}
