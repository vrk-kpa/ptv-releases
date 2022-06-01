using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Domain.Model.Models.V2.Common.Connections;
using PTV.Framework;
using PTV.Next.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Models;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Interfaces.DbContext;

namespace PTV.Database.DataAccess.Next.ServiceChannelConnection
{
    public interface ISaveConnectionMapper
    {
        VmConnectionsInput Map(ConnectionModel source, List<VmConnectionOutput> allConnections);
    }

    [RegisterService(typeof(ISaveConnectionMapper), RegisterType.Transient)]
    internal class SaveConnectionMapper : ISaveConnectionMapper
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly IDialCodeCache dialCodeCache;
        private readonly IMunicipalityCache municipalityCache;
        private readonly IPostalCodeCache postalCodeCache;
        private readonly ICountryCache countryCache;
        private readonly Interfaces.Services.IStreetService streetService;
        private readonly IContextManager contextManager;

        public SaveConnectionMapper(ITypesCache typesCache,
            ILanguageCache languageCache,
            IDialCodeCache dialCodeCache,
            IMunicipalityCache municipalityCache,
            IPostalCodeCache postalCodeCache,
            ICountryCache countryCache,
            Interfaces.Services.IStreetService streetService,
            IContextManager contextManager)
        {
            this.typesCache = typesCache;
            this.languageCache = languageCache;
            this.dialCodeCache = dialCodeCache;
            this.municipalityCache = municipalityCache;
            this.postalCodeCache = postalCodeCache;
            this.countryCache = countryCache;
            this.streetService = streetService;
            this.contextManager = contextManager;
        }

        public VmConnectionsInput Map(ConnectionModel source, List<VmConnectionOutput> allConnections)
        {
            // Take all the existing connections except the one we want to change
            var filtered = allConnections
                .Where(x => x.UnificRootId != source.ServiceChannelUnificRootId)
                .Select(ConvertToInputModel).ToList();

            filtered.Add(MapConnection(source));

            var result = new VmConnectionsInput();
            result.IsAsti = source.IsASTIConnection;
            result.Id = source.ServiceId;
            result.SelectedConnections = filtered;
            result.UseOrder = false;
            return result;
        }

        private VmConnectionInput ConvertToInputModel(VmConnectionOutput source)
        {
            return new VmConnectionInput
            {
                ConnectedEntityId = source.UnificRootId,
                BasicInformation = source.BasicInformation,
                DigitalAuthorization = MapOldDigitalAuthorizations(source.DigitalAuthorization),
                ContactDetails = source.ContactDetails,
                OpeningHours = source.OpeningHours,
            };
        }

        private VmConnectionDigitalAuthorizationInput MapOldDigitalAuthorizations(
            VmConnectionDigitalAuthorizationOutput source)
        {
            if (source == null) return null;
            return new VmConnectionDigitalAuthorizationInput
            {
                DigitalAuthorizations = source.DigitalAuthorizations.Select(x => x.Id).ToList()
            };
        }

        private VmConnectionInput MapConnection(ConnectionModel source)
        {
            var result = new VmConnectionInput();
            result.ConnectedEntityId = source.ServiceChannelUnificRootId;
            result.OpeningHours = MapOpeningHours(source);
            result.BasicInformation = new VmConnectionBasicInformation
            {
                AdditionalInformation =
                    source.LanguageVersions.ToDictionary(x => x.Key.ToString(), a => a.Value.Charge.Info),
                ChargeType = source.ChargeType.HasValue
                    ? typesCache.Get<ServiceChargeType>(source.ChargeType.Value.ToString())
                    : (Guid?) null,
                Description = source.LanguageVersions.ToDictionary(x => x.Key.ToString(), a => a.Value.Description)
            };
            result.DigitalAuthorization = new VmConnectionDigitalAuthorizationInput
            {
                DigitalAuthorizations = source.DigitalAuthorizations
            };
            result.ContactDetails = new VmContactDetails
            {
                Emails = MapEmails(source.Emails),
                FaxNumbers = MapFaxNumbers(source.FaxNumbers),
                PhoneNumbers = MapPhoneNumbers(source.PhoneNumbers),
                PostalAddresses = MapAddresses(source.Addresses),
                WebPages = MapWebPages(source.WebPages),
            };

            return result;
        }

