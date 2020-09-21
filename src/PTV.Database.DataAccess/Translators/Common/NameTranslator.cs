﻿/**
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
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Common
{
    [RegisterService(typeof(ITranslator<IName, string>), RegisterType.Transient)]
    internal class NameTranslator : Translator<IName, string>
    {
        public NameTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override string TranslateEntityToVm(IName entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => input.Name, output => output)
                .GetFinal();
        }

        public override IName TranslateVmToEntity(string vModel)
        {
            throw new NotSupportedException();
        }
    }

    [RegisterService(typeof(ITranslator<IText, string>), RegisterType.Transient)]
    internal class TextTranslator : Translator<IText, string>
    {
        public TextTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override string TranslateEntityToVm(IText entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => input.Text, output => output)
                .GetFinal();
        }

        public override IText TranslateVmToEntity(string vModel)
        {
            throw new NotSupportedException();
        }
    }

    [RegisterService(typeof(ITranslator<IName, JsonLanguageLabel>), RegisterType.Transient)]
    internal class NameExportTranslator : Translator<IName, JsonLanguageLabel>
    {
        public NameExportTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override JsonLanguageLabel TranslateEntityToVm(IName entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => input.Name, output => output.Label)
                .AddNavigation(input => languageCache.GetByValue(input.LocalizationId), output => output.Lang)
                .GetFinal();
        }

        public override IName TranslateVmToEntity(JsonLanguageLabel vModel)
        {
            throw new NotSupportedException();
        }
    }
}
