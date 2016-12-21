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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Finto
{
    [RegisterService(typeof(ITranslator<IFintoItem, VmTreeItem>), RegisterType.Transient)]
    internal class FintoTreeTranslator : Translator<IFintoItem, VmTreeItem>
    {
        public FintoTreeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmTreeItem TranslateEntityToVm(IFintoItem entity)
        {
            return CreateEntityViewModelDefinition<VmTreeItem>(entity)
                .AddNavigation(i => i.Label, o => o.Name)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => !i.Children.Any(), o => o.IsLeaf)
                .GetFinal();
        }

        public override IFintoItem TranslateVmToEntity(VmTreeItem vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<IFintoItemBase, VmTreeItem>), RegisterType.Transient)]
    internal class FintoBaseTreeTranslator : Translator<IFintoItemBase, VmTreeItem>
    {
        public FintoBaseTreeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmTreeItem TranslateEntityToVm(IFintoItemBase entity)
        {
            return CreateEntityViewModelDefinition<VmTreeItem>(entity)
                .AddNavigation(i => i.Label, o => o.Name)
                .AddSimple(i => i.Id, o => o.Id)
                .GetFinal();
        }

        public override IFintoItemBase TranslateVmToEntity(VmTreeItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
