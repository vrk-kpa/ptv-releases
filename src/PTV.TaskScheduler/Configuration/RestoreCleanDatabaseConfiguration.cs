using System;
using System.Collections.Generic;
using System.Text;
using PTV.Domain.Model.Models.Configuration;
using PTV.TaskScheduler.Interfaces;

namespace PTV.TaskScheduler.Configuration
{
    public class RestoreCleanDatabaseConfiguration : MaintenanceDatabaseConfiguration
    {
        public override string ConfigurationName => this.GetType().Name;
    }
}
