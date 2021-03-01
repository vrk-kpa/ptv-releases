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
using PTV.Framework.Attributes.ValidationAttributes;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of organization for IN api - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiOrganizationInVersionBase" />
    public class VmOpenApiOrganizationInVersionBase : VmOpenApiOrganizationBase, IVmOpenApiOrganizationInVersionBase
    {
        private IList<string> _availableLanguages;
        private IList<string> _requiredPropertiesAvailableLanguages;

        /// <summary>
        /// Organization OID. - must match the regex @"^[A-Za-z0-9.-]*$"
        /// </summary>
        [JsonProperty(Order = 2)]
//SFIPTV-806        [RegularExpression(@"^[A-Za-z0-9.-]*$")]
        [RegularExpression(DomainConstants.OidParser)]
        [MaxLength(100)]
        public new string Oid { get; set; }

        /// <summary>
        /// Municipality code (like 491 or 091).
        /// </summary>
        [JsonProperty(Order = 3)]
        [RequiredIf("OrganizationType", "Municipality")]
        [RegularExpression(ValidationConsts.Municipality)]
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
        /// List of addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public virtual IList<V9VmOpenApiAddressIn> Addresses { get; set; } = new List<V9VmOpenApiAddressIn>();

        /// <summary>
        /// Publishing status (Draft, Published, Deleted or Modified).
        /// </summary>
        [JsonProperty(Order = 26)]
        [ValidEnum(typeof(PublishingStatus))]
        [Required]
        public virtual string PublishingStatus { get; set; }

        /// <summary>
        /// Parent organization identifier.
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

// SOTE has been disabled (SFIPTV-1177)
//        /// <summary>
//        /// Responsible organization identifier.
//        /// </summary>
//        [JsonProperty(Order = 37)]
//        [ValidGuid]
//        public virtual string ResponsibleOrganizationId { get; set; }

        /// <summary>
        /// Date when item should be published.
        /// </summary>
        [JsonProperty(Order = 38)]
        [DateInFuture]
        public virtual DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Date when item should be archived.
        /// </summary>
        [JsonProperty(Order = 39)]
        [DateInFuture]
        public virtual DateTime? ValidTo { get; set; }

        /// <summary>
        /// Entity identifier.
        /// </summary>
        [JsonIgnore]
        public override Guid? Id { get; set; }

        /// <summary>
        /// Current version publishing status.
        /// </summary>
        [JsonIgnore]
        public string CurrentPublishingStatus { get; set; }

        /// <summary>
        /// User name.
        /// </summary>
        [JsonIgnore]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets available languages
        /// </summary>
        [JsonIgnore]
        public IList<string> AvailableLanguages
        {
            get
            {
                // Return available languages or calculate from organizationNames
                // SFIPTV-1913: All localized lists need to be taken into account: OrganizationNames, OrganizationDescriptions,
                // Emails, PhoneNumbers, Addresses and ElectronicInvoicings
                if (this._availableLanguages == null)
                {
                    var list = new HashSet<string>();
                    list.GetAvailableLanguages(this.OrganizationNames);
                    list.GetAvailableLanguages(this.OrganizationDescriptions);
                    list.GetAvailableLanguages(this.Emails);
                    list.GetPhoneAvailableLanguages(this.PhoneNumbers);
                    list.GetAvailableLanguages(this.Addresses);
                    list.GetAvailableLanguages(this.ElectronicInvoicings);

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
        /// Internal property to check the languages within required lists: OrganizationNames and OrganizationDescriptions
        /// </summary>
        [JsonIgnore]
        public IList<string> RequiredPropertiesAvailableLanguages
        {
            get
            {
                if (_requiredPropertiesAvailableLanguages == null)
                {
                    var list = new HashSet<string>();
                    list.GetAvailableLanguages(this.OrganizationNames);
                    list.GetAvailableLanguages(this.OrganizationDescriptions);

                    _requiredPropertiesAvailableLanguages = list.ToList();
                }

                return _requiredPropertiesAvailableLanguages;
            }
            set
            {
                _requiredPropertiesAvailableLanguages = value;
            }
        }

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
            vm.Addresses = this.Addresses;
            vm.ParentOrganizationId = this.ParentOrganizationId;
            vm.DeleteAllEmails = this.DeleteAllEmails;
            vm.DeleteAllPhones = this.DeleteAllPhones;
            vm.DeleteAllWebPages = this.DeleteAllWebPages;
            vm.DeleteAllAddresses = this.DeleteAllAddresses;
            vm.DeleteAllElectronicInvoicings = this.DeleteAllElectronicInvoicings;
            vm.PublishingStatus = this.PublishingStatus;
            vm.ValidFrom = this.ValidFrom;
            vm.ValidTo = this.ValidTo;
            vm.Id = this.Id;
            vm.CurrentPublishingStatus = this.CurrentPublishingStatus;

// SOTE has been disabled (SFIPTV-1177)
//            vm.ResponsibleOrganizationId = this.ResponsibleOrganizationId;

            vm.UserName = this.UserName;
            vm.AvailableLanguages = this.AvailableLanguages;

            return vm;
        }
        #endregion
    }
}
