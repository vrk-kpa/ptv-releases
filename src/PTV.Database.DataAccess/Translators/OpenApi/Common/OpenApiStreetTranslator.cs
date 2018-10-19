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
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Linq;
using System.Globalization;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<AddressStreet, VmOpenApiAddressStreet>), RegisterType.Transient)]
    internal class OpenApiStreetTranslator : OpenApiStreetBaseTranslator<VmOpenApiAddressStreet>
    {

        public OpenApiStreetTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiAddressStreet TranslateEntityToVm(AddressStreet entity)
        {
            return base.TranslateEntityToVm(entity);
        }

        public override AddressStreet TranslateVmToEntity(VmOpenApiAddressStreet vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<AddressStreet, VmOpenApiAddressStreetWithOrder>), RegisterType.Transient)]
    internal class OpenApiStreetWithOrderTranslator : OpenApiStreetWithCoordinatesBaseTranslator<VmOpenApiAddressStreetWithOrder>
    {

        public OpenApiStreetWithOrderTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache)
            : base(resolveManager, translationPrimitives, typesCache)
        {
        }

        public override VmOpenApiAddressStreetWithOrder TranslateEntityToVm(AddressStreet entity)
        {
            return base.TranslateEntityToVm(entity);
        }

        public override AddressStreet TranslateVmToEntity(VmOpenApiAddressStreetWithOrder vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<AddressStreet, VmOpenApiAddressStreetWithCoordinates>), RegisterType.Transient)]
    internal class OpenApiStreetWithCoordinatesTranslator : OpenApiStreetWithCoordinatesBaseTranslator<VmOpenApiAddressStreetWithCoordinates>
    {
        private readonly ITypesCache typesCache;

        public OpenApiStreetWithCoordinatesTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache)
            : base(resolveManager, translationPrimitives, typesCache)
        {
            this.typesCache = typesCache;
        }

        public override VmOpenApiAddressStreetWithCoordinates TranslateEntityToVm(AddressStreet entity)
        {
            return base.TranslateEntityToVm(entity);
        }

        public override AddressStreet TranslateVmToEntity(VmOpenApiAddressStreetWithCoordinates vModel)
        {
            throw new NotImplementedException();
        }
    }
    internal abstract class OpenApiStreetWithCoordinatesBaseTranslator<TVmOpenApiStreetWithCoordinates> : OpenApiStreetBaseTranslator<TVmOpenApiStreetWithCoordinates>
        where TVmOpenApiStreetWithCoordinates : class, IVmOpenApiAddressStreetWithCoordinates
    {
        private readonly ITypesCache typesCache;

        public OpenApiStreetWithCoordinatesBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache)
            : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override TVmOpenApiStreetWithCoordinates TranslateEntityToVm(AddressStreet entity)
        {
            if (entity == null) return null;

            var definition = CreateBaseDefinitions(entity);

            // Select user defined coordinates or default to service provided ones.
            var preferredCoordinate = entity.Address.Coordinates.FirstOrDefault(c => typesCache.GetByValue<CoordinateType>(c.TypeId) == CoordinateTypeEnum.User.ToString())
                ?? entity.Address.Coordinates.FirstOrDefault(c => typesCache.GetByValue<CoordinateType>(c.TypeId) == CoordinateTypeEnum.Main.ToString());

            if (preferredCoordinate != null)
            {
                definition.AddNavigation(i => preferredCoordinate.Latitude.ToString(CultureInfo.InvariantCulture), o => o.Latitude);
                definition.AddNavigation(i => preferredCoordinate.Longitude.ToString(CultureInfo.InvariantCulture), o => o.Longitude);
                definition.AddNavigation(i => preferredCoordinate.CoordinateState, o => o.CoordinateState);
            }
            return definition.GetFinal();
        }
    }

    internal abstract class OpenApiStreetBaseTranslator<TVmOpenApiStreet> : Translator<AddressStreet, TVmOpenApiStreet> where TVmOpenApiStreet : class, IVmOpenApiAddressStreet
    {
        public OpenApiStreetBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override TVmOpenApiStreet TranslateEntityToVm(AddressStreet entity)
        {
            if (entity == null) return null;

            return CreateBaseDefinitions(entity)
               .GetFinal();
        }

        public override AddressStreet TranslateVmToEntity(TVmOpenApiStreet vModel)
        {
            throw new NotImplementedException();
        }

        protected ITranslationDefinitions<AddressStreet, TVmOpenApiStreet> CreateBaseDefinitions(AddressStreet entity)
        {
            if (entity == null) return null;

            var postalCode = entity.PostalCode;
            bool codeExists = postalCode != null && postalCode.Code.ToLower() != "undefined";
            return CreateEntityViewModelDefinition(entity)
                .AddCollection(i => i.StreetNames, o => o.Street)
                .AddNavigation(i => i.StreetNumber, o => o.StreetNumber)
                .AddNavigation(i => codeExists ? postalCode.Code : null, o => o.PostalCode)
                .AddCollection(i => codeExists ? postalCode.PostalCodeNames : null, o => o.PostOffice)
                .AddNavigation(i => i.Municipality, o => o.Municipality)
                .AddCollection(i => i.Address.AddressAdditionalInformations, o => o.AdditionalInformation);
        }
    }
}
