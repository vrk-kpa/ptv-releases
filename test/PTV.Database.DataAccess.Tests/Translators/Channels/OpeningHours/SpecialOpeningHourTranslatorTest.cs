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
using PTV.Database.DataAccess.Translators.Channels.V2;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Channels.OpeningHours
{
    public class SpecialOpeningHourTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;
        private ItemListModelGenerator listModelGenerator;

        public SpecialOpeningHourTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            var typesCacheMock = SetupTypesCacheMock<ServiceHourType>();
            listModelGenerator = new ItemListModelGenerator();

            translators = new List<object>
            {
                new SpecialOpeningHourTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object, new PTV.Database.DataAccess.Translators.Channels.EntityDefinitionHelper()),

                new ServiceHourTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object),
                new DailyHoursTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceHoursAdditionalInformation, VmStringText>>(), unitOfWorkMock)
            };
            RegisterDbSet(new List<ServiceHours>(), unitOfWorkMockSetup);
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
            var model = TestHelper.CreateVmHoursModel<VmSpecialHours>(serviceHoursType);
            model.ServiceHoursType = serviceHoursType;
            model.ValidFrom = DateTime.Now.ToEpochTime();
            model.TimeFrom = 100;
            model.TimeTo = 200;

            var toTranslate = new List<VmSpecialHours> { model };

            var translations = RunTranslationModelToEntityTest<VmSpecialHours, ServiceHours>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }

        private static void CheckTranslation(VmSpecialHours model, ServiceHours translation)
        {
            translation.Id.Should().NotBeEmpty();
            translation.OpeningHoursFrom.Should().Be(model.ValidFrom.FromEpochTime());
            translation.DailyOpeningTimes.Count.Should().Be(1);
            var day = translation.DailyOpeningTimes.First();
            day.From.Should().Be(model.TimeFrom.Value.FromEpochTimeOfDay());
            day.To.Should().Be(model.TimeTo.Value.FromEpochTimeOfDay());
        }

        /// <summary>
        /// test for ServiceChannelServiceHourTranslator entity - vm
        /// </summary>
        [Theory]
        [InlineData(ServiceHoursTypeEnum.Standard, true, true, "Additional info")]
        [InlineData(ServiceHoursTypeEnum.Special, false, true, "Additional info")]
        [InlineData(ServiceHoursTypeEnum.Exception, true, false, "")]
        [InlineData(ServiceHoursTypeEnum.Exception, false, false, null)]
        public void TranslateServiceHoursToVm(ServiceHoursTypeEnum hoursType, bool setDateFrom, bool setDateTo, string additionalInfo)
        {
            var model = CreateModel(hoursType);
            model.OpeningHoursFrom = DateTime.Now;
            model.DailyOpeningTimes.Add(new DailyOpeningTime
            {
                From = new TimeSpan(0, 10, 0, 0),
                To = new TimeSpan(0, 11, 0, 0),
                DayFrom = 0,
                DayTo = 1,
            });
            var toTranslate = new List<ServiceHours> { model };

            var translations = RunTranslationEntityToModelTest<ServiceHours, VmSpecialHours>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }

        private static void CheckTranslation(ServiceHours model, VmSpecialHours translation)
        {
//            translation.AdditionalInformation.Should().Be(model.AdditionalInformations.Select(x => x.Text).FirstOrDefault() ?? string.Empty);
            translation.Id.Should().Be(model.Id);
            translation.ValidFrom.Should().Be(model.OpeningHoursFrom.ToEpochTime());
            var day = model.DailyOpeningTimes.First();
            translation.TimeFrom.Should().BeGreaterThan(0);
            translation.TimeTo.Should().BeGreaterThan(0);
            ((int)translation.DayFrom).Should().Be(day.DayFrom);
            ((int?)translation.DayTo).Should().Be(day.DayTo);
//            translation.DailyHours.Count.Should()
//                .Be(model.DailyOpeningTimes.Count - model.DailyOpeningTimes.Count(x => x.IsExtra));
        }

        private ServiceHours CreateModel(ServiceHoursTypeEnum shType)
        {
            return new ServiceHours
            {
                Id = Guid.NewGuid(),
                ServiceHourTypeId = shType.ToString().GetGuid(),
                AdditionalInformations = new HashSet<ServiceHoursAdditionalInformation>(),
                DailyOpeningTimes = new HashSet<DailyOpeningTime>()
            };
        }
    }
}