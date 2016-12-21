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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Converters;
using PTV.Framework.ServiceManager;

namespace PTV.Domain.Model.Models
{
    public class VmServiceStep1 : VmEnumEntityStatusBase, IVmServiceStep1
    {
        [JsonConverter(typeof(ServiceGeneralDescriptionConverter))]
        public VmServiceGeneralDescription GeneralDescription { get; set; }
        public Guid? ChargeType { get; set; }

        public Guid ServiceTypeId { get; set; }
        public string ServiceName { get; set; }
        public string AlternateServiceName { get; set; }
        public string ShortDescriptions { get; set; }
        public string Description { get; set; }
        public string UserInstruction { get; set; }
        public string ServiceUsage { get; set; }
        public string AdditionalInformation { get; set; }
        public string AdditionalInformationTasks { get; set; }
        public string AdditionalInformationDeadLine { get; set; }
        public string AdditionalInformationProcessingTime { get; set; }
        public string AdditionalInformationValidityTime { get; set; }
		public List<Guid> Languages { get; set; }
        public LanguageCode Language { get; set; }
        public List<Guid> Municipalities { get; set; }
        public Guid ServiceCoverageTypeId { get; set; }
        public VmServiceStep1()
        {
            ServiceName = string.Empty;
            AlternateServiceName = string.Empty;
            ShortDescriptions = string.Empty;
            Description = string.Empty;
            UserInstruction = string.Empty;
            ServiceUsage = string.Empty;
            AdditionalInformation = string.Empty;
            AdditionalInformationTasks = string.Empty;
            AdditionalInformationDeadLine = string.Empty;
            AdditionalInformationProcessingTime = string.Empty;
            AdditionalInformationValidityTime = string.Empty;
            Languages = new List<Guid>();
            Municipalities = new List<Guid>();
        }
    }
}
