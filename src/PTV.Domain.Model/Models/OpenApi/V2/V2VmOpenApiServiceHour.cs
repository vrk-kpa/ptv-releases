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
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.Extensions;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    /// <summary>
    /// OPEN API V2 - View Model of service hours
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceHourBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V2.IV2VmOpenApiServiceHour" />
    public class V2VmOpenApiServiceHour : VmOpenApiServiceHourBase, IV2VmOpenApiServiceHour
    {
        /// <summary>
        /// Localized list of additional information.
        /// </summary>
        public new IList<VmOpenApiLanguageItem> AdditionalInformation { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// Set to true to present that this entry is valid for now.
        /// </summary>
        [JsonIgnore]
        public override bool ValidForNow
        {
            get
            {
                return base.ValidForNow;
            }

            set
            {
                base.ValidForNow = value;
            }
        }

        /// <summary>
        /// Converts to version4.
        /// </summary>
        /// <returns>converted model to the version 4</returns>
        public V4VmOpenApiServiceHour ConvertToVersion4()
        {
            var serviceHour = ConvertBase<V4VmOpenApiServiceHour>();
            serviceHour.AdditionalInformation = this.AdditionalInformation.SetListValueLength(100);
            return serviceHour;
        }
    }
}
