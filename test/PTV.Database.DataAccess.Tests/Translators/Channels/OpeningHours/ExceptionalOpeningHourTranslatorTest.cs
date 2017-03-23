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
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Channels.OpeningHours
{
    public class ExceptionalOpeningHourTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;
        private ItemListModelGenerator listModelGenerator;

        public ExceptionalOpeningHourTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            var typesCacheMock = SetupTypesCacheMock<ServiceHourType>();
            listModelGenerator = new ItemListModelGenerator();

            translators = new List<object>
            {
                new ExceptionalOpeningHourTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object, new ServiceChannelTranslationDefinitionHelper(CacheManager)),

                new ServiceChannelServiceHourTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object),
                new DailyHoursTranslator(ResolveManager, TranslationPrimitives),
//                RegisterTranslatorMock(new Mock<ITranslator<DailyOpeningTime, IDailyHours>>(), unitOfWorkMock,
//                    d => new DailyOpeningTime { IsExtra = false, CreatedBy = "daily", From = new TimeSpan(1,0,0), To = new TimeSpan(2,0,0)}
//                    ),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceHoursAdditionalInformation, VmStringText>>(), unitOfWorkMock),
            };
            RegisterDbSet(new List<ServiceChannelServiceHours>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<DailyOpeningTime>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for ServiceChannelServiceHourTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(ServiceHoursTypeEnum.Standard, WeekDayEnum.Monday, WeekDayEnum.Friday)]
        [InlineData(ServiceHoursTypeEnum.Exception, WeekDayEnum.Thursday, WeekDayEnum.Saturday)]
        [InlineData(ServiceHoursTypeEnum.Special, WeekDayEnum.Tuesday, null)]
        [InlineData(ServiceHoursTypeEnum.Standard, WeekDayEnum.Saturday, null)]
        public void TranslateServiceChannelServiceHourToEntity(ServiceHoursTypeEnum serviceHoursType, WeekDayEnum dayFrom, int dayTo)
        {
            var model = TestHelper.CreateVmHoursModel<VmExceptionalHours>(serviceHoursType);
            model.ServiceHoursType = serviceHoursType;
//            model.DailyHours = listModelGenerator.Create(dailyCount, i =>
//                new VmDailyHours
//                {
//                    Extra = i < extraCount ? DailyHoursExtraTypes.Vissible : DailyHoursExtraTypes.Hidden
//                });
            model.TimeFrom = 100;
            model.TimeTo = 200;

            var toTranslate = new List<VmExceptionalHours> { model };

            var translations = RunTranslationModelToEntityTest<VmExceptionalHours, ServiceChannelServiceHours>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }

        private static void CheckTranslation(VmExceptionalHours model, ServiceChannelServiceHours translation)
        {
            translation.Id.Should().NotBeEmpty();
            translation.IsClosed.Should().Be(model.Closed);
            translation.DailyOpeningTimes.Count.Should().Be(model.Closed ? 0 : 1);
            if (!model.Closed)
            {
                var day = translation.DailyOpeningTimes.First();
                day.From.Should().Be(model.TimeFrom.Value.FromEpochTimeOfDay());
                day.To.Should().Be(model.TimeTo.Value.FromEpochTimeOfDay());
                day.DayFrom.Should().Be((int) WeekDayEnum.Monday);
                day.DayTo.Should().Be((int?) WeekDayEnum.Sunday);
            }
        }

        /// <summary>
        /// test for ServiceChannelServiceHourTranslator entity - vm
        /// </summary>
        [Theory]
        [InlineData(ServiceHoursTypeEnum.Standard, true)]
        [InlineData(ServiceHoursTypeEnum.Special, false)]
        [InlineData(ServiceHoursTypeEnum.Exception, true)]
        [InlineData(ServiceHoursTypeEnum.Exception, false)]
        public void TranslateServiceHoursToVm(ServiceHoursTypeEnum hoursType, bool isClosed)
        {
            var model = CreateModel(hoursType);
            model.IsClosed = isClosed;
            model.DailyOpeningTimes = listModelGenerator.Create(1,
                i => new DailyOpeningTime {From = new TimeSpan(0, 8, 0)});
            model.ValidFrom = DateTime.Now;
            var toTranslate = new List<ServiceChannelServiceHours> { model };

            var translations = RunTranslationEntityToModelTest<ServiceChannelServiceHours, VmExceptionalHours>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }

        private static void CheckTranslation(ServiceChannelServiceHours model, VmExceptionalHours translation)
        {
            translation.Closed.Should().Be(model.IsClosed);
//            translation.AdditionalInformation.Should().Be(model.AdditionalInformations.Select(x => x.Text).FirstOrDefault() ?? string.Empty);
            translation.Id.Should().Be(model.Id);
            translation.ValidFrom.Should().Be(model.ValidFrom.ToEpochTime());
            if (!model.IsClosed)
            {
                translation.TimeFrom.Should().BeGreaterThan(0);
            }
//            translation.DailyHours.Count.Should()
//                .Be(model.DailyOpeningTimes.Count - model.DailyOpeningTimes.Count(x => x.IsExtra));
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