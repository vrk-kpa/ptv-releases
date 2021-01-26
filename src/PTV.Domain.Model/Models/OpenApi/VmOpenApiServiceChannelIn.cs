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
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Framework;
using PTV.Framework.Attributes;
using PTV.Framework.Attributes.ValidationAttributes;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service channel for IN api
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceChannelBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceChannelIn" />
    public abstract class VmOpenApiServiceChannelIn : VmOpenApiServiceChannelBase, IVmOpenApiServiceChannelIn
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
        /// PTV organization identifier for organization responsible for this service channel.
        /// </summary>
        [JsonProperty(Order = 5)]
        [ValidGuid]
        public virtual string OrganizationId { get; set; }

        // SFIPTV-236
        /// <summary>
        /// Localized list of service channel names.
        /// </summary>
        [JsonIgnore]
        public virtual IList<VmOpenApiLocalizedListItem> ServiceChannelNamesWithType { get; set; }

        /// <summary>
        /// List of areas. List can contain different types of areas.
        /// </summary>
        [ListRequiredIf("AreaType", "AreaType")]
        [JsonProperty(Order = 9)]
        public virtual IList<VmOpenApiAreaIn> Areas { get; set; } = new List<VmOpenApiAreaIn>();

        /// <summary>
        /// Publishing status. Possible values are: Draft, Published, Deleted or Modified.
        /// </summary>
        [JsonProperty(Order = 30)]
        [Required]
        [ValidEnum(typeof(PublishingStatus))]
        public virtual string PublishingStatus { get; set; }

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
        /// Date when item should be published.
        /// </summary>
        [JsonProperty(Order = 38)]
        [DateInFuture]
        public virtual DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Date when item should be archived.
        /// </summary>
        [JsonProperty(Order = 39)]
        [DateInFuture]
        public virtual DateTime? ValidTo { get; set; }

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

        /// <summary>
        /// User name.
        /// </summary>
        [JsonIgnore]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets available languages
        /// </summary>
        [JsonIgnore]
        public virtual IList<string> AvailableLanguages { get; set; }

        /// <summary>
        /// Internal property to check the languages within required lists
        /// </summary>
        [JsonIgnore]
        public abstract IList<string> RequiredPropertiesAvailableLanguages { get; set; }

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
        protected TModel GetServiceChannelModel<TModel>(int version) where TModel : IVmOpenApiServiceChannelIn, new()
        {
            var vm = base.GetServiceChannelBaseModel<TModel>();
            vm.OrganizationId = OrganizationId;
            vm.ServiceChannelNamesWithType = ServiceChannelNamesWithType;
            vm.Areas = Areas;
            vm.DeleteAllWebPages = DeleteAllWebPages;
            vm.DeleteAllServiceHours = DeleteAllServiceHours;
            vm.DeleteAllSupportEmails = DeleteAllSupportEmails;
            vm.DeleteAllSupportPhones = DeleteAllSupportPhones;
            vm.CurrentPublishingStatus = CurrentPublishingStatus;
            vm.ServiceChannelServices = ServiceChannelServices;
            vm.PublishingStatus = PublishingStatus;
            vm.ValidFrom = ValidFrom;
            vm.ValidTo = ValidTo;
            vm.UserName = UserName;
            vm.AvailableLanguages = AvailableLanguages;

            // Set order number for daily opening times
            vm.ServiceHours?.ForEach(h =>
            {
                if (h.OpeningHour?.Count > 0)
                {
                    // The first opening hour for a day has always order number 0.
                    var dailyOpeningHours = h.OpeningHour
                        .Where(h => !h.DayFrom.IsNullOrEmpty())
                        .GroupBy(o => o.DayFrom)
                        .ToDictionary(o => o.Key, o => o.ToList());
                    dailyOpeningHours.ForEach(day =>
                    {
                        var allHours = day.Value;
                        int order = 0;
                        allHours.ForEach(o => o.Order = order++);
                    });
                }
            });
            return vm;
        }
        #endregion
    }
}
