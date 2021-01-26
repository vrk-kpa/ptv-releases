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

using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V5;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of Address - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiAddressVersionBase" />
    public class VmOpenApiAddressVersionBase : VmOpenApiBase, IVmOpenApiAddressVersionBase
    {
        /// <summary>
        /// Address type, Visiting or Postal.
        /// </summary>
        [JsonProperty(Order = 1)]
        public virtual string Type { get; set; }

        /// <summary>
        /// Address sub type, Street, PostOfficeBox or Foreign.
        /// </summary>
        [JsonProperty(Order = 2)]
        public virtual string SubType { get; set; }

        /// <summary>
        /// Post office box address
        /// </summary>
        [JsonProperty(Order = 3)]
        public virtual VmOpenApiAddressPostOfficeBox PostOfficeBoxAddress { get; set; }

        /// <summary>
        /// Street address.
        /// </summary>
        [JsonProperty(Order = 4)]
        public virtual VmOpenApiAddressStreetWithCoordinates StreetAddress { get; set; }

        /// <summary>
        /// Country code (ISO 3166-1 alpha-2), for example FI.
        /// </summary>
        [JsonProperty(Order = 10)]
        public string Country { get; set; }

        /// <summary>
        /// Converts to version.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>converted model</returns>
        protected TModel ConvertToVersion<TModel>() where TModel : IVmOpenApiAddressVersionBase, new()
        {
            return new TModel
            {
                Id = this.Id,
                Type = this.Type,
                SubType = this.SubType,
                PostOfficeBoxAddress = this.PostOfficeBoxAddress,
                StreetAddress = this.StreetAddress,
                Country = this.Country
            };
        }
    }
}
