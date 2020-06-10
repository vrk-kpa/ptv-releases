/**
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
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Converters;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of phone
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEntityBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmPhone" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmOwnerReference" />
    public class VmPhone : VmEntityBase, IVmPhone, IVmOwnerReference, IVmOrderable
    {
        /// <summary>
        /// Gets or sets the charge description.
        /// </summary>
        /// <value>
        /// The charge description.
        /// </value>
        public string ChargeDescription { get; set; }
        /// <summary>
        /// Gets or sets the additional information.
        /// </summary>
        /// <value>
        /// The additional information.
        /// </value>
        public string AdditionalInformation { get; set; }
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        /// <value>
        /// The number.
        /// </value>
        [JsonProperty("phoneNumber")]
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets the charge type identifier.
        /// </summary>
        /// <value>
        /// The charge type identifier.
        /// </value>
        public Guid ChargeType { get; set; }

        /// <summary>
        /// Gets or sets the prefix number.
        /// </summary>
        /// <value>
        /// The prefix number.
        /// </value>
        [JsonConverter(typeof(DialCodeConverter))]
        public VmDialCode DialCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is finnish service number.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is finnish service number; otherwise, <c>false</c>.
        /// </value>
        public bool IsLocalNumber { get; set; }

        /// <summary>
        /// Gets or sets the language identifier.
        /// </summary>
        /// <value>
        /// The language identifier.
        /// </value>
        [JsonIgnore]
        public Guid? LanguageId { get; set; }
        /// <summary>
        /// Id of owner entity
        /// </summary>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
        /// <summary>
        /// Id of owner entity
        /// </summary>
        [JsonIgnore]
        public Guid? OwnerReferenceId2 { get; set; }
        /// <summary>
        /// Order of entity
        /// </summary>
        [JsonIgnore]
        public int? OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the type identifier.
        /// </summary>
        /// <value>
        /// The type identifier.
        /// </value>
//        [JsonIgnore]
        [JsonProperty("Type")]
        public Guid TypeId { get; set; }

        /// <summary>
        /// Gets or sets the extra types
        /// </summary>
//SFIPTV-814        [JsonIgnore]
        public List<Guid> ExtraTypes { get; set; }
    }
    /// <summary>
    /// View model of phone
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEntityBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmPhone" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmOwnerReference" />
    public class VmPhoneWithType : VmPhone
    {
        /// <summary>
        /// Gets or sets the type identifier.
        /// </summary>
        /// <value>
        /// The type identifier.
        /// </value>
        public Guid Type { get => TypeId; set => TypeId = value; }
    }
}
