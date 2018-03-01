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
using PTV.Domain.Model.Models.Interfaces.OpenApi.V7;
using PTV.Domain.Model.Models.OpenApi.V6;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Domain.Model.Models.OpenApi.V7
{
    /// <summary>
    /// OPEN API V7 - View Model of service channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiServiceServiceChannelVersionBase" />
    public class V7VmOpenApiServiceServiceChannel : VmOpenApiServiceServiceChannelVersionBase, IV7VmOpenApiServiceServiceChannel
    {
        /// <summary>
        /// Contact details for connection.
        /// </summary>
        [JsonProperty(Order = 7)]
        public new virtual VmOpenApiContactDetails ContactDetails { get; set; }

        #region methods

        /// <summary>
        /// Converts to version 6.
        /// </summary>
        /// <returns>model converted to version 6</returns>
        public V6VmOpenApiServiceServiceChannel ConvertToVersion6()
        {
            return base.GetVersionBaseModel<V6VmOpenApiServiceServiceChannel>();
        }

        /// <summary>
        /// Set contact details values for version 7.
        /// </summary>
        public override void SetV7Values()
        {
            base.SetV7Values();
            if (ContactDetails != null)
            {
                ContactDetails.SetV7Values();
            }
        }

        #endregion
    }
}
