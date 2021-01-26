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
using System.ComponentModel.DataAnnotations;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Framework.Formatters;
using PTV.Framework.Formatters.Attributes;

namespace PTV.Database.Model.Models
{
    internal partial class Phone : EntityIdentifierBase, IPhone, IOrderable
    {
        public Phone()
        {
            ServiceChannelPhones = new HashSet<ServiceChannelPhone>();
            ServiceServiceChannelPhones = new HashSet<ServiceServiceChannelPhone>();
            ExtraTypes = new HashSet<PhoneExtraType>();
        }

        public Guid? PrefixNumberId { get; set; }
        [MaxLength(20)]
        [TrimSpacesFormatter]
        public string Number { get; set; }
        public Guid ChargeTypeId { get; set; }
        public ServiceChargeType ChargeType { get; set; }
        public Guid TypeId { get; set; }
        public virtual PhoneNumberType Type { get; set; }
        public Guid LocalizationId { get; set; }
        public virtual Language Localization { get; set; }
        public virtual DialCode PrefixNumber { get; set; }
        [MaxLength(150)]
        [TrimSpacesFormatter]
        public string ChargeDescription { get; set; }
        [MaxLength(150)]
        [TrimSpacesFormatter]
        public string AdditionalInformation { get; set; }
        public int? OrderNumber { get; set; }

        public virtual ICollection<ServiceChannelPhone> ServiceChannelPhones { get; set; }
        public virtual ICollection<ServiceServiceChannelPhone> ServiceServiceChannelPhones { get; set; }
        public virtual ICollection<PhoneExtraType> ExtraTypes { get; set; }
    }
}
