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

using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;
using System;

namespace PTV.Domain.Model.Models.OpenApi.V6
{
    /// <summary>
    /// OPEN API V6 - View Model of electronic channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiElectronicChannelVersionBase" />
    public class V6VmOpenApiElectronicChannel : VmOpenApiElectronicChannelVersionBase
    {
        /// <summary>
        /// Localized list of channel urls.
        /// </summary>
        [Display(Name = "Urls")]
        [JsonProperty(Order = 13, PropertyName = "urls")]
        public virtual IList<VmOpenApiLanguageItem> Urls { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of service channel web pages.
        /// </summary>
        [JsonProperty(Order = 19)]
        public virtual new IList<VmOpenApiWebPageWithOrderNumber> WebPages { get; set; } = new List<VmOpenApiWebPageWithOrderNumber>();

        /// <summary>
        /// List of service channel service hours.
        /// </summary>
        [JsonProperty(Order = 25)]
        public virtual new IList<V4VmOpenApiServiceHour> ServiceHours { get; set; } = new List<V4VmOpenApiServiceHour>();

        /// <summary>
        /// External system identifier for this service channel.
        /// </summary>
        [JsonIgnore]
        public override string SourceId { get => base.SourceId; set => base.SourceId = value; }

        /// <summary>
        /// List of linked services including relationship data.
        /// </summary>
        [JsonIgnore]
        public override IList<V9VmOpenApiServiceChannelService> Services { get => base.Services; set => base.Services = value; }

        /// <summary>
        /// The accessibility classification level.
        /// </summary>
        [JsonIgnore]
        public override string AccessibilityClassificationLevel { get => base.AccessibilityClassificationLevel; set => base.AccessibilityClassificationLevel = value; }

        /// <summary>
        /// The web content accessibility level.
        /// </summary>
        [JsonIgnore]
        public override string WCAGLevel { get => base.WCAGLevel; set => base.WCAGLevel = value; }

        /// <summary>
        /// List of accessibility statement web pages. One per language.
        /// </summary>
        [JsonIgnore]
        public override IList<V9VmOpenApiWebPage> AccessibilityStatementWebPage { get => base.AccessibilityStatementWebPage; set => base.AccessibilityStatementWebPage = value; }

        #region methods

        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 6;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceChannel PreviousVersion()
        {
            throw new NotSupportedException("No previous version for class V6VmOpenApiElectronicChannel!");
        }
        #endregion
    }
}
