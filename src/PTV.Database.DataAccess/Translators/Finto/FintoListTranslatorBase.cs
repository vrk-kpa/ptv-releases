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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Finto
{
    [RegisterService(typeof(ITranslator<IFintoItemBase, VmListItem>), RegisterType.Transient)]
    internal class FintoListTranslatorBase : Translator<IFintoItemBase, VmListItem>
    {
        public FintoListTranslatorBase(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmListItem TranslateEntityToVm(IFintoItemBase entity)
        {
            var model = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddPartial<INameReferences, IVmTranslatedItem>(i => i as INameReferences, o => o)
                .GetFinal();

            if (string.IsNullOrEmpty(model.Name))
            {
                model.Name = entity.Label;
            }

            return model;
        }

        public override IFintoItemBase TranslateVmToEntity(VmListItem vModel)
        {
            throw new NotSupportedException();
        }
    }

    [RegisterService(typeof(ITranslator<IFintoItemBase, VmSelectableItem>), RegisterType.Transient)]
    internal class FintoSelectableListTranslatorBase : Translator<IFintoItemBase, VmSelectableItem>
    {
        public FintoSelectableListTranslatorBase(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmSelectableItem TranslateEntityToVm(IFintoItemBase entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => !i.IsValid, o => o.Invalid)
                .AddPartial(i => i, o => o as VmListItem)
                .GetFinal();
        }

        public override IFintoItemBase TranslateVmToEntity(VmSelectableItem vModel)
        {
            throw new NotSupportedException();
        }
    }
}
