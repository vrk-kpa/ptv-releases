/**
 * The MIT License
 * Copyright(c) 2016 Population Register Centre(VRK)
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
using PTV.Domain.Model.Models.Interfaces.OpenApi.V7;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    internal abstract class OpenApiAddressBaseTranslator<TModel> : Translator<Address, TModel> where TModel : class, IV7VmOpenApiAddress
    {
        private readonly ITypesCache typesCache;

        public OpenApiAddressBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override TModel TranslateEntityToVm(Address entity)
        {
            return CreateBaseEntityVmDefinitions(entity).GetFinal();
        }

        public override Address TranslateVmToEntity(TModel vModel)
        {
            throw new NotImplementedException();
        }

        protected ITranslationDefinitions<Address, TModel> CreateBaseEntityVmDefinitions(Address entity)
        {
            var addressType = typesCache.GetByValue<AddressType>(entity.TypeId);

            var definition = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => addressType, o => o.SubType)
                .AddNavigation(i => i.Country, o => o.Country);

            switch (addressType.Parse<AddressTypeEnum>())
            {
                case AddressTypeEnum.PostOfficeBox:
                    if (entity.AddressPostOfficeBoxes?.Count > 0)
                    {
                        definition.AddNavigation(i => i.AddressPostOfficeBoxes.FirstOrDefault(), o => o.PostOfficeBoxAddress);
                    }                    
                    break;
                case AddressTypeEnum.Street:
                    if (entity.AddressStreets?.Count > 0)
                    {
                        definition.AddNavigation(i => i.AddressStreets.FirstOrDefault(), o => o.StreetAddress);
                    }                    
                    break;
                case AddressTypeEnum.Foreign:
                    if (entity.AddressForeigns?.Count > 0)
                    {
                        definition.AddCollection(i => i.AddressForeigns.FirstOrDefault()?.ForeignTextNames, o => o.ForeignAddress);
                    }                    
                    break;
                default:
                    break;
            }

            return definition;
        }
    }
}
