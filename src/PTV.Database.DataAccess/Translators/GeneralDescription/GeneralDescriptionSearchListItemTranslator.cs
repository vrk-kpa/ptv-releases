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
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System;
using System.Linq;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionSearchListItem>), RegisterType.Transient)]
    internal class GeneralDescriptionSearchListItemTranslator : Translator<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionSearchListItem>
    {
        private ILanguageOrderCache languageOrderCache;
        public GeneralDescriptionSearchListItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageOrderCache languageOrderCache)
            : base(resolveManager, translationPrimitives)
        {
            this.languageOrderCache = languageOrderCache;
        }

        public override VmGeneralDescriptionSearchListItem TranslateEntityToVm(StatutoryServiceGeneralDescriptionVersioned entity)
        {
            var languageId = entity.Names
                .Select(x => x.LocalizationId)
                .OrderBy(x => languageOrderCache.Get(x))
                .First();
            SetLanguage(languageId);
            var generalDescription = CreateEntityViewModelDefinition<VmGeneralDescriptionSearchListItem>(entity)
                .AddPartial(i => i, o => o as VmGeneralDescriptionListItem)
                .GetFinal();
            return generalDescription;
        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(VmGeneralDescriptionSearchListItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
