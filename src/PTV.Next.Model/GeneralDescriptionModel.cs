using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;

namespace PTV.Next.Model
{
    public class GeneralDescriptionModel
    {
        public Guid Id { get; set; }
        public Guid UnificRootId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PublishingStatus Status { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public GeneralDescriptionTypeEnum GeneralDescriptionType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceChargeTypeEnum? ChargeType { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceTypeEnum ServiceType { get; set; }

        public Dictionary<LanguageEnum, GeneralDescriptionLanguageVersionModel> LanguageVersions = new Dictionary<LanguageEnum, GeneralDescriptionLanguageVersionModel>();
        public List<string> TargetGroups { get; set; } = new List<string>();
        public List<Guid> IndustrialClasses { get; set; } = new List<Guid>();
        public List<Guid> ServiceClasses { get; set; } = new List<Guid>();
        public List<Guid> LifeEvents { get; set; } = new List<Guid>();
        public List<OntologyTermModel> OntologyTerms { get; set; } = new List<OntologyTermModel>();
        public List<GdServiceChannelModel> Channels { get; set; } = new List<GdServiceChannelModel>();
    }

    public class GeneralDescriptionLanguageVersionModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LanguageEnum Language { get; set; }
        public string Name { get; set;}
        public string AlternativeName { get; set;}
        [JsonConverter(typeof(StringEnumConverter))]
        public PublishingStatus Status { get; set; }
        public string Description { get; set;}
        public string Summary { get; set;}
        public string UserInstructions { get; set;}
        public string Deadline { get; set;}
        public string ProcessingTime { get; set;}
        public string PeriodOfValidity { get; set;}
        public string Conditions { get; set;}
        public string BackgroundDescription { get; set;}
        
        public string GeneralDescriptionTypeAdditionalInformation { get; set;}
        public ChargeModel Charge { get; set; }
        public List<string> Keywords { get; set; } = new List<string>();
        public List<LinkModel> Laws { get; set; } = new List<LinkModel>();
    }
}
