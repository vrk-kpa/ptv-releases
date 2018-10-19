using System;

namespace PTV.Domain.Model
{
    /// <summary>
    /// Model for data regarding restriction filter
    /// </summary>
    public class RestrictedEntityInfo
    {
        /// <summary>
        /// Name of entity type
        /// </summary>
        public string EntityTypeName { get; set; }

        /// <summary>
        /// Value in mentioned type column
        /// </summary>
        public Guid EntityTypeValue { get; set; }
    }
}