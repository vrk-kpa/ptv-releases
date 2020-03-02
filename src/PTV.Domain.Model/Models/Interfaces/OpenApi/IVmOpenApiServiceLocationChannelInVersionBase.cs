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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;
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
        /// Service channel OID. Must match the regex @"^[A-Za-z0-9.-]*$".
        /// </summary>
        string Oid { get; set; }
        /// <summary>
        /// Localized list of service channel names.
        /// </summary>
        IList<VmOpenApiLocalizedListItem> ServiceChannelNames { get; set; }
        /// <summary>
        /// List of Display name types (Name or AlternativeName) for each language version of ServiceChannelNames.
        /// </summary>
        IList<VmOpenApiNameTypeByLanguage> DisplayNameType { get; set; }
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
        /// Gets or sets the addresses.
        /// </summary>
        /// <value>
        /// The addresses.
        /// </value>
        IList<V9VmOpenApiAddressLocationIn> Addresses { get; set; }
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
        /// <summary>
        /// Set to true to delete OID. The Oid property should be empty when this property is set to true.
        /// </summary>
        bool DeleteOid { get; set; }
    }
}
