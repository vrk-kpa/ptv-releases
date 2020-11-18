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
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Services
{

    public class ServiceProducerAdditionalInformationTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;

        public ServiceProducerAdditionalInformationTranslatorTest()
        {
            translators = new List<object>
            {
                new ServiceProducerAdditionalInformationTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock<ServiceProducerAdditionalInformation, VmServiceProducer>()
            };
            RegisterDbSet(new List<ServiceProducerAdditionalInformation>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for ServiceNameTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("Test", "sv")]
        [InlineData("", null)]
        public void TranslateToEntityTest(string name, string language)
        {
            var model = new VmServiceProducerAdditionalInformation { LocalizationId = language.GetGuid(), Text = name };

            var translation = RunTranslationModelToEntityTest<VmServiceProducerAdditionalInformation, ServiceProducerAdditionalInformation>(translators, model, unitOfWorkMockSetup.Object);
            CheckTranslation(model, translation);
        }

        private void CheckTranslation(VmServiceProducerAdditionalInformation source, ServiceProducerAdditionalInformation target)
        {
            target.LocalizationId.Should().Be(source.LocalizationId ?? DomainConstants.DefaultLanguage.GetGuid());
            target.Text.Should().Be(source.Text);
        }
    }
}
