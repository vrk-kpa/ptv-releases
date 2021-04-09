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
    [RegisterService(typeof(ITranslator<IFintoItem, VmTreeItem>), RegisterType.Transient)]
    internal class FintoTreeTranslator : Translator<IFintoItem, VmTreeItem>
    {
        private readonly ILanguageOrderCache orderCache;

        public FintoTreeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) 
            : base(resolveManager, translationPrimitives)
        {
            orderCache = cacheManager.LanguageOrderCache;
        }

        public override VmTreeItem TranslateEntityToVm(IFintoItem entity)
        {
            var model =  CreateEntityViewModelDefinition<VmTreeItem>(entity)
                .AddSimple(i => !i.IsValid, o => o.Invalid)
                .AddSimple(i => i.Id, o => o.Id)
                .AddPartial<INameReferences, IVmTranslatedItem>(i => i as INameReferences, o => o)
                .AddDictionary(i => GetDescription(i as IDescriptionReferences), o => o.Description, k => languageCache.GetByValue(k.LocalizationId))
                .AddSimple(i => !i.Children.Any(), o => o.IsLeaf)
                .GetFinal();

            if (string.IsNullOrEmpty(model.Name))
            {
                model.Name = entity.Label;
            }

            return model;
        }

        public override IFintoItem TranslateVmToEntity(VmTreeItem vModel)
        {
            throw new NotImplementedException();
        }
        
        private IEnumerable<IDescription> GetDescription(IDescriptionReferences entity)
        {
            if (entity != null)
            {
                return entity.Descriptions.OrderBy(x => orderCache.Get(x.LocalizationId));
            }
            return new List<IDescription>();
        }
    }

    [RegisterService(typeof(ITranslator<IFintoItemBase, VmTreeItem>), RegisterType.Transient)]
    internal class FintoBaseTreeTranslator : Translator<IFintoItemBase, VmTreeItem>
    {
        private readonly ILanguageOrderCache orderCache;
        public FintoBaseTreeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) 
            : base(resolveManager, translationPrimitives)
        {
            orderCache = cacheManager.LanguageOrderCache;
        }

        public override VmTreeItem TranslateEntityToVm(IFintoItemBase entity)
        {
            var model = CreateEntityViewModelDefinition<VmTreeItem>(entity)
                .AddPartial(i => i, o => o as VmSelectableItem)
                .AddDictionary(i => GetDescription(i as IDescriptionReferences), o => o.Description, k => languageCache.GetByValue(k.LocalizationId))
                .GetFinal();

            return model;
        }

        private IEnumerable<IDescription> GetDescription(IDescriptionReferences entity)
        {
            if (entity != null)
            {
                return entity.Descriptions.OrderBy(x => orderCache.Get(x.LocalizationId));
            }
            return new List<IDescription>();
        }

        public override IFintoItemBase TranslateVmToEntity(VmTreeItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