        private List<VmAddressSimple> MapAddresses(List<AddressModel> source)
        {
            var result = new List<VmAddressSimple>();

            foreach (var item in source)
            {
                var address = new VmAddressSimple();
                address.StreetType = item.Type.ToString();
                address.Street = GetStreet(item);
                address.StreetName = GetStreetName(item);
                address.StreetNumber = item.StreetNumber;
                address.StreetNumberRange = GetStreetNumberRange(item);
                address.AdditionalInformation =
                    item.LanguageVersions.ToDictionary(x => x.Key.ToString(), v => v.Value.AdditionalInformation);
                address.Municipality = GetMunicipalityId(item);
                address.PostalCode = GetPostalCode(item);
                address.Coordinates = new List<VmCoordinate>();
                address.PoBox = GetPoBox(item);
                address.ForeignAddressText = GetForeignAddressText(item);
                address.Country = GetCountry(item);
                result.Add(address);
            }

            return result;
        }

        private Dictionary<string, string> GetPoBox(AddressModel model)
        {
            if (model.Type != AddressTypeEnum.PostOfficeBox) return null;
            return model.LanguageVersions.ToDictionary(x => x.Key.ToString(), v => v.Value.PoBox);
        }

        private VmCountry GetCountry(AddressModel model)
        {
            if (model.Type != AddressTypeEnum.Foreign) return null;
            var country = countryCache.GetByCode(model.CountryCode);

            return new VmCountry
            {
                Id = country.Id,
                Code = country.Code
            };
        }

        private Dictionary<string, string> GetForeignAddressText(AddressModel model)
        {
            if (model.Type != AddressTypeEnum.Foreign) return null;
            return model.LanguageVersions.ToDictionary(x => x.Key.ToString(), v => v.Value.ForeignAddress);
        }

        private VmStreetNumber GetStreetNumberRange(AddressModel model)
        {
            if (model.Street == null || model.Type != AddressTypeEnum.Street) return null;

            var postalCodeId = postalCodeCache.GuidByCode(model.PostalCode);

            var id = model.Street.Id.HasValue
                ? contextManager.ExecuteReader(unitOfWork =>
                    streetService.GetStreetNumberRangeId(unitOfWork, postalCodeId, model.StreetNumber,
                        model.Street.Id.Value))
                : null;

            if (!id.HasValue) return null;

            return new VmStreetNumber
            {
                Id = id.Value
            };
        }

        private Dictionary<string, string> GetStreetName(AddressModel address)
        {
            if (address.Type != AddressTypeEnum.Street || address.StreetName == null)
            {
                return null;
            }

            // If the user has selected existing address, use the names from that
            // This is because deep inside the translation layer (StreetTranslator)
            // there is code that uses those names to find the address. If the address
            // is not found then it creates new address into the database.
            if (address.Street.Id.HasValue && address.Street.Id != Guid.Empty)
            {
                return address.Street.Names.ToDictionary(x => x.Key.ToString().ToLowerInvariant(), v => v.Value);
            }
            
            return address.StreetName.ToDictionary(x => x.Key.ToString().ToLowerInvariant(), v => v.Value);
        }

        private VmPostalCode GetPostalCode(AddressModel address)
        {
            if (string.IsNullOrEmpty(address.PostalCode)) return null;

            var postalCodeId = postalCodeCache.GuidByCode(address.PostalCode);

            return new VmPostalCode {Id = postalCodeId};
        }

        private Guid? GetMunicipalityId(AddressModel address)
        {
            if (address.Type != AddressTypeEnum.Street) return null;

            var code = address.Street?.MunicipalityCode;
            if (!string.IsNullOrEmpty(code))
            {
                return municipalityCache.Get(code).Id;
            }

            // If user writes address that is not found, we don't know
            // the municipality code as address.Street is missing. Use postal
            // code to find it
            return municipalityCache.GetByPostalCode(address.PostalCode).Id;
        }

        private VmStreet GetStreet(AddressModel address)
        {
            if (address.Type != AddressTypeEnum.Street || address.Street == null ||
                !address.Street.Id.HasValue) return null;
            return new VmStreet {Id = address.Street.Id.Value};
        }

        private VmOpeningHours MapOpeningHours(ConnectionModel source)
        {
            return new VmOpeningHours
            {
                StandardHours = MapStandardHours(source.OpeningHours.StandardHours),
                SpecialHours = MapSpecialHours(source.OpeningHours.SpecialHours),
                HolidayHours = MapHolidayHours(source.OpeningHours.HolidayHours),
                ExceptionHours = MapExceptionalHours(source.OpeningHours.ExceptionalHours),
            };
        }

