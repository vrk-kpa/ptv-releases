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
using PTV.Domain.Model.Enums.Security;

namespace PTV.Domain.Model.Models.V2.Common.Connections
{
    /// <summary>
    /// ViewModel for all connections information from connection page
    /// </summary>
    public class VmConnectionsPageInput
    {
        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        /// <value>
        /// The Main Entity Type - Services/Channels
        /// </value>
        public DomainEnum MainEntityType { get; set; }
        /// <summary>
        /// Gets or sets the list of connections.
        /// </summary>
        /// <value>
        /// The list of connectios.
        /// </value>
        public List<VmConnectionPageInput> Connections { get; set; }
    }
    /// <summary>
    /// ViewModel for one entity connections information from connection page
    /// </summary>
    public class VmConnectionPageInput
    {
        /// <summary>
        /// Gets or sets the main entity.
        /// </summary>
        /// <value>
        /// The Main Entity
        /// </value>
        public MainConnectionEntity MainEntity { get; set; }
        /// <summary>
        /// Gets or sets the list of connections to main entity.
        /// </summary>
        /// <value>
        /// The list of connectios.
        /// </value>
        public List<VmConnectionInput> Childs { get; set; }
    }
    /// <summary>
    /// ViewModel for main entity connection information
    /// </summary>
    public class MainConnectionEntity
    {
        /// <summary>
        /// Gets or sets the main entity root id.
        /// </summary>
        /// <value>
        /// The Main Entity id
        /// </value>
        public Guid UnificRootId { get; set; }
        /// <summary>
        /// Gets or sets the main entity id.
        /// </summary>
        /// <value>
        /// The Main Entity id
        /// </value>
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? ChannelTypeId { get; set; }
    }

}
