﻿/**
* The MIT License
* Copyright(c) 2016 Population Register Centre(VRK)
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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V8
{
    /// <summary>
    /// OPEN API V8 - View Model of web page channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiWebPageChannelVersionBase" />
    public class V8VmOpenApiWebPageChannel : VmOpenApiWebPageChannelVersionBase
    {

        /// <summary>
        /// Area type (WholeCountry, WholeCountryExceptAlandIslands, AreaType). 
        /// </summary>
        [JsonIgnore]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// List of service channel areas.
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiArea> Areas { get => base.Areas; set => base.Areas = value; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 8;
        }
        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceChannel PreviousVersion()
        {
            var vm = base.GetVersionBaseModel<V7VmOpenApiWebPageChannel>();
            // Change property values into ones used in older versions (PTV-2184)
            SetV7Values(vm);
            this.WebPages?.ForEach(w => vm.Urls.Add(new VmOpenApiLanguageItem { Language = w.Language, Value = w.Url }));
            // Remove the items from web page
            vm.WebPages = new List<VmOpenApiWebPageWithOrderNumber>();
            return vm;
        }

        #endregion
    }
}
