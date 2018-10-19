using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Domain.Model.Models.V2.Common.Connections
{
    /// <summary>
    /// 
    /// </summary>
    public class VmConnectionOperation
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the languages availabilities.
        /// </summary>
        /// <value>
        /// The languages availabilities.
        /// </value>
        public IReadOnlyList<VmLanguageAvailabilityInfo> LanguagesAvailabilities { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public Dictionary<string, string> Name { get; set; }
        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        /// <value>
        /// The operation type.
        /// </value>
        public string OperationType { get; set; }
        /// <summary>
        /// Gets or sets the modified by.
        /// </summary>
        /// <value>
        /// The modified by.
        /// </value>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Gets or sets the modified.
        /// </summary>
        /// <value>
        /// The modified.
        /// </value>
        public long Created { get; set; }
        /// <summary>
        /// Gets or sets the status of entity
        /// </summary>
        public Guid PublishingStatusId { get; set; }
    }
}
