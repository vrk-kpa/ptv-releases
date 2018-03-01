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
    public class SourceTransactionFormEntity : BaseSourceEntity
    {
        public string Email { get; set; }

        public List<SourceLink> Links { get; set; }

        public string Phone { get; set; }

        public bool Charge { get; set; }

        [JsonProperty("keyword")]
        public List<string> Keywords { get; set; }

        [JsonProperty("language")]
        public List<string> Languages { get; set; }

        public List<SourceAttachment> Attachments { get; set; }

        public string Description { get; set; }

        [JsonProperty("targetGroup")]
        public List<string> TargetGroups { get; set; }

        public string FormReceiver { get; set; }

        [JsonProperty("ontologyTerm")]
        public List<string> OntologyTerms { get; set; }

        public int Organization { get; set; }

        [JsonProperty("alternateName")]
        public List<string> AlternateNames { get; set; }

        public string FormIdentifier { get; set; }

        public string ShortDescription { get; set; }

        public string ChargeDescription { get; set; }

        public string PhoneServiceChargeInformation { get; set; }

        public string DeliveryPostalCode { get; set; }

        public string DeliveryPostOfficeBox { get; set; }

        public string DeliveryStreetAddress { get; set; }

        public string DeliveryPostalDistrict { get; set; }

        public string DeliveryAddressDescription { get; set; }

        public string DeliveryStreetAddressNumber { get; set; }

    }
}
