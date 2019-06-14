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
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Organization;

namespace PTV.Domain.Model.Models.V2.Common
{
    /// <summary>
    /// VmEntityOperation
    /// </summary>
    public class VmEntityOperation : ICopyTemplate
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long Created { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyList<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof (StringEnumConverter))]
        public PublishingStatus PublishingStatus { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int VersionMajor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int VersionMinor { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonConverter(typeof (StringEnumConverter))]
        public HistoryAction HistoryAction { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<VmEntityOperation> SubOperations { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long? ActionDate { get; set; }

        /// <inheritdoc />
        public Guid? TemplateId { get; set; }

        /// <inheritdoc />
        [JsonIgnore]
        public Guid? TemplateOrganizationId { get; set; }

        /// <summary>
        /// Organization to which belongs the source template entity.
        /// </summary>
        public VmOrganizationHeader TemplateOrganization { get; set; }
        
        /// <summary>
        /// Id of the source language if history action is connected to translation orders.
        /// </summary>
        public Guid? SourceLanguageId { get; set; }
        
        /// <summary>
        /// Ids of the target languages if history action is connected to translation orders.
        /// </summary>
        public List<string> TargetLanguageIds { get; set; }
    }
}
