using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PTV.Domain.Model.Enums;

namespace PTV.Next.Model
{
    public class ServiceChannelConnectionModel
    {
        public Guid ServiceId { get; set; }
        public Guid ServiceUnificRootId { get; set; }        
        public OrganizationModel ServiceOrganization { get; set; }
        public Dictionary<LanguageEnum, ServiceChannelConnectionLvModel> LanguageVersions { get; set; } = new Dictionary<LanguageEnum, ServiceChannelConnectionLvModel>();
    }

    public class ServiceChannelConnectionLvModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LanguageEnum Language { get; set; }

        public string ServiceName { get; set; }
    }

    public class ConnectionModel
    {
        public Guid ServiceId {get; set; }
        public Guid ServiceUnificRootId { get; set; }
        public Guid ServiceChannelUnificRootId { get; set; }        
        public bool IsASTIConnection { get; set; }
        public int? ChannelOrderNumber { get; set; }
        public int? ServiceOrderNumber { get; set; }
        public DateTimeOffset Modified { get; set; }
        public string ModifiedBy { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceChannelTypeEnum ChannelType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceChargeTypeEnum? ChargeType { get; set; }

        public Dictionary<LanguageEnum, ConnectionLvModel> LanguageVersions { get; set; } = new Dictionary<LanguageEnum, ConnectionLvModel>();
        public OpeningHourModel OpeningHours { get; set; }
        public List<AddressModel> Addresses { get; set; } = new List<AddressModel>();
        public List<Guid> DigitalAuthorizations { get; set; } = new List<Guid>();
        public Dictionary<LanguageEnum, List<EmailLvModel>> Emails { get; set; } = new Dictionary<LanguageEnum, List<EmailLvModel>>();
        public Dictionary<LanguageEnum, List<WebPageLvModel>> WebPages { get; set; } = new Dictionary<LanguageEnum, List<WebPageLvModel>>();
        public Dictionary<LanguageEnum, List<FaxNumberLvModel>> FaxNumbers { get; set; } = new Dictionary<LanguageEnum, List<FaxNumberLvModel>>();
        public Dictionary<LanguageEnum, List<PhoneNumberLvModel>> PhoneNumbers { get; set; } = new Dictionary<LanguageEnum, List<PhoneNumberLvModel>>();
    }

    public class WebPageLvModel
    {
        public Guid? Id {get;set;}
        public string Name { get; set; }
        public string Url { get; set; }
        public string AdditionalInformation { get; set; }
        public int? OrderNumber { get; set; }
    }

    public class FaxNumberLvModel
    {
        public Guid? Id {get;set;}
        public string Number {get; set; }
        public int OrderNumber {get; set;}
        public Guid? DialCodeId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PhoneNumberTypeEnum Type { get; set; }
    }

    public class PhoneNumberLvModel
    {
        public Guid? Id {get;set;}
        public string Number {get; set; }
        public int OrderNumber {get; set;}
        public Guid? DialCodeId { get; set; }
        public string AdditionalInformation { get; set; }
        public string ChargeDescription { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceChargeTypeEnum ChargeType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public PhoneNumberTypeEnum Type { get; set; }
    }


    public class ConnectionLvModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LanguageEnum Language { get; set; }

        public string Description { get; set; }
        public ChargeModel Charge { get; set; }        
    }

    public class OpeningHourModel
    {
        public List<StandardServiceHourModel> StandardHours { get; set; } = new List<StandardServiceHourModel>();
        public List<SpecialServiceHourModel> SpecialHours { get; set; } = new List<SpecialServiceHourModel>();
        public List<HolidayServiceHourModel> HolidayHours { get; set; } = new List<HolidayServiceHourModel>();
        public List<ExceptionalServiceHourModel> ExceptionalHours { get; set; } = new List<ExceptionalServiceHourModel>();
    }

    public class HolidayServiceHourModel
    {
        public Guid? Id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public HolidayEnum Code { get; set; }

        public bool IsClosed {get; set; }
        public TimeSpan From {get;set;}
        public TimeSpan To {get;set;}

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHoursTypeEnum Type { get; set; }
    }

    public class ExceptionalServiceHourModel
    {
        public Guid? Id { get; set; }

        public DateTimeOffset? OpeningHoursFrom { get; set; }
        public DateTimeOffset? OpeningHoursTo { get; set; }
        public bool IsClosed {get; set; }
        public TimeSpan TimeFrom { get; set;}
        public TimeSpan TimeTo { get; set;}
        public int OrderNumber { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHoursTypeEnum Type { get; set; }
        public Dictionary<LanguageEnum, ServiceHourLvModel> LanguageVersions { get; set; } = new Dictionary<LanguageEnum, ServiceHourLvModel>();
    }

    public class SpecialServiceHourModel
    {
        public Guid? Id { get; set; }
        public DateTimeOffset? OpeningHoursFrom { get; set; }
        public DateTimeOffset? OpeningHoursTo { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public WeekDayEnum DayFrom { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public WeekDayEnum? DayTo { get; set; }

        public TimeSpan TimeFrom { get; set;}
        public TimeSpan TimeTo { get; set;}

        public bool IsPeriod {get; set; }
        public int OrderNumber { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHoursTypeEnum Type { get; set; }
        public Dictionary<LanguageEnum, ServiceHourLvModel> LanguageVersions { get; set; } = new Dictionary<LanguageEnum, ServiceHourLvModel>();
    }

    public class StandardServiceHourModel
    {
        public Guid? Id { get; set; }
        public DateTimeOffset? OpeningHoursFrom { get; set; }
        public DateTimeOffset? OpeningHoursTo { get; set; }
        public bool IsPeriod {get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ServiceHoursTypeEnum Type { get; set; }

        public bool IsReservation { get; set; }
        public bool IsNonStop { get; set; }
        public int OrderNumber { get; set; }

        public Dictionary<WeekDayEnum, DailyOpeningTimeModel> DailyOpeningTimes { get; set; } = new Dictionary<WeekDayEnum, DailyOpeningTimeModel>();       
        public Dictionary<LanguageEnum, ServiceHourLvModel> LanguageVersions { get; set; } = new Dictionary<LanguageEnum, ServiceHourLvModel>();
    }

    public class ServiceHourLvModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LanguageEnum Language { get; set; }

        public string AdditionalInformation { get; set; }
    }

    public class DailyOpeningTimeModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public WeekDayEnum Day { get; set; }
        public List<FromTo> Times = new List<FromTo>();
    }

    public class FromTo
    {
        public TimeSpan From { get; set; }
        public TimeSpan To { get; set; }
    }

    public class EmailLvModel
    {
        public Guid? Id {get;set;}
        public int? OrderNumber { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }

    public class AddressModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public AddressTypeEnum Type { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AddressCharacterEnum Character { get; set; }

        public int? OrderNumber { get; set; }

        public string PostalCode { get; set; }

        public string StreetNumber {get; set;}

        public string CountryCode { get; set; }

        public StreetModel Street { get; set; }

        public StreetNumberModel StreetNumberRange { get; set; }

        public Dictionary<LanguageEnum, string> StreetName { get; set; } = new Dictionary<LanguageEnum, string>();

        public Dictionary<LanguageEnum, AddressLvModel> LanguageVersions { get; set; } = new Dictionary<LanguageEnum, AddressLvModel>();
    }

    public class AddressLvModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public LanguageEnum Language { get; set; }

        public string AdditionalInformation { get; set; }
        public string PoBox {get; set; }
        public string ForeignAddress {get; set; }
    }
}
