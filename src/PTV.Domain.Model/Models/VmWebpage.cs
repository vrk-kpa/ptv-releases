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
using PTV.Domain.Model.Models.Interfaces;
using System;
using Newtonsoft.Json;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of web page
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEntityBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmUrl" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmOwnerReference" />
    public class VmWebPage : VmUrl, IVmOrderable
    {
        /// <summary>
        /// Gets or sets the type identifier.
        /// </summary>
        /// <value>
        /// The type identifier.
        /// </value>
        [JsonProperty("Type")]
        public Guid? TypeId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the order number.
        /// </summary>
        /// <value>
        /// The order number.
        /// </value>
        [JsonIgnore]
        public int? OrderNumber { get; set; }
        /// <summary>
        /// Id of owner entity
        /// </summary>
        [JsonIgnore]
        public Guid? OwnerReferenceId2 { get; set; }
    }

    /// <summary>
    /// View model of simple web page
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEntityBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmUrl" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmOwnerReference" />
    public class VmUrl : VmEntityBase, IVmUrl
    {
        /// <summary>
        /// Gets or sets the URL address.
        /// </summary>
        /// <value>
        /// The URL address.
        /// </value>
        public string UrlAddress { get; set; }
        /// <summary>
        /// Id of related web page
        /// </summary>
        [JsonIgnore]
        public Guid? WebPageId { get; set; }
        /// <summary>
        /// Id of language for web page
        /// </summary>
        [JsonIgnore]
        public Guid? LocalizationId { get; set; }
    }


}
