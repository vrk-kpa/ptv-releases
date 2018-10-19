using System.Collections.Generic;
using PTV.Domain.Model.Models.V2.Common;

namespace PTV.Domain.Model.Models.V2.Mass
{
    /// <summary>
    /// View model for saving language availabilities 
    /// </summary>
    public class VmMassLanguageAvailabilityModelList
    {
        /// <summary>
        /// Constructor of vm  
        /// </summary>
        public VmMassLanguageAvailabilityModelList()
        {
            LanguageAvailabilities = new List<VmMassLanguageAvailabilityModel>();
        }

        /// <summary>
        /// list of Language availabilities
        /// </summary>
        public List<VmMassLanguageAvailabilityModel> LanguageAvailabilities { get; set; }
    }
}