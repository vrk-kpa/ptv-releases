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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V1;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Domain.Model.Models.OpenApi.V1
{
    /// <summary>
    /// OPEN API - View Model of service location channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceLocationChannelInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V1.IVmOpenApiServiceLocationChannelInBase" />
    public class VmOpenApiServiceLocationChannelInBase : VmOpenApiServiceLocationChannelInVersionBase, IVmOpenApiServiceLocationChannelInBase
    {
        /// <summary>
        /// List of localized service channel descriptions.
        /// </summary>
        [ListPropertyMaxLength(4000, "Value", "Description")]
        public override IList<VmOpenApiLocalizedListItem> ServiceChannelDescriptions
        {
            get
            {
                return base.ServiceChannelDescriptions;
            }

            set
            {
                base.ServiceChannelDescriptions = value;
            }
        }

        /// <summary>
        /// Service location contact e-mail address.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 12)]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Service location contact phone number.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [RegularExpression(@"^\d{1,20}$")]
        [JsonProperty(Order = 13)]
        public string Phone { get; set; }

        /// <summary>
        /// Service location contact fax number.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 14)]
        public string Fax { get; set; }

        /// <summary>
        /// List of service charge types. Valid values are: Charged, Free and Other.
        /// </summary>
        [JsonProperty(Order = 16)]
        [ListWithEnum(typeof(ServiceChargeTypeEnum))]
        public IReadOnlyList<string> ServiceChargeTypes { get; set; } = new List<string>();

        /// <summary>
        /// Localized phone charge description. Required when ServiceChargeTypes collection contains value Other.
        /// </summary>
        [JsonProperty(Order = 17)]
        [ListPropertyMaxLength(150, "Value")]
        [ListRequiredIf("ServiceChargeTypes", "Other")]
        public IReadOnlyList<VmOpenApiLanguageItem> PhoneChargeDescriptions { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of service channel web pages.
        /// </summary>
        [JsonProperty(Order = 19)]
        public new IList<VmOpenApiWebPage> WebPages { get; set; } = new List<VmOpenApiWebPage>();

        /// <summary>
        /// Service location latitude coordinate. This property is not used in the API anymore. Do not use
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 20)]
        public string Latitude { get; set; }

        /// <summary>
        /// Service location longitude coordinate. This property is not used in the API anymore. Do not use
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 21)]
        public string Longitude { get; set; }

        /// <summary>
        /// Coordinate system used. This property is not used in the API anymore. Do not use
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 22)]
        public string CoordinateSystem { get; set; }

        /// <summary>
        /// Are coordinates set manually. This property is not used in the API anymore. Do not use
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 23)]
        public bool CoordinatesSetManually { get; set; }

        /// <summary>
        /// List of service location addresses.
        /// </summary>
        [JsonProperty(Order = 24)]
        public new virtual IList<VmOpenApiAddressWithType> Addresses { get; set; }

        /// <summary>
        /// List of service channel service hours.
        /// </summary>
        [JsonProperty(Order = 25)]
        public new IList<VmOpenApiServiceHour> ServiceHours { get; set; } = new List<VmOpenApiServiceHour>();

        /// <summary>
        /// Set to true to delete email. The email property should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 26)]
        public virtual bool DeleteEmail { get; set; }

        /// <summary>
        /// Set to true to delete phone number. The prohone property should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 27)]
        public virtual bool DeletePhone { get; set; }

        /// <summary>
        /// Set to true to delete fax number. The fax property should be empty when this property is set to true.
        /// </summary>
        [JsonProperty(Order = 28)]
        public virtual bool DeleteFax { get; set; }

        /// <summary>
        /// Set to true to delete all existing service charge types. This property is not used in the API anymore. Do not use.
        /// </summary>
        [JsonProperty(Order = 29)]
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        public virtual bool DeleteAllServiceChargeTypes { get; set; }

        /// <summary>
        /// Service location contact fax numbers.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiPhoneSimple> FaxNumbers
        {
            get
            {
                return base.FaxNumbers;
            }

            set
            {
                base.FaxNumbers = value;
            }
        }

        /// <summary>
        /// Set to true to delete fax number. The fax property should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllFaxNumbers
        {
            get
            {
                return base.DeleteAllFaxNumbers;
            }

            set
            {
                base.DeleteAllFaxNumbers = value;
            }
        }

        /// <summary>
        /// List of phone numbers for the service channel. Includes also fax numbers.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiPhone> PhoneNumbers
        {
            get
            {
                return base.PhoneNumbers;
            }

            set
            {
                base.PhoneNumbers = value;
            }
        }

        /// <summary>
        /// Set to true to delete phone number. The prohone property should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllPhoneNumbers
        {
            get
            {
                return base.DeleteAllPhoneNumbers;
            }

            set
            {
                base.DeleteAllPhoneNumbers = value;
            }
        }

        /// <summary>
        /// List email addresses for the service channel.
        /// </summary>
        [JsonIgnore]
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
        /// Set to true to delete emails. The email property should be empty when this property is set to true.
        /// </summary>
        [JsonIgnore]
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
        /// Set to true to delete all existing support phones for the service channel. The SupportPhones collection should be empty when this property is set to true.
        /// </summary>
        /// <value>
        /// <c>true</c> if all support phones should be deleted; otherwise, <c>false</c>.
        /// </value>
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

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 1;
        }

        /// <summary>
        /// Gets the version base.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceChannelIn VersionBase()
        {
#pragma warning disable 612, 618
            var vm = base.GetVersionBaseModel<VmOpenApiServiceLocationChannelInVersionBase>();
            vm.ServiceChannelDescriptions = ServiceChannelDescriptions.SetListValueLength();
            WebPages.ForEach(w => vm.WebPages.Add(w.ConvertToWebPageWithOrderNumber()));
            if (!string.IsNullOrEmpty(Email))
            {
                vm.SupportEmails.Add(new VmOpenApiEmail() { Value = Email.SetStringValueLength(100), Language = LanguageCode.fi.ToString() });
            }
            // Set the phone numebers and the data related to it (service charge type, phone charge description etc...)
            if (!string.IsNullOrEmpty(Phone))
            {
                if (ServiceChargeTypes?.Count == 0)
                {
                    ServiceChargeTypes = new List<string>() { ServiceChargeTypeEnum.Charged.ToString() };
                }
                if (PhoneChargeDescriptions?.Count > 0)
                {
                    PhoneChargeDescriptions.ForEach(d =>
                    {
                        vm.PhoneNumbers.Add(new V4VmOpenApiPhone()
                        {
                            Number = Phone,
                            Language = d.Language,
                            ServiceChargeType = ServiceChargeTypes.FirstOrDefault(),
                            ChargeDescription = d.Value
                        });
                    });
                }
                else
                {
                    vm.PhoneNumbers.Add(new V4VmOpenApiPhone()
                    {
                        Number = Phone,
                        Language = LanguageCode.fi.ToString(),
                        ServiceChargeType = ServiceChargeTypes.FirstOrDefault()
                    });
                }
            }

            // Set fax
            if (!string.IsNullOrEmpty(Fax))
            {
                vm.FaxNumbers.Add(new V4VmOpenApiPhoneSimple() { Number = Fax, Language = LanguageCode.fi.ToString() });
            }

            vm.Addresses = new List<V4VmOpenApiAddressWithTypeIn>();
            Addresses.ForEach(a => vm.Addresses.Add(a.ConvertToVersion2()));
            vm.DeleteAllSupportEmails = DeleteEmail;
            vm.DeleteAllPhoneNumbers = DeletePhone;
            vm.DeleteAllFaxNumbers = DeleteFax;
            vm.ServiceHours = TranslateToV4ServiceHours(ServiceHours);
            return vm;
#pragma warning restore 612, 618
        }
        #endregion
    }
}
