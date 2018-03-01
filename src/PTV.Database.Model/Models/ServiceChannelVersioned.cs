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
using System.ComponentModel.DataAnnotations.Schema;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.Model.Models
{
    internal class ServiceChannelVersioned : ValidityBase, IPublishingStatus, IArchivable, IVersionedVolume<ServiceChannel>, IMultilanguagedEntity<ServiceChannelLanguageAvailability>, IBaseInformation, IRoleBased
    {
        //private IBaseInformation _baseInformationImplementation;

        public ServiceChannelVersioned()
        {
            ServiceChannelNames = new HashSet<ServiceChannelName>();
            ServiceChannelDescriptions = new HashSet<ServiceChannelDescription>();
            ServiceChannelServiceHours = new HashSet<ServiceChannelServiceHours>();
            WebPages = new HashSet<ServiceChannelWebPage>();
            Attachments = new HashSet<ServiceChannelAttachment>();
            Phones = new HashSet<ServiceChannelPhone>();
            Emails = new HashSet<ServiceChannelEmail>();
            Areas = new HashSet<ServiceChannelArea>();
            AreaMunicipalities = new HashSet<ServiceChannelAreaMunicipality>();

            TargetGroups = new HashSet<ServiceChannelTargetGroup>();
            OntologyTerms = new HashSet<ServiceChannelOntologyTerm>();
            Keywords = new HashSet<ServiceChannelKeyword>();
            ServiceClasses = new HashSet<ServiceChannelServiceClass>();
            Languages = new HashSet<ServiceChannelLanguage>();

            ElectronicChannels = new HashSet<ElectronicChannel>();
            WebpageChannels = new HashSet<WebpageChannel>();
            ServiceLocationChannels = new HashSet<ServiceLocationChannel>();
            PrintableFormChannels = new HashSet<PrintableFormChannel>();

            LanguageAvailabilities = new HashSet<ServiceChannelLanguageAvailability>();
        }

        public Guid TypeId { get; set; }

        public virtual ServiceChannelType Type { get; set; }

        public Guid? AreaInformationTypeId { get; set; }
        public virtual AreaInformationType AreaInformationType { get; set; }

        /// <summary>
        /// Responsible organization guid.
        /// </summary>
        public Guid OrganizationId { get; set; }
        /// <summary>
        /// Responsible organization.
        /// </summary>
        public Organization Organization { get; set; }

        /// <summary>
        /// Is the service channel chargeable
        /// </summary>
        public bool Charge { get; set; }

        // CHANNELS

        // electronic channel
        public virtual ICollection<ElectronicChannel> ElectronicChannels { get; set; }

        // webpage channel
        public virtual ICollection<WebpageChannel> WebpageChannels { get; set; }

        // service location channel
        public virtual ICollection<ServiceLocationChannel> ServiceLocationChannels { get; set; }

        // printable form channel
        public virtual ICollection<PrintableFormChannel> PrintableFormChannels { get; set; }

        public virtual ICollection<ServiceChannelServiceHours> ServiceChannelServiceHours { get; set; }

        public virtual ICollection<ServiceChannelTargetGroup> TargetGroups { get; set; }

        public virtual ICollection<ServiceChannelOntologyTerm> OntologyTerms { get; set; }

        public virtual ICollection<ServiceChannelKeyword> Keywords { get; set; }

        public virtual ICollection<ServiceChannelServiceClass> ServiceClasses { get; set; }

		/// <summary>
        /// The languages you can get service from the customer service persons (languages they can speak).
        /// </summary>
        public virtual ICollection<ServiceChannelLanguage> Languages { get; set; }

        public virtual ICollection<ServiceChannelWebPage> WebPages { get; set; }
        public virtual ICollection<ServiceChannelAttachment> Attachments { get; set; }

        public virtual ICollection<ServiceChannelPhone> Phones { get; set; }
        public virtual ICollection<ServiceChannelEmail> Emails { get; set; }

        public virtual ICollection<ServiceChannelName> ServiceChannelNames { get; set; }
        public virtual ICollection<ServiceChannelDescription> ServiceChannelDescriptions { get; set; }
        public virtual ICollection<ServiceChannelArea> Areas { get; set; }
        public virtual ICollection<ServiceChannelAreaMunicipality> AreaMunicipalities { get; set; }

        public PublishingStatusType PublishingStatus { get; set; }
        public Guid PublishingStatusId { get; set; }

        public ICollection<ServiceChannelLanguageAvailability> LanguageAvailabilities{ get; set; }
        public Guid? VersioningId { get; set; }
        public Versioning Versioning { get; set; }
        public Guid UnificRootId { get; set; }
        public ServiceChannel UnificRoot { get; set; }

        public Guid ConnectionTypeId { get; set; }

        public virtual ServiceChannelConnectionType ConnectionType { get; set; }

        [NotMapped]
        bool IVersionedTrackedEntity.VersioningApplied { get; set; }
        IEnumerable<IName> INameReferences.Names => ServiceChannelNames;
        IEnumerable<ILanguageAvailability> IMultilanguagedEntity.LanguageAvailabilitiesReference => LanguageAvailabilities;
    }
}
