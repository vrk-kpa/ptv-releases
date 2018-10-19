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
using PTV.Framework;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V4;
using System.Linq;
using System;

namespace PTV.Domain.Model.Models.OpenApi.V6
{
    /// <summary>
    /// OPEN API V6 - View Model of general description
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiGeneralDescriptionVersionBase" />
    public class V6VmOpenApiGeneralDescription : VmOpenApiGeneralDescriptionVersionBase
    {
        /// <summary>
        /// List of service classes.
        /// </summary>
        [JsonProperty(Order = 5)]
        public new IList<V4VmOpenApiFintoItem> ServiceClasses { get; set; } = new List<V4VmOpenApiFintoItem>();

        /// <summary>
        /// List of linked service channels including relationship data.
        /// </summary>
        [JsonIgnore]
        public override IList<V6VmOpenApiServiceServiceChannel> ServiceChannels { get => base.ServiceChannels; set => base.ServiceChannels = value; }

        /// <summary>
        /// General description type is not allowed for older versions than 8
        /// </summary>        
        [JsonIgnore]
        public override string GeneralDescriptionType { get => base.GeneralDescriptionType; set => base.GeneralDescriptionType = value; }
        
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
            throw new NotSupportedException("No previous version for class V6VmOpenApiGeneralDescription!");
        }
        #endregion
    }
}
