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

using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V8;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>), RegisterType.Transient)]
    internal class OpenApiServiceLocationChannelTranslator : OpenApiServiceChannelBaseTranslator<VmOpenApiServiceLocationChannelVersionBase>
    {
        public OpenApiServiceLocationChannelTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
        }

        public override VmOpenApiServiceLocationChannelVersionBase TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var definitions = CreateChannelDefinitions(entity)
                .AddCollection(i => i.DisplayNameTypes, o => o.DisplayNameType)
                .AddNavigation(i => i.UnificRoot?.SocialHealthCenters?.FirstOrDefault(), o => o.Oid)
                .AddCollection(i => i.Phones.Select(p => p.Phone).OrderBy(p => p.TypeId).ThenByDescending(p => p.LocalizationId).ThenBy(p => p.OrderNumber), o => o.PhoneNumbers)
                .AddCollection(i => i.Addresses.OrderBy(x => x.CharacterId).ThenBy(x => x.Address.OrderNumber).ThenBy(x => x.Address.Modified), o => o.Addresses)
                .AddCollection(i => i.WebPages.OrderByDescending(w => w.LocalizationId).ThenBy(w => w.OrderNumber), o => o.WebPages);

            return definitions.GetFinal();
        }
    }
}
