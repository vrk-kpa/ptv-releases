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

using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V1;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi.V1
{
    /// <summary>
    /// OPEN API - View model interface of organization
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiOrganizationVersionBase" />
    public interface IVmOpenApiOrganization : IVmOpenApiOrganizationVersionBase
    {
        /// <summary>
        /// Municipality name (like Mikkeli or Helsinki).
        /// </summary>
        new string Municipality { get; set; }

        /// <summary>
        /// This property is not used in the API anymore.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        bool StreetAddressAsPostalAddress { get; set; }

        /// <summary>
        /// List of organizations email addresses.
        /// </summary>
        new IList<VmOpenApiOrganizationEmail> EmailAddresses { get; set; }

        /// <summary>
        /// List of organizations phone numbers.
        /// </summary>
        new IList<VmOpenApiOrganizationPhone> PhoneNumbers { get; set; }

        /// <summary>
        /// List of organizations web pages.
        /// </summary>
        new IList<VmOpenApiWebPage> WebPages { get; set; }

        /// <summary>
        /// List of organizations addresses.
        /// </summary>
        new IList<VmOpenApiAddressWithType> Addresses { get; set; }

        /// <summary>
        /// List of organizations services.
        /// </summary>
        new IList<VmOpenApiOrganizationService> Services { get; set; }
    }
}
