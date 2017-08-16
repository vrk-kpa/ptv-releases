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
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.ServiceAndChannels
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannel, V7VmOpenApiServiceServiceChannel>), RegisterType.Transient)]
    internal class OpenApiServiceServiceChannelTranslator : Translator<ServiceServiceChannel, V7VmOpenApiServiceServiceChannel>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceServiceChannelTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V7VmOpenApiServiceServiceChannel TranslateEntityToVm(ServiceServiceChannel entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .AddCollection(i => i.ServiceServiceChannelDescriptions, o => o.Description)
                .AddNavigation(i => i.ServiceChargeTypeId.HasValue ? typesCache.GetByValue<ServiceChargeType>(i.ServiceChargeTypeId.Value) : null, o => o.ServiceChargeType)
                .AddCollection(i => i.ServiceServiceChannelDigitalAuthorizations?.Select(d => d.DigitalAuthorization).ToList(), o => o.DigitalAuthorizations);
                        
            if (entity.ServiceChannel != null)
            {
                var publishedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                definition.AddNavigation(i => i.ServiceChannel.Versions.Where(x => x.PublishingStatusId == publishedId).FirstOrDefault(), o => o.ServiceChannel);
            }
            else
            {
                var channel = new ServiceChannelVersioned() { UnificRootId = entity.ServiceChannelId };
                definition.AddNavigation(i => channel, o => o.ServiceChannel);
            }

            return definition.GetFinal();
        }

        public override ServiceServiceChannel TranslateVmToEntity(V7VmOpenApiServiceServiceChannel vModel)
        {
            throw new NotImplementedException();
        }
    }
}
