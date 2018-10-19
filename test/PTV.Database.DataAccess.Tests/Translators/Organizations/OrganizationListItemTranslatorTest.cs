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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.DataAccess.Translators.Organizations;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using Xunit;
using PTV.Framework;
using PTV.Database.DataAccess.Translators.Values;
using PTV.Database.DataAccess.Caches;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;

namespace PTV.Database.DataAccess.Tests.Translators.Organizations
{
    public class OrganizationListItemTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;

        public OrganizationListItemTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            var unitOfWorkMock = unitOfWorkMockSetup.Object;
            var typesCache = SetupTypesCacheMock<NameType>(typeof(NameTypeEnum));
            translators = new List<object>()
            {
                new OrganizationListItemTranslator(ResolveManager, TranslationPrimitives, typesCache.Object, LanguageCache, InternalLanguageCache),
                new DateTimeLongValueTranslator(),
//                new PublishingStatusTranslator(ResolveManager, TranslationPrimitives)
                RegisterTranslatorMock(new Mock<ITranslator<OrganizationName, string>>(), unitOfWorkMock, text => new OrganizationName { Name = text}, entity => entity.Name),

        };
        }
        // TODO: FIX TEST
//        /// <summary>
//        /// test for OrganizationListItemTranslator entity - > vm
//        /// </summary>
//        [Fact]
//        public void TranslateOrganizationsToViewModel()
//        {
//            var organization = CreateOrganization();
//            var toTranslate = new List<OrganizationVersioned>() { organization };
//            var translations = RunTranslationEntityToModelTest<OrganizationVersioned, VmOrganizationListItem>(translators, toTranslate);
//            var translation = translations.First();
//
//            Assert.Equal(translations.Count, toTranslate.Count);
//            Assert.Equal(translation.Modified, organization.Modified.ToEpochTime());
//            Assert.Equal(translation.ModifiedBy, organization.ModifiedBy);
//            Assert.Equal(translation.Id, organization.Id);
//            translation.Name.Should().Be(organization.OrganizationNames.First().Name);
//            translation.MainOrganization.Should().Be(organization.Parent.OrganizationNames.First().Name);
//            translation.SubOrganizations.Should().Be(organization.Children.ElementAt(0).OrganizationNames.First().Name + ", " +organization.Children.ElementAt(1).OrganizationNames.First().Name);
//        }
//
//        private OrganizationVersioned CreateOrganization()
//        {
//            var topParentOrganization = new OrganizationVersioned() { OrganizationNames = new List<OrganizationName>() { new OrganizationName() { Name = "topParentTestOrganizationName", TypeId = NameTypeEnum.Name.ToString().GetGuid() } }, DisplayNameTypeId = NameTypeEnum.Name.ToString().GetGuid() };
//            var parentOrganization = new OrganizationVersioned() { Parent = topParentOrganization, OrganizationNames = new List<OrganizationName>() { new OrganizationName() { Name = "parentTestOrganizationName", TypeId = NameTypeEnum.Name.ToString().GetGuid() } }, DisplayNameTypeId = NameTypeEnum.Name.ToString().GetGuid() };
//            var childOrganization1 = new OrganizationVersioned() { Parent = parentOrganization, OrganizationNames = new List<OrganizationName>() { new OrganizationName() { Name = "childTestOrganizationName1", TypeId = NameTypeEnum.Name.ToString().GetGuid() } }, DisplayNameTypeId = NameTypeEnum.Name.ToString().GetGuid() };
//            var childOrganization2 = new OrganizationVersioned() { Parent = parentOrganization, OrganizationNames = new List<OrganizationName>() { new OrganizationName() { Name = "childTestOrganizationName2", TypeId = NameTypeEnum.AlternateName.ToString().GetGuid() } }, DisplayNameTypeId = NameTypeEnum.AlternateName.ToString().GetGuid() };
//
//            return new OrganizationVersioned()
//            {
//                Created = DateTime.Now,
//                CreatedBy = "testCreator",
//                Id = Guid.NewGuid(),
//                OrganizationNames = new List<OrganizationName>() { new OrganizationName() { Name = "testOrganizationName", TypeId = NameTypeEnum.Name.ToString().GetGuid() } },
//                Parent = parentOrganization,
//                Children = new List<OrganizationVersioned>() { childOrganization1, childOrganization2 },
//                DisplayNameTypeId = NameTypeEnum.Name.ToString().GetGuid()
//            };
//        }
    }
}
