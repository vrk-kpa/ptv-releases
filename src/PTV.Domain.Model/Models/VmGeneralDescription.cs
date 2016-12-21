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
using System.Linq;
using System.Threading.Tasks;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Domain.Model.Models
{
    public class VmGeneralDescription : VmEnumEntityBase, IVmGeneralDescription
    {
        public string Name { get; set; }
        public string AlternateName { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string ServiceUserInstruction { get; set; }
        public Guid? ChargeTypeId { get; set; }
        public string ChargeType { get; set; }
        public Guid TypeId { get; set; }
        public string Type { get; set; }
        public string ChargeTypeAdditionalInfo { get; set; }
        public string ConditionOfServiceUsage { get; set; }
        public string DeadLineAdditionalInfo { get; set; }
        public string ProcessingTimeAdditionalInfo { get; set; }
        public string ValidityTimeAdditionalInfo { get; set; }
        public List<Guid> ServiceClasses { get; set; }
        public List<Guid> TargetGroups { get; set; }
        public List<VmTreeItem> IndustrialClasses { get; set; }
        public List<VmTreeItem> LifeEvents { get; set; }
        public List<VmExpandedVmTreeItem> OntologyTerms { get; set; }
    }
	
    public class VmServiceGeneralDescription : VmEntityBase
    {
        public string Name { get; set; }
//        public string AlternateName { get; set; }
//        public string ShortDescription { get; set; }
        public string Description { get; set; }
//        public string ServiceUserInstruction { get; set; }
        public List<VmListItem> ServiceClasses { get; set; }
        public List<Guid> TargetGroups { get; set; }
        public List<VmListItem> IndustrialClasses { get; set; }
        public List<VmListItem> LifeEvents { get; set; }
        public List<VmListItem> OntologyTerms { get; set; }
    }
}
