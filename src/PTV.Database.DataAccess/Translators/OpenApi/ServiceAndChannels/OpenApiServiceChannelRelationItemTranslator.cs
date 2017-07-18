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
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;

namespace PTV.Database.DataAccess.Translators.OpenApi.ServiceAndChannels
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannel, V5VmOpenApiServiceServiceChannelInBase>), RegisterType.Transient)]
    internal class OpenApiServiceChannelRelationItemTranslator : Translator<ServiceServiceChannel, V5VmOpenApiServiceServiceChannelInBase>
    {
        ITypesCache typeCache;
        public OpenApiServiceChannelRelationItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typeCache = cacheManager.TypesCache;
        }

        public override V5VmOpenApiServiceServiceChannelInBase TranslateEntityToVm(ServiceServiceChannel entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceServiceChannel TranslateVmToEntity(V5VmOpenApiServiceServiceChannelInBase vModel)
        {
            Guid channelId = vModel.ChannelGuid.IsAssigned() ? vModel.ChannelGuid : Guid.Parse(vModel.ServiceChannelId);
            Guid? typeId = null;
            if(!vModel.ServiceChargeType.IsNullOrEmpty()) typeId = typeCache.Get<ServiceChargeType>(vModel.ServiceChargeType);

            vModel.Description.ForEach(d =>
            {
                d.OwnerReferenceId = vModel.ServiceGuid;
                d.OwnerReferenceId2 = channelId;
            });

            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(i => true, i => o => channelId == o.ServiceChannelId && i.ServiceGuid == o.ServiceId, def => def.UseDataContextCreate(i => true))
                .AddSimple(i => i.ServiceGuid, o => o.ServiceId)
                .AddSimple(i => channelId, o => o.ServiceChannelId)
                .AddSimple(i => typeId, o => o.ServiceChargeTypeId)
                .AddCollection(i => i.Description, o => o.ServiceServiceChannelDescriptions, false)
                .GetFinal();
        }
    }
}
