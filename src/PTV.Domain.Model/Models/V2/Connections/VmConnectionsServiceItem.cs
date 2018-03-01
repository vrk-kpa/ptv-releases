using PTV.Domain.Model.Models.V2.Common;
using System;
using System.Collections.Generic;

namespace PTV.Domain.Model.Models.V2.Connections
{
    /// <summary>
    /// 
    /// </summary>
    public class VmConnectionsServiceItem : VmConnectionsOutput
    {
        /// <summary>
        /// Gets or sets the unific root identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid UnificRootId { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public Dictionary<string, string> Name { get; set; }
        /// <summary>
        /// Gets or sets the service type identifier.
        /// </summary>
        /// <value>
        /// The service type identifier.
        /// </value>
        public Guid? ServiceTypeId { get; set; }
        /// <summary>
        /// Gets or sets the service type identifier.
        /// </summary>
        /// <value>
        /// The service type identifier.
        /// </value>
        public string ServiceType { get; set; }
        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        /// <value>
        /// The organization.
        /// </value>
        public Guid? OrganizationId { get; set; }
        /// <summary>
        /// Gets or sets the SuggestedChannels.
        /// </summary>
        public List<Guid> SuggestedChannelIds { get; set; }
    }
}
