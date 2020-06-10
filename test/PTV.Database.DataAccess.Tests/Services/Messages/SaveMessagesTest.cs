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
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Moq;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Domain.Model.Models.Localization;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Messages
{
    public class SaveMessagesTest : TestBase
    {
        private readonly MessagesService messagesService;
        private readonly Mock<ITranslationViewModel> translateToEntity;

        private readonly string testFolder = Path.Combine("..", "..", "..", "Services", "Messages");

        public SaveMessagesTest() : base(new Mock<IUnitOfWorkWritable>(MockBehavior.Strict))
        {
            var environmentMock = new Mock<IWebHostEnvironment>(MockBehavior.Strict);
            environmentMock.SetupGet(x => x.ContentRootPath).Returns(testFolder);
            translateToEntity = new Mock<ITranslationViewModel>(MockBehavior.Strict);
            messagesService = new MessagesService(contextManagerMock.Object, translateToEntity.Object, null);
        }

        [Fact]
        public void SaveTest()
        {
            SetupContextManager<object, Dictionary<string, IVmLanguageMessages>>();
            var messages = new List<IVmLanguageMessages>
            {
                new VmLanguageMessages { LanguageCode = "fi", Texts = new Dictionary<string, string> { { "key1", "text5" }}}
            };
            translateToEntity
                .Setup(x => x.TranslateAll<IVmLanguageMessages, Localization>(It.IsAny<IEnumerable<IVmLanguageMessages>>(), unitOfWorkMockSetup.Object))
                .Returns((IEnumerable<IVmLanguageMessages> input, IUnitOfWorkWritable unitOfWork) =>
                {
                    input.Should().BeSameAs(messages);
                    unitOfWork.Should().NotBeNull();
                    return new List<Localization>();
                });
            unitOfWorkMockSetup
                .Setup(x => x.Save(SaveMode.AllowAnonymous, PreSaveAction.Standard, null, null))
                .Returns(0);

            messagesService.SaveMessages(messages);

            VerifyContextManagerCalls<object, object>(Times.Never(), Times.Never(), Times.Once());
            unitOfWorkMockSetup.Verify
            (
                x => x.Save(SaveMode.AllowAnonymous, PreSaveAction.Standard, null, null),
                Times.Once
            );
        }
    }
}
