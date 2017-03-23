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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PTV.Database.Model.Models.Base;
using PTV.Database.Model.Interfaces;
using PTV.Framework.Formatters;
using PTV.Framework.Formatters.Attributes;

namespace PTV.Database.Model.Models
{
    internal partial class OrganizationVersioned : ValidityBase, IPublishingStatus, IArchivable, IVersionedVolume<Organization>, IMultilanguagedEntity<OrganizationLanguageAvailability>, INameReferences, IRoleBased
    {
        public OrganizationVersioned()
        {
            OrganizationWebAddress = new HashSet<OrganizationWebPage>();
            OrganizationNames = new HashSet<OrganizationName>();
            OrganizationDescriptions = new HashSet<OrganizationDescription>();
            OrganizationPhones = new HashSet<OrganizationPhone>();
            OrganizationEmails = new HashSet<OrganizationEmail>();
            OrganizationAddresses = new HashSet<OrganizationAddress>();
            LanguageAvailabilities = new HashSet<OrganizationLanguageAvailability>();
        }

        public Guid? BusinessId { get; set; }
        public Guid? MunicipalityId { get; set; }
        [MaxLength(100)]
        [ValueFormatter(typeof(TrimSpacesFormatter))]
        public string Oid { get; set; }
        public Guid? ParentId { get; set; }
        public Guid TypeId { get; set; }
        public Guid DisplayNameTypeId { get; set; }

        public virtual ICollection<OrganizationWebPage> OrganizationWebAddress { get; set; }
        public virtual ICollection<OrganizationName> OrganizationNames { get; set; }
        public virtual ICollection<OrganizationDescription> OrganizationDescriptions { get; set; }
        public virtual ICollection<OrganizationEmail> OrganizationEmails { get; set; }
        public virtual ICollection<OrganizationPhone> OrganizationPhones { get; set; }
        public virtual OrganizationType Type { get; set; }
        public virtual ICollection<OrganizationAddress> OrganizationAddresses { get; set; }
        public virtual Municipality Municipality { get; set; }
        public virtual Business Business { get; set; }
        public virtual NameType DisplayNameType { get; set; }
        public virtual Organization Parent { get; set; }

        public PublishingStatusType PublishingStatus { get; set; }
        public Guid PublishingStatusId { get; set; }

        public virtual ICollection<OrganizationLanguageAvailability> LanguageAvailabilities { get; set; }
        public Guid? VersioningId { get; set; }
        public Versioning Versioning { get; set; }
        public Guid UnificRootId { get; set; }
        public Organization UnificRoot { get; set; }
        IEnumerable<IName> INameReferences.Names => OrganizationNames;

        [NotMapped]
        bool IVersionedTrackedEntity.VersioningApplied { get; set; }
    }
}
