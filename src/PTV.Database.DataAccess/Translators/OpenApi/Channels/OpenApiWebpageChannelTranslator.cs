﻿/**
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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>), RegisterType.Transient)]
    internal class OpenApiWebPageChannelTranslator : OpenApiServiceChannelBaseTranslator<VmOpenApiWebPageChannelVersionBase>
    {
        private readonly ITypesCache typesCache;

        public OpenApiWebPageChannelTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives, cacheManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiWebPageChannelVersionBase TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var webPageChannel = entity.WebpageChannels?.FirstOrDefault();
            if (webPageChannel == null)
            {
                base.TranslateEntityToVm(entity);
            }

            var definition = CreateChannelDefinitions(entity)
                .AddCollection(i => i.WebpageChannels.SafePropertyFromFirst(j => j.LocalizedUrls).OrderByDescending(x => x.LocalizationId), o => o.WebPages)
                .AddSimple(i => webPageChannel.Id, o => o.ChannelId)
                .AddNavigation(i => i.AccessibilityClassificationLevelTypeId.IsAssigned() ?
                    typesCache.GetByValue<AccessibilityClassificationLevelType>(i.AccessibilityClassificationLevelTypeId.Value) : null, o => o.AccessibilityClassificationLevel)
                .AddNavigation(i => i.WcagLevelTypeId.IsAssigned() ? typesCache.GetByValue<WcagLevelType>(i.WcagLevelTypeId.Value)
                    : null, o => o.WCAGLevel)
                .AddCollection(i => i.WebPages.Select(w => w.WebPage).OrderByDescending(w => w.LocalizationId).ThenBy(w => w.OrderNumber), o => o.AccessibilityStatementWebPage);

            return definition.GetFinal();
        }
    }
}
