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
using PTV.Database.DataAccess.Translators.Channels;
using PTV.Database.Model.Interfaces;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Domain.Model.Models.Interfaces;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class WebPageChannelTranslatorTest : TranslatorTestBase
    {
        private readonly List<object> translators;
        private readonly IUnitOfWork unitOfWorkMock;

        public WebPageChannelTranslatorTest()
        {
            unitOfWorkMock = unitOfWorkMockSetup.Object;
//#warning needs refactoring to separated tests
            translators = new List<object>()
            {
                new WebPageChannelTranslator(ResolveManager, TranslationPrimitives),
                new WebPageChannelUrlTranslator(ResolveManager, TranslationPrimitives),
                new ServiceChannelDescriptionTranslator(ResolveManager, TranslationPrimitives),
                new ServiceChannelNameTranslator(ResolveManager, TranslationPrimitives),
                new ServiceChannelTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new PublishingStatusTypeTranslator(ResolveManager, TranslationPrimitives),
                new NameTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new DescriptionTypeCodeTranslator(ResolveManager, TranslationPrimitives),
                new WebPageChannelMainTranslator(ResolveManager, TranslationPrimitives),
                new WebPageChannelStep1Translator(ResolveManager, TranslationPrimitives, CacheManager, new ServiceChannelTranslationDefinitionHelper(CacheManager)),
                new WebPageChannelMainStep1Translator(ResolveManager, TranslationPrimitives,LanguageCache),
                new ServiceChannelLanguageTranslator(ResolveManager, TranslationPrimitives,LanguageCache),
                new WebPageChannelWebPageUrlTranslator(ResolveManager, TranslationPrimitives),
                RegisterTranslatorMock(new Mock<ITranslator<Language, string>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelVersioned, IVmChannelDescription>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelEmail, VmEmailData>>(), unitOfWorkMock),
                RegisterTranslatorMock(new Mock<ITranslator<ServiceChannelPhone, VmPhone>>(), unitOfWorkMock)
            };

            RegisterDbSet(CreateCodeData<ServiceChannelType>(typeof(ServiceChannelTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<PublishingStatusType>(typeof(PublishingStatus)), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceChannelVersioned>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<WebpageChannel>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<WebpageChannelUrl>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceChannelName>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceChannelDescription>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<Attachment>(), unitOfWorkMockSetup);
            RegisterDbSet(new List<ServiceChannelServiceHours>(), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<AttachmentType>(typeof(AttachmentTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<NameType>(typeof(NameTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<DescriptionType>(typeof(DescriptionTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<ServiceHourType>(typeof(ServiceHoursTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(CreateCodeData<ExceptionHoursStatusType>(typeof(ExceptionHoursStatus)), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for WebPageChannelTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateWebPageChannelToEntityWebpageChannel()
        {
            var model = TestHelper.CreateVmWebPageChannelModel();
            var toTranslate = new List<VmWebPageChannel>() { model };

            var translations = RunTranslationModelToEntityTest<VmWebPageChannel, WebpageChannel>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
        }

        /// <summary>
        /// test for WebPageChannelMainTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateWebPageChannelToEntityServiceChannel()
        {
            var model = TestHelper.CreateVmWebPageChannelModel();
            var toTranslate = new List<VmWebPageChannel>() { model };

            var translations = RunTranslationModelToEntityTest<VmWebPageChannel, ServiceChannelVersioned>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
//            translation.Languages.Count.Should().Be(model.Step1Form.Languages.Count);
//            Assert.Equal(model.PublishingStatus, translation.PublishingStatusId);
//            Assert.Equal(model.Step1Form.OrganizationId, translation.OrganizationId);
//            Assert.Equal(ServiceChannelTypeEnum.WebPage.ToString(), translation.Type.Code);
        }

        /// <summary>
        /// test for WebPageChannelUrlTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateWebPageChannelUrlToEntity()
        {
            var model = TestHelper.CreateVmChannelUrlModel();
            var toTranslate = new List<VmChannelAttachment>() { model };

            var translations = RunTranslationModelToEntityTest<VmChannelAttachment, WebpageChannelUrl>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(model.UrlAddress, translation.Url);
        }
    }
}
