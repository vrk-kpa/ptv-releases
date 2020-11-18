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
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Types.Base
{
    internal abstract class TypeNameBaseJsonTranslator<TName, TType> : Translator<TName, VmJsonTypeName> where TName : NameBase<TType> where TType : TypeBase
    {
        protected TypeNameBaseJsonTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) 
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmJsonTypeName TranslateEntityToVm(TName entity)
        {
            return GetDefaultTranslationDefinitions(entity)
                .GetFinal();
        }

        protected ITranslationDefinitions<TName, VmJsonTypeName> GetDefaultTranslationDefinitions(TName entity)
        {
            return CreateEntityViewModelDefinition<VmJsonTypeName>(entity)
                .AddNavigation(input => input.Name, output => output.Name)
                .AddSimple(input => input.TypeId, output => output.TypeId)
                .AddNavigation(input => input.Localization.Code, output => output.Language);
        }

        public override TName TranslateVmToEntity(VmJsonTypeName vModel)
        {
            if (!string.IsNullOrEmpty(vModel.Language))
            {
                SetLanguage(languageCache.Get(vModel.Language));
            }
            return CreateViewModelEntityDefinition<TName>(vModel)
                .UseDataContextLocalizedUpdate(i => i.TypeId.IsAssigned(), i => o => (i.TypeId == o.TypeId), def => def.UseDataContextCreate(i => true))
                .AddNavigation(input => input.Name, output => output.Name)
                .AddRequestLanguage(o => o)
                .GetFinal();
        }
    }
}
