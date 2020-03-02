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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<Keyword, VmKeywordItem>), RegisterType.Transient)]
    internal class KeywordTranslator : Translator<Keyword, VmKeywordItem>
    {
        private ILanguageCache languageCache;
        public KeywordTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
        }

        public override VmKeywordItem TranslateEntityToVm(Keyword entity)
        {
            return CreateEntityViewModelDefinition<VmKeywordItem>(entity)
                .AddNavigation(i => i.Name, o => o.Name)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.LocalizationCode)
                .GetFinal();
        }

        public override Keyword TranslateVmToEntity(VmKeywordItem vModel)
        {
            SetLanguage(vModel.LocalizationId);

            if (string.IsNullOrEmpty(vModel?.Name))
            {
                return null;
            }
            bool exists = vModel.Id.IsAssigned();

            return CreateViewModelEntityDefinition<Keyword>(vModel)
               .UseDataContextCreate(input => !exists, output => output.Id, input => Guid.NewGuid())
               .UseDataContextLocalizedUpdate(input => exists, input => output => input.Id == output.Id)
                .AddNavigation(input => input.Name, output => output.Name)
                .AddRequestLanguage(o=>o)
                .GetFinal();
        }
    }
}
