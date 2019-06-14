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

namespace PTV.Domain.Model.Models.Import
{
    /// <summary>
    /// Base model for organization
    /// </summary>
    public class VmJsonOrganizationTypeRelation
    {
        /// <summary>
        /// Organization Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Type Id
        /// </summary>
        public Guid RestrictionTypeId { get; set; }
        /// <summary>
        /// Gets or sets the names.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        public List<VmJsonName> Names { get; set; }
    }
    
    /// <summary>
    /// Model for GD type restrictions
    /// </summary>
    public class VmJsonOrganizationTypeRestriction
    {
        /// <summary>
        /// Gets or sets the type code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public string Code { get; set; }
        /// <summary>
        /// Type of entity
        /// </summary>
        /// <value>
        /// Entity type.
        /// </value>
        public string EntityType { get; set; }
        /// <summary>
        /// Name of the Filter
        /// </summary>
        /// <value>
        /// FilterName.
        /// </value>
        public string FilterName { get; set; }
        /// <summary>
        /// Name of the type
        /// </summary>
        /// <value>
        /// Type Name.
        /// </value>
        public string TypeName { get; set; }
        /// <summary>
        /// Name of the restricted column of entity
        /// </summary>
        /// <value>
        /// Type Name.
        /// </value>
        public string EntityColumnName { get; set; }
        /// <summary>
        /// Type Id
        /// </summary>
        public Guid TypeValue { get; set; }
        /// <summary>
        /// Gets or sets the Organization.
        /// </summary>
        /// <value>
        /// The names.
        /// </value>
        public List<VmJsonOrganizationTypeRelation> Organizations { get; set; }
        
        
        /// <summary>
        /// Type of filter - allow/deny
        /// </summary>
        public string FilterType { get; set; }
    }
}