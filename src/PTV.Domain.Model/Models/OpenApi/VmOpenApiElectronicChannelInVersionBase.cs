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
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceChannelIn" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiElectronicChannelInVersionBase" />
    public class VmOpenApiElectronicChannelInVersionBase : VmOpenApiServiceChannelIn, IVmOpenApiElectronicChannelInVersionBase
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
        /// How many signatures are required (number). Required if RequiresSignature is true.
        /// </summary>
        [JsonProperty(Order = 10)]
        [RegularExpression(@"^[1-9]\d*$")]
        [RequiredIf("RequiresSignature", true)]
        public string SignatureQuantity { get; set; }

        /// <summary>
        /// Signature required.
        /// </summary>
        [JsonProperty(Order = 11)]
        public bool RequiresSignature { get; set; }

        /// <summary>
        /// Does the service require authentication.
        /// </summary>
        [JsonProperty(Order = 12)]
        [Required]
        public virtual bool RequiresAuthentication { get; set; }

        /// <summary>
        /// List of localized urls.
        /// </summary>
        [JsonProperty(Order = 13)]
        [ListWithUrl("Value")]
        [ListPropertyMaxLength(500, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual IList<VmOpenApiLanguageItem> WebPage { get; set; }

        /// <summary>
        /// List of attachments.
        /// </summary>
        [JsonProperty(Order = 14)]
        public IList<VmOpenApiAttachment> Attachments { get; set; } = new List<VmOpenApiAttachment>();

        /// <summary>
        /// The accessibility classification.
        /// </summary>
        [JsonProperty(Order = 26)]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual IList<VmOpenApiAccessibilityClassification> AccessibilityClassification { get; set; }

        /// <summary>
        /// Set to true to delete all existing attachments. The attachments collection should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 35)]
        public virtual bool DeleteAllAttachments { get; set; }

        /// <summary>
        /// List of service channel web pages.
        /// </summary>
        [JsonIgnore]
        public override IList<V9VmOpenApiWebPage> WebPages { get; set; }

        // SFIPTV-1963: deleteAllWebPages should not exist in EChannel model!
        /// <summary>
        /// Set to true to delete all existing web pages for the service channel. The WebPages collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllWebPages { get => base.DeleteAllWebPages; set => base.DeleteAllWebPages = value; }

        /// <summary>
        /// Gets or sets available languages
        /// </summary>
        [JsonIgnore]
        public override IList<string> AvailableLanguages {
            get
            {
                // Return available languages or calculate from organizationNames
                // SFIPTV-1913: All localized lists need to be taken into account: ServiceChannelNames, ServiceChannelDescriptions,
                /// WebPage, AccessibilityClassification, Attachments, SupportPhones, SupportEmails, ServiceHours lists.
                if (this._availableLanguages == null)
                {
                    var list = new HashSet<string>();
                    list.GetAvailableLanguages(this.ServiceChannelNames);
                    list.GetAvailableLanguages(this.ServiceChannelNamesWithType);
                    list.GetAvailableLanguages(this.ServiceChannelDescriptions);
                    list.GetAvailableLanguages(this.WebPage);
                    list.GetAvailableLanguages(this.AccessibilityClassification);
                    list.GetAvailableLanguages(this.Attachments);
                    list.GetPhoneAvailableLanguages(this.SupportPhones);
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
        /// Internal property to check the languages within required lists: ServiceChannelNames, ServiceChannelDescriptions,
        /// WebPage and AccessibilityClassification lists.
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
                    list.GetAvailableLanguages(this.WebPage);
                    list.GetAvailableLanguages(this.AccessibilityClassification);

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
        /// <returns>
        /// version number
        /// </returns>
        public override int VersionNumber()
        {
            return 0;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>
        /// view model of base
        /// </returns>
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            return base.VersionBase();
        }

        /// <summary>
        /// Gets the base version model.
        /// </summary>
        /// <returns>base version model</returns>
        protected VmOpenApiElectronicChannelInVersionBase GetVersionBaseModel(int version)
        {
            var vm = GetServiceChannelModel<VmOpenApiElectronicChannelInVersionBase>(version);
            if (ServiceChannelNames?.Count > 0)
            {
                if (vm.ServiceChannelNamesWithType == null) { vm.ServiceChannelNamesWithType = new List<VmOpenApiLocalizedListItem>(); }
                ServiceChannelNames.ForEach(name => vm.ServiceChannelNamesWithType.Add(
                    new VmOpenApiLocalizedListItem { Value = name.Value, Language = name.Language, Type = NameTypeEnum.Name.ToString() }
                ));
                //vm.AvailableLanguages = GetAvailableLanguages(vm); // TODO!!!!
            }
            vm.SignatureQuantity = SignatureQuantity;
            vm.RequiresSignature = RequiresSignature;
            vm.RequiresAuthentication = RequiresAuthentication;
            vm.WebPage = WebPage;
            vm.Attachments = Attachments;
            vm.AccessibilityClassification = AccessibilityClassification;
            vm.DeleteAllAttachments = DeleteAllAttachments;
            return vm;
        }
        #endregion
    }
}
