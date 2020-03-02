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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceChannelBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceChannel" />
    public class VmOpenApiServiceChannel : VmOpenApiServiceChannelBase, IVmOpenApiServiceChannel, IVmEntitySecurity
    {
        /// <summary>
        /// Type of the service channel. Channel types: EChannel, WebPage, PrintableForm, Phone or ServiceLocation.
        /// </summary>
        [JsonProperty(Order = 4)]
        public string ServiceChannelType { get; set; }

        /// <summary>
        /// PTV organization identifier responsible for the channel.
        /// </summary>
        [JsonProperty(Order = 5)]
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Localized list of service channel names. Possible type values are: Name, AlternativeName (in version 7 AlternateName).
        /// </summary>
        [JsonProperty(Order = 6)]
        public IList<VmOpenApiLocalizedListItem> ServiceChannelNames { get; set; }

        /// <summary>
        /// List of service channel areas.
        /// </summary>
        [JsonProperty(Order = 9)]
        public virtual IList<VmOpenApiArea> Areas { get; set; } = new List<VmOpenApiArea>();

        // PTV-4184, SFIPTV-1912
        /// <summary>
        /// List of linked services including relationship data.
        /// </summary>
        [JsonProperty(Order = 29)]
        public virtual IList<V11VmOpenApiServiceChannelService> Services { get; set; } = new List<V11VmOpenApiServiceChannelService>();

        /// <summary>
        /// Publishing status. Possible values are: Draft, Published, Deleted or Modified.
        /// </summary>
        [JsonProperty(Order = 30)]
        public virtual string PublishingStatus { get; set; }

        /// <summary>
        /// Date when item was modified/created (UTC).
        /// </summary>
        [JsonProperty(Order = 40)]
        public virtual DateTime Modified { get; set; }

        /// <summary>
        /// Sote organization that is responsible for the service channel. Notice! At the moment always empty - the property is a placeholder for later use.
        /// </summary>
        [JsonProperty(Order = 41)]
        public virtual string ResponsibleSoteOrganization { get; set; }

        /// <summary>
        /// List of ontology terms related to the all service connections.
        /// </summary>
        [JsonProperty(Order = 42)]
        public virtual IList<V4VmOpenApiFintoItem> OntologyTerms { get; set; } = new List<V4VmOpenApiFintoItem>();

        /// <summary>
        /// List of municipalities including municipality code and a localized list of municipality names.
        /// </summary>
        [JsonIgnore]
        public virtual IList<VmOpenApiMunicipality> AreaMunicipalities { get; set; } = new List<VmOpenApiMunicipality>();

        /// <summary>
        /// Indicates if channel can be used (referenced within services) by other users from other organizations.
        /// </summary>
        [JsonIgnore]
        public override bool IsVisibleForAll { get => base.IsVisibleForAll; set => base.IsVisibleForAll = value; }

        /// <summary>
        /// Entity security information. Is used with IsVisibleForAll property.
        /// </summary>
        [JsonIgnore]
        public ISecurityOwnOrganization Security { get; set; }

        /// <summary>
        /// Gets or sets available languages
        /// </summary>
        [JsonIgnore]
        public virtual IList<string> AvailableLanguages { get; set; }

        #region Methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>
        /// version number
        /// </returns>
        /// <exception cref="System.NotSupportedException">No version for base class VmOpenApiServiceChannel!</exception>
        public virtual int VersionNumber()
        {
            throw new NotSupportedException("No version for base class VmOpenApiServiceChannel!");
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>
        /// model of previous version
        /// </returns>
        /// <exception cref="System.NotSupportedException">No previous version for base class VmOpenApiServiceChannel!</exception>
        public virtual IVmOpenApiServiceChannel PreviousVersion()
        {
            throw new NotSupportedException("No previous version for base class VmOpenApiServiceChannel!");
        }

        /// <summary>
        /// Gets the service channel model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>service channel model</returns>
        protected TModel GetServiceChannelModel<TModel>() where TModel : IVmOpenApiServiceChannel, new()
        {
            var vm = base.GetServiceChannelBaseModel<TModel>();
            vm.ServiceChannelType = ServiceChannelType;
            vm.OrganizationId = OrganizationId;
            vm.ServiceChannelNames = ServiceChannelNames;
            vm.Areas = Areas;
            vm.Services = Services;
            vm.PublishingStatus = PublishingStatus;
            vm.Modified = Modified;
            vm.ResponsibleSoteOrganization = this.ResponsibleSoteOrganization;
            vm.OntologyTerms = this.OntologyTerms;
            // Map the area municipalities into areas
            if (AreaMunicipalities.Count > 0)
            {
                vm.Areas.Add(new VmOpenApiArea { Type = AreaTypeEnum.Municipality.ToString(), Municipalities = AreaMunicipalities });
            }
            vm.AvailableLanguages = this.AvailableLanguages;
            return vm;
        }
        #endregion
    }

}
