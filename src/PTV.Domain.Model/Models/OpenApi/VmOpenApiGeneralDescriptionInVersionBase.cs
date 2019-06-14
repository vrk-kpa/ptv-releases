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

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Enums;
using PTV.Framework.Extensions;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of general description for IN api base (PUT)
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiGeneralDescriptionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiGeneralDescriptionInVersionBase" />
    public class VmOpenApiGeneralDescriptionInVersionBase : VmOpenApiGeneralDescriptionBase, IVmOpenApiGeneralDescriptionInVersionBase, IVmEntityBase
    {
        /// <summary>
        /// Entity Guid identifier.
        /// </summary>
        [JsonIgnore]
        public override Guid? Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// List of service class urls. Sample url: http://urn.fi/URN:NBN:fi:au:ptvl:v1065
        /// </summary>
        [ListRegularExpression(@"^http://urn.fi/URN:NBN:fi:au:ptvl:v[0-9]+$")]
        [JsonProperty(Order = 5)]
        public virtual IList<string> ServiceClasses { get; set; }

        /// <summary>
        /// List of ontology term urls. Sample url: http://www.yso.fi/onto/koko/p2435
        /// </summary>
        [ListRegularExpression(@"^http://www.yso.fi/onto/[a-z]*/p[0-9]{1,5}$")]
        [JsonProperty(Order = 6)]
        public virtual IList<string> OntologyTerms { get; set; }

        /// <summary>
        /// List of target group urls. Sample url: http://urn.fi/URN:NBN:fi:au:ptvl:v2004
        /// </summary>
        [ListRegularExpression(@"^http://urn.fi/URN:NBN:fi:au:ptvl:v[0-9]+$")]
        [JsonProperty(Order = 7)]
        public virtual IList<string> TargetGroups { get; set; }

        /// <summary>
        /// List of life event urls. Sample url: http://urn.fi/URN:NBN:fi:au:ptvl:v3017
        /// </summary>
        [ListRegularExpression(@"^http://urn.fi/URN:NBN:fi:au:ptvl:v[0-9]+$")]
        [JsonProperty(Order = 8)]
        public IList<string> LifeEvents { get; set; }

        /// <summary>
        /// List of industrial class codes (see http://tilastokeskus.fi/meta/luokitukset/toimiala/001-2008/tekstitiedosto_en.txt).
        /// </summary>
        [ListRegularExpression(@"^http://www.stat.fi/meta/luokitukset/toimiala/001-2008/[0-9]{5}$")]
        [JsonProperty(Order = 9)]
        public IList<string> IndustrialClasses { get; set; } = new List<string>();

        /// <summary>
        /// Set to true to delete all existing life events (the LifeEvents collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonProperty(Order = 45)]
        public virtual bool DeleteAllLifeEvents { get; set; }

        /// <summary>
        /// Set to true to delete all existing industrial classes (the IndustrialClasses collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonProperty(Order = 46)]
        public virtual bool DeleteAllIndustrialClasses { get; set; }

        /// <summary>
        /// Set to true to delete all existing laws within legislation (the legislation collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonProperty(Order = 47)]
        public virtual bool DeleteAllLaws { get; set; }

        /// <summary>
        /// Set to true to delete service charge type (ServiceChargeType property for this object should be empty when this option is used).
        /// </summary>
        [JsonProperty(Order = 48)]
        public virtual bool DeleteServiceChargeType { get; set; }

        /// <summary>
        /// Current version publishing status.
        /// </summary>
        [JsonIgnore]
        public string CurrentPublishingStatus { get; set; }

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
        /// <exception cref="System.NotImplementedException">VmOpenApiGeneralDescriptionInVersionBase does not have next version available!</exception>
        public virtual IVmOpenApiGeneralDescriptionInVersionBase VersionBase()
        {
            throw new NotImplementedException("VmOpenApiGeneralDescriptionInVersionBase does not have next version available!");
        }

        /// <summary>
        /// Gets the in base version model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>in base version model</returns>
        protected TModel GetInVersionBaseModel<TModel>(int version) where TModel : IVmOpenApiGeneralDescriptionInVersionBase, new()
        {
            var vm = base.GetBaseModel<TModel>();
           
            vm.ServiceClasses = this.ServiceClasses;
            vm.OntologyTerms = this.OntologyTerms;
            vm.TargetGroups = this.TargetGroups;
            vm.LifeEvents = this.LifeEvents;
            vm.IndustrialClasses = this.IndustrialClasses;
            vm.DeleteAllLifeEvents = this.DeleteAllLifeEvents;
            vm.DeleteAllIndustrialClasses = this.DeleteAllIndustrialClasses;
            vm.DeleteAllLaws = this.DeleteAllLaws;
            vm.DeleteServiceChargeType = this.DeleteServiceChargeType;
            vm.CurrentPublishingStatus = this.CurrentPublishingStatus;

            // Convert new open api values into enums used within db (PTV-2184)
            if (version > 7)
            {
                vm.Type = string.IsNullOrEmpty(vm.Type) ? vm.Type : vm.Type.GetEnumValueByOpenApiEnumValue<ServiceTypeEnum>();
                vm.GeneralDescriptionType = string.IsNullOrEmpty(vm.GeneralDescriptionType) ? vm.GeneralDescriptionType : vm.GeneralDescriptionType.GetEnumValueByOpenApiEnumValue<GeneralDescriptionTypeEnum>();
                vm.Names.ForEach(n => n.Type = n.Type.GetEnumValueByOpenApiEnumValue<NameTypeEnum>());
                vm.Descriptions?.ForEach(d => d.Type = d.Type.GetEnumValueByOpenApiEnumValue<DescriptionTypeEnum>());
                vm.ServiceChargeType = string.IsNullOrEmpty(vm.ServiceChargeType) ? vm.ServiceChargeType : vm.ServiceChargeType.GetEnumValueByOpenApiEnumValue<ServiceChargeTypeEnum>();                
            }

            return vm;
        }
        #endregion
    }
}
