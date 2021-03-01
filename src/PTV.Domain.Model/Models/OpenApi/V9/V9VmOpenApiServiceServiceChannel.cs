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

using Newtonsoft.Json;
using PTV.Domain.Model.Models.OpenApi.V8;
using PTV.Framework;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.OpenApi.V9
{
    /// <summary>
    /// OPEN API V9 - View Model of service channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceServiceChannelVersionBase" />
    public class V9VmOpenApiServiceServiceChannel : VmOpenApiServiceServiceChannelVersionBase
    {
        /// <summary>
        /// List of connection related service hours.
        /// </summary>
        [JsonProperty(Order = 6)]
        public virtual new IList<V8VmOpenApiServiceHour> ServiceHours { get; set; } = new List<V8VmOpenApiServiceHour>();

        /// <summary>
        /// Date when connection was modified/created (UTC).
        /// </summary>
        [JsonIgnore]
        public override DateTime Modified { get => base.Modified; set => base.Modified = value; }

        #region methods

        /// <summary>
        /// Converts to version 8.
        /// </summary>
        /// <returns></returns>
        public V8VmOpenApiServiceServiceChannel ConvertToVersion8()
        {
            var vm = GetVersionBaseModel<V8VmOpenApiServiceServiceChannel>();
            this.ExtraTypes?.ForEach(e => vm.ExtraTypes.Add(e.ConvertToVersion8()));// PTV-4184
            vm.ContactDetails = this.ContactDetails?.ConvertToVersion8();// PTV-3705
            vm.ServiceHours = this.ServiceHours;// SFIPTV-1912
            return vm;
        }

        #endregion
    }
}
