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
using PTV.Domain.Model.Models.Interfaces;
using System;
using PTV.Domain.Model.Models.OpenApi.V2;
using System.Collections.Generic;
using PTV.Domain.Model.Enums;
using PTV.Framework.Attributes;

namespace PTV.Domain.Model.Models.OpenApi
{
    public class VmOpenApiOrganizationInBase : V2VmOpenApiOrganizationInBase, IVmOpenApiOrganizationInBase
    {
        /// <summary>
        /// This property is not used in the API anymore.
        /// </summary>
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        [JsonProperty(Order = 4)]
        public bool StreetAddressAsPostalAddress { get; set; }

        /// <summary>
        /// List of email addresses.
        /// </summary>
        [JsonProperty(Order = 20)]
        public new IList<VmOpenApiOrganizationEmailIn> EmailAddresses { get; set; } = new List<VmOpenApiOrganizationEmailIn>();

        /// <summary>
        /// List of phone numbers.
        /// </summary>
        [JsonProperty(Order = 21)]
        [ListWithEnum(typeof(PhoneNumberTypeEnum), "Type")]
        public new IList<VmOpenApiOrganizationPhoneIn> PhoneNumbers { get; set; }

        /// <summary>
        /// List of addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public new IList<VmOpenApiAddressWithType> Addresses { get; set; } = new List<VmOpenApiAddressWithType>();

    }
}
