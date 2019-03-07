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
using PTV.Database.DataAccess.Translators.Channels.V2.OpeningHours;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;
using Xunit;
using EntityDefinitionHelper = PTV.Database.DataAccess.Translators.Channels.V2.EntityDefinitionHelper;

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
                new SpecialOpeningHourTranslator2(ResolveManager, TranslationPrimitives, typesCacheMock.Object, new EntityDefinitionHelper(CacheManager)),

                RegisterTranslatorMock(new Mock<ITranslator<DailyOpeningTime, VmDailyHourCommon>>(), unitOfWorkMock, entity => entity != null ? new VmDailyHourCommon() : null),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceHours, VmOpeningHour>>(), unitOfWorkMock),
            };
            RegisterDbSet(new List<ServiceHours>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<DailyOpeningTime>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for ServiceChannelServiceHourTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(ServiceHoursTypeEnum.Standard)]
        [InlineData(ServiceHoursTypeEnum.Exception)]
        [InlineData(ServiceHoursTypeEnum.Special)]
        public void TranslateServiceChannelServiceHourToEntity(ServiceHoursTypeEnum serviceHoursType)
        {
            var model = TestHelper.CreateVmHoursModel<VmSpecialHours>(serviceHoursType);
            model.ServiceHoursType = serviceHoursType;
            model.TimeFrom = 100;
            model.TimeTo = 200;

            var translation = RunTranslationModelToEntityTest<VmSpecialHours, ServiceHours>(translators, model, unitOfWorkMock);
            CheckTranslation(model, translation);
        }

        private static void CheckTranslation(VmSpecialHours model, ServiceHours translation)
        {
            translation.Id.Should().NotBeEmpty();
            translation.DailyOpeningTimes.Count.Should().Be(1);
        }

        /// <summary>
        /// test for ServiceChannelServiceHourTranslator entity - vm
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(0)]
        [InlineData(null)]
        public void TranslateServiceHoursToVm(int? dailyTimesCount)
        {
            var model = CreateModel();
            switch (dailyTimesCount)
            {
                case 0:
                    model.DailyOpeningTimes = new List<DailyOpeningTime>();
                    break;
                case 1:
                    model.DailyOpeningTimes = new List<DailyOpeningTime>
                    {
                        new DailyOpeningTime()
                    };
                    break;
                default:
                    model.DailyOpeningTimes = null;
                    break;
            }

            var translation = RunTranslationEntityToModelTest<ServiceHours, VmSpecialHours>(translators, model);
            CheckTranslation(model, translation);
        }

        private static void CheckTranslation(ServiceHours source, VmSpecialHours translation)
        {
            translation.OpeningPeriod.Should().NotBeNull();
            if (source.DailyOpeningTimes?.Count != 1)
            {
                translation.OpeningPeriod.DayFrom.Should().Be(WeekDayEnum.Monday);
                translation.OpeningPeriod.DayTo.Should().Be(WeekDayEnum.Monday);
            }
        }

        private ServiceHours CreateModel()
        {
            return new ServiceHours
            {
                Id = Guid.NewGuid(),
                AdditionalInformations = new HashSet<ServiceHoursAdditionalInformation>(),
                DailyOpeningTimes = new HashSet<DailyOpeningTime>()
            };
        }
    }
}