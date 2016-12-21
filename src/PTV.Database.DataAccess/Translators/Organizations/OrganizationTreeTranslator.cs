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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Organizations
{
    [RegisterService(typeof(ITranslator<Organization, VmTreeItem>), RegisterType.Transient)]
    internal class OrganizationTreeTranslator : Translator<Organization, VmTreeItem>
    {
        private ILanguageCache languageCache;

        public OrganizationTreeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageCache languageCache) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = languageCache;
        }

        public override VmTreeItem TranslateEntityToVm(Organization entity)
        {
            return CreateEntityViewModelDefinition<VmTreeItem>(entity)
                .AddNavigation(i => languageCache.Filter(i.OrganizationNames.Where(name => name.TypeId == i.DisplayNameTypeId), RequestLanguageCode)?.Name, o => o.Name)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => i.Children.Count == 0, o => o.IsLeaf)
                .GetFinal();
        }

        public override Organization TranslateVmToEntity(VmTreeItem vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<Organization, VmExpandedVmTreeItem>), RegisterType.Transient)]
    internal class OrganizationTreeExpandedTranslator : Translator<Organization, VmExpandedVmTreeItem>
    {
        private ILanguageCache languageCache;

        public OrganizationTreeExpandedTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageCache languageCache) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = languageCache;
        }

        public override VmExpandedVmTreeItem TranslateEntityToVm(Organization entity)
        {
            return CreateEntityViewModelDefinition<VmExpandedVmTreeItem>(entity)
                .AddNavigation(i => languageCache.Filter(i.OrganizationNames.Where(name => name.TypeId == i.DisplayNameTypeId), RequestLanguageCode)?.Name, o => o.Name)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => i.Children.Any(), o => o.AreChildrenLoaded)
                .AddCollection(i => i.Children, o => o.Children)
                .AddSimple(i => i.Children.Count == 0, o => o.IsLeaf)
                .GetFinal();
        }

        public override Organization TranslateVmToEntity(VmExpandedVmTreeItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
