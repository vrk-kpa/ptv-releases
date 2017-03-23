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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceVersionBase" />
    public class VmOpenApiServiceVersionBase : VmOpenApiServiceBase, IVmOpenApiServiceVersionBase
    {
        /// <summary>
        /// PTV identifier for linked general description.
        /// </summary>
        [JsonProperty(Order = 3)]
        public Guid? StatutoryServiceGeneralDescriptionId { get; set; }

        /// <summary>
        /// List of service classes related to the service.
        /// </summary>
        [JsonProperty(Order = 10)]
        public IReadOnlyList<V4VmOpenApiFintoItem> ServiceClasses { get; set; } = new List<V4VmOpenApiFintoItem>();

        /// <summary>
        /// List of ontology terms related to the service.
        /// </summary>
        [JsonProperty(Order = 11)]
        public IReadOnlyList<V4VmOpenApiFintoItem> OntologyTerms { get; set; } = new List<V4VmOpenApiFintoItem>();

        /// <summary>
        /// List of target groups related to the service.
        /// </summary>
        [JsonProperty(Order = 12)]
        public IReadOnlyList<V4VmOpenApiFintoItem> TargetGroups { get; set; } = new List<V4VmOpenApiFintoItem>();

        /// <summary>
        /// List of life events  related to the service.
        /// </summary>
        [JsonProperty(Order = 13)]
        public IReadOnlyList<V4VmOpenApiFintoItem> LifeEvents { get; set; } = new List<V4VmOpenApiFintoItem>();

        /// <summary>
        /// List of industrial classes related to the service.
        /// </summary>
        [JsonProperty(Order = 14)]
        public IReadOnlyList<V4VmOpenApiFintoItem> IndustrialClasses { get; set; } = new List<V4VmOpenApiFintoItem>();

        /// <summary>
        /// List of municipality codes and names that the service is available for. Used in conjunction with service coverage type Local.
        /// </summary>
        [JsonProperty(Order = 22)]
        public IReadOnlyList<VmOpenApiMunicipality> Municipalities { get; set; } = new List<VmOpenApiMunicipality>();

        /// <summary>
        /// List of linked service channels including relationship data.
        /// </summary>
        [JsonProperty(Order = 29)]
        public IReadOnlyList<V4VmOpenApiServiceServiceChannel> ServiceChannels { get; set; } = new List<V4VmOpenApiServiceServiceChannel>();

        /// <summary>
        /// List of organizations, responsible and producer organizations of the service.
        /// </summary>
        [JsonProperty(Order = 30)]
        public IReadOnlyList<V4VmOpenApiServiceOrganization> Organizations { get; set; }

        /// <summary>
        /// General description type. Possible values are: Service or PermissionAndObligation.
        /// </summary>
        [JsonIgnore]
        public virtual string GeneralDescriptionType { get; set; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>
        /// version number
        /// </returns>
        public virtual int VersionNumber()
        {
            return 0;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>
        /// model of previous version
        /// </returns>
        public virtual IVmOpenApiServiceVersionBase PreviousVersion()
        {
            var vm = GetVersionBaseModel<V4VmOpenApiService>();
            return vm;
        }

        /// <summary>
        /// Gets the base version model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>base version model</returns>
        protected TModel GetVersionBaseModel<TModel>() where TModel : IVmOpenApiServiceVersionBase, new()
        {
            var vm = base.GetBaseModel<TModel>();
            vm.StatutoryServiceGeneralDescriptionId = this.StatutoryServiceGeneralDescriptionId;
            vm.ServiceClasses = this.ServiceClasses;
            vm.OntologyTerms = this.OntologyTerms;
            vm.TargetGroups = this.TargetGroups;
            vm.LifeEvents = this.LifeEvents;
            vm.IndustrialClasses = this.IndustrialClasses;
            vm.Municipalities = this.Municipalities;
            vm.ServiceChannels = this.ServiceChannels;
            vm.Organizations = this.Organizations;
            vm.GeneralDescriptionType = this.GeneralDescriptionType;
            return vm;
        }

        /// <summary>
        /// Gets the v3 finto item list.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>finto list item</returns>
        protected IList<VmOpenApiFintoItem> GetV3FintoItemList(IList<V4VmOpenApiFintoItem> list)
        {
            var vm3List = new List<VmOpenApiFintoItem>();
            list.ForEach(l => vm3List.Add(l.ConvertToVersion3()));

            return vm3List;
        }
    }
    #endregion
}
