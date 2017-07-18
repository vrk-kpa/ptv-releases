using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace PTV.Framework.Enums
{
    [Flags]
    public enum EnvironmentTypeEnum
    {
        [JsonProperty("test")]
        Test = 1,
        [JsonProperty("dev")]
        Dev = 2,
        [JsonProperty("qa")]
        Qa = 4,
        Trn = 8,
        Prod = 16
    }
}
