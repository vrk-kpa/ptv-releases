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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>), RegisterType.Transient)]
    internal class OpenApiPhoneChannelTranslator : OpenApiServiceChannelBaseTranslator<VmOpenApiPhoneChannelVersionBase>
    {
        private readonly ITypesCache typesCache;

        public OpenApiPhoneChannelTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiPhoneChannelVersionBase TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var phone = entity.Phones?.FirstOrDefault()?.Phone;
            if (phone == null)
            {
                return base.TranslateEntityToVm(entity);
            }

            var typeCode = typesCache.GetByValue<PhoneNumberType>(phone.TypeId);

            return CreateChannelDefinitions(entity)
                .AddCollection(
                    i => i.Phones.Select(p => p.Phone).OrderByDescending(p => p.LocalizationId)
                        .ThenBy(p => p.OrderNumber), o => o.PhoneNumbers)
                .AddCollection(
                    i => i.WebPages.OrderByDescending(w => w.LocalizationId)
                        .ThenBy(w => w.OrderNumber), o => o.WebPages)
                .GetFinal();
        }

    }
}
