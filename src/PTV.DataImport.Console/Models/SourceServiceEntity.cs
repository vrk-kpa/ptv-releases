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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.DataImport.ConsoleApp.Models
{
    public class SourceServiceEntity : BaseSourceEntity
    {
        [JsonProperty("keyword")]
        public List<string> Keywords { get; set; }

        [JsonProperty("webPage")]
        public List<SourceWebpage> Webpages { get; set; }

        [JsonProperty("language")]
        public List<string> Languages { get; set; }

        public string CreatedBy { get; set; }

        [JsonProperty("lifeEvent")]
        public List<string> LifeEvents { get; set; }

        public string Description { get; set; }

        [JsonProperty("targetGroup")]
        public List<string> TargetGroups { get; set; }

        [JsonProperty("municipality")]
        public List<string> Municipalities { get; set; }

        [JsonProperty("ontologyTerm")]
        public List<string> OntologyTerms { get; set; }

        public string Requirements { get; set; }

        [JsonProperty("serviceClass")]
        public List<string> ServiceClasses { get; set; }

        [JsonProperty("alternateName")]
        public List<string> AlternateNames { get; set; }

        public string ServiceCoverage { get; set; }

        public string ShortDescription { get; set; }

        public int? GeneralDescription { get; set; }

        public SourceServiceToOrganization ServiceToOrganization { get; set; }

        public bool? ElectronicNotification { get; set; }

        public bool? ElectronicCommunication { get; set; }

        public SourceServiceToServiceChannel ServiceToServiceChannel { get; set; }

        public string ServiceUserInstructions { get; set; }

        [JsonProperty("serviceChargeDescription")]
        public List<string> ServiceChargeDescriptions { get; set; }

        public string ServiceCharge { get; set; }

        public string ElectronicNotificationChannel { get; set; }

        public string ElectronicCommunicationChannel { get; set; }

        public string ServiceType { get; set; } // some weird property that has been added to only one source entity!
    }
}
