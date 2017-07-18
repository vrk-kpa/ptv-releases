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

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    internal abstract class OpenApiAddressBaseTranslator<TVmOpenApiAddress> : Translator<Address, TVmOpenApiAddress> where TVmOpenApiAddress : class, IVmOpenApiAddressVersionBase
    {
        protected OpenApiAddressBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
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

        protected ITranslationDefinitions<Address, TVmOpenApiAddress> CreateBaseDefinitions(Address entity)
        {
            bool codeExists = entity.PostalCode != null && entity.PostalCode.Code.ToLower() != "undefined";
            return CreateEntityViewModelDefinition<TVmOpenApiAddress>(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddCollection(i => i.PostOfficeBoxNames, o => o.PostOfficeBox)
                .AddNavigation(i => codeExists ? i.PostalCode.Code : null, o => o.PostalCode)
                .AddCollection(i => codeExists ? i.PostalCode.PostalCodeNames : null, o => o.PostOffice)

                .AddNavigation(i => i.Municipality, o => o.Municipality)
                .AddCollection(i => i.StreetNames, o => o.StreetAddress)
                .AddNavigation(i => i.StreetNumber, o => o.StreetNumber)
                .AddNavigation(i => i.Country, o => o.Country)
                .AddCollection(i => i.AddressAdditionalInformations, o => o.AdditionalInformations);
        }
    }
}
