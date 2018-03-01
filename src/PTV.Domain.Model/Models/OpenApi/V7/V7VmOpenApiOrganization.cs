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
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Framework;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.OpenApi.V4;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi.V7
{
    /// <summary>
    /// OPEN API V7 - View Model of organization
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationVersionBase" />
    public class V7VmOpenApiOrganization : VmOpenApiOrganizationVersionBase
    {
        /// <summary>
        /// Organizations parent organization identifier if exists.
        /// </summary>
        [Display(Name = "ParentOrganization")]
        [JsonProperty(Order = 3, PropertyName = "parentOrganization")]
        public override Guid? ParentOrganizationId { get => base.ParentOrganizationId; set => base.ParentOrganizationId = value; }

        /// <summary>
        /// List of Display name types (Name or AlternateName) for each language version of OrganizationNames.
        /// </summary>
        [JsonProperty(Order = 14)]
        public override IList<VmOpenApiNameTypeByLanguage> DisplayNameType { get => base.DisplayNameType; set => base.DisplayNameType = value; }

        /// <summary>
        /// Area type (WholeCountry, WholeCountryExceptAlandIslands, AreaType). 
        /// </summary>
        [JsonProperty(Order = 16)]
        public override string AreaType { get => base.AreaType; set => base.AreaType = value; }

        /// <summary>
        /// List of organizations email addresses.
        /// </summary>
        [Display(Name = "EmailAddresses")]
        [JsonProperty(Order = 21, PropertyName = "emailAddresses")]
        public override IReadOnlyList<V4VmOpenApiEmail> Emails { get => base.Emails; set => base.Emails = value; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 7;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiOrganizationVersionBase PreviousVersion()
        {
            var vm = base.GetVersionBaseModel<V6VmOpenApiOrganization>();
            Addresses.ForEach(a => vm.Addresses.Add(a.ConvertToVersion5()));
            // handle organization services - 'OtherResponsible' role available only for version 7 (and upper)
            vm.Services.ForEach(o =>
            {
                if (o.RoleType == "OtherResponsible")
                {
                    o.RoleType = "Responsible";
                }
            });
            return vm;
        }
        #endregion
    }
}
