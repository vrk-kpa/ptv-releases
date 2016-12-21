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


using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    public class V2VmOpenApiServiceHour : IV2VmOpenApiServiceHour
    {
        /// <summary>
        /// Type of service hour. Valid values are: Standard, Exception or Special.
        /// </summary>
        [ValidEnum(typeof(ServiceHoursTypeEnum))]
        [Required]
        public string ServiceHourType { get; set; }

        /// <summary>
        /// Date time where from this entry is valid.
        /// </summary>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Date time to this entry is valid.
        /// </summary>
        public DateTime? ValidTo { get; set; }

        /// <summary>
        /// Set to true to present a time between the valid from and to times as closed.
        /// </summary>
        public bool IsClosed { get; set; }

        /// <summary>
        /// Localized list of additional information.
        /// </summary>
        public IReadOnlyList<VmOpenApiLanguageItem> AdditionalInformation { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of servicing hours (open and closes times).
        /// </summary>
        public IReadOnlyList<V2VmOpenApiDailyOpeningTime> OpeningHour { get; set; } = new List<V2VmOpenApiDailyOpeningTime>();

        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }

        public List<VmOpenApiServiceHour> ConvertToVersion1()
        {
            var list = new List<VmOpenApiServiceHour>();
            OpeningHour.ForEach(o =>
            {
                //if (string.IsNullOrEmpty(o.DayTo))
                list.Add(new VmOpenApiServiceHour()
                {
                    ServiceHourType = this.ServiceHourType,
                    ValidFrom = this.ValidFrom,
                    ValidTo = this.ValidTo,
                    ExceptionHourType = this.IsClosed ? ExceptionHoursStatus.Closed.ToString() : ExceptionHoursStatus.Open.ToString(),
                    AdditionalInformation = this.AdditionalInformation,
                    Monday = IsValidForDay((int)WeekDayEnum.Monday, o),
                    Tuesday = IsValidForDay((int)WeekDayEnum.Tuesday, o),
                    Wednesday = IsValidForDay((int)WeekDayEnum.Wednesday, o),
                    Thursday = IsValidForDay((int)WeekDayEnum.Thursday, o),
                    Friday = IsValidForDay((int)WeekDayEnum.Friday, o),
                    Saturday = IsValidForDay((int)WeekDayEnum.Saturday, o),
                    Sunday = IsValidForDay((int)WeekDayEnum.Sunday, o),
                    Opens = o.From.ToString(),
                    Closes = o.To.ToString(),
                });
            });
            return list;
        }

        private bool IsValidForDay(int day, V2VmOpenApiDailyOpeningTime dailyOpeningTime)
        {
            int dayFrom = GetWeekDayNumber(dailyOpeningTime.DayFrom);
            int dayTo = GetWeekDayNumber(dailyOpeningTime.DayTo);

            if (dayFrom < 0)
            {
                throw new Exception($"DayFrom field is invalid: { dailyOpeningTime.DayFrom }.");
            }

            if (dayTo < 0)
            {   // There was no value in DayTo field so we only need to match the DayFrom field
                return day == dayFrom;
            }
            else
            {   // The opening time is related to certain time frame ( DayForm - DayTo ).
                if (day >= dayFrom && day <= dayTo)
                    return true;

                return false;
            }
        }

        private int GetWeekDayNumber(string strDay)
        {
            if (string.IsNullOrEmpty(strDay))
                return -1;

            return (int)((WeekDayEnum)Enum.Parse(typeof(WeekDayEnum), strDay));
        }
    }
}
