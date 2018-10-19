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
using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Converters;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View model of address
    /// </summary>
    public class VmAddressSimple : VmAddressSimpleBase, IVmOwnerReference, IVmOrderable
    {
        /// <summary>
        /// Type of the street - Street, PostBox, Both
        /// </summary>
        public string StreetType { get; set; }
        /// <summary>
        /// PoBox value
        /// </summary>
        public Dictionary<string, string> PoBox { get; set; }
        /// <summary>
        /// Street of the address
        /// </summary>
        public Dictionary<string, string> Street { get; set; }
        /// <summary>
        /// Foreign address
        /// </summary>
        public Dictionary<string, string> ForeignAddressText { get; set; }
        /// <summary>
        /// Street Number of the Address
        /// </summary>
        public string StreetNumber { get; set; }
        /// <summary>
        /// Address additional address
        /// </summary>
        public Dictionary<string, string> AdditionalInformation { get; set; }
        /// <summary>
        /// No address additional infomration
        /// </summary>
        public Dictionary<string, string> NoAddressAdditionalInformation { get; set; }
        /// <summary>
        /// Id of Municipality
        /// </summary>
        public Guid? Municipality { get; set; }
        /// <summary>
        /// Country model
        /// </summary>
        [JsonConverter(typeof(CountryConverter))]
        public VmCountry Country { get; set; }
        /// <summary>
        /// Type of address - Visiting, Postal
        /// </summary>
        [JsonIgnore]
        public AddressCharacterEnum AddressCharacter { get; set; }
        /// <summary>
        /// Postal code model
        /// </summary>
        [JsonConverter(typeof(PostalCodeConverter))]
        public VmPostalCode PostalCode { get; set; }
        /// <summary>
        /// Id of entuty owner
        /// </summary>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
        /// <summary>
        /// Id of entuty owner
        /// </summary>
        [JsonIgnore]
        public Guid? OwnerReferenceId2 { get; set; }
        /// <summary>
        /// Order number
        /// </summary>
        [JsonIgnore]
        public int? OrderNumber { get; set; }

        /// <summary>
        /// AccessibilityRegister
        /// </summary>
        public VmAccessibilityRegisterUI AccessibilityRegister { get; set; }
        
        /// <summary>
        /// Gets or sets the form receiver.
        /// </summary>
        /// <value>
        /// The form receiver.
        /// </value>
        public Dictionary<string, string> Receivers { get; set; } 
    }
    
    /// <summary>
    /// View model of address with coordinates
    /// </summary>
    public class VmAddressSimpleBase : VmEntityBase
    {
        /// <summary>
        /// List of all coordinates for Address
        /// </summary>
        public List<VmCoordinate> Coordinates { get; set; }
        
        /// <summary>
        /// UniqueId
        /// </summary>
        public Guid UniqueId { get; set; }
    }
}
