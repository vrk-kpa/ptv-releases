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
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Channels.OpeningHours
{
    public class OpeningHoursTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;
        private TestConversion conversion;
        private ItemListModelGenerator itemListGenerator;

        public OpeningHoursTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            var typesCacheMock = SetupTypesCacheMock<ServiceHourType>();
            translators = new List<object>
            {
                new ServiceChannelOpeningHoursTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object),
//                new PublishingStatusTypeTranslator(ResolveManager, TranslationPrimitives),

                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelServiceHours, VmNormalHours>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelServiceHours, VmSpecialHours>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelServiceHours, VmExceptionalHours>>(), unitOfWorkMock),

            };
//            RegisterDbSet(CreateCodeData<ServiceChannelType>(typeof(ServiceChannelTypeEnum)), unitOfWorkMockSetup);
//            RegisterDbSet(CreateCodeData<PublishingStatusType>(typeof(PublishingStatus)), unitOfWorkMockSetup);

            RegisterDbSet(new List<ServiceChannelVersioned>(), unitOfWorkMockSetup);
//            RegisterDbSet(new List<ServiceChannelServiceHours>(), unitOfWorkMock);

            itemListGenerator = new ItemListModelGenerator();
            conversion = new TestConversion();
            itemListGenerator = new ItemListModelGenerator();
        }

        /// <summary>
        /// test for ElectronicChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(5, 6, 2)]
        [InlineData(0, 3, 0)]
        [InlineData(10, 0, 0)]
        [InlineData(0, 0, 5)]
        [InlineData(0, 0, 0)]
        public void TranslateOpeningHours(int standardHoursCount, int exceptionHoursCount, int specialHoursCount)
        {
            var model = CreateModel();
            model.StandardHours = itemListGenerator.Create<VmNormalHours>(standardHoursCount);
            model.ExceptionHours = itemListGenerator.Create<VmExceptionalHours>(exceptionHoursCount);
            model.SpecialHours = itemListGenerator.Create<VmSpecialHours>(exceptionHoursCount);

            

            var toTranslate = new List<VmOpeningHoursStep>() { model };

            var translations = RunTranslationModelToEntityTest<VmOpeningHoursStep, ServiceChannelVersioned>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }


        private void CheckTranslation(VmOpeningHoursStep model, ServiceChannelVersioned target)
        {
            //target.Id.Should().NotBe(Guid.Empty);
            target.ServiceHours.Count.Should().Be(model.StandardHours.Count + model.ExceptionHours.Count + model.SpecialHours.Count, "Opening hours");
        }

        private VmOpeningHoursStep CreateModel()
        {
            return new VmOpeningHoursStep();
        }
    }
}
