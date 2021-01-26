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
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Services
{
    public class ServiceOrganizerTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;
        private readonly IUnitOfWork unitOfWorkMock;

        public ServiceOrganizerTranslatorTest()
        {
            translators = new List<object>
            {
                new ServiceOrganizerTranslator(ResolveManager, TranslationPrimitives)
            };
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            RegisterDbSet(new List<OrganizationService>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceVersioned>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for ServiceNameTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("C01D6342-BBF2-4943-A2AE-2DCE06C9D813")]
        public void TranslateServiceOrganizerEntityTest(string organizationId)
        {
            var model = CreateModel(organizationId);
            var toTranslate = new List<VmTreeItem> { model };

            var translations = RunTranslationModelToEntityTest<VmTreeItem, OrganizationService>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }

        private static void CheckTranslation(VmListItem source, OrganizationService target)
        {
            target.ServiceVersionedId.Should().Be(Guid.Empty);
            target.OrganizationId.Should().Be(source.Id);
        }

        private static VmTreeItem CreateModel(string organizationId)
        {
            return new VmTreeItem
            {
                Id = Guid.Parse(organizationId),
            };
        }

    }
}
