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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Enums;
using Newtonsoft.Json;
using System;
using PTV.Domain.Model.Models.OpenApi.V2;

namespace PTV.Domain.Model.Models.OpenApi
{
    public class VmOpenApiOrganizationPhone : VmOpenApiBase, IVmOpenApiOrganizationPhone
    {
        /// <summary>
        /// Phone number prefix (like country code +358).
        /// </summary>
        public string PrefixNumber { get; set; }

        /// <summary>
        /// Phone number.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Phone number type (Phone, Sms or Fax).
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Type { get; set; }

        /// <summary>
        /// Phone call charge type (Charged, Free or Other).
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [ValidEnum(typeof(ServiceChargeTypeEnum))]
        public string ChargeType { get; set; }

        /// <summary>
        /// Localized list of phone number descriptions. The type value of the localized item needs to be one the following values: ChargeDescription or AdditionalInformation.
        /// </summary>
        [ListWithEnum(typeof(PhoneDescriptionTypeEnum), "Type")]
        public IReadOnlyList<VmOpenApiLocalizedListItem> Descriptions { get; set; } = new List<VmOpenApiLocalizedListItem>();

        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }

    }
}
