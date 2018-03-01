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
using System.Linq.Expressions;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Finto.Base
{
    internal abstract class FintoItemNameTranslator<TFintoItemName> : Translator<TFintoItemName, JsonLanguageLabel> where TFintoItemName : NameBase
    {
        private ILanguageCache languageCache;
        public FintoItemNameTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageCache languageCache) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = languageCache;
        }

        public override JsonLanguageLabel TranslateEntityToVm(TFintoItemName entity)
        {
            throw new NotImplementedException();
        }

        public override TFintoItemName TranslateVmToEntity(JsonLanguageLabel vModel)
        {
            Guid languageId = languageCache.Get(vModel.Lang);
            var entity = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.OwnerReferenceId.IsAssigned())
                .UseDataContextUpdate(i => i.OwnerReferenceId.IsAssigned(), input => GetUpdateCondition(input.OwnerReferenceId, languageId), def => def.UseDataContextCreate(i => true))
                .AddNavigation(i => i.Label, o => o.Name)
                .AddSimple(i => languageId, o => o.LocalizationId)
                .GetFinal();
            return entity;
        }

        protected abstract Expression<Func<TFintoItemName, bool>> GetUpdateCondition(Guid? guid, Guid languageId);
    }

    internal abstract class FintoItemDescriptionTranslator<TFintoItemDescription> : Translator<TFintoItemDescription, JsonLanguageLabel> where TFintoItemDescription : DescriptionBase
    {
        private ILanguageCache languageCache;
        public FintoItemDescriptionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageCache languageCache) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = languageCache;
        }

        public override JsonLanguageLabel TranslateEntityToVm(TFintoItemDescription entity)
        {
            throw new NotImplementedException();
        }

        public override TFintoItemDescription TranslateVmToEntity(JsonLanguageLabel vModel)
        {
            Guid languageId = languageCache.Get(vModel.Lang);
            var entity = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.OwnerReferenceId.IsAssigned())
                .UseDataContextUpdate(i => i.OwnerReferenceId.IsAssigned(), input => GetUpdateCondition(input.OwnerReferenceId, languageId), def => def.UseDataContextCreate(i => true))
                .AddNavigation(i => i.Label, o => o.Description)
                .AddSimple(i => languageId, o => o.LocalizationId)
                .GetFinal();
            return entity;
        }

        protected abstract Expression<Func<TFintoItemDescription, bool>> GetUpdateCondition(Guid? guid, Guid languageId);
    }
}
