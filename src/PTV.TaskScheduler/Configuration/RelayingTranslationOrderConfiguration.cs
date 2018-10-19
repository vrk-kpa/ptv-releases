using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.TaskScheduler.Configuration
{
    public class RelayingTranslationOrderConfiguration : UrlBaseConfiguration
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public override string ConfigurationName => this.GetType().Name;
    }
}
