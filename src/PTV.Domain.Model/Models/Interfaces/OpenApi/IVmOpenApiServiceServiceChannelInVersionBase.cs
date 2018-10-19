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
using PTV.Domain.Model.Models.OpenApi.V9;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of service and service channel connection - version base for IN (POST/PUT)
    /// </summary>
    public interface IVmOpenApiServiceServiceChannelInVersionBase : IVmOpenApiServiceServiceChannelBase
    {
        /// <summary>
        /// PTV service identifier.
        /// </summary>
        string ServiceId { get; set; }
        /// <summary>
        /// Gets or sets the service channel identifier.
        /// </summary>
        /// <value>
        /// The service channel identifier.
        /// </value>
        string ServiceChannelId { get; set; }
        /// <summary>
        /// Gets or sets the service unique identifier.
        /// </summary>
        /// <value>
        /// The service unique identifier.
        /// </value>
        Guid ServiceGuid { get; set; }
        /// <summary>
        /// Gets or sets the channel unique identifier.
        /// </summary>
        /// <value>
        /// The channel unique identifier.
        /// </value>
        Guid ChannelGuid { get; set; }
        /// <summary>
        /// The extra types related to service and service channel connection.
        /// </summary>
        IList<VmOpenApiExtraType> ExtraTypes { get; set; }
        /// <summary>
        /// List of connection related service hours.
        /// </summary>
        VmOpenApiContactDetailsInBase ContactDetails { get; set; }
        /// <summary>
        /// Indicates if value for property ServiceChargeType should be deleted.
        /// </summary>
        bool DeleteServiceChargeType { get; set; }
        /// <summary>
        /// Indicates if all descriptions should be deleted.
        /// </summary>
        bool DeleteAllDescriptions { get; set; }
        /// <summary>
        /// Internal property to map digital authorizations from GD proposed channels (PTV-2315)
        /// </summary>
        IList<Guid> DigitalAuthorizations { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether all emails should be delted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all email should be deleted; otherwise, <c>false</c>.
        /// </value>
        bool DeleteAllServiceHours { get; set; }
        /// <summary>
        /// Gets the in base version model.
        /// </summary>
        /// <returns></returns>
        V9VmOpenApiServiceServiceChannelAstiInBase GetLatestInVersionModel();
    }
}
