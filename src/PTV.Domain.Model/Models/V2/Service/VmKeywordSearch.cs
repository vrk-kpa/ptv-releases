using System;
using System.Collections.Generic;
using System.Text;
using PTV.Framework.Interfaces;

namespace PTV.Domain.Model.Models.V2.Service
{
    /// <inheritdoc />
    /// <summary>
    /// Model for keyword searching
    /// </summary>
    public class VmKeywordSearch : VmKeywordItem
    {}

    /// <summary>
    /// 
    /// </summary>
    public class VmKeywordSearchOutput : IVmBase
    {
        /// <summary>
        /// Result list of keywords
        /// </summary>
        public IReadOnlyList<VmKeywordItem> Keywords { get; set; }

        /// <summary>
        /// Is there more keywords available
        /// </summary>
        public bool MoreAvailable { get; set; }
    }
}
