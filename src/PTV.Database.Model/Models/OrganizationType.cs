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
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationType.cs" company="Tieto">
//   Tieto
// </copyright>
// <summary>
//   The organization type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using PTV.Database.Model.Models.Base;
using System;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.Model.Models
{
    /// <summary>
    /// The organization type.
    /// </summary>
    internal partial class OrganizationType : FintoItemBase<OrganizationType>, IFintoItemNames<OrganizationTypeName>, ITypeNamed<OrganizationTypeName>, INameReferences
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrganizationType"/> class.
        /// </summary>
        public OrganizationType()
        {
            Organization = new HashSet<OrganizationVersioned>();
            Names = new HashSet<OrganizationTypeName>();
        }
        /// <summary>
        /// Gets or sets the suborganization.
        /// </summary>
        public virtual ICollection<OrganizationVersioned> Organization { get; set; }
        /// <summary>
        /// Gets or sets the localizations of type name.
        /// </summary>
        public virtual ICollection<OrganizationTypeName> Names { get; set; }
        IEnumerable<IName> INameReferences.Names => Names;
    }
}
