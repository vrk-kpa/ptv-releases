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
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models.Interfaces
{
    /// <summary>
    /// Base view model interface of entities
    /// </summary>
    public interface IVmEntityBase : IVmBase
    {
        /// <summary>
        /// Entity identifier.
        /// </summary>
        Guid? Id { get; set; }
    }

    /// <summary>
    /// Base view model interface of entities
    /// </summary>
    public interface IVmRootBasedEntity : IVmBase
    {
        /// <summary>
        /// Identifier of root node.
        /// </summary>
        Guid UnificRootId { get; set; }
    }

    /// <summary>
    /// Base view model interface of entities with included publishing status
    /// </summary>
    public interface IVmEntityStatusBase : IVmEntityBase
    {
        /// <summary>
        /// Gets or sets the publishing status.
        /// </summary>
        /// <value>
        /// The publishing status.
        /// </value>
        Guid? PublishingStatusId { get; set; }
    }

    /// <summary>
    /// base view model interface of entity with locking functionality
    /// </summary>
    /// <seealso cref="PTV.Domain.Model.Models.Interfaces.IVmEntityBase" />
    public interface IVmEntityLockBase : IVmEntityBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether entity locked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if entity locked; otherwise, <c>false</c>.
        /// </value>
        bool EntityLockedForMe { get; set; }

        /// <summary>
        /// Gets or sets the locked by.
        /// </summary>
        /// <value>
        /// The locked by.
        /// </value>
        string LockedBy { get; set; }
    }
}
