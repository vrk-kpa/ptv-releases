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
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of web page - base version
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiWebPageVersionBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiBase" />
    public class VmOpenApiWebPageVersionBase : IVmOpenApiWebPageVersionBase, IVmOpenApiBase
    {
        /// <summary>
        /// Web page url.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [Url]
        [MaxLength(500)]
        public string Url { get; set; }

        /// <summary>
        /// Name of the web page.
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// Language code.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [ValidEnum(typeof(LanguageCode))]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonIgnore]
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or sets the owner reference identifier.
        /// </summary>
        /// <value>
        /// The owner reference identifier.
        /// </value>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }

        /// <summary>
        /// Internal flag to tell if there can be one or many web pages per language (a list of web pages or a single page, how it is displayed in the app UI).
        /// </summary>
        [JsonIgnore]
        public bool ExistsOnePerLanguage { get; set; }

        // it's allowed to use obsolete attributes for conversion V1 <-> V2
#pragma warning disable 612, 618
        /// <summary>
        /// Converts to version2.
        /// </summary>
        /// <returns>model converted to version 2</returns>
        public VmOpenApiWebPage ConvertToVersion2()
        {
            var vm = ConvertToVersion<VmOpenApiWebPage>();
            vm.Type = WebPageTypeEnum.HomePage.ToString();
            return vm;
        }

        /// <summary>
        /// Converts to version4.
        /// </summary>
        /// <returns>model converted to version 4</returns>
        public V4VmOpenApiWebPage ConvertToVersion4()
        {
            var vm = ConvertToVersion<V4VmOpenApiWebPage>();
            return vm;
        }

        /// <summary>
        /// Converts to web page with order number.
        /// </summary>
        /// <returns>model converted with order number</returns>
        public VmOpenApiWebPageWithOrderNumber ConvertToWebPageWithOrderNumber()
        {
            var vm = ConvertToVersion<VmOpenApiWebPageWithOrderNumber>();
            vm.OrderNumber = "0";
            return vm;
        }

        /// <summary>
        /// Converts to version.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <returns>converted model</returns>
        protected TModel ConvertToVersion<TModel>() where TModel : VmOpenApiWebPageVersionBase, new()
        {
            return new TModel()
            {
                Id = this.Id,
                ExistsOnePerLanguage = this.ExistsOnePerLanguage,
                Language = this.Language,
                OwnerReferenceId = this.OwnerReferenceId,
                Url = this.Url,
                Value = this.Value.SetStringValueLength(110)
            };
        }
#pragma warning restore 612, 618
    }
}
