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
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Framework;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V6;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi.V6
{
    /// <summary>
    /// OPEN API V6 - View Model of general description
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiGeneralDescriptionVersionBase" />
    public class V6VmOpenApiGeneralDescription : VmOpenApiGeneralDescriptionVersionBase
    {
        
        #region methods
        /// <summary>
        /// Open api version number.
        /// </summary>
        /// <returns>Open api version number</returns>
        public override int VersionNumber()
        {
            return 6;
        }

        /// <summary>
        /// Converts model to previous version.
        /// </summary>
        /// <returns></returns>
        public override IVmOpenApiGeneralDescriptionVersionBase PreviousVersion()
        {
            var vm = GetVersionBaseModel<V4VmOpenApiGeneralDescription>();
            if (vm.Type == ServiceTypeEnum.ProfessionalQualifications.ToString())
            {
                // ProfessionalQualifications type is new service type for version 6 (PTV-1397).
                // In older versions PermissionAndObligation is used.
                vm.Type = ServiceTypeEnum.PermissionAndObligation.ToString();
            }

            // Version 6+ allow Markdown line breaks, older versions do not (PTV-1978)
            // Actually line breaks are allowed also in older versions so replace into space is removed! PTV-2346
            // In version 5 and lower the description type 'BackgroudDescription' does not exist. We have to combine backgroud description with Description (PTV-2347)
            var backGroundList = vm.Descriptions.Where(d => d.Type == DescriptionTypeEnum.BackgroundDescription.ToString());
            backGroundList.ForEach(backGround =>
            {
                var description = vm.Descriptions.Where(d => d.Type == DescriptionTypeEnum.Description.ToString() && d.Language == backGround.Language).FirstOrDefault();
                if (description != null)
                {
                    // Lets combine background description and description
                    description.Value = backGround.Value + "\n" + description.Value;
                }
                else
                {
                    backGround.Type = DescriptionTypeEnum.Description.ToString();
                }
            });
            // Let's remove all background descriptions
            vm.Descriptions = vm.Descriptions.Where(d => d.Type != DescriptionTypeEnum.BackgroundDescription.ToString()).ToList();

            return vm;
        }
        #endregion
    }
}
