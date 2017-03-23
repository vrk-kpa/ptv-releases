using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PTV.Framework.TextManager.Models
{
    public class VmTextBlock
    {
        /// <summary>
        /// EntityMap of draftJs component
        /// </summary>
        public object EntityMap { get; set; }

        /// <summary>
        /// Blocks of draftJs component
        /// </summary>
        public List<VmTextLine> Blocks { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public VmTextBlock()
        {
            Blocks = new List<VmTextLine>();
            EntityMap = new object();
        }
    }
}
