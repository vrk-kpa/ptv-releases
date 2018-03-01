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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.ServiceAndChannels
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmOpenApiContactDetails>), RegisterType.Transient)]
    internal class OpenApiContactDetailsTranslator : Translator<ServiceServiceChannel, VmOpenApiContactDetails>
    {
        public OpenApiContactDetailsTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiContactDetails TranslateEntityToVm(ServiceServiceChannel entity)
        {
            var vm = CreateEntityViewModelDefinition(entity)
                .AddCollection(i => i.ServiceServiceChannelAddresses, o => o.Addresses)
                .AddCollection(i => i.ServiceServiceChannelEmails?.Select(e => e.Email).ToList(), o => o.Emails)
                .AddCollection(i => i.ServiceServiceChannelPhones?.Select(p => p.Phone).ToList(), o => o.PhoneNumbers)
                .AddCollection(i => i.ServiceServiceChannelWebPages?.Select(w => w.WebPage).ToList(), o => o.WebPages)
                .GetFinal();

            if (vm != null && (vm.Addresses?.Count > 0 || vm.Emails?.Count > 0 || vm.PhoneNumbers?.Count > 0 || vm.WebPages?.Count > 0))
            {
                return vm;
            }

            return null;
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmOpenApiContactDetails vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceServiceChannelAddress, V7VmOpenApiAddress>), RegisterType.Transient)]
    internal class OpenApiContactDetailsAddressTranslator : Translator<ServiceServiceChannelAddress, V7VmOpenApiAddress>
    {
        private readonly ITypesCache typesCache;

        public OpenApiContactDetailsAddressTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V7VmOpenApiAddress TranslateEntityToVm(ServiceServiceChannelAddress entity)
        {
            return CreateEntityViewModelDefinition<V7VmOpenApiAddress>(entity)
                .AddPartial(i => i.Address)
                .AddNavigation(i => typesCache.GetByValue<AddressCharacter>(i.CharacterId), o => o.Type)
                .GetFinal();
        }

        public override ServiceServiceChannelAddress TranslateVmToEntity(V7VmOpenApiAddress vModel)
        {
            throw new NotImplementedException();
        }
    }
}
