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

using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of Saha organization
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEntityBase" />
    public class VmOpenApiOrganizationSaha : VmEntityBase, IVmOpenApiOrganizationSaha
    {
        /// <summary>
        /// Organizations parent organization identifier if exists.
        /// </summary>
        [JsonProperty(Order = 2)]
        public Guid? ParentOrganizationId { get; set; }

        /// <summary>
        /// Organizations Saha identifier if exists.
        /// </summary>
        [JsonProperty(Order = 3)]
        public string SahaId { get; set; }

        /// <summary>
        /// List of organization names. Possible type values are: Name, AlternativeName.
        /// </summary>
        [JsonProperty(Order = 4)]
        public virtual IList<VmOpenApiLocalizedListItem> OrganizationNames { get; set; }

        /// <summary>
        /// Date when item was modified/created (UTC).
        /// </summary>
        [JsonProperty(Order = 5)]
        public DateTime Modified { get; set; }

        /// <summary>
        /// Publishing status (Published or Deleted).
        /// </summary>
        [JsonProperty(Order = 6)]
        public string PublishingStatus { get; set; }
    }
}
