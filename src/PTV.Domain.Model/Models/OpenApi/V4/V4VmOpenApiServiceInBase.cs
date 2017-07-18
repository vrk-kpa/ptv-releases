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
using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi.Extensions;

namespace PTV.Domain.Model.Models.OpenApi.V4
{
    /// <summary>
    /// OPEN API V4 - View Model of service for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V4.IV4VmOpenApiServiceInBase" />
    public class V4VmOpenApiServiceInBase : VmOpenApiServiceInVersionBase, IV4VmOpenApiServiceInBase
    {
        /// <summary>
        /// Service type. Possible values are: Service or PermissionAndObligation.
        /// NOTE! If service type has been defined within attached statutory service general description, the type for service is ignored.
        /// </summary>
        [AllowedValues("Type", new[] { "Service", "PermissionAndObligation" })]
        public override string Type { get => base.Type; set => base.Type = value; }

        /// <summary>
        /// Service funding type. Possible values are: PubliclyFunded or MarketFunded.
        /// </summary>
        [JsonIgnore]
        public override string FundingType { get => base.FundingType; set => base.FundingType = value; }

        /// <summary>
        /// Area type. 
        /// </summary>
        [JsonIgnore]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// Area codes related to sub area type. For example if SubAreaType = Municipality, Areas-list need to include municipality codes like 491 or 091.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiAreaIn> Areas { get => base.Areas; set => base.Areas = value; }

        /// <summary>
        /// Service channel publishing status. Values: Draft, Published, Deleted or Modified.
        /// </summary>
        [AllowedValues("PublishingStatus", new[] { "Draft", "Published", "Deleted", "Modified" })]
        [ValidEnum(null)] // PTV-1792: suppress base ValidEnum validation
        public override string PublishingStatus
        {
            get => base.PublishingStatus;
            set => base.PublishingStatus = value;
        }

        /// <summary>
        /// List of localized service keywords.
        /// </summary>
        [JsonProperty(Order = 21)]
        public new IList<VmOpenApiLanguageItem> Keywords { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of organizations responsible or producing the service.
        /// </summary>
        [JsonProperty(Order = 29)]
        [ListWithPropertyValueRequired("RoleType", "Responsible")]
        [ListWithPropertyValueRequired("RoleType", "Producer")]
        public new virtual IList<V4VmOpenApiServiceOrganization> ServiceOrganizations { get; set; }

        /// <summary>
        /// List of service vouchers.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiServiceVoucher> ServiceVouchers { get; set; }

        /// <summary>
        /// Set to true to delete service charge type (ServiceChargeType property for this object should be empty when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteServiceChargeType { get => base.DeleteServiceChargeType; set => base.DeleteServiceChargeType = value; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 4;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceInVersionBase VersionBase()
        {
            var vm = base.GetInVersionBaseModel<VmOpenApiServiceInVersionBase>();
            vm.ServiceOrganizations = new List<V6VmOpenApiServiceOrganizationIn>();
            vm.ServiceVouchers = new List<VmOpenApiServiceVoucher>();
            ServiceOrganizations.ForEach(s => vm.ServiceOrganizations.Add(s.ConvertToVersion6()));
            HandleServiceVouchersFor_V3_V4_V5(vm);
            vm.Keywords = Keywords.SetListValueLength(150);
            return vm;
        }
        #endregion
    }
}
