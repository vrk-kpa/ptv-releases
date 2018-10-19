using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Database.Model.Interfaces
{
    interface IOrderable
    {
        /// <summary>
        /// Gets or sets the order number.
        /// </summary>
        int? OrderNumber { get; set; }
    }
}
