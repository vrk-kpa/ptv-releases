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
    /// OPEN API - View Model of service collection - in version base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceCollectionInVersionBase" />
    public class VmOpenApiServiceCollectionInVersionBase : VmOpenApiServiceCollectionBase, IVmOpenApiServiceCollectionInVersionBase
    {
        /// <summary>
        /// PTV service identifier.
        /// </summary>
        [JsonIgnore]
        public override Guid? Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Current version publishing status.
        /// </summary>
        [JsonIgnore]
        public string CurrentPublishingStatus { get; set; }

        /// <summary>
        /// Main responsible organization Id
        /// </summary>
        [JsonProperty(Order = 3)]
        [ValidGuid]
        public virtual new string MainResponsibleOrganization { get; set; }

        /// <summary>
        /// Internal property for adding service collection services for service collection.
        /// </summary>
        [JsonIgnore]
        public IList<Guid> ServiceCollectionServices { get; set; } = new List<Guid>();

        /// <summary>
        /// Set to true to delete all existing services (the services collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonProperty(Order = 49)]
        public virtual bool DeleteAllServices { get; set; }

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
        /// Gets the base version.
        /// </summary>
        /// <returns>
        /// view model of base
        /// </returns>
        /// <exception cref="System.NotImplementedException">VmOpenApiServiceCollectionInVersionBase does not have next version available!</exception>
        public virtual IVmOpenApiServiceCollectionInVersionBase VersionBase()
        {
            throw new NotImplementedException("VmOpenApiServiceCollectionInVersionBase does not have next version available!");
        }

        /// <summary>
        /// Gets the base version model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>base version model</returns>
        protected TModel GetInVersionBaseModel<TModel>() where TModel : IVmOpenApiServiceCollectionInVersionBase, new()
        {
            var vm = base.GetBaseModel<TModel>();
            vm.MainResponsibleOrganization = this.MainResponsibleOrganization;
            vm.SourceId = this.SourceId;
            vm.ServiceCollectionServices = this.ServiceCollectionServices;
            vm.DeleteAllServices = this.DeleteAllServices;
            return vm;
        }
        #endregion
    }
}
