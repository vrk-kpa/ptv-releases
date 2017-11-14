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
using PTV.Framework.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.OpenApi
{
    /// <summary>
    /// OPEN API - View Model of phone
    /// </summary>
    public class VmOpenApiPhoneSimpleVersionBase : VmOpenApiBase, IVmOpenApiPhoneSimpleVersionBase
    {
        /// <summary>
        /// Prefix for the phone number.
        /// </summary>
        /// <seealso cref="PTV.Domain.Model.Models.OpenApi.VmOpenApiBase" />
        /// <seealso cref="PTV.Domain.Model.Models.Interfaces.OpenApi.IVmOpenApiPhoneSimpleVersionBase" />
        [RegularExpression(@"^\+[0-9]{1,4}$")]
        public virtual string PrefixNumber { get; set; }

        /// <summary>
        /// Phone number.
        /// </summary>
        [Phone]
        [Required]
        [RegularExpression(@"^\d{1,20}$")]
        public string Number { get; set; }

        /// <summary>
        /// Language of this object. Valid values are: fi, sv or en.
        /// </summary>
        [ValidEnum(typeof(LanguageCode))]
        [Required(AllowEmptyStrings = false)]
        public string Language { get; set; }

        /// <summary>
        /// Defines if number is Finnish service number. If true prefix number can be left empty.
        /// </summary>
        public virtual bool IsFinnishServiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the owner reference identifier.
        /// </summary>
        /// <value>
        /// The owner reference identifier.
        /// </value>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }

        /// <summary>
        /// The order of phone number.
        /// </summary>
        [JsonIgnore]
        public int? OrderNumber { get; set; }

        /// <summary>
        /// Returns the phone base model.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns>Phone base model</returns>
        protected TModel GetVersionBaseModel<TModel>() where TModel : IVmOpenApiPhoneSimpleVersionBase, new()
        {
            return new TModel
            {
                PrefixNumber = PrefixNumber,
                Number = Number,
                Language = Language,
                IsFinnishServiceNumber = IsFinnishServiceNumber,
                OwnerReferenceId = OwnerReferenceId
            };
        }
    }
}
