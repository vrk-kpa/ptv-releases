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
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V1;
using PTV.Domain.Model.Models.OpenApi.Extensions;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    /// <summary>
    /// OPEN API V2 - View Model of address with type
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.V2.V2VmOpenApiAddress" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V2.IV2VmOpenApiAddressWithType" />
    public class V2VmOpenApiAddressWithType : V2VmOpenApiAddress, IV2VmOpenApiAddressWithType
    {
        /// <summary>
        /// Address type, Visiting or Postal.
        /// </summary>
        [ValidEnum(typeof(AddressTypeEnum))]
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the owner reference identifier.
        /// </summary>
        /// <value>
        /// The owner reference identifier.
        /// </value>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }

        /// <summary>
        /// Converts to version1.
        /// </summary>
        /// <returns></returns>
        public VmOpenApiAddressWithType ConvertToVersion1()
        {
            var vm = new VmOpenApiAddressWithType()
            {
                Id = this.Id,
                PostOfficeBox = this.PostOfficeBox,
                PostalCode = this.PostalCode,
                PostOffice = this.PostOffice,
                StreetAddress = this.StreetAddress,
                StreetNumber = this.StreetNumber,
                Municipality = this.Municipality,
                Country = this.Country,
                AdditionalInformations = this.AdditionalInformations,
                Type = this.Type,
                OwnerReferenceId = this.OwnerReferenceId
            };

            if (!string.IsNullOrEmpty(StreetNumber))
            {
                // Add the street number after each street address
                vm.StreetAddress.ForEach(a => a.Value = $"{a.Value} {StreetNumber}");
            }

            return vm;
        }

        /// <summary>
        /// Converts to version4.
        /// </summary>
        /// <returns></returns>
        public new V4VmOpenApiAddressWithTypeIn ConvertToVersion4()
        {
            return new V4VmOpenApiAddressWithTypeIn()
            {
                Id = this.Id,
                PostOfficeBox = this.PostOfficeBox,
                PostalCode = this.PostalCode,
                PostOffice = this.PostOffice,
                StreetAddress = this.StreetAddress.SetListValueLength(100),
                StreetNumber = this.StreetNumber.SetStringValueLength(30),
                Municipality = this.Municipality,
                Country = this.Country,
                AdditionalInformations = this.AdditionalInformations.SetListValueLength(150),
                Type = this.Type,
                OwnerReferenceId = this.OwnerReferenceId
            };
        }
    }
}
