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
using PTV.Domain.Model.Models.V2.TranslationOrder;

namespace PTV.Database.DataAccess.Translators.TranslationOrder
{
    [RegisterService(typeof(ITranslator<TranslationOrderState, VmTranslationOrderStateInput>), RegisterType.Transient)]
    internal class TranslationOrderStateInputTranslator : Translator<TranslationOrderState, VmTranslationOrderStateInput>
    {
        public TranslationOrderStateInputTranslator(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives) : base(resolveManager,
            translationPrimitives)
        {
        }

        public override VmTranslationOrderStateInput TranslateEntityToVm(TranslationOrderState entity)
        {
            throw new NotImplementedException();
        }

        public override TranslationOrderState TranslateVmToEntity(VmTranslationOrderStateInput vModel)
        {
            var exists = vModel.Id.IsAssigned();

            return CreateViewModelEntityDefinition<TranslationOrderState>(vModel)
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => i.Id == o.Id)
                //.UseDataContextUpdate(input => input.TranslationOrderId.IsAssigned(), input => output => input.TranslationOrderId == output.TranslationOrderId && input.TranslationStateId == output.TranslationStateId, def => def.UseDataContextCreate(x => true))
                .AddSimple(i => DateTime.UtcNow, o => o.SendAt)
                .AddSimple(i => i.TranslationOrderId, o => o.TranslationOrderId)
                .AddSimple(i => i.TranslationStateId, o => o.TranslationStateId)
                .GetFinal();
        }
    };
}