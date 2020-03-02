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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<LawWebPage, V4VmOpenApiWebPage>), RegisterType.Transient)]
    internal class OpenApiLawWebPageTranslator : Translator<LawWebPage, V4VmOpenApiWebPage>
    {
        private readonly ILanguageCache languageCache;

        public OpenApiLawWebPageTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) :
            base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
        }

        public override V4VmOpenApiWebPage TranslateEntityToVm(LawWebPage entity)
        {
            return CreateEntityViewModelDefinition<V4VmOpenApiWebPage>(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => i.LawId, o => o.OwnerReferenceId)
                .AddNavigation(i => i.WebPage, o => o.Url)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .AddNavigation(i => i.OrderNumber.ToString(), o => o.OrderNumber)
                .GetFinal();
        }

        public override LawWebPage TranslateVmToEntity(V4VmOpenApiWebPage vModel)
        {
            return CreateViewModelEntityDefinition<LawWebPage>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(x => true)
                .AddNavigation(i => i.Url, o => o.WebPage)
                .AddSimple(i => languageCache.Get(i.Language), o => o.LocalizationId)
                .AddSimple(i => i.OrderNumber.ParseToInt(), o => o.OrderNumber)
                .GetFinal();
        }
    }
}
