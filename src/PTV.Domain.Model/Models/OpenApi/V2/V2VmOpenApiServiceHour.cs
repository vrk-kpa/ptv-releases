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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.Extensions;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    /// <summary>
    /// OPEN API V2 - View Model of service hours
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceHourBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V2.IV2VmOpenApiServiceHour" />
    public class V2VmOpenApiServiceHour : VmOpenApiServiceHourBase, IV2VmOpenApiServiceHour
    {
        /// <summary>
        /// Localized list of additional information.
        /// </summary>
        public new IList<VmOpenApiLanguageItem> AdditionalInformation { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// Set to true to present that this entry is valid for now.
        /// </summary>
        [JsonIgnore]
        public override bool ValidForNow
        {
            get
            {
                return base.ValidForNow;
            }

            set
            {
                base.ValidForNow = value;
            }
        }

        /// <summary>
        /// Converts to version1.
        /// </summary>
        /// <returns>converted model to version 1</returns>
        public List<VmOpenApiServiceHour> ConvertToVersion1()
        {
            var list = new List<VmOpenApiServiceHour>();
            OpeningHour.ForEach(o =>
            {
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

        /// <summary>
        /// Converts to version4.
        /// </summary>
        /// <returns>converted model to the version 4</returns>
        public V4VmOpenApiServiceHour ConvertToVersion4()
        {
            var serviceHour = ConvertBase<V4VmOpenApiServiceHour>();
            serviceHour.AdditionalInformation = this.AdditionalInformation.SetListValueLength(100);
            return serviceHour;
        }

        /// <summary>
        /// Determines whether it is valid day opening hours.
        /// </summary>
        /// <param name="day">The day.</param>
        /// <param name="dailyOpeningTime">The daily opening time.</param>
        /// <returns>
        ///   <c>true</c> if valid; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.Exception"></exception>
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
