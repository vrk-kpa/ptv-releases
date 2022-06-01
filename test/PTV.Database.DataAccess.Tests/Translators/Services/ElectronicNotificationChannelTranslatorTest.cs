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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.Services;
using PTV.Database.Model.Models;
using PTV.Framework.ServiceManager;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Services
{
    public class ElectronicNotificationChannelTranslatorTest : TranslatorTestBase
    {

        /// <summary>
        /// test for ElectronicNotificationChannel vm - > entity
        /// </summary>
        [Fact]
        public void RunTestVmToEntity()
        {
            var translator = new ElectronicNotificationChannelTranslator(ResolveManager, TranslationPrimitives);
            var languageTranslator = new LanguageCodeTranslator(ResolveManager, TranslationPrimitives);
            List<ITranslator> translators = new List<ITranslator> { translator, languageTranslator };
            var toTranslate = new List<string> { "NotificationChannelText" };

            var translations = RunTranslationModelToEntityTest<string, ServiceElectronicNotificationChannel>(translators, toTranslate);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(toTranslate.First(), translation.ElectronicNotificationChannel);
            Assert.NotEqual(translation.LocalizationId, Guid.Empty);
        }
    }
}
