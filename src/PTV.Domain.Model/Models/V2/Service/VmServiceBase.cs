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

namespace PTV.Domain.Model.Models.V2.Service
{
    /// <summary>
    /// ViewModel for basic service information
    /// </summary>
    public class VmServiceBase : VmServiceHeader
    {
        /// <summary>
        /// Gets or sets the general description id.
        /// </summary>
        /// <value>
        /// The general description id.
        /// </value>
        public Guid? GeneralDescriptionId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public Dictionary<string, string> AlternateName { get; set; }

        /// <summary>
        /// Gets or sets the short description.
        /// </summary>
        /// <value>
        /// The short description.
        /// </value>
        public Dictionary<string, string> ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public Dictionary<string, string> Description { get; set; }
        /// <summary>
        /// Gets or sets the additional information dead line.
        /// </summary>
        /// <value>
        /// The additional information dead line.
        /// </value>
        public Dictionary<string, string> DeadLineInformation { get; set; }
        /// <summary>
        /// Gets or sets the additional information processing time.
        /// </summary>
        /// <value>
        /// The additional information processing time.
        /// </value>
        public Dictionary<string, string> ProcessingTimeInformation { get; set; }
        /// <summary>
        /// Gets or sets the additional information validity time.
        /// </summary>
        /// <value>
        /// The additional information validity time.
        /// </value>
        public Dictionary<string, string> ValidityTimeInformation { get; set; }
        /// <summary>
        /// Gets or sets the funding type id.
        /// </summary>
        /// <value>
        /// The funding type id.
        /// </value>
        public Guid? FundingType { get; set; }
        /// <summary>
        /// Gets or sets the service type id.
        /// </summary>
        /// <value>
        /// The service type id.
        /// </value>
        public Guid? ServiceType { get; set; }
        /// <summary>
        /// Gets or sets the organization id.
        /// </summary>
        /// <value>
        /// The organization id.
        /// </value>
        public Guid? Organization { get; set; }
        /// <summary>
        /// Gets or sets the responsible organizations.
        /// </summary>
        /// <value>
        /// The responsible organizations.
        /// </value>
        public List<Guid> ResponsibleOrganizations { get; set; }
        /// <summary>
        /// Gets or sets the conditions and criteria.
        /// </summary>
        /// <value>
        /// The conditions and criteria.
        /// </value>
        public Dictionary<string, string> ConditionOfServiceUsage { get; set; }
        /// <summary>
        /// Gets or sets the user instruction (operating procedure).
        /// </summary>
        /// <value>
        /// The user instruction.
        /// </value>
        public Dictionary<string, string> UserInstruction { get; set; }
        /// <summary>
        /// Gets or sets the charge type.
        /// </summary>
        /// <value>
        /// The charge type.
        /// </value>
        public VmChargeType ChargeType{ get; set; }
        /// <summary>
        /// Gets or sets the area information.
        /// </summary>
        /// <value>
        /// The area information.
        /// </value>
        public VmAreaInformation AreaInformation { get; set; }
        /// <summary>
        /// Gets or sets the languages.
        /// </summary>
        /// <value>
        /// The languages.
        /// </value>
        public List<Guid> Languages{ get; set; }
        /// <summary>
        /// Gets or sets the target groups.
        /// </summary>
        /// <value>
        /// The target groups.
        /// </value>
        public List<Guid> TargetGroups { get; set; }
        /// <summary>
        /// Gets or sets the overrided target groups.
        /// </summary>
        /// <value>
        /// The overrided target groups.
        /// </value>
        public List<Guid> OverrideTargetGroups { get; set; }
        /// <summary>
        /// Gets or sets the service producers.
        /// </summary>
        /// <value>
        /// The service producers.
        /// </value>
        public List<VmServiceProducer> ServiceProducers { get; set; }
        /// <summary>
        /// Gets or sets the service voucher in use.
        /// </summary>
        /// <value>
        /// The sevrice voucher in use.
        /// </value>
        public bool ServiceVoucherInUse { get; set; }
        /// <summary>
        /// Gets or sets the service vouchers.
        /// </summary>
        /// <value>
        /// The service vouchers.
        /// </value>
        public Dictionary<string, List<VmServiceVoucher>> ServiceVouchers { get; set; }
        /// <summary>
        /// Gets or sets the service laws.
        /// </summary>
        /// <value>
        /// The service laws.
        /// </value>
        public List<VmLaw> Laws { get; set; }
        /// <summary>
        /// Gets or sets the general description service type id.
        /// </summary>
        /// <value>
        /// The general description service type id.
        /// </value>
        [JsonIgnore]
        public Guid? GeneralDescriptionServiceTypeId { get; set; }
    }
}