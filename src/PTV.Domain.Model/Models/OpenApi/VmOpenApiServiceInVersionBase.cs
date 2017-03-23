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
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Framework;
using System;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service for IN api - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceInVersionBase" />
    public class VmOpenApiServiceInVersionBase : VmOpenApiServiceBase, IVmOpenApiServiceInVersionBase
    {
        /// <summary>
        /// External system identifier for the entity.
        /// </summary>
        [JsonProperty(Order = 1)]
        [RegularExpression(@"^[A-Za-z0-9-]*$")]
        public string SourceId { get; set; }

        /// <summary>
        /// Valid PTV statutory service general description identifier that this service will be linked to. List of valid identifiers can be retrieved from the endpoint /api/GeneralDescription
        /// </summary>
        [JsonProperty(Order = 2)]
        [ValidGuid]
        public string StatutoryServiceGeneralDescriptionId { get; set; }

        /// <summary>
        /// List of service class urls. Sample url: http://urn.fi/URN:NBN:fi:au:ptvl:P6.5.
        /// NOTE! If service class has been defined within attached statutory service general description, the service class is not added for service.
        /// </summary>
        [JsonProperty(Order = 10)]
        [ListRegularExpression(@"^http://urn.fi/URN:NBN:fi:au:ptvl:P[0-9]{1,2}(\.[0-9]{1,2})?$")]
        public virtual IList<string> ServiceClasses { get; set; }

        /// <summary>
        /// List of ontology term urls. Sample url: http://www.yso.fi/onto/koko/p2435.
        /// NOTE! If ontology term has been defined within attached statutory service general description, the ontology term is not added for service.
        /// </summary>
        [JsonProperty(Order = 11)]
        [ListRegularExpression(@"^http://www.yso.fi/onto/[a-z]*/p[0-9]{1,5}$")]
        public virtual IList<string> OntologyTerms { get; set; }

        /// <summary>
        /// List of target group urls. Sample url: http://urn.fi/URN:NBN:fi:au:ptvl:KR1.3.
        /// NOTE! If target group has been defined within attached statutory service general description, the target group is not added for service.
        /// </summary>
        [JsonProperty(Order = 12)]
        [ListRegularExpression(@"^http://urn.fi/URN:NBN:fi:au:ptvl:KR\d(\.\d)?$")]
        public virtual IList<string> TargetGroups { get; set; }

        /// <summary>
        /// List of life event urls. Sample url: http://urn.fi/URN:NBN:fi:au:ptvl:KE7
        /// NOTE! If life event has been defined within attached statutory service general description, the life event is not added for service.
        /// </summary>
        [JsonProperty(Order = 13)]
        [ListRegularExpression(@"^http://urn.fi/URN:NBN:fi:au:ptvl:KE[0-9]{1,2}(\.[0-9]{1})?$")]
        public IList<string> LifeEvents { get; set; } = new List<string>();

        /// <summary>
        /// List of industrial class codes (see http://tilastokeskus.fi/meta/luokitukset/toimiala/001-2008/tekstitiedosto_en.txt).
        /// NOTE! If industrial class has been defined within attached statutory service general description, the industrial class is not added for service.
        /// </summary>
        [JsonProperty(Order = 14)]
        [ListRegularExpression(@"^http://www.stat.fi/meta/luokitukset/toimiala/001-2008/[0-9]{5}$")]
        public IList<string> IndustrialClasses { get; set; } = new List<string>();

        /// <summary>
        /// List of municipality codes that the service is available for. Used in conjunction with service coverage type Local.
        /// </summary>
        [JsonProperty(Order = 22)]
        [ListRequiredIf("ServiceCoverageType", "Local")]
        [ListRegularExpression(@"^[0-9]{1,3}$", ErrorMessage = CoreMessages.OpenApi.MustBeNumeric)]
        public IList<string> Municipalities { get; set; } = new List<string>();

        /// <summary>
        /// List of organizations responsible or producing the service.
        /// </summary>
        [JsonProperty(Order = 29)]
        [ListWithPropertyValueRequired("RoleType", "Responsible")]
        [ListWithPropertyValueRequired("RoleType", "Producer")]
        public virtual IList<V4VmOpenApiServiceOrganization> ServiceOrganizations { get; set; }

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
        /// Set to true to delete all existing keywords (the Keywords collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonProperty(Order = 48)]
        public virtual bool DeleteAllKeywords { get; set; }

        /// <summary>
        /// Set to true to delete all existing municipalities (the Municipalities collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonProperty(Order = 49)]
        public virtual bool DeleteAllMunicipalities { get; set; }

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
        /// <exception cref="System.NotImplementedException">VmOpenApiOrganizationInVersionBase does not have next version available!</exception>
        public virtual IVmOpenApiServiceInVersionBase VersionBase()
        {
            throw new NotImplementedException("VmOpenApiOrganizationInVersionBase does not have next version available!");
        }

        /// <summary>
        /// Gets the in base version model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>in base version model</returns>
        protected TModel GetInVersionBaseModel<TModel>() where TModel : IVmOpenApiServiceInVersionBase, new()
        {
            var vm = base.GetBaseModel<TModel>();
            vm.SourceId = this.SourceId;
            vm.StatutoryServiceGeneralDescriptionId = this.StatutoryServiceGeneralDescriptionId;
            vm.ServiceClasses = this.ServiceClasses;
            vm.OntologyTerms = this.OntologyTerms;
            vm.TargetGroups = this.TargetGroups;
            vm.LifeEvents = this.LifeEvents;
            vm.IndustrialClasses = this.IndustrialClasses;
            vm.Municipalities = this.Municipalities;
            vm.ServiceOrganizations = this.ServiceOrganizations;
            vm.DeleteAllLifeEvents = this.DeleteAllLifeEvents;
            vm.DeleteAllIndustrialClasses = this.DeleteAllIndustrialClasses;
            vm.DeleteAllLaws = this.DeleteAllLaws;
            vm.DeleteAllKeywords = this.DeleteAllKeywords;
            vm.DeleteAllMunicipalities = this.DeleteAllMunicipalities;
            vm.CurrentPublishingStatus = this.CurrentPublishingStatus;
            return vm;
        }
        #endregion
    }
}
