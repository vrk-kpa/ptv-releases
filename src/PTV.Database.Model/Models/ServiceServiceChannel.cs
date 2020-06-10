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
using PTV.Database.Model.Models.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using PTV.Database.Model.Interfaces;
using PTV.Framework.Attributes;

namespace PTV.Database.Model.Models
{
    internal class ServiceServiceChannel : EntityBase, IRoleBased, IChargeType
    {
        public ServiceServiceChannel()
        {
            ServiceServiceChannelDescriptions = new HashSet<ServiceServiceChannelDescription>();
            ServiceServiceChannelDigitalAuthorizations = new HashSet<ServiceServiceChannelDigitalAuthorization>();
            ServiceServiceChannelExtraTypes = new HashSet<ServiceServiceChannelExtraType>();
            ServiceServiceChannelServiceHours = new HashSet<ServiceServiceChannelServiceHours>();
            ServiceServiceChannelAddresses = new HashSet<ServiceServiceChannelAddress>();
            ServiceServiceChannelEmails = new HashSet<ServiceServiceChannelEmail>();
            ServiceServiceChannelPhones = new HashSet<ServiceServiceChannelPhone>();
            ServiceServiceChannelWebPages = new HashSet<ServiceServiceChannelWebPage>();
        }

        public Guid ServiceId { get; set; }
        public Guid ServiceChannelId { get; set; }
        public Guid? ChargeTypeId { get; set; }
        public virtual Service Service { get; set; }
        public virtual ServiceChannel ServiceChannel { get; set; }
        public virtual ServiceChargeType ChargeType { get; set; }
        public virtual ICollection<ServiceServiceChannelDescription> ServiceServiceChannelDescriptions { get; set; }
        public virtual ICollection<ServiceServiceChannelDigitalAuthorization> ServiceServiceChannelDigitalAuthorizations { get; set; }
        public virtual ICollection<ServiceServiceChannelExtraType> ServiceServiceChannelExtraTypes { get; set; }
        public virtual ICollection<ServiceServiceChannelServiceHours> ServiceServiceChannelServiceHours { get; set; }
        public virtual ICollection<ServiceServiceChannelAddress> ServiceServiceChannelAddresses { get; set; }
        public virtual ICollection<ServiceServiceChannelEmail> ServiceServiceChannelEmails { get; set; }
        public virtual ICollection<ServiceServiceChannelPhone> ServiceServiceChannelPhones { get; set; }
        public virtual ICollection<ServiceServiceChannelWebPage> ServiceServiceChannelWebPages { get; set; }
        public bool IsASTIConnection { get; set; }
        /// <summary>
        /// The ordering of channels with respect to a particular service.
        /// </summary>
        [IgnoreWhenModified]
        public int? ChannelOrderNumber { get; set; }
        /// <summary>
        /// The ordering of services with respect to a particular channel.
        /// </summary>
        [IgnoreWhenModified]
        public int? ServiceOrderNumber { get; set; }
        IEnumerable<IDescription> IDescriptionReferences.Descriptions => ServiceServiceChannelDescriptions;
        [NotMapped]
        public Guid? RequestedForServiceChannel { get; set; }
    }
}
