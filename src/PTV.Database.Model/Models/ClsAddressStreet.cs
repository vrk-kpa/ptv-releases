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
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.Model.Models
{
    internal interface IClsAddressEntity
    {
        bool NonCls { get; set; }
        Guid UpdateIdentifier { get; set; }
    }
    
    
    internal partial class ClsAddressStreet : EntityIdentifierBase, IClsAddressEntity, INameReferences, IIsValid
    {    
        public ClsAddressStreet()
        {
            StreetNames = new HashSet<ClsAddressStreetName>();
            StreetNumbers = new HashSet<ClsAddressStreetNumber>();
            AddressPoints = new HashSet<ClsAddressPoint>();
        }

        public Guid MunicipalityId { get; set; }  
        public Municipality Municipality { get; set; }  

        public virtual ICollection<ClsAddressStreetName> StreetNames { get; set; }
        public virtual ICollection<ClsAddressStreetNumber> StreetNumbers { get; set; }
        
        public bool IsValid { get; set; } = true;
        public bool NonCls { get; set; }
        public Guid UpdateIdentifier { get; set; }

        public IEnumerable<IName> Names => StreetNames;
        
        public virtual ICollection<ClsAddressPoint> AddressPoints { get; set; }
    }
}