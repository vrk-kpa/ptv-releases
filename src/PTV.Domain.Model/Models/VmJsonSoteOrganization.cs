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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Domain.Model.Models
{

    /// <summary>
    /// Model for Sote service
    /// </summary>
    public class VmJsonSoteOrganization : ILanguagesAvailabilities
    {

        /// <summary>
        /// Id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Main organization id
        /// </summary>
        public Guid MainOrganizationId { get; set; }

        /// <summary>
        /// Responsible organization id
        /// </summary>
        public Guid ResponsibleOrganizationId { get; set; }

        /// <summary>
        /// Organization type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Area information type
        /// </summary>
        public string AreaInformationType { get; set; }

        /// <summary>
        /// Oid
        /// </summary>
        public string Oid { get; set; }

        /// <summary>
        /// Sote organization name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Contact info
        /// </summary>
        public VmJsonSoteContactInfo ContactInfo { get; set; }

        /// <summary>
        /// Service locations
        /// </summary>
        public List<VmJsonSoteServiceLocation> ServiceLocations { get; set; }

        /// <summary>
        /// Available languages
        /// </summary>
        public IReadOnlyList<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }

        /// <summary>
        /// Valid from
        /// </summary>
        public DateTime? ValidFrom { get; set; }

        /// <summary>
        /// Valid to
        /// </summary>
        public DateTime? ValidTo { get; set; }
    }

    /// <summary>
    /// Model for sote contact info
    /// </summary>
    public class VmJsonSoteContactInfo
    {
        /// <summary>
        /// Phone number
        /// </summary>
        public string  PhoneNumber { get; set; }

        /// <summary>
        /// Street address
        /// </summary>
        public VmJsonSoteAddress Address { get; set; }
    }

    /// <summary>
    /// Model for sote address
    /// </summary>
    public class VmJsonSoteAddress : IVmOrderable
    {
        /// <summary>
        /// Address Id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Street address
        /// </summary>
        public VmJsonSoteStreetAddress StreetAddress { get; set; }

        /// <summary>
        /// Postal code
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Postal code Id
        /// </summary>
        public Guid? PostalCodeId { get; set; }

        /// <summary>
        /// Post office
        /// </summary>
        public string PostOffice { get; set; }

        /// <summary>
        /// Municipality name
        /// </summary>
        public string MunicipalityName { get; set; }

        /// <summary>
        /// Municipality Id
        /// </summary>
        public Guid? MunicipalityId { get; set; }

        /// <summary>
        /// Order number
        /// </summary>
        public int? OrderNumber { get; set; }
    }

    /// <summary>
    /// Model for sote street address
    /// </summary>
    public class VmJsonSoteStreetAddress
    {
        /// <summary>
        /// Street number
        /// </summary>
        public string StreetNumber { get; set; }

        /// <summary>
        /// ID of related street range
        /// </summary>
        public Guid? StreetRangeId { get; set; }

        /// <summary>
        /// Street name
        /// </summary>
        public string StreetName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public Guid? StreetId { get; set; }
    }

    /// <summary>
    /// Service location
    /// </summary>
    public class VmJsonSoteServiceLocation : ILanguagesAvailabilities
    {

        /// <summary>
        /// Id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// OrganizationId
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        /// Oid
        /// </summary>
        public string Oid { get; set; }

        /// <summary>
        /// Sote service location name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Sote service location alternate name
        /// </summary>
        public string AlternateName { get; set; }

        /// <summary>
        /// Localization Id
        /// </summary>
        public Guid LocalizationId { get; set; }

        /// <summary>
        /// Contact info
        /// </summary>
        public VmJsonSoteContactInfo ContactInfo { get; set; }

        /// <summary>
        /// Available languages
        /// </summary>
        public IReadOnlyList<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }
    }
}
