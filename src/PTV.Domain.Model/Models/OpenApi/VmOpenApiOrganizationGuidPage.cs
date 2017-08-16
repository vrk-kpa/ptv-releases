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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Framework;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of organization guid page
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiGuidPageVersionBase" />
    public class VmOpenApiOrganizationGuidPage : VmOpenApiGuidPageVersionBase, IVmOpenApiOrganizationGuidPage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmOpenApiOrganizationGuidPage"/> class.
        /// </summary>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        public VmOpenApiOrganizationGuidPage(int pageNumber, int pageSize) : base(pageNumber, pageSize) { }

        /// <summary>
        /// List of entity Guids.
        /// </summary>
        [JsonProperty(Order = 4)]
        public virtual IList<VmOpenApiOrganizationItem> ItemList { get; set; }

        #region methods

        /// <summary>
        /// Converts model into version 6.
        /// </summary>
        /// <returns></returns>
        public VmOpenApiGuidPageVersionBase ConvertToVersion6()
        {
            var vm = new V3VmOpenApiGuidPage(this.PageNumber, this.PageSize);
            vm.PageCount = this.PageCount;
            if (this.ItemList?.Count > 0)
            {
                vm.ItemList = new List<VmOpenApiItem>();
            }
            this.ItemList.ForEach(i => vm.ItemList.Add(i.ConvertToVersion6()));
            return vm;
        }

        #endregion
    }
}
