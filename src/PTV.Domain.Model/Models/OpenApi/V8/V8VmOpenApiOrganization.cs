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
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Domain.Model.Models.OpenApi.V8
{
    /// <summary>
    /// OPEN API V87 - View Model of organization
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiOrganizationVersionBase" />
    public class V8VmOpenApiOrganization : VmOpenApiOrganizationVersionBase
    {
        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 8;
        }

        /// <summary>
        /// Gets the previous version.
        /// </summary>
        /// <returns>previous version</returns>
        public override IVmOpenApiOrganizationVersionBase PreviousVersion()
        {
            var vm = base.GetVersionBaseModel<V7VmOpenApiOrganization>();
            // Change property values (name type, service charge type, area information type, area type) into ones used in older versions (PTV-2184)
            vm.OrganizationNames.ForEach(n => n.SetV7Values<NameTypeEnum>());
            vm.DisplayNameType.ForEach(d => d.Type = d.Type.GetEnumValueByOpenApiEnumValue<NameTypeEnum>());
            vm.OrganizationDescriptions.ForEach(d => d.SetV7Values<DescriptionTypeEnum>());
            vm.PhoneNumbers.ForEach(p => p.SetV7Values());
            vm.AreaType = vm.AreaType == null ? null : vm.AreaType.GetEnumValueByOpenApiEnumValue<AreaInformationTypeEnum>();
            //if (vm.Areas?.Count > 0)
            //{
                vm.Areas?.ForEach(a => a.SetV7Values());
            //}
            return vm;
        }
        #endregion
    }
}
