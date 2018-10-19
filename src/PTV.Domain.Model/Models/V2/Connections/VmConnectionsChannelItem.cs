using PTV.Domain.Model.Models.V2.Common;
using System;
using System.Collections.Generic;
using System.Text;
using PTV.Domain.Model.Models.V2.Common.Connections;

namespace PTV.Domain.Model.Models.V2.Connections
{
    /// <summary>
    /// 
    /// </summary>
    public class VmConnectionsChannelItem : VmChannelConnectionsOutput
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
        public Guid? ChannelTypeId { get; set; }
        /// <summary>
        /// Gets or sets the service type identifier.
        /// </summary>
        /// <value>
        /// The service type identifier.
        /// </value>
        public string ChannelType { get; set; }
        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        /// <value>
        /// The organization.
        /// </value>
        public Guid? OrganizationId { get; set; }        
    }
}
