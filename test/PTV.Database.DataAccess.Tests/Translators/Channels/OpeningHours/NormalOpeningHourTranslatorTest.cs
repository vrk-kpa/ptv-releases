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
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.DataAccess.Translators.Channels.V2.OpeningHours;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Channels.OpeningHours
{
    public class NormalOpeningHourTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;
        private ItemListModelGenerator listModelGenerator;
        private TestConversion conversion;

        public NormalOpeningHourTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            var typesCacheMock = SetupTypesCacheMock<ServiceHourType>();
            listModelGenerator = new ItemListModelGenerator();
            conversion = new TestConversion();

            translators = new List<object>
            {
                new NormalOpeningHourTranslator(ResolveManager, TranslationPrimitives, new ServiceChannelTranslationDefinitionHelper()),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceHoursAdditionalInformation, VmStringText>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<DailyOpeningTime, VmDailyHourCommon>>(), unitOfWorkMock,
                    d => new DailyOpeningTime { DayFrom = (int) d.DayFrom }
                ),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceHours, VmOpeningHour>>(), unitOfWorkMock),
                RegisterTranslatorMock<DailyOpeningHoursEntitiesModel, VmNormalDailyOpeningHour>
                    (
                        model => null,
                        entity => new VmNormalDailyOpeningHour { Intervals = entity.Hours.Select(h => new VmDailyHourCommon { DayFrom = (WeekDayEnum?)h.DayFrom } ).ToList() }
                    ),
            };
            RegisterDbSet(new List<ServiceHours>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for ServiceChannelServiceHourTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7 }, false, "10.15.2018", "10.17.2018", true, new [] { WeekDayEnum.Monday, WeekDayEnum.Tuesday, WeekDayEnum.Wednesday})]
        [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7 }, false, "10.15.2018", "10.17.2018", false, null)]
        [InlineData(new[] { 1, 2, 3, 4, 5, 6, 7 }, false, "10.15.2018", "10.25.2018", true, null)]
        [InlineData(new[] { 0, 2, 3, 4 }, false, "10.15.2018", "10.16.2018", true, new [] { WeekDayEnum.Monday, WeekDayEnum.Tuesday})]
        [InlineData(new[] { 0, 2, 3, 4 }, false, "10.15.2018", "10.25.2018", true, null)]
        [InlineData(new [] { 1, 0, 0, 4 }, false, null, null, true, null)]
        [InlineData(new [] { -1 }, false, null, null, false, null)]
        [InlineData(new [] { 0 }, true, "12.13.2015", "12.23.2015", true, null)]
        [InlineData(new [] { -1 }, true, "12.13.2015", "12.23.2015", false, null)]
        [InlineData(null, false, null, null, true, null)]
        public void TranslateServiceChannelServiceHourToEntity(int[] days, bool nonstop, string dateFrom, string dateTo, bool isPeriod, WeekDayEnum[] filteredDays)
        {
            var model = TestHelper.CreateVmHoursModel<VmNormalHours>(ServiceHoursTypeEnum.Standard);
            model.DailyHours = days?.Select(
                (x, index) =>
                {
                    return new
                    {
                        Key = (WeekDayEnum) index,
                        Value = x >= 0 ?new VmNormalDailyOpeningHour
                        {
                            Active = true,
                            Intervals = listModelGenerator
                                .Create(x, i => new VmDailyHourCommon {DayFrom = (WeekDayEnum?) index}).ToList()
                        } : null
                    };
                }).ToDictionary(x => x.Key.ToString(), x => x.Value);

            model.IsPeriod = isPeriod;
            model.IsNonStop = nonstop;
            model.DateFrom = conversion.GetDate(dateFrom);
            model.DateTo = conversion.GetDate(dateTo);

            var translation = RunTranslationModelToEntityTest<VmNormalHours, ServiceHours>(translators, model, unitOfWorkMock);

            CheckTranslation(model, translation, filteredDays);
        }

        private static void CheckTranslation(VmNormalHours model, ServiceHours translation, WeekDayEnum[] filteredDays)
        {
            translation.Id.Should().NotBeEmpty();
            if (!model.IsNonStop)
            {
                var dailyHours = filteredDays != null
                    ? model.DailyHours
                        .Where(x => filteredDays.Select(w => w.ToString()).Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value)
                    : model.DailyHours;
                translation.DailyOpeningTimes.Count.Should().Be(dailyHours?.Sum(x => x.Value?.Intervals.Count) ?? 0);
                if (dailyHours != null)
                {
                    translation.DailyOpeningTimes.GroupBy(x => (WeekDayEnum) x.DayFrom).ForEach
                    (
                        x => x.Count().Should().Be(dailyHours[x.Key.ToString()].Intervals.Count)
                    );
                }
            }
            else
            {
                translation.DailyOpeningTimes.Count.Should().Be(0);
            }
        }

        /// <summary>
        /// test for ServiceChannelServiceHourTranslator entity - vm
        /// </summary>
        [Theory]
        [InlineData(new[] { 0 }, true)]
        [InlineData(new [] { 0, 1, 0, 2, 5 }, false)]
        [InlineData(new [] { 1, 2, 3, 4, 5, 6, 7 }, true)]
        [InlineData(new[] { 1 }, false)]
        public void TranslateServiceHoursToVm(int[] dailyCount, bool isNonstop)
        {
            var model = CreateModel();
            model.DailyOpeningTimes = dailyCount.SelectMany((x, index) => listModelGenerator.Create(x, i => new DailyOpeningTime { DayFrom = index })).ToList();
            model.IsNonStop = isNonstop;

            var translation = RunTranslationEntityToModelTest<ServiceHours, VmNormalHours>(translators, model);
            CheckTranslation(model, translation);
        }

        private void CheckTranslation(ServiceHours source, VmNormalHours translation)
        {
            translation.IsNonStop.Should().Be(source.IsNonStop);
            translation.DailyHours.Count.Should().Be(!source.IsNonStop ? 7 : source.DailyOpeningTimes.GroupBy(x => x.DayFrom).Count());
            foreach (var dailyHour in translation.DailyHours)
            {
                int day = (int)Enum.Parse<WeekDayEnum>(dailyHour.Key);
                (dailyHour.Value ?? new VmNormalDailyOpeningHour { Intervals = new List<VmDailyHourCommon>()}).Intervals.Count.Should()
                    .Be(source.DailyOpeningTimes.Count(x => x.DayFrom == day));
            }
        }

        private ServiceHours CreateModel()
        {
            return new ServiceHours
            {
                Id = Guid.NewGuid(),
                DailyOpeningTimes = new HashSet<DailyOpeningTime>()
            };
        }
    }
}
