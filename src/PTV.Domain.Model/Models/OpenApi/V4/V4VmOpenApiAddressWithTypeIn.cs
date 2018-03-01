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
using PTV.Framework.Attributes;
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V7;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V7;

namespace PTV.Domain.Model.Models.OpenApi.V4
{
    /// <summary>
    /// OPEN API V4 - View Model of address with type
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.V4.V4VmOpenApiAddressIn" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V4.IV4VmOpenApiAddressWithTypeIn" />
    public class V4VmOpenApiAddressWithTypeIn : V4VmOpenApiAddressIn, IV4VmOpenApiAddressWithTypeIn
    {
        /// <summary>
        /// Address type, Visiting or Postal.
        /// </summary>
        [ValidEnum(typeof(AddressCharacterEnum))]
        [Required]
        public string Type { get; set; }

        /// <summary>
        /// Converts to latest version.
        /// </summary>
        /// <returns></returns>
        public TModel LatestVersion<TModel>() where TModel : IV7VmOpenApiAddressInVersionBase, new()
        {
            var vm = base.ConvertToVersionIn<TModel>();
            vm.Type = this.Type;
            vm.Country = this.Country;
            if (!string.IsNullOrEmpty(this.PostOfficeBox) && vm.PostOfficeBoxAddress != null)
            {
                vm.PostOfficeBoxAddress.PostOfficeBox = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Value = PostOfficeBox, Language = LanguageCode.fi.ToString() } };
            }

            return vm;
        }

        /// <summary>
        /// Converts to moving address.
        /// </summary>
        /// <returns></returns>
        public V7VmOpenApiAddressWithForeignIn ConvertToVersion7()
        {
            var vm = base.ConvertToVersionIn<V7VmOpenApiAddressWithForeignIn>();
            vm.Type = this.Type;
            vm.Country = this.Country;
            if (!string.IsNullOrEmpty(this.PostOfficeBox) && vm.PostOfficeBoxAddress != null)
            {
                vm.PostOfficeBoxAddress.PostOfficeBox = new List<VmOpenApiLanguageItem> { new VmOpenApiLanguageItem { Value = PostOfficeBox, Language = LanguageCode.fi.ToString() } };
            }

            return vm;
        }
    }
}