        private List<VmExceptionalHours> MapExceptionalHours(List<ExceptionalServiceHourModel> source)
        {
            var result = new List<VmExceptionalHours>();

            foreach (var sourceHour in source)
            {
                var isOpen = !sourceHour.IsClosed;

                var hour = new VmExceptionalHours();
                hour.Id = sourceHour.Id;
                hour.IsPeriod = sourceHour.OpeningHoursTo.HasValue;
                hour.ClosedForPeriod = sourceHour.IsClosed;
                hour.OpeningPeriod.TimeFrom = isOpen ? sourceHour.TimeFrom.ToEpochTimeOfDay() : (long?) null;
                hour.OpeningPeriod.TimeTo = isOpen ? sourceHour.TimeTo.ToEpochTimeOfDay() : (long?) null;
                hour.DateFrom = sourceHour.OpeningHoursFrom.ToEpochTime();
                hour.DateTo = sourceHour.OpeningHoursTo.ToEpochTime();
                hour.ServiceHoursType = sourceHour.Type;
                hour.Name = sourceHour.LanguageVersions.ToDictionary(x => x.Key.ToString(),
                    v => v.Value.AdditionalInformation);
                result.Add(hour);
            }

            return result;
        }

        private List<VmHolidayHours> MapHolidayHours(List<HolidayServiceHourModel> source)
        {
            var result = new List<VmHolidayHours>();

            foreach (var sourceHour in source)
            {
                var hour = new VmHolidayHours();

                // If you map the Id, it does not work. It seems translation layer
                // removes all the existing rows and generates new ones

                hour.Active = true;
                hour.Code = sourceHour.Code.ToString();
                hour.ServiceHoursType = sourceHour.Type;
                hour.IsClosed = sourceHour.IsClosed;
                hour.Intervals = MapHolidayIntervals(sourceHour);
                result.Add(hour);
            }

            return result;
        }

        private List<VmDailyHourCommon> MapHolidayIntervals(HolidayServiceHourModel source)
        {
            var result = new List<VmDailyHourCommon>();

            if (source.IsClosed) return result;

            result.Add(new VmDailyHourCommon
            {
                TimeFrom = source.From.ToEpochTimeOfDay(),
                TimeTo = source.To.ToEpochTimeOfDay()
            });

            return result;
        }

        private List<VmSpecialHours> MapSpecialHours(List<SpecialServiceHourModel> source)
        {
            var result = new List<VmSpecialHours>();

            foreach (var sourceHour in source)
            {
                var hour = new VmSpecialHours();
                hour.Id = sourceHour.Id;
                hour.ServiceHoursType = sourceHour.Type;
                hour.Name = sourceHour.LanguageVersions.ToDictionary(x => x.Key.ToString(),
                    v => v.Value.AdditionalInformation);
                hour.IsPeriod = sourceHour.IsPeriod;
                hour.OpeningPeriod.DayFrom = sourceHour.DayFrom;
                hour.OpeningPeriod.DayTo = sourceHour.DayTo;
                hour.OpeningPeriod.TimeFrom = sourceHour.TimeFrom.ToEpochTimeOfDay();
                hour.OpeningPeriod.TimeTo = sourceHour.TimeTo.ToEpochTimeOfDay();
                hour.DateFrom = sourceHour.OpeningHoursFrom.ToEpochTime();
                hour.DateTo = sourceHour.OpeningHoursTo.ToEpochTime();
                result.Add(hour);
            }

            return result;
        }

        private List<VmNormalHours> MapStandardHours(List<StandardServiceHourModel> source)
        {
            var result = new List<VmNormalHours>();

            foreach (var sourceHour in source)
            {
                var hour = new VmNormalHours();
                hour.Id = sourceHour.Id;
                hour.Name = sourceHour.LanguageVersions.ToDictionary(x => x.Key.ToString(),
                    v => v.Value.AdditionalInformation);
                hour.DailyHours = MapDailyHours(sourceHour.DailyOpeningTimes);
                hour.IsNonStop = sourceHour.IsNonStop;
                hour.IsReservation = sourceHour.IsReservation;
                hour.IsPeriod = sourceHour.IsPeriod;
                hour.ServiceHoursType = sourceHour.Type;
                hour.DateFrom = sourceHour.OpeningHoursFrom.ToEpochTime();
                hour.DateTo = sourceHour.OpeningHoursTo.ToEpochTime();
                result.Add(hour);
            }

            return result;
        }

