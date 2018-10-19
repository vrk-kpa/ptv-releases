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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V8;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of Address in - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiAddressOldInVersionBase" />
    public class VmOpenApiAddressOldInVersionBase : VmOpenApiBase, IVmOpenApiAddressOldInVersionBase
    {
        /// <summary>
        /// Post office box like PL 310
        /// </summary>
        [ListPropertyMaxLength(100, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public virtual IList<VmOpenApiLanguageItem> PostOfficeBox { get; set; }

        /// <summary>
        /// Postal code, for example 00100.
        /// </summary>
        [Required]
        [RegularExpression(@"\d{5}?")]
        public string PostalCode { get; set; }

        /// <summary>
        /// List of localized street addresses.
        /// </summary>
        [LocalizedListLanguageDuplicityForbidden]
        [ListPropertyMaxLength(100, "Value")]
        public virtual IList<VmOpenApiLanguageItem> StreetAddress { get; set; }

        /// <summary>
        /// Street number for street address.
        /// </summary>
        [MaxLength(30)]
        public virtual string StreetNumber { get; set; }

        /// <summary>
        /// Municipality code (e.g. 091).
        /// </summary>
        [RegularExpression(@"^[0-9]{1,3}$", ErrorMessage = CoreMessages.OpenApi.MustBeNumeric)]
        public string Municipality { get; set; }

        /// <summary>
        /// Country code (ISO 3166-1 alpha-2), for example FI.
        /// </summary>
        [RegularExpression(@"^[A-Z]{2}$")]
        public string Country { get; set; }

        /// <summary>
        /// Localized list of additional information about the address.
        /// </summary>
        [ListValueNotEmpty("Value")]
        [ListPropertyMaxLength(150, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public IList<VmOpenApiLanguageItem> AdditionalInformations { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// Location latitude coordinate.
        /// </summary>
        [RegularExpression(@"^\d+\.?\d*$")]
        public virtual string Latitude { get; set; }

        /// <summary>
        /// Location longitude coordinate.
        /// </summary>
        [RegularExpression(@"^\d+\.?\d*$")]
        public virtual string Longitude { get; set; }

        /// <summary>
        /// Gets or sets the owner reference identifier.
        /// </summary>
        /// <value>
        /// The owner reference identifier.
        /// </value>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the order number
        /// </summary>
        [JsonIgnore]
        public int? OrderNumber { get; set; }

        /// <summary>
        /// Converts to version.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>converted model</returns>
        protected TModel ConvertToVersionIn<TModel>() where TModel : IVmOpenApiAddressInVersionBase, new()
        {
            var vm =  new TModel()
            {
                Id = this.Id,
            };

            if (this.StreetAddress?.Count > 0)
            {
                vm.SubType = AddressTypeEnum.Street.ToString();
                vm.StreetAddress = new VmOpenApiAddressStreetWithCoordinatesIn()
                {
                    Street = this.StreetAddress,
                    StreetNumber = this.StreetNumber,
                    PostalCode = this.PostalCode,
                    Municipality = this.Municipality,
                    Latitude = this.Latitude,
                    Longitude = this.Longitude,
                    AdditionalInformation = this.AdditionalInformations
                };
            }
            else
            {
                vm.SubType = AddressTypeEnum.PostOfficeBox.ToString();
                vm.PostOfficeBoxAddress = new VmOpenApiAddressPostOfficeBoxIn()
                {
                    PostOfficeBox = PostOfficeBox,
                    PostalCode = this.PostalCode,
                    Municipality = this.Municipality,
                    AdditionalInformation = this.AdditionalInformations,
                };
            }

            return vm;
        }
        /// <summary>
        /// Converts to version.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>converted model</returns>
        protected TModel ConvertToDeliveryAddressVersionIn<TModel>() where TModel : IV8VmOpenApiAddressDeliveryIn, new()
        {
            var vm = new TModel
            {
                Id = this.Id,
            };

            if (this.StreetAddress?.Count > 0)
            {
                vm.SubType = AddressTypeEnum.Street.ToString();
                vm.StreetAddress = new VmOpenApiAddressStreetIn()
                {
                    Street = this.StreetAddress,
                    StreetNumber = this.StreetNumber,
                    PostalCode = this.PostalCode,
                    Municipality = this.Municipality,
                    AdditionalInformation = this.AdditionalInformations
                };
            }
            else
            {
                vm.SubType = AddressTypeEnum.PostOfficeBox.ToString();
                vm.PostOfficeBoxAddress = new VmOpenApiAddressPostOfficeBoxIn()
                {
                    PostOfficeBox = PostOfficeBox,
                    PostalCode = this.PostalCode,
                    Municipality = this.Municipality,
                    AdditionalInformation = this.AdditionalInformations,
                };
            }
            
            return vm;
        }
    }
}
