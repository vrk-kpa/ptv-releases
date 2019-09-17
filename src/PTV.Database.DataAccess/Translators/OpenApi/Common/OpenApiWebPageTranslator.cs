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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<WebPage, V9VmOpenApiWebPage>), RegisterType.Transient)]
    internal class OpenApiWebPageTranslator : OpenApiWebPageBaseTranslator<V9VmOpenApiWebPage>
    {
        public OpenApiWebPageTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
        }

        public override V9VmOpenApiWebPage TranslateEntityToVm(WebPage entity)
        {
            return base.CreateBaseEntityVmDefinitions(entity)
                .AddSimple(i => i.OrderNumber.GetValueOrDefault(), o => o.OrderNumber)
                .GetFinal();
        }
        public override WebPage TranslateVmToEntity(V9VmOpenApiWebPage vModel)
        {
            return base.CreateBaseVmEntityDefinitions(vModel)
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<WebPage, V4VmOpenApiWebPage>), RegisterType.Transient)]
    internal class OpenApiWebPageOldTranslator : OpenApiWebPageBaseTranslator<V4VmOpenApiWebPage>
    {
        public OpenApiWebPageOldTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
        }

        public override V4VmOpenApiWebPage TranslateEntityToVm(WebPage entity)
        {
            return base.CreateBaseEntityVmDefinitions(entity)
                .AddNavigation(i => i.OrderNumber.HasValue ? i.OrderNumber.Value.ToString() : "0", o => o.OrderNumber)
                .GetFinal();
        }
        public override WebPage TranslateVmToEntity(V4VmOpenApiWebPage vModel)
        {
            return base.CreateBaseVmEntityDefinitions(vModel)
                .AddSimple(i => i.OrderNumber.IsNullOrEmpty() ? 0 : i.OrderNumber.ParseToInt(), o => o.OrderNumber)
                .GetFinal();
        }
    }
}
