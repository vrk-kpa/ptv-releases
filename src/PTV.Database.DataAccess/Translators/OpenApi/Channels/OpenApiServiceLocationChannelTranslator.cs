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

using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V2;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannel, IVmOpenApiServiceLocationChannel>), RegisterType.Transient)]
    [RegisterService(typeof(ITranslator<ServiceChannel, V2VmOpenApiServiceLocationChannel>), RegisterType.Transient)]
    internal class OpenApiServiceLocationChannelTranslator : OpenApiServiceChannelTranslator<V2VmOpenApiServiceLocationChannel>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public OpenApiServiceLocationChannelTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override V2VmOpenApiServiceLocationChannel TranslateEntityToVm(ServiceChannel entity)
        {
            if (entity.ServiceLocationChannels?.FirstOrDefault() == null)
            {
                return base.TranslateEntityToVm(entity);
            }

            var definitions = CreateChannelDefinitions(entity)
                .AddSimple(i => i.ServiceLocationChannels.SafePropertyFromFirst(j => j.ServiceAreaRestricted), o => o.ServiceAreaRestricted)
                .AddCollection(i => i.Phones, o => o.PhoneNumbers)
                .AddSimple(i => i.ServiceLocationChannels?.FirstOrDefault()?.PhoneServiceCharge ?? false, o => o.PhoneServiceCharge)
                .AddCollection(i => i.ServiceLocationChannels?.FirstOrDefault()?.ServiceAreas.Select(m => m.Municipality.Name).ToList(), o => o.ServiceAreas)
                .AddCollection(i => i.Languages.Select(j => j.Language.Code)?.ToList(), o => o.Languages)
                .AddCollection(i => i.ServiceLocationChannels?.FirstOrDefault()?.Addresses, o => o.Addresses);

            return definitions.GetFinal();
        }
    }
}
