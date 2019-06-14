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
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<ServiceLaw, V4VmOpenApiLaw>), RegisterType.Transient)]
    internal class OpenApiServiceLawTranslator : Translator<ServiceLaw, V4VmOpenApiLaw>
    {
        private readonly ILanguageCache languageCache;
        public OpenApiServiceLawTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override V4VmOpenApiLaw TranslateEntityToVm(ServiceLaw entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceLawTranslator");
        }

        public override ServiceLaw TranslateVmToEntity(V4VmOpenApiLaw vModel)
        {
            var webPage = vModel.WebPages.FirstOrDefault(w => w.Language == DomainConstants.DefaultLanguage);
            if (webPage == null)
            {
                webPage = vModel.WebPages.FirstOrDefault();
            }

            var exists = vModel.OwnerReferenceId.IsAssigned();
            if (webPage == null)
            {
                exists = false;
            }

            var language = webPage?.Language;
            var langId = languageCache.Get(language);

            var definition = CreateViewModelEntityDefinition<ServiceLaw>(vModel)
                .UseDataContextUpdate(i => exists,
                i => o => i.OwnerReferenceId == o.ServiceVersionedId && o.Law.WebPages.Where(w => w.WebPage.LocalizationId == langId && w.WebPage.Url == webPage.Url).FirstOrDefault() != null,
                x => x.UseDataContextCreate(i => true));

            if (exists)
            {
                var entity = definition.GetFinal();
                if (entity.Created != DateTime.MinValue) // We are updating existing law
                {
                    vModel.Id = entity.LawId;
                }
            }

            return definition.AddNavigation(i => i, o => o.Law)
                .GetFinal();
        }
    }
}
