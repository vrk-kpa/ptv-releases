using System;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// Model for item in header table for channel form
    /// </summary>
    public class VmChannelHeaderListItem
    {
        /// <summary>
        /// Modified by user name
        /// </summary>
        public string ModifiedByName { get; set; }
        /// <summary>
        /// Modified by user email
        /// </summary>
        public string ModifiedByEmail { get; set; }
        /// <summary>
        /// Modified at
        /// </summary>
        public DateTime ModifiedAt { get; set; }
        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Commented by
        /// </summary>
        public string CommentedByName { get; set; }
        /// <summary>
        /// Commented at
        /// </summary>
        public DateTime CommentedAt { get; set; }
    }
}
