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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<Keyword, VmOpenApiLanguageItem>), RegisterType.Transient)]
    internal class OpenApiKeywordTranslator : Translator<Keyword, VmOpenApiLanguageItem>
    {
        public OpenApiKeywordTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) :
            base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiLanguageItem TranslateEntityToVm(Keyword entity)
        {
            throw new NotImplementedException();
        }

        public override Keyword TranslateVmToEntity(VmOpenApiLanguageItem vModel)
        {
            bool exists = vModel.Id.IsAssigned();

            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(input => exists, input => output => input.Id == output.Id)
                .UseDataContextCreate(input => !exists, output => output.Id, input => Guid.NewGuid())
                .AddNavigation(i => i.Value, o => o.Name)
                .AddSimple(i => languageCache.Get(i.Language), o => o.LocalizationId)
                .GetFinal();
        }
    }
}
