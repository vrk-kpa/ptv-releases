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
using PTV.Domain.Model.Enums;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi.V8;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<Address, V8VmOpenApiAddressDelivery>), RegisterType.Transient)]
    internal class OpenApiAddressDeliveryTranslator : Translator<Address, V8VmOpenApiAddressDelivery>
    {
        private readonly ITypesCache typesCache;

        public OpenApiAddressDeliveryTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override V8VmOpenApiAddressDelivery TranslateEntityToVm(Address entity)
        {
            var addressType = typesCache.GetByValue<AddressType>(entity.TypeId);

            var definition = CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => addressType, o => o.SubType);

            switch (addressType.Parse<AddressTypeEnum>())
            {
                case AddressTypeEnum.PostOfficeBox:
                    definition.AddNavigation(i => i.AddressPostOfficeBoxes.FirstOrDefault(), o => o.PostOfficeBoxAddress);
                    break;
                case AddressTypeEnum.Street:
                    definition.AddNavigation(i => i.ClsAddressPoints.FirstOrDefault(), o => o.StreetAddress);
                    break;
                case AddressTypeEnum.NoAddress:
                    definition.AddCollection(i => i.AddressAdditionalInformations, o => o.DeliveryAddressInText);
                    break;
                default:
                    break;
            }
            
            if (!entity.Receivers.IsNullOrEmpty())
            {
                definition.AddCollection(i => i.Receivers, o => o.Receiver);
            }

            return definition.GetFinal();
        }

        public override Address TranslateVmToEntity(V8VmOpenApiAddressDelivery vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiAddressWithCoordinatesTranslator");
        }
    }
}
