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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmOpenApiElectronicChannelInVersionBase>), RegisterType.Transient)]
    internal class OpenApiEChannelMainInTranslator : OpenApiServiceChannelInTranslator<VmOpenApiElectronicChannelInVersionBase>
    {
        ITypesCache typesCache;

        public OpenApiEChannelMainInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives, cacheManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmOpenApiElectronicChannelInVersionBase vModel)
        {
            if (vModel.VersionId.IsAssigned())
            {
                vModel.AccessibilityStatementWebPage.ForEach(w => w.OwnerReferenceId = vModel.VersionId);
            }

            var definitions = CreateVmToChannelDefinitions(vModel)
                .AddNavigation(i => ServiceChannelTypeEnum.EChannel.ToString(), o => o.Type)
                .AddNavigationOneMany(i => i, o => o.ElectronicChannels);

            if ((vModel.DeleteAllAttachments && vModel.Attachments?.Count == 0) || vModel.Attachments?.Count > 0)
            {
                // VmOpenApiAttachment -> ServiceChannelAttachment
                var order = 1;
                vModel.Attachments.ForEach(a => a.OrderNumber = order++);
                definitions.AddCollectionWithRemove(i => i.Attachments, o => o.Attachments, x => true);
            }
            
            if (!string.IsNullOrEmpty(vModel.AccessibilityClassificationLevel))
            {
                definitions = definitions.AddSimple(i => typesCache.Get<AccessibilityClassificationLevelType>(vModel.AccessibilityClassificationLevel), o => o.AccessibilityClassificationLevelTypeId);
                // We need to empty the WcagLevel info if user has selected that the channel is not compliant or unknown (SFIPTV-37).
                if (vModel.AccessibilityClassificationLevel == AccessibilityClassificationLevelTypeEnum.NonCompliant.ToString() ||
                    vModel.AccessibilityClassificationLevel == AccessibilityClassificationLevelTypeEnum.Unknown.ToString())
                {
                    definitions.AddSimple(i => (Guid?)null, o => o.WcagLevelTypeId);
                }
            }

            if (!string.IsNullOrEmpty(vModel.WCAGLevel))
            {
                definitions.AddSimple(i => typesCache.Get<WcagLevelType>(vModel.WCAGLevel), o => o.WcagLevelTypeId);
            }

            if (vModel.AccessibilityStatementWebPage?.Count > 0)
            {
                definitions.AddCollectionWithRemove(i => i.AccessibilityStatementWebPage, o => o.WebPages, x => true);
            }

            return definitions.GetFinal();
        }
    }
}
