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
using NetTopologySuite.Geometries;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.Model.Models
{
    internal partial class ClsAddressStreetNumber : EntityIdentifierBase, IIsValid,IClsAddressEntity
    {    
        public ClsAddressStreetNumber()
        {
            Coordinates = new HashSet<ClsStreetNumberCoordinate>();
            AddressPoints = new HashSet<ClsAddressPoint>();
        }
        
        public Guid ClsAddressStreetId { get; set; }  
        public ClsAddressStreet ClsAddressStreet { get; set; }  
        
        public Guid PostalCodeId { get; set; }
        public PostalCode PostalCode { get; set; }  
        
        public bool IsEven { get; set; }
        public int StartNumber { get; set; }
        public string StartCharacter { get; set; }
        public int StartNumberEnd { get; set; }
        public string StartCharacterEnd { get; set; }
        
        public int EndNumber { get; set; }
        public string EndCharacter { get; set; }
        public int EndNumberEnd { get; set; }
        public string EndCharacterEnd { get; set; }

        public bool IsValid { get; set; } = true;
        public bool NonCls { get; set; }
        public Guid UpdateIdentifier { get; set; }
        
        public ICollection<ClsStreetNumberCoordinate> Coordinates { get; set; }
        
        public virtual ICollection<ClsAddressPoint> AddressPoints { get; set; }
    }
}