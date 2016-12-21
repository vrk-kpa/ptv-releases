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

namespace PTV.Database.DataAccess.Tests.Translators.Organizations
{
    public class OrganizationListItemTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;

        public OrganizationListItemTranslatorTest()
        {
            var typesCacheMock = new Mock<ITypesCache>();
            var languagesCacheMock = new Mock<ILanguageCache>();
            translators = new List<object>()
            {
                new OrganizationListItemTranslator(ResolveManager, TranslationPrimitives, typesCacheMock.Object, languagesCacheMock.Object),
                new DateTimeLongValueTranslator(),
                new PublishingStatusTranslator(ResolveManager, TranslationPrimitives)
        };
        }

        /// <summary>
        /// test for OrganizationListItemTranslator entity - > vm
        /// </summary>
        [Fact]
        public void TranslateOrganizationsToViewModel()
        {
            var organization = CreateOrganization();
            var toTranslate = new List<Organization>() { organization };
            var translations = RunTranslationEntityToModelTest<Organization, VmOrganizationListItem>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(translations.Count, toTranslate.Count);
            Assert.Equal(translation.Modified, organization.Modified.ToEpochTime());
            Assert.Equal(translation.ModifiedBy, organization.ModifiedBy);
            Assert.Equal(translation.Id, organization.Id);
            Assert.Equal(translation.Name, organization.OrganizationNames.First().Name);
            Assert.Equal(translation.MainOrganization, organization.Parent.Parent.OrganizationNames.First().Name + ", " + organization.Parent.OrganizationNames.First().Name);
            Assert.Equal(translation.SubOrganizations, organization.Children.ElementAt(0).OrganizationNames.First().Name + ", " +organization.Children.ElementAt(1).OrganizationNames.First().Name);
        }

        private Organization CreateOrganization()
        {
            var topParentOrganization = new Organization() { OrganizationNames = new List<OrganizationName>() { new OrganizationName() { Name = "topParentTestOrganizationName", Type = new NameType() { Code = NameTypeEnum.Name.ToString() } } } };
            var parentOrganization = new Organization() { Parent = topParentOrganization, OrganizationNames = new List<OrganizationName>() { new OrganizationName() { Name = "parentTestOrganizationName", Type = new NameType() { Code = NameTypeEnum.Name.ToString() } } } };
            var childOrganization1 = new Organization() { Parent = parentOrganization, OrganizationNames = new List<OrganizationName>() { new OrganizationName() { Name = "childTestOrganizationName1", Type = new NameType() { Code = NameTypeEnum.Name.ToString() } } } };
            var childOrganization2 = new Organization() { Parent = parentOrganization, OrganizationNames = new List<OrganizationName>() { new OrganizationName() { Name = "childTestOrganizationName2", Type = new NameType() { Code = NameTypeEnum.Name.ToString() } } } };

            return new Organization()
            {
                Created = DateTime.Now,
                CreatedBy = "testCreator",
                Id = Guid.NewGuid(),
                OrganizationNames = new List<OrganizationName>() { new OrganizationName() { Name = "testOrganizationName", Type = new NameType() { Code = NameTypeEnum.Name.ToString() } } },
                Parent = parentOrganization,
                Children = new List<Organization>() { childOrganization1, childOrganization2 },
            };
        }
    }
}
