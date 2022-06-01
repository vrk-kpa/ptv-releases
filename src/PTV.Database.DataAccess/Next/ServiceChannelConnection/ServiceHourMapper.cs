using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Next.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Next.ServiceChannelConnection
{
    internal interface IServiceHourMapper
    {
        OpeningHourModel Map(List<ServiceHours> hours, List<Guid> languageIds);
    }

    [RegisterService(typeof(IServiceHourMapper), RegisterType.Transient)]
    internal class ServiceHourMapper : IServiceHourMapper
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public ServiceHourMapper(ILanguageCache languageCache, ITypesCache typesCache)
        {
            this.languageCache = languageCache;
            this.typesCache = typesCache;
        }

        public OpeningHourModel Map(List<ServiceHours> hours, List<Guid> languageIds)
        {
            var model = new OpeningHourModel();
            model.StandardHours = GetStandardHours(hours, languageIds);
            model.SpecialHours = GetSpecialHours(hours, languageIds);
            model.HolidayHours = GetHolidayHours(hours);
            model.ExceptionalHours = GetExceptionalHours(hours, languageIds);
            return model;
        }

        private List<ExceptionalServiceHourModel> GetExceptionalHours(List<ServiceHours> hours, List<Guid> languageIds)
        {
            var serviceHourTypeId = typesCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Exception.ToString());
            return hours
                .Where(x => x.HolidayServiceHour == null && x.ServiceHourTypeId == serviceHourTypeId)
                .OrderBy(x => x.OrderNumber)
                .ThenBy(x => x.Created)
                .Select(m => MapToExceptionalHour(m, languageIds))
                .ToList();
        }


        private List<HolidayServiceHourModel> GetHolidayHours(List<ServiceHours> hours)
        {
            var serviceHourTypeId = typesCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Exception.ToString());
            return hours
                .Where(x => x.HolidayServiceHour != null && x.ServiceHourTypeId == serviceHourTypeId)
                .OrderBy(x => x.OrderNumber)
                .ThenBy(x => x.Created)
                .Select(MapToHolidayHour)
                .ToList();
        }

        private List<SpecialServiceHourModel> GetSpecialHours(List<ServiceHours> hours, List<Guid> languageIds)
        {
            var serviceHourTypeId = typesCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Special.ToString());
            return hours
                .Where(x => x.HolidayServiceHour == null && x.ServiceHourTypeId == serviceHourTypeId)
                .OrderBy(x => x.OrderNumber)
                .ThenBy(x => x.Created)
                .Select(m => MapToSpecialHour(m, languageIds))
                .ToList();
        }

        private ExceptionalServiceHourModel MapToExceptionalHour(ServiceHours source, List<Guid> languageIds)
        {
            var sh = new ExceptionalServiceHourModel();
            sh.Id = source.Id;
            sh.Type = typesCache.GetByValue<ServiceHourType>(source.ServiceHourTypeId).ToEnum<ServiceHoursTypeEnum>();            
            sh.IsClosed = source.IsClosed;
            sh.OpeningHoursFrom = source.OpeningHoursFrom.HasValue ? source.OpeningHoursFrom.Value.ToUtcDateWithoutConversion() : (DateTimeOffset?)null;
            sh.OpeningHoursTo = source.OpeningHoursTo.HasValue ? source.OpeningHoursTo.Value.ToUtcDateWithoutConversion() : (DateTimeOffset?)null;
            sh.OrderNumber = source.OrderNumber == null ? 0 : source.OrderNumber.Value;

            if (source.DailyOpeningTimes.Any())
            {
                var dot = source.DailyOpeningTimes.ElementAt(0);
                sh.TimeFrom = dot.From;
                sh.TimeTo = dot.To;
            }

            var languages = Helpers.ToLanguageList(languageCache, languageIds);
            sh.LanguageVersions = languages
                .Select(x => Map(source, x.LanguageId))
                .ToDictionary(x => x.Language);

            return sh;
        }

        private HolidayServiceHourModel MapToHolidayHour(ServiceHours source)
        {
            var holiday = source.HolidayServiceHour;
            var sh = new HolidayServiceHourModel();
            sh.Id = source.Id;
            sh.Code = holiday.Holiday.Code.ToEnum<HolidayEnum>();
            sh.IsClosed = source.IsClosed;
            sh.Type = typesCache.GetByValue<ServiceHourType>(source.ServiceHourTypeId).ToEnum<ServiceHoursTypeEnum>();

            if (!source.IsClosed)
            {
                var dot = source.DailyOpeningTimes.ElementAt(0);
                sh.From = dot.From;
                sh.To = dot.To;
            }

            return sh;
        }

        private SpecialServiceHourModel MapToSpecialHour(ServiceHours source, List<Guid> languageIds)
        {
            var sh = new SpecialServiceHourModel();
            sh.Id = source.Id;
            sh.Type = typesCache.GetByValue<ServiceHourType>(source.ServiceHourTypeId).ToEnum<ServiceHoursTypeEnum>();        
            sh.OpeningHoursFrom = source.OpeningHoursFrom.HasValue ? source.OpeningHoursFrom.Value.ToUtcDateWithoutConversion() : (DateTimeOffset?)null;            
            sh.OpeningHoursTo = source.OpeningHoursTo.HasValue ? source.OpeningHoursTo.Value.ToUtcDateWithoutConversion() : (DateTimeOffset?)null;
            sh.IsPeriod = source.OpeningHoursTo.HasValue;
            sh.OrderNumber = source.OrderNumber == null ? 0 : source.OrderNumber.Value;

            var dot = source.DailyOpeningTimes.ElementAt(0);
            sh.DayFrom = (WeekDayEnum) dot.DayFrom;
            sh.DayTo = dot.DayTo.HasValue ? (WeekDayEnum) dot.DayTo.Value : (WeekDayEnum?)null;
            sh.TimeFrom = dot.From;
            sh.TimeTo = dot.To;

            var languages = Helpers.ToLanguageList(languageCache, languageIds);

            sh.LanguageVersions = languages
                .Select(x => Map(source, x.LanguageId))
                .ToDictionary(x => x.Language);

            return sh;
        }

        private List<StandardServiceHourModel> GetStandardHours(List<ServiceHours> hours, List<Guid> languageIds)
        {
            var serviceHourTypeId = typesCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Standard.ToString());
            return hours
                .Where(x => x.HolidayServiceHour == null && x.ServiceHourTypeId == serviceHourTypeId)
                .OrderBy(x => x.OrderNumber)
                .ThenBy(x => x.Created)
                .Select(m => MapToStandardHour(m, languageIds))
                .ToList();
        }

        private StandardServiceHourModel MapToStandardHour(ServiceHours source, List<Guid> languageIds)
        {
            var sh = new StandardServiceHourModel();
            sh.Id = source.Id;
            sh.Type = typesCache.GetByValue<ServiceHourType>(source.ServiceHourTypeId).ToEnum<ServiceHoursTypeEnum>();            
            sh.IsNonStop = source.IsNonStop;
            sh.IsReservation = source.IsReservation;
            sh.IsPeriod = source.OpeningHoursTo.HasValue;
            sh.OpeningHoursFrom = source.OpeningHoursFrom.HasValue ? source.OpeningHoursFrom.Value.ToUtcDateWithoutConversion() : (DateTimeOffset?)null;
            sh.OpeningHoursTo = source.OpeningHoursTo.HasValue ? source.OpeningHoursTo.Value.ToUtcDateWithoutConversion() : (DateTimeOffset?)null;
            sh.OrderNumber = source.OrderNumber == null ? 0 : source.OrderNumber.Value;
            sh.DailyOpeningTimes = Map(source.DailyOpeningTimes);

            var languages = Helpers.ToLanguageList(languageCache, languageIds);

            sh.LanguageVersions = languages
                .Select(x => Map(source, x.LanguageId))
                .ToDictionary(x => x.Language);

            return sh;
        }

        private ServiceHourLvModel Map(ServiceHours source, Guid languageId)
        {
            var lv = new ServiceHourLvModel();
            lv.Language = Helpers.MapLanguage(languageCache, languageId);
            var info = source.AdditionalInformations.FirstOrDefault(x => x.LocalizationId == languageId)?.Text;
            lv.AdditionalInformation = info.NullToEmpty();
            return lv;
        }

        private Dictionary<WeekDayEnum, DailyOpeningTimeModel> Map(ICollection<DailyOpeningTime> source)
        {
            // Since each day can have multiple opening times
            // (e.g. 08-16, 21-22) group by day to get result like
            // Monday:
            //      08-16
            //      21-22
            // Tuesday...
            return source.GroupBy(x => x.DayFrom)
                .ToDictionary(x => (WeekDayEnum)x.Key, v => MapDay( (WeekDayEnum)v.Key, v.ToList()));
        }

        private DailyOpeningTimeModel MapDay(WeekDayEnum day, List<DailyOpeningTime> openingTimes)
        {
            var source = openingTimes[0];

            var model = new DailyOpeningTimeModel();
            model.Day = day;
            model.Times = openingTimes.Select(x => new FromTo
            {
                From = x.From,
                To = x.To
            }).ToList();
            return model;
        }
    }
}
