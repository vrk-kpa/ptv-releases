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
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;

using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class LocationChannelServiceAreaTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;
        private TestConversion conversion;
        private ItemListModelGenerator itemListGenerator;

        public LocationChannelServiceAreaTranslatorTest()
        {
            translators = new List<object>()
            {
                new ServiceLocationChannelServiceAreaTranslator(ResolveManager, TranslationPrimitives),

//                RegisterTranslatorMock(new Mock<ITranslator<WebPage, VmWebPage>>(), unitOfWorkMock),

            };
            //            RegisterDbSet(CreateCodeData<AddressType>(typeof(AddressTypeEnum)), unitOfWorkMock);
            //            RegisterDbSet(CreateTypeData<PublishingStatusType>(typeof(PublishingStatus)), unitOfWorkMock);

            //            RegisterDbSet(new List<Attachment>(), unitOfWorkMock);
            //            RegisterDbSet(new List<ServiceChannelServiceHours>(), unitOfWorkMock);
            RegisterDbSet(new List<ServiceLocationChannelServiceArea>(), unitOfWorkMockSetup);
            itemListGenerator = new ItemListModelGenerator();
            conversion = new TestConversion();
            itemListGenerator = new ItemListModelGenerator();
            unitOfWorkMock = unitOfWorkMockSetup.Object;
        }

        /// <summary>
        /// test for WebPage vm - > entity
        /// </summary>
        /// <param name="municipalityId"></param>
        [Theory]
        [InlineData("")]
        [InlineData("11D495DE-E1E7-4B57-BC86-20A87BA83324")]
        public void TranslateLocationChannelArea(string municipalityId)
        {
            var model = CreateModel();
            model.Id = conversion.GetGuid(municipalityId) ?? Guid.Empty;

            var toTranslate = new List<VmListItem>() { model };

            var translations = RunTranslationModelToEntityTest<VmListItem, ServiceLocationChannelServiceArea>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }


        private void CheckTranslation(VmListItem source, ServiceLocationChannelServiceArea target)
        {
            target.Should().NotBe(Guid.Empty);
            target.MunicipalityId.Should().Be(source.Id);
        }


        private VmListItem CreateModel()
        {
            return new VmListItem();
        }
    }
}
