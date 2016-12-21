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

using System.Collections.Generic;
using Newtonsoft.Json;

namespace PTV.Domain.Model.Models.Import
{
    public class ImportStatutoryServiceGeneralDescription
    {
        public ImportStatutoryServiceGeneralDescription()
        {
            UniqueLaws = new List<ImportLawGroup>();
        }

        public string ReferenceCode { get; set; }
        public string ServiceType { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public List<ImportLawGroup> Laws { get; set; }
        [JsonIgnore]
        public List<ImportLawGroup> UniqueLaws { get; set; }
        public string ServiceRestrictions { get; set; }
        public string UserInstructions { get; set; }
        public string ChargeTypeAdditionalInfo { get; set; }
        public string TasksAdditionalInfo { get; set; }
        public string DeadLineAdditionalInfo { get; set; }
        public string ProcessingTimeAdditionalInfo { get; set; }
        public string ValidityTimeAdditionalInfo { get; set; }

        public string ChargeType { get; set; }
        [JsonProperty("ServiceClass")]
        public List<ImportFintoItem> ServiceClasses { get; set; } = new List<ImportFintoItem>();
        [JsonProperty("OntologyTerm")]
        public List<ImportFintoItem> OntologyTerms { get; set; } = new List<ImportFintoItem>();
        [JsonProperty("TargetGroup")]
        public List<ImportFintoItem> TargetGroups { get; set; } = new List<ImportFintoItem>();
        [JsonProperty("LifeEvent")]
        public List<ImportFintoItem> LifeEvents { get; set; } = new List<ImportFintoItem>();
        [JsonProperty("IndustrialClass")]
        public List<ImportFintoItem> IndustrialClasses { get; set; } = new List<ImportFintoItem>();
    }
}
