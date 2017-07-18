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
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V3;

namespace PTV.Domain.Model.Models.OpenApi.V3
{
    /// <summary>
    /// OPEN API V3 - View Model of organization service
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V3.IV3VmOpenApiOrganizationService" />
    public class V3VmOpenApiOrganizationService : VmOpenApiOrganizationServiceVersionBase, IV3VmOpenApiOrganizationService
    {
        /// <summary>
        /// PTV service identifier.
        /// </summary>
        public virtual string ServiceId { get; set; }

        /// <summary>
        /// PTV organization identifier (organization related to the service).
        /// </summary>
        public string OrganizationId { get; set; }

        /// <summary>
        /// List of web pages.
        /// </summary>
        public new IList<V3VmOpenApiWebPage> WebPages { get; set; } = new List<V3VmOpenApiWebPage>();

        /// <summary>
        /// Service identifier and name.
        /// </summary>
        [JsonIgnore]
        public override VmOpenApiItem Service { get => base.Service; set => base.Service = value; }
    }
}
