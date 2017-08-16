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
using System.Collections.Generic;
using System.Linq;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using Xunit;
using PTV.Database.DataAccess.Translators.Channels;
using FluentAssertions;
using System;
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Framework;
using PTV.Database.DataAccess.Tests.Translators.Common;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class PrintableFormChannelMainTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;

        public PrintableFormChannelMainTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            translators = new List<object>()
            {
                new PrintableFormChannelMainTranslator(ResolveManager, TranslationPrimitives),
                new PrintableFormChannelStep1Translator(ResolveManager, TranslationPrimitives, CacheManager),
                new ServiceChannelTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new PublishingStatusTranslator(ResolveManager, TranslationPrimitives),

                RegisterTranslatorMock(new Mock<ITranslator<PrintableFormChannel, VmPrintableFormChannel>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, VmPrintableFormChannelStep1>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelName, VmName>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelDescription, VmDescription>>(), unitOfWorkMock),
            };

            RegisterDbSet(CreateCodeData<ServiceChannelType>(typeof(ServiceChannelTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<PublishingStatusType>(typeof(PublishingStatus)), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for PrintableFormChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("C83C80FD-60CF-454B-938A-C2EC85AA5657")]
        public void TranslatePrintableFormChannelToEntityCreate(string guid)
        {
            RegisterDbSet(
                string.IsNullOrEmpty(guid)
                    ? new List<ServiceChannelVersioned>()
                    : new List<ServiceChannelVersioned>() {new ServiceChannelVersioned() {Id = guid.ParseToGuid().Value}},

                unitOfWorkMockSetup);
            var model = TestHelper.CreateVmPrintableFormChannelModel();
            var toTranslate = new List<VmPrintableFormChannel>() { model };

            var translations = RunTranslationModelToEntityTest<VmPrintableFormChannel, ServiceChannelVersioned>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();
            //Assert.Equal(model.PublishingStatusId, translation.PublishingStatusId);
            //Assert.NotEqual(guid.ParseToGuid() ?? Guid.Empty, translation.Id);
        }

        /// <summary>
        /// test for PrintableFormChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(false, "C83C80FD-60CF-454B-938A-C2EC85AA5657")]
        [InlineData(true, "C83C80FD-60CF-454B-938A-C2EC85AA5657")]
        public void TranslatePrintableFormChannelToEntityUpdate(bool isSet, string guid)
        {
            var parsedGuid = guid.ParseToGuid();
            RegisterDbSet(isSet ? new List<ServiceChannelVersioned>() { new ServiceChannelVersioned() { Id = parsedGuid.Value} } : new List<ServiceChannelVersioned>(), unitOfWorkMockSetup);
            var model = TestHelper.CreateVmPrintableFormChannelModel();
            var toTranslate = new List<VmPrintableFormChannel>() { model };
            model.Id = parsedGuid;

            if (isSet)
            {
                var translations = RunTranslationModelToEntityTest<VmPrintableFormChannel, ServiceChannelVersioned>(translators, toTranslate, unitOfWorkMock);
                var translation = translations.First();
                //Assert.Equal(model.PublishingStatusId, translation.PublishingStatusId);
                //Assert.Equal(parsedGuid.Value, translation.Id);
            }
            else
            {
                Assert.Throws(typeof(DbEntityNotFoundException), () => RunTranslationModelToEntityTest<VmPrintableFormChannel, ServiceChannelVersioned>(translators, toTranslate, unitOfWorkMock));
            }
        }

        /// <summary>
        /// test for PrintableFormChannelMainTranslator entity - > vm
        /// </summary>
        [Fact]
        public void TranslatePrintableFormChannelToVm()
        {
            var toTranslate = new List<ServiceChannelVersioned>() { new ServiceChannelVersioned()};
            Assert.Throws(typeof(NotImplementedException), () => RunTranslationEntityToModelTest<ServiceChannelVersioned, VmPrintableFormChannel>(translators, toTranslate));
        }
    }
}
