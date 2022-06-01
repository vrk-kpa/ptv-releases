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

using Newtonsoft.Json;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework.Attributes.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V11;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of service for IN api - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiServiceInVersionBase" />
    public class VmOpenApiServiceInVersionBase : VmOpenApiServiceBase, IVmOpenApiServiceInVersionBase
    {
        private IList<string> _availableLanguages;
        private IList<string> _requiredPropertiesAvailableLanguages;

        /// <summary>
        /// PTV service identifier.
        /// </summary>
        [JsonIgnore]
        public override Guid? Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Valid PTV statutory service general description identifier that this service will be linked to. List of valid identifiers can be retrieved from the endpoint /api/GeneralDescription
        /// </summary>
        [JsonProperty(Order = 2)]
        [ValidGuid]
        public virtual string GeneralDescriptionId { get; set; }

        /// <summary>
        /// List of areas. List can contain different types of areas.
        /// </summary>
        [ListRequiredIf("AreaType", "AreaType")]
        [JsonProperty(Order = 8)]
        public virtual IList<VmOpenApiAreaIn> Areas { get; set; } = new List<VmOpenApiAreaIn>();

        /// <summary>
        /// List of service class urls. Sample url: http://urn.fi/URN:NBN:fi:au:ptvl:v1065.
        /// NOTE! If service class has been defined within attached statutory service general description, the service class is not added for service.
        /// </summary>
        [JsonProperty(Order = 15)]
        [UrlListRegularExpression(ValidationConsts.ServiceClass)]
        public virtual IList<string> ServiceClasses { get; set; }

        /// <summary>
        /// List of ontology term urls. Sample url: http://www.yso.fi/onto/koko/p2435.
        /// NOTE! If ontology term has been defined within attached statutory service general description, the ontology term is not added for service.
        /// </summary>
        [JsonProperty(Order = 16)]
        [ListRegularExpression(ValidationConsts.OntologyTerm)]
        public virtual IList<string> OntologyTerms { get; set; }

        /// <summary>
        /// List of target group urls. Sample url: http://urn.fi/URN:NBN:fi:au:ptvl:v2004.
        /// NOTE! If target group has been defined within attached statutory service general description, the target group is not added for service.
        /// </summary>
        [JsonProperty(Order = 17)]
        [UrlListRegularExpression(ValidationConsts.TargetGroup)]
        public virtual IList<string> TargetGroups { get; set; }

        /// <summary>
        /// List of life event urls. Sample url: http://urn.fi/URN:NBN:fi:au:ptvl:v3017
        /// NOTE! If life event has been defined within attached statutory service general description, the life event is not added for service.
        /// </summary>
        [JsonProperty(Order = 18)]
        [UrlListRegularExpression(ValidationConsts.LifeEvent)]
        public IList<string> LifeEvents { get; set; } = new List<string>();

        /// <summary>
        /// List of industrial class codes (see http://tilastokeskus.fi/meta/luokitukset/toimiala/001-2008/tekstitiedosto_en.txt).
        /// NOTE! If industrial class has been defined within attached statutory service general description, the industrial class is not added for service.
        /// </summary>
        [JsonProperty(Order = 19)]
        [UrlListRegularExpression(ValidationConsts.IndustrialClass)]
        public IList<string> IndustrialClasses { get; set; } = new List<string>();

        /// <summary>
        /// List of other responsible organizations for the service.
        /// </summary>
        [JsonProperty(Order = 29)]
        public virtual IList<Guid> OtherResponsibleOrganizations { get; set; }

        /// <summary>
        /// List of service producers
        /// </summary>
        [JsonProperty(Order = 31)]
        public virtual IList<V9VmOpenApiServiceProducerIn> ServiceProducers { get; set; } = new List<V9VmOpenApiServiceProducerIn>();

        /// <summary>
        /// Publishing status. Possible values are: Draft, Published, Deleted or Modified.
        /// </summary>
        [JsonProperty(Order = 40)]
        [ValidEnum(typeof(PublishingStatus))]
        [Required]
        public virtual string PublishingStatus { get; set; }

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
        /// Set to true to delete service charge type (ServiceChargeType property for this object should be empty when this option is used).
        /// </summary>
        [JsonProperty(Order = 50)]
        public virtual bool DeleteServiceChargeType { get; set; }

        /// <summary>
        /// Set to true to delete statutory service general description (GeneralDescriptionId property for this object should be empty when this option is used).
        /// </summary>
        [JsonProperty(Order = 51)]
        public virtual bool DeleteGeneralDescriptionId { get; set; }

        /// <summary>
        /// Main responsible organization Id
        /// </summary>
        [JsonProperty(Order = 52)]
        [ValidGuid]
        public virtual string MainResponsibleOrganization { get; set; }

        /// <summary>
        /// Set to true to delete all existing service vouchers (the ServiceVouchers collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonProperty(Order = 53)]
        public virtual bool DeleteAllServiceVouchers { get; set; }

        /// <summary>
        /// Date when item should be published.
        /// </summary>
        [JsonProperty(Order = 60)]
        [DateInFuture]
        public virtual DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Date when item should be archived.
        /// </summary>
        [JsonProperty(Order = 61)]
        [DateInFuture]
        public virtual DateTime? ValidTo { get; set; }

        /// <summary>
        /// Current version publishing status.
        /// </summary>
        [JsonIgnore]
        public string CurrentPublishingStatus { get; set; }

        /// <summary>
        /// Internal property for adding service channel connections for a service.
        /// This property is also used when attaching general description propsed channels into service (PTV-2315).
        /// </summary>
        [JsonIgnore]
        public IList<V11VmOpenApiServiceServiceChannelAstiInBase> ServiceServiceChannels { get; set; } = new List<V11VmOpenApiServiceServiceChannelAstiInBase>();

        /// <summary>
        /// User name.
        /// </summary>
        [JsonIgnore]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets available languages
        /// </summary>
        [JsonIgnore]
        public IList<string> AvailableLanguages
        {
            get
            {
                // Return available languages or calculate them.
                // SFIPTV-1913: All localized lists need to be taken into account: ServiceNames, ServiceDescriptions,
                // Legislation, Keywords, Requirements, ServiceProducers (AdditionalInformation) and ServiceVouchers
                if (this._availableLanguages == null)
                {
                    var list = new HashSet<string>();
                    list.GetAvailableLanguages(this.ServiceNames);
                    list.GetAvailableLanguages(this.ServiceDescriptions);
                    list.GetAvailableLanguages(this.Legislation);
                    list.GetAvailableLanguages(this.Keywords);
                    list.GetAvailableLanguages(this.Requirements);
                    list.GetAvailableLanguages(this.ServiceProducers);
                    list.GetAvailableLanguages(this.ServiceVouchers);

                    this._availableLanguages = list.ToList();
                }

                return this._availableLanguages;
            }
            set
            {
                this._availableLanguages = value;
            }
        }

        /// <summary>
        /// Internal property to check the languages within required lists: ServiceNames and ServiceDescriptions
        /// </summary>
        [JsonIgnore]
        public IList<string> RequiredPropertiesAvailableLanguages
        {
            get
            {
                if (_requiredPropertiesAvailableLanguages == null)
                {
                    var list = new HashSet<string>();
                    list.GetAvailableLanguages(this.ServiceNames);
                    list.GetAvailableLanguages(this.ServiceDescriptions);

                    _requiredPropertiesAvailableLanguages = list.ToList();
                }

                return _requiredPropertiesAvailableLanguages;
            }
            set
            {
                _requiredPropertiesAvailableLanguages = value;
            }
        }

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
        /// <exception cref="System.NotImplementedException">VmOpenApiServiceInVersionBase does not have next version available!</exception>
        public virtual IVmOpenApiServiceInVersionBase VersionBase()
        {
            throw new NotImplementedException("VmOpenApiServiceInVersionBase does not have next version available!");
        }

        /// <summary>
        /// Gets the in base version model.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>in base version model</returns>
        protected TModel GetInVersionBaseModel<TModel>(int version) where TModel : IVmOpenApiServiceInVersionBase, new()
        {
            var vm = base.GetBaseModel<TModel>();
            vm.SourceId = this.SourceId;
            vm.GeneralDescriptionId = this.GeneralDescriptionId;
            vm.Areas = this.Areas;
            vm.ServiceClasses = this.ServiceClasses;
            vm.OntologyTerms = this.OntologyTerms;
            vm.TargetGroups = this.TargetGroups;
            vm.LifeEvents = this.LifeEvents;
            vm.IndustrialClasses = this.IndustrialClasses;
            vm.OtherResponsibleOrganizations = this.OtherResponsibleOrganizations;
            vm.DeleteAllLifeEvents = this.DeleteAllLifeEvents;
            vm.DeleteAllIndustrialClasses = this.DeleteAllIndustrialClasses;
            vm.DeleteAllLaws = this.DeleteAllLaws;
            vm.DeleteAllKeywords = this.DeleteAllKeywords;
            vm.DeleteAllMunicipalities = this.DeleteAllMunicipalities;
            vm.DeleteServiceChargeType = this.DeleteServiceChargeType;
            vm.DeleteGeneralDescriptionId = this.DeleteGeneralDescriptionId;
            vm.CurrentPublishingStatus = this.CurrentPublishingStatus;
            vm.ServiceVouchers = this.ServiceVouchers;
            vm.ServiceProducers = this.ServiceProducers;
            vm.ServiceServiceChannels = this.ServiceServiceChannels;
            vm.MainResponsibleOrganization = this.MainResponsibleOrganization;
            vm.DeleteAllServiceVouchers = this.DeleteAllServiceVouchers;
            vm.PublishingStatus = this.PublishingStatus;
            vm.ValidFrom = this.ValidFrom;
            vm.ValidTo = this.ValidTo;
            vm.UserName = this.UserName;
            vm.AvailableLanguages = this.AvailableLanguages;

            return vm;
        }
        #endregion
    }
}
