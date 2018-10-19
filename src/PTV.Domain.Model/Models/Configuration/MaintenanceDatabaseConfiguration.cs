using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Domain.Model.Models.Configuration
{
    /// <summary>
    /// Database configuration for maintenance restore, dump, etc.
    /// </summary>
    public class MaintenanceDatabaseConfiguration
    {
        /// <summary>
        /// CommandsPath
        /// </summary>
        public string CommandsPath { get; set; }
        /// <summary>
        /// DumpName
        /// </summary>
        public string DumpName { get; set; }
        /// <summary>
        /// DumpSchema
        /// </summary>
        public string DumpSchema { get; set; }
        /// <summary>
        /// Database
        /// </summary>
        public string Database { get; set; }
        /// <summary>
        /// Host
        /// </summary> 
        public string Host { get; set; }
        /// <summary>
        /// Port
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Source folder Name
        /// </summary>
        public string RestoreDumpFolderName { get; set; }
        /// <summary>
        /// Target folder Name
        /// </summary>
        public string BackupDumpFolderName { get; set; }

        /// <summary>
        /// ConfigurationName
        /// </summary>
        public virtual string ConfigurationName => this.GetType().Name;
    }
}
