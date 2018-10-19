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
using PTV.Framework.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.Interfaces.OpenApi;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of address with type
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiAddressInVersionBase" />
    public class VmOpenApiAddressInVersionBase : VmOpenApiBase, IVmOpenApiAddressInVersionBase
    {
        /// <summary>
        /// Address type, Visiting or Postal.
        /// </summary>
        [Required]
        [JsonProperty(Order = 1)]
        public virtual string Type { get; set; }

        /// <summary>
        /// Address sub type, Street, PostOfficeBox or Foreign.
        /// </summary>        
        [Required]
        [JsonProperty(Order = 2)]
        public virtual string SubType { get; set; }

        /// <summary>
        /// Post office box address
        /// </summary>
        [RequiredIf("SubType", AddressConsts.POSTOFFICEBOX)]
        [JsonProperty(Order = 3)]
        public virtual VmOpenApiAddressPostOfficeBoxIn PostOfficeBoxAddress { get; set; }

        /// <summary>
        /// Street address.
        /// </summary>
        [RequiredIf("SubType", AddressConsts.STREET)]
        [JsonProperty(Order = 4)]
        public virtual VmOpenApiAddressStreetWithCoordinatesIn StreetAddress { get; set; }

        /// <summary>
        /// Localized list of foreign address information.
        /// </summary>
        [ListValueNotEmpty("Value")]
        [ListPropertyMaxLength(500, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        [JsonProperty(Order = 5)]
        public virtual IList<VmOpenApiLanguageItem> ForeignAddress { get; set; }

        /// <summary>
        /// Country code (ISO 3166-1 alpha-2), for example FI.
        /// </summary>
        [RegularExpression(@"^[A-Z]{2}$")]
        [JsonProperty(Order = 10)]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the order number
        /// </summary>
        public virtual int OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the owner reference identifier.
        /// </summary>
        /// <value>
        /// The owner reference identifier.
        /// </value>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
        
        /// <summary>
        /// Gets or sets the address unique id
        /// </summary>
        [JsonIgnore]
        public Guid? UniqueId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public TModel GetBaseMode<TModel>() where TModel : IVmOpenApiAddressInVersionBase, new()
        {
            return new TModel()
            {
                Id = this.Id,
                UniqueId =  this.UniqueId,
                Type = this.Type,
                SubType = this.SubType,
                PostOfficeBoxAddress = this.PostOfficeBoxAddress,
                StreetAddress = this.StreetAddress,
                ForeignAddress = this.ForeignAddress,
                Country = this.Country,
                OrderNumber = OrderNumber,
            };
        }
    }
}
