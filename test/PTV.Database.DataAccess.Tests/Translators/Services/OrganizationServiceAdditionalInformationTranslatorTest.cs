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
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;

using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Services
{
    public class OrganizationServiceAdditionalInformationTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;

        public OrganizationServiceAdditionalInformationTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            translators = new List<object>
            {
                new OrganizationServiceAdditionalInformationTranslator(ResolveManager, TranslationPrimitives),
                new LanguageCodeTranslator(ResolveManager, TranslationPrimitives)
            };

            RegisterDbSet(CreateCodeData<PublishingStatusType>(typeof(PublishingStatus)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<Language>(typeof(LanguageCode)), unitOfWorkMockSetup);
            RegisterDbSet(new List<OrganizationServiceAdditionalInformation>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for ServiceNameTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("Test")]
        [InlineData("")]
        public void TranslateToEntityTest(string name)
        {
            var model = new VmServiceProducerDetail { FreeDescription = name };
            var toTranslate = new List<VmServiceProducerDetail> { model };

            var translations = RunTranslationModelToEntityTest<VmServiceProducerDetail, OrganizationServiceAdditionalInformation>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();
            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }

        private void CheckTranslation(VmServiceProducerDetail source, OrganizationServiceAdditionalInformation target)
        {
            target.LocalizationId.Should().NotBe(Guid.Empty);
            //target.Localization.Code.Should().Be(LanguageCode.fi.ToString());
            target.Text.Should().Be(source.FreeDescription);
        }

        private VmListItem CreateModel()
        {
            return new VmListItem()
            {
                Id = Guid.NewGuid()
            };
        }

    }
}
