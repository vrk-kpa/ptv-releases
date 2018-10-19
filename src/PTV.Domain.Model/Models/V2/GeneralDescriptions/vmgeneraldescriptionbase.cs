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
using System.Text;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Service;

namespace PTV.Domain.Model.Models.V2.GeneralDescriptions
{
    /// <summary>
    /// ViewModel for basic general desion information
    /// </summary>
    public class VmGeneralDescriptionBase : VmGeneralDescriptionHeader
    {
        /// <summary>
        /// Service type.
        /// </summary>
        public Guid? ServiceType { get; set; }
        /// <summary>
        /// General Description type.
        /// </summary>
        public Guid? GeneralDescriptionType { get; set; }
        /// <summary>
        /// Gets or sets the charge type.
        /// </summary>
        /// <value>
        /// The charge type.
        /// </value>
        public VmChargeType ChargeType { get; set; }
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
        /// Gets or sets the background description.
        /// </summary>
        /// <value>
        /// The background description.
        /// </value>
        public Dictionary<string, string> BackgroundDescription { get; set; }
        /// <summary>
        /// Gets or sets the dead line additional info.
        /// </summary>
        /// <value>
        /// The dead line additional info.
        /// </value>
        public Dictionary<string, string> DeadLineInformation { get; set; }
        /// <summary>
        /// Gets or sets the processing time additional info.
        /// </summary>
        /// <value>
        /// The processing time additional info.
        /// </value>
        public Dictionary<string, string> ProcessingTimeInformation { get; set; }
        /// <summary>
        /// Gets or sets the user instruction.
        /// </summary>
        /// <value>
        /// The user instruction.
        /// </value>
        public Dictionary<string, string> UserInstruction { get; set; }
        /// <summary>
        /// Gets or sets the validity time additional info.
        /// </summary>
        /// <value>
        /// The validity time additional info.
        /// </value>
        public Dictionary<string, string> ValidityTimeInformation { get; set; }
        /// <summary>
        /// Gets or sets the condition of service usage.
        /// </summary>
        /// <value>
        /// The condition of service usage.
        /// </value>
        public Dictionary<string, string> ConditionOfServiceUsage { get; set; }
        /// <summary>
        /// Gets or sets the charge type additional information.
        /// </summary>
        /// <value>
        /// The charge type additional information.
        /// </value>
        public Dictionary<string, string> GeneralDescriptionTypeAdditionalInformation { get; set; }
        /// <summary>
        /// Gets or sets the target groups.
        /// </summary>
        /// <value>
        /// The target groups.
        /// </value>
        public List<Guid> TargetGroups { get; set; }
        /// <summary>
        /// Gets or sets the service laws.
        /// </summary>
        /// <value>
        /// The service laws.
        /// </value>
        public List<VmLaw> Laws { get; set; }
    }
}
