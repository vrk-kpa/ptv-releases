using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;
namespace PTV.Next.Model
{
    public class GdServiceChannelModel
    {
        public Guid Id { get; set; }
        public Guid UnificRootId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceChannelTypeEnum ChannelType { get; set; }

        public OrganizationModel Organization { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceChannelConnectionTypeEnum ConnectionType { get; set; }

        public string ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Dictionary<LanguageEnum, GdServiceChannelLvModel> LanguageVersions { get; set; } = new Dictionary<LanguageEnum, GdServiceChannelLvModel>();
    }

    public class GdServiceChannelLvModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public PublishingStatus Status { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public LanguageEnum Language { get; set; }

        public string Name { get; set; }
        public string ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }
        public DateTime? ScheduledPublish { get; set; }
    }
}
