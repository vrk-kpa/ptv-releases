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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Attributes;
using PTV.Framework.Attributes.ValidationAttributes;
using PTV.Framework.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        private IList<string> _availableLanguages;
        private IList<string> _requiredPropertiesAvailableLanguages;

        // SFIPTV-1963
        /// <summary>
        /// Service channel OID. Must match the regex @"^[A-Za-z0-9.-]*$".
        /// NOTICE! At the moment the property is only a placeholder. The data is not saved into database!
        /// </summary>
        [JsonProperty(Order = 3)]
        [RegularExpression(ValidationConsts.Oid)]
        public virtual string Oid { get; set; }

        // SFIPTV-236
        /// <summary>
        /// Localized list of service channel names. Possible type values are: Name, AlternativeName.
        /// </summary>
        [JsonProperty(Order = 5)]
        [ListPropertyMaxLength(100, "Value")]
        [LocalizedListPropertyDuplicityForbidden("Type")]
        [ListWithOpenApiEnum(typeof(NameTypeEnum), "Type")]
        public virtual IList<VmOpenApiLocalizedListItem> ServiceChannelNames { get; set; }

        /// <summary>
        /// List of Display name types (Name or AlternativeName) for each language version of ServiceChannelNames.
        /// </summary>
        [JsonProperty(Order = 7)]
        [ListWithOpenApiEnum(typeof(NameTypeEnum), "Type")]
        public virtual IList<VmOpenApiNameTypeByLanguage> DisplayNameType { get; set; } = new List<VmOpenApiNameTypeByLanguage>();

        /// <summary>
        /// List email addresses for the service channel.
        /// </summary>
        [JsonProperty(Order = 12)]
        [EmailAddressList("Value")]
        public virtual IList<VmOpenApiLanguageItem> Emails { get; set; }

        /// <summary>
        /// List email addresses for the service channel.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiLanguageItem> SupportEmails { get => base.SupportEmails; set => base.SupportEmails = value; }

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
        public virtual IList<V9VmOpenApiAddressLocationIn> Addresses { get; set; } = new List<V9VmOpenApiAddressLocationIn>();

        /// <summary>
        /// Set to true to delete emails. The email property should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(PropertyName = "deleteAllEmails", Order = 26)]
        public override bool DeleteAllSupportEmails { get => base.DeleteAllSupportEmails; set => base.DeleteAllSupportEmails = value; }

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
        /// Set to true to delete OID. The Oid property should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 29)]
        public virtual bool DeleteOid { get; set; }

        /// <summary>
        /// List of support phone numbers for the service channel.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiPhone> SupportPhones { get; set; }

        /// <summary>
        /// Set to true to delete all existing support phone numbers for the service channel. The SupportPhones collection should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllSupportPhones { get; set; }

        /// <summary>
        /// Gets or sets available languages
        /// </summary>
        [JsonIgnore]
        public override IList<string> AvailableLanguages
        {
            get
            {
                // Return available languages or calculate from organizationNames
                // SFIPTV-1913: All localized lists need to be taken into account: ServiceChannelNames, ServiceChannelDescriptions,
                // DisplayNameType, Emails, PhoneNumbers, FaxNumbers, WebPages, ServiceHours lists.
                if (this._availableLanguages == null)
                {
                    var list = new HashSet<string>();
                    list.GetAvailableLanguages(this.ServiceChannelNames);
                    list.GetAvailableLanguages(this.ServiceChannelNamesWithType);
                    list.GetAvailableLanguages(this.ServiceChannelDescriptions);
                    list.GetAvailableLanguages(this.DisplayNameType);
                    list.GetAvailableLanguages(this.Addresses);
                    list.GetAvailableLanguages(this.Emails);
                    list.GetPhoneAvailableLanguages(this.PhoneNumbers);
                    list.GetPhoneAvailableLanguages(this.FaxNumbers);
                    list.GetAvailableLanguages(this.WebPages);
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
        /// Internal property to check the languages within required lists: ServiceChannelNames, ServiceChannelDescriptions
        /// and ChannelUrls lists.
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
                    list.GetAvailableLanguages(this.Addresses);

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
        /// Gets the version base model.
        /// </summary>
        /// <returns>base version model</returns>
        protected VmOpenApiServiceLocationChannelInVersionBase GetVersionBaseModel(int version)
        {
            var vm = GetServiceChannelModel<VmOpenApiServiceLocationChannelInVersionBase>(version);
            vm.Oid = Oid;
            vm.ServiceChannelNamesWithType = this.ServiceChannelNames;
            vm.AvailableLanguages = AvailableLanguages;
            vm.DisplayNameType = DisplayNameType;
            vm.SupportEmails = Emails;
            vm.FaxNumbers = FaxNumbers;
            vm.PhoneNumbers = PhoneNumbers;
            vm.Addresses = Addresses;
            // set foreign addresses - PTV-2910
            var foreignAddresses = vm.Addresses.Where(a => a.LocationAbroad != null).ToList();
            foreignAddresses.ForEach(a => a.ForeignAddress = a.LocationAbroad);

            vm.DeleteAllSupportEmails = DeleteAllSupportEmails;
            vm.DeleteAllPhoneNumbers = DeleteAllPhoneNumbers;
            vm.DeleteAllFaxNumbers = DeleteAllFaxNumbers;
            vm.DeleteOid = DeleteOid;

            return vm;
        }

        /// <summary>
        /// Handles addresses
        /// </summary>
        /// <param name="addresses"></param>
        public void HandleAddresses(IList<V7VmOpenApiAddressWithMovingIn> addresses)
        {
            // Multipoint addresses do not exist anymore. PTV-2470
            // Multipoint addresses needs to be handeled differently since one multipoint address will generate several single addresses.
            addresses.ForEach(address =>
            {
                if (address.SubType  == AddressConsts.MULTIPOINT)
                {
                    address.MultipointLocation?.ForEach(multipoint => this.Addresses.Add(new V9VmOpenApiAddressLocationIn
                    {
                        Type = address.Type,
                        SubType = AddressConsts.SINGLE,
                        StreetAddress = multipoint,
                        Country = address.Country,
                        OrderNumber = address.OrderNumber
                    }));
                }
                else
                {
                    this.Addresses.Add(address.ConvertToInBaseModel());
                }
            });
        }

        /// <summary>
        /// Set address properties
        /// </summary>
        public void SetAddressProperties()
        {
            if (Addresses == null) return;

            Addresses.ForEach(a =>
            {
                if (a.Type == AddressConsts.LOCATION)
                {
                    a.Type = AddressCharacterEnum.Visiting.ToString();
                }

                switch (a.SubType)
                {
                    case AddressConsts.ABROAD:
                        a.SubType = AddressTypeEnum.Foreign.ToString();
                        break;
                    case AddressConsts.SINGLE:
                        a.SubType = AddressTypeEnum.Street.ToString();
                        break;
                    default:
                        break;
                }
            });
        }

        /// <summary>
        /// Handles service location channel names for older versions (9).
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="names"></param>
        protected void HandleNames(IVmOpenApiServiceLocationChannelInVersionBase vm, IList<VmOpenApiLanguageItem> names)
        {
            // SFIPTV-236
            if (names != null)
            {
                if (vm.ServiceChannelNamesWithType == null)
                {
                    vm.ServiceChannelNamesWithType = new List<VmOpenApiLocalizedListItem>();
                }
                names.ForEach(n => vm.ServiceChannelNamesWithType.Add(new VmOpenApiLocalizedListItem
                { Value = n.Value, Language = n.Language, Type = NameTypeEnum.Name.ToString() }));
                // Set DisplayNameType as Name by default
                if (vm.DisplayNameType == null)
                {
                    vm.DisplayNameType = new List<VmOpenApiNameTypeByLanguage>();
                }

                names.Select(n => n.Language).Distinct().ToList().ForEach(d => vm.DisplayNameType.Add(new VmOpenApiNameTypeByLanguage
                {
                    Language = d,
                    Type = NameTypeEnum.Name.ToString()
                }));
            }
        }
        #endregion
    }
}
