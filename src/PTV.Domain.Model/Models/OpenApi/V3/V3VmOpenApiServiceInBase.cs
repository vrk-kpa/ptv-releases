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
using PTV.Domain.Model.Models.Interfaces.OpenApi.V3;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;
using PTV.Framework.Attributes;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V3
{
    /// <summary>
    /// OPEN API V3 - View Model of service for IN api - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceInVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.V3.IV3VmOpenApiServiceInBase" />
    public class V3VmOpenApiServiceInBase : VmOpenApiServiceInVersionBase, IV3VmOpenApiServiceInBase
    {
        /// <summary>
        /// Service type. Possible values are: Service or PermissionAndObligation.
        /// NOTE! If service type has been defined within attached statutory service general description, the type for service is ignored.
        /// </summary>
        [ValidEnum(typeof(ServiceTypeEnum))]
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
        /// List of localized service descriptions.
        /// </summary>
        [ListPropertyMaxLength(2500, "Value", "Description")]
        [ListPropertyMaxLength(2500, "Value", "ServiceUserInstruction")]
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
        /// Localized service usage requirements (description of requirement).
        /// </summary>
        [ListPropertyMaxLength(2500, "Value")]
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
        /// List of laws related to the service.
        /// </summary>
        [JsonIgnore]
        public override IList<V4VmOpenApiLaw> Legislation { get; set; }

        /// <summary>
        /// List of organizations responsible or producing the service.
        /// </summary>
        [ListWithPropertyValueRequired("RoleType", "Responsible")]
        [ListWithPropertyValueRequired("RoleType", "Producer")]
        public virtual new IList<V3VmOpenApiServiceOrganization> ServiceOrganizations { get; set; }


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
            return 3;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceInVersionBase VersionBase()
        {
            var vm = base.GetInVersionBaseModel<VmOpenApiServiceInVersionBase>();
            vm.ServiceOrganizations = new List<V4VmOpenApiServiceOrganization>();
            ServiceOrganizations.ForEach(s => vm.ServiceOrganizations.Add(s.ConvertToVersion4()));
            return vm;
        }
        #endregion
    }
}
