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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Database.DataAccess.Translators.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationWebPage, VmWebPage>), RegisterType.Transient)]
    internal class OrganizationWebPageTranslator : Translator<OrganizationWebPage, VmWebPage>
    {
        private ITypesCache typesCache;
        public OrganizationWebPageTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmWebPage TranslateEntityToVm(OrganizationWebPage entity)
        {
            return CreateEntityViewModelDefinition<VmWebPage>(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => i.TypeId, o => o.TypeId)
                .AddPartial(i => i.WebPage, o => o as VmUrl)
                .AddSimple(i => i.LocalizationId, o => o.LocalizationId)
                .AddNavigation(i => i.Name, o => o.Name)
                .AddNavigation(i => i.Description, o => o.Description)
                .AddPartial(i => i as IOrderable, o => o as IVmOrderable)
                .GetFinal();
        }

        public override OrganizationWebPage TranslateVmToEntity(VmWebPage vModel)
        {
            if (vModel?.UrlAddress.IsNullOrEmpty() ?? true)
            {
                return null;
            }

            var homePageType = typesCache.Get<WebPageType>(WebPageTypeEnum.HomePage.ToString());
            var exists = vModel.Id.IsAssigned();
            var languageId = vModel.LocalizationId.IsAssigned()
                ? vModel.LocalizationId.Value
                : RequestLanguageId;

            var result = CreateViewModelEntityDefinition<OrganizationWebPage>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(x => !exists)
                .UseDataContextUpdate(x => exists, i => o => i.Id == o.Id)
                .AddSimple(i => languageId, o => o.LocalizationId)
                .AddNavigation(i => i as VmUrl, o => o.WebPage)
                // HACK: There is a rare case when a content contains multiple references
                // to the same web page and the user wants to change just one URL, that
                // the later loads of the old web page will overwrite the new one.
                // Thus we need to change the FK ID, which will not be overwritten and
                // will take precedence during saving to database.
                .Propagation((i, o) =>
                {
                    if (o.WebPage?.Id.IsAssigned() ?? false)
                    {
                        o.WebPageId = o.WebPage.Id;
                    }
                })
                .AddNavigation(i => i.Name, o => o.Name)
                .AddNavigation(i => i.Description, o => o.Description)
                .AddSimple(i => i.TypeId ?? homePageType, o => o.TypeId)
                .AddPartial(i => i as IVmOrderable, o => o as IOrderable)
                .GetFinal();

            return result;
        }
    }
}
