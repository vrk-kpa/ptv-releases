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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    public class VmOpenApiServiceLocationChannelIn : V2VmOpenApiServiceLocationChannelIn, IVmOpenApiServiceLocationChannelIn
    {
        /// <summary>
        /// Service location contact e-mail address.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 12)]
        public string Email { get; set; }
        /// <summary>
        /// Service location contact phone number.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
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
        /// List of visiting addresses.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        [ListWithPropertyValueRequired("Type", "Visiting")]
        [JsonProperty(Order = 24)]
        public new IList<VmOpenApiAddressWithType> Addresses { get; set; }

        /// <summary>
        /// List of service hours.
        /// </summary>
        [JsonProperty(Order = 25)]
        public new IReadOnlyList<VmOpenApiServiceHour> ServiceHours { get; set; } = new List<VmOpenApiServiceHour>();

        [JsonIgnore]
        public virtual bool DeleteAllServiceChargeTypes { get; set; }

        [JsonIgnore]
        public virtual bool DeleteEmail { get; set; }

        [JsonIgnore]
        public virtual bool DeletePhone { get; set; }

        [JsonIgnore]
        public virtual bool DeleteFax { get; set; }

        [JsonIgnore]
        public override IList<VmOpenApiPhoneSimple> FaxNumbers
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

        [JsonIgnore]
        public override IList<VmOpenApiPhone> PhoneNumbers
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

        [JsonIgnore]
        public override bool DeleteAllServiceHours
        {
            get
            {
                return base.DeleteAllServiceHours;
            }

            set
            {
                base.DeleteAllServiceHours = value;
            }
        }

        [JsonIgnore]
        public override bool DeleteAllWebPages
        {
            get
            {
                return base.DeleteAllWebPages;
            }

            set
            {
                base.DeleteAllWebPages = value;
            }
        }

        [JsonIgnore]
        public virtual bool DeleteAllSupportContacts { get; set; }

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

        [JsonIgnore]
        public override IList<VmOpenApiPhone> SupportPhones
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
    }
}