        private Dictionary<string, VmNormalDailyOpeningHour> MapDailyHours(
            Dictionary<WeekDayEnum, DailyOpeningTimeModel> source)
        {
            var result = new Dictionary<string, VmNormalDailyOpeningHour>();

            foreach (var kvp in source)
            {
                var hour = new VmNormalDailyOpeningHour();
                hour.Active = true;
                hour.Intervals = kvp.Value.Times.Select(x => new VmDailyHourCommon
                {
                    DayFrom = kvp.Key,
                    TimeFrom = x.From.ToEpochTimeOfDay(),
                    TimeTo = x.To.ToEpochTimeOfDay()
                }).ToList();
                result.Add(kvp.Key.ToString().ToLower(), hour);
            }

            return result;
        }

        private Dictionary<string, List<VmPhone>> MapPhoneNumbers(
            Dictionary<LanguageEnum, List<PhoneNumberLvModel>> source)
        {
            var result = new Dictionary<string, List<VmPhone>>();

            foreach (var kvp in source)
            {
                var items = kvp.Value.Select(x => new VmPhone
                {
                    Id = x.Id,
                    Number = x.Number,
                    IsLocalNumber = !x.DialCodeId.HasValue,
                    ChargeType = typesCache.Get<ServiceChargeType>(x.ChargeType.ToString()),
                    ChargeDescription = x.ChargeDescription,
                    AdditionalInformation = x.AdditionalInformation,
                    DialCode = MapDialCode(x.DialCodeId),
                    TypeId = typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString()),
                    LanguageId = languageCache.Get(kvp.Key.ToString())
                }).ToList();

                if (items.Any())
                {
                    result.Add(kvp.Key.ToString(), items);
                }
            }

            return result;
        }

        private Dictionary<string, List<VmPhone>> MapFaxNumbers(Dictionary<LanguageEnum, List<FaxNumberLvModel>> source)
        {
            var result = new Dictionary<string, List<VmPhone>>();

            foreach (var kvp in source)
            {
                var items = kvp.Value.Select(x => new VmPhone
                {
                    Id = x.Id,
                    Number = x.Number,
                    IsLocalNumber = !x.DialCodeId.HasValue,
                    ChargeType = typesCache.Get<ServiceChargeType>(ServiceChargeTypeEnum.Charged.ToString()),
                    DialCode = MapDialCode(x.DialCodeId),
                    TypeId = typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString()),
                    LanguageId = languageCache.Get(kvp.Key.ToString())
                }).ToList();

                if (items.Any())
                {
                    result.Add(kvp.Key.ToString(), items);
                }
            }

            return result;
        }

        private VmDialCode MapDialCode(Guid? dialCodeId)
        {
            if (!dialCodeId.HasValue) return null;

            return new VmDialCode
            {
                Id = dialCodeId.Value
            };
        }

        private Dictionary<string, List<VmEmailData>> MapEmails(Dictionary<LanguageEnum, List<EmailLvModel>> source)
        {
            var result = new Dictionary<string, List<VmEmailData>>();

            foreach (var kvp in source)
            {
                var emails = kvp.Value.Select(x => new VmEmailData
                {
                    Id = x.Id,
                    Email = x.Value,
                    LanguageId = languageCache.Get(kvp.Key.ToString())
                }).ToList();

                if (emails.Any())
                {
                    result.Add(kvp.Key.ToString(), emails);
                }
            }

            return result;
        }

        private Dictionary<string, List<VmWebPage>> MapWebPages(Dictionary<LanguageEnum, List<WebPageLvModel>> source)
        {
            var result = new Dictionary<string, List<VmWebPage>>();

            foreach (var kvp in source)
            {
                var items = kvp.Value.Select(x => new VmWebPage
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlAddress = x.Url,
                    Description = x.AdditionalInformation,
                    LocalizationId = languageCache.Get(kvp.Key.ToString())
                }).ToList();

                if (items.Any())
                {
                    result.Add(kvp.Key.ToString(), items);
                }
            }

            return result;
        }
    }
}