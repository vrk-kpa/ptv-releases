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
using PTV.Database.DataAccess.Translators.Organizations;
using PTV.Database.DataAccess.Tests.Translators.Common;
using System;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.DataAccess.Translators.Common.V2;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Translators.Organizations
{
    public class OrganizationWebPageTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;

        public OrganizationWebPageTranslatorTest()
        {
            var typesCacheMock = SetupTypesCacheMock<WebPageType>(typesEnum: typeof(WebPageTypeEnum));

            translators = new List<object>
            {
                new OrganizationWebPageTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object),
//                new WebPageTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new UrlTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock<WebPage, VmWebPage>(),
                RegisterTranslatorMock<IOrderable, IVmOrderable>(model => new OrganizationWebPage()),
            };
            RegisterDbSet(new List<OrganizationWebPage>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<WebPage>(), unitOfWorkMockSetup);

        }

        /// <summary>
        /// test for OrganizationWebPageTranslator vm - > entity
        /// </summary>
        /// <param name="createHomePage">true - chceck if translator create Home page</param>
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TranslateOrganizationWebPageToEntity(bool createHomePage)
        {
            var vmWebPage = TestHelper.CreateVmWebPageUrlModel();
            vmWebPage.TypeId = WebPageTypeEnum.HomePage.ToString().GetGuid();

            vmWebPage.TypeId = createHomePage ? null : vmWebPage.TypeId;

            var translation = RunTranslationModelToEntityTest<VmWebPage, OrganizationWebPage>(translators, vmWebPage, unitOfWorkMockSetup.Object);
            if (createHomePage)
            {
                Assert.Equal(WebPageTypeEnum.HomePage.ToString().GetGuid(), translation.TypeId);
            }
            else
            {
                Assert.Equal(vmWebPage.TypeId, translation.TypeId);
            }

        }

        /// <summary>
        /// test for OrganizationWebPageTranslator entity - > vm
        /// </summary>
        [Fact]
        public void TranslateOrganizationWebPageToViewModel()
        {
            var organizationWebPage = CreateOrganizationWebPage();
            var toTranslate = new List<OrganizationWebPage> { organizationWebPage };

            var translations = RunTranslationEntityToModelTest<OrganizationWebPage, VmWebPage>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(organizationWebPage.TypeId, translation.TypeId);
            Assert.Equal(organizationWebPage.Id, translation.Id);
            Assert.Equal(organizationWebPage.WebPage.Id, translation.WebPageId);
        }

        private OrganizationWebPage CreateOrganizationWebPage()
        {
            return new OrganizationWebPage
            {
                TypeId = Guid.NewGuid(),
                WebPage = new WebPage{ Id = Guid.NewGuid() },
                OrganizationVersionedId = Guid.NewGuid()
            };
        }
    }
}
