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

namespace PTV.Database.Model.Models
{
    internal partial class StatutoryServiceGeneralDescription : EntityIdentifierBase
    {
        public StatutoryServiceGeneralDescription()
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
        public virtual ICollection<StatutoryServiceDescription> Descriptions { get; set; }
        public virtual ICollection<StatutoryServiceLanguage> Languages { get; set; }
        public virtual ICollection<StatutoryServiceOntologyTerm> OntologyTerms { get; set; }
        public virtual ICollection<StatutoryServiceLifeEvent> LifeEvents { get; set; }
        public virtual ICollection<StatutoryServiceRequirement> StatutoryServiceRequirements { get; set; }
        public virtual ICollection<StatutoryServiceLaw> StatutoryServiceLaws { get; set; }
    }
}
