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
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<PrintableFormChannelReceiver, VmPrintableFormChannelReceiver>), RegisterType.Transient)]
    internal class PrintableFormChannelReceiverTranslator : Translator<PrintableFormChannelReceiver, VmPrintableFormChannelReceiver>
    {
        public PrintableFormChannelReceiverTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmPrintableFormChannelReceiver TranslateEntityToVm(PrintableFormChannelReceiver entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.FormReceiver, o => o)
                .AddSimple(i => i.PrintableFormChannelId, o => o.PrintableFormChannelId)
                .GetFinal();
        }

        public override PrintableFormChannelReceiver TranslateVmToEntity(VmPrintableFormChannelReceiver vModel)
        {
            if (vModel.LocalizationId.IsAssigned())
            {
                SetLanguage(vModel.LocalizationId.Value);
            }
            return CreateViewModelEntityDefinition<PrintableFormChannelReceiver>(vModel)
                .UseDataContextCreate(i => !i.PrintableFormChannelId.IsAssigned())
                .UseDataContextLocalizedUpdate(i => i.PrintableFormChannelId.IsAssigned(), i => o => i.PrintableFormChannelId == o.PrintableFormChannelId, def => def.UseDataContextCreate(i => true))
               .AddNavigation(input => input.FormReceiver, output => output.FormReceiver)
               .AddSimple(i => i.LocalizationId.Value, o => o.LocalizationId)
               .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<PrintableFormChannelReceiver, string>), RegisterType.Transient)]
    internal class PrintableFormChannelReceiverStringTranslator : Translator<PrintableFormChannelReceiver, string>
    {
        public PrintableFormChannelReceiverStringTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override string TranslateEntityToVm(PrintableFormChannelReceiver entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.FormReceiver, o => o).GetFinal();
        }

        public override PrintableFormChannelReceiver TranslateVmToEntity(string vModel)
        {
            // not allowed to translate string to entity without proper update
            throw new NotSupportedException();
        }
    }
}
