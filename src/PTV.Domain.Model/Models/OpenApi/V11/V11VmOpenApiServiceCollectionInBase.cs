﻿/**
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

using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework;
using PTV.Framework.Attributes;

namespace PTV.Domain.Model.Models.OpenApi.V11
{
    /// <summary>
    /// OPEN API V11 - View Model of service collection IN - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceCollectionInVersionBase" />
    public class V11VmOpenApiServiceCollectionInBase : VmOpenApiServiceCollectionInVersionBase, IVmOpenApiServiceCollectionInVersionBase
    {
        /// <summary>
        /// List of localized service colections descriptions. Possible type values are: Description, Summary.
        /// </summary>
        [ListPropertyAllowedValues(propertyName: "Type", allowedValues: new[] {"Summary", "Description"})]
        [ListPropertyMaxLength(150, "Value", "Summary")]
        [ListPropertyMaxLength(2500, "Value", "Description")]
        public override IList<VmOpenApiLocalizedListItem> ServiceCollectionDescriptions { get => base.ServiceCollectionDescriptions; set => base.ServiceCollectionDescriptions = value; }

        #region methods
        /// <summary>
        /// Gets the version number.
        /// </summary>
        /// <returns>version number</returns>
        public override int VersionNumber()
        {
            return 11;
        }

        /// <summary>
        /// Gets the base version.
        /// </summary>
        /// <returns>base version</returns>
        public override IVmOpenApiServiceCollectionInVersionBase VersionBase()
        {
            var vm = base.GetInVersionBaseModel<VmOpenApiServiceCollectionInVersionBase>();

            // Set services
            if (Services?.Count > 0)
            {
                Services.ForEach(s => vm.ServiceCollectionServices.Add(s.ParseToGuid().Value));
            }
            return vm;
        }
        #endregion
    }
}
