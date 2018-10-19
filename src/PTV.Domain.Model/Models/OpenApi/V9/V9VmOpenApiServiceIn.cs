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
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi.V8;

namespace PTV.Domain.Model.Models.OpenApi.V9
{
    /// <summary>
    /// OPEN API V9 - View Model of service for IN api
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.V9.V9VmOpenApiServiceInBase" />
    public class V9VmOpenApiServiceIn : V9VmOpenApiServiceInBase
    {
        /// <summary>
        /// Service type. Possible values are: Service, PermitOrObligation or ProfessionalQualification.
        /// NOTE! If service type has been defined within attached statutory service general description, the type for service is ignored.
        /// </summary>
        [RequiredIf("GeneralDescriptionId", null)]
        public override string Type { get => base.Type; set => base.Type = value; }

        /// <summary>
        /// Service funding type. Possible values are: PubliclyFunded or MarketFunded.
        /// </summary>
        [Required]
        public override string FundingType { get => base.FundingType; set => base.FundingType = value; }

        /// <summary>
        /// List of service names.
        /// </summary>
        [ListRequiredIf("GeneralDescriptionId", null)]
        [ListWithPropertyValueRequired("Type", "Name")]
        public override IList<VmOpenApiLocalizedListItem> ServiceNames { get => base.ServiceNames; set => base.ServiceNames = value; }

        /// <summary>
        /// Area type (Nationwide, NationwideExceptAlandIslands, LimitedType). 
        /// </summary>
        [Required]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// List of service descriptions.
        /// </summary>
        [ListWithPropertyValueRequired("Type", "Summary")]
        [ListRequired]
        public override IList<VmOpenApiLocalizedListItem> ServiceDescriptions { get => base.ServiceDescriptions; set => base.ServiceDescriptions = value; }

        /// <summary>
        /// List of service language codes.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        public override IList<string> Languages { get => base.Languages; set => base.Languages = value; }

        /// <summary>
        /// List of service class urls (see http://finto.fi/ptvl/fi/).
        /// NOTE! If service class has been defined within attached statutory service general description, the service class is not added for service.
        /// </summary>
        [ListRequiredIf("GeneralDescriptionId", null)] // PTV-1374
        public override IList<string> ServiceClasses { get => base.ServiceClasses; set => base.ServiceClasses = value; }

        /// <summary>
        /// List of ontology term urls (see http://finto.fi/koko/fi/).
        /// NOTE! If ontology term has been defined within attached statutory service general description, the ontology term is not added for service.
        /// </summary>
        [ListRequiredIf("GeneralDescriptionId", null)] // PTV-1374
        public override IList<string> OntologyTerms { get => base.OntologyTerms; set => base.OntologyTerms = value; }

        /// <summary>
        /// List of target group urls (see http://finto.fi/ptvl/fi/page/?uri=http://urn.fi/URN:NBN:fi:au:ptvl:KR).
        /// NOTE! If target group has been defined within attached statutory service general description, the target group is not added for service.
        /// </summary>
        [ListRequiredIf("GeneralDescriptionId", null)] // PTV-1374
        public override IList<string> TargetGroups { get => base.TargetGroups; set => base.TargetGroups = value; }

        /// <summary>
        /// List of service producers
        /// </summary>
        [ListRequired]
        public override IList<V9VmOpenApiServiceProducerIn> ServiceProducers { get => base.ServiceProducers; set => base.ServiceProducers = value; }

        /// <summary>
        /// Publishing status. Possible values are: Draft or Published.
        /// </summary>
        [AllowedValues(propertyName: "PublishingStatus", allowedValues: new[] { "Draft", "Published" })]
        public override string PublishingStatus { get => base.PublishingStatus; set => base.PublishingStatus = value; }

        /// <summary>
        /// List of related service channels (GUID).
        /// </summary>
        [JsonProperty(Order = 41)]
        [ListWithGuid]
        public virtual IList<string> ServiceChannels { get; set; } // Only for POST (PTV-2317)

        /// <summary>
        /// Main organization id.
        /// </summary>
        [Required]
        public override string MainResponsibleOrganization { get => base.MainResponsibleOrganization; set => base.MainResponsibleOrganization = value; }

        /// <summary>
        /// Set to true to delete all existing life events (the LifeEvents collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllLifeEvents { get => base.DeleteAllLifeEvents; set => base.DeleteAllLifeEvents = value; }

        /// <summary>
        /// Set to true to delete all existing industrial classes (the IndustrialClasses collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllIndustrialClasses { get => base.DeleteAllIndustrialClasses; set => base.DeleteAllIndustrialClasses = value; }

        /// <summary>
        /// Set to true to delete all existing keywords (the Keywords collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllKeywords { get => base.DeleteAllKeywords; set => base.DeleteAllKeywords = value; }

        /// <summary>
        /// Set to true to delete all existing municipalities (the Municipalities collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllMunicipalities { get => base.DeleteAllMunicipalities; set => base.DeleteAllMunicipalities = value; }

        /// <summary>
        /// Set to true to delete all existing laws within legislation (the legislation collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllLaws { get => base.DeleteAllLaws; set => base.DeleteAllLaws = value; }

        /// <summary>
        /// Set to true to delete service charge type (ServiceChargeType property for this object should be empty when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteServiceChargeType { get => base.DeleteServiceChargeType; set => base.DeleteServiceChargeType = value; }

        /// <summary>
        /// Set to true to delete statutory service general description (GeneralDescriptionId property for this object should be empty when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteGeneralDescriptionId { get => base.DeleteGeneralDescriptionId; set => base.DeleteGeneralDescriptionId = value; }

        #region methods

        /// <summary>
        /// gets the base version
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceInVersionBase VersionBase()
        {
            var vm = base.VersionBase();
            // Set services
            if (ServiceChannels?.Count > 0)
            {
                ServiceChannels.ForEach(s => vm.ServiceServiceChannels.Add(new V9VmOpenApiServiceServiceChannelAstiInBase { ChannelGuid = s.ParseToGuid().Value }));
            }
            return vm;
        }
        #endregion
    }
}
