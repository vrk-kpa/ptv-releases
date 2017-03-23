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
using PTV.Domain.Model.Models.OpenApi.V4;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of service location channel for IN api - version base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceChannelIn" />
    public interface IVmOpenApiServiceLocationChannelInVersionBase : IVmOpenApiServiceChannelIn
    {
        /// <summary>
        /// Gets or sets a value indicating whether service area is restricted.
        /// </summary>
        /// <value>
        /// <c>true</c> if service area is restricted; otherwise, <c>false</c>.
        /// </value>
        bool ServiceAreaRestricted { get; set; }
        /// <summary>
        /// Gets or sets the service areas.
        /// </summary>
        /// <value>
        /// The service areas.
        /// </value>
        IList<string> ServiceAreas { get; set; }
        /// <summary>
        /// Gets or sets the fax numbers.
        /// </summary>
        /// <value>
        /// The fax numbers.
        /// </value>
        IList<V4VmOpenApiPhoneSimple> FaxNumbers { get; set; }
        /// <summary>
        /// Gets or sets the phone numbers.
        /// </summary>
        /// <value>
        /// The phone numbers.
        /// </value>
        IList<V4VmOpenApiPhone> PhoneNumbers { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether it is phone service charge.
        /// </summary>
        /// <value>
        ///   <c>true</c> if it is phone service charge; otherwise, <c>false</c>.
        /// </value>
        bool PhoneServiceCharge { get; set; }
        /// <summary>
        /// Gets or sets the addresses.
        /// </summary>
        /// <value>
        /// The addresses.
        /// </value>
        IList<V4VmOpenApiAddressWithTypeIn> Addresses { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all phone numbers should be deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all phone numbers should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllPhoneNumbers { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all fax numbers should be deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all fax numbers should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllFaxNumbers { get; set; }
    }
}
