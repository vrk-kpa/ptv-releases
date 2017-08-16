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
using PTV.Framework;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Database.DataAccess.Interfaces.Translators;
using System;
using System.Linq;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;

namespace PTV.Database.DataAccess.Translators.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationVersioned, VmOrganizationSearchListItem>), RegisterType.Transient)]
    internal class OrganizationSearchListItem : Translator<OrganizationVersioned, VmOrganizationSearchListItem>
    {
        private ILanguageCache languageCache;
        private ILanguageOrderCache languageOrderCache;
        public OrganizationSearchListItem(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ILanguageCache languageCache, ILanguageOrderCache languageOrderCache)
            : base(resolveManager, translationPrimitives)
        {
            this.languageCache = languageCache;
            this.languageOrderCache = languageOrderCache;
        }

        public override VmOrganizationSearchListItem TranslateEntityToVm(OrganizationVersioned entity)
        {
            var languageId = entity.OrganizationNames
                .Select(x => x.LocalizationId)
                .OrderBy(x => languageOrderCache.Get(x))
                .First();
            SetLanguage(languageId);
            var channelSearch = CreateEntityViewModelDefinition<VmOrganizationSearchListItem>(entity)
                .AddPartial(i => i, o => o as VmOrganizationListItem)
                .GetFinal();
            return channelSearch;
        }

        public override OrganizationVersioned TranslateVmToEntity(VmOrganizationSearchListItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
