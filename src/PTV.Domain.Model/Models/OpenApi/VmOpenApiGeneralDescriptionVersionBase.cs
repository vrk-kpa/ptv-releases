/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;
using System;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Domain.Model.Models.OpenApi.V10;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of general description - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiGeneralDescriptionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiGeneralDescriptionVersionBase" />
    public class VmOpenApiGeneralDescriptionVersionBase : VmOpenApiGeneralDescriptionBase, IVmOpenApiGeneralDescriptionVersionBase
    {
        /// <summary>
        /// List of service classes.
        /// </summary>
        [JsonProperty(Order = 5)]
        public IList<V7VmOpenApiFintoItemWithDescription> ServiceClasses { get; set; } = new List<V7VmOpenApiFintoItemWithDescription>();

        /// <summary>
        /// List of ontology terms.
        /// </summary>
        [JsonProperty(Order = 6)]
        public IList<V4VmOpenApiOntologyTerm> OntologyTerms { get; set; } = new List<V4VmOpenApiOntologyTerm>();

        /// <summary>
        /// List of target groups.
        /// </summary>
        [JsonProperty(Order = 7)]
        public IList<V4VmOpenApiFintoItem> TargetGroups { get; set; } = new List<V4VmOpenApiFintoItem>();

        /// <summary>
        /// List of life events.
        /// </summary>
        [JsonProperty(Order = 8)]
        public IList<V4VmOpenApiFintoItem> LifeEvents { get; set; } = new List<V4VmOpenApiFintoItem>();

        /// <summary>
        /// List of industrial classes.
        /// </summary>
        [JsonProperty(Order = 9)]
        public virtual IList<V4VmOpenApiFintoItem> IndustrialClasses { get; set; } = new List<V4VmOpenApiFintoItem>();

        /// <summary>
        /// Date when item was modified/created (UTC).
        /// </summary>
        [JsonProperty(Order = 50)]
        public virtual DateTime Modified { get; set; }

        /// <summary>
        /// List of linked service channels including relationship data.
        /// </summary>
        [JsonProperty(Order = 51)]
        public virtual IList<V6VmOpenApiServiceServiceChannel> ServiceChannels { get; set; } = new List<V6VmOpenApiServiceServiceChannel>();

        #region methods
        /// <summary>
        /// Open api version number.
        /// </summary>
        /// <returns>Open api version number</returns>
        public virtual int VersionNumber()
        {
            return 0;
        }

        /// <summary>
        /// Converts model to previous version.
        /// </summary>
        /// <returns></returns>
        public virtual IVmOpenApiGeneralDescriptionVersionBase PreviousVersion()
        {
            var vm = GetVersionBaseModel<V10VmOpenApiGeneralDescription>();
            return vm;
        }

        /// <summary>
        /// Get general description base model.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        protected TModel GetVersionBaseModel<TModel>() where TModel : IVmOpenApiGeneralDescriptionVersionBase, new()
        {
            var vm = base.GetBaseModel<TModel>();
            vm.ServiceClasses = this.ServiceClasses;
            vm.OntologyTerms = this.OntologyTerms;
            vm.TargetGroups = this.TargetGroups;
            vm.LifeEvents = this.LifeEvents;
            vm.IndustrialClasses = this.IndustrialClasses;
            vm.Modified = this.Modified;
            vm.ServiceChannels = this.ServiceChannels;
            return vm;
        }

        #endregion
    }
}
