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
    [RegisterService(typeof(ITranslator<PrintableFormChannelIdentifier, VmPrintableFormChannelIdentifier>), RegisterType.Transient)]
    internal class PrintableFormChannelIdentifierTranslator : Translator<PrintableFormChannelIdentifier, VmPrintableFormChannelIdentifier>
    {
        public PrintableFormChannelIdentifierTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmPrintableFormChannelIdentifier TranslateEntityToVm(PrintableFormChannelIdentifier entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.FormIdentifier, o => o.FormIdentifier)
                .AddSimple(i => i.PrintableFormChannelId, o => o.PrintableFormChannelId)
                .GetFinal();
        }

        public override PrintableFormChannelIdentifier TranslateVmToEntity(VmPrintableFormChannelIdentifier vModel)
        {
            if (vModel.LocalizationId.IsAssigned())
            {
                SetLanguage(vModel.LocalizationId.Value);
            }
            return CreateViewModelEntityDefinition<PrintableFormChannelIdentifier>(vModel)
                .UseDataContextCreate(i => !i.PrintableFormChannelId.IsAssigned())
                .UseDataContextLocalizedUpdate(i => i.PrintableFormChannelId.IsAssigned(), i=> o => i.PrintableFormChannelId == o.PrintableFormChannelId, def=> def.UseDataContextCreate(i => true))
               .AddNavigation(input => input.FormIdentifier, output => output.FormIdentifier)
               .AddSimple(i => i.LocalizationId.Value, o => o.LocalizationId)
               .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<PrintableFormChannelIdentifier, string>), RegisterType.Transient)]
    internal class FormIdentifierToStringTranslator : Translator<PrintableFormChannelIdentifier, string>
    {
        public FormIdentifierToStringTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override string TranslateEntityToVm(PrintableFormChannelIdentifier entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => input.FormIdentifier, output => output)
                .GetFinal();
        }

        public override PrintableFormChannelIdentifier TranslateVmToEntity(string vModel)
        {
            throw new NotSupportedException();
        }
    }
    
    [RegisterService(typeof(ITranslator<PrintableFormChannelIdentifier, string>), RegisterType.Transient)]
    internal class PrintableFormChannelIdentifierStringTranslator : Translator<PrintableFormChannelIdentifier, string>
    {
        public PrintableFormChannelIdentifierStringTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override string TranslateEntityToVm(PrintableFormChannelIdentifier entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.FormIdentifier, o => o).GetFinal();
        }

        public override PrintableFormChannelIdentifier TranslateVmToEntity(string vModel)
        {
            // not allowed to translate string to entity without proper update
            throw new NotSupportedException();
        }
    }
}
