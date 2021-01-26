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
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<AddressPostOfficeBox, VmOpenApiAddressPostOfficeBox>), RegisterType.Transient)]
    internal class OpenApiPostOfficeBoxTranslator : Translator<AddressPostOfficeBox, VmOpenApiAddressPostOfficeBox>
    {
        public OpenApiPostOfficeBoxTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiAddressPostOfficeBox TranslateEntityToVm(AddressPostOfficeBox entity)
        {
            if (entity == null) return null;

            var postalCode = entity.PostalCode;
            bool codeExists = postalCode != null && postalCode.Code.ToLower() != "undefined";
            return CreateEntityViewModelDefinition(entity)
                .AddCollection(i => i.PostOfficeBoxNames, o => o.PostOfficeBox)
                .AddNavigation(i => codeExists ? postalCode.Code : null, o => o.PostalCode)
                .AddCollection(i => codeExists ? postalCode.PostalCodeNames : null, o => o.PostOffice)
                .AddNavigation(i => i.Municipality, o => o.Municipality)
                .AddCollection(i => i.Address.AddressAdditionalInformations, o => o.AdditionalInformation)
                .GetFinal();
        }

        public override AddressPostOfficeBox TranslateVmToEntity(VmOpenApiAddressPostOfficeBox vModel)
        {
            throw new NotImplementedException();
        }
    }
}
