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
using PTV.Domain.Model.Models.Interfaces;
using Newtonsoft.Json;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of phone channel step 1
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.VmEnumEntityStatusBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmPhoneChannelStep1" />
    public class VmPhoneChannelStep1 : VmServiceChannel, IVmPhoneChannelStep1
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmPhoneChannelStep1"/> class.
        /// </summary>
        public VmPhoneChannelStep1()
        {
            Languages = new List<Guid>();
        }

        /// <summary>
        /// Gets or sets the phone channel identifier.
        /// </summary>
        /// <value>
        /// The phone channel identifier.
        /// </value>
        [JsonIgnore]
        public Guid?  PhoneChannelId { get; set; }

        /// <summary>
        /// Gets or sets the web page.
        /// </summary>
        /// <value>
        /// The web page.
        /// </value>
        public VmWebPage WebPage { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public VmEmailData Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public VmPhone PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the language guids.
        /// </summary>
        /// <value>
        /// The languages.
        /// </value>
        public List<Guid> Languages { get; set; }
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public LanguageCode Language { get; set; }
    }
}
