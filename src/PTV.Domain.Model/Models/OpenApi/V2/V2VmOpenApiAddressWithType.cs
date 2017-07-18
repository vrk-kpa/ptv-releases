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
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V5;

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
        /// Converts to version 5.
        /// </summary>
        /// <returns></returns>
        public new V5VmOpenApiAddressWithTypeIn ConvertToVersion5()
        {
            var vm = ConvertToVersionIn<V5VmOpenApiAddressWithTypeIn>();
            vm.Type = this.Type;
            vm.PostOfficeBox = this.PostOfficeBox.ConvertToLanguageList();
            vm.Municipality = this.Municipality;
            return vm;
        }
    }
}
