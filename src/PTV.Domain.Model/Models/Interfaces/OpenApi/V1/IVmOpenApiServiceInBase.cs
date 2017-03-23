using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi;
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

namespace PTV.Domain.Model.Models.Interfaces.OpenApi.V1
{
    /// <summary>
    /// OPEN API - View model interface of service for IN api -base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceInVersionBase" />
    public interface IVmOpenApiServiceInBase : IVmOpenApiServiceInVersionBase
    {
        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>
        /// The keywords.
        /// </value>
        new IList<string> Keywords { get; set; }

        /// <summary>
        /// Gets or sets the web pages.
        /// </summary>
        /// <value>
        /// The web pages.
        /// </value>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        IList<VmOpenApiWebPage> WebPages { get; set; }

        /// <summary>
        /// Gets or sets the service additional informations.
        /// </summary>
        /// <value>
        /// The service additional informations.
        /// </value>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        IList<VmOpenApiLocalizedListItem> ServiceAdditionalInformations { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [delete all web pages].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [delete all web pages]; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        bool DeleteAllWebPages { get; set; }

        /// <summary>
        /// Gets or sets the service organizations.
        /// </summary>
        /// <value>
        /// The service organizations.
        /// </value>
        new IList<VmOpenApiServiceOrganization> ServiceOrganizations { get; set; }
    }
}
