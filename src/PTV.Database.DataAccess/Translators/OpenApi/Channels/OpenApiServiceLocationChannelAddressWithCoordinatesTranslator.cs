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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using PTV.Domain.Model.Models.OpenApi.V5;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceLocationChannelAddress, V5VmOpenApiAddressWithTypeAndCoordinates>), RegisterType.Transient)]
    internal class OpenApiServiceLocationChannelAddressWithCoordinatesTranslator : Translator<ServiceLocationChannelAddress, V5VmOpenApiAddressWithTypeAndCoordinates>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        public OpenApiServiceLocationChannelAddressWithCoordinatesTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override V5VmOpenApiAddressWithTypeAndCoordinates TranslateEntityToVm(ServiceLocationChannelAddress entity)
        {

            var definition = CreateEntityViewModelDefinition<V5VmOpenApiAddressWithTypeAndCoordinates>(entity)
                .AddPartial(i => i.Address)
                .AddNavigation(i => typesCache.GetByValue<AddressCharacter>(i.CharacterId), o => o.Type);

            return definition.GetFinal();
        }

        public override ServiceLocationChannelAddress TranslateVmToEntity(V5VmOpenApiAddressWithTypeAndCoordinates vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceLocationChannelAddressWithCoordinatesTranslator");
        }
    }
}
