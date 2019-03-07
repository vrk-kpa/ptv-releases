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
using PTV.Domain.Model.Models.OpenApi.V10;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of Electronic channel - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceChannel" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiElectronicChannelVersionBase" />
    public class VmOpenApiElectronicChannelVersionBase : VmOpenApiServiceChannel, IVmOpenApiElectronicChannelVersionBase
    {
        /// <summary>
        /// How many signatures are required.
        /// </summary>
        [JsonProperty(Order = 10)]
        public int? SignatureQuantity { get; set; }
        /// <summary>
        /// Is signature required.
        /// </summary>
        [JsonProperty(Order = 11)]
        public bool RequiresSignature { get; set; }
        /// <summary>
        /// Does the electronic channel require authentication.
        /// </summary>
        [JsonProperty(Order = 12)]
        public bool RequiresAuthentication { get; set; }
        /// <summary>
        /// List of attachments.
        /// </summary>
        [JsonProperty(Order = 14)]
        public IReadOnlyList<VmOpenApiAttachmentWithType> Attachments { get; set; }
        /// <summary>
        /// The accessibility classification.
        /// </summary>
        [JsonProperty(Order = 26)]
        public virtual IList<VmOpenApiAccessibilityClassification> AccessibilityClassification { get; set; }
       
        #region Methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 0;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceChannel PreviousVersion()
        {
            return GetVersionBaseModel<V10VmOpenApiElectronicChannel>();
        }

        /// <summary>
        /// Gets the version base model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>base model</returns>
        protected TModel GetVersionBaseModel<TModel>() where TModel : IVmOpenApiElectronicChannelVersionBase, new()
        {
            var vm = base.GetServiceChannelModel<TModel>();
            vm.SignatureQuantity = this.SignatureQuantity;
            vm.RequiresSignature = this.RequiresSignature;
            vm.RequiresAuthentication = this.RequiresAuthentication;
            vm.Attachments = this.Attachments;
            vm.AccessibilityClassification = this.AccessibilityClassification;
            vm.OntologyTerms = this.OntologyTerms;
            return vm;
        }
        #endregion
    }
}
