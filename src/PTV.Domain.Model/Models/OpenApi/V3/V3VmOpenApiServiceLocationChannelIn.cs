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
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V3
{
    /// <summary>
    /// OPEN API V3 - View Model of service location channel for IN api
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.V3.V3VmOpenApiServiceLocationChannelInBase" />
    public class V3VmOpenApiServiceLocationChannelIn : V3VmOpenApiServiceLocationChannelInBase
    {
        /// <summary>
        /// PTV organization identifier of organization responsible for this channel.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public override string OrganizationId
        {
            get
            {
                return base.OrganizationId;
            }

            set
            {
                base.OrganizationId = value;
            }
        }

        /// <summary>
        /// List of localized service channel names.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        public override IList<VmOpenApiLanguageItem> ServiceChannelNames
        {
            get
            {
                return base.ServiceChannelNames;
            }

            set
            {
                base.ServiceChannelNames = value;
            }
        }

        /// <summary>
        /// List of localized service channel descriptions.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        [ListWithPropertyValueRequired("Type", "ShortDescription")]
        [ListWithPropertyValueRequired("Type", "Description")]
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
        /// List of languages the service channel is available in (two letter language code).
        /// </summary>
        [ListRequired]
        public override IList<string> Languages
        {
            get
            {
                return base.Languages;
            }

            set
            {
                base.Languages = value;
            }
        }

        /// <summary>
        /// List of visiting addresses.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        [ListWithPropertyValueRequired("Type", "Visiting")]
        public override IList<V2VmOpenApiAddressWithType> Addresses
        {
            get
            {
                return base.Addresses;
            }

            set
            {
                base.Addresses = value;
            }
        }

        /// <summary>
        /// Publishing status. Possible values are: Draft, Published, Deleted or Modified.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public override string PublishingStatus
        {
            get
            {
                return base.PublishingStatus;
            }

            set
            {
                base.PublishingStatus = value;
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
        /// Set to true to delete all existing service hours for the service channel. The ServiceHours collection should be empty when this property is set to true.
        /// </summary>
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

        /// <summary>
        /// Set to true to delete all existing web pages for the service channel. The WebPages collection should be empty when this property is set to true.
        /// </summary>
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
    }
}
