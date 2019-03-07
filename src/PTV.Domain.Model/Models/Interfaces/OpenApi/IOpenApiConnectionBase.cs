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

using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - Interface for connection
    /// </summary>
    public interface IOpenApiConnectionBase<TModelContact, TModelServiceHours, TModelOpeningTime>
        where TModelContact : IVmOpenApiContactDetailsInVersionBase
        where TModelServiceHours : IVmOpenApiServiceHourBase<TModelOpeningTime>
        where TModelOpeningTime : IVmOpenApiDailyOpeningTime
    {
        /// <summary>
        /// Service charge type. Possible values are: Chargeable, FreeOfCharge or Other.
        /// In version 7 and older: Charged, Free or Other
        /// </summary>
        string ServiceChargeType { get; set; }
        /// <summary>
        /// Contact details
        /// </summary>
        TModelContact ContactDetails { get; set; }
        /// <summary>
        /// Service hours
        /// </summary>
        IList<TModelServiceHours> ServiceHours { get; set; }
    }
}
