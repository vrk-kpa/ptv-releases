﻿/**
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
using PTV.Domain.Model.Models.OpenApi.V7;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V8
{
    /// <summary>
    /// OPEN API V8 - View Model of address
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiAddressVersionBase" />
    public class V8VmOpenApiAddressWithMoving : VmOpenApiAddressVersionBase
    {
        /// <summary>
        /// Address type, Location or Postal.
        /// </summary>
        public override string Type { get => base.Type; set => base.Type = value; }

        /// <summary>
        /// Address sub type, Single, Street, PostOfficeBox, Abroad or Multipoint.
        /// </summary>
        public override string SubType { get => base.SubType; set => base.SubType = value; }

        /// <summary>
        /// Localized list of foreign address information.
        /// </summary>
        [JsonProperty(Order = 5)]
        public virtual IList<VmOpenApiLanguageItem> LocationAbroad { get; set; }
        /// <summary>
        /// Moving address. Includes several street addresses.
        /// </summary>
        [JsonProperty(Order = 6)]
        public IList<VmOpenApiAddressStreetWithOrder> MultipointLocation { get; set; }

        /// <summary>
        /// Entrances for an address. Includes accessibility sentences.
        /// </summary>
        [JsonProperty(Order = 7)]
        public IList<VmOpenApiEntrance> Entrances { get; set; } = new List<VmOpenApiEntrance>();

        /// <summary>
        /// Converts to version 7.
        /// </summary>
        /// <returns></returns>
        public V7VmOpenApiAddressWithMoving ConvertToVersion7()
        {
            var model = base.ConvertToVersion<V7VmOpenApiAddressWithMoving>();
            model.LocationAbroad = this.LocationAbroad;
            model.MultipointLocation = this.MultipointLocation;
            return model;
        }
    }
}
