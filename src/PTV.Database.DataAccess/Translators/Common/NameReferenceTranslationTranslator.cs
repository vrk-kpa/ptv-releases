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
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Common
{
    [RegisterService(typeof(ITranslator<INameReferences, IVmTranslationItem>), RegisterType.Transient)]
//    [RegisterService(typeof(ITranslator<INameReference, VmTranslation>), RegisterType.Transient)]
    internal class NameReferenceTranslationTranslator : Translator<INameReferences, IVmTranslationItem>
    {
        private readonly ILanguageCache languageCache;
        private readonly ILanguageOrderCache languageOrderCache;

        public NameReferenceTranslationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            languageOrderCache = cacheManager.LanguageOrderCache;
        }

        public override IVmTranslationItem TranslateEntityToVm(INameReferences entity)
        {
            var model = CreateEntityViewModelDefinition<VmTranslationItem>(entity)
                .AddSimple(input => input.Id, output => output.Id)
                .AddNavigation(i => i.Names.OrderBy(x => languageOrderCache.Get(x.LocalizationId)).FirstOrDefault(), o => o.DefaultText)
                .GetFinal();

            model.Texts = entity.Names.GroupBy(x => x.LocalizationId).ToDictionary(n => languageCache.GetByValue(n.Key), n => n.Select(x => x.Name).FirstOrDefault());
            return model;
        }

        public override INameReferences TranslateVmToEntity(IVmTranslationItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
