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

using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V8;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of printable form channel for IN api - version base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceChannelIn" />
    public interface IVmOpenApiPrintableFormChannelInVersionBase : IVmOpenApiServiceChannelIn
    {
        /// <summary>
        /// Gets or sets the service channel names.
        /// </summary>
        /// <value>
        /// The service channel names.
        /// </value>
        IList<VmOpenApiLanguageItem> ServiceChannelNames { get; set; }
        /// <summary>
        /// Gets or sets the form identifier.
        /// </summary>
        /// <value>
        /// The form identifier.
        /// </value>
        IList<VmOpenApiLanguageItem> FormIdentifier { get; set; }
        /// <summary>
        /// Gets or sets the delivery addresses.
        /// </summary>
        /// <value>
        /// The delivery addresses.
        /// </value>
        IList<V8VmOpenApiAddressDeliveryIn> DeliveryAddresses { get; set; }
        /// <summary>
        /// Gets or sets the channel urls.
        /// </summary>
        /// <value>
        /// The channel urls.
        /// </value>
        IList<VmOpenApiLocalizedListItem> ChannelUrls { get; set; }
        /// <summary>
        /// Gets or sets the attachments.
        /// </summary>
        /// <value>
        /// The attachments.
        /// </value>
        IList<VmOpenApiAttachment> Attachments { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether delivery addresses should be deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if delivery addresses should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllDeliveryAddresses { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all channel urls should be deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all channel urls should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllChannelUrls { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all attachments should be deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all attachments should be delted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllAttachments { get; set; }
        /// <summary>
        /// Set to true to delete all existing form identifiers for the service channel. The form identifiers collection should be empty when this property is set to true.
        /// </summary>
        bool DeleteAllFormIdentifiers { get; set; }
    }
}
