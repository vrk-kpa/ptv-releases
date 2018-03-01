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
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;

namespace PTV.Database.DataAccess
{
    internal static class EntityExtensions
    {
        /// <summary>
        /// Returns true if service hour is not valid anymore (happens in past).
        /// </summary>
        /// <param name="hours"></param>
        /// <param name="type"></param>
        /// <returns>true/false</returns>
        public static bool HasExpired(this ServiceHours hours, ITypesCache typesCache)
        {
            var now = DateTime.Today;

            // Service hour is expired if valid to field is in past.
            if (hours.OpeningHoursTo.HasValue)
            {
                var to = DateTime.SpecifyKind(hours.OpeningHoursTo.Value, DateTimeKind.Utc).ToLocalTime();
                if (to < now) return true;
            }
            DateTime? from = hours.OpeningHoursFrom.HasValue ? DateTime.SpecifyKind(hours.OpeningHoursFrom.Value, DateTimeKind.Utc).ToLocalTime()
                : hours.OpeningHoursFrom;

            // Check exception hour that is valid only for one single day.
            var type = typesCache.GetByValue<ServiceHourType>(hours.ServiceHourTypeId);
            if (type == ServiceHoursTypeEnum.Exception.ToString() && !hours.OpeningHoursTo.HasValue && hours.OpeningHoursFrom.HasValue)
            {
                var day = DateTime.SpecifyKind(hours.OpeningHoursFrom.Value, DateTimeKind.Utc).ToLocalTime();
                if (day < now) return true;
            }

            return false;
        }

        public static V4VmOpenApiServiceHour GetOpenApiModel(this ServiceHours hours, ITypesCache typesCache, ILanguageCache languageCache)
        {
            if (hours.HasExpired(typesCache)) return null;
            
            IList<VmOpenApiLanguageItem> aiList = new List<VmOpenApiLanguageItem>();
            if (hours.AdditionalInformations?.Count > 0)
            {
                hours.AdditionalInformations.ForEach(info =>
                {
                    aiList.Add(new VmOpenApiLanguageItem { Value = string.IsNullOrEmpty(info.Text) ? null : info.Text, Language = languageCache.GetByValue(info.LocalizationId) });
                });
            }
            IList<V2VmOpenApiDailyOpeningTime> times = new List<V2VmOpenApiDailyOpeningTime>();
            if (hours.DailyOpeningTimes?.Count > 0)
            {
                hours.DailyOpeningTimes.ForEach(da =>
                {
                    times.Add(new V2VmOpenApiDailyOpeningTime
                    {
                        DayFrom = ((WeekDayEnum)da.DayFrom).ToString(),
                        DayTo = da.DayTo.HasValue ? ((WeekDayEnum)da.DayTo).ToString() : string.Empty,
                        From = da.From.ToString(),
                        To = da.To.ConvertToString()
                    });
                });
            }

            // ServiceHourType values changed into new ones (PTV-2184)
            var type = typesCache.GetByValue<ServiceHourType>(hours.ServiceHourTypeId);

            var vm = new V4VmOpenApiServiceHour
            {
                ServiceHourType = string.IsNullOrEmpty(type) ? null : type.GetOpenApiEnumValue<ServiceHoursTypeEnum>(),
                ValidFrom = hours.OpeningHoursFrom.HasValue ? DateTime.SpecifyKind(hours.OpeningHoursFrom.Value, DateTimeKind.Utc).ToLocalTime() : hours.OpeningHoursFrom,
                ValidTo = hours.OpeningHoursTo.HasValue ? DateTime.SpecifyKind(hours.OpeningHoursTo.Value, DateTimeKind.Utc).ToLocalTime() : hours.OpeningHoursTo,
                IsClosed = hours.IsClosed,
                AdditionalInformation = aiList,
                OpeningHour = times,
                OrderNumber = hours.OrderNumber
            };
            vm.CheckServiceHour();
            
            return vm;
        }

        public static VmOpenApiLanguageItem GetOpenApiModel(this Email email, ILanguageCache languageCache)
        {
            return new VmOpenApiLanguageItem
            {
                Value = string.IsNullOrEmpty(email.Value) ? null : email.Value,
                Language = languageCache.GetByValue(email.LocalizationId)
            };
        }

        public static V4VmOpenApiPhone GetOpenApiModel(this Phone phone, ITypesCache typesCache, ILanguageCache languageCache)
        {
            return new V4VmOpenApiPhone
            {
                Number = phone.Number,
                PrefixNumber = phone.PrefixNumber?.Code,
                AdditionalInformation = phone.AdditionalInformation,
                ChargeDescription = phone.ChargeDescription,
                // Service charge type enum value has been renamed (PTV-2184)
                ServiceChargeType = typesCache.GetByValue<ServiceChargeType>(phone.ChargeTypeId).GetOpenApiEnumValue<ServiceChargeTypeEnum>(),
                Language = languageCache.GetByValue(phone.LocalizationId),
                IsFinnishServiceNumber = string.IsNullOrEmpty(phone.PrefixNumber?.Code) ? true : false,
                OrderNumber = phone.OrderNumber
            };
        }

        public static VmOpenApiWebPageWithOrderNumber GetOpenApiModel(this WebPage wp, ILanguageCache languageCache)
        {
            return new VmOpenApiWebPageWithOrderNumber
            {
                Id = wp.Id,
                Value = wp.Name,
                Url = wp.Url,
                Language = languageCache.GetByValue(wp.LocalizationId),
                OrderNumber = wp.OrderNumber.HasValue ? wp.OrderNumber.Value.ToString() : null,
            };
        }

        public static V4VmOpenApiFintoItem GetOpenApiModel(this DigitalAuthorization da, ILanguageCache languageCache)
        {
            var names = new List<VmOpenApiLanguageItem>();
            if (da.Names?.Count > 0)
            {
                da.Names.ForEach(n => names.Add(new VmOpenApiLanguageItem { Value = n.Name, Language = languageCache.GetByValue(n.LocalizationId) }));
            }
            
            return new V4VmOpenApiFintoItem
            {
                Id = da.Id,
                Name = names,
                Code = da.Code,
                OntologyType = da.OntologyType,
                Uri = da.Uri,
                ParentUri = da.ParentUri,
                ParentId = da.ParentId
            };
        }
    }
}
