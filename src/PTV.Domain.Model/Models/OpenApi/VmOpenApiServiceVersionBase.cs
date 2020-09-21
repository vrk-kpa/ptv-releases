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
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Domain.Model.Models.OpenApi.V10;
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V7;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceVersionBase" />
    public class VmOpenApiServiceVersionBase : VmOpenApiServiceBase, IVmOpenApiServiceVersionBase, IVmEntitySecurity
    {
        /// <summary>
        /// PTV identifier for linked general description.
        /// </summary>
        [JsonProperty(Order = 2)]
        public virtual Guid? GeneralDescriptionId { get; set; }

        /// <summary>
        /// Service sub-type. It is used for SOTE and its taken from GeneralDescription's generalDescriptionType. Possible values are: PrescribedByFreedomOfChoiceAct, OtherPermissionGrantedSote.
        /// </summary>
        [JsonProperty(Order = 3)]
        public virtual string SubType { get; set; }

        /// <summary>
        /// List of service areas.
        /// </summary>
        [JsonProperty(Order = 8)]
        public virtual IList<VmOpenApiArea> Areas { get; set; }

        /// <summary>
        /// List of service classes related to the service.
        /// </summary>
        [JsonProperty(Order = 15)]
        public IList<V7VmOpenApiFintoItemWithDescription> ServiceClasses { get; set; } = new List<V7VmOpenApiFintoItemWithDescription>();

        /// <summary>
        /// List of ontology terms related to the service.
        /// </summary>
        [JsonProperty(Order = 16)]
        public IList<V4VmOpenApiFintoItem> OntologyTerms { get; set; } = new List<V4VmOpenApiFintoItem>();

        /// <summary>
        /// List of target groups related to the service.
        /// </summary>
        [JsonProperty(Order = 17)]
        public IList<V4VmOpenApiFintoItem> TargetGroups { get; set; } = new List<V4VmOpenApiFintoItem>();

        /// <summary>
        /// List of life events  related to the service.
        /// </summary>
        [JsonProperty(Order = 18)]
        public IReadOnlyList<V4VmOpenApiFintoItem> LifeEvents { get; set; } = new List<V4VmOpenApiFintoItem>();

        /// <summary>
        /// List of industrial classes related to the service.
        /// </summary>
        [JsonProperty(Order = 19)]
        public IReadOnlyList<V4VmOpenApiFintoItem> IndustrialClasses { get; set; } = new List<V4VmOpenApiFintoItem>();

        // PTV-4184, SFIPTV-362
        /// <summary>
        /// List of linked service channels including relationship data.
        /// </summary>
        [JsonProperty(Order = 29)]
        public virtual IList<V11VmOpenApiServiceServiceChannel> ServiceChannels { get; set; } = new List<V11VmOpenApiServiceServiceChannel>();

        /// <summary>
        /// List of organizations, responsible and producer organizations of the service.
        /// </summary>
        [JsonProperty(Order = 30)]
        public IList<V6VmOpenApiServiceOrganization> Organizations { get; set; } = new List<V6VmOpenApiServiceOrganization>();

        /// <summary>
        /// List of service collections that the service has been linked to
        /// </summary>
        [JsonProperty(Order = 35)]
        public virtual IList<VmOpenApiServiceServiceCollection> ServiceCollections { get; set; } = new List<VmOpenApiServiceServiceCollection>();

        /// <summary>
        /// Publishing status. Possible values are: Draft, Published, Deleted or Modified.
        /// </summary>
        [JsonProperty(Order = 40)]
        public virtual string PublishingStatus { get; set; }

        /// <summary>
        /// Date when item was modified/created (UTC).
        /// </summary>
        [JsonProperty(Order = 50)]
        public virtual DateTime Modified { get; set; }

        /// <summary>
        /// Sote organization that is responsible for the service. Notice! At the moment always empty - the property is a placeholder for later use.
        /// </summary>
        [JsonProperty(Order = 51)]
        public virtual string ResponsibleSoteOrganization { get; set; }

        /// <summary>
        /// PTV identifier for main responsible organization.
        /// </summary>
        [JsonIgnore]
        public virtual VmOpenApiItem MainOrganization { get; set; }

        /// <summary>
        /// List of service producers
        /// </summary>
        [JsonIgnore]
        public virtual IList<VmOpenApiServiceProducer> ServiceProducers { get; set; } = new List<VmOpenApiServiceProducer>();

        /// <summary>
        /// Entity security information. Is used with IsVisibleForAll property.
        /// </summary>
        [JsonIgnore]
        public ISecurityOwnOrganization Security { get; set; }

        /// <summary>
        /// List of municipality codes and names that the service is available for. Used in conjunction with service coverage type Local.
        /// </summary>
        [JsonIgnore]
        public virtual IReadOnlyList<VmOpenApiMunicipality> Municipalities { get; set; } = new List<VmOpenApiMunicipality>();

        /// <summary>
        /// Gets or sets available languages
        /// </summary>
        [JsonIgnore]
        public IList<string> AvailableLanguages { get; set; }

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
            var vm = GetVersionBaseModel<V11VmOpenApiService>();
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
            vm.GeneralDescriptionId = this.GeneralDescriptionId;
            vm.SubType = this.SubType;
            vm.Areas = this.Areas;
            vm.ServiceClasses = this.ServiceClasses;
            vm.OntologyTerms = this.OntologyTerms;
            vm.TargetGroups = this.TargetGroups;
            vm.LifeEvents = this.LifeEvents;
            vm.IndustrialClasses = this.IndustrialClasses;
            vm.ServiceChannels = this.ServiceChannels;
            vm.Organizations = this.Organizations;
            vm.PublishingStatus = this.PublishingStatus;
            vm.Modified = this.Modified;
            vm.ServiceProducers = this.ServiceProducers;
            vm.MainOrganization = this.MainOrganization;
            vm.ServiceCollections = this.ServiceCollections;
            vm.ResponsibleSoteOrganization = this.ResponsibleSoteOrganization;
            return vm;
        }
    }
    #endregion
}
