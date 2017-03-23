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
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi.V1
{
    /// <summary>
    /// OPEN API - View model interface of phone channel for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiPhoneChannelInVersionBase" />
    public interface IVmOpenApiPhoneChannelInBase : IVmOpenApiPhoneChannelInVersionBase
    {
        /// <summary>
        /// Gets or sets the type of the phone.
        /// </summary>
        /// <value>
        /// The type of the phone.
        /// </value>
        string PhoneType { get; set; }

        /// <summary>
        /// Gets or sets the service charge types.
        /// </summary>
        /// <value>
        /// The service charge types.
        /// </value>
        IReadOnlyList<string> ServiceChargeTypes { get; set; }

        /// <summary>
        /// Gets or sets the phone numbers.
        /// </summary>
        /// <value>
        /// The phone numbers.
        /// </value>
        new IReadOnlyList<VmOpenApiLanguageItem> PhoneNumbers { get; set; }

        /// <summary>
        /// Gets or sets the urls.
        /// </summary>
        /// <value>
        /// The urls.
        /// </value>
        new IList<VmOpenApiLanguageItem> Urls { get; set; }

        /// <summary>
        /// Gets or sets the phone charge description.
        /// </summary>
        /// <value>
        /// The phone charge description.
        /// </value>
        string PhoneChargeDescription { get; set; }

        /// <summary>
        /// Gets or sets the support contact email.
        /// </summary>
        /// <value>
        /// The support contact email.
        /// </value>
        string SupportContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the service hours.
        /// </summary>
        /// <value>
        /// The service hours.
        /// </value>
        new IList<VmOpenApiServiceHour> ServiceHours { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all service charge types should be deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all service charge types should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllServiceChargeTypes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all support contacts should be deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all support contacts should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllSupportContacts { get; set; }
    }
}
