/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using PTV.Domain.Model.Enums;
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IHolidayService), RegisterType.Transient)]
    internal class HolidayService : IHolidayService
    {
        private readonly IContextManager contextManager;
        private readonly ITranslationViewModel translationManagerToEntity;
        private readonly HolidayConfiguration holidayConfiguration;

        public HolidayService(IContextManager contextManager, ITranslationViewModel translationManagerToEntity, ApplicationConfiguration configuration)
        {
            this.contextManager = contextManager;
            this.translationManagerToEntity = translationManagerToEntity;
            this.holidayConfiguration = configuration.GetHolidayConfiguration();
        }

        public void SeedNewHolidayDates()
        {
            var requiredNumberOfYears = holidayConfiguration?.GenerateForRequiredNumberOfYears > 0
                ? holidayConfiguration.GenerateForRequiredNumberOfYears
                : 3; //default
            var startYear = DateTime.UtcNow.Year;
            var requiredYears = Enumerable.Range(startYear, requiredNumberOfYears).ToArray();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                var holidayRepo = unitOfWork.CreateRepository<IHolidayRepository>();
                var holidays = holidayRepo.All()
                    .Where(y => y.IsValid)
                    .ToList();

                var vmHolidays = new List<VmHolidayDateListItem>();
                foreach (var requiredYear in requiredYears)
                {
                    vmHolidays.AddRange(holidays.Select(holiday =>
                            new VmHolidayDateListItem()
                            {
                                Date = holiday.Date.HasValue
                                    ? new DateTime(requiredYear, holiday.Date.Value.Month, holiday.Date.Value.Day)
                                    : CalculateHoliday(holiday.Code, requiredYear),
                                HolidayId = holiday.Id
                            })
                        .ToList());

                }
                translationManagerToEntity.TranslateAll<VmHolidayDateListItem, HolidayDate>(vmHolidays, unitOfWork);
                unitOfWork.Save(SaveMode.AllowAnonymous);
            });
        }

        private DateTime CalculateHoliday(string holidayCode, int calculateYear)
        {
            var holiday = ParseEnum<HolidayEnum>(holidayCode);
            switch (holiday)
            {
                case HolidayEnum.MaundyThursday:
                    return CalculateEasterSunday(calculateYear).AddDays(-3);
                case HolidayEnum.GoodFriday:
                    return CalculateEasterSunday(calculateYear).AddDays(-2);
                case HolidayEnum.EasterSunday:
                    return CalculateEasterSunday(calculateYear);
                case HolidayEnum.EasterMonday:
                    return CalculateEasterSunday(calculateYear).AddDays(1);
                case HolidayEnum.AscensionDay:
                    return CalculateEasterSunday(calculateYear).AddDays(39);
                case HolidayEnum.WhitSunday:
                    return CalculateEasterSunday(calculateYear).AddDays(49);
                case HolidayEnum.MidsummerEve:
                    return CalculateMidsummerDay(calculateYear).AddDays(-1);
                case HolidayEnum.MidsummerDay:
                    return CalculateMidsummerDay(calculateYear);
                case HolidayEnum.AllSaintsDay:
                    return CalculateAllSaintsDay(calculateYear);
                default: return DateTime.MinValue;
            }
        }


        private DateTime CalculateMidsummerDay(int year)
        {
            var start = new DateTime(year,6,20);
            var end = new DateTime(year, 6, 26);
            return GetDateOfRange(DayOfWeek.Saturday, start, end);
        }

        private DateTime CalculateAllSaintsDay(int year)
        {
            var start = new DateTime(year,10,31);
            var end = new DateTime(year, 11, 6);
            return GetDateOfRange(DayOfWeek.Saturday, start, end);
        }

        private DateTime CalculateEasterSunday(int year)
        {
            int day = 0;
            int month = 0;

            int g = year % 19;
            int c = year / 100;
            int h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25) + 19 * g + 15) % 30;
            int i = h - (int)(h / 28) * (1 - (int)(h / 28) * (int)(29 / (h + 1)) * (int)((21 - g) / 11));

            day   = i - ((year + (int)(year / 4) + i + 2 - c + (int)(c / 4)) % 7) + 28;
            month = 3;

            if (day > 31)
            {
                month++;
                day -= 31;
            }

            var result = new DateTime(year, month, day);
            return result;
        }

        private DateTime GetDateOfRange(DayOfWeek dayOfWeek, DateTime start, DateTime end)
        {
            var dates = new List<DateTime>();
            for (var dt = start; dt <= end; dt = dt.AddDays(1))
            {
                dates.Add(dt);
            }
            return dates.FirstOrDefault(x => x.DayOfWeek == dayOfWeek);
        }

        private T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
