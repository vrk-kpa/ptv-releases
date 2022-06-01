using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Common;

namespace PTV.Next.Model
{
    public class EntityHistoryModel
    {
        public string Editor { get; set; }
        public DateTime EditedAt { get; set; }
        public Guid Id { get; set; }
        [JsonConverter(typeof (StringEnumConverter))]
        public EntityTypeEnum EntityType { get; set; }
        public string SubEntityType { get; set; }
        [JsonConverter(typeof (StringEnumConverter))]
        public HistoryAction HistoryAction { get; set; }
        public Guid OperationId { get; set; }
        public Dictionary<LanguageEnum, SimpleLanguageVersionModel> LanguageVersions { get; set; }
        public string Version { get; set; }
        public string NextVersion { get; set; }
        public CopyDetails CopyInfo { get; set; }
        [JsonConverter(typeof (StringEnumConverter))]
        public LanguageEnum? SourceLanguage { get; set; }
        [JsonProperty (ItemConverterType = typeof(StringEnumConverter))]
        public List<LanguageEnum> TargetLanguages { get; set; } = new List<LanguageEnum>();

        public bool ShowLink { get; set; }

        public class CopyDetails
        {
            public Guid TemplateId { get; set; }
            public Guid TemplateOrganizationId { get; set; }
            public Dictionary<LanguageEnum,string> TemplateOrganizationNames { get; set; }
        }
    }
}