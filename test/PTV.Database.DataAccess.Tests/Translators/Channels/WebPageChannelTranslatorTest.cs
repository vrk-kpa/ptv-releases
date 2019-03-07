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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using Xunit;
using PTV.Database.Model.Interfaces;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Database.DataAccess.Translators.Channels.V2.WebPage;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class WebPageChannelTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;
        private readonly IUnitOfWork unitOfWorkMock;

        public WebPageChannelTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            translators = new List<object>()
            {
                new WebPageChannelTranslator(ResolveManager, TranslationPrimitives, CacheManager),
                RegisterTranslatorMock(new Mock<ITranslator<WebpageChannelUrl, VmWebPage>>(), unitOfWorkMock, model => new WebpageChannelUrl { Url = model.UrlAddress }),
                RegisterTranslatorMock<IText, string>
                (
                    model => new WebpageChannelUrl { Url = model },
                    entity => entity.Text
                ),
            };

            RegisterDbSet(new List<WebpageChannel>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for WebPageChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("fi")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("fi;sv;en")]
        public void TranslateToEntity(string urls)
        {
            var model = new VmWebPageChannel
            {
                WebPage = urls?.Split(";").Where(x => !string.IsNullOrEmpty(x))
                    .ToDictionary(x => x)
            };
            var translation = RunTranslationModelToEntityTest<VmWebPageChannel, WebpageChannel>(translators, model, unitOfWorkMock);
            CheckTranslation(model, translation);
        }

        private void CheckTranslation(VmWebPageChannel source, WebpageChannel translation)
        {
            translation.LocalizedUrls.Count.Should().Be(source.WebPage?.Count ?? 0);
            if (source.WebPage?.Count > 0)
            {
                translation.LocalizedUrls.Select(x => x.Url).Should().Contain(source.WebPage.Values);
            }
        }
        
        /// <summary>
        /// test for WebPageChannelMainTranslator vm - > entity
        /// </summary>
        [Theory]
        [InlineData("fi")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("fi;sv;en")]
        public void TranslateToViewModel(string urls)
        {
            var entity = new WebpageChannel
            {
                LocalizedUrls = urls?.Split(";").Where(x => !string.IsNullOrEmpty(x)).Select(x =>
                    new WebpageChannelUrl { Url = x, LocalizationId = x.GetGuid() }).ToList()
            };
            var translation = RunTranslationEntityToModelTest<WebpageChannel, VmWebPageChannel>(translators, entity);
            CheckTranslation(entity, translation);
        }

        private void CheckTranslation(WebpageChannel source, VmWebPageChannel translation)
        {
            if (source.LocalizedUrls != null)
            {
                translation.WebPage.Count.Should().Be(source.LocalizedUrls.Count);
                if (source.LocalizedUrls?.Count > 0)
                {
                    translation.WebPage.Select(x => x.Value).Should().Contain(source.LocalizedUrls.Select(x => x.Url));
                }
            }
            else
            {
                translation.WebPage.Should().BeNull();
            }
        }
        
    }
}
