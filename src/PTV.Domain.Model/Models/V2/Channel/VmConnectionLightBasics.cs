using System;

namespace PTV.Domain.Model.Models.V2.Channel
{
    /// <summary>
    /// 
    /// </summary>
    public class VmConnectionLightBasics
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid ServiceId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ChannelId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Modified { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ModifiedBy { get; set; }
    }
}