using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;

namespace PTV.Next.Model
{
    public class PublishingModel
    {
        public DateTime? PublishAt { get; set; }
        public DateTime? ArchiveAt { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PublishingStatus Status { get; set; }
    }
}