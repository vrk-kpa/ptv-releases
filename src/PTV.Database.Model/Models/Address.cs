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
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Attributes;
using PTV.Database.Model.Models.Base;
using PTV.Framework.Formatters.Attributes;

namespace PTV.Database.Model.Models
{
    internal partial class Address : EntityIdentifierBase, IOrderable
    {
        public Address()
        {
            AddressAdditionalInformations = new HashSet<AddressAdditionalInformation>();
            Coordinates = new HashSet<AddressCoordinate>();
            AddressPostOfficeBoxes = new HashSet<AddressPostOfficeBox>();
            AddressForeigns = new HashSet<AddressForeign>();
            AddressOthers = new HashSet<AddressOther>();
            AccessibilityRegisterEntrances = new HashSet<AccessibilityRegisterEntrance>();
            AccessibilityRegisters = new HashSet<AccessibilityRegister>();
            Receivers = new HashSet<AddressReceiver>();
            ExtraTypes = new HashSet<AddressExtraType>();
            ServiceServiceChannelAddresses = new HashSet<ServiceServiceChannelAddress>();
            ServiceChannelAddresses = new HashSet<ServiceChannelAddress>();
            ClsAddressPoints = new HashSet<ClsAddressPoint>();
        }

        public Guid? CountryId { get; set; }
        public Guid TypeId { get; set; }

        public virtual AddressType Type { get; set; }
        public virtual Country Country { get; set; }

        public virtual ICollection<AddressAdditionalInformation> AddressAdditionalInformations { get; set; }
        public virtual ICollection<AddressCoordinate> Coordinates { get; set; }
        public virtual ICollection<AddressPostOfficeBox> AddressPostOfficeBoxes { get; set; }
        public virtual ICollection<AddressForeign> AddressForeigns { get; set; }
        public virtual ICollection<AddressOther> AddressOthers { get; set; }
        public virtual ICollection<AccessibilityRegisterEntrance> AccessibilityRegisterEntrances { get; set; }
        public virtual ICollection<AccessibilityRegister> AccessibilityRegisters { get; set; }
        public virtual ICollection<AddressReceiver> Receivers { get; set; }
        public virtual ICollection<AddressExtraType> ExtraTypes { get; set; }
        public virtual ICollection<ServiceServiceChannelAddress> ServiceServiceChannelAddresses { get; set; }
        public virtual ICollection<ServiceChannelAddress> ServiceChannelAddresses { get; set; }
        public virtual ICollection<ClsAddressPoint> ClsAddressPoints { get; set; }

        public int? OrderNumber { get; set; }
        [NotForeignKey]
        public Guid UniqueId { get; set; }
    }
}