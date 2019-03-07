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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
    [RegisterService(typeof(ITranslator<LawWebPage, VmWebPage>), RegisterType.Transient)]
    internal class LawWebPageTranslator : Translator<LawWebPage, VmWebPage>
    {
        private ILanguageCache languageCache;
        public LawWebPageTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
        }

        public override VmWebPage TranslateEntityToVm(LawWebPage entity)
        {
            ;
            throw new NotImplementedException();
        }

        public override LawWebPage TranslateVmToEntity(VmWebPage vModel)
        {
            SetLanguage(vModel.LocalizationId.Value);
            bool created = false;
            var transaltionDefinition = CreateViewModelEntityDefinition<LawWebPage>(vModel)
                .UseDataContextUpdate(i => true, i => o => (i.OwnerReferenceId == o.LawId && ((i.Id.HasValue && o.WebPageId == i.Id) || (!i.Id.HasValue && o.WebPage.LocalizationId == RequestLanguageId))), def => { def.UseDataContextCreate(x => true); created = true; }
                ).Propagation((x,y)=> { if (!created) x.Id = y.WebPageId; })
                .AddNavigation(input => input, output => output.WebPage);

            var entity = transaltionDefinition.GetFinal();
            return entity;
        }

    }
}