using System;

namespace PTV.Database.DataAccess.Interfaces
{
    /// <summary>
    /// Model holding information about versioning
    /// </summary>
    public class VersionInfo
    {
        public Guid VersionId { get; set; }
        public Guid EntityId { get; set; }
        public int VersionMajor { get; set; }
        public int VersionMinor { get; set; }

        public Guid PublishingStatusId { get; set; }
    }
}