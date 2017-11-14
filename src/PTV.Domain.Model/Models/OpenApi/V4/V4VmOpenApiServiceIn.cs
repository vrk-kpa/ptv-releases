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

namespace PTV.Domain.Model.Models.OpenApi.V4
{
    /// <summary>
    /// OPEN API V4 - View Model of service for IN api
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.V4.V4VmOpenApiServiceInBase" />
    public class V4VmOpenApiServiceIn : V4VmOpenApiServiceInBase
    {
        /// <summary>
        /// Service type. Possible values are: Service or PermissionAndObligation.
        /// NOTE! If service type has been defined within attached statutory service general description, the type for service is ignored.
        /// </summary>
        [RequiredIf("StatutoryServiceGeneralDescriptionId", null)]
        public override string Type
        {
            get
            {
                return base.Type;
            }

            set
            {
                base.Type = value;
            }
        }

        /// <summary>
        /// List of service names.
        /// </summary>
        [ListRequiredIf("StatutoryServiceGeneralDescriptionId", null)]
        [ListWithPropertyValueRequired("Type", "Name")]
        public override IList<VmOpenApiLocalizedListItem> ServiceNames
        {
            get
            {
                return base.ServiceNames;
            }

            set
            {
                base.ServiceNames = value;
            }
        }

        /// <summary>
        /// List of service descriptions.
        /// </summary>
        [ListRequired]
        [ListWithPropertyValueRequired("Type", "Description")]
        [ListWithPropertyValueRequired("Type", "ShortDescription")]
        public override IList<VmOpenApiLocalizedListItem> ServiceDescriptions
        {
            get
            {
                return base.ServiceDescriptions;
            }

            set
            {
                base.ServiceDescriptions = value;
            }
        }

        /// <summary>
        /// List of service language codes.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        public override IList<string> Languages
        {
            get
            {
                return base.Languages;
            }

            set
            {
                base.Languages = value;
            }
        }

        /// <summary>
        /// List of service class urls (see http://finto.fi/ptvl/fi/).
        /// NOTE! If service class has been defined within attached statutory service general description, the service class is not added for service.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        public override IList<string> ServiceClasses
        {
            get
            {
                return base.ServiceClasses;
            }

            set
            {
                base.ServiceClasses = value;
            }
        }

        /// <summary>
        /// List of ontology term urls (see http://finto.fi/koko/fi/).
        /// NOTE! If ontology term has been defined within attached statutory service general description, the ontology term is not added for service.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        public override IList<string> OntologyTerms
        {
            get
            {
                return base.OntologyTerms;
            }

            set
            {
                base.OntologyTerms = value;
            }
        }

        /// <summary>
        /// List of target group urls (see http://finto.fi/ptvl/fi/page/?uri=http://urn.fi/URN:NBN:fi:au:ptvl:KR).
        /// NOTE! If target group has been defined within attached statutory service general description, the target group is not added for service.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        public override IList<string> TargetGroups
        {
            get
            {
                return base.TargetGroups;
            }

            set
            {
                base.TargetGroups = value;
            }
        }

        /// <summary>
        /// Service coverage type. Possible values are: Local or Nationwide.
        /// </summary>
        [Required]
        public override string ServiceCoverageType
        {
            get
            {
                return base.ServiceCoverageType;
            }

            set
            {
                base.ServiceCoverageType = value;
            }
        }

        /// <summary>
        /// List of organizations responsible or producing the service.
        /// </summary>
        [JsonProperty(Order = 29)]
        [ListRequired]
        public override IList<V4VmOpenApiServiceOrganization> ServiceOrganizations
        {
            get => base.ServiceOrganizations;
            set => base.ServiceOrganizations = value;
        }

        /// <summary>
        /// Publishing status. Possible values are: Draft or Published.
        /// </summary>
        [AllowedValues(propertyName: "PublishingStatus", allowedValues: new[] { "Draft", "Published" })]
        [ValidEnum(null)] // PTV-1792: suppress base ValidEnum validation
        public override string PublishingStatus
        {
            get => base.PublishingStatus;
            set => base.PublishingStatus = value;
        }

        /// <summary>
        /// Set to true to delete all existing life events (the LifeEvents collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllLifeEvents
        {
            get
            {
                return base.DeleteAllLifeEvents;
            }

            set
            {
                base.DeleteAllLifeEvents = value;
            }
        }

        /// <summary>
        /// Set to true to delete all existing industrial classes (the IndustrialClasses collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllIndustrialClasses
        {
            get
            {
                return base.DeleteAllIndustrialClasses;
            }

            set
            {
                base.DeleteAllIndustrialClasses = value;
            }
        }

        /// <summary>
        /// Set to true to delete all existing keywords (the Keywords collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllKeywords
        {
            get
            {
                return base.DeleteAllKeywords;
            }

            set
            {
                base.DeleteAllKeywords = value;
            }
        }

        /// <summary>
        /// Set to true to delete all existing municipalities (the Municipalities collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllMunicipalities
        {
            get
            {
                return base.DeleteAllMunicipalities;
            }

            set
            {
                base.DeleteAllMunicipalities = value;
            }
        }

        /// <summary>
        /// Set to true to delete all existing laws within legislation (the legislation collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllLaws
        {
            get
            {
                return base.DeleteAllLaws;
            }

            set
            {
                base.DeleteAllLaws = value;
            }
        }
    }
}
