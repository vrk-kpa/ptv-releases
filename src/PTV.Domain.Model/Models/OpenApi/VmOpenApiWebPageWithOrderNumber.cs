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

using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V3;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of web page with order number
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiWebPageVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiWebPageWithOrderNumber" />
    public class VmOpenApiWebPageWithOrderNumber : VmOpenApiWebPageVersionBase, IVmOpenApiWebPageWithOrderNumber
    {
        /// <summary>
        /// The order of web pages.
        /// </summary>
        [RegularExpression(@"^[0-9]+$")]
        [Required]
        public string OrderNumber { get; set; }

        /// <summary>
        /// Name of the web page.
        /// </summary>
        [MaxLength(110)]
        public override string Value
        {
            get
            {
                return base.Value;
            }

            set
            {
                base.Value = value;
            }
        }

        /// <summary>
        /// Converts to version3.
        /// </summary>
        /// <returns>model converted to version 3</returns>
        public V3VmOpenApiWebPage ConvertToVersion3()
        {
           return ConvertToVersion<V3VmOpenApiWebPage>();
        }
    }
}
