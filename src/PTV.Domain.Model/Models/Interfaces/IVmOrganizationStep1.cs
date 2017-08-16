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
using PTV.Domain.Model.Models.Interfaces.Security;

namespace PTV.Domain.Model.Models.Interfaces
{
    /// <summary>
    /// View model interface of organization step 1
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmEntityBase" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IWebPages" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IAddresses" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IEmails" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IPhoneNumbers" />
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmLocalized" />
    /// <seealso cref="IVmEntitySecurity" />
    public interface IVmOrganizationStep1 : IVmEntityBase, IWebPages, IAddresses, IEmails, IPhoneNumbers, IVmLocalized, IVmEntitySecurity
    {
//        /// <summary>
//        /// Identifier of unific root id
//        /// </summary>
//        Guid UnificRootId { get; set; }
        /// <summary>
        /// Gets or sets the parent identifier.
        /// </summary>
        /// <value>
        /// The parent identifier.
        /// </value>
        Guid? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the organization type identifier.
        /// </summary>
        /// <value>
        /// The organization type identifier.
        /// </value>
        Guid? OrganizationTypeId { get; set; }

        /// <summary>
        /// Gets or sets the municipality.
        /// </summary>
        /// <value>
        /// The municipality.
        /// </value>
        VmListItem Municipality { get; set; }

        /// <summary>
        /// Gets or sets the name of the organization.
        /// </summary>
        /// <value>
        /// The name of the organization.
        /// </value>
        string OrganizationName { get; set; }

        /// <summary>
        /// Gets or sets the alternate name of the organization.
        /// </summary>
        /// <value>
        /// The alternate name of the organization.
        /// </value>
        string OrganizationAlternateName { get; set; }

        /// <summary>
        /// Gets or sets the display name identifier.
        /// </summary>
        /// <value>
        /// The display name identifier.
        /// </value>
        Guid DisplayNameId { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is alternate name used as display name.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is alternate name used as display name; otherwise, <c>false</c>.
        /// </value>
        bool IsAlternateNameUsedAsDisplayName { get; set; }
        /// <summary>
        /// Gets or sets the organization identifier.
        /// </summary>
        /// <value>
        /// The organization identifier.
        /// </value>
        string OrganizationId { get; set; }
        /// <summary>
        /// Gets or sets the are infomration type identifier.
        /// </summary>
        /// <value>
        /// The area information type identifier.
        /// </value>
        Guid AreaInformationTypeId { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        string Description { get; set; }

        /// <summary>
        /// Gets or sets the business.
        /// </summary>
        /// <value>
        /// The business.
        /// </value>
        VmBusiness Business { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether postal address should be shown or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if postal address should be shown; otherwise, <c>false</c>.
        /// </value>
        bool ShowPostalAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether visiting address should be shown.
        /// </summary>
        /// <value>
        ///   <c>true</c> if visiting address should shown; otherwise, <c>false</c>.
        /// </value>
        bool ShowVisitingAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether contacts should be shown.
        /// </summary>
        /// <value>
        ///   <c>true</c> if contacts should be shown; otherwise, <c>false</c>.
        /// </value>
        bool ShowContacts { get; set; }
    }
}
