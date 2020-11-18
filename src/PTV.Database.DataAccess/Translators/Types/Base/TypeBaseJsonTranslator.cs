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
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models.Import;
using PTV.Framework.Interfaces;
using PTV.Framework;

namespace PTV.Database.DataAccess.Translators.Types.Base
{
    internal abstract class TypeBaseJsonTranslator<T, TName> : TypeBaseJsonTranslator<T> where T : TypeBase<TName> where TName : NameBase
    {
        protected TypeBaseJsonTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }
        public override VmJsonTypeItem TranslateEntityToVm(T entity)
        {
            throw new NotSupportedException();
        }

        protected override ITranslationDefinitions<VmJsonTypeItem, T> GetDefaultMappingViewModelToEntity(VmJsonTypeItem vModel)
        {
            var definition = base.GetDefaultMappingViewModelToEntity(vModel);
            var instance = definition.GetFinal();
            vModel.Names.ForEach(i => i.TypeId = instance.Id);
            definition.AddCollection(input => input.Names, output => output.Names);
            return definition;
        }

        public override T TranslateVmToEntity(VmJsonTypeItem vModel)
        {
            return GetDefaultMappingViewModelToEntity(vModel).GetFinal();
        }
    }

    internal abstract class TypeBaseJsonTranslator<T> : Translator<T, VmJsonTypeItem> where T : TypeBase
    {
        protected TypeBaseJsonTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        protected Dictionary<string, int?> OrderMapping = new Dictionary<string, int?>();

        public override VmJsonTypeItem TranslateEntityToVm(T entity)
        {
            throw new NotSupportedException();
        }

        protected virtual ITranslationDefinitions<VmJsonTypeItem, T> GetDefaultMappingViewModelToEntity(VmJsonTypeItem vModel)
        {
            return CreateViewModelEntityDefinition<T>(vModel)
                .UseDataContextLocalizedUpdate(i => true, i => o => i.Code == o.Code, def =>
                {
                    def.UseDataContextCreate(i => true, o => o.Id, i => i.Code.GetGuid<T>())
                        .AddNavigation(input => input.Code, output => output.Code);
                })
                .AddSimple(input => OrderMapping.TryGet(input.Code) ?? input.OrderNumber, output => output.OrderNumber);
        }

        public override T TranslateVmToEntity(VmJsonTypeItem vModel)
        {
            return GetDefaultMappingViewModelToEntity(vModel).GetFinal();
        }
    }
}
