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
using System;
using System.Reflection.Metadata.Ecma335;
using Moq;
using Xunit;

using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Database.DataAccess.Translators.Organizations;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Tests.Translators.Organizations
{
    public class OrganizationTreeTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;

        public OrganizationTreeTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;

            var languageCacheMock = new Mock<ILanguageCache>();
            languageCacheMock.Setup(i => i.Filter(It.IsAny<Guid>(), It.IsAny<LanguageCode>())).Returns(true);
            languageCacheMock.Setup(i => i.Filter(It.IsAny<List<ILocalizable>>(), It.IsAny<LanguageCode>())).Returns((List<ILocalizable> i) => i.FirstOrDefault());
            var languageCache = languageCacheMock.Object;

            translators = new List<object>()
            {
                new OrganizationTreeTranslator(ResolveManager, TranslationPrimitives, languageCache),
                new OrganizationTreeExpandedTranslator(ResolveManager, TranslationPrimitives, languageCache),
                RegisterTranslatorMock(new Mock<ITranslator<Organization, VmListItem>>(), unitOfWorkMock),
            };
        }
//   TODO: FIX TEST
//        /// <summary>
//        /// test for OrganizationTreeTranslator entity - > vm
//        /// </summary>
//        [Theory]
//        [InlineData(true)]
//        [InlineData(false)]
//        public void TranslateOrganizationToTreeVieModel(bool createSubOrganization)
//        {
//            var organization = CreateOrganization(createSubOrganization);
//            var toTranslate = new List<OrganizationVersioned> { organization };
//
//            var translations = RunTranslationEntityToModelTest<OrganizationVersioned, VmTreeItem>(translators, toTranslate);
//            var translation = translations.First();
//
//            Assert.Equal(toTranslate.Count, translations.Count);
//            Assert.Equal(organization.Id, translation.Id);
//            Assert.Equal(translation.Children.Count, 0);
//            Assert.Equal(createSubOrganization, !translation.IsLeaf);
//        }
//
//        /// <summary>
//        /// test for OrganizationTreeTranslator entity - > vm
//        /// </summary>
//        [Theory]
//        [InlineData(true)]
//        [InlineData(false)]
//        public void TranslateOrganizationToExpandedTreeVieModel(bool createSubOrganization)
//        {
//            var organization = CreateOrganization(createSubOrganization);
//            var toTranslate = new List<OrganizationVersioned> { organization };
//
//            var translations = RunTranslationEntityToModelTest<OrganizationVersioned, VmExpandedVmTreeItem>(translators, toTranslate);
//            var translation = translations.First();
//
//            Assert.Equal(toTranslate.Count, translations.Count);
//            Assert.Equal(organization.Id, translation.Id);
//            Assert.Equal(organization.Children.Count, translation.Children.Count);
//            Assert.Equal(createSubOrganization, !translation.IsLeaf);
//        }
//
//        private OrganizationVersioned CreateOrganization(bool createSubOrganization)
//        {
//            var topParentOrganization = new OrganizationVersioned() { OrganizationNames = new List<OrganizationName>() { new OrganizationName() { Name = "topParentTestOrganizationName", Type = new NameType() { Code = NameTypeEnum.Name.ToString() } } } };
//            var parentOrganization = new OrganizationVersioned() { Parent = topParentOrganization, OrganizationNames = new List<OrganizationName>() { new OrganizationName() { Name = "parentTestOrganizationName", Type = new NameType() { Code = NameTypeEnum.Name.ToString() } } } };
//            var childOrganization1 = new OrganizationVersioned() { Parent = parentOrganization, OrganizationNames = new List<OrganizationName>() { new OrganizationName() { Name = "childTestOrganizationName1", Type = new NameType() { Code = NameTypeEnum.Name.ToString() } } } };
//            var childOrganization2 = new OrganizationVersioned() { Parent = parentOrganization, OrganizationNames = new List<OrganizationName>() { new OrganizationName() { Name = "childTestOrganizationName2", Type = new NameType() { Code = NameTypeEnum.Name.ToString() } } } };
//
//            return new OrganizationVersioned()
//            {
//                Created = DateTime.Now,
//                CreatedBy = "testCreator",
//                Id = Guid.NewGuid(),
//                OrganizationNames = new List<OrganizationName>() { new OrganizationName() { Name = "testOrganizationName", Type = new NameType() { Code = NameTypeEnum.Name.ToString() },Localization = new Language() { Code = LanguageCode.fi.ToString()} } },
//                Parent = parentOrganization,
//                Children = createSubOrganization ? new List<OrganizationVersioned>() { childOrganization1, childOrganization2 } : new List<OrganizationVersioned>(),
//            };
//        }
    }
}
