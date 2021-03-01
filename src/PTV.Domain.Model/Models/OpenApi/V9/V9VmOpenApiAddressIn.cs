/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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

using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PTV.Domain.Model.Models.OpenApi.V9
{
    /// <summary>
    /// OPEN API V9 - View Model of address
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiAddressInVersionBase" />
    public class V9VmOpenApiAddressIn : VmOpenApiAddressInVersionBase
    {
        /// <summary>
        /// Address type, Visiting or Postal.
        /// </summary>
        [Required]
        [AllowedValues("Type", new[] { AddressConsts.VISITING, AddressConsts.POSTAL })]
        public override string Type { get => base.Type; set => base.Type = value; }
        /// <summary>
        /// Address sub type, Street, PostOfficeBox, Foreign or Other.
        /// </summary>
        [Required]
        [AllowedValues("SubType", new[] { AddressConsts.STREET, AddressConsts.POSTOFFICEBOX, AddressConsts.FOREIGN, AddressConsts.OTHER })]
        public override string SubType { get => base.SubType; set => base.SubType = value; }

        /// <summary>
        /// Address coordinates with additional information.
        /// </summary>
        [JsonProperty(Order = 5)]
        [RequiredIf("SubType", AddressConsts.OTHER)]
        public VmOpenApiAddressOtherIn OtherAddress { get; set; }

        /// <summary>
        /// Localized list of foreign address information.
        /// </summary>
        [ListRequiredIf("SubType", AddressConsts.FOREIGN)]
        public override IList<VmOpenApiLanguageItem> ForeignAddress { get => base.ForeignAddress; set => base.ForeignAddress = value; }

        /// <summary>
        /// Gets or sets the order number
        /// </summary>
        [JsonIgnore]
        public override int OrderNumber { get => base.OrderNumber; set => base.OrderNumber = value; }
    }
}
