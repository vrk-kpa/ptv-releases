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

using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V1;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    /// <summary>
    /// OPEN API V2 - View Model of service
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V2.IV2VmOpenApiService" />
    public class V2VmOpenApiService : VmOpenApiServiceVersionBase, IV2VmOpenApiService
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
        /// List of municipality names that the service is available for. Used in conjunction with service coverage type Local.
        /// </summary>
        [JsonProperty(Order = 22)]
        public new IList<string> Municipalities { get; set; } = new List<string>();

        /// <summary>
        /// Localized service additional information.
        /// </summary>
        [JsonProperty(Order = 26)]
        public virtual IList<VmOpenApiLocalizedListItem> ServiceAdditionalInformations { get; set; } = new List<VmOpenApiLocalizedListItem>();

        /// <summary>
        /// List of linked service channels including relationship data.
        /// </summary>
        [JsonProperty(Order = 29)]
        public new IList<V2VmOpenApiServiceServiceChannel> ServiceChannels { get; set; } = new List<V2VmOpenApiServiceServiceChannel>();

        /// <summary>
        /// List of organizations, responsible and producer organizations of the service.
        /// </summary>
        [JsonProperty(Order = 30)]
        public new IList<VmOpenApiServiceOrganization> Organizations { get; set; }

        /// <summary>
        /// List of laws related to the service.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiLaw> Legislation { get;  set; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 2;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceVersionBase PreviousVersion()
        {
            var vm = GetVersionBaseModel<VmOpenApiService>();
            vm.ServiceClasses = ServiceClasses;
            vm.OntologyTerms = OntologyTerms;
            vm.TargetGroups = TargetGroups;
            vm.LifeEvents = LifeEvents;
            vm.IndustrialClasses = IndustrialClasses;

            vm.ServiceAdditionalInformations = ServiceAdditionalInformations;

            vm.Municipalities = Municipalities;
            // Set the v1 keywords vm
            vm.Keywords = Keywords.Select(k => k.Value).ToList();
            vm.ServiceChannels = ServiceChannels?.Select(s => s.ServiceChannelId).ToList();
            vm.Organizations = Organizations;
            return vm;
        }
        #endregion
    }
}
