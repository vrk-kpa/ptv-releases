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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    internal abstract class OpenApiAddressBaseTranslator<TVmOpenApiAddress> : Translator<Address, TVmOpenApiAddress> where TVmOpenApiAddress : class, IV2VmOpenApiAddress
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
            return CreateVmEntityBaseDefinitions(vModel)
                .GetFinal();
        }

        protected ITranslationDefinitions<Address, TVmOpenApiAddress> CreateBaseDefinitions(Address entity)
        {
            return CreateEntityViewModelDefinition<TVmOpenApiAddress>(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.PostOfficeBox, o => o.PostOfficeBox)
                .AddNavigation(i => i.PostalCode?.Code, o => o.PostalCode)
                .AddNavigation(i => i.PostalCode?.PostOffice, o => o.PostOffice)
                .AddNavigation(i => i.Municipality?.Name, o => o.Municipality)
                .AddCollection(i => i.StreetNames, o => o.StreetAddress)
                .AddNavigation(i => i.StreetNumber, o => o.StreetNumber)
                .AddNavigation(i => i.Country, o => o.Country)
                .AddCollection(i => i.AddressAdditionalInformations, o => o.AdditionalInformations);
            //.AddCollection(i => i.Country?.CountryNames, o => o.Country);
        }

        protected ITranslationDefinitions<TVmOpenApiAddress, Address> CreateVmEntityBaseDefinitions(TVmOpenApiAddress vModel)
        {
            var exists = vModel.Id.IsAssigned();

            if (exists)
            {
                vModel.StreetAddress.ForEach(s => s.OwnerReferenceId = vModel.Id);
            }

            var definition = CreateViewModelEntityDefinition<Address>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => (vModel.Id.Value == o.Id));

            if (!string.IsNullOrEmpty(vModel.PostOfficeBox))
            {
                definition.AddNavigation(i => i.PostOfficeBox, o => o.PostOfficeBox);
            }

            if (!string.IsNullOrEmpty(vModel.PostalCode))
            {
                definition.AddNavigation(i => i.PostalCode, o => o.PostalCode);
            }

            if (!string.IsNullOrEmpty(vModel.Municipality))
            {
                definition.AddNavigation(i => i.Municipality, o => o.Municipality);
            }

            if (vModel.StreetAddress != null && vModel.StreetAddress.Count > 0)
            {
                definition.AddCollection(i => i.StreetAddress, o => o.StreetNames);
            }

            if (!string.IsNullOrEmpty(vModel.StreetNumber))
            {
                definition.AddNavigation(i => i.StreetNumber, o => o.StreetNumber);
            }

            if (!string.IsNullOrEmpty(vModel.Country))
            {
                definition.AddNavigation(i => i.Country, o => o.Country);
            }

            if (vModel.AdditionalInformations?.Count > 0)
            {
                definition.AddCollection(i => i.AdditionalInformations, o => o.AddressAdditionalInformations);
            }
//            if (!string.IsNullOrEmpty(vModel.PublishingStatus))
//            {
//                definition.AddNavigation(i => i.PublishingStatus, o => o.PublishingStatus);
//            }

            return definition;
        }
    }
}
