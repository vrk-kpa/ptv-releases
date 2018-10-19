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
using System.Collections.Generic;
using PTV.Database.Model.Models.Base;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Privileges;

namespace PTV.Database.Model.Models
{
    internal partial class Organization : VersionedRoot<OrganizationVersioned>, ILockable, IBlockableEntity<OrganizationBlockedAccessRight>
    {
        public Organization()
        {
            Children = new HashSet<OrganizationVersioned>();
            OrganizationServices = new HashSet<OrganizationService>();
            OrganizationServiceChannelsVersioned = new HashSet<ServiceChannelVersioned>();
            BlockedAccessRights = new HashSet<OrganizationBlockedAccessRight>();
            ServiceProducerOrganizations = new HashSet<ServiceProducerOrganization>();
            OrganizationServicesVersioned = new HashSet<ServiceVersioned>();
            SahaOrganizationInformations = new HashSet<SahaOrganizationInformation>();
            OrganizationFilters = new HashSet<OrganizationFilter>();
            RegionRelatedOrganizations = new HashSet<OrganizationVersioned>();
        }

        public virtual ICollection<OrganizationVersioned> Children { get; set; }
        public virtual ICollection<OrganizationService> OrganizationServices { get; set; }
        public virtual ICollection<ServiceChannelVersioned> OrganizationServiceChannelsVersioned { get; set; }
        public virtual ICollection<OrganizationBlockedAccessRight> BlockedAccessRights { get; set; }
        public virtual ICollection<ServiceVersioned> OrganizationServicesVersioned { get; set; }
        public virtual ICollection<ServiceProducerOrganization> ServiceProducerOrganizations { get; set; }
        public virtual ICollection<SahaOrganizationInformation> SahaOrganizationInformations { get; set; }
        public virtual ICollection<OrganizationFilter> OrganizationFilters { get; set; }
        public virtual ICollection<OrganizationVersioned> RegionRelatedOrganizations { get; set; }
    }
}