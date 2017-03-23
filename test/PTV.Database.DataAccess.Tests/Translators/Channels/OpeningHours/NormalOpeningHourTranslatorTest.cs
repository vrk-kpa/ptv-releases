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
using System.Reflection.Metadata;
using FluentAssertions;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Channels.OpeningHours
{
    public class NormalOpeningHourTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;
        private ItemListModelGenerator listModelGenerator;

        public NormalOpeningHourTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            var typesCacheMock = SetupTypesCacheMock<ServiceHourType>();
            listModelGenerator = new ItemListModelGenerator();

            translators = new List<object>
            {
                new NormalOpeningHourTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object, new ServiceChannelTranslationDefinitionHelper(CacheManager)),
                new ServiceChannelServiceHourTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceHoursAdditionalInformation, VmStringText>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<DailyOpeningTime, VmDailyHours>>(), unitOfWorkMock,
                    d => new DailyOpeningTime { IsExtra = false, CreatedBy = "daily"},
                    dot => new VmDailyHours { DayFrom = (WeekDayEnum) Enum.ToObject(typeof(WeekDayEnum), dot.DayFrom), Extra = dot.IsExtra ? DailyHoursExtraTypes.Vissible : DailyHoursExtraTypes.Hidden, TimeFrom = dot.From.ToEpochTimeOfDay(), TimeTo = dot.To.ToEpochTimeOfDay() }
                ),
                RegisterTranslatorMock(new Mock<ITranslator<DailyOpeningTime, VmExtraHours>>(), unitOfWorkMock, d => new DailyOpeningTime { IsExtra = true, CreatedBy = "extra"}),
            };
            RegisterDbSet(new List<ServiceChannelServiceHours>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for ServiceChannelServiceHourTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(ServiceHoursTypeEnum.Standard, 3, 2, false)]
        [InlineData(ServiceHoursTypeEnum.Exception, 6, 0, false)]
        [InlineData(ServiceHoursTypeEnum.Special, 0, 0, false)]
        [InlineData(ServiceHoursTypeEnum.Standard, 4, 4, false)]
        [InlineData(ServiceHoursTypeEnum.Standard, 0, 0, true)]
        [InlineData(ServiceHoursTypeEnum.Standard, 5, 2, true)]
        public void TranslateServiceChannelServiceHourToEntity(ServiceHoursTypeEnum serviceHoursType, int dailyCount, int extraCount, bool nonstop)
        {
            var model = TestHelper.CreateVmHoursModel<VmNormalHours>(serviceHoursType);
            model.ServiceHoursType = serviceHoursType;
            model.DailyHours = listModelGenerator.Create(dailyCount, i =>
                new VmDailyHours
                {
                    Extra = i < extraCount ? DailyHoursExtraTypes.Vissible : DailyHoursExtraTypes.Hidden
                });
            model.DailyHours.Count.Should().Be(dailyCount);
            model.DailyHours.Count(x => x.Extra == DailyHoursExtraTypes.Vissible).Should().Be(extraCount);
            model.Nonstop = nonstop;

            var toTranslate = new List<VmNormalHours> { model };

            var translations = RunTranslationModelToEntityTest<VmNormalHours, ServiceChannelServiceHours>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation, dailyCount, extraCount);
        }

        private static void CheckTranslation(VmNormalHours model, ServiceChannelServiceHours translation, int normalCount, int extraCount)
        {
            translation.Id.Should().NotBeEmpty();
//            translation.ServiceHourTypeId.Should().Be(model.ServiceHoursType.ToString().GetGuid());
            translation.ValidFrom.Should().Be(model.ValidFrom.FromEpochTime());
            if (!model.Nonstop)
            {
                translation.DailyOpeningTimes.Count.Should().Be(normalCount + extraCount);
                translation.DailyOpeningTimes.Count(x => !x.IsExtra).Should().Be(normalCount);
                translation.DailyOpeningTimes.Count(x => x.IsExtra).Should().Be(extraCount);
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
        [InlineData(ServiceHoursTypeEnum.Standard, 0, 0)]
        [InlineData(ServiceHoursTypeEnum.Special, 2, 1)]
        [InlineData(ServiceHoursTypeEnum.Exception, 3, 0)]
        [InlineData(ServiceHoursTypeEnum.Exception, 3, 3)]
        public void TranslateServiceHoursToVm(ServiceHoursTypeEnum hoursType, int dailyCount, int extraCount)
        {
            var model = CreateModel(hoursType);
            var list = listModelGenerator.Create(dailyCount, i => new DailyOpeningTime { IsExtra = false, DayFrom = i, From = GetTime(i, false, true), To = GetTime(i, false, false)});
            list.AddRange(listModelGenerator.Create(extraCount, i => new DailyOpeningTime { IsExtra = true, DayFrom = i, From = GetTime(i, true, true), To = GetTime(i, true, false)}));
            model.DailyOpeningTimes = list;
            model.DailyOpeningTimes.Count.Should().Be(dailyCount + extraCount);
            var toTranslate = new List<ServiceChannelServiceHours> { model };

            var translations = RunTranslationEntityToModelTest<ServiceChannelServiceHours, VmNormalHours>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation, dailyCount, extraCount);
        }

        private void CheckTranslation(ServiceChannelServiceHours model, VmNormalHours translation, int dailyCout, int extraCount)
        {
            translation.Id.Should().Be(model.Id);
            translation.ValidFrom.Should().Be(model.ValidFrom.ToEpochTime());
            translation.Nonstop.Should().Be(model.DailyOpeningTimes.Count == 0);
            translation.DailyHours.Count.Should()
                .Be(model.DailyOpeningTimes.Count - model.DailyOpeningTimes.Count(x => x.IsExtra));
            foreach (var vmDailyHourse in translation.DailyHours)
            {
                int index = (int) vmDailyHourse.DayFrom;
                vmDailyHourse.TimeFrom.FromEpochTimeOfDay().Should().Be(GetTime(index, false, true));
                vmDailyHourse.TimeTo.FromEpochTimeOfDay().Should().Be(GetTime(index, false, false));
                if (index < extraCount)
                {
                    vmDailyHourse.TimeFromExtra.FromEpochTimeOfDay().Should().Be(GetTime(index, true, true));
                    vmDailyHourse.TimeToExtra.FromEpochTimeOfDay().Should().Be(GetTime(index, true, false));
                }
            }
        }

        private TimeSpan GetTime(int index, bool isExtra, bool isFrom)
        {
            return new TimeSpan(isExtra ? 10 : 0, index, isFrom ? 0 : 30);
        }
        private ServiceChannelServiceHours CreateModel(ServiceHoursTypeEnum shType)
        {
            return new ServiceChannelServiceHours
            {
                Id = Guid.NewGuid(),
                ServiceHourTypeId = shType.ToString().GetGuid(),
                AdditionalInformations = new HashSet<ServiceHoursAdditionalInformation>(),
                DailyOpeningTimes = new HashSet<DailyOpeningTime>()
            };
        }
    }
}