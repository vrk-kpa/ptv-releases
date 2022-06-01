using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Framework.Enums;

namespace PTV.Application.Web.ViewModels
{
    public class EnvSettingsModel
    {
        public string UiApiUrl { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public EnvironmentTypeEnum EnvironmentType { get; set; }

        public bool IsPAHALoginEnabled { get; set; }
        public bool IsFakeAuthenticationEnabled { get; set; }
        public string VersionPrefix { get; set; }
        public string Version { get; set; }
        public string ReleaseNumber { get; set; }
        public string PAHARedirectUrl { get; set; }
        public string PAHAReturnUrl { get; set; }
    }
}