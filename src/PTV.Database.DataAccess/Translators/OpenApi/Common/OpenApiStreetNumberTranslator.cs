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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Translators.Addresses;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<ClsAddressStreetNumber, VmOpenApiAddressStreetWithCoordinatesIn>), RegisterType.Transient)]
    internal class OpenApiStreetNumberTranslator : Translator<ClsAddressStreetNumber, VmOpenApiAddressStreetWithCoordinatesIn>
    {
        private readonly ILanguageOrderCache languageOrderCache;

        public OpenApiStreetNumberTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            this.languageOrderCache = cacheManager.LanguageOrderCache;
        }

        public override VmOpenApiAddressStreetWithCoordinatesIn TranslateEntityToVm(ClsAddressStreetNumber entity)
        {
            throw new System.NotImplementedException();
        }

        public override ClsAddressStreetNumber TranslateVmToEntity(VmOpenApiAddressStreetWithCoordinatesIn vModel)
        {
            try
            {
                var parsedStreetNumber = StreetService.ParseStreetNumber(vModel.StreetNumber) ?? -1;
                var definition = CreateViewModelEntityDefinition<ClsAddressStreetNumber>(vModel)
                    .UseDataContextUpdate(
                        i => true,
                        i => o => (i.ReferencedStreetId == o.ClsAddressStreetId) &&
                                  (parsedStreetNumber >= o.StartNumber && (parsedStreetNumber <= o.EndNumber || parsedStreetNumber <= o.EndNumberEnd || o.EndNumber == 0)) &&
                                  (o.PostalCode.Code == i.PostalCode),
                        def =>
                        {
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
                                def.UseDataContextUpdate(
                                    i => true,
                                    i => o => o.ClsAddressStreet.StreetNames.Any(j =>
                                                  j.LocalizationId == streetName.LanguageId && j.Name3 == streetName.Name3 &&
                                                  j.Name.ToLower().Contains(streetName.Name)) && (o.PostalCode.Code == i.PostalCode) &&
                                              (parsedStreetNumber >= o.StartNumber &&
                                               (parsedStreetNumber <= o.EndNumber || parsedStreetNumber <= o.EndNumberEnd || o.EndNumber == 0) ), d => found = false, i => o => o.IsValid, i => o => o.NonCls);
                                if (found) break;
                            }
                            if (!found)
                            {
                                CreateIfNotFound(vModel, def);
                            }
                        },i => o => o.IsValid, i => o => o.NonCls);
                return definition.GetFinal();
            }
            // Return null in case entity was not found, since street number range is optional
            catch (DbEntityNotFoundException)
            {
                return null;
            }
        }

            private void CreateIfNotFound(VmOpenApiAddressStreetWithCoordinatesIn vModel, ITranslationDefinitionsForVersioning<VmOpenApiAddressStreetWithCoordinatesIn, ClsAddressStreetNumber> def)
            {
                var parsedNumber = StreetService.ParseStreetNumber(vModel.StreetNumber) ?? -1;
                def.UseDataContextCreate(i => true);

                def.AddSimple(i => false, o => o.IsValid)
                    .AddSimple(i => i.ReferencedStreetId.IsAssigned() ? i.ReferencedStreetId : Guid.Empty, o => o.ClsAddressStreetId)
                    .AddSimple(i => parsedNumber % 2 == 0, o => o.IsEven)
                    .AddSimple(i => true, o => o.NonCls)
                    .AddSimple(i => parsedNumber, o => o.EndNumber)
                    .AddSimple(i => parsedNumber, o => o.StartNumber)
                    .AddNavigation(i => new VmPostalCode { Code  = i.PostalCode }, o => o.PostalCode)
                    .AddCollection(i => new List<VmCoordinate>(), o => o.Coordinates);
            }
    }
}
