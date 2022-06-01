using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;

namespace PTV.Next.Model
{
    public class ConnectionHistoryModel
    {
        public string Editor { get; set; }
        public DateTime EditedAt { get; set; }
        [JsonConverter(typeof (StringEnumConverter))]
        public ConnectionHistoryOperation OperationType { get; set; }
        public Dictionary<LanguageEnum, SimpleLanguageVersionModel> LanguageVersions { get; set; }
        public Guid OperationId { get; set; }
        [JsonConverter(typeof (StringEnumConverter))]
        public EntityTypeEnum EntityType { get; set; }
        [JsonConverter(typeof (StringEnumConverter))]
        public SubEntityType SubEntityType { get; set; }
        public Guid Id { get; set; }
    }

    public enum ConnectionHistoryOperation
    {
        Detached,
        Unchanged,
        Deleted,
        Modified,
        Added,
    }
}
