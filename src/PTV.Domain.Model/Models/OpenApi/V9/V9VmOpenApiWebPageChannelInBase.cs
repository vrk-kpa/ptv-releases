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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Attributes;
using PTV.Framework.Attributes.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi.V9
{
    /// <summary>
    /// OPEN API V9 - View Model of web page channel for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiWebPageChannelInVersionBase" />
    public class V9VmOpenApiWebPageChannelInBase : VmOpenApiWebPageChannelInVersionBase
    {
        /// <summary>
        /// List of localized service channel descriptions. Possible type values are: Summary, Description.
        /// </summary>
        [JsonProperty(Order = 5)]
        [ListValueNotEmpty("Value")]
        [ListPropertyAllowedValues(propertyName: "Type", allowedValues: new[] { "Description", "Summary" })]
        [ListPropertyMaxLength(150, "Value", "Summary")]
        public override IList<VmOpenApiLocalizedListItem> ServiceChannelDescriptions { get => base.ServiceChannelDescriptions; set => base.ServiceChannelDescriptions = value; }

        /// <summary>
        /// Area type (WholeCountry, WholeCountryExceptAlandIslands, AreaType).
        /// </summary>
        [JsonIgnore]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// List of areas. List can contain different types of areas.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiAreaIn> Areas { get => base.Areas; set => base.Areas = value; }

        /// <summary>
        /// List of support phone numbers for the service channel.
        /// </summary>
        [ListWithOpenApiEnum(typeof(ServiceChargeTypeEnum), "ServiceChargeType")]
        public override IList<V4VmOpenApiPhone> SupportPhones { get => base.SupportPhones; set => base.SupportPhones = value; }

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
        /// Service channel publishing status. Values: Draft, Published, Deleted or Modified.
        /// </summary>
        [AllowedValues("PublishingStatus", new[] { "Draft", "Published", "Deleted", "Modified" })]
        [ValidEnum(null)] // PTV-1792: suppress base ValidEnum validation
        public override string PublishingStatus { get => base.PublishingStatus; set => base.PublishingStatus = value; }

        /// <summary>
        /// The accessibility classification.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiAccessibilityClassification> AccessibilityClassification { get => base.AccessibilityClassification; set => base.AccessibilityClassification = value; }

        #region Methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 9;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
            var vm = base.GetVersionBaseModel(VersionNumber());
            // SFIPTV-850
            if (!this.AccessibilityClassificationLevel.IsNullOrEmpty() || !this.WCAGLevel.IsNullOrEmpty() || this.AccessibilityStatementWebPage?.Count > 0)
            {
                vm.AccessibilityClassification = new List<VmOpenApiAccessibilityClassification>();
                vm.AvailableLanguages.ForEach(language => {
                    var webPage = this.AccessibilityStatementWebPage.FirstOrDefault(w => w.Language == language);
                    vm.AccessibilityClassification.Add(new VmOpenApiAccessibilityClassification
                    {
                        AccessibilityClassificationLevel = this.AccessibilityClassificationLevel,
                        WcagLevel = this.WCAGLevel,
                        AccessibilityStatementWebPageName = webPage?.Value,
                        AccessibilityStatementWebPage = webPage?.Url,
                        Language = language
                    });
                });
            }
            return vm;
        }
        #endregion
    }
}
