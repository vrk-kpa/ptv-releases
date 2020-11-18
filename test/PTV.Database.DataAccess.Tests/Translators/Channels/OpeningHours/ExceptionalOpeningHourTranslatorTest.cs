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
    public class ExceptionalOpeningHourTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;
        private ItemListModelGenerator listModelGenerator;

        public ExceptionalOpeningHourTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            var typesCacheMock = SetupTypesCacheMock<ServiceHourType>();
            listModelGenerator = new ItemListModelGenerator();

            translators = new List<object>
            {
                new ExceptionalOpeningHourTranslator2(ResolveManager, TranslationPrimitives, new ServiceChannelTranslationDefinitionHelper()),
//
//                new ServiceHourTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object),
//                new DailyHoursTranslator(ResolveManager, TranslationPrimitives),
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
        [InlineData(true)]
        [InlineData(false)]
        public void TranslateServiceChannelServiceHourToEntity(bool isClosed)
        {
            var model = TestHelper.CreateVmHoursModel<VmExceptionalHours>(ServiceHoursTypeEnum.Exception);
            model.OpeningPeriod = new VmDailyHourCommon();
            model.ClosedForPeriod = isClosed;

            var translation = RunTranslationModelToEntityTest<VmExceptionalHours, ServiceHours>(translators, model, unitOfWorkMock);

            CheckTranslation(model, translation);
        }

        private static void CheckTranslation(VmExceptionalHours model, ServiceHours translation)
        {
            translation.Id.Should().NotBeEmpty();
            translation.IsClosed.Should().Be(model.ClosedForPeriod);
            translation.DailyOpeningTimes.Count.Should().Be(model.ClosedForPeriod ? 0 : 1);
        }

        /// <summary>
        /// test for ServiceChannelServiceHourTranslator entity - vm
        /// </summary>
        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        public void TranslateServiceHoursToVm(bool isClosed, bool hasTimes)
        {
            var model = CreateModel(ServiceHoursTypeEnum.Exception);
            model.IsClosed = isClosed;
            model.DailyOpeningTimes = listModelGenerator.Create(hasTimes ? 1 : 0,
                i => new DailyOpeningTime());

            var translation = RunTranslationEntityToModelTest<ServiceHours, VmExceptionalHours>(translators, model);
            CheckTranslation(model, translation);
        }

        private static void CheckTranslation(ServiceHours source, VmExceptionalHours translation)
        {
            translation.ClosedForPeriod.Should().Be(source.IsClosed);
//            translation.Id.Should().Be(source.Id);
            if (source.DailyOpeningTimes.Count > 0)
            {
                translation.OpeningPeriod.Should().NotBeNull();
            }
            else
            {
                translation.OpeningPeriod.Should().BeNull();
            }
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
