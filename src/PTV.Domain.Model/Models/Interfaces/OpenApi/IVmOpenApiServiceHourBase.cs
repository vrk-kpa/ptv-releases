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


using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V8;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of service hours - base
    /// </summary>
    public interface IVmOpenApiServiceHourBase<TModelOpeningTime> where TModelOpeningTime : IVmOpenApiDailyOpeningTime
    {
        /// <summary>
        /// Gets or sets the type of the service hour.
        /// </summary>
        /// <value>
        /// The type of the service hour.
        /// </value>
        string ServiceHourType { get; set; }
        /// <summary>
        /// Gets or sets the valid from.
        /// </summary>
        /// <value>
        /// The valid from.
        /// </value>
        DateTime? ValidFrom { get; set; }
        /// <summary>
        /// Gets or sets the valid to.
        /// </summary>
        /// <value>
        /// The valid to.
        /// </value>
        DateTime? ValidTo { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is closed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is closed; otherwise, <c>false</c>.
        /// </value>
        bool IsClosed { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether is valid for now.
        /// </summary>
        /// <value>
        ///   <c>true</c> if is valid for now; otherwise, <c>false</c>.
        /// </value>
        bool ValidForNow { get; set; }
        /// <summary>
        /// Gets or sets the additional information.
        /// </summary>
        /// <value>
        /// The additional information.
        /// </value>
        IList<VmOpenApiLanguageItem> AdditionalInformation { get; set; }
        /// <summary>
        /// Gets or sets the opening hour.
        /// </summary>
        /// <value>
        /// The opening hour.
        /// </value>
        IList<TModelOpeningTime> OpeningHour { get; set; }
        /// <summary>
        /// Gets or sets the order number
        /// </summary>
        /// <value>
        /// The order number.
        /// </value>
        int? OrderNumber { get; set; }
    }
}
