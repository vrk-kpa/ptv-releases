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

using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<ClsAddressStreet, VmOpenApiAddressStreetWithCoordinatesIn>), RegisterType.Transient)]
    internal class OpenApiStreetTranslator : Translator<ClsAddressStreet, VmOpenApiAddressStreetWithCoordinatesIn>
    {
        private readonly ILanguageCache languageCache;
        private readonly ILanguageOrderCache languageOrderCache;
        private readonly IPostalCodeCache postalCodeCache;

        public OpenApiStreetTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
            this.languageOrderCache = cacheManager.LanguageOrderCache;
            this.postalCodeCache = cacheManager.PostalCodeCache;
        }

        public override VmOpenApiAddressStreetWithCoordinatesIn TranslateEntityToVm(ClsAddressStreet entity)
        {
            return CreateEntityViewModelDefinition<VmOpenApiAddressStreetWithCoordinatesIn>(entity)
                .AddCollection(i => i.StreetNames, o => o.Street)
                .GetFinal();
        }

        private VmLanguageText CreateName(string language, string value, Guid? id)
        {
            return new VmLanguageText
            {
                Text = value.FirstCharToUpper(),
                Id = id,
                LocalizationId = languageCache.Get(language)
            };
        }

        public override ClsAddressStreet TranslateVmToEntity(VmOpenApiAddressStreetWithCoordinatesIn vModel)
        {
           var definition = CreateViewModelEntityDefinition<ClsAddressStreet>(vModel);
           var searchedStreetNames = vModel.Street
                .OrderBy(s => languageOrderCache.Get(languageCache.Get(s.Language)))
                .Select(s => new
                {
                    LanguageId = languageCache.Get(s.Language),
                    Name = s.Value?.Trim().ToLower(),
                    Name3 = s.Value?.Trim()?.ToLower()?.SafeSubstring(0, 3)
                }).ToList();
            bool found = false;
            foreach (var streetName in searchedStreetNames)
            {
                found = true;
                definition.UseDataContextUpdate(
                    i => !string.IsNullOrEmpty(streetName?.Name) && streetName.LanguageId.IsAssigned(),
                    i => o =>  (o.Municipality.PostalCodes.Select(j => j.Code).Any(code => i.PostalCode == code))
                               && o.StreetNames.Any(osn =>
                                   osn.LocalizationId == streetName.LanguageId
                                   && osn.Name3 == streetName.Name3
                                   && osn.Name.ToLower() == streetName.Name), d => found = false, i => o => o.IsValid, i => o => o.NonCls);
                if (found) break;
            }
            if (!found)
            {
                definition.UseDataContextCreate();
                definition.AddSimple(i => postalCodeCache.MunicipalityIdForCode(i.PostalCode) ?? Guid.Empty, o => o.MunicipalityId);
                definition.AddCollection(i => i.Street.Select(t => CreateName(t.Language, t.Value, i.Id)), o => o.StreetNames);
                definition.AddSimple(i => true, o => o.NonCls);
            }
            definition.AddSimple(i => true, o => o.IsValid);
            return definition.GetFinal();
        }
    }
}
