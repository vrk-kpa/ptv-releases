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
using System;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Enums;
using System.Linq;
using PTV.Framework.Extensions;
using PTV.Domain.Model.Models.OpenApi.V11;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelServiceHours, V11VmOpenApiServiceHour>), RegisterType.Transient)]
    internal class OpenApiServiceChannelServiceHourTranslator : Translator<ServiceChannelServiceHours, V11VmOpenApiServiceHour>
    {

        public OpenApiServiceChannelServiceHourTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override V11VmOpenApiServiceHour TranslateEntityToVm(ServiceChannelServiceHours entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceChannelServiceHours TranslateVmToEntity(V11VmOpenApiServiceHour vModel)
        {
            if (vModel == null)
            {
                return null;
            }

            // It is impossible to try to map service hours from model into existing service hours so we always add new rows. Remember to delete old ones in ChannelService!
            return CreateViewModelEntityDefinition<ServiceChannelServiceHours>(vModel)
                .UseDataContextCreate(i => true)
                .AddNavigation(i => i, o => o.ServiceHours)
                .GetFinal();
        }
    }
    [RegisterService(typeof(ITranslator<ServiceHours, V11VmOpenApiServiceHour>), RegisterType.Transient)]
    internal class OpenApiServiceHourTranslator : Translator<ServiceHours, V11VmOpenApiServiceHour>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceHourTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V11VmOpenApiServiceHour TranslateEntityToVm(ServiceHours entity)
        {
            var type = typesCache.GetByValue<ServiceHourType>(entity.ServiceHourTypeId);

            // Let's not return service hours that are already expired. PTV-3067
            if (entity.HasExpired(typesCache)) return null;

            var definition = CreateEntityViewModelDefinition<V11VmOpenApiServiceHour>(entity)
                // Type terms changed for open api (PTV-2184)
                .AddNavigation(
                    i => string.IsNullOrEmpty(type) ? null : type.GetOpenApiEnumValue<ServiceHoursTypeEnum>(),
                    o => o.ServiceHourType)
                .AddSimple(i => i.IsClosed, o => o.IsClosed)
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
                .AddSimple(i => i.IsNonStop, o => o.IsAlwaysOpen)
                .AddSimple(i => i.IsReservation, o => o.IsReservation);

            // handle holidays (SFIPTV-1822)
            if (entity.HolidayServiceHour == null)
            {
                // Opening hours are not anymore saved as UTC in db (PTV-4132)
                definition.AddSimple(i => i.OpeningHoursFrom, o => o.ValidFrom);
                definition.AddSimple(i => i.OpeningHoursTo, o => o.ValidTo);
                definition.AddCollection(i => i.AdditionalInformations, o => o.AdditionalInformation);
            }
            else
            {
                definition.AddSimple(i => GetNextHolidayDate(entity.HolidayServiceHour.Holiday), o => o.ValidFrom);
                definition.AddCollection(i =>  entity.HolidayServiceHour.Holiday?.Names?.Select(n => new ServiceHoursAdditionalInformation
                {
                    LocalizationId = n.LocalizationId,
                    Text = n.Name,
                }), o => o.AdditionalInformation);
            }

            if (!entity.IsNonStop && !entity.IsReservation)
            {
                definition.AddCollection(i => i.DailyOpeningTimes.OrderBy(x => x.DayFrom).ThenBy(x => x.OrderNumber),
                    o => o.OpeningHour);
            }

            var vm = definition.GetFinal();
            vm.FixServiceHourData();
            return vm;
        }

        public override ServiceHours TranslateVmToEntity(V11VmOpenApiServiceHour vModel)
        {
            // It is impossible to try to map service hours from model into existing service hours so we always add new rows.
            var serviceHourType = string.IsNullOrEmpty(vModel.ServiceHourType) ? ServiceHoursTypeEnum.Standard.ToString() : vModel.ServiceHourType.GetEnumValueByOpenApiEnumValue<ServiceHoursTypeEnum>();
            var definition = CreateViewModelEntityDefinition<ServiceHours>(vModel)
                .UseDataContextCreate(i => true, o => o.Id, i => Guid.NewGuid())
                .DisableAutoTranslation()
                .AddSimple(i => typesCache.Get<ServiceHourType>(serviceHourType), o => o.ServiceHourTypeId)
                // Opening hours are not anymore saved as UTC in db (PTV-4132)
                .AddSimple(i => i.ValidFrom.HasValue ? i.ValidFrom.Value : DateTime.Now, o => o.OpeningHoursFrom)
                .AddSimple(i => i.ValidTo, o => o.OpeningHoursTo)
                .AddSimple(i => i.IsClosed, o => o.IsClosed)
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
                .AddSimple(i => i.IsAlwaysOpen, o => o.IsNonStop)
                .AddSimple(i => i.IsReservation, o => o.IsReservation);

            if (vModel.AdditionalInformation != null && vModel.AdditionalInformation.Count > 0)
            {
                definition.AddCollection(i => i.AdditionalInformation, o => o.AdditionalInformations);
            }

            // For Exception date DayFrom is optional in model but mandatory in database.
            // Also, in UI DayFrom and DayTo are not used for Exception type OpeningHours,
            // and only one OpeningHour is allowed per Exception.
            if (serviceHourType == ServiceHoursTypeEnum.Exception.ToString())
            {
                // Set default values
                vModel.OpeningHour.ForEach(h => {
                    h.DayFrom = WeekDayEnum.Monday.ToString();
                    h.DayTo = WeekDayEnum.Sunday.ToString();
                });
            }

            definition.AddCollection(i => i.OpeningHour, o => o.DailyOpeningTimes);

            return definition.GetFinal();
        }

        private static DateTime? GetNextHolidayDate(Holiday holiday)
        {
            return holiday?.HolidayDates?.OrderBy(date => date.Date.Date).FirstOrDefault(date => date.Date.Date >= DateTime.Now.Date)?.Date;
        }
    }
}
