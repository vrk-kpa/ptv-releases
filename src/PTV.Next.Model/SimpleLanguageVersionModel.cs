using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;

namespace PTV.Next.Model
{
    public class SimpleLanguageVersionModel
    {
        [JsonConverter(typeof (StringEnumConverter))]
        public PublishingStatus Status { get; set; }
        public string Name { get; set; }
        public bool IsScheduled { get; set; }
    }
}