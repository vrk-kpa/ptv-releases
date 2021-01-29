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
    internal abstract class OpenApiFintoItemWithParentTranslator<TFintoItem, TFintoName, TModel> : OpenApiFintoItemTranslator<TFintoItem, TFintoName, TModel>
        where TFintoItem : FintoItemBase<TFintoItem>, IFintoItemNames<TFintoName> where TFintoName : NameBase where TModel : class, IVmOpenApiFintoItemVersionBase
    {
        protected OpenApiFintoItemWithParentTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override TModel TranslateEntityToVm(TFintoItem entity)
        {
            return CreateDefinitions(entity).GetFinal();
        }

        public override TFintoItem TranslateVmToEntity(TModel vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiFintoItemWithParentTranslator");
        }

        protected ITranslationDefinitions<TFintoItem, TModel> CreateDefinitions(TFintoItem entity)
        {
            return base.CreateBaseDefinitions(entity)
                .AddSimple(i => i.ParentId, o => o.ParentId);
        }
    }
}
