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
using Newtonsoft.Json;

namespace PTV.Domain.Model.Models.Import
{
    /// <summary>
    /// General descriptions model for import from excel
    /// </summary>
    public class ImportStatutoryServiceGeneralDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportStatutoryServiceGeneralDescription"/> class.
        /// </summary>
        public ImportStatutoryServiceGeneralDescription()
        {
            UniqueLaws = new List<ImportLaw>();
        }

        /// <summary>
        /// Id of general description, if available
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or sets the reference code.
        /// </summary>
        /// <value>
        /// The reference code.
        /// </value>
        public string ReferenceCode { get; set; }
        /// <summary>
        /// Gets or sets the type of the service.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        public string ServiceType { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public List<JsonLanguageLabel> Name { get; set; }
        /// <summary>
        /// Gets or sets the short description.
        /// </summary>
        /// <value>
        /// The short description.
        /// </value>
        public List<JsonLanguageLabel> ShortDescription { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public List<JsonLanguageLabel> Description { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public List<JsonLanguageLabel> BackgroundDescription { get; set; }
        /// <summary>
        /// Gets or sets the laws.
        /// </summary>
        /// <value>
        /// The laws.
        /// </value>
        public List<ImportLaw> Laws { get; set; }
        /// <summary>
        /// Gets or sets the unique laws.
        /// </summary>
        /// <value>
        /// The unique laws.
        /// </value>
        [JsonIgnore]
        public List<ImportLaw> UniqueLaws { get; set; }
        /// <summary>
        /// Gets or sets the service restrictions.
        /// </summary>
        /// <value>
        /// The service restrictions.
        /// </value>
        public List<JsonLanguageLabel> ServiceRestrictions { get; set; }
        /// <summary>
        /// Gets or sets the user instructions.
        /// </summary>
        /// <value>
        /// The user instructions.
        /// </value>
        public List<JsonLanguageLabel> UserInstructions { get; set; }
        /// <summary>
        /// Gets or sets the charge type additional information.
        /// </summary>
        /// <value>
        /// The charge type additional information.
        /// </value>
        public List<JsonLanguageLabel> ChargeTypeAdditionalInfo { get; set; }
        /// <summary>
        /// Gets or sets the dead line additional information.
        /// </summary>
        /// <value>
        /// The dead line additional information.
        /// </value>
        public List<JsonLanguageLabel> DeadLineAdditionalInfo { get; set; }
        /// <summary>
        /// Gets or sets the processing time additional information.
        /// </summary>
        /// <value>
        /// The processing time additional information.
        /// </value>
        public List<JsonLanguageLabel> ProcessingTimeAdditionalInfo { get; set; }
        /// <summary>
        /// Gets or sets the validity time additional information.
        /// </summary>
        /// <value>
        /// The validity time additional information.
        /// </value>
        public List<JsonLanguageLabel> ValidityTimeAdditionalInfo { get; set; }

        /// <summary>
        /// Gets or sets the type of the charge.
        /// </summary>
        /// <value>
        /// The type of the charge.
        /// </value>
        public string ChargeType { get; set; }
        /// <summary>
        /// Gets or sets the service classes.
        /// </summary>
        /// <value>
        /// The service classes.
        /// </value>
        [JsonProperty("ServiceClass")]
        public List<ImportFintoItem> ServiceClasses { get; set; } = new List<ImportFintoItem>();
        /// <summary>
        /// Gets or sets the ontology terms.
        /// </summary>
        /// <value>
        /// The ontology terms.
        /// </value>
        [JsonProperty("OntologyTerm")]
        public List<ImportFintoItem> OntologyTerms { get; set; } = new List<ImportFintoItem>();
        /// <summary>
        /// Gets or sets the target groups.
        /// </summary>
        /// <value>
        /// The target groups.
        /// </value>
        [JsonProperty("TargetGroup")]
        public List<ImportFintoItem> TargetGroups { get; set; } = new List<ImportFintoItem>();
        /// <summary>
        /// Gets or sets the life events.
        /// </summary>
        /// <value>
        /// The life events.
        /// </value>
        [JsonProperty("LifeEvent")]
        public List<ImportFintoItem> LifeEvents { get; set; } = new List<ImportFintoItem>();
        /// <summary>
        /// Gets or sets the industrial classes.
        /// </summary>
        /// <value>
        /// The industrial classes.
        /// </value>
        [JsonProperty("IndustrialClass")]
        public List<ImportFintoItem> IndustrialClasses { get; set; } = new List<ImportFintoItem>();
        /// <summary>
        /// Type id
        /// </summary>
        public Guid TypeId { get; set; }
    }
}
