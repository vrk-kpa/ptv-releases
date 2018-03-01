using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models.V2.Channel.PrintableForm
{
    /// <summary>
    /// View model from printable form input
    /// </summary>
    public class VmPrintableFormInput : VmServiceChannel, Interfaces.V2.IAttachments, IVmLocalized
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> FormIdentifier { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> FormReceiver { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, List<VmWebPage>> FormFiles { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public VmAddressSimple DeliveryAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, List<VmChannelAttachment>> Attachments { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public Guid PrintableFormChannelId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public LanguageCode Language { get; set; }
    }
}
