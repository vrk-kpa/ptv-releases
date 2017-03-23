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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V1;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Domain.Model.Models.OpenApi.V1
{
    /// <summary>
    /// OPEN API - View Model of service
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V1.IVmOpenApiService" />
    public class VmOpenApiService : VmOpenApiServiceVersionBase, IVmOpenApiService
    {
        /// <summary>
        /// List of service classes.
        /// </summary>
        [JsonProperty(Order = 10)]
        public new IList<VmOpenApiFintoItem> ServiceClasses { get; set; } = new List<VmOpenApiFintoItem>();

        /// <summary>
        /// List of ontology terms.
        /// </summary>
        [JsonProperty(Order = 11)]
        public new IList<VmOpenApiFintoItem> OntologyTerms { get; set; } = new List<VmOpenApiFintoItem>();

        /// <summary>
        /// List of target groups.
        /// </summary>
        [JsonProperty(Order = 12)]
        public new IList<VmOpenApiFintoItem> TargetGroups { get; set; } = new List<VmOpenApiFintoItem>();

        /// <summary>
        /// List of life events.
        /// </summary>
        [JsonProperty(Order = 13)]
        public new IList<VmOpenApiFintoItem> LifeEvents { get; set; } = new List<VmOpenApiFintoItem>();

        /// <summary>
        /// List of industrial classes.
        /// </summary>
        [JsonProperty(Order = 14)]
        public virtual new IList<VmOpenApiFintoItem> IndustrialClasses { get; set; } = new List<VmOpenApiFintoItem>();

        /// <summary>
        /// List of service keywords.
        /// </summary>
        [JsonProperty(Order = 20)]
        public new IReadOnlyList<string> Keywords { get; set; } = new List<string>();

        /// <summary>
        /// List of municipality names that the service is available for. Used in conjunction with service coverage type Local.
        /// </summary>
        [JsonProperty(Order = 22)]
        public new IList<string> Municipalities { get; set; } = new List<string>();

        /// <summary>
        /// List of service web pages. This property is not used in the API anymore. Do not use.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 23)]
        public IList<VmOpenApiWebPage> WebPages { get; set; } = new List<VmOpenApiWebPage>();

        /// <summary>
        /// Localized service additional information.
        /// </summary>
        [JsonProperty(Order = 26)]
        public virtual IList<VmOpenApiLocalizedListItem> ServiceAdditionalInformations { get; set; } = new List<VmOpenApiLocalizedListItem>();

        /// <summary>
        /// List of PTV identifiers of linked service channels.
        /// </summary>
        [JsonProperty(Order = 29)]
        public new IList<string> ServiceChannels { get; set; }

        /// <summary>
        /// List of organizations, responsible and producer organizations of the service.
        /// </summary>
        [JsonProperty(Order = 30)]
        public new IList<VmOpenApiServiceOrganization> Organizations { get; set; }

        /// <summary>
        /// List of laws related to the service.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiLaw> Legislation { get; set; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 1;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        /// <exception cref="System.NotSupportedException">No previous version for VmOpenApiService!</exception>
        public override IVmOpenApiServiceVersionBase PreviousVersion()
        {
            throw new NotSupportedException("No previous version for VmOpenApiService!");
        }
        #endregion
    }
}
