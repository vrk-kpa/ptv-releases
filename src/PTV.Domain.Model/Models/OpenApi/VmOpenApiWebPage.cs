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
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// Open API web page model.
    /// </summary>
    /// <remarks>
    /// <para>For allowed <i>Type</i> values <see cref="PTV.Domain.Model.Enums.WebPageTypeEnum"/></para>
    /// </remarks>
    public class VmOpenApiWebPage : VmOpenApiLocalizedListItem, IVmOpenApiWebPage, IVmOpenApiBase
    {
        /// <summary>
        /// Web page description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Web page url.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [Url]
        [MaxLength(500)]
        public string Url { get; set; }

        // TODO : suggestion use JsonProperty to name the value as name in JSON?

        /// <summary>
        /// Name of the web page.
        /// </summary>
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
        /// Web page internal identifier.
        /// </summary>
        [JsonIgnore]
        public Guid? Id { get; set; }

        /// <summary>
        /// Internal flag to tell if there can be one or many web pages per language (a list of web pages or a single page, how it is displayed in the app UI).
        /// </summary>
        [JsonIgnore]
        public bool ExistsOnePerLanguage { get; set; }
    }
}
