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
using PTV.Database.DataAccess.Caches;
using System;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelAddress, V9VmOpenApiAddressIn>), RegisterType.Transient)]
    internal class OpenApiServiceChannelAddressInTranslator : Translator<ServiceChannelAddress, V9VmOpenApiAddressIn>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceChannelAddressInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V9VmOpenApiAddressIn TranslateEntityToVm(ServiceChannelAddress entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceChannelAddressTranslator");
        }

        public override ServiceChannelAddress TranslateVmToEntity(V9VmOpenApiAddressIn vModel)
        {
            var exists = vModel.Id.IsAssigned();
            var characterId = typesCache.Get<AddressCharacter>(vModel.Type);
            return CreateViewModelEntityDefinition<ServiceChannelAddress>(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextUpdate(input => exists, input => output => input.Id == output.AddressId && input.OwnerReferenceId == output.ServiceChannelVersionedId && characterId == output.CharacterId, def => def.UseDataContextCreate(input => true))
                .AddSimple(i => typesCache.Get<AddressCharacter>(i.Type), o => o.CharacterId)
                .AddNavigation(i => i, o => o.Address)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceChannelAddress, V8VmOpenApiAddressDeliveryIn>), RegisterType.Transient)]
    internal class OpenApiDeliveryAddressInTranslator : Translator<ServiceChannelAddress, V8VmOpenApiAddressDeliveryIn>
    {
        private readonly ITypesCache typesCache;

        public OpenApiDeliveryAddressInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V8VmOpenApiAddressDeliveryIn TranslateEntityToVm(ServiceChannelAddress entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceChannelAddressTranslator");
        }

        public override ServiceChannelAddress TranslateVmToEntity(V8VmOpenApiAddressDeliveryIn vModel)
        {
            return CreateViewModelEntityDefinition<ServiceChannelAddress>(vModel)
                .UseDataContextCreate(input => true)
                .AddSimple(i => typesCache.Get<AddressCharacter>(AddressCharacterEnum.Delivery.ToString()), o => o.CharacterId)
                .AddNavigation(i => i, o => o.Address)
                .GetFinal();
        }
    }
}
