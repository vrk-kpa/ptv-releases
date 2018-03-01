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
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using Xunit;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class PrintableFormChannelMainStep1TranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;

        public PrintableFormChannelMainStep1TranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            var typesCacheMock = SetupTypesCacheMock<ServiceChannelType>(typeof(ServiceChannelTypeEnum));
            var serviceChannel = new ServiceChannelVersioned();
            translators = new List<object>()
            {
//                new PrintableFormChannelMainStep1Translator(ResolveManager, TranslationPrimitives, new ServiceChannelTranslationDefinitionHelper(CacheManager), typesCacheMock.Object),

//                RegisterTranslatorMock(new Mock<ITranslator<PrintableFormChannel, VmPrintableFormChannelStep1>>(), unitOfWorkMock),
//                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, IVmChannelDescription>>(), unitOfWorkMock,
//                    desc =>
//                    {

//                        serviceChannel.OrganizationId = desc.OrganizationId ?? Guid.Empty;
//                        serviceChannel.ServiceChannelDescriptions.Add(new ServiceChannelDescription());
//                        serviceChannel.ServiceChannelNames.Add(new ServiceChannelName());
//                        return serviceChannel;
//                    },
//                    setTargetAction: sc => serviceChannel = sc ),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelAttachment, VmChannelAttachment>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelEmail, VmEmailData>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelPhone, VmPhone>>(), unitOfWorkMock)
            };

            RegisterDbSet(CreateCodeData<ServiceChannelType>(typeof(ServiceChannelTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<PublishingStatusType>(typeof(PublishingStatus)), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for PrintableFormChannelMainStep1Translator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(false, "C83C80FD-60CF-454B-938A-C2EC85AA5657")]
        [InlineData(true, "C83C80FD-60CF-454B-938A-C2EC85AA5657")]
        public void TranslatePrintableFormChannelStep1ToEntityUpdate(bool isSet, string guid)
        {
            Assert.False(true, "redesign needed");
            var parsedGuid = guid.ParseToGuid();
            var serviceChannel = new ServiceChannelVersioned() { Id = parsedGuid.Value };
            RegisterDbSet(isSet ? new List<ServiceChannelVersioned>() { serviceChannel } : new List<ServiceChannelVersioned>(), unitOfWorkMockSetup);
//            var model = TestHelper.CreateVmPrintableFormChannelModel().Step1Form;
//            var toTranslate = new List<VmPrintableFormChannelStep1>() { model };
//            model.Id = parsedGuid;
//
//            if (isSet)
//            {
//                var translations = RunTranslationModelToEntityTest<VmPrintableFormChannelStep1, ServiceChannelVersioned>(translators, toTranslate, unitOfWorkMock);
//                var translation = translations.First();
//                translation.OrganizationId.Should().Be(model.OrganizationId ?? Guid.Empty);
//                translation.ServiceChannelDescriptions.Count.Should().Be(1);
//                translation.ServiceChannelNames.Count.Should().Be(1);
//                translation.TypeId.Should().Be(ServiceChannelTypeEnum.PrintableForm.ToString().GetGuid());
//            }
//            else
//            {
//                Assert.Throws(typeof(DbEntityNotFoundException), () => RunTranslationModelToEntityTest<VmPrintableFormChannelStep1, ServiceChannelVersioned>(translators, toTranslate, unitOfWorkMock));
//            }
        }

        /// <summary>
        /// test for PrintableFormChannelMainStep1Translator entity - > vm
        /// </summary>
        [Fact]
        public void TranslatePrintableFormChannelStep1ToModelUpdate()
        {
            Assert.False(true, "redesign needed");
            var parsedGuid = Guid.NewGuid();
            var toTranslate = new List<ServiceChannelVersioned>() {new ServiceChannelVersioned() {Id = parsedGuid}};

//            var translations = RunTranslationEntityToModelTest<ServiceChannelVersioned, VmPrintableFormChannelStep1>(translators, toTranslate);
//            var translation = translations.First();
//            Assert.Equal(parsedGuid, translation.Id.Value);
        }
    }
}
