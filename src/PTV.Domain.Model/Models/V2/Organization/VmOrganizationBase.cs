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

namespace PTV.Domain.Model.Models.V2.Organization
{
    /// <summary>
    /// ViewModel for basic organization information
    /// </summary>
    public class VmOrganizationBase : VmOrganizationHeader
    {
        /// <summary>
        /// 
        /// </summary>
        public VmOrganizationBase()
        {
            AreaInformation = new VmAreaInformation();
        }
        /// <summary>
        /// Gets or sets the alternate organization name.
        /// </summary>
        /// <value>
        /// The alternate name.
        /// </value>
        public Dictionary<string, string> AlternateName { get; set; }
        /// <summary>
        /// Gets or sets the short description.
        /// </summary>
        /// <value>
        /// The short description.
        /// </value>
        public Dictionary<string, string> ShortDescription { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public Dictionary<string, string> Description { get; set; }
        /// <summary>
        /// Gets or sets the organization type id.
        /// </summary>
        /// <value>
        /// The funding type id.
        /// </value>
        public Guid? OrganizationType { get; set; }        
        /// <summary>
        /// Gets or sets the Oid.
        /// </summary>
        /// <value>
        /// The organization id.
        /// </value>
        public string OrganizationId { get; set; }
        /// <summary>
        /// Gets or sets the area information.
        /// </summary>
        /// <value>
        /// The area information.
        /// </value>
        public VmAreaInformation AreaInformation { get; set; }
        /// <summary>
        /// Gets or sets the organization phones.
        /// </summary>
        /// <summary>
        /// phone number of channel
        /// </summary>
        public Dictionary<string, List<VmPhone>> PhoneNumbers { get; set; }
        /// <summary>
        /// Gets or sets the organization emails.
        /// </summary>
        /// <summary>
        /// email of the channel
        /// </summary>
        public Dictionary<string, List<VmEmailData>> Emails { get; set; }
        /// <summary>
        /// Gets or sets the organization attachments.
        /// </summary>
        /// <summary>
        /// email of the channel
        /// </summary>
        public Dictionary<string, List<VmWebPage>> WebPages{ get; set; }
        /// <summary>
        /// Gets or sets the parent organization id.
        /// </summary>
        /// <summary>
        /// parent id
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// Gets or sets is main organization.
        /// </summary>
        /// <summary>
        /// is main organization
        /// </summary>
        public bool IsMainOrganization { get; set; }
        /// <summary>
        /// Gets or sets list of language codes.
        /// </summary>
        public List<string> IsAlternateNameUsedAsDisplayName { get; set; }
        /// <summary>
        /// Gets or sets the business.
        /// </summary>
        /// <value>
        /// The business.
        /// </value>
        public VmBusiness Business { get; set; }
        /// <summary>
        /// Gets or sets the Area information type.
        /// </summary>
        /// <value>
        /// The  Area information type.
        /// </value>
        public Guid? AreaInformationTypeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<VmAddressSimple> PostalAddresses { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<VmAddressSimple> VisitingAddresses { get; set; }
    }
}