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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of service channel - base
    /// </summary>
    public interface IVmOpenApiServiceChannelBase : IVmEntityBase
    {
        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        /// <value>
        /// The source identifier.
        /// </value>
        string SourceId { get; set; }
        /// <summary>
        /// Gets or sets the service channel descriptions.
        /// </summary>
        /// <value>
        /// The service channel descriptions.
        /// </value>
        IList<VmOpenApiLocalizedListItem> ServiceChannelDescriptions { get; set; }
        /// <summary>
        /// Area type (WholeCountry, WholeCountryExceptAlandIslands, AreaType). 
        /// </summary>
        string AreaType { get; set; }
        /// <summary>
        /// Gets or sets the support phones.
        /// </summary>
        /// <value>
        /// The support phones.
        /// </value>
        IList<V4VmOpenApiPhone> SupportPhones { get; set; }
        /// <summary>
        /// Gets or sets the support emails.
        /// </summary>
        /// <value>
        /// The support emails.
        /// </value>
        IList<VmOpenApiLanguageItem> SupportEmails { get; set; }
        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        /// <value>
        /// The languages.
        /// </value>
        IList<string> Languages { get; set; }
        /// <summary>
        /// Gets or sets the web pages.
        /// </summary>
        /// <value>
        /// The web pages.
        /// </value>
        IList<V9VmOpenApiWebPage> WebPages { get; set; }
        /// <summary>
        /// Gets or sets the service hours.
        /// </summary>
        /// <value>
        /// The service hours.
        /// </value>
        IList<V8VmOpenApiServiceHour> ServiceHours { get; set; }
        /// <summary>
        /// Gets or sets the publishing status.
        /// </summary>
        /// <value>
        /// The publishing status.
        /// </value>
        string PublishingStatus { get; set; }
        /// <summary>
        /// Indicates if channel can be used (referenced within services) by other users from other organizations.
        /// </summary>
        bool IsVisibleForAll { get; set; }
        /// <summary>
        /// Gets or sets availble languages
        /// </summary>
        IList<string> AvailableLanguages { get; set; }
        /// <summary>
        /// Gets or sets the special channel identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        Guid? ChannelId { get; set; }
    }
}
