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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Framework;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service channel for IN api
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceChannelBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceChannelIn" />
    public class VmOpenApiServiceChannelIn : VmOpenApiServiceChannelBase, IVmOpenApiServiceChannelIn
    {
        /// <summary>
        /// PTV identifier for the service channel.
        /// </summary>
        [JsonIgnore]
        public override Guid? Id
        {
            get
            {
                return base.Id;
            }

            set
            {
                base.Id = value;
            }
        }

        /// <summary>
        /// External system identifier for this service channel.
        /// </summary>
        [JsonProperty(Order = 1)]
        [RegularExpression(@"^[A-Za-z0-9-.]*$")]
        public string SourceId { get; set; }

        /// <summary>
        /// PTV organization identifier for organization responsible for this service channel.
        /// </summary>
        [JsonProperty(Order = 3)]
        [ValidGuid]
        public virtual string OrganizationId { get; set; }

        /// <summary>
        /// Localized list of service channel names.
        /// </summary>
        [JsonProperty(Order = 5)]
        [ListPropertyMaxLength(100, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual IList<VmOpenApiLanguageItem> ServiceChannelNames { get; set; }

        /// <summary>
        /// List of areas. List can contain different types of areas.
        /// </summary>
        [ListRequiredIf("AreaType", "AreaType")]
        [JsonProperty(Order = 8)]
        public virtual IList<VmOpenApiAreaIn> Areas { get; set; } = new List<VmOpenApiAreaIn>();

        /// <summary>
        /// Set to true to delete all existing web pages for the service channel. The WebPages collection should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 45)]
        public virtual bool DeleteAllWebPages { get; set; }

        /// <summary>
        /// Set to true to delete all existing service hours for the service channel. The ServiceHours collection should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 50)]
        public virtual bool DeleteAllServiceHours { get; set; }

        /// <summary>
        /// Set to true to delete all existing support email addresses for the service channel. The SupportEmails collection should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 55)]
        public virtual bool DeleteAllSupportEmails { get; set; }

        /// <summary>
        /// Set to true to delete all existing support phone numbers for the service channel. The SupportPhones collection should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 60)]
        public virtual bool DeleteAllSupportPhones { get; set; }

        /// <summary>
        /// Current version publishing status.
        /// </summary>
        [JsonIgnore]
        public string CurrentPublishingStatus { get; set; }

        /// <summary>
        /// Internal property for adding service relations for a service channel.
        /// </summary>
        [JsonIgnore]
        public IList<Guid> ServiceChannelServices { get; set; } = new List<Guid>();

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>
        /// version number
        /// </returns>
        /// <exception cref="System.NotSupportedException">VmOpenApiServiceChannelIn does not have version number available!</exception>
        public virtual int VersionNumber()
        {
            throw new NotSupportedException("VmOpenApiServiceChannelIn does not have version number available!");
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>
        /// view model of base
        /// </returns>
        /// <exception cref="System.NotImplementedException">VmOpenApiServiceChannelIn does not have base version available!</exception>
        public virtual IVmOpenApiServiceChannelIn VersionBase()
        {
            throw new NotImplementedException("VmOpenApiServiceChannelIn does not have base version available!");
        }

        /// <summary>
        /// Gets the service channel model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>service channel model</returns>
        protected TModel GetServiceChannelModel<TModel>() where TModel : IVmOpenApiServiceChannelIn, new()
        {
            var vm = base.GetServiceChannelBaseModel<TModel>();
            vm.SourceId = SourceId;
            vm.OrganizationId = OrganizationId;
            vm.ServiceChannelNames = ServiceChannelNames;
            vm.Areas = Areas;
            vm.DeleteAllWebPages = DeleteAllWebPages;
            vm.DeleteAllServiceHours = DeleteAllServiceHours;
            vm.DeleteAllSupportEmails = DeleteAllSupportEmails;
            vm.DeleteAllSupportPhones = DeleteAllSupportPhones;
            vm.CurrentPublishingStatus = CurrentPublishingStatus;
            vm.ServiceChannelServices = ServiceChannelServices;
            return vm;
        }
        #endregion
    }
}
