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
using PTV.Domain.Model.Models.OpenApi.V1;
using PTV.Domain.Model.Models.OpenApi.V3;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi.V3
{
    /// <summary>
    /// OPEN API V3 - View model interface of organization
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiOrganizationVersionBase" />
    public interface IV3VmOpenApiOrganization : IVmOpenApiOrganizationVersionBase
    {
        /// <summary>
        /// Gets or sets the email addresses.
        /// </summary>
        /// <value>
        /// The email addresses.
        /// </value>
        new IList<VmOpenApiEmail> EmailAddresses { get; set; }
        /// <summary>
        /// List of organizations phone numbers.
        /// </summary>
        new IList<VmOpenApiPhone> PhoneNumbers { get; set; }
        /// <summary>
        /// Gets or sets the web pages.
        /// </summary>
        /// <value>
        /// The web pages.
        /// </value>
        new IList<V3VmOpenApiWebPage> WebPages { get; set; }
        /// <summary>
        /// Gets or sets the addresses.
        /// </summary>
        /// <value>
        /// The addresses.
        /// </value>
        new IList<V3VmOpenApiAddressWithTypeAndCoordinates> Addresses { get; set; }
        /// <summary>
        /// List of organizations services.
        /// </summary>
        new IList<V3VmOpenApiOrganizationService> Services { get; set; }
    }
}
