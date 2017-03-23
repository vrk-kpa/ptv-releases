using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;

namespace PTV.Domain.Model.Models.Interfaces
{
    /// <summary>
    /// ViewModel interface of entity type
    /// </summary>
    public interface IVmEntityType
    {
        /// <summary>
        /// Gets or sets the type of the main entity.
        /// </summary>
        /// <value>
        /// The type of the main entity.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        EntityTypeEnum MainEntityType { get; set; }

        /// <summary>
        /// Gets or sets the type of the sub entity.
        /// </summary>
        /// <value>
        /// The type of the sub entity.
        /// </value>
        string SubEntityType { get; set; }
    }
}