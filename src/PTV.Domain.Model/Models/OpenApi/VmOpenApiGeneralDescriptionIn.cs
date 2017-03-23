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
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of general description for IN api
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiGeneralDescriptionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiGeneralDescriptionIn" />
    public class VmOpenApiGeneralDescriptionIn : VmOpenApiGeneralDescriptionBase, IVmOpenApiGeneralDescriptionIn
    {
        /// <summary>
        /// Entity Guid identifier.
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
        /// List of name entities. Value of "type" has to be always "Name".
        ///  Sample single JSON object: {"language": "fi", "value": "Perhepäivähoito esiopetusikäisille", "type": "Name"}.
        /// </summary>
        [Required]
        [ListWithPropertyValueRequired("Type", "Name")]
        public override IReadOnlyList<VmOpenApiLocalizedListItem> Names
        {
            get
            {
                return base.Names;
            }

            set
            {
                base.Names = value;
            }
        }

        /// <summary>
        /// List of description entities. Requires two entities where ones type is "Description" and the other ones type is "ShortDescription".
        /// Sample single JSON object: {"language": "fi", "value": "Lyhyen kuvauksen kuvaus esimerkki teksti.", "type": "ShortDescription"}.
        /// Other optional description types are ServiceUserInstruction, ChargeTypeAdditionalInfo, DeadLineAdditionalInfo, ProcessingTimeAdditionalInfo, ValidityTimeAdditionalInfo.
        /// </summary>
        [Required]
        [ListWithPropertyValueRequired("Type", "Description")]
        [ListWithPropertyValueRequired("Type", "ShortDescription")]
        public override IReadOnlyList<VmOpenApiLocalizedListItem> Descriptions
        {
            get
            {
                return base.Descriptions;
            }

            set
            {
                base.Descriptions = value;
            }
        }

        /// <summary>
        /// List of language codes.
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
        /// List of service class urls. Sample url: http://urn.fi/URN:NBN:fi:au:ptvl:P6.5
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        [ListRegularExpression(@"^http://urn.fi/URN:NBN:fi:au:ptvl:P[0-9]{1,2}(\.[0-9]{1,2})?$")]
        [JsonProperty(Order = 5)]
        public virtual IList<string> ServiceClasses { get; set; }

        /// <summary>
        /// List of ontology term urls. Sample url: http://www.yso.fi/onto/koko/p2435
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        [ListRegularExpression(@"^http://www.yso.fi/onto/[a-z]*/p[0-9]{1,5}$")]
        [JsonProperty(Order = 6)]
        public virtual IList<string> OntologyTerms { get; set; }

        /// <summary>
        /// List of target group urls. Sample url: http://urn.fi/URN:NBN:fi:au:ptvl:KR1.3
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        [ListRegularExpression(@"^http://urn.fi/URN:NBN:fi:au:ptvl:KR\d(\.\d)?$")]
        [JsonProperty(Order = 7)]
        public virtual IList<string> TargetGroups { get; set; }

        /// <summary>
        /// List of life event urls. Sample url: http://urn.fi/URN:NBN:fi:au:ptvl:KE7
        /// </summary>
        [ListRegularExpression(@"^http://urn.fi/URN:NBN:fi:au:ptvl:KE[0-9]{1,2}(\.[0-9]{1})?$")]
        [JsonProperty(Order = 8)]
        public IList<string> LifeEvents { get; set; }

        /// <summary>
        /// List of industrial class codes (see http://tilastokeskus.fi/meta/luokitukset/toimiala/001-2008/tekstitiedosto_en.txt).
        /// </summary>
        [ListRegularExpression(@"^http://www.stat.fi/meta/luokitukset/toimiala/001-2008/[0-9]{5}$")]
        [JsonProperty(Order = 9)]
        public IList<string> IndustrialClasses { get; set; } = new List<string>();

        /// <summary>
        /// Publishing status. Possible values are: Draft, Published, Deleted, Modified or OldPublished.
        /// </summary>
        [Required]
        //[ValidEnum(typeof(PublishingStatus))]
        [AllowedValues(propertyName: "PublishingStatus", allowedValues: new[] { "Draft", "Published" })]
        [JsonProperty(Order = 10)]
        public string PublishingStatus { get; set; }
    }
}
