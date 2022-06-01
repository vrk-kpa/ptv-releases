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

using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Extensions;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi.V8
{
    /// <summary>
    /// OPEN API V8 - View Model of service hour
    /// </summary>
    public class V8VmOpenApiServiceHour : VmOpenApiServiceHourBase<V8VmOpenApiDailyOpeningTime>
    {
        /// <summary>
        /// Set to true to present a time between the valid from and to times as allways open.
        /// </summary>
        [JsonIgnore]
        public override bool IsAlwaysOpen { get => base.IsAlwaysOpen; set => base.IsAlwaysOpen = value; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open on reservation.
        /// </summary>
        [JsonIgnore]
        public override bool IsReservation => base.IsReservation;

        #region Methods

        /// <summary>
        /// Converts model into in base model.
        /// </summary>
        /// <returns></returns>
        public V11VmOpenApiServiceHour ConvertToInVersionBase()
        {
            var model = new V11VmOpenApiServiceHour
            {
                ServiceHourType = this.ServiceHourType,
                ValidFrom = this.ValidFrom,
                ValidTo = this.ValidTo,
                IsClosed = this.IsClosed,
                ValidForNow = this.ValidForNow,
                AdditionalInformation = this.AdditionalInformation,
                OpeningHour = this.OpeningHour,
                OrderNumber = this.OrderNumber,
            };

            if (model.OpeningHour?.Count > 0)
            {
                // The first opening hour for a day has always order number 0.
                var dailyOpeningHours = model.OpeningHour
                    .Where(h => !h.DayFrom.IsNullOrEmpty())
                    .GroupBy(o => o.DayFrom)
                    .ToDictionary(o => o.Key, o => o.ToList());
                dailyOpeningHours.ForEach(day =>
                {
                    var allHours = day.Value;
                    int order = 0;
                    allHours.ForEach(o => o.Order = order++);
                });
            }

            return model;
        }

        #endregion
    }
}
