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
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    public class V2VmOpenApiOrganizationInBase : VmOpenApiOrganizationBase, IV2VmOpenApiOrganizationInBase
    {

        /// <summary>
        /// Organization external system identifier.
        /// </summary>
        [JsonProperty(Order = 1)]
        [RegularExpression(@"^[A-Za-z0-9-]*$")]
        public string SourceId { get; set; }

        /// <summary>
        /// Localized list of organization descriptions.
        /// </summary>
        [JsonProperty(Order = 15)]
        [ListPropertyMaxLength(500, "Value")]
        public IReadOnlyList<VmOpenApiLanguageItem> OrganizationDescriptions { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of email addresses.
        /// </summary>
        [JsonProperty(Order = 20)]
        public IList<VmOpenApiEmail> EmailAddresses { get; set; } = new List<VmOpenApiEmail>();

        /// <summary>
        /// List of phone numbers.
        /// </summary>
        [JsonProperty(Order = 21)]
        //[ListWithEnum(typeof(PhoneNumberTypeEnum), "Type")]
        public IList<VmOpenApiPhone> PhoneNumbers { get; set; } = new List<VmOpenApiPhone>();

        /// <summary>
        /// List of web pages.
        /// </summary>
        [JsonProperty(Order = 22)]
        [ListWithEnum(typeof(WebPageTypeEnum), "Type")]
        public IList<VmOpenApiWebPageIn> WebPages { get; set; } = new List<VmOpenApiWebPageIn>();

        /// <summary>
        /// List of addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public IList<V2VmOpenApiAddressWithType> Addresses { get; set; } = new List<V2VmOpenApiAddressWithType>();

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

        [JsonIgnore]
        public override Guid? Id
        {
            get
            {
                return base.Id;
            }

            set
            {
                base.Id = value;
            }
        }
    }
}
