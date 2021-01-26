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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Common
{
    [RegisterService(typeof(ITranslator<IDescription, string>), RegisterType.Transient)]
    internal class DescriptionTranslator : Translator<IDescription, string>
    {
        public DescriptionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override string TranslateEntityToVm(IDescription entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => input.Description, output => output)
                .GetFinal();
        }

        public override IDescription TranslateVmToEntity(string vModel)
        {
            throw new NotSupportedException();
        }
    }

    [RegisterService(typeof(ITranslator<IDescription, JsonLanguageLabel>), RegisterType.Transient)]
    internal class DescriptionExportTranslator : Translator<IDescription, JsonLanguageLabel>
    {
        public DescriptionExportTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) 
            : base(resolveManager, translationPrimitives)
        {
        }

        public override JsonLanguageLabel TranslateEntityToVm(IDescription entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => input.Description, output => output.Label)
                .AddNavigation(input => languageCache.GetByValue(input.LocalizationId), output => output.Lang)
                .GetFinal();
        }

        public override IDescription TranslateVmToEntity(JsonLanguageLabel vModel)
        {
            throw new NotSupportedException();
        }
    }
}
