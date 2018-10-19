using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Domain.Model.Models.Interfaces.V2
{
    /// <summary>
    /// Model conating localized name
    /// </summary>
    public interface IName
    {
        /// <summary>
        /// Gets or sets the Name
        /// </summary>
        Dictionary<string, string> Name { get; set; }
    }
}
