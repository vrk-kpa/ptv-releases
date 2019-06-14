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
using PTV.Domain.Model.Models.V2.Common;

namespace PTV.Domain.Model.Models.Interfaces.V2
{
    /// <summary>
    ///  View model for requesting publishing of entities
    /// </summary>
    public interface IVmMassDataModel<T> where T : IVmLocalizedEntityModel
    {
        /// <summary>
        /// List of service publishing models
        /// </summary>
        IReadOnlyList<T> Services { get; set; }
                
        /// <summary>
        /// List of channels publishing models
        /// </summary>
        IReadOnlyList<T> Channels { get; set; }
                
        /// <summary>
        /// List of organizations publishing models
        /// </summary>
        IReadOnlyList<T> Organizations { get; set; }
                
        /// <summary>
        /// List of general descriptions publishing models
        /// </summary>
        IReadOnlyList<T> GeneralDescriptions { get; set; }
                
        /// <summary>
        /// List of service collecions publishing models
        /// </summary>
        IReadOnlyList<T> ServiceCollections { get; set; }
                
        /// <summary>
        /// Valid from for scheduling
        /// </summary>
        long? PublishAt { get; set; }
                
        /// <summary>
        /// Valid to for scheduling
        /// </summary>
        long? ArchiveAt { get; set; }
        
        /// <summary>
        /// OrganizationId
        /// </summary>
        Guid OrganizationId { get; set; }

        /// <summary>
        /// Identifier of process
        /// </summary>
        Guid Id { get; set; }
    }
}