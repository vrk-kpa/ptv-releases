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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmOpenApiWebPageChannelInVersionBase>), RegisterType.Transient)]
    internal class OpenApiWebPageChannelMainInTranslator : OpenApiServiceChannelInTranslator<VmOpenApiWebPageChannelInVersionBase>
    {
        public OpenApiWebPageChannelMainInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives, cacheManager)
        {}

        public override ServiceChannelVersioned TranslateVmToEntity(VmOpenApiWebPageChannelInVersionBase vModel)
        {
            var definitions = base.CreateVmToChannelDefinitions(vModel)
                .AddSimple(i => typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.WebPage.ToString()), o => o.TypeId)
                .AddNavigationOneMany(i => i, o => o.WebpageChannels);


            if (vModel.AccessibilityClassification?.Count > 0)
            {
                if (vModel.VersionId.IsAssigned())
                {
                    vModel.AccessibilityClassification.ForEach(w => w.OwnerReferenceId = vModel.VersionId);
                }
                definitions.AddCollection(i => i.AccessibilityClassification, o => o.AccessibilityClassifications, true);
            }

            return definitions.GetFinal();
        }
    }
}
