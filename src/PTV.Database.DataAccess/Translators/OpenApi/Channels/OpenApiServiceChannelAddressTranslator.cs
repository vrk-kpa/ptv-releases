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
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.Extensions;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelAddress, V9VmOpenApiAddressLocation>), RegisterType.Transient)]
    internal class OpenApiServiceChannelAddressTranslator : Translator<ServiceChannelAddress, V9VmOpenApiAddressLocation>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceChannelAddressTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V9VmOpenApiAddressLocation TranslateEntityToVm(ServiceChannelAddress entity)
        {
            var type = typesCache.GetByValue<AddressCharacter>(entity.CharacterId);
            var definition = CreateEntityViewModelDefinition(entity)
                .AddPartial(i => i.Address)
                .AddNavigation(i => type == AddressCharacterEnum.Visiting.ToString() ? AddressConsts.LOCATION : type, o => o.Type); // PTV-2910

            var model = definition.GetFinal();
            // Rename address related properties (PTV-2910):
            // when type = 'Visiting' then 'Street' -> 'Single'
            if (type == AddressCharacterEnum.Visiting.ToString() && model.SubType == AddressTypeEnum.Street.ToString())
            {
                model.SubType = AddressConsts.SINGLE;
            }
            return model;
        }

        public override ServiceChannelAddress TranslateVmToEntity(V9VmOpenApiAddressLocation vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceChannelAddressTranslator");
        }
    }
}
