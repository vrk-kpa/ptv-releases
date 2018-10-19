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

using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.OpenApi.V4;
using System;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V6
{
    /// <summary>
    /// OPEN API V6 - View Model of service
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceVersionBase" />
    public class V6VmOpenApiService : VmOpenApiServiceVersionBase
    {
        /// <summary>
        /// PTV identifier for linked general description.
        /// </summary>
        [Display(Name = "StatutoryServiceGeneralDescriptionId")]
        [JsonProperty(Order = 2, PropertyName = "statutoryServiceGeneralDescriptionId")]
        public override Guid? GeneralDescriptionId { get => base.GeneralDescriptionId; set => base.GeneralDescriptionId = value; }

        /// <summary>
        /// List of service classes related to the service.
        /// </summary>
        [JsonProperty(Order = 15)]
        public new IList<V4VmOpenApiFintoItem> ServiceClasses { get; set; } = new List<V4VmOpenApiFintoItem>();
       
        /// <summary>
        /// External system identifier for the entity.
        /// </summary>
        [JsonIgnore]
        public override string SourceId { get => base.SourceId; set => base.SourceId = value; }

        /// <summary>
        /// List of linked service channels including relationship data.
        /// </summary>
        [JsonProperty(Order = 29)]
        public new IList<V6VmOpenApiServiceServiceChannel> ServiceChannels { get; set; } = new List<V6VmOpenApiServiceServiceChannel>();

        /// <summary>
        /// Indicates if service vouchers are used in the service.
        /// </summary>
        [JsonIgnore]
        public override bool ServiceVouchersInUse { get => base.ServiceVouchersInUse; set => base.ServiceVouchersInUse = value; }

        /// <summary>
        /// List of service vouchers.
        /// </summary>
        [JsonProperty(Order = 32)]
        public virtual new IList<VmOpenApiServiceVoucher> ServiceVouchers { get; set; } = new List<VmOpenApiServiceVoucher>();

        /// <summary>
        /// List of service collections that the service has been linked to
        /// </summary>
        [JsonIgnore]
        public override IList<VmOpenApiServiceServiceCollection> ServiceCollections { get => base.ServiceCollections; set => base.ServiceCollections = value; }

        /// <summary>
        /// Service sub-type. It is used for SOTE and its taken from GeneralDescription type.
        /// </summary>
        [JsonIgnore]
        public override string SubType { get; set; }
        
        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 6;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceVersionBase PreviousVersion()
        {
            throw new NotSupportedException("No previous version for class V6VmOpenApiService!");
        }
        #endregion
    }
}
