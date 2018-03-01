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
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of organization for IN api - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiOrganizationInVersionBase" />
    public class VmOpenApiOrganizationInVersionBase : VmOpenApiOrganizationBase, IVmOpenApiOrganizationInVersionBase
    {
        /// <summary>
        /// Organization OID. - must match the regex @"^[A-Za-z0-9.-]*$"
        /// </summary>
        [JsonProperty(Order = 2)]
        [RegularExpression(@"^[A-Za-z0-9.-]*$")]
        [MaxLength(100)]
        public new string Oid { get; set; }

        /// <summary>
        /// Municipality code (like 491 or 091).
        /// </summary>
        [JsonProperty(Order = 3)]
        [RequiredIf("OrganizationType", "Municipality")]
        [RegularExpression(@"^[0-9]{1,3}$")]
        public virtual string Municipality { get; set; }

        /// <summary>
        /// Sub area type (Municipality, Province, BusinessRegions, HospitalRegions).
        /// </summary>
        [RequiredIf("AreaType", "AreaType")]
        [JsonProperty(Order = 17)]
        public virtual string SubAreaType { get; set; }

        /// <summary>
        /// Area codes related to sub area type. For example if SubAreaType = Municipality, Areas-list need to include municipality codes like 491 or 091.
        /// </summary>
        [JsonProperty(Order = 18)]
        public virtual IList<string> Areas { get; set; }

        /// <summary>
        /// List of email addresses.
        /// </summary>
        [JsonProperty(Order = 21)]
        [EmailAddressList("Value")]
        public virtual IList<V4VmOpenApiEmail> Emails { get; set; } = new List<V4VmOpenApiEmail>();

        /// <summary>
        /// List of addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public IList<V7VmOpenApiAddressWithForeignIn> Addresses { get; set; } = new List<V7VmOpenApiAddressWithForeignIn>();

        /// <summary>
        /// Parent organization identifier if exists.
        /// </summary>
        [JsonProperty(Order = 30)]
        [ValidGuid]
        public string ParentOrganizationId { get; set; }

        /// <summary>
        /// Set to true to delete all existing emails (the EmailAddresses collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonProperty(Order = 31)]
        public virtual bool DeleteAllEmails { get; set; }

        /// <summary>
        /// Set to true to delete all existing phone numbers (the PhoneNumbers collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonProperty(Order = 32)]
        public virtual bool DeleteAllPhones { get; set; }

        /// <summary>
        /// Set to true to delete all existing web pages (the WebPages collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonProperty(Order = 33)]
        public virtual bool DeleteAllWebPages { get; set; }

        /// <summary>
        /// Set to true to delete all existing addresses (the Addresses collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonProperty(Order = 34)]
        public virtual bool DeleteAllAddresses { get; set; }

        /// <summary>
        /// Set to true to delete all existing electronic invoicing addresses (the ElectronicInvoicings collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonProperty(Order = 36)]
        public virtual bool DeleteAllElectronicInvoicings { get; set; }

        /// <summary>
        /// Entity identifier.
        /// </summary>
        [JsonIgnore]
        public override Guid? Id { get; set;  }

        /// <summary>
        /// Current version publishing status.
        /// </summary>
        [JsonIgnore]
        public string CurrentPublishingStatus { get; set; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>
        /// version number
        /// </returns>
        public virtual int VersionNumber()
        {
            return 0;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>
        /// view model of base
        /// </returns>
        /// <exception cref="System.NotImplementedException">VmOpenApiOrganizationInVersionBase does not have next version available!</exception>
        public virtual IVmOpenApiOrganizationInVersionBase VersionBase()
        {
            throw new NotImplementedException("VmOpenApiOrganizationInVersionBase does not have next version available!");
        }

        /// <summary>
        /// Gets the in base version model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>in base version model</returns>
        protected TModel GetInVersionBaseModel<TModel>(int version) where TModel : IVmOpenApiOrganizationInVersionBase, new()
        {
            var vm = base.GetBaseModel<TModel>();
            vm.Oid = this.Oid;
            vm.Municipality = this.Municipality;
            vm.OrganizationDescriptions = this.OrganizationDescriptions;
            vm.SubAreaType = this.SubAreaType;
            vm.Areas = this.Areas;
            vm.Emails = this.Emails;
            vm.Addresses = this.Addresses;
            vm.ParentOrganizationId = this.ParentOrganizationId;
            vm.DeleteAllEmails = this.DeleteAllEmails;
            vm.DeleteAllPhones = this.DeleteAllPhones;
            vm.DeleteAllWebPages = this.DeleteAllWebPages;
            vm.DeleteAllAddresses = this.DeleteAllAddresses;
            vm.DeleteAllElectronicInvoicings = this.DeleteAllElectronicInvoicings;
            vm.Id = this.Id;
            vm.CurrentPublishingStatus = this.CurrentPublishingStatus;

            // Convert new open api values into enums used within db (PTV-2184)
            if (version > 7)
            {
                vm.OrganizationNames.ForEach(n => n.Type = n.Type.GetEnumValueByOpenApiEnumValue<NameTypeEnum>());
                vm.DisplayNameType.ForEach(d => d.Type = d.Type.GetEnumValueByOpenApiEnumValue<NameTypeEnum>());
                vm.OrganizationDescriptions.ForEach(d => d.Type = d.Type.GetEnumValueByOpenApiEnumValue<DescriptionTypeEnum>());
                vm.AreaType = string.IsNullOrEmpty(vm.AreaType) ? vm.AreaType : vm.AreaType.GetEnumValueByOpenApiEnumValue<AreaInformationTypeEnum>();
                vm.SubAreaType = string.IsNullOrEmpty(vm.SubAreaType) ? vm.SubAreaType : vm.SubAreaType.GetEnumValueByOpenApiEnumValue<AreaTypeEnum>();
                vm.PhoneNumbers.ForEach(p => p.ServiceChargeType = string.IsNullOrEmpty(p.ServiceChargeType) ?
                    p.ServiceChargeType :
                    p.ServiceChargeType.GetEnumValueByOpenApiEnumValue<ServiceChargeTypeEnum>());
            }
            return vm;
        }
        #endregion
    }
}
