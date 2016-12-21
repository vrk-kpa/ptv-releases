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
using System.Threading.Tasks;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Domain.Model.Models
{
    public class VmOrganizationStep1 : VmEnumEntityStatusBase, IVmOrganizationStep1
    {
        public VmOrganizationStep1()
        {
            PhoneNumbers = new List<VmPhone>();
            Emails = new List<VmEmailData>();
            WebPages = new List<VmWebPage>();
            VisitingAddresses = new List<VmAddressSimple>();
            PostalAddresses = new List<VmAddressSimple>();
        }

        public Guid? ParentId { get; set; }

        public Guid? OrganizationTypeId { get; set; }

        public Guid? MunicipalityId { get; set; }

        public VmMunicipality Municipality { get; set; }

        public string OrganizationName { get; set; }

        public string OrganizationAlternateName { get; set; }

        [JsonIgnore]
        public Guid DisplayNameId { get; set; }
        public bool IsAlternateNameUsedAsDisplayName { get; set; }
        public string OrganizationId { get; set; }
        public string Description { get; set; }

        public VmBusiness Business { get; set; }

        public List<VmEmailData> Emails { get; set; }

        public List<VmPhone> PhoneNumbers { get; set; }

        public List<VmWebPage> WebPages { get; set; }

        public List<VmAddressSimple> VisitingAddresses { get; set; }

        public List<VmAddressSimple> PostalAddresses { get; set; }

        public bool ShowPostalAddress { get; set; }

        public bool ShowVisitingAddress { get; set; }

        public bool ShowContacts { get; set; }

    }

}
