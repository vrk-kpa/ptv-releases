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
using PTV.Domain.Model.Models.OpenApi.V2;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi.V3
{
    /// <summary>
    /// OPEN API V3 - View model interface of phone channel for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiPhoneChannelInVersionBase" />
    public interface IV3VmOpenApiPhoneChannelInBase : IVmOpenApiPhoneChannelInVersionBase
    {
        /// <summary>
        /// List of phone numbers for the service channel.
        /// </summary>
        new IList<VmOpenApiPhoneWithType> PhoneNumbers { get; set; }
        /// <summary>
        /// List of service channel service hours.
        /// </summary>
        new IList<V2VmOpenApiServiceHour> ServiceHours { get; set; }
        /// <summary>
        /// Gets or sets the urls.
        /// </summary>
        /// <value>
        /// The urls.
        /// </value>
        new IList<VmOpenApiLanguageItem> Urls { get; set; }
    }
}
