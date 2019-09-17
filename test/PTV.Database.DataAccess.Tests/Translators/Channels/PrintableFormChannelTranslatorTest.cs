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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using Xunit;
using FluentAssertions;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Database.DataAccess.Translators.Channels.V2;
using PTV.Database.DataAccess.Translators.Channels.V2.PrintableForm;
using PTV.Domain.Model.Models.V2.Channel.PrintableForm;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class PrintableFormChannelTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;

        public PrintableFormChannelTranslatorTest()
        {
            translators = new List<object>()
            {
                new PrintableFormTranslator(ResolveManager, TranslationPrimitives, new EntityDefinitionHelper(CacheManager)),
                RegisterTranslatorMock<PrintableFormChannelUrl, VmWebPage>(),
                RegisterTranslatorMock<PrintableFormChannelIdentifier, VmPrintableFormChannelIdentifier>(),
                RegisterTranslatorMock<PrintableFormChannelIdentifier, string>(),
            };
            RegisterDbSet(new List<PrintableFormChannel>(), unitOfWorkMockSetup);
        }


        /// <summary>
        /// test for PrintableFormChannelStep1TranslatorTest vm - > entity
        /// </summary>
        [Theory]
        [InlineData("fi")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("fi;sv;en")]
        public void TranslatePrintableFormChannelToEntity(string list)
        {
            var model = new VmPrintableForm
            {
                FormIdentifier = list?.Split(";").Where(x => !string.IsNullOrEmpty(x)).ToDictionary(x => x),
                FormFiles = list?.Split(";").Where(x => !string.IsNullOrEmpty(x)).ToDictionary(x => x, x => new List<VmWebPage>
                {
                    new VmWebPage { UrlAddress = x }
                })
            };

            var translation = RunTranslationModelToEntityTest<VmPrintableForm, PrintableFormChannel>(translators, model, unitOfWorkMockSetup.Object);
            CheckTranslation(model, translation);
        }
        
        private void CheckTranslation(VmPrintableForm source, PrintableFormChannel target)
        {
            if (source.FormIdentifier != null)
            {
                target.FormIdentifiers.Count.Should().Be(source.FormIdentifier.Count);
            }
            else
            {
                target.FormIdentifiers.Should().BeEmpty();
            }

            if (source.FormFiles != null)
            {
                target.ChannelUrls.Count.Should().Be(source.FormFiles.Count);
            }
            else
            {
                target.ChannelUrls.Should().BeEmpty();
            }
        }


        /// <summary>
        /// test for PrintableFormChannelStep1TranslatorTest entity - > vm
        /// </summary>
        [Theory]
        [InlineData("fi")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("fi;sv;en")]
        public void TranslatePrintableFormChannelToModel(string list)
        {
            var toTranslate = new PrintableFormChannel
            {
                FormIdentifiers = list?.Split(";").Where(x => !string.IsNullOrEmpty(x)).Select(x => 
                    new PrintableFormChannelIdentifier { FormIdentifier = x, LocalizationId = x.GetGuid() }).ToList()
                ,
            };

            var translation = RunTranslationEntityToModelTest<PrintableFormChannel, VmPrintableForm>(translators, toTranslate);

            CheckTranslation(toTranslate, translation);           
        }

        private void CheckTranslation(PrintableFormChannel source, VmPrintableForm  target)
        {
            if (source.FormIdentifiers != null)
            {
                target.FormIdentifier.Count.Should().Be(source.FormIdentifiers.Count);
            }
            else
            {
                target.FormIdentifier.Should().BeNull();
            }

            if (source.ChannelUrls != null)
            {
                target.FormFiles.Count.Should().Be(source.ChannelUrls.Count);
            }
            else
            {
                target.FormFiles.Should().BeNull();
            }
        }
    }
}
