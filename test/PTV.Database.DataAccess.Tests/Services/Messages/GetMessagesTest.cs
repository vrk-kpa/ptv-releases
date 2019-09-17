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
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Services;
using PTV.Database.DataAccess.Tests.TestHelpers;
using PTV.Database.DataAccess.Tests.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework;
using Xunit;

namespace PTV.Database.DataAccess.Tests.Services.Messages
{
    public class GetMessagesTest : TestBase
    {
        private readonly MessagesService messagesService;
        private readonly ItemListModelGenerator modelGenerator = new ItemListModelGenerator();
        private Mock<ILocalizationMessagesCache> localizationCache;

        public GetMessagesTest() : base(new Mock<IUnitOfWorkWritable>(MockBehavior.Strict))
        {
            localizationCache = new Mock<ILocalizationMessagesCache>(MockBehavior.Strict);
            messagesService = new MessagesService(null, null, localizationCache.Object);
        }

        [Theory]
        [InlineData("fi", new [] { 1 })]
        [InlineData("sv;fi", new [] { 1 })]
        public void GetAllTest(string languages, int[] indexes)
        {
            var localizationData = modelGenerator.CreateList(languages, language => 
                new VmLanguageMessages
                {
                    LanguageCode = language,
                    Texts = indexes
                        .Select(index => new { Key = $"key{index}", Text = $"text {index} {languages}"})
                        .ToDictionary(x => x.Key, x => x.Text)
                }
            ).ToDictionary<IVmLanguageMessages, string>(x => x.LanguageCode);

            localizationCache.Setup(x => x.GetData()).Returns(localizationData);
            
            var messages = messagesService.GetMessages();
            
            localizationCache.Verify(x => x.GetData(), Times.Once());
            messages.Should().HaveSameCount(localizationData);
            localizationData.Values.ForEach(x => { messages.Should().Contain(x); });
            
        }
    }
}