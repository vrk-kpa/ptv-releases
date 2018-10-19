using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Domain.Model.Models.V2.Connections
{
    /// <summary>
    ///
    /// </summary>
    public class VmConnectionsChannelSearch : IVmMultiLocalized, IVmSearchParamsBase
    {
        /// <summary>
        ///
        /// </summary>
        public Guid? Id { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string Fulltext { get; set; }
        /// <summary>
        ///
        /// </summary>
        public List<string> Languages { get; set; }
        /// <summary>
        ///
        /// </summary>
        public Guid? OrganizationId { get; set; }
        /// <summary>
        ///
        /// </summary>
        public string ChannelType { get; set; }
        /// <summary>
        ///
        /// </summary>
        public int PageNumber { get; set; }
        /// <summary>
        ///
        /// </summary>
        public int MaxPageCount { get; set; }
        /// <summary>
        ///
        /// </summary>
        public int Skip { get; set; }
        /// <summary>
        /// Gets or sets the selected publishing statuses.
        /// </summary>
        /// <value>
        /// The selected publishing statuses.
        /// </value>
        public List<Guid> SelectedPublishingStatuses { get; set; }
        /// <summary>
        /// Gets or sets the selected area information types.
        /// </summary>
        public List<Guid> AreaInformationTypes { get; set; }
        /// <summary>
        /// Gets or sets the channel ids.
        /// </summary>
        public List<Guid> ChannelIds { get;set; }
    }
}
