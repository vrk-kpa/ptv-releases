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
using PTV.Domain.Model.Models.OpenApi.V1;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi.V1
{
    /// <summary>
    /// OPEN API - View model interface of printable form channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiPrintableFormChannelVersionBase" />
    public interface IVmOpenApiPrintableFormChannel : IVmOpenApiPrintableFormChannelVersionBase
    {
        /// <summary>
        /// Gets or sets the form identifier.
        /// </summary>
        /// <value>
        /// The form identifier.
        /// </value>
        new string FormIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the form receiver.
        /// </summary>
        /// <value>
        /// The form receiver.
        /// </value>
        new string FormReceiver { get; set; }

        /// <summary>
        /// Gets or sets the delivery address.
        /// </summary>
        /// <value>
        /// The delivery address.
        /// </value>
        new VmOpenApiAddress DeliveryAddress { get; set; }

        /// <summary>
        /// Gets or sets the delivery address descriptions.
        /// </summary>
        /// <value>
        /// The delivery address descriptions.
        /// </value>
        IReadOnlyList<VmOpenApiLanguageItem> DeliveryAddressDescriptions { get; set; }

        /// <summary>
        /// Gets or sets the web pages.
        /// </summary>
        /// <value>
        /// The web pages.
        /// </value>
        new IList<VmOpenApiWebPage> WebPages { get; set; }

        /// <summary>
        /// Gets or sets the support contacts.
        /// </summary>
        /// <value>
        /// The support contacts.
        /// </value>
        IList<VmOpenApiSupport> SupportContacts { get; set; }
    }
}
