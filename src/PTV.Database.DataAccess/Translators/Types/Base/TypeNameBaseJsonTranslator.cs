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
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models.Import;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Types.Base
{
    internal abstract class TypeNameBaseJsonTranslator<TName, TType> : Translator<TName, VmJsonTypeName> where TName : NameBase<TType> where TType : TypeBase
    {
        protected TypeNameBaseJsonTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
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
                .AddNavigation(input => input.TypeId.ToString(), output => output.TypeId)
                .AddNavigation(input => input.Localization.Code, output => output.Language);
        }

        public override TName TranslateVmToEntity(VmJsonTypeName vModel)
        {
            return CreateViewModelEntityDefinition<TName>(vModel)
                .UseDataContextUpdate(i => i.TypeId != null, i => o => (i.Language == o.Localization.Code) && (i.TypeId == o.TypeId.ToString()), def => def.UseDataContextCreate(i => true))
                .AddNavigation(input => input.Name, output => output.Name)
                .AddNavigation(input => input.Language, output => output.Localization)
                .GetFinal();
        }
    }
}
