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
using System.Collections.Generic;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Linq;
using System.Globalization;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<ClsAddressPoint, VmOpenApiAddressStreet>), RegisterType.Transient)]
    internal class OpenApiClsPointTranslator : OpenApiStreetBaseTranslator<VmOpenApiAddressStreet>
    {

        public OpenApiClsPointTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiAddressStreet TranslateEntityToVm(ClsAddressPoint entity)
        {
            return base.TranslateEntityToVm(entity);
        }

        public override ClsAddressPoint TranslateVmToEntity(VmOpenApiAddressStreet vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<ClsAddressPoint, VmOpenApiAddressStreetWithOrder>), RegisterType.Transient)]
    internal class OpenApiStreetWithOrderTranslator : OpenApiStreetWithCoordinatesBaseTranslator<VmOpenApiAddressStreetWithOrder>
    {

        public OpenApiStreetWithOrderTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache)
            : base(resolveManager, translationPrimitives, typesCache)
        {
        }

        public override VmOpenApiAddressStreetWithOrder TranslateEntityToVm(ClsAddressPoint entity)
        {
            return base.TranslateEntityToVm(entity);
        }

        public override ClsAddressPoint TranslateVmToEntity(VmOpenApiAddressStreetWithOrder vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<ClsAddressPoint, VmOpenApiAddressStreetWithCoordinates>), RegisterType.Transient)]
    internal class OpenApiStreetWithCoordinatesTranslator : OpenApiStreetWithCoordinatesBaseTranslator<VmOpenApiAddressStreetWithCoordinates>
    {
        public OpenApiStreetWithCoordinatesTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache)
            : base(resolveManager, translationPrimitives, typesCache)
        {
        }

        public override VmOpenApiAddressStreetWithCoordinates TranslateEntityToVm(ClsAddressPoint entity)
        {
            return base.TranslateEntityToVm(entity);
        }

        public override ClsAddressPoint TranslateVmToEntity(VmOpenApiAddressStreetWithCoordinates vModel)
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

        public override TVmOpenApiStreetWithCoordinates TranslateEntityToVm(ClsAddressPoint entity)
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

    internal abstract class OpenApiStreetBaseTranslator<TVmOpenApiStreet>
        : Translator<ClsAddressPoint, TVmOpenApiStreet>
        where TVmOpenApiStreet : class, IVmOpenApiAddressStreet
    {
        public OpenApiStreetBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override TVmOpenApiStreet TranslateEntityToVm(ClsAddressPoint entity)
        {
            if (entity == null) return null;

            return CreateBaseDefinitions(entity)
               .GetFinal();
        }

        public override ClsAddressPoint TranslateVmToEntity(TVmOpenApiStreet vModel)
        {
            throw new NotImplementedException();
        }

        protected ITranslationDefinitions<ClsAddressPoint, TVmOpenApiStreet> CreateBaseDefinitions(ClsAddressPoint entity)
        {
            if (entity == null) return null;

            var postalCode = entity.PostalCode;
            bool codeExists = postalCode != null && postalCode.Code.ToLower() != "undefined";
            var streetNames = FillAllStreetNames(entity.AddressStreet?.StreetNames);

            return CreateEntityViewModelDefinition(entity)
                .AddCollection(i => streetNames, o => o.Street)
                .AddNavigation(i => i.StreetNumber, o => o.StreetNumber)
                .AddNavigation(i => codeExists ? postalCode.Code : null, o => o.PostalCode)
                .AddCollection(i => codeExists ? postalCode.PostalCodeNames : null, o => o.PostOffice)
                .AddNavigation(i => i.Municipality, o => o.Municipality)
                .AddCollection(i => i.Address.AddressAdditionalInformations, o => o.AdditionalInformation);
        }

        private ICollection<ClsAddressStreetName> FillAllStreetNames(ICollection<ClsAddressStreetName> streetNames)
        {
            List<ClsAddressStreetName> result = null;
            if (streetNames != null && streetNames.Any())
            {
                result = new List<ClsAddressStreetName>();
                var defName = streetNames.FirstOrDefault(x => x.LocalizationId == languageCache.Get("fi")) ??
                              streetNames.First();
                languageCache.AllowedLanguageCodes.ForEach(langCode =>
                {
                    var missingName = new ClsAddressStreetName { Name = defName.Name, LocalizationId = languageCache.Get(langCode), ClsAddressStreetId = defName.ClsAddressStreetId};
                    result.Add(streetNames.FirstOrDefault(x => x.LocalizationId == languageCache.Get(langCode)) ?? missingName);
                });
            }
            return result;
        }
    }
}
