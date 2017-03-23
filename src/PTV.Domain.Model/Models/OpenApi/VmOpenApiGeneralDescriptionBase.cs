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

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Framework.Attributes;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of general description - base
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiGeneralDescriptionBase" />
    public class VmOpenApiGeneralDescriptionBase : IVmOpenApiGeneralDescriptionBase
    {
        /// <summary>
        /// Entity Guid identifier.
        /// </summary>
        [JsonProperty(Order = 1)]
        public virtual Guid? Id { get; set; }

        /// <summary>
        /// List of localized names.
        /// </summary>
        [JsonProperty(Order = 2)]
        [ListWithEnum(typeof(NameTypeEnum), "Type")]
        [ListPropertyMaxLength(100, "Value")]
        public virtual IReadOnlyList<VmOpenApiLocalizedListItem> Names { get; set; }

        /// <summary>
        /// List of localized descriptions.
        /// </summary>
        [JsonProperty(Order = 3)]
        [ListWithEnum(typeof(DescriptionTypeEnum), "Type")]
        [ListPropertyMaxLength(150, "Value", "ShortDescription")]
        [ListPropertyMaxLength(4000, "Value", "Description")]
        [ListPropertyMaxLength(4000, "Value", "ServiceUserInstruction")]
        public virtual IReadOnlyList<VmOpenApiLocalizedListItem> Descriptions { get; set; }

        /// <summary>
        /// List of statutory service general description languages.
        /// </summary>
        [JsonProperty(Order = 4)]
        [ListRegularExpression(@"^[a-z]{2}$")]
        public virtual IReadOnlyList<string> Languages { get; set; }

        /// <summary>
        /// Localized service usage requirements (description of requirement).
        /// </summary>
        [JsonProperty(Order = 10)]
        [ListPropertyMaxLength(4000, "Value")]
        public virtual IReadOnlyList<VmOpenApiLanguageItem> Requirements { get; set; } = new List<VmOpenApiLanguageItem>();

        /// <summary>
        /// Service type. Possible values are: Service, Notice, Registration or Permission.
        /// </summary>
        [JsonProperty(Order = 11)]
        [ValidEnum(typeof(ServiceTypeEnum))]
        public virtual string Type { get; set; }

        /// <summary>
        /// Service charge type. Possible values are: Charged, Free or Other
        /// </summary>
        [JsonProperty(Order = 12)]
        [ValidEnum(typeof(ServiceChargeTypeEnum))]
        public virtual string ServiceChargeType { get; set; }

        /// <summary>
        /// Laws that a general description is based on.
        /// </summary>
        [JsonProperty(Order = 13)]
        public virtual IList<V4VmOpenApiLaw> Legislation { get; set; }

        /// <summary>
        /// Gets general description base model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        protected virtual TModel GetBaseModel<TModel>() where TModel : IVmOpenApiGeneralDescriptionBase, new()
        {
            return new TModel
            {
                Id = this.Id,
                Names = this.Names,
                Descriptions = this.Descriptions,
                Languages = this.Languages,
                Requirements = this.Requirements,
                Type = this.Type,
                ServiceChargeType = this.ServiceChargeType,
                Legislation = this.Legislation,
            };
        }
    }
}