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
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of address in version - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiBase" />
    public interface IVmOpenApiAddressInVersionBase : IVmOpenApiBase
    {
        /// <summary>
        /// Gets or sets the post office box.
        /// </summary>
        /// <value>
        /// The post office box.
        /// </value>
        IList<VmOpenApiLanguageItem> PostOfficeBox { get; set; }
        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        /// <value>
        /// The postal code.
        /// </value>
        string PostalCode { get; set; }
        /// <summary>
        /// Gets or sets the street address.
        /// </summary>
        /// <value>
        /// The street address.
        /// </value>
        IList<VmOpenApiLanguageItem> StreetAddress { get; set; }
        /// <summary>
        /// Gets or sets the street number.
        /// </summary>
        /// <value>
        /// The street number.
        /// </value>
        string StreetNumber { get; set; }
        /// <summary>
        /// Gets or sets the municipality.
        /// </summary>
        /// <value>
        /// The municipality.
        /// </value>
        string Municipality { get; set; }
        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>
        /// The country.
        /// </value>
        string Country { get; set; }
        /// <summary>
        /// Gets or sets the additional informations.
        /// </summary>
        /// <value>
        /// The additional informations.
        /// </value>
        IList<VmOpenApiLanguageItem> AdditionalInformations { get; set; }
        /// <summary>
        /// Gets or sets the service location latitude coordinate.
        /// </summary>
        /// <value>
        /// The service location latitude coordinate.
        /// </value>
        string Latitude { get; set; }
        /// <summary>
        /// Gets or sets the service location longitude coordinate.
        /// </summary>
        /// <value>
        /// The service location longitude coordinate.
        /// </value>
        string Longitude { get; set; }
        /// <summary>
        /// Gets or sets the owner reference identifier.
        /// </summary>
        /// <value>
        /// The owner reference identifier.
        /// </value>
        Guid? OwnerReferenceId { get; set; }
    }
}
