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
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V3;
using PTV.Framework;
using PTV.Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Domain.Model.Models.OpenApi.V2
{
    /// <summary>
    /// OPEN API V2 - View Model of service for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V2.IV2VmOpenApiServiceInBase" />
    public class V2VmOpenApiServiceInBase : VmOpenApiServiceInVersionBase, IV2VmOpenApiServiceInBase
    {
        /// <summary>
        /// Service type. Possible values are: Service, Notice, Registration, Permission or PermissionAndObligation.
        /// NOTE! Current PTV database does not anymore support for types Notice, Registration or Permission - they are automatically mapped into PermissionAndObligation type.
        /// POST and PUT methods accepts old types but GET method only can return Service or Permission types. PermissionAndObligation is automatically converted into Permission!
        /// NOTE 2! If service type has been defined within attached statutory service general description, the type for service is ignored.
        /// </summary>
        [AllowedValues(propertyName: "Type", allowedValues: new[] { "Service", "Notice", "Registration", "Permission", "PermissionAndObligation" })]
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
        /// List of localized service descriptions. In current PTV version database only accepts 2500 characters.
        /// So Description and ServiceUserInstruction will be cut if more characters are added.
        /// </summary>
        [JsonProperty(Order = 6)]
        [ListPropertyMaxLength(4000, "Value", "Description")]
        [ListPropertyMaxLength(4000, "Value", "ServiceUserInstruction")]
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
        /// Localized service usage requirements (description of requirement). In current PTV version database only accepts 2500 characters.
        /// So requirements will be cut if more characters are added.
        /// </summary>
        [ListPropertyMaxLength(4000, "Value")]
        public override IList<VmOpenApiLanguageItem> Requirements
        {
            get
            {
                return base.Requirements;
            }

            set
            {
                base.Requirements = value;
            }
        }

        /// <summary>
        /// Localized service additional information. This property is not used in the API anymore. Do not use.
        /// </summary>
        [JsonProperty(Order = 26)]
        [ListValueNotEmpty("Value")]
        [LocalizedListPropertyDuplicityForbidden("Type")]
        [ListWithEnum(typeof(DescriptionTypeEnum), "Type")]
        [ListPropertyAllowedValues(propertyName: "Type", allowedValues: new[] {"ValidityTimeAdditionalInfo", "ProcessingTimeAdditionalInfo",
            "DeadLineAdditionalInfo", "ChargeTypeAdditionalInfo", "ServiceTypeAdditionalInfo"})]
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        public virtual IReadOnlyList<VmOpenApiLocalizedListItem> ServiceAdditionalInformations { get; set; } = new List<VmOpenApiLocalizedListItem>();

        /// <summary>
        /// List of organizations responsible or producing the service.
        /// </summary>
        [JsonProperty(Order = 29)]
        [ListWithPropertyValueRequired("RoleType", "Responsible")]
        [ListWithPropertyValueRequired("RoleType", "Producer")]
        public virtual new IReadOnlyList<VmOpenApiServiceOrganization> ServiceOrganizations { get; set; }

        /// <summary>
        /// List of laws related to the service.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiLaw> Legislation { get; set; }

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


        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 2;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceInVersionBase VersionBase()
        {
            // it's allowed to use obsolete attributes for conversion V1 <-> V2 into version base
#pragma warning disable 612, 618

            var vm = base.GetInVersionBaseModel<VmOpenApiServiceInVersionBase>();

            // Set the right type for service
            // Previously (in version 1 and 2) the service type could be Service, Notice, Registration or Permission
            if (!string.IsNullOrEmpty(Type))
            {
                if (Type != ServiceTypeEnum.Service.ToString())
                {
                    vm.Type = ServiceTypeEnum.PermissionAndObligation.ToString();
                }
            }

            vm.ServiceDescriptions = ServiceDescriptions.SetListValueLength();
            vm.Requirements = Requirements.SetListValueLength();

            // Combine serviceDescriptions and serviceAdditionalInformations
            if (ServiceAdditionalInformations?.Count > 0)
            {
                if (vm.ServiceDescriptions == null)
                {
                    vm.ServiceDescriptions = ServiceAdditionalInformations.ToList();
                }
                else
                {
                    vm.ServiceDescriptions = vm.ServiceDescriptions.Concat(ServiceAdditionalInformations).ToList();
                    // Check description list for duplicates
                    vm.ServiceDescriptions = vm.ServiceDescriptions.DisregardDuplicates();
                }
            }

            vm.ServiceOrganizations = new List<V4VmOpenApiServiceOrganization>();
            ServiceOrganizations.ForEach(s => vm.ServiceOrganizations.Add(s.ConvertToVersion4()));
            return vm;
#pragma warning restore 612, 618
        }
        #endregion
    }
}
