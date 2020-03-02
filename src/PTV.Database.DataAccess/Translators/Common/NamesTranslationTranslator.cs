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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Common
{
    [RegisterService(typeof(ITranslator<INameReferences, IVmTranslatedItem>), RegisterType.Transient)]
    internal class NamesTranslationTranslator : Translator<INameReferences, IVmTranslatedItem>
    {
        private ILanguageOrderCache orderCache;
        public NamesTranslationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageOrderCache orderCache) : base(resolveManager, translationPrimitives)
        {
            this.orderCache = orderCache;
        }

        public override IVmTranslatedItem TranslateEntityToVm(INameReferences entity)
        {
            var model = CreateEntityViewModelDefinition(entity)
                .AddLocalizable(i => i.Names, o => o.DefaultTranslation)
                .AddNavigation(i => i, o => o.Translation)
                .GetFinal();
            var nameReference = entity as INameReferences;
            if (nameReference != null && string.IsNullOrEmpty(model.DefaultTranslation))
            {
                model.DefaultTranslation =
                    nameReference.Names
                        .OrderBy(x => orderCache.Get(x.LocalizationId))
                        .FirstOrDefault()?.Name;
            }
            return model;
        }

        public override INameReferences TranslateVmToEntity(IVmTranslatedItem vModel)
        {
            throw new NotSupportedException();
        }
    }

    [RegisterService(typeof(TranslatedItemDefinitionHelper), RegisterType.Scope)]
    internal class TranslatedItemDefinitionHelper
    {
        public ITranslationDefinitions<TIn, TOut> AddTranslations<TIn, TOut>(ITranslationDefinitions<TIn, TOut> definition, string defaultName = null) where TIn : INameReferences where TOut : IVmTranslatedItem
        {
            var model = definition.AddPartial<INameReferences, IVmTranslatedItem>(i => i, o => o).GetFinal();
            if (string.IsNullOrEmpty(model?.DefaultTranslation) && !string.IsNullOrEmpty(defaultName))
            {
                model.DefaultTranslation = defaultName;
            }
            return definition;

        }
    }
}
