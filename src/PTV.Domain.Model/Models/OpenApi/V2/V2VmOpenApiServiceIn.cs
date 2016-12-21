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
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    public class V2VmOpenApiServiceIn : V2VmOpenApiServiceInBase, IV2VmOpenApiServiceInBase
    {

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
        /// Service type. Possible values are: Service, Notice, Registration or Permission.
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
        public override IReadOnlyList<VmOpenApiLocalizedListItem> ServiceNames
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
        public override IReadOnlyList<VmOpenApiLocalizedListItem> ServiceDescriptions
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
        public override IReadOnlyList<string> Languages
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
