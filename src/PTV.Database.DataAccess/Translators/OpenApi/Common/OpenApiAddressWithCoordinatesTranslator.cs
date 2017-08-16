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
using System.Globalization;
using System.Linq;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi.V5;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<Address, V5VmOpenApiAddressWithCoordinates>), RegisterType.Transient)]
    internal class OpenApiAddressWithCoordinatesTranslator : OpenApiAddressBaseTranslator<V5VmOpenApiAddressWithCoordinates>
    {
        private readonly ITypesCache typesCache;

        public OpenApiAddressWithCoordinatesTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives, typesCache)
        {
            this.typesCache = typesCache;
        }

        public override V5VmOpenApiAddressWithCoordinates TranslateEntityToVm(Address entity)
        {
            var definition = CreateBaseDefinitions(entity);

            // Select user defined coordinates or default to service provided ones.
            var preferredCoordinate = entity.Coordinates.FirstOrDefault(c => typesCache.GetByValue<CoordinateType>(c.TypeId) == CoordinateTypeEnum.User.ToString())
                ?? entity.Coordinates.FirstOrDefault(c => typesCache.GetByValue<CoordinateType>(c.TypeId) == CoordinateTypeEnum.Main.ToString());

            if (preferredCoordinate != null)
            {
                definition.AddNavigation(i => preferredCoordinate.Latitude.ToString(CultureInfo.InvariantCulture), o => o.Latitude);
                definition.AddNavigation(i => preferredCoordinate.Longtitude.ToString(CultureInfo.InvariantCulture), o => o.Longitude);
                definition.AddNavigation(i => preferredCoordinate.CoordinateState, o => o.CoordinateState);
            }

            return definition.GetFinal();
        }

        public override Address TranslateVmToEntity(V5VmOpenApiAddressWithCoordinates vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiAddressWithCoordinatesTranslator");
        }
    }
}
