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

using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework;

namespace PTV.Domain.Model.Models.OpenApi.V9
{
    /// <summary>
    /// OPEN API V9 - View Model of organization
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationVersionBase" />
    public class V9VmOpenApiOrganization : VmOpenApiOrganizationVersionBase
    {
        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 9;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiOrganizationVersionBase PreviousVersion()
        {
            var vm = base.GetVersionBaseModel<V8VmOpenApiOrganization>();
            // In version 8 and older we do not have SotePublic, SotePrivate and Region available. (SFIPTV-40)
            if (vm.OrganizationType == OrganizationTypeEnum.SotePublic.ToString())
            {
                vm.OrganizationType = OrganizationTypeEnum.RegionalOrganization.ToString();
            }
            else if (vm.OrganizationType == OrganizationTypeEnum.SotePrivate.ToString())
            {
                vm.OrganizationType = OrganizationTypeEnum.Organization.ToString();
            }
            else if(vm.OrganizationType == OrganizationTypeEnum.Region.ToString())
            {
                vm.OrganizationType = OrganizationTypeEnum.Municipality.ToString();
            }

            this.WebPages.ForEach(w => vm.WebPages.Add(w.ConvertToPreviousVersion()));
            this.Addresses.ForEach(a => vm.Addresses.Add(a.ConvertToVersion7()));
            return vm;
        }
        #endregion
    }
}
