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
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Framework;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi.V4
{
    /// <summary>
    /// OPEN API V4 - View Model of service hour
    /// </summary>
    public class V4VmOpenApiServiceHour : VmOpenApiServiceHourBase<V2VmOpenApiDailyOpeningTime>
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
        /// Converts model into latest version.
        /// </summary>
        /// <returns></returns>
        public V11VmOpenApiServiceHour ConvertToLatestVersion()
        {
            var vm = new V11VmOpenApiServiceHour
            {
                ServiceHourType = this.ServiceHourType,
                ValidFrom = this.ValidFrom,
                ValidTo = this.ValidTo,
                IsClosed = this.IsClosed,
                ValidForNow = this.ValidForNow,
                AdditionalInformation = this.AdditionalInformation,
                OrderNumber = this.OrderNumber,
            };
            // Set the order of opening hours according to isExtra column and also according the order of items. PTV-3575
            this.OpeningHour.Where(o => o.IsExtra == false).ForEach(o => o.Order = 0);
            var order = 1;
            this.OpeningHour.Where(o => o.IsExtra == true).ForEach(o => o.Order = order++);

            this.OpeningHour?.ForEach(o => vm.OpeningHour.Add(new V8VmOpenApiDailyOpeningTime
                {
                    DayFrom = o.DayFrom,
                    DayTo = o.DayTo,
                    From = o.From,
                    To = o.To,
                    Order = o.Order
                }));
            return vm;
        }

        #endregion
    }
}
