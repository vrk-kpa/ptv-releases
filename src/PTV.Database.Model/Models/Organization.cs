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
using PTV.Database.Model.Models.Base;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.Model.Models
{
    internal partial class Organization : ValidityBase, IHierarchy<Organization>, IPublishingStatus, IArchivable
    {
        public Organization()
        {
            OrganizationWebAddress = new HashSet<OrganizationWebPage>();
            OrganizationNames = new HashSet<OrganizationName>();
            OrganizationDescriptions = new HashSet<OrganizationDescription>();
            OrganizationPhones = new HashSet<OrganizationPhone>();
            OrganizationEmails = new HashSet<OrganizationEmail>();
            OrganizationServices = new HashSet<OrganizationService>();
            OrganizationAddresses = new HashSet<OrganizationAddress>();
            Children = new List<Organization>();
        }

        public Guid? BusinessId { get; set; }
        public Guid? MunicipalityId { get; set; }
        public string Oid { get; set; }
        public Guid? ParentId { get; set; }
        public Guid TypeId { get; set; }
        public Guid DisplayNameTypeId { get; set; }

        public virtual ICollection<OrganizationWebPage> OrganizationWebAddress { get; set; }
        public virtual ICollection<OrganizationName> OrganizationNames { get; set; }
        public virtual ICollection<OrganizationDescription> OrganizationDescriptions { get; set; }
        public virtual ICollection<OrganizationService> OrganizationServices { get; set; }
        public virtual ICollection<OrganizationEmail> OrganizationEmails { get; set; }
        public virtual ICollection<Organization> Children { get; set; }
        public virtual ICollection<OrganizationPhone> OrganizationPhones { get; set; }
        public virtual OrganizationType Type { get; set; }
        public virtual ICollection<OrganizationAddress> OrganizationAddresses { get; set; }
        public virtual Municipality Municipality { get; set; }
        public virtual Business Business { get; set; }
        public virtual NameType DisplayNameType { get; set; }
        public Organization Parent { get; set; }

        public PublishingStatusType PublishingStatus { get; set; }
        public Guid? PublishingStatusId { get; set; }
    }
}
