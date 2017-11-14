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
using System.Runtime;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Attributes;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.Model.Models
{
    internal partial class ServiceVersioned : ValidityBase,
        IPublishingStatus,
        IVersionedVolume<Service>,
        IMultilanguagedEntity<ServiceLanguageAvailability>,
        IArchivable,
        IBaseInformation,
        IRoleBased,
        IChargeType
    {
        public ServiceVersioned()
        {
            ServiceKeywords = new HashSet<ServiceKeyword>();
            ServiceLanguages = new HashSet<ServiceLanguage>();
            ServiceNames = new HashSet<ServiceName>();
            ServiceDescriptions = new HashSet<ServiceDescription>();
            ServiceLifeEvents = new HashSet<ServiceLifeEvent>();
            ServiceOntologyTerms = new HashSet<ServiceOntologyTerm>();
            ServiceServiceClasses = new HashSet<ServiceServiceClass>();
            ServiceIndustrialClasses = new HashSet<ServiceIndustrialClass>();
            ServiceTargetGroups = new HashSet<ServiceTargetGroup>();            
            ServiceLaws = new HashSet<ServiceLaw>();
            ServiceElectronicNotificationChannels = new HashSet<ServiceElectronicNotificationChannel>();
            ServiceElectronicCommunicationChannels = new HashSet<ServiceElectronicCommunicationChannel>();
            ServiceRequirements = new HashSet<ServiceRequirement>();
            OrganizationServices = new HashSet<OrganizationService>();
            LanguageAvailabilities = new HashSet<ServiceLanguageAvailability>();
            Areas = new HashSet<ServiceArea>();
            AreaMunicipalities = new HashSet<ServiceAreaMunicipality>();
            ServiceWebPages = new HashSet<ServiceWebPage>();
            ServiceProducers = new HashSet<ServiceProducer>();
        }

        public bool ElectronicNotification { get; set; }

        public bool ElectronicCommunication { get; set; }

        public Guid? ChargeTypeId { get; set; }
        public ServiceChargeType ChargeType { get; set; }

        public Guid? StatutoryServiceGeneralDescriptionId { get; set; }
        public Guid? TypeId { get; set; }
        public Guid AreaInformationTypeId { get; set; }
        /// <summary>
        /// Responsible organization guid.
        /// </summary>
        public Guid OrganizationId { get; set; }
        /// <summary>
        /// Responsible organization.
        /// </summary>
        public Organization Organization { get; set; }
        public virtual ServiceType Type { get; set; }
        public virtual AreaInformationType AreaInformationType { get; set; }
        public virtual StatutoryServiceGeneralDescription StatutoryServiceGeneralDescription { get; set; }
        public virtual ICollection<ServiceName> ServiceNames { get; set; }
        public virtual ICollection<ServiceDescription> ServiceDescriptions { get; set; }
        public virtual ICollection<ServiceKeyword> ServiceKeywords { get; set; }
        public virtual ICollection<ServiceLanguage> ServiceLanguages { get; set; }
        public virtual ICollection<ServiceLifeEvent> ServiceLifeEvents { get; set; }
        public virtual ICollection<ServiceOntologyTerm> ServiceOntologyTerms { get; set; }
        public ICollection<OrganizationService> OrganizationServices { get; set; }
        public virtual ICollection<ServiceServiceClass> ServiceServiceClasses { get; set; }
        public virtual ICollection<ServiceIndustrialClass> ServiceIndustrialClasses { get; set; }
        public virtual ICollection<ServiceTargetGroup> ServiceTargetGroups { get; set; }
        public virtual ICollection<ServiceRequirement> ServiceRequirements { get; set; }
        public virtual ICollection<ServiceLaw> ServiceLaws { get; set; }
        public virtual ICollection<ServiceElectronicNotificationChannel> ServiceElectronicNotificationChannels { get; set; }
        public virtual ICollection<ServiceElectronicCommunicationChannel> ServiceElectronicCommunicationChannels { get; set; }
        public virtual ICollection<ServiceArea> Areas { get; set; }
        public virtual ICollection<ServiceAreaMunicipality> AreaMunicipalities { get; set; }
        public virtual ICollection<ServiceWebPage> ServiceWebPages { get; set; }
        public PublishingStatusType PublishingStatus { get; set; }
        public Guid PublishingStatusId { get; set; }
        public Guid? VersioningId { get; set; }
        public virtual Versioning Versioning { get; set; }
        public virtual ICollection<ServiceLanguageAvailability> LanguageAvailabilities { get; set; }
        public Guid UnificRootId { get; set; }
        public Service UnificRoot { get; set; }

        public Guid? FundingTypeId { get; set; }
        public virtual ServiceFundingType FundingType { get; set; }
        public virtual ICollection<ServiceProducer> ServiceProducers { get; set; }
        [NotMapped]
        bool IVersionedTrackedEntity.VersioningApplied { get; set; }
        IEnumerable<IName> INameReferences.Names => ServiceNames;
        IEnumerable<IDescription> IDescriptionReferences.Descriptions => ServiceDescriptions;
        IEnumerable<ILanguageAvailability> IMultilanguagedEntity.LanguageAvailabilitiesReference => LanguageAvailabilities;
    }
}
