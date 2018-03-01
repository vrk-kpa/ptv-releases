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
using PTV.Database.Model.Interfaces;

namespace PTV.Database.Model.Models.Base
{
    internal abstract class FintoItemBase : EntityIdentifierBase, IFintoItemBase, IIsValid
    {
        public string Code { get; set; }
        public string Label { get; set; }
        public int? OrderNumber { get; set; }
        public string Uri { get; set; }
        public string ParentUri { get; set; }
        public string OntologyType { get; set; }
        public bool IsValid { get; set; }
    }

    internal abstract class FintoItemBase<TParent> : FintoItemBase, IHierarchy<TParent>, IFintoItem, IFintoItemChildren
        where TParent : FintoItemBase<TParent> //where TName : NameBase
    {
        public FintoItemBase()
        {
            Children = new List<TParent>();
        }

        public Guid? ParentId { get; set; }

        public virtual TParent Parent { get; set; }

        IFintoItem IHierarchy.Parent => Parent;
        IEnumerable<IHierarchy> IHierarchy.Children => Children;

        public virtual ICollection<TParent> Children { get; set; }

        //        ICollection<IFintoItem> IHierarchy<IFintoItem>.Children { get; set; } = new List<IFintoItem>();
        IEnumerable<IFintoItemChildren> IFintoItemChildren.Children => Children;

    }

//    internal abstract class FintoItemBase<TName> : FintoItemBase where TName : NameBase
//    {
//        protected FintoItemBase()
//        {
//            Names = new HashSet<TName>();
//        }
//
//        public virtual ICollection<TName> Names { get; set; }
//
////        IFintoItem IHierarchy<IFintoItem>.Parent
////        {
////            get { return Parent; }
////            set { Parent = (TParent) value; }
////        }
////
////        IEnumerable<IFintoItem> IHierarchy<IFintoItem>.Children => Children;
//
//    }
}
