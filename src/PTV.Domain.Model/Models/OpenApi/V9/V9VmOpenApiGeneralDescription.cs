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
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Domain.Model.Models.OpenApi.V9
{
    /// <summary>
    /// OPEN API V9 - View Model of general description
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiGeneralDescriptionVersionBase" />
    public class V9VmOpenApiGeneralDescription : VmOpenApiGeneralDescriptionVersionBase
    {
        /// <summary>
        /// List of statutory service general description languages.
        /// </summary>
        [JsonProperty(Order = 4)]
        [Obsolete("This property is not used in the API anymore. Do not use.", false)]
        public virtual IList<string> Languages { get; set; }

        #region methods
        /// <summary>
        /// Open api version number.
        /// </summary>
        /// <returns>Open api version number</returns>
        public override int VersionNumber()
        {
            return 9;
        }

        /// <summary>
        /// Converts model to previous version.
        /// </summary>
        /// <returns></returns>
        public override IVmOpenApiGeneralDescriptionVersionBase PreviousVersion()
        {
            var vm = GetVersionBaseModel<V8VmOpenApiGeneralDescription>();
/* SOTE has been disabled (SFIPTV-1177)
            // Sote types will be converted into 'Municipality' (SFIPTV-116).
            if (vm.GeneralDescriptionType == GeneralDescriptionTypeEnum.OtherPermissionGrantedSote.ToString())
            {
                vm.GeneralDescriptionType = GeneralDescriptionTypeEnum.BusinessSubregion.ToString();
            }
            else if(vm.GeneralDescriptionType == GeneralDescriptionTypeEnum.PrescribedByFreedomOfChoiceAct.ToString())
            {
                vm.GeneralDescriptionType = GeneralDescriptionTypeEnum.Municipality.ToString();
            }
*/
            // Usage descriptions are not shown in older versions
            vm.Descriptions = vm.Descriptions == null ? null : vm.Descriptions.Where(d => d.Type != DescriptionTypeEnum.GeneralDescriptionTypeAdditionalInformation.ToString()).ToList();
            return vm;
        }
        #endregion
    }
}
