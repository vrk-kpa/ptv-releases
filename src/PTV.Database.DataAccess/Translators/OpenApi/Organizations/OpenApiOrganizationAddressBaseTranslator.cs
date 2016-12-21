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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    internal abstract class OpenApiOrganizationAddressBaseTranslator<TModel> : Translator<OrganizationAddress, TModel> where TModel : class, IV2VmOpenApiAddressWithType
    {
        private readonly ITypesCache typesCache;

        public OpenApiOrganizationAddressBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override TModel TranslateEntityToVm(OrganizationAddress entity)
        {
            return CreateBaseEntityVmDefinitions(entity).GetFinal();
        }
        public override OrganizationAddress TranslateVmToEntity(TModel vModel)
        {
            return CreateBaseVmEntityDefinitions(vModel).GetFinal();
        }

        protected ITranslationDefinitions<OrganizationAddress, TModel> CreateBaseEntityVmDefinitions(OrganizationAddress entity)
        {
            return CreateEntityViewModelDefinition<TModel>(entity)
                .AddNavigation(i => typesCache.GetByValue<AddressType>(i.TypeId), o => o.Type)
                .AddNavigation(i => i.Address.PostOfficeBox, o => o.PostOfficeBox)
                .AddNavigation(i => i.Address.PostalCode?.Code, o => o.PostalCode)
                .AddNavigation(i => i.Address.PostalCode?.PostOffice, o => o.PostOffice)
                .AddNavigation(i => i.Address.Municipality?.Name, o => o.Municipality)
                .AddCollection(i => i.Address.StreetNames, o => o.StreetAddress)
                .AddNavigation(i => i.Address.StreetNumber, o => o.StreetNumber)
                .AddNavigation(i => i.Address.Country, o => o.Country)
                .AddSimple(i => i.AddressId, o => o.Id)
                .AddCollection(i => i.Address.AddressAdditionalInformations, o => o.AdditionalInformations);
        }
        protected ITranslationDefinitions<TModel, OrganizationAddress> CreateBaseVmEntityDefinitions(TModel vModel)
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
                    (!i.OwnerReferenceId.IsAssigned() || o.OrganizationId == vModel.OwnerReferenceId))
                .AddNavigation(i => i, o => o.Address);

            if (!string.IsNullOrEmpty(vModel.Type))
            {
                definition.AddSimple(i => typesCache.Get<AddressType>(i.Type), o => o.TypeId);
            }
            
            return definition;
        }
    }
}
