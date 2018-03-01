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

namespace PTV.Domain.Model.Models.OpenApi.V6
{
    /// <summary>
    /// OPEN API - View Model of general description for IN api (POST)
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.V6.V6VmOpenApiGeneralDescriptionInBase" />
    public class V6VmOpenApiGeneralDescriptionIn : V6VmOpenApiGeneralDescriptionInBase
    {
        /// <summary>
        /// List of name entities. Value of "type" has to be always "Name".
        ///  Sample single JSON object: {"language": "fi", "value": "Perhepäivähoito esiopetusikäisille", "type": "Name"}.
        /// </summary>
        [Required]
        [ListWithPropertyValueRequired("Type", "Name")]
        public override IList<VmOpenApiLocalizedListItem> Names { get => base.Names; set => base.Names = value; }

        /// <summary>
        /// List of description entities. Requires two entities where ones type is "Description" or "BackgroundDescription" and the other ones type is "ShortDescription".
        /// Sample single JSON object: {"language": "fi", "value": "Lyhyen kuvauksen kuvaus esimerkki teksti.", "type": "ShortDescription"}.
        /// Other optional description types are ServiceUserInstruction, ChargeTypeAdditionalInfo, DeadLineAdditionalInfo, ProcessingTimeAdditionalInfo, ValidityTimeAdditionalInfo.
        /// </summary>
        [Required]
        [ListWithPropertyValueRequired("Type", "ShortDescription")] // Description and BackgroundDescription validation is done in GeneralDescriptionValidator.cs!
        public override IList<VmOpenApiLocalizedListItem> Descriptions { get => base.Descriptions; set => base.Descriptions = value; }

        /// <summary>
        /// List of language codes.
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        public override IList<string> Languages { get => base.Languages; set => base.Languages = value; }

        /// <summary>
        /// List of service class urls. Sample url: http://urn.fi/URN:NBN:fi:au:ptvl:v1065
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        public override IList<string> ServiceClasses { get => base.ServiceClasses; set => base.ServiceClasses = value; }

        /// <summary>
        /// List of ontology term urls. Sample url: http://www.yso.fi/onto/koko/p2435
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        public override IList<string> OntologyTerms { get => base.OntologyTerms; set => base.OntologyTerms = value; }

        /// <summary>
        /// List of target group urls. Sample url: http://urn.fi/URN:NBN:fi:au:ptvl:v2004
        /// </summary>
        [ListRequired(AllowEmptyStrings = false)]
        public override IList<string> TargetGroups { get => base.TargetGroups; set => base.TargetGroups = value; }

        /// <summary>
        /// Publishing status. Possible values are: Draft or Published.
        /// </summary>
        [AllowedValues(propertyName: "PublishingStatus", allowedValues: new[] { "Draft", "Published" })]
        public override string PublishingStatus { get => base.PublishingStatus; set => base.PublishingStatus = value; }

        /// <summary>
        /// Set to true to delete all existing industrial classes (the IndustrialClasses collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllIndustrialClasses { get => base.DeleteAllIndustrialClasses; set => base.DeleteAllIndustrialClasses = value; }

        /// <summary>
        /// Set to true to delete all existing laws within legislation (the legislation collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllLaws { get => base.DeleteAllLaws; set => base.DeleteAllLaws = value; }

        /// <summary>
        /// Set to true to delete all existing life events (the LifeEvents collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteAllLifeEvents { get => base.DeleteAllLifeEvents; set => base.DeleteAllLifeEvents = value; }

        /// <summary>
        /// Set to true to delete service charge type (ServiceChargeType property for this object should be empty when this option is used).
        /// </summary>
        [JsonIgnore]
        public override bool DeleteServiceChargeType { get => base.DeleteServiceChargeType; set => base.DeleteServiceChargeType = value; }
    }
}
