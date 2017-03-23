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
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V1;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Framework.Attributes;

namespace PTV.Domain.Model.Models.OpenApi.V1
{
    /// <summary>
    ///
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.V1.VmOpenApiAddress" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V1.IVmOpenApiAddressWithType" />
    public class VmOpenApiAddressWithType : VmOpenApiAddress, IVmOpenApiAddressWithType
    {
        /// <summary>
        /// Address qualifier. This property is not used in the API anymore. Do not use.
        /// </summary>
        //[Obsolete("This property is not used in the API anymore. Do not use.", false)]
        //public string Qualifier { get; set; }

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
        /// Street number for street address.
        /// </summary>
        [JsonIgnore]
        public override string StreetNumber
        {
            get
            {
                return base.StreetNumber;
            }

            set
            {
                base.StreetNumber = value;
            }
        }

        /// <summary>
        /// Converts to version2.
        /// </summary>
        /// <returns></returns>
        public new V4VmOpenApiAddressWithTypeIn ConvertToVersion2()
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
