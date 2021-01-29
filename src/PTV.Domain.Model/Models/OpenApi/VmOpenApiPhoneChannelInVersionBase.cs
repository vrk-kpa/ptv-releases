﻿/**
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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Attributes;
using PTV.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of phone channel for IN api - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceChannelIn" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiPhoneChannelInVersionBase" />
    public class VmOpenApiPhoneChannelInVersionBase : VmOpenApiServiceChannelIn, IVmOpenApiPhoneChannelInVersionBase
    {
        private IList<string> _availableLanguages;
        private IList<string> _requiredPropertiesAvailableLanguages;

        // SFIPTV-236
        /// <summary>
        /// Localized list of service channel names.
        /// </summary>
        [JsonProperty(Order = 6)]
        [ListPropertyMaxLength(100, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual IList<VmOpenApiLanguageItem> ServiceChannelNames { get; set; }

        /// <summary>
        /// List of phone numbers for the service channel.
        /// </summary>
        [JsonProperty(Order = 12)]
        public virtual IList<V4VmOpenApiPhoneWithType> PhoneNumbers { get; set; }

        /// <summary>
        /// List of localized urls.
        /// </summary>
        [JsonProperty(Order = 13)]
        [ListWithUrl("Value")]
        [ListPropertyMaxLength(500, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual IList<VmOpenApiLanguageItem> WebPage { get; set; }

        /// <summary>
        /// List of service channel web pages.
        /// </summary>
        [JsonIgnore]
        public override IList<V9VmOpenApiWebPage> WebPages { get; set; }

        /// <summary>
        /// List of support phone numbers for the service channel.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiPhone> SupportPhones { get; set; }

        /// <summary>
        /// Set to true to delete all existing support email addresses for the service channel. The SupportEmails collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllSupportEmails { get; set; }

        /// <summary>
        /// Set to true to delete all existing support phone numbers for the service channel. The SupportPhones collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllSupportPhones { get; set; }

        /// <summary>
        /// Gets or sets available languages
        /// </summary>
        [JsonIgnore]
        public override IList<string> AvailableLanguages
        {
            get
            {
                // Return available languages or calculate from organizationNames
                // SFIPTV-1913: All localized lists need to be taken into account: ServiceChannelNames, ServiceChannelDescriptions,
                /// PhoneNumbers, WebPage, SupportEmails, ServiceHours lists.
                if (this._availableLanguages == null)
                {
                    var list = new HashSet<string>();
                    list.GetAvailableLanguages(this.ServiceChannelNames);
                    list.GetAvailableLanguages(this.ServiceChannelNamesWithType);
                    list.GetAvailableLanguages(this.ServiceChannelDescriptions);
                    list.GetAvailableLanguages(this.WebPage);
                    list.GetPhoneAvailableLanguages(this.PhoneNumbers);
                    list.GetAvailableLanguages(this.SupportEmails);
                    list.GetAvailableLanguages(this.ServiceHours);

                    this._availableLanguages = list.ToList();
                }

                return this._availableLanguages;
            }
            set
            {
                this._availableLanguages = value;
            }
        }

        /// <summary>
        /// Internal property to check the languages within required lists: ServiceChannelNames, ServiceChannelDescriptions
        /// and PhoneNumbers lists.
        /// </summary>
        [JsonIgnore]
        public override IList<string> RequiredPropertiesAvailableLanguages
        {
            get
            {
                if (_requiredPropertiesAvailableLanguages == null)
                {
                    var list = new HashSet<string>();
                    list.GetAvailableLanguages(this.ServiceChannelNames);
                    list.GetAvailableLanguages(this.ServiceChannelNamesWithType);
                    list.GetAvailableLanguages(this.ServiceChannelDescriptions);
                    list.GetPhoneAvailableLanguages(this.PhoneNumbers);

                    _requiredPropertiesAvailableLanguages = list.ToList();
                }

                return _requiredPropertiesAvailableLanguages;
            }
            set
            {
                _requiredPropertiesAvailableLanguages = value;
            }
        }

        #region Methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 0;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            return base.VersionBase();
        }

        /// <summary>
        /// Gets the version base model.
        /// </summary>
        /// <returns>base version model</returns>
        protected VmOpenApiPhoneChannelInVersionBase GetVersionBaseModel(int version)
        {
            var vm = GetServiceChannelModel<VmOpenApiPhoneChannelInVersionBase>(version);
            if (ServiceChannelNames?.Count > 0)
            {
                if (vm.ServiceChannelNamesWithType == null) { vm.ServiceChannelNamesWithType = new List<VmOpenApiLocalizedListItem>(); }
                ServiceChannelNames.ForEach(name => vm.ServiceChannelNamesWithType.Add(
                    new VmOpenApiLocalizedListItem { Value = name.Value, Language = name.Language, Type = NameTypeEnum.Name.ToString() }
                ));
            }
            vm.PhoneNumbers = PhoneNumbers;
            vm.WebPage = WebPage;
            return vm;
        }
        #endregion
    }
}
