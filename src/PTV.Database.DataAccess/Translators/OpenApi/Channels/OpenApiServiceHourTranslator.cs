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
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using PTV.Database.DataAccess.Caches;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelServiceHours, V2VmOpenApiServiceHour>), RegisterType.Transient)]
    internal class OpenApiServiceHourTranslator : Translator<ServiceChannelServiceHours, V2VmOpenApiServiceHour>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceHourTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V2VmOpenApiServiceHour TranslateEntityToVm(ServiceChannelServiceHours entity)
        {
            return CreateEntityViewModelDefinition<V2VmOpenApiServiceHour>(entity)
                .AddNavigation(i => typesCache.GetByValue<ServiceHourType>(i.ServiceHourTypeId), o => o.ServiceHourType)
                .AddSimple(i => i.ValidFrom, o => o.ValidFrom)
                .AddSimple(i => i.ValidTo, o => o.ValidTo)
                .AddSimple(i => i.IsClosed, o => o.IsClosed)
                .AddCollection(i => i.AdditionalInformations, o => o.AdditionalInformation)
                .AddCollection(i => i.DailyOpeningTimes, o => o.OpeningHour)
                .GetFinal();
        }

        public override ServiceChannelServiceHours TranslateVmToEntity(V2VmOpenApiServiceHour vModel)
        {
            // It is impossible to try to map service hours from model into existing service hours so we always add new rows. Remember to delete old ones in ChannelService!
            var definition = CreateViewModelEntityDefinition<ServiceChannelServiceHours>(vModel)
                .DisableAutoTranslation()
                .AddNavigation(i => i.ServiceHourType, o => o.ServiceHourType)
                .AddSimple(i => i.ValidFrom, o => o.ValidFrom)
                .AddSimple(i => i.ValidTo, o => o.ValidTo)
                .AddSimple(i => i.IsClosed, o => o.IsClosed);

            if (vModel.OwnerReferenceId.IsAssigned())
            {
                var entity = definition.GetFinal();
                if (entity.Created != DateTime.MinValue)
                {
                    // We are updating existing item
                    vModel.AdditionalInformation.ForEach(a => a.OwnerReferenceId = entity.Id);
                    vModel.OpeningHour.ForEach(o => o.OwnerReferenceId = entity.Id);
                }
            }

            if (vModel.AdditionalInformation != null && vModel.AdditionalInformation.Count > 0)
            {
                definition.AddCollection(i => i.AdditionalInformation, o => o.AdditionalInformations);
            }

            if (vModel.OpeningHour?.Count > 0)
            {
                definition.AddCollection(i => i.OpeningHour, o => o.DailyOpeningTimes);
            }

            return definition.GetFinal();
        }
    }
}
