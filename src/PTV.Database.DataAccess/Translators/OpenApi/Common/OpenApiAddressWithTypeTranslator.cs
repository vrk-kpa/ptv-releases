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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<Address, V5VmOpenApiAddressWithTypeIn>), RegisterType.Transient)]
    internal class OpenApiAddressWithTypeTranslator : Translator<Address, V5VmOpenApiAddressWithTypeIn>
    {
        private readonly ITypesCache typesCache;

        public OpenApiAddressWithTypeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V5VmOpenApiAddressWithTypeIn TranslateEntityToVm(Address entity)
        {

            throw new NotImplementedException("No translation implemented in OpenApiAddressWithTypeTranslator");
        }
        public override Address TranslateVmToEntity(V5VmOpenApiAddressWithTypeIn vModel)
        {
            if (vModel == null) return null;

            var exists = vModel.Id.IsAssigned();

            if (exists)
            {
                vModel.StreetAddress.ForEach(s => s.OwnerReferenceId = vModel.Id);
                vModel.AdditionalInformations.ForEach(a => a.OwnerReferenceId = vModel.Id);
            }

            var definition = CreateViewModelEntityDefinition<Address>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => (vModel.Id.Value == o.Id));

            if (vModel.PostOfficeBox?.Count > 0)
            {
                definition.AddCollection(i => i.PostOfficeBox, o => o.PostOfficeBoxNames);
            }

            if (!string.IsNullOrEmpty(vModel.PostalCode))
            {
                definition.AddNavigation(i => i.PostalCode, o => o.PostalCode);
            }

            if (!string.IsNullOrEmpty(vModel.Municipality))
            {
                definition.AddNavigation(i => i.Municipality, o => o.Municipality);
            }

            if (vModel.StreetAddress?.Count > 0)
            {
                definition.AddCollection(i => i.StreetAddress, o => o.StreetNames);
            }

            if (!string.IsNullOrEmpty(vModel.StreetNumber))
            {
                definition.AddNavigation(i => i.StreetNumber, o => o.StreetNumber);
            }

            if (!exists && string.IsNullOrEmpty(vModel.Country))
            {
                definition.AddNavigation(i => CountryCode.FI.ToString(), o => o.Country);
            }
            else if (!string.IsNullOrEmpty(vModel.Country))
            {
                definition.AddNavigation(i => i.Country, o => o.Country);
            }

            if (vModel.AdditionalInformations?.Count > 0)
            {
                definition.AddCollection(i => i.AdditionalInformations, o => o.AddressAdditionalInformations);
            }

            if (!string.IsNullOrEmpty(vModel.Latitude) && !string.IsNullOrEmpty(vModel.Longitude))
            {
                double latitude, longitude;

                if (double.TryParse(vModel.Latitude, NumberStyles.Number, CultureInfo.InvariantCulture, out latitude) &&
                    double.TryParse(vModel.Longitude, NumberStyles.Number, CultureInfo.InvariantCulture, out longitude))
                {
                    definition.AddCollection(
                    i => new List<VmCoordinate>
                    {
                        new VmCoordinate
                        {
                            OwnerReferenceId = definition.GetFinal().Id,
                            TypeId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.User.ToString()),
                            CoordinateState = CoordinateStates.EnteredByUser.ToString(),
                            Latitude = latitude,
                            Longtitude = longitude
                        }
                    }, o => o.Coordinates);
                }
            }

            return definition.GetFinal();
        }
    }
}
