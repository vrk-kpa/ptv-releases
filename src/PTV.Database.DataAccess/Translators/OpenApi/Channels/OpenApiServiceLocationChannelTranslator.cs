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

using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V7;
using System.Collections.Generic;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>), RegisterType.Transient)]
    internal class OpenApiServiceLocationChannelTranslator : OpenApiServiceChannelBaseTranslator<VmOpenApiServiceLocationChannelVersionBase>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public OpenApiServiceLocationChannelTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiServiceLocationChannelVersionBase TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var serviceLocation = entity.ServiceLocationChannels?.FirstOrDefault();
            if (serviceLocation == null)
            {
                return base.TranslateEntityToVm(entity);
            }

            var definitions = CreateChannelDefinitions(entity)
                .AddCollection(i => i.Phones.Select(p => p.Phone).OrderBy(p => p.OrderNumber), o => o.PhoneNumbers)
                .AddCollection(i => serviceLocation.Addresses.OrderBy(x => x.CharacterId).ThenBy(x => x.Address.OrderNumber).ThenBy(x => x.Address.Modified), o => o.Addresses)
                .AddSimple(i => serviceLocation.Id, o => o.ChannelId);

            var vm = definitions.GetFinal();

            // Organize moving addresses - we need to group moving addresses into one address (address.MovingAddresses includes all the moving addresses).
            if (vm.Addresses?.Count > 0)
            {
                V7VmOpenApiAddressWithMoving movingAddress = null;
                var addresses = new List<V7VmOpenApiAddressWithMoving>();
                vm.Addresses.ForEach(a =>
                {
                    if (a.SubType == AddressTypeEnum.Moving.ToString())
                    {
                        if (movingAddress != null)
                        {
                            a.MultipointLocation.ForEach(m => movingAddress.MultipointLocation.Add(m));
                        }
                        else
                        {
                            movingAddress = a;
                            addresses.Add(movingAddress);
                        }
                    }
                    else
                    {
                        addresses.Add(a);
                    }
                });
                vm.Addresses = addresses;
            }

            return vm;
        }
    }
}
