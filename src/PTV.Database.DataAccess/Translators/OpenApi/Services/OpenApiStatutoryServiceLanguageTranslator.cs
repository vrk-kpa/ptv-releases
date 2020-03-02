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
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{

    [RegisterService(typeof(ITranslator<StatutoryServiceLanguage, VmOpenApiStringItem>), RegisterType.Scope)]
    internal class OpenApiStatutoryServiceLanguageTranslator : Translator<StatutoryServiceLanguage, VmOpenApiStringItem>
    {
        private readonly ILanguageCache languageCache;

        public OpenApiStatutoryServiceLanguageTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageCache languageCache) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = languageCache;
        }
        public override VmOpenApiStringItem TranslateEntityToVm(StatutoryServiceLanguage entity)
        {
            throw new NotImplementedException();
        }
        public override StatutoryServiceLanguage TranslateVmToEntity(VmOpenApiStringItem vModel)
        {
            var languageId = languageCache.Get(vModel.Value);

            return CreateViewModelEntityDefinition<StatutoryServiceLanguage>(vModel)
                .UseDataContextUpdate(
                  i => i.OwnerReferenceId.IsAssigned(),
                  i => o => i.OwnerReferenceId.Value == o.StatutoryServiceGeneralDescriptionVersionedId && languageId == o.LanguageId,
                  def => def.UseDataContextCreate(i => true)
                )
                .AddNavigation(i => vModel.Value, o => o.Language)
                .AddSimple(i => languageId, o => o.LanguageId)
                .GetFinal();
        }
    }
}
