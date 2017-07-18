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

using PTV.Framework;
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V5;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    /// <summary>
    /// OPEN API V2 - View Model of Address
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiAddressVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V2.IV2VmOpenApiAddress" />
    public class V2VmOpenApiAddress : VmOpenApiAddressVersionBase, IV2VmOpenApiAddress
    {
        /// <summary>
        /// Post office box like PL 310
        /// </summary>
        [RequiredIf("StreetAddress", null)]
        public new string PostOfficeBox { get; set; }

        /// <summary>
        /// List of localized street addresses.
        /// </summary>
        [RequiredIf("PostOfficeBox", null, typeof(string))]
        [LocalizedListLanguageDuplicityForbidden]
        public override IList<VmOpenApiLanguageItem> StreetAddress
        {
            get
            {
                return base.StreetAddress;
            }

            set
            {
                base.StreetAddress = value;
            }
        }

        /// <summary>
        /// Post office, for example Helsinki.
        /// </summary>
        public new string PostOffice { get; set; }

        /// <summary>
        /// Municipality code, for example 491.
        /// </summary>
        [RegularExpression(@"^[0-9]{1,3}$", ErrorMessage = CoreMessages.OpenApi.MustBeNumeric)]
        public new string Municipality { get; set; }

        /// <summary>
        /// Converts to version4.
        /// </summary>
        /// <returns></returns>
        public virtual V5VmOpenApiAddressIn ConvertToVersion5()
        {
            var vm = ConvertToVersionIn<V5VmOpenApiAddressIn>();
            vm.PostOfficeBox = PostOfficeBox.ConvertToLanguageList();
            vm.Municipality = Municipality;
            return vm;
        }
    }
}
