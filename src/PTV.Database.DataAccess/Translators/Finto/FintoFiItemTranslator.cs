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
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.Import;

namespace PTV.Database.DataAccess.Translators.Finto
{
    internal abstract class FintoFiItemTranslator<TFintoItem, TName> : Translator<TFintoItem, FintoFiJsonItem> where TFintoItem : FintoItemBase<TFintoItem>, IFintoItemNames<TName> where TName : NameBase
    {
        public FintoFiItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        protected ITranslationDefinitions<FintoFiJsonItem, TFintoItem> GetDefaultMappingViewModelToEntity(FintoFiJsonItem vModel)
        {
            return CreateViewModelEntityDefinition<TFintoItem>(vModel)
//                .UseDataContextCreate(i => true, o => o.Id, i => Guid.NewGuid())
                .AddSimple(i => Guid.NewGuid(), o => o.Id)
                .AddNavigation(i => i.Label?.Value, o => o.Label)
                .AddNavigation(i => i.Uri, o => o.Uri)
                .AddNavigation(i => i.Notation ?? i.Uri, o => o.Code)
                .AddNavigation(i => i.Broader?.Uri, o => o.ParentUri)
                .AddNavigation(i => i.Type, o => o.OntologyType)
//                .AddCollection(m => new List<string> { m.Label?.Value }, o => o.Names)
                ;
        }

        public override TFintoItem TranslateVmToEntity(FintoFiJsonItem vModel)
        {
            return GetDefaultMappingViewModelToEntity(vModel)
                .GetFinal();
        }

        public override FintoFiJsonItem TranslateEntityToVm(TFintoItem entity)
        {
            throw new NotSupportedException();
        }
    }
}
