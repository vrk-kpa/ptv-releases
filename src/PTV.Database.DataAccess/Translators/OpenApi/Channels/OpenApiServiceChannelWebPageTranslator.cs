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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using System;
using PTV.Database.DataAccess.Translators.OpenApi.Common;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelWebPage, VmOpenApiWebPageWithOrderNumber>), RegisterType.Transient)]
    internal class OpenApiServiceChannelWebPageTranslator : OpenApiEntityWithWebPageTranslator<ServiceChannelWebPage, VmOpenApiWebPageWithOrderNumber>
    {
        private ITypesCache typesCache;
        private ILanguageCache languageCache;

        public OpenApiServiceChannelWebPageTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) :
            base(resolveManager, translationPrimitives, cacheManager)
        {
            this.typesCache = cacheManager.TypesCache;
            this.languageCache = cacheManager.LanguageCache;
        }

        public override VmOpenApiWebPageWithOrderNumber TranslateEntityToVm(ServiceChannelWebPage entity)
        {
            return base.CreateEntityVmBaseDefinitions(entity)
                .GetFinal();
        }

        public override ServiceChannelWebPage TranslateVmToEntity(VmOpenApiWebPageWithOrderNumber vModel)
        {
            var typeId = typesCache.Get<WebPageType>(WebPageTypeEnum.HomePage.ToString());

            var definition = CreateViewModelEntityDefinition<ServiceChannelWebPage>(vModel)
               .DisableAutoTranslation();

            if (vModel.ExistsOnePerLanguage)
            {
                definition.UseDataContextUpdate(i => i.OwnerReferenceId.HasValue, i => o => i.OwnerReferenceId == o.ServiceChannelVersionedId &&
                    typeId == o.TypeId && languageCache.Get(i.Language) == o.WebPage.LocalizationId, w => w.UseDataContextCreate(x => true).AddSimple(i => typeId, o => o.TypeId));
            }
            else
            {
                definition.UseDataContextUpdate(i => i.OwnerReferenceId.HasValue, i => o => i.OwnerReferenceId == o.ServiceChannelVersionedId &&
                    typeId == o.TypeId && languageCache.Get(i.Language) == o.WebPage.LocalizationId && i.Url == o.WebPage.Url, w => w.UseDataContextCreate(x => true).AddSimple(i => typeId, o => o.TypeId));
            }

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
            
            return definition.GetFinal();
        }
    }
}
