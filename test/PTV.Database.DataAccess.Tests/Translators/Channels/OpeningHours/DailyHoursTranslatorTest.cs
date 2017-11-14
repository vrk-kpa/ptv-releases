/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
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
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Channels.OpeningHours
{
    public class DailyOpeningHourTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;
        private TestConversion conversion;
        private ItemListModelGenerator itemListGenerator;

        public DailyOpeningHourTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            translators = new List<object>
            {
                new DailyOpeningHourTranslator(ResolveManager, TranslationPrimitives),
                new DailyOpeningHourExtraTranslator(ResolveManager, TranslationPrimitives),
                new DailyHoursTranslator(ResolveManager, TranslationPrimitives)
//                RegisterTranslatorMock(new Mock<ITranslator<DailyOpeningTime, IDailyHours>>(), unitOfWorkMock),

            };

            RegisterDbSet(new List<DailyOpeningTime>(), unitOfWorkMockSetup);

            itemListGenerator = new ItemListModelGenerator();
            conversion = new TestConversion();
            itemListGenerator = new ItemListModelGenerator();
        }

        /// <summary>
        /// test for ElectronicChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(WeekDayEnum.Monday, 118800000, 115200000, 123800000, 125200000, null)]
        [InlineData(WeekDayEnum.Tuesday, 28800000, 20800000, 29800000, 30800000, DailyHoursExtraTypes.Hidden)]
        [InlineData(WeekDayEnum.Wednesday, 118800000, 115200000, 123800000, 125200000, DailyHoursExtraTypes.Vissible)]
        [InlineData(WeekDayEnum.Friday, 28800000, 20800000, 29800000, 30800000, DailyHoursExtraTypes.Vissible)]
        public void TranslateOpeningHours(WeekDayEnum dayOfWeek, long timeFrom, long timeTo, long timeFromExtra, long timeToExtra, DailyHoursExtraTypes? extra)
        {
            var model = CreateModel();
            model.DayFrom = dayOfWeek;
            model.TimeFrom = timeFrom;
            model.TimeFromExtra = timeFromExtra;
            model.TimeTo = timeTo;
            model.TimeToExtra = timeToExtra;
            model.Extra = extra;

            var toTranslate = new List<VmDailyHours>() { model };

            var translations = RunTranslationModelToEntityTest<VmDailyHours, DailyOpeningTime>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation, false);
        }

        [Theory]
        [InlineData(WeekDayEnum.Monday, WeekDayEnum.Tuesday, 118800000, 115200000, 123800000, 125200000, DailyHoursExtraTypes.Vissible)]
        [InlineData(WeekDayEnum.Tuesday, WeekDayEnum.Thursday, 28800000, 20800000, 29800000, 30800000, DailyHoursExtraTypes.Vissible)]
        [InlineData(WeekDayEnum.Wednesday, WeekDayEnum.Friday, 118800000, 115200000, 123800000, 125200000, DailyHoursExtraTypes.Vissible)]
        [InlineData(WeekDayEnum.Friday, WeekDayEnum.Sunday, 28800000, 20800000, 29800000, 30800000, DailyHoursExtraTypes.Vissible)]
        [InlineData(WeekDayEnum.Friday, null, 28800000, 20800000, 29800000, 30800000, DailyHoursExtraTypes.Hidden)]
        public void TranslateOpeningHoursExtra(WeekDayEnum dayOfWeek, WeekDayEnum? dayTo, long timeFrom, long timeTo, long timeFromExtra, long timeToExtra, DailyHoursExtraTypes? extra)
        {
            var model = CreateModel();
            model.DayFrom = dayOfWeek;
            model.DayTo = dayTo;
            model.TimeFrom = timeFrom;
            model.TimeFromExtra = timeFromExtra;
            model.TimeTo = timeTo;
            model.TimeToExtra = timeToExtra;
            model.Extra = extra;

            var toTranslate = new List<VmExtraHours>() { new VmExtraHours { Hours = model } };

            var translations = RunTranslationModelToEntityTest<VmExtraHours, DailyOpeningTime>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation, true);
        }

        /// <summary>
        /// test for ElectronicChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(0, 1, 480, 560, false)]
        [InlineData(1, 2,1680, 1820, false)]
        [InlineData(5, null, 480, 560, true)]
        [InlineData(6, null, 1680, 1820, true)]
        public void TranslateOpeningHoursToVm(int dayOfWeek, int? dayTo, int timeFrom, int timeTo, bool extra)
        {
            var model = CreateEntity();
            model.DayFrom = dayOfWeek;
            model.DayTo = dayTo;
            model.From = new TimeSpan(GetUnixTime(timeFrom) * 10000);
            model.To = new TimeSpan(GetUnixTime(timeTo) * 10000);

            var toTranslate = new List<DailyOpeningTime>() { model };

            var translations = RunTranslationEntityToModelTest<DailyOpeningTime, VmDailyHours>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }

        private void CheckTranslation(VmDailyHours model, DailyOpeningTime target, bool isExtra)
        {
            if (isExtra && model.Extra != DailyHoursExtraTypes.Vissible)
            {
                target.Should().BeNull();
            }
            else
            {
                long timeFrom = isExtra ? model.TimeFromExtra.Value : model.TimeFrom.Value;
                long timeTo = isExtra ? model.TimeToExtra.Value : model.TimeTo.Value;
                target.DayFrom.Should().Be((int)model.DayFrom);
                target.DayTo.Should().Be((int?)model.DayTo);
                target.From.Should().Be(new TimeSpan((timeFrom % (24 * 3600 * 1000)) * 10000));
                target.To.Should().Be(new TimeSpan((timeTo % (24 * 3600 * 1000)) * 10000));
            }
        }
        private void CheckTranslation(DailyOpeningTime model, VmDailyHours target)
        {
            ((int)target.DayFrom).Should().Be(model.DayFrom);
            target.Day.Should().BeTrue();
//            if (model.IsExtra)
//            {
//                target.TimeFromExtra.Should().Be(GetTime(model.From));
//                target.TimeToExtra.Should().Be(GetTime(model.To));
//                target.TimeFrom.Should().Be(0);
//                target.TimeTo.Should().Be(0);
//            }
//            else
//            {
                target.TimeFrom.Should().Be(GetTime(model.From));
                target.TimeTo.Should().Be(GetTime(model.To));
//                target.TimeFromExtra.HasValue.Should().BeFalse();
//                target.TimeToExtra.HasValue.Should().BeFalse();
//            }
        }

        private long GetTime(TimeSpan time)
        {
            return (time.Ticks % new TimeSpan(1, 0, 0, 0).Ticks + new TimeSpan(1, 0, 0, 0).Ticks) / 10000;
        }

        private long GetUnixTime(int minutes)
        {
            return minutes * 60 * 1000;
        }

        private VmDailyHours CreateModel()
        {
            return new VmDailyHours
            {

            };
        }

        private DailyOpeningTime CreateEntity()
        {
            return new DailyOpeningTime
            {
                OpeningHourId = Guid.NewGuid()

            };
        }
    }
}