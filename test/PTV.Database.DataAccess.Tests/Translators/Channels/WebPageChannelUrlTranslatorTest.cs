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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class WebPageChannelUrlTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;

        public WebPageChannelUrlTranslatorTest()
        {
            translators = new List<object>()
            {
                new WebPageChannelUrlTranslator(ResolveManager, TranslationPrimitives),
            };

            RegisterDbSet(new List<WebpageChannelUrl>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for WebPageChannelUrlTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateWebPageChannelUrlToEntity()
        {
            var model = TestHelper.CreateVmChannelUrlModel();
            var toTranslate = new List<VmChannelAttachment>() { model };

            var translations = RunTranslationModelToEntityTest<VmChannelAttachment, WebpageChannelUrl>(translators, toTranslate, unitOfWorkMockSetup.Object);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(model.UrlAddress, translation.Url);
        }
    }
}