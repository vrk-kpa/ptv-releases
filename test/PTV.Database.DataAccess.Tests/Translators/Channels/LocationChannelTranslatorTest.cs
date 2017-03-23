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
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;

using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class LocationChannelTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;
        private TestConversion conversion;
        private ItemListModelGenerator itemListGenerator;

        public LocationChannelTranslatorTest()
        {
            translators = new List<object>()
            {
                new LocationChannelStep1Translator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceLocationChannelAddress, VmAddressSimple>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceLocationChannelServiceArea, VmListItem>>(), unitOfWorkMock),

            };

            RegisterDbSet(new List<ServiceLocationChannel>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceLocationChannelServiceArea>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceChannelWebPage>(), unitOfWorkMockSetup);
            itemListGenerator = new ItemListModelGenerator();
            conversion = new TestConversion();
            itemListGenerator = new ItemListModelGenerator();
            unitOfWorkMock = unitOfWorkMockSetup.Object;
        }

        /// <summary>
        /// test for ElectronicChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(false, 0)]
        [InlineData(true, 0)]
        [InlineData(false, 5)]
        [InlineData(true, 6)]
        public void TranslateLocationChannelStep1(bool isRestrictedArea, int municipalityCount)
        {
            var model = CreateModel().Step1Form;
            model.IsRestrictedRegion = isRestrictedArea;
            model.Municipalities = itemListGenerator.Create<VmListItem>(municipalityCount).Select(x=>x.Id).ToList();

            var toTranslate = new List<VmLocationChannelStep1>() { model };

            var translations = RunTranslationModelToEntityTest<VmLocationChannelStep1, ServiceLocationChannel>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }


        private void CheckTranslation(VmLocationChannelStep1 step1, ServiceLocationChannel target)
        {
            target.Id.Should().NotBe(Guid.Empty);
            target.ServiceAreaRestricted.Should().Be(step1.IsRestrictedRegion);
            if (step1.IsRestrictedRegion)
            {
                target.ServiceAreas.Count.Should().Be(step1.Municipalities.Count);
            }
            else
            {
                target.ServiceAreas.Count.Should().Be(0);
            }
        }

        /// <summary>
        /// test for ElectronicChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData(5, 6, false)]
        [InlineData(0, 3, false)]
        [InlineData(10, 0, false)]
        [InlineData(0, 0, false)]
        [InlineData(5, 6, true)]
        [InlineData(0, 3, true)]
        [InlineData(10, 0, true)]
        [InlineData(0, 0, true)]
        public void TranslateLocationChannelStep3(int visitingAddressCount, int mailAddressCount, bool mailAddressSelected)
        {
            var model = CreateModel().Step1Form;
            model.VisitingAddresses = itemListGenerator.Create<VmAddressSimple>(visitingAddressCount);
            model.PostalAddresses = itemListGenerator.Create<VmAddressSimple>(mailAddressCount);

            var toTranslate = new List<VmLocationChannelStep1>() { model };

            var translations = RunTranslationModelToEntityTest<VmLocationChannelStep1, ServiceLocationChannel>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }


        //private void CheckTranslation(VmLocationChannelStep1 step3, ServiceLocationChannel target)
        //{
        //    target.Id.Should().NotBe(Guid.Empty);
        //    var expectedCount = step3.VisitingAddresses.Count + step3.PostalAddresses.Count;
        //    target.Addresses.Count.Should().Be(expectedCount, "Addresses");
        //}

        private VmLocationChannel CreateModel()
        {
            return new VmLocationChannel();
        }
    }
}
