using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.Framework.TextManager.Models
{
    /// <summary>
    /// Line structure of draftJs component
    /// </summary>
    public class VmTextLine
    {
        /// <summary>
        /// Text of draftJs component
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Types of line of draftJs component
        /// </summary>
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public TextFormatClientTypeEnum Type { get; set; }

        /// <summary>
        /// Unique key of line draftJs component
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Depth key of line draftJs component
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// InlineStyleRanges of line draftJs component
        /// </summary>
        public HashSet<object> InlineStyleRanges { get; set; }

        /// <summary>
        /// EntityRanges of line draftJs component
        /// </summary>
        public HashSet<object> EntityRanges { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public VmTextLine()
        {
            InlineStyleRanges = new HashSet<object>();
            EntityRanges = new HashSet<object>();
        }

    }
}
