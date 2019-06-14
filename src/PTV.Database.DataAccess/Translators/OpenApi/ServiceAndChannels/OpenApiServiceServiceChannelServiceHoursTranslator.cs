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
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;

namespace PTV.Database.DataAccess.Translators.OpenApi.ServiceAndChannels
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannelServiceHours, V8VmOpenApiServiceHour>), RegisterType.Transient)]
    internal class OpenApiServiceServiceChannelServiceHoursTranslator : Translator<ServiceServiceChannelServiceHours, V8VmOpenApiServiceHour>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceServiceChannelServiceHoursTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
                : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V8VmOpenApiServiceHour TranslateEntityToVm(ServiceServiceChannelServiceHours entity)
        {
            if (entity == null || entity.ServiceHours == null)
            {
                return null;
            }

            return CreateEntityViewModelDefinition(entity)
                .AddPartial(i => i.ServiceHours)
                .GetFinal();
        }

        public override ServiceServiceChannelServiceHours TranslateVmToEntity(V8VmOpenApiServiceHour vModel)
        {
            if (vModel == null)
            {
                return null;
            }

            // It is impossible to try to map service hours from model into existing service hours so we always add new rows. Remember to delete old ones in ChannelService!
            return CreateViewModelEntityDefinition<ServiceServiceChannelServiceHours>(vModel)
                .UseDataContextCreate(i => true)
                .AddNavigation(i => i, o => o.ServiceHours)
                .GetFinal();
        }
    }
}
