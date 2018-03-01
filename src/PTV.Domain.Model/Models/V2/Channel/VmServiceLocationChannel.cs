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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;

namespace PTV.Domain.Model.Models.V2.Channel
{
    /// <summary>
    /// View model of service location channel step1
    /// </summary>
    /// <seealso cref="VmServiceChannel" />
    public class VmServiceLocationChannel : VmServiceChannel, ILanguages, IOpeningHours
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmServiceChannel"/> class.
        /// </summary>
        public VmServiceLocationChannel()
        {
            Languages = new List<Guid>();
            VisitingAddresses = new List<VmAddressSimple>();
            PostalAddresses = new List<VmAddressSimple>();
            WebPages = new Dictionary<string, List<VmWebPage>>();
            FaxNumbers = new Dictionary<string, List<VmPhone>>();
        }

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
        /// <summary>
        /// Gets or sets the faxes.
        /// </summary>
        /// <value>
        /// The fax.
        /// </value>
        public Dictionary<string, List<VmPhone>> FaxNumbers { get; set; }
        /// <summary>
        /// Gets or sets the web pages.
        /// </summary>
        /// <value>
        /// The web pages.
        /// </value>
        public Dictionary<string, List<VmWebPage>> WebPages { get; set; }
        /// <summary>
        /// Gets or sets the visiting addresses.
        /// </summary>
        /// <value>
        /// The visiting addresses.
        /// </value>
        public List<VmAddressSimple> VisitingAddresses { get; set; }
        /// <summary>
        /// Gets or sets the postal addresses.
        /// </summary>
        /// <value>
        /// The postal addresses.
        /// </value>
        public List<VmAddressSimple> PostalAddresses { get; set; }

        /// <summary>
        /// Gets or sets the opening hours.
        /// </summary>
        /// <value>
        /// The opening hours.
        /// </value>
        public VmOpeningHours OpeningHours { get; set; }

        /// <summary>
        /// Gets or sets the accesiibility register info
        /// </summary>
        public VmAccessibilityRegister AccessibilityRegister { get; set; }
    }
}