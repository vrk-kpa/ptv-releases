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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.AccessibilityRegister
{
    [RegisterService(typeof(ITranslator<AccessibilityRegisterGroup, VmAccessibilityRegisterGroup>), RegisterType.Transient)]
    internal class AccessibilityRegisterGroupTranslator : Translator<AccessibilityRegisterGroup, VmAccessibilityRegisterGroup>
    {
        private readonly ILanguageCache languageCache;

        public AccessibilityRegisterGroupTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override VmAccessibilityRegisterGroup TranslateEntityToVm(AccessibilityRegisterGroup entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                //AR LGE                .AddSimple(i => i.AccessibilityRegisterId, o => o.AccessibilityRegisterId)
                .AddSimple(i => i.OrderNumber.GetValueOrDefault(), o => o.OrderNumber)
                .AddCollection(i => i.Sentences.OrderBy(s => s.OrderNumber), o => o.Sentences)
                .AddDictionary(i => i.Values, o => o.SentenceGroups, k => languageCache.GetByValue(k.LocalizationId)); ;
            return definition.GetFinal();
        }

        public override AccessibilityRegisterGroup TranslateVmToEntity(VmAccessibilityRegisterGroup vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => true, o => o.Id, i => Guid.NewGuid())
                //AR LGE                .AddSimple(i => i.AccessibilityRegisterId, o => o.AccessibilityRegisterId)
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
                .AddCollectionWithRemove(i => i.SentenceGroups?.Select(
                        pair => new VmAccessibilityRegisterLanguageItem
                        {
                            OwnerReferenceId = Guid.Empty,
                            Value = pair.Value,
                            LocalizationId = languageCache.Get(pair.Key)
                        }),
                    o => o.Values, x => true)
                .AddCollectionWithRemove(i => i.Sentences, o => o.Sentences, x => true);

            return definition.GetFinal();
        }
    }
}
