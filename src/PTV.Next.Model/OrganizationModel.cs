using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;
using System;
using System.Collections.Generic;

namespace PTV.Next.Model
{
    public class OrganizationModel
    {
        public Guid Id{get;set;}
        public string Name{get;set;}
        public string AlternateName{get;set;}
        public string Code { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PublishingStatus PublishingStatus { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public OrganizationTypeEnum? Type{get;set;}
        public IDictionary<string, string> Texts { get; set; }
        public Guid VersionedId { get; set; }
    }
}
