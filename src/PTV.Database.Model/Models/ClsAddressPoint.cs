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
using System.ComponentModel.DataAnnotations;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Framework.Formatters;

namespace PTV.Database.Model.Models
{
    internal partial class ClsAddressPoint : EntityIdentifierBase, IIsValid
    {
        public ClsAddressPoint()
        {
            
        }
        public Guid PostalCodeId { get; set; }
        public Guid MunicipalityId { get; set; }
        public Guid AddressStreetId { get; set; }
        public Guid? AddressStreetNumberId { get; set; }
        public Guid AddressId { get; set; }
        [MaxLength(30)]
        [TrimSpacesFormatter]
        public string StreetNumber { get; set; }
        
        public virtual Municipality Municipality { get; set; }
        public virtual PostalCode PostalCode { get; set; }
        public virtual ClsAddressStreetNumber AddressStreetNumber { get; set; }
        public virtual ClsAddressStreet AddressStreet { get; set; }
        public virtual Address Address { get; set; }

        public bool IsValid { get; set; } = true;
    }
}