using System;
using Newtonsoft.Json;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Domain.Model.Models
{
    /// <summary>
    /// View Model for coordinates
    /// </summary>
    public class VmCoordinate : VmEntityBase, IVmCoordinate
    {
        /// <summary>
        /// Id of owner entity
        /// </summary>
        [JsonIgnore]
        public Guid? OwnerReferenceId { get; set; }
        /// <summary>
        /// Id of coordinate type
        /// </summary>
        [JsonIgnore]
        public Guid? TypeId { get; set; }
        /// <summary>
        /// Indicates, whether cooridnate is main or not
        /// </summary>
        public bool IsMain { get; set; }
        /// <summary>
        /// latitutde of coordinate
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// longtitude of coordinate
        /// </summary>
        public double Longtitude { get; set; }
        /// <summary>
        /// State of the coordinate
        /// </summary>
        public string CoordinateState { get; set; }
    }
}