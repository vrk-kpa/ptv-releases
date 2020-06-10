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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.OpenApi.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using FluentAssertions;

namespace PTV.Database.DataAccess.Tests.Translators.OpenApi
{
    public class OpenApiServiceTargetGroupTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;

        public OpenApiServiceTargetGroupTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;

            translators = new List<object>
            {
                new OpenApiServiceTargetGroupInTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock(new Mock<ITranslator<TargetGroup, string>>(), unitOfWorkMock, targetGroup => new TargetGroup { Uri = TestDataFactory.URI })
            };
        }

        [Theory]
        [InlineData(TestDataFactory.URI)]
        public void TranslateVmToEntity(string vModel)
        {
            var toTranslate = new List<VmOpenApiStringItem> { new VmOpenApiStringItem { Value = vModel } };
            var translations = RunTranslationModelToEntityTest<VmOpenApiStringItem, ServiceTargetGroup>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(1, translations.Count);
            CheckTranslations(vModel, translation);
        }

        private void CheckTranslations(string source, ServiceTargetGroup target)
        {
            target.TargetGroup.Uri.Should().Be(source);
        }
    }
}
