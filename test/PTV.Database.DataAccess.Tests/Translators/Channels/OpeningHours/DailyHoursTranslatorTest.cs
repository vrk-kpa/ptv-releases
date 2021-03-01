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
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.DataAccess.Translators.Channels.V2.OpeningHours;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Channels.OpeningHours
{
    public class DailyOpeningHourTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;
        private TestConversion conversion;

        public DailyOpeningHourTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            translators = new List<object>
            {
                new DailyOpeningHourCommonTranslator(ResolveManager, TranslationPrimitives),
//                RegisterTranslatorMock(new Mock<ITranslator<DailyOpeningTime, IDailyHours>>(), unitOfWorkMock),

            };

            RegisterDbSet(new List<DailyOpeningTime>(), unitOfWorkMockSetup);

            conversion = new TestConversion();
        }

        /// <summary>
        /// test for ElectronicChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(WeekDayEnum.Monday, WeekDayEnum.Monday, 118800000, 115200000)]
        [InlineData(WeekDayEnum.Tuesday, null, 28800000, 20800000)]
        [InlineData(WeekDayEnum.Wednesday, WeekDayEnum.Monday, 118800000, 115200000)]
        [InlineData(WeekDayEnum.Friday, null, 28800000, 20800000)]
        public void TranslateOpeningHours(WeekDayEnum dayFrom, WeekDayEnum? dayTo, long timeFrom, long timeTo)
        {
            var model = CreateModel();
            model.DayFrom = dayFrom;
            model.DayTo = dayTo;
            model.TimeFrom = timeFrom;
            model.TimeTo = timeTo;

            var translation = RunTranslationModelToEntityTest<VmDailyHourCommon, DailyOpeningTime>(translators, model, unitOfWorkMock);
            CheckTranslation(model, translation);
        }

        private void CheckTranslation(VmDailyHourCommon model, DailyOpeningTime target)
        {
            long timeFrom = conversion.GetTicks(model.TimeFrom.Value % conversion.GetUnixTime(24 * 60));
            long timeTo = conversion.GetTicks(model.TimeTo.Value % conversion.GetUnixTime(24 * 60));
            target.DayFrom.Should().Be((int)model.DayFrom);
            target.DayTo.Should().Be((int?)model.DayTo);
            target.From.Should().Be(new TimeSpan(timeFrom));
            target.To.Should().Be(new TimeSpan(timeTo));

        }

        /// <summary>
        /// test for ElectronicChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(0, 1, 480, 560)]
        [InlineData(1, 2, 1680, 1820)]
        [InlineData(5, null, 480, 560)]
        [InlineData(6, null, 1680, 1820)]
        public void TranslateOpeningHoursToVm(int dayFrom, int? dayTo, int timeFrom, int timeTo)
        {
            var model = CreateEntity();
            model.DayFrom = dayFrom;
            model.DayTo = dayTo;
            model.From = new TimeSpan(conversion.GetTicksFromMinutes(timeFrom));
            model.To = new TimeSpan(conversion.GetTicksFromMinutes(timeTo));

            var toTranslate = new List<DailyOpeningTime> { model };

            var translations = RunTranslationEntityToModelTest<DailyOpeningTime, VmDailyHourCommon>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }

        private void CheckTranslation(DailyOpeningTime model, VmDailyHourCommon target)
        {
            ((int?) target.DayFrom).Should().Be(model.DayFrom);
            ((int?) target.DayTo).Should().Be(model.DayTo ?? model.DayFrom);
            target.TimeFrom.Should().Be(conversion.GetTime(model.From) % conversion.GetUnixTime(24 * 60));
            target.TimeTo.Should().Be(conversion.GetTime(model.To) % conversion.GetUnixTime(24 * 60));
        }

        private VmDailyHourCommon CreateModel()
        {
            return new VmDailyHourCommon
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
