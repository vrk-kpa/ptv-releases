using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Domain.Model.Models.V2.Common
{
    /// <summary>
    ///  View model for Mass language availability
    /// </summary>
    public class VmMassLanguageAvailabilityModel
    {
        /// <summary>
        /// Identifier of entity that should be published
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Language Id
        /// </summary>
        public Guid LanguageId { get; set; }
        
        /// <summary>
        /// Valid from 
        /// </summary>
        public DateTime? ValidFrom { get; set; }
        
        /// <summary>
        /// Valid to
        /// </summary>
        public DateTime? ValidTo { get; set; }
        
        /// <summary>
        /// Reviewed
        /// </summary>
        public DateTime? Reviewed { get; set; }

        /// <summary>
        /// Reviewed by
        /// </summary>
        public string ReviewedBy { get; set; }
        
        /// <summary>
        /// Reviewed by
        /// </summary>
        public bool AllowSaveNull { get; set; }
 
    }
}