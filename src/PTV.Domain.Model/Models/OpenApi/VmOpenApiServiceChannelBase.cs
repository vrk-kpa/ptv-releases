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
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service channel - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceChannelBase" />
    public class VmOpenApiServiceChannelBase : IVmOpenApiServiceChannelBase
    {
        /// <summary>
        /// PTV identifier for the service channel.
        /// </summary>
        [JsonProperty(Order = 1)]
        public virtual Guid? Id { get; set; }

        /// <summary>
        /// List of localized service channel descriptions.
        /// </summary>
        [JsonProperty(Order = 5)]
        [ListValueNotEmpty("Value")]
        [ListPropertyAllowedValues(propertyName: "Type", allowedValues: new[] {"Description", "ShortDescription"})]
        [ListPropertyMaxLength(150, "Value", "ShortDescription")]
        [LocalizedListPropertyDuplicityForbidden("Type")]
        [ListPropertyMaxLength(2500, "Value", "Description")]
        public virtual IList<VmOpenApiLocalizedListItem> ServiceChannelDescriptions { get; set; }

        /// <summary>
        /// Area type (WholeCountry, WholeCountryExceptAlandIslands, AreaType). 
        /// </summary>
        [JsonProperty(Order = 6)]
        [ValidEnum(typeof(AreaInformationTypeEnum))]
        public virtual string AreaType { get; set; }

        /// <summary>
        /// List of support phone numbers for the service channel.
        /// </summary>
        [JsonProperty(Order = 15)]
        //[LocalizedListLanguageDuplicityForbidden]
        public virtual IList<V4VmOpenApiPhone> SupportPhones { get; set; } = new List<V4VmOpenApiPhone>();

        /// <summary>
        /// List of support email addresses for the service channel.
        /// </summary>
        [JsonProperty(Order = 16)]
        [EmailAddressList("Value")]
        //[LocalizedListLanguageDuplicityForbidden]
        [ListPropertyMaxLength(100, "Value")]
        public virtual IList<VmOpenApiLanguageItem> SupportEmails { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of languages the service channel is available in (two letter language code).
        /// </summary>
        [JsonProperty(Order = 17)]
        [ListRegularExpression(@"^[a-z]{2}$")]
        public virtual IList<string> Languages { get; set; }

        /// <summary>
        /// List of service channel web pages.
        /// </summary>
        [JsonProperty(Order = 19)]
        [LocalizedListPropertyDuplicityForbidden("OrderNumber")]
        public virtual IList<VmOpenApiWebPageWithOrderNumber> WebPages { get; set; } = new List<VmOpenApiWebPageWithOrderNumber>();

        /// <summary>
        /// List of service channel service hours.
        /// </summary>
        [JsonProperty(Order = 25)]
        public virtual IList<V4VmOpenApiServiceHour> ServiceHours { get; set; } = new List<V4VmOpenApiServiceHour>();

        /// <summary>
        /// Publishing status. Possible values are: Draft, Published, Deleted or Modified.
        /// </summary>
        [JsonProperty(Order = 30)]
        [ValidEnum(typeof(PublishingStatus))]
        public virtual string PublishingStatus { get; set; }

        /// <summary>
        /// Indicates if channel can be used (referenced within services) by other users from other organizations.
        /// </summary>
        [JsonProperty(Order = 31)]
        public virtual bool IsVisibleForAll { get; set; }

        /// <summary>
        /// Gets or sets availble languages
        /// </summary>
        [JsonIgnore]
        public IList<string> AvailableLanguages { get; set; }

        /// <summary>
        /// Gets or sets the special channel identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonIgnore]
        public Guid? ChannelId { get; set; }

        #region Methods
        /// <summary>
        /// Gets the service channel base model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>service channel base model</returns>
        protected TModel GetServiceChannelBaseModel<TModel>() where TModel : IVmOpenApiServiceChannelBase, new()
        {
            return new TModel
            {
                Id = this.Id,
                ServiceChannelDescriptions = this.ServiceChannelDescriptions,
                AreaType = this.AreaType,
                SupportPhones = this.SupportPhones,
                SupportEmails = this.SupportEmails,
                Languages = this.Languages,
                WebPages = this.WebPages,
                ServiceHours = this.ServiceHours,
                PublishingStatus = this.PublishingStatus,
                IsVisibleForAll = this.IsVisibleForAll,
                AvailableLanguages = this.AvailableLanguages,
                ChannelId = this.ChannelId,
            };
        }
        
        #endregion
    }
}
