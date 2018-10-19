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
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Attributes;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of web page channel for IN api - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceChannelIn" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiWebPageChannelInVersionBase" />
    public class VmOpenApiWebPageChannelInVersionBase : VmOpenApiServiceChannelIn, IVmOpenApiWebPageChannelInVersionBase
    {
        // SFIPTV-236
        /// <summary>
        /// Localized list of service channel names.
        /// </summary>
        [JsonProperty(Order = 6)]
        [ListPropertyMaxLength(100, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual IList<VmOpenApiLanguageItem> ServiceChannelNames { get; set; }

        /// <summary>
        /// List of localized urls.
        /// </summary>
        [JsonProperty(Order = 10)]
        [ListWithUrl("Value")]
        [ListPropertyMaxLength(500, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual IList<VmOpenApiLanguageItem> WebPage { get; set; }

        /// <summary>
        /// The accessibility classification level.
        /// </summary>
        [JsonProperty(Order = 26)]
        [ValidEnum(typeof(AccessibilityClassificationLevelTypeEnum))]
        public virtual string AccessibilityClassificationLevel { get; set; }

        /// <summary>
        /// The web content accessibility level.
        /// </summary>
        [JsonProperty(Order = 27)]
        [ValidEnum(typeof(WcagLevelTypeEnum))]
        public virtual string WCAGLevel { get; set; }

        /// <summary>
        /// List of accessibility web pages. One per language.
        /// </summary>
        [JsonProperty(Order = 28)]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual IList<V9VmOpenApiWebPage> AccessibilityStatementWebPage { get; set; } = new List<V9VmOpenApiWebPage>();

        /// <summary>
        /// List of service channel web pages.
        /// </summary>
        [JsonIgnore]
        public override IList<V9VmOpenApiWebPage> WebPages { get; set; }

        /// <summary>
        /// List of service channel service hours.
        /// </summary>
        [JsonIgnore]
        public override IList<V8VmOpenApiServiceHour> ServiceHours { get => base.ServiceHours; set => base.ServiceHours = value; }
        
        /// <summary>
        /// Set to true to delete all existing web pages for the service channel. The WebPages collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllWebPages { get => base.DeleteAllWebPages; set => base.DeleteAllWebPages = value; }

        /// <summary>
        /// Set to true to delete all existing service hours for the service channel. The ServiceHours collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllServiceHours
        {
            get
            {
                return base.DeleteAllServiceHours;
            }

            set
            {
                base.DeleteAllServiceHours = value;
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
        protected VmOpenApiWebPageChannelInVersionBase GetVersionBaseModel(int version)
        {
            var vm = GetServiceChannelModel<VmOpenApiWebPageChannelInVersionBase>(version);
            if (ServiceChannelNames?.Count > 0)
            {
                if (vm.ServiceChannelNamesWithType == null) { vm.ServiceChannelNamesWithType = new List<VmOpenApiLocalizedListItem>(); }
                ServiceChannelNames.ForEach(name => vm.ServiceChannelNamesWithType.Add(
                    new VmOpenApiLocalizedListItem { Value = name.Value, Language = name.Language, Type = NameTypeEnum.Name.ToString() }
                ));
            }
            vm.WebPage = WebPage;
            vm.AccessibilityClassificationLevel = AccessibilityClassificationLevel;
            vm.WCAGLevel = WCAGLevel;
            vm.AccessibilityStatementWebPage = AccessibilityStatementWebPage;
            return vm;
        }
        #endregion
    }
}
