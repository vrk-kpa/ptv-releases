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
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Domain.Model.Enums;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<Address, V9VmOpenApiAddress>), RegisterType.Transient)]
    internal class OpenApiAddressTranslator : OpenApiAddressBaseTranslator<V9VmOpenApiAddress>
    {
        private readonly ITypesCache typesCache;

        public OpenApiAddressTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache)
            : base(resolveManager, translationPrimitives, typesCache)
        {
            this.typesCache = typesCache;
        }

        public override V9VmOpenApiAddress TranslateEntityToVm(Address entity)
        {
            var definition = CreateBaseEntityVmDefinitions(entity);
            var addressType = typesCache.GetByValue<AddressType>(entity.TypeId);
            if (addressType == AddressTypeEnum.Foreign.ToString())
            {
                if (entity.AddressForeigns?.Count > 0)
                {
                    definition.AddCollection(i => i.AddressForeigns.FirstOrDefault()?.ForeignTextNames, o => o.ForeignAddress);
                }
            }
            else if (addressType == AddressTypeEnum.Other.ToString())
            {
                if (entity.AddressOthers?.Count > 0)
                {
                    definition.AddNavigation(i => i.AddressOthers.FirstOrDefault(), o => o.OtherAddress);
                }
            }

            return definition.GetFinal();
        }

        public override Address TranslateVmToEntity(V9VmOpenApiAddress vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiAddressWithTypeAndCoordinatesTranslator");
        }
    }

    [RegisterService(typeof(ITranslator<Address, V9VmOpenApiAddressLocation>), RegisterType.Transient)]
    internal class OpenApiAddressLocationTranslator : OpenApiAddressBaseTranslator<V9VmOpenApiAddressLocation>
    {
        private readonly ITypesCache typesCache;

        public OpenApiAddressLocationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache)
            : base(resolveManager, translationPrimitives, typesCache)
        {
            this.typesCache = typesCache;
        }

        public override V9VmOpenApiAddressLocation TranslateEntityToVm(Address entity)
        {
            var definition = CreateBaseEntityVmDefinitions(entity, true)
                .AddCollection(i => i.AccessibilityRegisterEntrances, o => o.Entrances);

            var addressType = typesCache.GetByValue<AddressType>(entity.TypeId);
            if (addressType == AddressTypeEnum.Foreign.ToString())
            {
                if (entity.AddressForeigns?.Count > 0)
                {
                    definition.AddCollection(i => i.AddressForeigns.FirstOrDefault()?.ForeignTextNames, o => o.LocationAbroad);
                }
            }
            else if (addressType == AddressTypeEnum.Other.ToString())
            {
                if (entity.AddressOthers?.Count > 0)
                {
                    definition.AddNavigation(i => i.AddressOthers.FirstOrDefault(), o => o.OtherAddress);
                }
            }

            return definition.GetFinal();
        }

        public override Address TranslateVmToEntity(V9VmOpenApiAddressLocation vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiAddressWithTypeAndCoordinatesTranslator");
        }
    }
}
