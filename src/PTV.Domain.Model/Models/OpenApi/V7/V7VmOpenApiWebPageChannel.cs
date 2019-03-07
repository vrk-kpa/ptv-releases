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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V7
{
    /// <summary>
    /// OPEN API V7 - View Model of web page channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiWebPageChannelVersionBase" />
    public class V7VmOpenApiWebPageChannel : VmOpenApiWebPageChannelVersionBase
    {
        /// <summary>
        /// List of localized urls.
        /// </summary>
        [JsonProperty(Order = 10)]
        public IList<VmOpenApiLanguageItem> Urls { get; set; } = new List<VmOpenApiLanguageItem>();

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
        /// List of linked services including relationship data.
        /// </summary>
        [JsonProperty(Order = 29)]
        public virtual new IList<VmOpenApiServiceChannelService> Services { get; set; } = new List<VmOpenApiServiceChannelService>();

        /// <summary>
        /// Area type (WholeCountry, WholeCountryExceptAlandIslands, AreaType). 
        /// </summary>
        [JsonIgnore]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// List of service channel areas.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiArea> Areas { get => base.Areas; set => base.Areas = value; }

        /// <summary>
        /// The accessibility classification.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiAccessibilityClassification> AccessibilityClassification { get => base.AccessibilityClassification; set => base.AccessibilityClassification = value; }

        /// <summary>
        /// List of ontology terms related to the service.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiFintoItem> OntologyTerms { get => base.OntologyTerms; set => base.OntologyTerms = value; }

        /// <summary>
        /// Sote organization that is responsible for the service channel. Notice! At the moment always empty - the property is a placeholder for later use.
        /// </summary>
        [JsonIgnore]
        public override string ResponsibleSoteOrganization { get => base.ResponsibleSoteOrganization; set => base.ResponsibleSoteOrganization = value; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 7;
        }
        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceChannel PreviousVersion()
        {
            throw new NotSupportedException("No previous version for class V7VmOpenApiWebPageChannel!");
        }

        #endregion
    }
}
