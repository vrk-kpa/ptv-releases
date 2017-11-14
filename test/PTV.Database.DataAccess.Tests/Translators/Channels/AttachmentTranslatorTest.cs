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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Languages;
using PTV.Database.DataAccess.Translators.Types;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using Xunit;
using PTV.Database.DataAccess.Translators.Channels;
using FluentAssertions;
using System;
using Moq;
using PTV.Database.DataAccess.Tests.Translators.Common;
using PTV.Framework;

namespace PTV.Database.DataAccess.Tests.Translators.Channels
{
    public class AttachmentTranslatorTest : TranslatorTestBase
    {
        private List<object> translators;
        private IUnitOfWork unitOfWorkMock;

        public AttachmentTranslatorTest()
        {
            var unitOfWorkMockSetup = new Mock<IUnitOfWork>();
            unitOfWorkMock = unitOfWorkMockSetup.Object;
            SetupTypesCacheMock<AttachmentType>();
            translators = new List<object>()
            {
                new AttachmentTranslator(ResolveManager, TranslationPrimitives, CacheManager),
            };

            RegisterDbSet(CreateCodeData<AttachmentType>(typeof(AttachmentTypeEnum)), unitOfWorkMockSetup);
            RegisterDbSet(new List<Attachment>(), unitOfWorkMockSetup);
        }

        /// <summary>
        /// test for AttachmentTranslator vm - > entity
        /// </summary>
        [Fact]
        public void TranslateAttachmentToEntity()
        {
            var model = TestHelper.CreateVmChannelAttachmentModel();
            model.Id = null;
            var toTranslate = new List<VmChannelAttachment>() { model };

            var translations = RunTranslationModelToEntityTest<VmChannelAttachment, Attachment>(translators, toTranslate, unitOfWorkMock);
            var translation = translations.First();

            Assert.Equal(toTranslate.Count, translations.Count);
            Assert.Equal(model.Description, translation.Description);
            Assert.Equal(model.Name, translation.Name);
            Assert.Equal(model.UrlAddress, translation.Url);
            Assert.Equal(AttachmentTypeEnum.Attachment.ToString().GetGuid(), translation.TypeId);
            translation.Id.Should().NotBe(Guid.Empty);
        }
    }

}
