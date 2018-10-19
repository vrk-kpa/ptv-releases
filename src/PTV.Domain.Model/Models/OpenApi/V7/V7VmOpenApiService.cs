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

using System.Collections;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using Newtonsoft.Json;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V4;
using System;
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Models.OpenApi.Extensions;

namespace PTV.Domain.Model.Models.OpenApi.V7
{
    /// <summary>
    /// OPEN API V7 - View Model of service
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceVersionBase" />
    public class V7VmOpenApiService : VmOpenApiServiceVersionBase
    {
        /// <summary>
        /// List of linked service channels including relationship data.
        /// </summary>
        [JsonProperty(Order = 29)]
        public new IList<V7VmOpenApiServiceServiceChannel> ServiceChannels { get; set; } = new List<V7VmOpenApiServiceServiceChannel>();

        /// <summary>
        /// PTV identifier for linked general description.
        /// </summary>
        [Display(Name = "StatutoryServiceGeneralDescriptionId")]
        [JsonProperty(Order = 2, PropertyName = "statutoryServiceGeneralDescriptionId")]
        public override Guid? GeneralDescriptionId { get => base.GeneralDescriptionId; set => base.GeneralDescriptionId = value; }

        /// <summary>
        /// List of service vouchers.
        /// </summary>
        [JsonProperty(Order = 32)]
        public virtual new IList<VmOpenApiServiceVoucher> ServiceVouchers { get; set; } = new List<VmOpenApiServiceVoucher>();

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
            return 7;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiServiceVersionBase PreviousVersion()
        {
            var vm = GetVersionBaseModel<V6VmOpenApiService>();
            ServiceChannels.ForEach(c => vm.ServiceChannels.Add(c.ConvertToVersion6()));

            // handle organization services - 'OtherResponsible' role available only for version 7 (and upper)
            vm.Organizations.ForEach(o =>
            {
                if (o.RoleType == CommonConsts.OTHER_RESPONSIBLE)
                {
                    o.RoleType = CommonConsts.RESPONSIBLE;
                }
            });

            this.ServiceClasses.ForEach(sc => vm.ServiceClasses.Add(new V4VmOpenApiFintoItem() {
                Code = sc.Code,
                Id = sc.Id,
                Name = sc.Name,
                Description = sc.Description,
                OntologyType = sc.OntologyType,
                Override = sc.Override,
                ParentId = sc.ParentId,
                ParentUri = sc.ParentUri,
                Uri = sc.Uri,
            }));
            vm.ServiceVouchers = this.ServiceVouchers;

            return vm;
        }
        #endregion
    }
}
