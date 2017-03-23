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
using System.ComponentModel.DataAnnotations;
using PTV.Database.Model.Models.Base;
using PTV.Framework.Formatters;
using PTV.Framework.Formatters.Attributes;

namespace PTV.Database.Model.Models
{
    internal partial class Address : ValidityBase
    {
        public Address()
        {
            StreetNames = new HashSet<StreetName>();
            AddressAdditionalInformations = new HashSet<AddressAdditionalInformation>();
            Coordinates = new HashSet<Coordinate>();
        }
        [MaxLength(30)]
        [ValueFormatter(typeof(TrimSpacesFormatter))]
        public string StreetNumber { get; set; }
        public Guid CountryId { get; set; }
        public Guid PostalCodeId { get; set; }
        public Guid? MunicipalityId { get; set; }
        public string PostOfficeBox { get; set; }

        public virtual PostalCode PostalCode { get; set; }
        public virtual Country Country { get; set; }
        public virtual Municipality Municipality { get; set; }
        public virtual ICollection<StreetName> StreetNames { get; set; }
        public virtual ICollection<AddressAdditionalInformation> AddressAdditionalInformations { get; set; }
        public virtual ICollection<Coordinate> Coordinates { get; set; }
    }
}
