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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using PTV.Framework.Attributes.ValidationAttributes;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Domain.Model.Models.OpenApi.V7;
using System;

namespace PTV.Domain.Model.Models.OpenApi.V8
{
    /// <summary>
    /// OPEN API V8 - View Model of organization for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationInVersionBase" />
    public class V8VmOpenApiOrganizationInBase : VmOpenApiOrganizationInVersionBase
    {
        /// <summary>
        /// Organization type (State, Municipality, RegionalOrganization, Organization, Company).
        /// </summary>
        [JsonProperty(Order = 5)]
        [AllowedValues("OrganizationType", new[] { "State", "Municipality", "RegionalOrganization", "Organization", "Company", "TT1", "TT2" })]
        public override string OrganizationType { get => base.OrganizationType; set => base.OrganizationType = value; }

        /// <summary>
        /// List of organization names. Possible type values are: Name, AlternativeName.
        /// </summary>
        [JsonProperty(Order = 13)]
        [ListRequiredToMatchDisplayNameTypes("DisplayNameType")]
        [ListWithOpenApiEnum(typeof(NameTypeEnum), "Type")]
        public override IList<VmOpenApiLocalizedListItem> OrganizationNames { get => base.OrganizationNames; set => base.OrganizationNames = value; }

        /// <summary>
        /// List of Display name types (Name or AlternativeName) for each language version of OrganizationNames.
        /// </summary>
        [ListWithOpenApiEnum(typeof(NameTypeEnum), "Type")]
        public override IList<VmOpenApiNameTypeByLanguage> DisplayNameType { get => base.DisplayNameType; set => base.DisplayNameType = value; }

        /// <summary>
        /// Area type (Nationwide, NationwideExceptAlandIslands, LimitedType).
        /// </summary>
        [JsonProperty(Order = 16)]
        [ValidOpenApiEnum(typeof(AreaInformationTypeEnum))]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// Sub area type (Municipality, Region, BusinessSubRegion, HospitalDistrict).
        /// </summary>
        [JsonProperty(Order = 17)]
        [RequiredIf("AreaType", "LimitedType")]
        [ValidOpenApiEnum(typeof(AreaTypeEnum))]
        public override string SubAreaType { get => base.SubAreaType; set => base.SubAreaType = value; }

        /// <summary>
        /// Area codes related to sub area type. For example if SubAreaType = Municipality, Areas-list need to include municipality codes like 491 or 091.
        /// </summary>
        [ListRequiredIf("AreaType", "LimitedType")]
        public override IList<string> Areas { get => base.Areas; set => base.Areas = value; }

        /// <summary>
        /// Localized list of organization descriptions. Possible type values are: Description, Summary.
        /// </summary>
        [JsonProperty(Order = 20)]
        [ListPropertyAllowedValues(propertyName: "Type", allowedValues: new[] { "Description", "Summary" })]
        [ListPropertyMaxLength(2500, "Value", "Description")]
        [ListPropertyMaxLength(150, "Value", "Summary")]
        public override IList<VmOpenApiLocalizedListItem> OrganizationDescriptions { get => base.OrganizationDescriptions; set => base.OrganizationDescriptions = value; }

        /// <summary>
        /// List of organizations phone numbers.
        /// </summary>
        [JsonProperty(Order = 22)]
        [ListWithOpenApiEnum(typeof(ServiceChargeTypeEnum), "ServiceChargeType")]
        public override IList<V4VmOpenApiPhone> PhoneNumbers { get => base.PhoneNumbers; set => base.PhoneNumbers = value; }

        /// <summary>
        /// List of organizations web pages.
        /// </summary>
        [JsonProperty(Order = 23)]
        [LocalizedListPropertyDuplicityForbidden("OrderNumber")]
        public virtual new IList<VmOpenApiWebPageWithOrderNumber> WebPages { get; set; } = new List<VmOpenApiWebPageWithOrderNumber>();

        /// <summary>
        /// List of addresses.
        /// </summary>
        [JsonProperty(Order = 24)]
        public virtual new IList<V7VmOpenApiAddressWithForeignIn> Addresses { get; set; } = new List<V7VmOpenApiAddressWithForeignIn>();

        /// <summary>
        /// Organization publishing status. Values: Draft, Published, Deleted or Modified.
        /// </summary>
        [AllowedValues("PublishingStatus", new[] { "Draft", "Published", "Deleted", "Modified" })]
        [ValidEnum(null)] // PTV-1792: suppress base ValidEnum validation
        public override string PublishingStatus
        {
            get => base.PublishingStatus;
            set => base.PublishingStatus = value;
        }

// SOTE has been disabled (SFIPTV-1177)
//        /// <summary>
//        /// Responsible organization identifier.
//        /// </summary>
//        [JsonIgnore]
//        public override string ResponsibleOrganizationId { get => base.ResponsibleOrganizationId; set => base.ResponsibleOrganizationId = value; }

        /// <summary>
        /// Date when item should be published.
        /// </summary>
        [JsonIgnore]
        public override DateTime? ValidFrom { get => base.ValidFrom; set => base.ValidFrom = value; }

        /// <summary>
        /// Date when item should be archived.
        /// </summary>
        [JsonIgnore]
        public override DateTime? ValidTo { get => base.ValidTo; set => base.ValidTo = value; }

        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 8;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiOrganizationInVersionBase VersionBase()
        {
            var model = base.GetInVersionBaseModel<VmOpenApiOrganizationInVersionBase>(VersionNumber());
            this.WebPages.ForEach(p => model.WebPages.Add(p.ConvertToInBaseModel()));//PTV-3705
            this.Addresses.ForEach(a => model.Addresses.Add(a.ConvertToInBaseModel()));
            return model;
        }
    }
}
