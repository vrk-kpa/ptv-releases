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
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PTV.Database.Model.Models
{
    internal class ServiceCollectionVersioned : ValidityBase, IPublishingStatus, IArchivable, IVersionedVolume<ServiceCollection>, IMultilanguagedEntity<ServiceCollectionLanguageAvailability>, IRoleBased
    {
        public ServiceCollectionVersioned()
        {
            ServiceCollectionNames = new HashSet<ServiceCollectionName>();
            ServiceCollectionDescriptions = new HashSet<ServiceCollectionDescription>();
            ServiceCollectionServices = new HashSet<ServiceCollectionService>();
            //ServiceCollectionOrganization = new HashSet<Organization>();

            LanguageAvailabilities = new HashSet<ServiceCollectionLanguageAvailability>();
        }

        /// <summary>
        /// Indicates if the service collection can be used (referenced within services) by other users from other organizations.
        /// </summary>
        public bool IsVisibleForAll { get; set; }

        /// <summary>
        /// Responsible organization guid.
        /// </summary>
        public Guid OrganizationId { get; set; }
        /// <summary>
        /// Responsible organization.
        /// </summary>
        public Organization Organization { get; set; }

        public virtual ICollection<ServiceCollectionName> ServiceCollectionNames { get; set; }
        public virtual ICollection<ServiceCollectionDescription> ServiceCollectionDescriptions { get; set; }
        //public virtual ICollection<Organization> ServiceCollectionOrganizations { get; set; }
        public virtual ICollection<ServiceCollectionService> ServiceCollectionServices { get; set; }

        public PublishingStatusType PublishingStatus { get; set; }
        public Guid PublishingStatusId { get; set; }

        public ICollection<ServiceCollectionLanguageAvailability> LanguageAvailabilities { get; set; }

        public Guid? VersioningId { get; set; }
        public Versioning Versioning { get; set; }

        public ServiceCollection UnificRoot { get; set; }
        public Guid UnificRootId { get; set; }

        [NotMapped]
        bool IVersionedTrackedEntity.VersioningApplied { get; set; }
        IEnumerable<ILanguageAvailability> IMultilanguagedEntity.LanguageAvailabilitiesReference => LanguageAvailabilities;
    }
}
