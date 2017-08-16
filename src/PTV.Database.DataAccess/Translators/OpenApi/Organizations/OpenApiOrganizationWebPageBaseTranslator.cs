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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.OpenApi.Common;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    internal abstract class OpenApiOrganizationWebPageBaseTranslator<TModel> : OpenApiEntityWithWebPageTranslator<OrganizationWebPage, TModel> where TModel : class, IVmOpenApiWebPageWithOrderNumber
    {
        private ITypesCache typesCache;
        private ILanguageCache languageCache;

        protected OpenApiOrganizationWebPageBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives, cacheManager)
        {
            this.typesCache = cacheManager.TypesCache;
            this.languageCache = cacheManager.LanguageCache;
        }
        public override TModel TranslateEntityToVm(OrganizationWebPage entity)
        {
           return CreateBaseEntityVmDefinitions(entity).GetFinal();
        }
        public override OrganizationWebPage TranslateVmToEntity(TModel vModel)
        {
            return CreateBaseVmEntityDefinitions(vModel).GetFinal();
        }
        protected ITranslationDefinitions<OrganizationWebPage, TModel> CreateBaseEntityVmDefinitions(OrganizationWebPage entity)
        {
            return base.CreateEntityVmBaseDefinitions(entity)
                .AddSimple(i => i.OrganizationVersionedId, o => o.OwnerReferenceId);
        }

        protected ITranslationDefinitions<TModel, OrganizationWebPage> CreateBaseVmEntityDefinitions(TModel vModel)
        {
            var definition = CreateViewModelEntityDefinition<OrganizationWebPage>(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => i.OwnerReferenceId.HasValue, i => o => i.OwnerReferenceId == o.OrganizationVersionedId && /*i.Type == o.Type.Code &&*/
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

            return definition;
        }
    }
}
