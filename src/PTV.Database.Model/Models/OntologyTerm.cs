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

using System.Collections.Generic;
using System.Linq;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.Model.Models
{
    internal class OntologyTerm : FintoItemBase, IFintoItemNames<OntologyTermName>, INameReferences
    {
        public OntologyTerm()
        {
            ServiceOntologyTerms = new HashSet<ServiceOntologyTerm>();
            StatutoryServiceOntologyTerms = new HashSet<StatutoryServiceOntologyTerm>();
            ServiceChannelOntologyTerms = new HashSet<ServiceChannelOntologyTerm>();
            OntologyTermExactMatches = new HashSet<OntologyTermExactMatch>();

            Names = new HashSet<OntologyTermName>();
            Children = new HashSet<OntologyTermParent>();
            Parents = new HashSet<OntologyTermParent>();
        }

        public virtual ICollection<ServiceOntologyTerm> ServiceOntologyTerms { get; set; }
        public virtual ICollection<StatutoryServiceOntologyTerm> StatutoryServiceOntologyTerms { get; set; }

        public virtual ICollection<ServiceChannelOntologyTerm> ServiceChannelOntologyTerms { get; set; }

        public virtual ICollection<OntologyTermName> Names { get; set; }

        public virtual ICollection<OntologyTermExactMatch> OntologyTermExactMatches { get; set; }

        public virtual ICollection<OntologyTermParent> Parents { get; set; }
        public virtual ICollection<OntologyTermParent> Children { get; set; }

        IEnumerable<IName> INameReferences.Names => Names;

    }
    //    internal partial class OntologyTerm : FintoItemBase<OntologyTermName>, IFintoItemChildren
    //    {
    //        public OntologyTerm()
    //        {
    //            ServiceOntologyTerms = new HashSet<ServiceOntologyTerm>();
    //        }

    //        public virtual ICollection<ServiceOntologyTerm> ServiceOntologyTerms { get; set; }
    //        public virtual ICollection<OntologyTermParent> Parents { get; set; }
    //        public virtual ICollection<OntologyTermParent> Children { get; set; }

    //        IEnumerable<IFintoItemChildren> IFintoItemChildren.Children
    //        {
    //            get { return Children.Select(x => x.Child); }
    //        }
    //    }
}
