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
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API base models - View Model of service and service channel connection IN (POST/PUT).
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceServiceChannelBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceServiceChannelInVersionBase" />
    public class VmOpenApiServiceServiceChannelInVersionBase : VmOpenApiServiceServiceChannelBase, IVmOpenApiServiceServiceChannelInVersionBase
    {
        /// <summary>
        /// PTV service identifier.
        /// </summary>
        [ValidGuid]
        [JsonProperty(Order = 1)]
        public virtual string ServiceId { get; set; }

        /// <summary>
        /// PTV service channel identifier.
        /// </summary>
        [ValidGuid]
        [JsonProperty(Order = 2)]
        public virtual string ServiceChannelId { get; set; }

        /// <summary>
        /// The extra types related to service and service channel connection.
        /// </summary>
        [JsonProperty(Order = 5)]
        public virtual IList<VmOpenApiExtraType> ExtraTypes { get; set; } = new List<VmOpenApiExtraType>();

        /// <summary>
        /// List of connection related service hours.
        /// </summary>
        [JsonProperty(Order = 7)]
        public virtual VmOpenApiContactDetailsInBase ContactDetails { get; set; }

        /// <summary>
        /// Indicates if value for property ServiceChargeType should be deleted.
        /// </summary>
        [JsonProperty(Order = 10)]
        public virtual bool DeleteServiceChargeType { get; set; }

        /// <summary>
        /// Indicates if all descriptions should be deleted.
        /// </summary>
        [JsonProperty(Order = 11)]
        public virtual bool DeleteAllDescriptions { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all service hours should be delted.
        /// </summary>
        /// <value>
        /// <c>true</c> if all service hours should be deleted; otherwise, <c>false</c>.
        /// </value>
        [JsonProperty(Order = 12)]
        public virtual bool DeleteAllServiceHours { get; set; }

        /// <summary>
        /// Gets or sets the service unique identifier.
        /// </summary>
        /// <value>
        /// The service unique identifier.
        /// </value>
        [JsonIgnore]
        public Guid ServiceGuid { get; set; }

        /// <summary>
        /// Gets or sets the channel unique identifier.
        /// </summary>
        /// <value>
        /// The channel unique identifier.
        /// </value>
        [JsonIgnore]
        public Guid ChannelGuid { get; set; }

        /// <summary>
        /// Internal property to map digital authorizations from GD proposed channels (PTV-2315)
        /// </summary>
        [JsonIgnore]
        public IList<Guid> DigitalAuthorizations { get; set; } = new List<Guid>();

        #region methods
        
        /// <summary>
        /// Gets the in base version model.
        /// </summary>
        /// <returns>in base version model</returns>
        public virtual V9VmOpenApiServiceServiceChannelAstiInBase GetLatestInVersionModel()
        {
            return new V9VmOpenApiServiceServiceChannelAstiInBase
            {
                ServiceId = this.ServiceId,
                ServiceChannelId = this.ServiceChannelId,
                ServiceChargeType = this.ServiceChargeType,
                Description = this.Description,
                ExtraTypes = this.ExtraTypes,
                ServiceHours = this.ServiceHours,
                ContactDetails = this.ContactDetails?.ConvertToInBaseModel(),
                DeleteServiceChargeType = this.DeleteServiceChargeType,
                DeleteAllDescriptions = this.DeleteAllDescriptions,
                DeleteAllServiceHours = this.DeleteAllServiceHours,
                IsASTIConnection = this.IsASTIConnection,
                OwnerReferenceId = this.OwnerReferenceId,
                ServiceGuid = this.ServiceGuid,
                ChannelGuid = this.ChannelGuid,
            };
        }

        /// <summary>
        /// Gets the base model.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        protected TModel GetBase<TModel>() where TModel : IVmOpenApiServiceServiceChannelInVersionBase, new()
        {
            var vm = base.GetBaseModel<TModel>();
            vm.ServiceId = this.ServiceId;
            vm.ServiceChannelId = this.ServiceChannelId;
            vm.ServiceGuid = this.ServiceGuid;
            vm.ChannelGuid = this.ChannelGuid;
            vm.ExtraTypes = this.ExtraTypes;
            vm.ContactDetails = this.ContactDetails;
            vm.DeleteServiceChargeType = this.DeleteServiceChargeType;
            vm.DeleteAllDescriptions = this.DeleteAllDescriptions;
            vm.DeleteAllServiceHours = this.DeleteAllServiceHours;
            return vm;
        }
        #endregion
    }
}
