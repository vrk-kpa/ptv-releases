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
using System.ComponentModel.DataAnnotations.Schema;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Database.Model.Models.Privileges;

namespace PTV.Database.Model.Models
{
    internal partial class StatutoryServiceGeneralDescriptionVersioned : ValidityBase,
        IVersionedVolume<StatutoryServiceGeneralDescription>,
        IMultilanguagedEntity<GeneralDescriptionLanguageAvailability>,
        IBaseInformation,
        IRoleBased,
        ILockable,
        IChargeType
    {
        public StatutoryServiceGeneralDescriptionVersioned()
        {
            ServiceClasses = new HashSet<StatutoryServiceServiceClass>();
            TargetGroups = new HashSet<StatutoryServiceTargetGroup>();
            Names = new HashSet<StatutoryServiceName>();
            Descriptions = new HashSet<StatutoryServiceDescription>();
            Languages = new HashSet<StatutoryServiceLanguage>();
            OntologyTerms = new HashSet<StatutoryServiceOntologyTerm>();
            LifeEvents = new HashSet<StatutoryServiceLifeEvent>();
            IndustrialClasses = new HashSet<StatutoryServiceIndustrialClass>();
            StatutoryServiceRequirements = new HashSet<StatutoryServiceRequirement>();
            StatutoryServiceLaws = new HashSet<StatutoryServiceLaw>();
            LanguageAvailabilities = new HashSet<GeneralDescriptionLanguageAvailability>();
        }

        /// <summary>
        /// Used only for updates during imports
        /// </summary>
        public string ReferenceCode { get; set; }

        public virtual ICollection<StatutoryServiceServiceClass> ServiceClasses { get; set; }
        public virtual ICollection<StatutoryServiceTargetGroup> TargetGroups { get; set; }
        public virtual ICollection<StatutoryServiceIndustrialClass> IndustrialClasses { get; set; }
        public virtual ICollection<StatutoryServiceName> Names { get; set; }
        public Guid TypeId { get; set; }
        public virtual ServiceType Type { get; set; }
        public Guid? ChargeTypeId { get; set; }
        public ServiceChargeType ChargeType { get; set; }
        public Guid GeneralDescriptionTypeId { get; set; }
        public virtual GeneralDescriptionType GeneralDescriptionType { get; set; }
        public virtual ICollection<StatutoryServiceDescription> Descriptions { get; set; }
        public virtual ICollection<StatutoryServiceLanguage> Languages { get; set; }
        public virtual ICollection<StatutoryServiceOntologyTerm> OntologyTerms { get; set; }
        public virtual ICollection<StatutoryServiceLifeEvent> LifeEvents { get; set; }
        public virtual ICollection<StatutoryServiceRequirement> StatutoryServiceRequirements { get; set; }
        public virtual ICollection<StatutoryServiceLaw> StatutoryServiceLaws { get; set; }

        public virtual ICollection<GeneralDescriptionLanguageAvailability> LanguageAvailabilities { get; set; }
        public PublishingStatusType PublishingStatus { get; set; }
        public Guid PublishingStatusId { get; set; }
        public Guid? VersioningId { get; set; }
        public Versioning Versioning { get; set; }
        public Guid UnificRootId { get; set; }
        public StatutoryServiceGeneralDescription UnificRoot { get; set; }
        [NotMapped]
        bool IVersionedTrackedEntity.VersioningApplied { get; set; }
        IEnumerable<IName> INameReferences.Names => Names;
        IEnumerable<IDescription> IDescriptionReferences.Descriptions => Descriptions;
        IEnumerable<ILanguageAvailability> IMultilanguagedEntity.LanguageAvailabilitiesReference => LanguageAvailabilities;
    }
}
