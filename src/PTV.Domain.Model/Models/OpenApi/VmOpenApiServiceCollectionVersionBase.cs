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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service collection - version base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceCollectionVersionBase" />
    public class VmOpenApiServiceCollectionVersionBase : VmOpenApiServiceCollectionBase, IVmOpenApiServiceCollectionVersionBase
    {
        /// <summary>
        /// List of service collection services.
        /// </summary>
        [JsonProperty(Order = 20)]
        //[ListRequired(AllowEmptyStrings = false)]
        public virtual IList<VmOpenApiServiceCollectionService> Services { get; set; }

        /// <summary>
        /// Date when item was modified/created (UTC).
        /// </summary>
        [JsonProperty(Order = 50)]
        public virtual DateTime Modified { get; set; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>
        /// version number
        /// </returns>
        public virtual int VersionNumber()
        {
            return 0;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>
        /// model of previous version
        /// </returns>
        public virtual IVmOpenApiServiceCollectionVersionBase PreviousVersion()
        {
            var vm = GetVersionBaseModel<V7VmOpenApiServiceCollection>();
            return vm;
        }

        /// <summary>
        /// Gets the base version model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>base version model</returns>
        protected TModel GetVersionBaseModel<TModel>() where TModel : IVmOpenApiServiceCollectionVersionBase, new()
        {
            var vm = base.GetBaseModel<TModel>();
            vm.Services = this.Services;
            vm.Modified = this.Modified;
            return vm;
        }
        #endregion
    }
}
