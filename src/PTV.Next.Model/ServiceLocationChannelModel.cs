using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;

namespace PTV.Next.Model
{
    public class ServiceChannelModel
    {
        public Guid Id { get; set; }
        public Guid UnificRootId { get; set; }
        public string ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }

        public bool OnlineAuthenticationRequired { get; set; }
        public bool ElectronicSigningRequired { get; set; }
        public int NumberOfSignaturesRequired { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceChannelTypeEnum ChannelType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceChannelConnectionTypeEnum ConnectionType { get; set; }

        public OrganizationModel Organization { get; set; }

        public List<Guid> BusinessRegions { get; set; } = new List<Guid>();
        public List<Guid> HospitalRegions { get; set; } = new List<Guid>();
        public List<Guid> Provinces { get; set; } = new List<Guid>();
        public List<Guid> Municipalities { get; set; } = new List<Guid>();
        public List<string> Languages { get; set; } = new List<string>();
        public List<ServiceChannelConnectionModel> Connections { get; set; } = new List<ServiceChannelConnectionModel>();

        public Dictionary<LanguageEnum, ServiceChannelLvModel> LanguageVersions { get; set; } = new Dictionary<LanguageEnum, ServiceChannelLvModel>();
    }

    public class ServiceChannelLvModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public PublishingStatus Status { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LanguageEnum Language { get; set; }
        public string Name { get; set; }
        public string AlternativeName { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Website { get; set; }
        public string FormIdentifier { get; set; }
        public DateTime? ScheduledPublish { get; set; }
    } 
}
