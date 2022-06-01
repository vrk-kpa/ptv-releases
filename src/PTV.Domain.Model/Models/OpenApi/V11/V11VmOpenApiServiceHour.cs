﻿/**
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

using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Domain.Model.Models.OpenApi.V11
{
    /// <summary>
    /// OPEN API V11 - View Model of service hour
    /// </summary>
    public class V11VmOpenApiServiceHour : VmOpenApiServiceHourBase<V8VmOpenApiDailyOpeningTime>
    {
        #region methods

        /// <summary>
        /// Checks and sets the service hours for out api.
        /// </summary>
        public void FixServiceHourData()
        {
            if (ServiceHourType == ServiceHoursTypeEnum.Standard.GetOpenApiValue())
            {
                // If ValidTo field is null opening hour is valid for now
                if (!ValidTo.HasValue)
                {
                    ValidFrom = null;
                    ValidForNow = true;
                }
                // Set DayTo as empty. In db DayTo is always 'Monday' - not very logical for end user since the time is supposed to be valid on this certain week day. PTV-3455
                OpeningHour.ForEach(h => { h.DayTo = ""; });
                return;
            }
            if (ServiceHourType == ServiceHoursTypeEnum.Special.GetOpenApiValue())
            {
                // If ValidTo field is null opening hour is valid for now
                if (!ValidTo.HasValue)
                {
                    ValidFrom = null;
                    ValidForNow = true;
                }
                return;
            }
        }

        /// <summary>
        /// Converts model to previous version.
        /// </summary>
        public V8VmOpenApiServiceHour ConvertToPreviousVersion()
        {
            return new V8VmOpenApiServiceHour
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
        }

        #endregion
    }
}
