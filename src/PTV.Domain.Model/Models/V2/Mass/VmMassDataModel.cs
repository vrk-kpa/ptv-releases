using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Domain.Model.Models.V2.Mass
{
    
    /// <summary>
    /// Generic view model for requesting publishing, coping, archiving
    /// </summary>
    public class VmMassDataModel<T> : IVmMassDataModel<T> where T : IVmLocalizedEntityModel
    {
        /// <summary>
        /// Constructor of publishing 
        /// </summary>
        public VmMassDataModel()
        {
            Services = new List<T>();
            Channels = new List<T>();
            GeneralDescriptions = new List<T>();
            Organizations = new List<T>();
            ServiceCollections = new List<T>();
        }
        
        /// <summary>
        /// List of service publishing models
        /// </summary>
        public IReadOnlyList<T> Services { get; set; }
        
        /// <summary>
        /// List of channels publishing models
        /// </summary>
        public IReadOnlyList<T> Channels { get; set; }
        
        /// <summary>
        /// List of organizations publishing models
        /// </summary>
        public IReadOnlyList<T> Organizations { get; set; }
        
        /// <summary>
        /// List of general descriptions publishing models
        /// </summary>
        public IReadOnlyList<T> GeneralDescriptions { get; set; }
        
        /// <summary>
        /// List of service collecions publishing models
        /// </summary>
        public IReadOnlyList<T> ServiceCollections { get; set; }
        
        /// <summary>
        /// Valid from for scheduling
        /// </summary>
        [JsonProperty("ValidFrom")]
        public long? PublishAt { get; set; }
        
        /// <summary>
        /// Valid to for scheduling
        /// </summary>
        [JsonProperty("ValidTo")]
        public long? ArchiveAt { get; set; }
        
        /// <summary>
        /// OrganizationId
        /// </summary>
        [JsonProperty("Organization")]
        public Guid OrganizationId { get; set; }
    }
}