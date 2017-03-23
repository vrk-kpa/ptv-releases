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
using PTV.Domain.Model.Models.OpenApi.V2;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi.V2
{
    /// <summary>
    /// OPEN API V2 - View model interface of service location channel for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceLocationChannelInVersionBase" />
    public interface IV2VmOpenApiServiceLocationChannelInBase : IVmOpenApiServiceLocationChannelInVersionBase
    {
        /// <summary>
        /// Gets or sets the support emails.
        /// </summary>
        /// <value>
        /// The support emails.
        /// </value>
        new IList<VmOpenApiLanguageItem> SupportEmails { get; set; }
        /// <summary>
        /// Service location contact fax numbers.
        /// </summary>
        new IList<VmOpenApiPhoneSimple> FaxNumbers { get; set; }
        /// <summary>
        /// List of phone numbers for the service channel. Includes also fax numbers.
        /// </summary>
        new IList<VmOpenApiPhone> PhoneNumbers { get; set; }
        /// <summary>
        /// Gets or sets the web pages.
        /// </summary>
        /// <value>
        /// The web pages.
        /// </value>
        new IList<VmOpenApiWebPage> WebPages { get; set; }
        /// <summary>
        /// Gets or sets the addresses.
        /// </summary>
        /// <value>
        /// The addresses.
        /// </value>
        new IList<V2VmOpenApiAddressWithType> Addresses { get; set; }
    }
}
