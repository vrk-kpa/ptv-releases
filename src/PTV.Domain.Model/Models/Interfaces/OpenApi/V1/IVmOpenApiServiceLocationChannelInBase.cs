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
    /// OPEN API - View model interface of service location channel for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceLocationChannelInVersionBase" />
    public interface IVmOpenApiServiceLocationChannelInBase : IVmOpenApiServiceLocationChannelInVersionBase
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        string Email { get; set; }
        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        /// <value>
        /// The phone.
        /// </value>
        string Phone { get; set; }
        /// <summary>
        /// Gets or sets the fax.
        /// </summary>
        /// <value>
        /// The fax.
        /// </value>
        string Fax { get; set; }
        /// <summary>
        /// Gets or sets the service charge types.
        /// </summary>
        /// <value>
        /// The service charge types.
        /// </value>
        IReadOnlyList<string> ServiceChargeTypes { get; set; }
        /// <summary>
        /// Gets or sets the phone charge descriptions.
        /// </summary>
        /// <value>
        /// The phone charge descriptions.
        /// </value>
        IReadOnlyList<VmOpenApiLanguageItem> PhoneChargeDescriptions { get; set; }
        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        /// <value>
        /// The latitude.
        /// </value>
        string Latitude { get; set; }
        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        /// <value>
        /// The longitude.
        /// </value>
        string Longitude { get; set; }
        /// <summary>
        /// Gets or sets the coordinate system.
        /// </summary>
        /// <value>
        /// The coordinate system.
        /// </value>
        string CoordinateSystem { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether coordinates are set manually.
        /// </summary>
        /// <value>
        /// <c>true</c> if coordinates are set manually; otherwise, <c>false</c>.
        /// </value>
        bool CoordinatesSetManually { get; set; }
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
        new IList<VmOpenApiAddressWithType> Addresses { get; set; }

        /// <summary>
        /// Gets or sets the service hours.
        /// </summary>
        /// <value>
        /// The service hours.
        /// </value>
        new IList<VmOpenApiServiceHour> ServiceHours { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether email should be deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if email should be delted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteEmail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether phone should be deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if phone should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeletePhone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fax should be deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if fax should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteFax { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all service charge types should be delted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all service charge types should be delted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllServiceChargeTypes { get; set; }
    }
}
