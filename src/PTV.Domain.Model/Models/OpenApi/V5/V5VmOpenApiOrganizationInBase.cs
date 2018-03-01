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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V5;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using PTV.Framework.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V5
{
    /// <summary>
    /// OPEN API V5 - View Model of organization for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V5.IV5VmOpenApiOrganizationInBase" />
    public class V5VmOpenApiOrganizationInBase : VmOpenApiOrganizationInVersionBase, IV5VmOpenApiOrganizationInBase
    {
        /// <summary>
        /// List of organization names.
        /// </summary>
        [JsonProperty(Order = 13)]
        [ListRequiredToMatchDisplayNameTypes("DisplayNameType")]
        [ListWithEnum(typeof(NameTypeEnum), "Type")]
        public override IList<VmOpenApiLocalizedListItem> OrganizationNames { get => base.OrganizationNames; set => base.OrganizationNames = value; }

        /// <summary>
        /// List of Display name types (Name or AlternateName) for each language version of OrganizationNames.
        /// </summary>
        [JsonProperty(Order = 14)]
        [ListWithEnum(typeof(NameTypeEnum), "Type")]
        public override IList<VmOpenApiNameTypeByLanguage> DisplayNameType { get => base.DisplayNameType; set => base.DisplayNameType = value; }

        /// <summary>
        /// Area type (WholeCountry, WholeCountryExceptAlandIslands, AreaType). 
        /// </summary>
        [JsonProperty(Order = 16)]
        [ValidEnum(typeof(AreaInformationTypeEnum))]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// Sub area type (Municipality, Province, BusinessRegions, HospitalRegions).
        /// </summary>
        [RequiredIf("AreaType", "AreaType")]
        [ValidEnum(typeof(AreaTypeEnum))]
        [JsonProperty(Order = 17)]
        public override string SubAreaType { get => base.SubAreaType; set => base.SubAreaType = value; }

        /// <summary>
        /// Area codes related to sub area type. For example if SubAreaType = Municipality, Areas-list need to include municipality codes like 491 or 091.
        /// </summary>
        [ListRequiredIf("AreaType", "AreaType")]
        public override IList<string> Areas { get => base.Areas; set => base.Areas = value; }

        /// <summary>
        /// Localized list of organization descriptions.
        /// </summary>
        [ListValueNotEmpty("Value")]
        [JsonProperty(Order = 15)]
        [ListPropertyMaxLength(2500, "Value")]
        [LocalizedListLanguageDuplicityForbidden]
        public new IList<VmOpenApiLanguageItem> OrganizationDescriptions { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// List of email addresses.
        /// </summary>
        [Display(Name = "EmailAddresses")]
        [JsonProperty(Order = 21, PropertyName = "emailAddresses")]
        public override IList<V4VmOpenApiEmail> Emails { get => base.Emails; set => base.Emails = value; }

        /// <summary>
        /// List of organizations phone numbers.
        /// </summary>
        [ListWithEnum(typeof(ServiceChargeTypeEnum), "ServiceChargeType")]
        public override IList<V4VmOpenApiPhone> PhoneNumbers { get => base.PhoneNumbers; set => base.PhoneNumbers = value; }

        /// <summary>
        /// List of addresses.
        /// </summary>
        [JsonProperty(Order = 23)]
        public new IList<V5VmOpenApiAddressWithTypeIn> Addresses { get; set; } = new List<V5VmOpenApiAddressWithTypeIn>();

        /// <summary>
        /// Service channel publishing status. Values: Draft, Published, Deleted or Modified.
        /// </summary>
        [AllowedValues("PublishingStatus", new[] { "Draft", "Published", "Deleted", "Modified" })]
        [ValidEnum(null)] // PTV-1792: suppress base ValidEnum validation
        public override string PublishingStatus
        {
            get => base.PublishingStatus;
            set => base.PublishingStatus = value;
        }

        /// <summary>
        /// List of organizations electronic invoicing information.
        /// </summary>
        [JsonIgnore]
        override public IList<VmOpenApiOrganizationEInvoicing> ElectronicInvoicings { get => base.ElectronicInvoicings; set => base.ElectronicInvoicings = value; }

        /// <summary>
        /// Set to true to delete all existing electronic invoicing addresses (the ElectronicInvoicings collection for this object should be empty collection when this option is used).
        /// </summary>
        [JsonIgnore]
        override public bool DeleteAllElectronicInvoicings { get => base.DeleteAllElectronicInvoicings; set => base.DeleteAllElectronicInvoicings = value; }

        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 5;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiOrganizationInVersionBase VersionBase()
        {
            var vm = base.GetInVersionBaseModel<VmOpenApiOrganizationInVersionBase>(VersionNumber());

            vm.OrganizationDescriptions = new List<VmOpenApiLocalizedListItem>();
            OrganizationDescriptions.ForEach(o => 
                vm.OrganizationDescriptions.Add(
                    new VmOpenApiLocalizedListItem() {
                        Language = o.Language,
                        Value = o.Value,
                        OwnerReferenceId = o.OwnerReferenceId,
                        Type = DescriptionTypeEnum.Description.ToString()
                    })
                );
            Addresses.ForEach(address =>
            {
                vm.Addresses.Add(address.LatestVersion<V7VmOpenApiAddressWithForeignIn>());
            });

            return vm;
        }
    }
}
