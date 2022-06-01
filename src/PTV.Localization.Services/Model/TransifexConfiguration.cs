using System.Collections.Generic;

namespace PTV.Localization.Services.Model
{
    public class TransifexConfiguration
    {
        public enum ParamTypes
        {
            Translation,
            Content
        }
        public string Project { get; set; }
        public string Url { get; set; }
        public Dictionary<string, TransifexProject> Projects { get; set; } = new Dictionary<string, TransifexProject>();
        public string Authorization { get; set; }
        public string WorkingFolder { get; set; }
    }
}
