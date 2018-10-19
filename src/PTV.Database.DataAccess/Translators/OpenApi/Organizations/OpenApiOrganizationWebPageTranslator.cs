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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi.V9;
using System;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Enums;
using PTV.Database.DataAccess.Translators.OpenApi.Common;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationWebPage, V9VmOpenApiWebPage>), RegisterType.Transient)]
    internal class OpenApiOrganizationWebPageTranslator : OpenApiEntityWithWebPageTranslator<OrganizationWebPage, V9VmOpenApiWebPage>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public OpenApiOrganizationWebPageTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override V9VmOpenApiWebPage TranslateEntityToVm(OrganizationWebPage entity)
        {
            return base.CreateEntityVmBaseDefinitions(entity)
                .AddSimple(i => i.OrganizationVersionedId, o => o.OwnerReferenceId)
                .GetFinal();
        }

        public override OrganizationWebPage TranslateVmToEntity(V9VmOpenApiWebPage vModel)
        {
            var definition = CreateViewModelEntityDefinition<OrganizationWebPage>(vModel)
               .DisableAutoTranslation()
               .UseDataContextUpdate(i => i.OwnerReferenceId.HasValue, i => o => i.OwnerReferenceId == o.OrganizationVersionedId &&
                   languageCache.Get(i.Language) == o.WebPage.LocalizationId && i.Url == o.WebPage.Url, w => w.UseDataContextCreate(x => true));

            if (vModel.OwnerReferenceId.HasValue)
            {
                var entity = definition.GetFinal();
                if (entity.WebPageId != null && entity.Created != DateTime.MinValue)
                {
                    // We are updating already existing item
                    vModel.Id = entity.WebPageId;
                }
            }

            definition.AddNavigation(i => i, o => o.WebPage);
            definition.AddSimple(i => typesCache.Get<WebPageType>(WebPageTypeEnum.HomePage.ToString()), o => o.TypeId);

            return definition.GetFinal();
        }
    }
}
