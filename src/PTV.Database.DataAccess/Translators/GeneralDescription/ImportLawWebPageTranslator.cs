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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
    [RegisterService(typeof(ITranslator<LawWebPage, JsonLanguageLabel>), RegisterType.Transient)]
    internal class ImportLawWebPageTranslator : Translator<LawWebPage, JsonLanguageLabel>
    {
        private ILanguageCache languageCache;
        public ImportLawWebPageTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override JsonLanguageLabel TranslateEntityToVm(LawWebPage entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => languageCache.GetByValue(input.LocalizationId), output => output.Lang)
                .AddNavigation(input => input.WebPage.Url, output => output.Label)
                .GetFinal();

        }

        public override LawWebPage TranslateVmToEntity(JsonLanguageLabel vModel)
        {
            bool isNew = !vModel.OwnerReferenceId.IsAssigned();
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.OwnerReferenceId.IsAssigned())
                .UseDataContextLocalizedUpdate(i => i.OwnerReferenceId.IsAssigned(), i => o => o.LawId == i.OwnerReferenceId && o.LocalizationId == languageCache.Get(i.Lang), def =>
                {
                    isNew = true;
                    def.UseDataContextCreate(i => true);
                });
            var entity = definition.GetFinal();

            definition.AddNavigation(
                i => new VmWebPage {UrlAddress = i.Label, Id = isNew ? (Guid?)null : entity.Id},
                o => o.WebPage);

            entity.LocalizationId = languageCache.Get(vModel.Lang);
            return entity;
        }
    }
}
