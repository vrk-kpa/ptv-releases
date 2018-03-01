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
using PTV.Framework;
using PTV.Database.DataAccess.Tests.Translators.Common;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class PrintableFormChannelStep1TranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;
        private readonly IUnitOfWork unitOfWorkMock;

        public PrintableFormChannelStep1TranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            translators = new List<object>()
            {
//                new PrintableFormChannelStep1Translator(ResolveManager, TranslationPrimitives, CacheManager),
                RegisterTranslatorMock(new Mock<ITranslator<PrintableFormChannelUrl, VmWebPage>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<Address, VmAddressSimple>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<PrintableFormChannelIdentifier, string>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<PrintableFormChannelReceiver, string>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<PrintableFormChannelIdentifier, VmPrintableFormChannelIdentifier>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<PrintableFormChannelReceiver, VmPrintableFormChannelReceiver>>(), unitOfWorkMock),
            };

        }


        /// <summary>
        /// test for PrintableFormChannelStep1TranslatorTest vm - > entity
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("C83C80FD-60CF-454B-938A-C2EC85AA5657")]
        public void TranslatePrintableFormChannelToEntity(string guid)
        {
            Assert.False(true, "redesign needed");
//            RegisterDbSet(
//                string.IsNullOrEmpty(guid)
//                    ? new List<PrintableFormChannel>()
//                    : new List<PrintableFormChannel>() { new PrintableFormChannel() { Id = guid.ParseToGuid().Value } },
//
//                unitOfWorkMockSetup);
//            var model = TestHelper.CreateVmPrintableFormChannelModel().Step1Form;
//            var toTranslate = new List<VmPrintableFormChannelStep1>() { model };
//
//            var translations = RunTranslationModelToEntityTest<VmPrintableFormChannelStep1, PrintableFormChannel>(translators, toTranslate, unitOfWorkMock);
//            var translation = translations.First();
//
//            Assert.Equal(toTranslate.Count, translations.Count);
//            CheckTranslation(model, translation);
        }

        /// <summary>
        /// test for PrintableFormChannelStep1TranslatorTest entity - > vm
        /// </summary>
        //[Fact]
        //public void TranslatePrintableFormChannelToModel()
        //{
        //    var toTranslate = new List<PrintableFormChannel>()
        //    {
        //        new PrintableFormChannel()
        //        {
        //            FormIdentifier = "form identifier",
        //            FormReceiver = "form receiver",
        //           // DeliveryAddressDescriptions = new List<PrintableFormChannelDeliveryAddressDescription>() { new PrintableFormChannelDeliveryAddressDescription() { Description = "printable form description" } }
        //        }
        //    };

        //    var translations = RunTranslationEntityToModelTest<PrintableFormChannel, VmPrintableFormChannelStep1>(translators, toTranslate);
        //    var translation = translations.First();

        //    Assert.Equal("form identifier", translation.FormIdentifier);
        //    Assert.Equal("form receiver", translation.FormReceiver);
        //   // Assert.Equal("printable form description", translation.DeliveryAddressDescription);
        //}

//        private void CheckTranslation(VmPrintableFormChannelStep1 source, PrintableFormChannel target)
//        {
//            target.FormIdentifiers.Count.Should().Be(1);
//            target.FormReceivers.Count.Should().Be(1);
//            target.ChannelUrls.Count.Should().Be(source.WebPages.Count);
//            // Assert.Equal(model.UrlAttachments.Count, translation.Attachments.Count);
//            // target.Id.Should().Be(source.Id);
//            target.Should().NotBe(Guid.Empty);
//            target.DeliveryAddress.Should().NotBeNull();
//        }
    }
}
