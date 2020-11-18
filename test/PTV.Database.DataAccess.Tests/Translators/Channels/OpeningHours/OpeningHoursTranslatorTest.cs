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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.EntityCloners;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.DataAccess.Translators.Channels.V2.OpeningHours;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Channels.OpeningHours
{
    public class OpeningHoursTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;
        private ItemListModelGenerator itemListGenerator;

        public OpeningHoursTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            var typesCacheMock = SetupTypesCacheMock<ServiceHourType>();
            translators = new List<object>
            {
                new OpeningHoursTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object),
//                new PublishingStatusTypeTranslator(ResolveManager, TranslationPrimitives),

                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelServiceHours, VmNormalHours>>(), unitOfWorkMock, model => CreateTranslatedEntity(ServiceHoursTypeEnum.Standard)),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelServiceHours, VmSpecialHours>>(), unitOfWorkMock, model => CreateTranslatedEntity(ServiceHoursTypeEnum.Special)),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelServiceHours, VmExceptionalHours>>(), unitOfWorkMock, model => CreateTranslatedEntity(ServiceHoursTypeEnum.Exception)),

            };

            RegisterServiceMock<ICloningManager>();
//            RegisterDbSet(CreateCodeData<ServiceChannelType>(typeof(ServiceChannelTypeEnum)), unitOfWorkMockSetup);
//            RegisterDbSet(CreateCodeData<PublishingStatusType>(typeof(PublishingStatus)), unitOfWorkMockSetup);

            RegisterDbSet(new List<ServiceChannelVersioned>(), unitOfWorkMockSetup);
//            RegisterDbSet(new List<ServiceChannelServiceHours>(), unitOfWorkMock);

            itemListGenerator = new ItemListModelGenerator();
        }

        private ServiceChannelServiceHours CreateTranslatedEntity(ServiceHoursTypeEnum type)
        {
            return new ServiceChannelServiceHours {ServiceHours = new ServiceHours {ServiceHourTypeId = type.ToString().GetGuid() }};
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
            model.SpecialHours = itemListGenerator.Create<VmSpecialHours>(specialHoursCount);

            var translation = RunTranslationModelToEntityTest<VmOpeningHours, ServiceChannelVersioned>(translators, model, unitOfWorkMock);
            CheckTranslation(model, translation);
        }


        private void CheckTranslation(VmOpeningHours model, ServiceChannelVersioned target)
        {
            //target.Id.Should().NotBe(Guid.Empty);
            target.ServiceChannelServiceHours.Count.Should().Be(model.StandardHours.Count + model.ExceptionHours.Count + model.SpecialHours.Count, "Opening hours");
            target.ServiceChannelServiceHours.Count(x => x.ServiceHours.ServiceHourTypeId == ServiceHoursTypeEnum.Standard.ToString().GetGuid()).Should().Be(model.StandardHours.Count, "normal hours");
            target.ServiceChannelServiceHours.Count(x => x.ServiceHours.ServiceHourTypeId == ServiceHoursTypeEnum.Special.ToString().GetGuid()).Should().Be(model.SpecialHours.Count, "special hours");
            target.ServiceChannelServiceHours.Count(x => x.ServiceHours.ServiceHourTypeId == ServiceHoursTypeEnum.Exception.ToString().GetGuid()).Should().Be(model.ExceptionHours.Count, "exceptional hours");
        }

        private VmOpeningHours CreateModel()
        {
            return new VmOpeningHours();
        }
    }
}
