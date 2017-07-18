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
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Services
{
    public class ServiceWebPageTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;
        private readonly IUnitOfWork unitOfWorkMock;

        public ServiceWebPageTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            translators = new List<object>
            {
                new ServiceWebPageServiceVoucherTranslator(ResolveManager, TranslationPrimitives),
                new LanguageCodeTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceWebPage, string>>(), unitOfWorkMock),
            };
            RegisterDbSet(Enum.GetNames(typeof(LanguageCode)).Select(x => new Language { Code = x }).ToList(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceWebPage>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for ServiceWebPageTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("Name", "Url", "AdditionalInformation", 1)]
        [InlineData("", "", "", null)]
        public void TranslateToEntityTest(string name, string urlAddress, string additionalInformation, int? orderNumber)
        {
            var model = new VmServiceVoucher { Name = name, UrlAddress = urlAddress, AdditionalInformation = additionalInformation, OrderNumber = orderNumber};
            var toTranslate = new List<VmServiceVoucher> { model };

            translators.Add(RegisterTranslatorMock(new Mock<ITranslator<WebPage, VmServiceVoucher>>(), unitOfWorkMock, 
                detail => new WebPage {Name  = detail.Name,  Url = detail.UrlAddress, Description = detail.AdditionalInformation, OrderNumber = detail.OrderNumber}));

            var translations = RunTranslationModelToEntityTest<VmServiceVoucher, ServiceWebPage>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();
            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }

        private static void CheckTranslation(VmServiceVoucher source, ServiceWebPage target)
       {
           target.WebPage.Name.Should().Be(source.Name);
           target.WebPage.Url.Should().Be(source.UrlAddress);
           target.WebPage.Description.Should().Be(source.AdditionalInformation);
           target.WebPage.OrderNumber.Should().Be(source.OrderNumber);
       }
    }
}
