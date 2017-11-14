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
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models.Interfaces
{
    /// <summary>
    /// View model interface of channel relation
    /// </summary>
    /// <seealso cref="PTV.Framework.Interfaces.IVmBase" />
    public interface IVmChannelRelation : IVmBase
    {
        /// <summary>
        /// Gets or sets the service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        Guid Service { get; set; }
        /// <summary>
        /// Gets or sets the type id of the charge.
        /// </summary>
        /// <value>
        /// The type id of the charge.
        /// </value>
        Guid? ChargeType { get; set; }
        /// <summary>
        /// Gets or sets the connected channel.
        /// </summary>
        /// <value>
        /// The connected channel.
        /// </value>
        VmConnectedChannel ConnectedChannel { get; set; }
        /// <summary>
        /// Gets or sets the digital authorizations.
        /// </summary>
        /// <value>
        /// The digital authorizations.
        /// </value>
        List<VmListItem> DigitalAuthorizations { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is new.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is new; otherwise, <c>false</c>.
        /// </value>
        bool IsNew { get; set; }
    }
}
