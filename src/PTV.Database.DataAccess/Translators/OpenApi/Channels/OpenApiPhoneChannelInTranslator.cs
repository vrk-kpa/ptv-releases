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

using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V9;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmOpenApiPhoneChannelInVersionBase>), RegisterType.Transient)]
    internal class OpenApiPhoneChannelInTranslator : OpenApiServiceChannelInTranslator<VmOpenApiPhoneChannelInVersionBase>
    {
        public OpenApiPhoneChannelInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {}

        public override ServiceChannelVersioned TranslateVmToEntity(VmOpenApiPhoneChannelInVersionBase vModel)
        {
            var order = 1;
            vModel.PhoneNumbers.ForEach(p => {
                p.OwnerReferenceId = vModel.VersionId;
                p.OrderNumber = order++;
            });

            var definitions = CreateVmToChannelDefinitions(vModel)
                .DisableAutoTranslation()
                .AddSimple(i => typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.Phone.ToString()), o => o.TypeId);

            if (vModel.PhoneNumbers?.Count > 0)
            {
                definitions.AddCollectionWithRemove(i => vModel.PhoneNumbers, o => o.Phones, x => true);
            }

            if (vModel.WebPage != null || vModel.DeleteAllWebPages)
            {
                if (vModel.WebPage == null)
                {
                    definitions.AddCollectionWithRemove(i => new List<V9VmOpenApiWebPage>(), o => o.WebPages, x => true);
                }
                else
                {
                    definitions.AddCollectionWithRemove(i => i.WebPage?.Where(j => !j.Value.IsNullOrWhitespace())
                        .Select(j => new V9VmOpenApiWebPage
                        {
                            Language = j.Language,
                            Url = j.Value,
                            OwnerReferenceId = j.OwnerReferenceId,
                            OwnerReferenceId2 = j.OwnerReferenceId2
                        }), o => o.WebPages, x => true);
                }
            }

            return definitions.GetFinal();
        }
    }
}
