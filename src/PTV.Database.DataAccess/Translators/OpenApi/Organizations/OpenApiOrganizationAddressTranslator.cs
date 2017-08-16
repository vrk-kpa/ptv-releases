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
using PTV.Database.DataAccess.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi.V5;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationAddress, V5VmOpenApiAddressWithTypeIn>), RegisterType.Transient)]
    internal class OpenApiOrganizationAddressTranslator : Translator<OrganizationAddress, V5VmOpenApiAddressWithTypeIn>
    {
        private readonly ITypesCache typesCache;
        public OpenApiOrganizationAddressTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V5VmOpenApiAddressWithTypeIn TranslateEntityToVm(OrganizationAddress entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiOrganizationAddressTranslator");
        }

        public override OrganizationAddress TranslateVmToEntity(V5VmOpenApiAddressWithTypeIn vModel)
        {
            if (vModel.Id.IsAssigned())
            {
                vModel.StreetAddress.ForEach(s => s.OwnerReferenceId = vModel.Id.Value);
                vModel.AdditionalInformations.ForEach(a => a.OwnerReferenceId = vModel.Id.Value);
            }
            var definition = CreateViewModelEntityDefinition<OrganizationAddress>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.Id.IsAssigned())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => (vModel.Id.Value == o.AddressId) &&
                    (!i.OwnerReferenceId.IsAssigned() || o.OrganizationVersionedId == vModel.OwnerReferenceId))
                .AddNavigation(i => i, o => o.Address);

            if (!string.IsNullOrEmpty(vModel.Type))
            {
                definition.AddSimple(i => typesCache.Get<AddressCharacter>(i.Type), o => o.CharacterId);
            }

            return definition.GetFinal();
        }
    }
}
