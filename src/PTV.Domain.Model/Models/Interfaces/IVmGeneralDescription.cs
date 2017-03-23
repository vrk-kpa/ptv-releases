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

namespace PTV.Domain.Model.Models.Interfaces
{
    /// <summary>
    /// View model interface of general description
    /// </summary>
    interface IVmGeneralDescription
    {
        /// <summary>
        /// Gets or sets the service user instruction.
        /// </summary>
        /// <value>
        /// The service user instruction.
        /// </value>
        Dictionary<string, string> ServiceUserInstruction { get; set; }
        /// <summary>
        /// Gets or sets the charge type identifier.
        /// </summary>
        /// <value>
        /// The charge type identifier.
        /// </value>
        Guid? ChargeTypeId { get; set; }
        /// <summary>
        /// Gets or sets the type of the charge.
        /// </summary>
        /// <value>
        /// The type of the charge.
        /// </value>
        string ChargeType { get; set; }
        /// <summary>
        /// Gets or sets the type identifier.
        /// </summary>
        /// <value>
        /// The type identifier.
        /// </value>
        Guid TypeId { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        string Type { get; set; }
        /// <summary>
        /// Gets or sets the charge type additional information.
        /// </summary>
        /// <value>
        /// The charge type additional information.
        /// </value>
        Dictionary<string, string> ChargeTypeAdditionalInfo { get; set; }
        /// <summary>
        /// Gets or sets the condition of service usage.
        /// </summary>
        /// <value>
        /// The condition of service usage.
        /// </value>
        Dictionary<string, string> ConditionOfServiceUsage { get; set; }
        /// <summary>
        /// Gets or sets the dead line additional information.
        /// </summary>
        /// <value>
        /// The dead line additional information.
        /// </value>
        Dictionary<string, string> DeadLineAdditionalInfo { get; set; }
        /// <summary>
        /// Gets or sets the processing time additional information.
        /// </summary>
        /// <value>
        /// The processing time additional information.
        /// </value>
        Dictionary<string, string> ProcessingTimeAdditionalInfo { get; set; }
        /// <summary>
        /// Gets or sets the validity time additional information.
        /// </summary>
        /// <value>
        /// The validity time additional information.
        /// </value>
        Dictionary<string, string> ValidityTimeAdditionalInfo { get; set; }
        /// <summary>
        /// Gets or sets the service classes.
        /// </summary>
        /// <value>
        /// The service classes.
        /// </value>
        List<Guid> ServiceClasses { get; set; }
        /// <summary>
        /// Gets or sets the target groups.
        /// </summary>
        /// <value>
        /// The target groups.
        /// </value>
        List<Guid> TargetGroups { get; set; }
        /// <summary>
        /// Gets or sets the ontology terms.
        /// </summary>
        /// <value>
        /// The ontology terms.
        /// </value>
        List<VmExpandedVmTreeItem> OntologyTerms { get; set; }
        /// <summary>
        /// Gets or sets the industrial classes.
        /// </summary>
        /// <value>
        /// The industrial classes.
        /// </value>
        List<VmTreeItem> IndustrialClasses { get; set; }
        /// <summary>
        /// Gets or sets the life events.
        /// </summary>
        /// <value>
        /// The life events.
        /// </value>
        List<VmTreeItem> LifeEvents { get; set; }
    }
}
