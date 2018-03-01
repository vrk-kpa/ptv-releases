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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using PTV.Framework.Attributes;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service location channel for IN api - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceChannelIn" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceLocationChannelInVersionBase" />
    public class VmOpenApiServiceLocationChannelInVersionBase : VmOpenApiServiceChannelIn, IVmOpenApiServiceLocationChannelInVersionBase
    {
        /// <summary>
        /// Is the service location channel restricted by service area.
        /// </summary>
        [JsonProperty(Order = 10)]
        public virtual bool ServiceAreaRestricted { get; set; }

        /// <summary>
        /// List of serviceareas. Used when location service channel is restricted by service area (ServiceAreaRestricted=true). List should contain municipality codes.
        /// </summary>
        [JsonProperty(Order = 11)]
        [ListRequiredIf("ServiceAreaRestricted", true)]
        public virtual IList<string> ServiceAreas { get; set; } = new List<string>();

        /// <summary>
        /// List email addresses for the service channel.
        /// </summary>
        [JsonProperty(PropertyName = "emails", Order = 12)]
        public override IList<VmOpenApiLanguageItem> SupportEmails
        {
            get
            {
                return base.SupportEmails;
            }

            set
            {
                base.SupportEmails = value;
            }
        }

        /// <summary>
        /// Service location contact fax numbers.
        /// </summary>
        [JsonProperty(Order = 13)]
        public virtual IList<V4VmOpenApiPhoneSimple> FaxNumbers { get; set; } = new List<V4VmOpenApiPhoneSimple>();

        /// <summary>
        /// List of phone numbers for the service channel. Includes also fax numbers.
        /// </summary>
        [JsonProperty(Order = 14)]
        public virtual IList<V4VmOpenApiPhone> PhoneNumbers { get; set; } = new List<V4VmOpenApiPhone>();

        /// <summary>
        /// List of service location addresses.
        /// </summary>
        [JsonProperty(Order = 24)]
        public virtual IList<V7VmOpenApiAddressWithMovingIn> Addresses { get; set; } = new List<V7VmOpenApiAddressWithMovingIn>();

        /// <summary>
        /// Set to true to delete emails. The email property should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(PropertyName = "deleteAllEmails", Order = 26)]
        public override bool DeleteAllSupportEmails
        {
            get
            {
                return base.DeleteAllSupportEmails;
            }

            set
            {
                base.DeleteAllSupportEmails = value;
            }
        }

        /// <summary>
        /// Set to true to delete phone number. The prohone property should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 27)]
        public virtual bool DeleteAllPhoneNumbers { get; set; }

        /// <summary>
        /// Set to true to delete fax number. The fax property should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 28)]
        public virtual bool DeleteAllFaxNumbers { get; set; }

        /// <summary>
        /// List of support phone numbers for the service channel.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiPhone> SupportPhones
        {
            get
            {
                return base.SupportPhones;
            }

            set
            {
                base.SupportPhones = value;
            }
        }

        /// <summary>
        /// Set to true to delete all existing support phone numbers for the service channel. The SupportPhones collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllSupportPhones
        {
            get
            {
                return base.DeleteAllSupportPhones;
            }

            set
            {
                base.DeleteAllSupportPhones = value;
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
        /// Gets the version base model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>base version model</returns>
        protected TModel GetVersionBaseModel<TModel>(int version) where TModel : IVmOpenApiServiceLocationChannelInVersionBase, new()
        {
            var vm = GetServiceChannelModel<TModel>(version);
            vm.SupportEmails = SupportEmails;
            vm.FaxNumbers = FaxNumbers;
            vm.PhoneNumbers = PhoneNumbers;
            vm.Addresses = Addresses;
            // set foreign addresses - PTV-2910
            var foreignAddresses = vm.Addresses.Where(a => a.LocationAbroad != null).ToList();
            foreignAddresses.ForEach(a => a.ForeignAddress = a.LocationAbroad);

            vm.DeleteAllSupportEmails = DeleteAllSupportEmails;
            vm.DeleteAllPhoneNumbers = DeleteAllPhoneNumbers;
            vm.DeleteAllFaxNumbers = DeleteAllFaxNumbers;

            // Set values from old properties serviceAreaRestricted and serviceAreas
            if (ServiceAreaRestricted && ServiceAreas.Count > 0)
            {
                vm.AreaType = AreaInformationTypeEnum.AreaType.ToString();
                vm.Areas.Add(new VmOpenApiAreaIn { Type = AreaTypeEnum.Municipality.ToString(), AreaCodes = ServiceAreas });
            }

            // Convert new open api values into enums used within db (PTV-2184)
            if (version > 7)
            {
                vm.PhoneNumbers?.ForEach(p => p.ServiceChargeType = string.IsNullOrEmpty(p.ServiceChargeType) ? p.ServiceChargeType : p.ServiceChargeType.GetEnumValueByOpenApiEnumValue<ServiceChargeTypeEnum>());
            }
            return vm;
        }
        #endregion
    }
}
