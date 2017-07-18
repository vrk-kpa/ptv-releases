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
using PTV.Domain.Model.Models.Interfaces.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi.V3
{
    /// <summary>
    /// OPEN API V3 - View Model of service
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V3.IV3VmOpenApiService" />
    public class V3VmOpenApiService : VmOpenApiServiceVersionBase, IV3VmOpenApiService
    {
        /// <summary>
        /// Service funding type. Possible values are: PubliclyFunded or MarketFunded.
        /// </summary>
        [JsonIgnore]
        public override string FundingType { get => base.FundingType; set => base.FundingType = value; }

        /// <summary>
        /// Area type (WholeCountry, WholeCountryExceptAlandIslands, AreaType). 
        /// </summary>
        [JsonIgnore]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// List of service areas.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiArea> Areas { get => base.Areas; set => base.Areas = value; }

        /// <summary>
        /// List of service classes.
        /// </summary>
        [JsonProperty(Order = 10)]
        public new IList<VmOpenApiFintoItem> ServiceClasses { get; set; } = new List<VmOpenApiFintoItem>();

        /// <summary>
        /// List of ontology terms.
        /// </summary>
        [JsonProperty(Order = 11)]
        public new IList<VmOpenApiFintoItem> OntologyTerms { get; set; } = new List<VmOpenApiFintoItem>();

        /// <summary>
        /// List of target groups.
        /// </summary>
        [JsonProperty(Order = 12)]
        public new IList<VmOpenApiFintoItem> TargetGroups { get; set; } = new List<VmOpenApiFintoItem>();

        /// <summary>
        /// List of life events.
        /// </summary>
        [JsonProperty(Order = 13)]
        public new IList<VmOpenApiFintoItem> LifeEvents { get; set; } = new List<VmOpenApiFintoItem>();

        /// <summary>
        /// List of industrial classes.
        /// </summary>
        [JsonProperty(Order = 14)]
        public new virtual IList<VmOpenApiFintoItem> IndustrialClasses { get; set; } = new List<VmOpenApiFintoItem>();

        /// <summary>
        /// List of linked service channels including relationship data.
        /// </summary>
        [JsonProperty(Order = 29)]
        public new IList<V3VmOpenApiServiceServiceChannel> ServiceChannels { get; set; } = new List<V3VmOpenApiServiceServiceChannel>();

        /// <summary>
        /// List of organizations, responsible and producer organizations of the service.
        /// </summary>
        [JsonProperty(Order = 30)]
        public new IList<V3VmOpenApiServiceOrganization> Organizations { get; set; } = new List<V3VmOpenApiServiceOrganization>();

        /// <summary>
        /// List of laws related to the service.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiLaw> Legislation { get; set; }

        /// <summary>
        /// Date when item was modified/created..
        /// </summary>
        [JsonIgnore]
        public override DateTime Modified { get => base.Modified; set => base.Modified = value; }

        /// <summary>
        /// List of service vouchers.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiServiceVoucher> ServiceVouchers { get; set; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>versio number</returns>
        public override int VersionNumber()
        {
            return 3;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous</returns>
        public override IVmOpenApiServiceVersionBase PreviousVersion()
        {
            throw new NotSupportedException("No previous version for class V3VmOpenApiService!");
        }
        #endregion
    }
}
