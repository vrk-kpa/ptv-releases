using System;
using System.Collections.Generic;
using System.Text;
using PTV.Framework.Interfaces;

namespace PTV.Framework
{
    /// <summary>
    /// View model of get channel (base)
    /// </summary>
    public class ValidationMessage : IError
    {
        public ValidationMessage()
        {
            ValidationPaths = new List<ValidationPath>();
        }

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Error messages
        /// </summary>
        public string ErrorMessages { get; set; }

        /// <summary>
        /// Validation Path
        /// </summary>
        public List<ValidationPath> ValidationPaths { get; set; }
    }
}
