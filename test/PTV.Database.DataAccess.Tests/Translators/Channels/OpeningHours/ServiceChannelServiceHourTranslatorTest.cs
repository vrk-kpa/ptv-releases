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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using Xunit;
using Xunit.Sdk;

namespace PTV.Database.DataAccess.Tests.Translators.Channels.OpeningHours
{
    public class ServiceChannelServiceHourTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;
        private ItemListModelGenerator listModelGenerator;

        public ServiceChannelServiceHourTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            var typesCacheMock = SetupTypesCacheMock<ServiceHourType>();
            listModelGenerator = new ItemListModelGenerator();

            translators = new List<object>
            {
                new ServiceChannelServiceHourTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object),

                RegisterTranslatorMock(new Mock<ITranslator<ServiceHoursAdditionalInformation, VmStringText>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<DailyOpeningTime, VmDailyHours>>(), unitOfWorkMock, d => new DailyOpeningTime { IsExtra = false, CreatedBy = "daily"}),
                RegisterTranslatorMock(new Mock<ITranslator<DailyOpeningTime, VmExtraHours>>(), unitOfWorkMock, d => new DailyOpeningTime { IsExtra = true, CreatedBy = "extra"}),
            };
            RegisterDbSet(new List<ServiceChannelServiceHours>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for ServiceChannelServiceHourTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(ServiceHoursTypeEnum.Standard, 3, 2, true)]
        [InlineData(ServiceHoursTypeEnum.Exception, 6, 0, true)]
        [InlineData(ServiceHoursTypeEnum.Special, 0, 0, false)]
        [InlineData(ServiceHoursTypeEnum.Standard, 4, 4, false)]
        public void TranslateServiceChannelServiceHourToEntity(ServiceHoursTypeEnum serviceHoursType, int dailyCount, int extraCount, bool isRange)
        {
            var model = TestHelper.CreateVmHoursModel<VmHours>(serviceHoursType);
            model.ServiceHoursType = serviceHoursType;
            model.IsDateRange = isRange;

            var toTranslate = new List<VmHours> { model };

            var translations = RunTranslationModelToEntityTest<VmHours, ServiceChannelServiceHours>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation, dailyCount, extraCount);
        }

        private static void CheckTranslation(VmHours model, ServiceChannelServiceHours translation, int normalCount, int extraCount)
        {
            translation.AdditionalInformations.Count.Should().Be(1);
            translation.Id.Should().NotBe(model.ServiceHoursType.ToString().GetGuid());
            translation.ServiceHourTypeId.Should().NotBeEmpty();

            translation.ValidFrom.Should().Be(model.ValidFrom.HasValue ? model.ValidFrom.FromEpochTime() : DateTime.UtcNow.Date);

            if (model.IsDateRange)
            {
                translation.ValidTo.Should().Be(model.ValidTo.FromEpochTime());
            }
            else
            {
                translation.ValidTo.HasValue.Should().BeFalse();
            }
//            translation.DailyOpeningTimes.FirstOrDefault()?.CreatedBy.Should().Be("test" + translation.DailyOpeningTimes.Count.ToString());
//            translation.DailyOpeningTimes.Count.Should().Be(normalCount + extraCount);
//            translation.DailyOpeningTimes.Count(x => !x.IsExtra).Should().Be(normalCount);
//            translation.DailyOpeningTimes.Count(x => x.IsExtra).Should().Be(extraCount);
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
            model.AdditionalInformations = listModelGenerator.Create(additionalInfo == null ? 0 : 1, i => new ServiceHoursAdditionalInformation {Text = additionalInfo});
            model.ValidTo = setDateFrom ? DateTime.Now : (DateTime?)null;
            model.ValidFrom = setDateTo ? DateTime.Now.AddDays(2) : (DateTime?) null;
            var toTranslate = new List<ServiceChannelServiceHours> { model };

            var translations = RunTranslationEntityToModelTest<ServiceChannelServiceHours, VmHours>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }

        private static void CheckTranslation(ServiceChannelServiceHours model, VmHours translation)
        {
            translation.AdditionalInformation.Should().Be(model.AdditionalInformations.Select(x => x.Text).FirstOrDefault() ?? string.Empty);
            translation.Id.Should().Be(model.Id);
            translation.ValidFrom.Should().Be(model.ValidFrom.ToEpochTime());
            translation.ValidTo.Should().Be(model.ValidTo.ToEpochTime());
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
