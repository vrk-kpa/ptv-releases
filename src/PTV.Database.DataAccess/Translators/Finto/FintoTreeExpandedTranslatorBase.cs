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
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Finto
{
    [RegisterService(typeof(ITranslator<IFintoItem, VmExpandedVmTreeItem>), RegisterType.Transient)]
    internal class FintoTreeExpandedTranslatorBase : Translator<IFintoItem, VmExpandedVmTreeItem>
    {
        private ITranslationEntity entityTranslationManager;

        public FintoTreeExpandedTranslatorBase(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITranslationEntity entityTranslationManager) : base(resolveManager, translationPrimitives)
        {
            this.entityTranslationManager = entityTranslationManager;
        }

        public override VmExpandedVmTreeItem TranslateEntityToVm(IFintoItem entity)
        {
            var model = CreateEntityViewModelDefinition<VmExpandedVmTreeItem>(entity)
                .AddPartial<IFintoItemBase, VmTreeItem>(i => i, o => o)
                .AddSimple(i => !i.Children.Any(), o => o.IsLeaf)
                .AddSimple(i => false, o => o.IsCollapsed)
                .AddSimple(i => true, o => o.AreChildrenLoaded)
                .GetFinal();
            model.Children = entity.Children.Select(input =>
                 entityTranslationManager.Translate<IFintoItem, VmExpandedVmTreeItem>(input as IFintoItem) as VmTreeItem).OrderBy(x => x.Name).ToList();
            return model;
        }

        public override IFintoItem TranslateVmToEntity(VmExpandedVmTreeItem vModel)
        {
            throw new NotImplementedException();
        }
    }
    [RegisterService(typeof(ITranslator<OntologyTerm, VmExpandedVmTreeItem>), RegisterType.Transient)]
    internal class FintoMultiParentTreeExpandedTranslatorBase : Translator<OntologyTerm, VmExpandedVmTreeItem>
    {
        private ITranslationEntity entityTranslationManager;

        public FintoMultiParentTreeExpandedTranslatorBase(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITranslationEntity entityTranslationManager) : base(resolveManager, translationPrimitives)
        {
            this.entityTranslationManager = entityTranslationManager;
        }

        public override VmExpandedVmTreeItem TranslateEntityToVm(OntologyTerm entity)
        {
            var model = CreateEntityViewModelDefinition<VmExpandedVmTreeItem>(entity)
                .AddPartial<IFintoItemBase, VmTreeItem>(i => i, o => o)
//                .AddNavigation(i => i.Label, o => o.Name)
//                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => !i.Children.Any(), o => o.IsLeaf)
                .AddSimple(i => false, o => o.IsCollapsed)
                .AddSimple(i => true, o => o.AreChildrenLoaded)
//                .AddCollection(i => i.Children.Select(x => x.Child), o => o.Children)
                .GetFinal();
            model.Children = entity.Children.Select(input =>
                 entityTranslationManager.Translate<OntologyTerm, VmExpandedVmTreeItem>(input.Child) as VmTreeItem).OrderBy(x => x.Name).ToList();

            return model;
        }

        public override OntologyTerm TranslateVmToEntity(VmExpandedVmTreeItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
