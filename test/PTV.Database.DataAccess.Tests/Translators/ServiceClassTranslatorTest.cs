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
using Moq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators;
using PTV.Database.DataAccess.Translators.Common;
using PTV.Database.DataAccess.Translators.Finto;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Framework.ServiceManager;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators
{
    public class ServiceClassTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;

        public ServiceClassTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            var unitOfWorkMock = unitOfWorkMockSetup.Object;

            var languageOrderCacheMock = new Mock<ILanguageOrderCache>();
            languageOrderCacheMock.Setup(i => i.Get(It.IsAny<Guid>())).Returns(1);
            translators = new List<object>
            {
                new ServiceClassTranslator(ResolveManager, TranslationPrimitives),
                new FintoListTranslatorBase(ResolveManager, TranslationPrimitives, new TranslatedItemDefinitionHelper()),
                RegisterTranslatorMock(new Mock<ITranslator<INameReferences, IVmTranslatedItem>>(), unitOfWorkMock, model => new ServiceClass()),
            };

        }

        /// <summary>
        /// test for ServiceClassTranslator entity - > vm
        /// </summary>
        [Fact]
        public void TranslateServiceClassesToViewModel()
        {

            var serviceClassName = CreateServiceClass();
            var toTranslate = new List<ServiceClass>() { serviceClassName };
            var translations = RunTranslationEntityToModelTest<ServiceClass, VmListItem>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(serviceClassName.Label, translation.Name);
            Assert.Equal(serviceClassName.Id, translation.Id);

        }

        private ServiceClass CreateServiceClass()
        {
            return new ServiceClass()
            {
                Label = "testServiceClass",
                Id = Guid.NewGuid()
            };
        }

    }
}
