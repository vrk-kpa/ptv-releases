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
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Domain.Model.Models.Import
{
    /// <summary>
    /// View model of types from json
    /// </summary>
    public class VmJsonTypeItem : IVmOrderable
    {
        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }
        /// <summary>
        /// Gets or sets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        [JsonProperty(Order = 1)]
        public List<VmJsonTypeName> Names { get; set; }
        /// <summary>
        /// Gets or sets the order number.
        /// </summary>
        /// <value>
        /// The order number.
        /// </value>
        public int? OrderNumber { get; set; }
        /// <summary>
        /// Gets or sets the fallback priority.
        /// </summary>
        /// <value>
        /// The fallback priority.
        /// </value>
        public int PriorityFallback { get; set; }

        /// <summary>
        /// Gets or sets the Date.
        /// </summary>
        /// <value>
        /// The Date.
        /// </value>
        public DateTime? Date { get; set; }

        /// <summary>
        /// Gets or sets the Is Valid.
        /// </summary>
        /// <value>
        /// The IsValid.
        /// </value>
        public bool IsValid { get; set; }

        /// <summary>
        /// Serialize only when OrderNumber is not null
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeOrderNumber()
        {
            return OrderNumber != null;
        }
        /// <summary>
        /// Serialize only when PriorityFallback is not null
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializePriorityFallback()
        {
            return PriorityFallback > 0 || Code.ToLower() == "published" && PriorityFallback == 0;
        }



    }

    /// <summary>
    /// View model of type name from json
    /// </summary>
    public class VmJsonTypeName
    {
        /// <summary>
        /// Gets or sets the type identifier.
        /// </summary>
        /// <value>
        /// The type identifier.
        /// </value>
        [JsonIgnore]
        public Guid TypeId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; set; }
    }
}
