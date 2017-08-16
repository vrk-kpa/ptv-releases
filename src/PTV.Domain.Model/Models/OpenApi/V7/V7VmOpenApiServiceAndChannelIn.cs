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
using System;
using System.ComponentModel.DataAnnotations;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V2;
using PTV.Framework.Attributes;

namespace PTV.Domain.Model.Models.OpenApi.V7
{
    /// <summary>
    /// OPEN API V7 - View Model of service and channel relation
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceServiceChannelInVersionBase" />
    public class V7VmOpenApiServiceAndChannelIn : VmOpenApiServiceServiceChannelInVersionBase
    {
        /// <summary>
        /// PTV service identifier.
        /// </summary>
        [Required]
        public override string ServiceId { get => base.ServiceId; set => base.ServiceId = value; }

        /// <summary>
        /// PTV service channel identifier.
        /// </summary>
        [Required]
        public override string ServiceChannelId { get => base.ServiceChannelId; set => base.ServiceChannelId = value; }

        #region methods

        /// <summary>
        /// Gets the in base version model.
        /// </summary>
        /// <returns></returns>
        public override VmOpenApiServiceServiceChannelInVersionBase VersionBase()
        {
            return base.GetInVersionBaseModel();
        }

        #endregion
    }
}
