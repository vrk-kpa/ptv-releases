using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;

namespace PTV.Next.Model
{
    public class LastTranslationModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LanguageEnum SourceLanguage { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public LanguageEnum TargetLanguage { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TranslationStateTypeEnum State { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public DateTime OrderedAt { get; set; }
        public bool Checked { get; set; }
        public Guid TranslationId { get; set; }
    }
}