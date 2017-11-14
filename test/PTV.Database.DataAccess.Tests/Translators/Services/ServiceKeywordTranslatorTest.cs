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
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Services
{
    public class ServiceKeywordTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;

        public ServiceKeywordTranslatorTest()
        {
            translators = new List<object>
            {
                new ServiceKeyWordTranslator(ResolveManager, TranslationPrimitives),

            };
            RegisterDbSet(new List<ServiceKeyword>(), unitOfWorkMockSetup);
            unitOfWorkMock = unitOfWorkMockSetup.Object;
        }

        /// <summary>
        /// test for ServiceNameTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("", "Hallo")]
        [InlineData("", "")]
        [InlineData("D6A4D10B-BA0A-45A1-B874-40F572A18C5C", "Hallo")]
        [InlineData("D6A4D10B-BA0A-45A1-B874-40F572A18C5C", "")]
        public void TranslateToEntityTest(string id, string keyword)
        {
            var model = CreateModel(id, keyword);
            var toTranslate = new List<VmKeywordItem>() { model };

            translators.Add(
                RegisterTranslatorMock(
                    new Mock<ITranslator<Keyword, VmKeywordItem>>(),
                    unitOfWorkMock,
                    x => new Keyword { Name = x.Name, Id = Guid.NewGuid() })
                );

            var translations = RunTranslationModelToEntityTest<VmKeywordItem, ServiceKeyword>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(model, translation);
        }

        private void CheckTranslation(VmKeywordItem source, ServiceKeyword target)
        {
            if (!source.Id.IsAssigned())
            {
                target.Keyword.Should().NotBeNull();
                target.Keyword.Id.Should().NotBeEmpty();
                target.Keyword.Name.Should().Be(source.Name);
            }
            else
            {
                target.KeywordId.Should().Be(source.Id.Value);
            }
        }

        private VmKeywordItem CreateModel(string id, string keyword)
        {
            return new VmKeywordItem
            {
                Id = id.ParseToGuid(),
                Name = keyword
            };
        }

    }
}
