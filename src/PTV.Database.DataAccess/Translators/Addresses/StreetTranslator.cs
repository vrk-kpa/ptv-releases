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
using Microsoft.EntityFrameworkCore.Internal;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<ClsAddressStreet, VmStreet>), RegisterType.Transient)]
    internal class StreetTranslator : Translator<ClsAddressStreet, VmStreet>
    {
        private readonly TranslatedItemDefinitionHelper definitionHelper;
        private readonly ICacheManager cacheManager;
        private readonly ILanguageOrderCache languageOrderCache;
        private readonly IPostalCodeCache postalCodeCache;

        public StreetTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            TranslatedItemDefinitionHelper definitionHelper,
            ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
            this.cacheManager = cacheManager;
            this.languageOrderCache = cacheManager.LanguageOrderCache;
            this.postalCodeCache = cacheManager.PostalCodeCache;
        }

        public override VmStreet TranslateEntityToVm(ClsAddressStreet entity)
        {
            var finnish = cacheManager.LanguageCache.Get("fi");

            var translation = CreateEntityViewModelDefinition<VmStreet>(entity)
                .AddSimple(i => i.MunicipalityId, o => o.MunicipalityId)
                .AddNavigation(i => i.Municipality?.Code, o => o.MunicipalityCode)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => i.IsValid, o => o.IsValid)
                .AddCollection(i => i.StreetNumbers.Where(sn => sn.IsValid), o => o.StreetNumbers);

            definitionHelper.AddTranslations(translation,
                entity.StreetNames.FirstOrDefault(sn => sn.LocalizationId == finnish)?.Name);
            return translation.GetFinal();
        }

        private VmLanguageText CreateName(string language, string value, Guid? id)
        {
            return new VmLanguageText
            {
                Text = value.FirstCharToUpper(),
                Id = id,
                LocalizationId = cacheManager.LanguageCache.Get(language)
            };
        }

        public override ClsAddressStreet TranslateVmToEntity(VmStreet vModel)
        {
            var possiblePostalCodes = vModel.StreetNumbers.Select(i => i.PostalCode.Id).ToList();
            if (!possiblePostalCodes.Any() && (!vModel.MunicipalityId.IsAssigned())) throw new PtvAppException("PostalCode Id or Municipality Id missing in VmStreet");
            var municipalityId = postalCodeCache.MunicipalityIdForPostalId(possiblePostalCodes.FirstOrDefault());
            var definition = CreateViewModelEntityDefinition<ClsAddressStreet>(vModel);
            var searchedStreetNames = vModel.Translation.Texts
                .OrderBy(s => languageOrderCache.Get(languageCache.Get(s.Key)))
                .Select(s => new
                {
                    LanguageId = languageCache.Get(s.Key),
                    Name = s.Value?.Trim().ToLower(),
                    Name3 = s.Value?.Trim()?.ToLower()?.SafeSubstring(0, 3)
                }).ToList();
                bool found = false;
                foreach (var streetName in searchedStreetNames)
                {
                    found = true;
                    definition.UseDataContextUpdate(
                        i => !string.IsNullOrEmpty(streetName?.Name) && streetName.LanguageId.IsAssigned(),
                        i => o =>  (o.Municipality.PostalCodes.Select(j => j.Id).Any(m => possiblePostalCodes.Contains(m)))
                                  && o.StreetNames.Any(osn =>
                                      osn.LocalizationId == streetName.LanguageId
                                      && osn.Name3 == streetName.Name3
                                      && osn.Name.ToLower() == streetName.Name), d => found = false, i => o => o.IsValid, i => o => o.NonCls);
                    if (found) break;
                }
                if (!found)
                {
                    definition.UseDataContextCreate();
                    definition.AddSimple(i => i.MunicipalityId.IsAssigned() ? i.MunicipalityId : (municipalityId ?? Guid.Empty), o => o.MunicipalityId);
                    //definition.AddNavigation(i => i.MunicipalityCode, o => o.Municipality);
                    definition.AddCollection(i => i.Translation.Texts.Select(t => CreateName(t.Key, t.Value, i.Id)), o => o.StreetNames);
                    definition.AddSimple(i => true, o => o.NonCls);
                }
                definition.AddSimple(i => true, o => o.IsValid);
                return definition.GetFinal();
        }
    }
}
