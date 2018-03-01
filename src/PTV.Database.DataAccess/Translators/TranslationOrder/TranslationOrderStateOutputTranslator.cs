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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.TranslationOrder;

namespace PTV.Database.DataAccess.Translators.TranslationOrder
{
    [RegisterService(typeof(ITranslator<TranslationOrderState, VmTranslationOrderStateOutput>), RegisterType.Transient)]
    internal class TranslationOrderStateOutputTranslator : Translator<TranslationOrderState, VmTranslationOrderStateOutput>
    {
        private ITypesCache typesCache;
        private ILanguageCache languageCache;

        public TranslationOrderStateOutputTranslator(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmTranslationOrderStateOutput TranslateEntityToVm(TranslationOrderState entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddSimple(input => input.Id, output => output.Id)
                .AddSimple(input => input.SendAt.ToEpochTime(), output => output.SentAt)
                .AddSimple(input => input.TranslationStateId, output => output.TranslationStateTypeId)
                .AddNavigation(input => input.TranslationOrder, output => output.TranslationOrder);

            if (entity.TranslationStateId == typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.InProgress.ToString()) && entity.TranslationOrder.DeliverAt.HasValue)
            {
                definition.AddSimple(input => input.TranslationOrder.DeliverAt.Value.ToEpochTime(), output => output.DeliverAt);
            }

            return definition.GetFinal();
        }

        public override TranslationOrderState TranslateVmToEntity(VmTranslationOrderStateOutput vModel)
        {
            throw new NotImplementedException();
        }
    };
}