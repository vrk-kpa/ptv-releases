﻿/**
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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.OpenApi.Common;
using PTV.Database.DataAccess.Translators.OpenApi.Finto;
using PTV.Database.DataAccess.Translators.OpenApi.Services;
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Tests.TestHelpers;
using FluentAssertions;
using PTV.Database.DataAccess.Interfaces.Repositories;

namespace PTV.Database.DataAccess.Tests.Translators.OpenApi
{
    public class OpenApiServiceIndustrialClassTranslatorTest : TranslatorTestBase
    {
        private const string INDUSTRIAL_CLASS_ID = "B41FDE69-4487-4F23-85B2-17AFD9C9C15B";

        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;

        public OpenApiServiceIndustrialClassTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;

            translators = new List<object>
            {
                new OpenApiServiceIndustrialClassTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock(new Mock<ITranslator<IndustrialClass, string>>(), unitOfWorkMock, industrialClass => new IndustrialClass() { Id = new Guid(INDUSTRIAL_CLASS_ID) })
            };
        }

        [Theory]
        [InlineData(INDUSTRIAL_CLASS_ID)]
        public void TranslateVmToEntity(string vModel)
        {
            var toTranslate = new List<VmOpenApiStringItem>() { new VmOpenApiStringItem { Value = vModel } };
            var translations = RunTranslationModelToEntityTest<VmOpenApiStringItem, ServiceIndustrialClass>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            CheckTranslation(vModel, translation);
        }

        private void CheckTranslation(string source, ServiceIndustrialClass target)
        {
            target.IndustrialClass.Id.Should().Be(source);
        }
    }
}
