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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of printable form channel
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEntityStatusBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmPrintableFormChannel" />
    public class VmPrintableFormChannel : VmEntityStatusBase, IVmPrintableFormChannel
    {
        /// <summary>
        /// Gets or sets the step1 form.
        /// </summary>
        /// <value>
        /// The step1 form.
        /// </value>
        public VmPrintableFormChannelStep1 Step1Form { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VmPrintableFormChannel"/> class.
        /// </summary>
        public VmPrintableFormChannel()
        {
            Step1Form = new VmPrintableFormChannelStep1();
        }

        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public LanguageCode Language { get; set; }
    }

    /// <summary>
    /// Model for holding data for printable form identifier
    /// </summary>
    public class VmPrintableFormChannelIdentifier : IVmBase
    {
        /// <summary>
        /// Text of form identifier
        /// </summary>
        public string FormIdentifier { get; set; }
        /// <summary>
        /// Owner printable form id
        /// </summary>
        public Guid? PrintableFormChannelId { get; set; }
        /// <summary>
        /// id of localization
        /// </summary>
        [JsonIgnore]
        public Guid? LocalizationId { get; set; }
    }

    /// <summary>
    /// Model for holding data for printable form receiver
    /// </summary>
    public class VmPrintableFormChannelReceiver : IVmBase
    {
        /// <summary>
        /// Text of form receiver
        /// </summary>
        public string FormReceiver { get; set; }
        /// <summary>
        /// Owner printable form id
        /// </summary>
        public Guid? PrintableFormChannelId { get; set; }
        /// <summary>
        /// id of localization
        /// </summary>
        [JsonIgnore]
        public Guid? LocalizationId { get; set; }
    }
}
