/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using System.ComponentModel.DataAnnotations;

namespace PTV.Domain.Model.Models.Security
{
    /// <summary>
    ///
    /// </summary>
    public class VmRestrictedType
    {
        /// <summary>
        ///
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Guid Value { get; set; }

        /// <summary>
        ///
        /// </summary>
        public List<VmRestrictionFilter> Filters { get; set; }
    }


    /// <summary>
    ///
    /// </summary>
    public class VmRestrictionFilter
    {
        /// <summary>
        ///
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string FilterName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public VmRestrictedType RestrictedType { get; set; }

        /// <summary>
        /// List of attached organizations
        /// </summary>
        public List<VmOrganizationFilter> OrganizationFilters { get; set; }

        /// <summary>
        /// Type of filter, is it allowed for or blocked for
        /// </summary>
        public EVmRestrictionFilterType FilterType { get; set; }

        /// <summary>
        /// Filter is related to specific type only and other values of this type should be blocked
        /// </summary>
        public bool BlockOtherTypes { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    public class VmOrganizationFilter
    {
        /// <summary>
        ///
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        ///
        /// </summary>
        public VmRestrictionFilter Filter { get; set; }
    }
}
