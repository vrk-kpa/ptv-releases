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
using PTV.Domain.Model.Models.Converters;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;

namespace PTV.Domain.Model.Models.V2.Channel
{
    /// <summary>
    /// View model of service location channel step1
    /// </summary>
    /// <seealso cref="VmServiceChannel" />
    public class VmServiceLocationChannelAddress: VmEntityBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VmServiceLocationChannelAddress"/> class.
        /// </summary>
        public VmServiceLocationChannelAddress()
        {
        }
        /// <summary>
        /// Entity identifier.
        /// </summary>
        public Guid ServiceChannelId { get; set; }
        /// <summary>
        /// Entity root identifier.
        /// </summary>
        public Guid UnificRootId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public Dictionary<string, string> Name { get; set; }
        /// <summary>
        /// Street of the address
        /// </summary>
        [JsonConverter(typeof(StreetConverter))]
        public VmStreet Street { get; set; }
        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>
        /// The organization identifier.
        /// </value>
        [JsonProperty("organization")]
        public Guid OrganizationId { get; set; }
        /// <summary>
        /// Street number
        /// </summary>
        public string StreetNumber { get; set; }
        /// <summary>
        /// Postal code model
        /// </summary>
        [JsonConverter(typeof(PostalCodeConverter))]
        public VmPostalCode PostalCode { get; set; }
        /// <summary>
        /// Gets or sets the languages availabilities.
        /// </summary>
        /// <value>
        /// The languages availabilities.
        /// </value>
        public IReadOnlyList<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }
    }
}