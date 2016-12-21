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
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Globalization;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceLocationChannelAddress, V2VmOpenApiAddressWithTypeAndCoordinates>), RegisterType.Transient)]
    internal class OpenApiServiceLocationChannelAddressWithCoordinatesTranslator : Translator<ServiceLocationChannelAddress, V2VmOpenApiAddressWithTypeAndCoordinates>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceLocationChannelAddressWithCoordinatesTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V2VmOpenApiAddressWithTypeAndCoordinates TranslateEntityToVm(ServiceLocationChannelAddress entity)
        {
            return CreateEntityViewModelDefinition<V2VmOpenApiAddressWithTypeAndCoordinates>(entity)
                .AddNavigation(i => typesCache.GetByValue<AddressType>(i.TypeId), o => o.Type)
                .AddNavigation(input => input.Address.PostOfficeBox, o => o.PostOfficeBox)
                .AddNavigation(i => i.Address.PostalCode?.Code, o => o.PostalCode)
                .AddNavigation(i => i.Address.PostalCode?.PostOffice, o => o.PostOffice)
                .AddNavigation(i => i.Address.Municipality?.Name, o => o.Municipality)
                .AddCollection(i => i.Address.StreetNames, o => o.StreetAddress)
                .AddNavigation(i => i.Address.StreetNumber, o => o.StreetNumber)
                .AddNavigation(i => i.Address.Country, o => o.Country)
                .AddCollection(i => i.Address.AddressAdditionalInformations, o => o.AdditionalInformations)
                .AddNavigation(i => i.Address.Latitude?.ToString(CultureInfo.InvariantCulture), o => o.Latitude)
                .AddNavigation(i => i.Address.Longtitude?.ToString(CultureInfo.InvariantCulture), o => o.Longitude)
                .AddNavigation(i => i.Address.CoordinateState, i => i.CoordinateState)
                .GetFinal();
        }

        public override ServiceLocationChannelAddress TranslateVmToEntity(V2VmOpenApiAddressWithTypeAndCoordinates vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceLocationChannelAddressWithCoordinatesTranslator");
        }
    }
}
