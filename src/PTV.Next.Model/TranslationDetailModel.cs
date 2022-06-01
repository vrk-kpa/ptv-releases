using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PTV.Next.Model
{
    public class TranslationDetailModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LanguageEnum SourceLanguage { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LanguageEnum TargetLanguage { get; set; }
        public string SubscriberEmail { get; set; }
        public DateTime OrderedAt { get; set; }
        public string AdditionalInformation { get; set; }
        public long OrderNumber { get; set; }
        public Guid OrderId { get; set; }
    }
}