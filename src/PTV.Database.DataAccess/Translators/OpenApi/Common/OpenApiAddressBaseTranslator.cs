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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Interfaces;
using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    internal abstract class OpenApiAddressBaseTranslator<TVmOpenApiAddress> : Translator<Address, TVmOpenApiAddress> where TVmOpenApiAddress : class, IVmOpenApiAddressVersionBase
    {
        private readonly ITypesCache typesCache;

        protected OpenApiAddressBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override TVmOpenApiAddress TranslateEntityToVm(Address entity)
        {
            return CreateBaseDefinitions(entity)
               .GetFinal();
        }
        public override Address TranslateVmToEntity(TVmOpenApiAddress vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiAddressBaseTranslator");
        }

        private AddressTypeEnum GetAddressType(Address address)
        {
            if (address.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString())) return AddressTypeEnum.Street;
            if (address.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.PostOfficeBox.ToString())) return AddressTypeEnum.PostOfficeBox;
            if (address.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.Foreign.ToString())) return AddressTypeEnum.Foreign;
            return AddressTypeEnum.Foreign;
        }

        protected ITranslationDefinitions<Address, TVmOpenApiAddress> CreateBaseDefinitions(Address entity)
        {
            var addressType = GetAddressType(entity);
            var postalCode = GetAddressPostalCode(entity);
            bool codeExists = postalCode != null && postalCode.Code.ToLower() != "undefined";
            var definition = CreateEntityViewModelDefinition<TVmOpenApiAddress>(entity)
                .AddSimple(i => i.Id, o => o.Id);

            switch (addressType)
                {
                    case AddressTypeEnum.PostOfficeBox:
                        definition.AddCollection(i => i.AddressPostOfficeBoxes.FirstOrDefault()?.PostOfficeBoxNames, o => o.PostOfficeBox);
                        break;
                    case AddressTypeEnum.Street:
                        definition.AddCollection(i => i.AddressStreets.FirstOrDefault()?.StreetNames, o => o.StreetAddress);
                        break;
                    case AddressTypeEnum.Foreign:
                        break;
                    default:
                        break;
                }
            
            return definition
            .AddNavigation(i => codeExists ? postalCode.Code : null, o => o.PostalCode)
                .AddCollection(i => codeExists ? postalCode.PostalCodeNames : null, o => o.PostOffice)
                .AddNavigation(i => GetAddressMunicipality(i), o => o.Municipality)
                .AddNavigation(i => GetAddressStreetNumber(i), o => o.StreetNumber)
                .AddNavigation(i => i.Country, o => o.Country)
                .AddCollection(i => i.AddressAdditionalInformations, o => o.AdditionalInformations);
        }

        public PostalCode GetAddressPostalCode(Address entity)
        {
            if (entity.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString()))
            {
                return entity.AddressStreets.FirstOrDefault()?.PostalCode;
            }
            if (entity.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.PostOfficeBox.ToString()))
            {
                return entity.AddressPostOfficeBoxes.FirstOrDefault()?.PostalCode;
            }
            return null;
        }

        public string GetAddressStreetNumber(Address entity)
        {
            if (entity.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString()))
            {
                return entity.AddressStreets.FirstOrDefault()?.StreetNumber;
            }
            return null;
        }

        public Municipality GetAddressMunicipality(Address entity)
        {
            if (entity.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString()))
            {
                return entity.AddressStreets.FirstOrDefault()?.Municipality;
            }
            if (entity.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.PostOfficeBox.ToString()))
            {
                return entity.AddressPostOfficeBoxes.FirstOrDefault()?.Municipality;
            }
            return null;
        }
    }
}
