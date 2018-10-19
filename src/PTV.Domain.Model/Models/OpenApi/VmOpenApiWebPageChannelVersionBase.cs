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
using PTV.Domain.Model.Models.OpenApi.V9;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of web page channel - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceChannel" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiWebPageChannelVersionBase" />
    public class VmOpenApiWebPageChannelVersionBase : VmOpenApiServiceChannel, IVmOpenApiWebPageChannelVersionBase
    {
        /// <summary>
        /// The accessibility classification level.
        /// </summary>
        [JsonProperty(Order = 26)]
        public virtual string AccessibilityClassificationLevel { get; set; }
        /// <summary>
        /// The web content accessibility level.
        /// </summary>
        [JsonProperty(Order = 27)]
        public virtual string WCAGLevel { get; set; }
        /// <summary>
        /// List of accessibility statement web pages. One per language.
        /// </summary>
        [JsonProperty(Order = 28)]
        public virtual IList<V9VmOpenApiWebPage> AccessibilityStatementWebPage { get; set; } = new List<V9VmOpenApiWebPage>();

        #region Methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>
        /// version number
        /// </returns>
        public override int VersionNumber()
        {
            return 0;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>
        /// model of previous version
        /// </returns>
        public override IVmOpenApiServiceChannel PreviousVersion()
        {
            return GetVersionBaseModel<V9VmOpenApiWebPageChannel>();
        }

        /// <summary>
        /// Gets the base version model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>base version model</returns>
        protected TModel GetVersionBaseModel<TModel>() where TModel : IVmOpenApiWebPageChannelVersionBase, new()
        {
            var vm = base.GetServiceChannelModel<TModel>();
            vm.AccessibilityClassificationLevel = this.AccessibilityClassificationLevel;
            vm.WCAGLevel = this.WCAGLevel;
            vm.AccessibilityStatementWebPage = this.AccessibilityStatementWebPage;
            return vm;
        }
        #endregion
    }
}
