using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;

namespace PTV.Next.Model
{
    public class TranslationHistoryModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LanguageEnum SourceLanguage { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LanguageEnum TargetLanguage { get; set; }
        public Guid OrderId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TranslationStateTypeEnum State { get; set; }
        public DateTime OrderedAt { get; set; }
        public string SubscriberEmail { get; set; }
        public long OrderNumber { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
    }
}