using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;

namespace PTV.Next.Model
{
    public class ServiceLanguageVersionModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LanguageEnum Language { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PublishingStatus Status { get; set; }
        public string Name { get; set; }
        public bool HasAlternativeName { get; set; }
        public string AlternativeName { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string UserInstructions { get; set; }
        public string Conditions { get; set; }
        public string Deadline { get; set; }
        public string ProcessingTime { get; set; }
        public string PeriodOfValidity { get; set; }
        public List<LinkModel> Laws { get; set; }
        public ChargeModel Charge { get; set; }
        public ServiceVoucherModel Voucher { get; set; }
        public List<string> Keywords { get; set; }
        public List<string> PurchaseProducerNames { get; set; }
        public List<string> OtherProducerNames { get; set; }
        public DateTime Modified { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ScheduledPublish { get; set; }
        public DateTime? ScheduledArchive { get; set; }
        public TranslationAvailabilityModel TranslationAvailability { get; set; }
    }
}