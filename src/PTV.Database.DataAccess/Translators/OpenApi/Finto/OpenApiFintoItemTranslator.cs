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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models.Base;
using PTV.Framework.Interfaces;
using System;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces.OpenApi;

namespace PTV.Database.DataAccess.Translators.OpenApi.Finto
{
    internal abstract class OpenApiFintoItemTranslator<TFintoItem, TFintoName, TModel> : Translator<TFintoItem, TModel>
        where TFintoItem : FintoItemBase, IFintoItemNames<TFintoName> where TFintoName : NameBase where TModel : class, IVmOpenApiFintoItemVersionBase
    {
        protected OpenApiFintoItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override TModel TranslateEntityToVm(TFintoItem entity)
        {
            return CreateBaseDefinitions(entity).GetFinal();
        }

        protected ITranslationDefinitions<TFintoItem, TModel> CreateBaseDefinitions(TFintoItem entity)
        {
            return CreateEntityViewModelDefinition<TModel>(entity)
                .AddCollection(i => i.Names, o => o.Name)
                // For empty items let's return null (PTV-2775)
                .AddNavigation(i => string.IsNullOrEmpty(i.Code) ? null : i.Code, o => o.Code)
                .AddNavigation(i => string.IsNullOrEmpty(i.OntologyType) ? null : i.OntologyType, o => o.OntologyType)
                .AddNavigation(i => string.IsNullOrEmpty(i.Uri) ? null : i.Uri, o => o.Uri)
                .AddNavigation(i => string.IsNullOrEmpty(i.ParentUri) ? null : i.ParentUri, o => o.ParentUri);
        }

        public override TFintoItem TranslateVmToEntity(TModel vModel)
        {
            throw new NotImplementedException();
        }
    }
}
