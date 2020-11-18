﻿/**
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
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.Interfaces.OpenApi
{
    /// <summary>
    /// OPEN API - View Model interface of service and service channel connection - version base
    /// </summary>
    public interface IVmOpenApiServiceChannelServiceVersionBase : IVmOpenApiServiceServiceChannelBase
    {
        /// <summary>
        /// Service identifier and name.
        /// </summary>
        VmOpenApiItem Service { get; set; }
        /// <summary>
        /// The extra types related to service and service channel connection.
        /// </summary>
        IList<V9VmOpenApiExtraType> ExtraTypes { get; set; }

        /// <summary>
        /// Contact details for connection.
        /// </summary>
        V9VmOpenApiContactDetails ContactDetails { get; set; }
        /// <summary>
        /// Gets or sets the digital authorizations.
        /// </summary>
        /// <value>
        /// The digital authorizations.
        /// </value>
        IReadOnlyList<V4VmOpenApiFintoItem> DigitalAuthorizations { get; set; }
        /// <summary>
        /// Date when connection was modified/created (UTC).
        /// </summary>
        DateTime Modified { get; set; }
    }
}
