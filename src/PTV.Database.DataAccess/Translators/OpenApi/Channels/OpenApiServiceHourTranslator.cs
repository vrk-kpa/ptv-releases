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
using System.Globalization;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelServiceHours, V4VmOpenApiServiceHour>), RegisterType.Transient)]
    internal class OpenApiServiceHourTranslator : Translator<ServiceChannelServiceHours, V4VmOpenApiServiceHour>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceHourTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V4VmOpenApiServiceHour TranslateEntityToVm(ServiceChannelServiceHours entity)
        {
            var vm = CreateEntityViewModelDefinition<V4VmOpenApiServiceHour>(entity)
                .AddNavigation(i => typesCache.GetByValue<ServiceHourType>(i.ServiceHourTypeId), o => o.ServiceHourType)
                .AddSimple(i => i.OpeningHoursFrom.HasValue ? DateTime.SpecifyKind(i.OpeningHoursFrom.Value, DateTimeKind.Utc).ToLocalTime() : i.OpeningHoursFrom, o => o.ValidFrom)
                .AddSimple(i => i.OpeningHoursTo.HasValue ? DateTime.SpecifyKind(i.OpeningHoursTo.Value, DateTimeKind.Utc).ToLocalTime() : i.OpeningHoursTo, o => o.ValidTo)
                .AddSimple(i => i.IsClosed, o => o.IsClosed)
                .AddCollection(i => i.AdditionalInformations, o => o.AdditionalInformation)
                .AddCollection(i => i.DailyOpeningTimes, o => o.OpeningHour)
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
                .GetFinal();

            return CheckServiceHours(vm);
        }

        public override ServiceChannelServiceHours TranslateVmToEntity(V4VmOpenApiServiceHour vModel)
        {
            // It is impossible to try to map service hours from model into existing service hours so we always add new rows. Remember to delete old ones in ChannelService!
            var definition = CreateViewModelEntityDefinition<ServiceChannelServiceHours>(vModel)
                .UseDataContextCreate(i => true, o => o.Id, i => Guid.NewGuid())
                .DisableAutoTranslation()
                .AddSimple(i => typesCache.Get<ServiceHourType>(i.ServiceHourType), o => o.ServiceHourTypeId)
                .AddSimple(i => i.ValidFrom.HasValue ? i.ValidFrom.Value.ToUniversalTime() : DateTime.Now.ToUniversalTime(), o => o.OpeningHoursFrom)
                .AddSimple(i => i.ValidTo.HasValue ? i.ValidTo.Value.ToUniversalTime() : i.ValidTo, o => o.OpeningHoursTo)
                .AddSimple(i => i.IsClosed, o => o.IsClosed)
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber);

            if (vModel.AdditionalInformation != null && vModel.AdditionalInformation.Count > 0)
            {
                definition.AddCollection(i => i.AdditionalInformation, o => o.AdditionalInformations);
            }

            if (vModel.OpeningHour?.Count > 0)
            {
                // For Exception date DayFrom is optional in model but mandatory in database
                if (vModel.ServiceHourType == ServiceHoursTypeEnum.Exception.ToString())
                {
                    // Set default values
                    vModel.OpeningHour.ForEach(h =>  {
                        h.DayFrom = WeekDayEnum.Monday.ToString();
                        h.DayTo = WeekDayEnum.Sunday.ToString();
                    });
                }

                definition.AddCollection(i => i.OpeningHour, o => o.DailyOpeningTimes);
            }

            return definition.GetFinal();
        }

        private V4VmOpenApiServiceHour CheckServiceHours(V4VmOpenApiServiceHour hours)
        {
            var hourType = ServiceHoursTypeEnum.Standard;
            Enum.TryParse(hours.ServiceHourType, out hourType);
            switch (hourType)
            {
                case ServiceHoursTypeEnum.Standard:
                    // If ValidTo field is null opening hour is valid for now
                    if (!hours.ValidTo.HasValue)
                    {
                        hours.ValidFrom = null;
                        hours.ValidForNow = true;
                    }
                    break;
                case ServiceHoursTypeEnum.Exception:
                    // Set DayFrom and DayTo fields as null - in database these fields are not set correctly!
                    hours.OpeningHour.ForEach(h => { h.DayFrom = null; h.DayTo = null; });
                    break;
                case ServiceHoursTypeEnum.Special:
                    // If ValidTo field is null opening hour is valid for now
                    if (!hours.ValidTo.HasValue)
                    {
                        hours.ValidFrom = null;
                        hours.ValidForNow = true;
                    }
                    break;
            }

            return hours;
        }
    }
}
