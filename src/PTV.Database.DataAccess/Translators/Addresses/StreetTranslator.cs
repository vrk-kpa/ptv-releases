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
using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<ClsAddressStreet, VmStreet>), RegisterType.Transient)]
    internal class StreetTranslator : Translator<ClsAddressStreet, VmStreet>
    {
        private readonly TranslatedItemDefinitionHelper definitionHelper;
        private readonly ICacheManager cacheManager;

        public StreetTranslator(
            IResolveManager resolveManager, 
            ITranslationPrimitives translationPrimitives,
            TranslatedItemDefinitionHelper definitionHelper, 
            ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
            this.cacheManager = cacheManager;
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
        }private VmLanguageText CreateName(string language, string value, Guid? id)
        {
            return new VmLanguageText
            {
                Text = value,
                Id = id,
                LocalizationId = cacheManager.LanguageCache.Get(language)
            };
        }

        public override ClsAddressStreet TranslateVmToEntity(VmStreet vModel)
        {
            var entity = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.Id)
                .AddSimple(i => i.MunicipalityId, o => o.MunicipalityId)
                .AddSimple(i => i.IsValid, o => o.IsValid)
                .AddCollection(i => i.Translation.Texts.Select(t => CreateName(t.Key, t.Value, i.Id)),
                    o => o.StreetNames)
                .GetFinal();

            return entity;
        }
    }
}
