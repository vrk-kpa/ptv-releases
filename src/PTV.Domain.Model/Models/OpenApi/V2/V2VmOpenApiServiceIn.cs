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
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    /// <summary>
    /// OPEN API V2 - View Model of service for IN api
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.V2.V2VmOpenApiServiceInBase" />
    public class V2VmOpenApiServiceIn : V2VmOpenApiServiceInBase
    {
        /// <summary>
        /// PTV service identifier.
        /// </summary>
        [JsonIgnore]
        public override Guid? Id
        {
            get
            {
                return base.Id;
            }

            set
            {
                base.Id = value;
            }
        }

        /// <summary>
        /// Service type. Possible values are: Service, Notice, Registration, Permission or PermissionAndObligation.
        /// NOTE! Current PTV database does not anymore support for types Notice, Registration or Permission - they are automatically mapped into PermissionAndObligation type.
        /// POST and PUT methods accepts old types but GET method only can return Service or PermissionAndObligation types.
        /// NOTE 2! If service type has been defined within attached statutory service general description, the type for service is ignored.
        /// </summary>
        [Required]
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
        [Required]
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
        [Required]
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
        /// List of ontology term urls (see http://finto.fi/yso/fi/).
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
        [ListRequired]
        public override IReadOnlyList<VmOpenApiServiceOrganization> ServiceOrganizations
        {
            get
            {
                return base.ServiceOrganizations;
            }

            set
            {
                base.ServiceOrganizations = value;
            }
        }

        /// <summary>
        /// Publishing status. Possible values are: Draft, Published, Deleted, Modified or OldPublished.
        /// In POST method you can only create services as Draft or Published so statuses Deleted, Modified or OldPublished will be automatically converted to Draft.
        /// </summary>
        [Required]
        public override string PublishingStatus
        {
            get
            {
                return base.PublishingStatus;
            }

            set
            {
                base.PublishingStatus = value;
            }
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
    }
}
