using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PTV.Next.Model
{
    public class TranslationOrderModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LanguageEnum Source { get; set; }
        [JsonProperty("targets", ItemConverterType=typeof(StringEnumConverter))]
        public List<LanguageEnum> Targets { get; set; }
        public string Subscriber { get; set; }
        public string Email { get; set; }
        public string AdditionalInformation { get; set; }
    }
}