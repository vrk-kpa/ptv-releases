﻿/**
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

using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Attributes;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V7
{
    /// <summary>
    /// OPEN API V7 - View Model of address
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiAddressInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V7.IV7VmOpenApiAddressWithMovingIn" />
    public class V7VmOpenApiAddressWithMovingIn : VmOpenApiAddressInVersionBase, IV7VmOpenApiAddressWithMovingIn
    {
        /// <summary>
        /// Address type, Location or Postal.
        /// </summary>
        [AllowedValues("Type", new[] { AddressConsts.LOCATION, AddressConsts.POSTAL })]
        public override string Type { get => base.Type; set => base.Type = value; }

        /// <summary>
        /// Address sub type, Single, Street, PostOfficeBox, Abroad or Multipoint.
        /// </summary>
        [AllowedValues("SubType", new[] { AddressConsts.SINGLE, AddressConsts.STREET, AddressConsts.POSTOFFICEBOX, AddressConsts.ABROAD, AddressConsts.MULTIPOINT })]
        public override string SubType { get => base.SubType; set => base.SubType = value; }

        /// <summary>
        /// Localized list of foreign address information.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiLanguageItem> ForeignAddress { get => base.ForeignAddress; set => base.ForeignAddress = value; }

        /// <summary>
        /// Street address.
        /// </summary>
        [RequiredIf("SubType", AddressConsts.SINGLE)]
        [JsonProperty(Order = 4)]
        public override VmOpenApiAddressStreetWithCoordinatesIn StreetAddress { get => base.StreetAddress; set => base.StreetAddress = value; }

        /// <summary>
        /// Localized list of foreign address information.
        /// </summary>
        [ListValueNotEmpty("Value")]
        [ListPropertyMaxLength(500, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        [ListRequiredIf("SubType", AddressConsts.ABROAD)]
        [JsonProperty(Order = 5)]
        public IList<VmOpenApiLanguageItem> LocationAbroad { get; set; }

        /// <summary>
        /// Moving address. Includes several street addresses.
        /// </summary>
        [JsonProperty(Order = 6)]
        [RequiredIf("SubType", AddressConsts.MULTIPOINT)]
        public IList<VmOpenApiAddressStreetWithCoordinatesIn> MultipointLocation { get; set; }

        /// <summary>
        /// Gets or sets the order number
        /// </summary>
        [JsonIgnore]
        public override int OrderNumber { get => base.OrderNumber; set => base.OrderNumber = value; }

        /// <summary>
        /// Converts into latest in base model.
        /// </summary>
        /// <returns></returns>
        public V9VmOpenApiAddressLocationIn ConvertToInBaseModel()
        {
            var model = base.GetBaseMode<V9VmOpenApiAddressLocationIn>();
            model.LocationAbroad = this.LocationAbroad;
            return model;
        }
    }
}
