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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
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
    public class LoadMessagesTest : TestBase
    {
        private readonly LoadMessagesService messagesService;
        private readonly Mock<ITranslationEntity> translateToModel;
        private readonly ItemListModelGenerator modelGenerator = new ItemListModelGenerator();
        
        private readonly string testFolder = Path.Combine("..", "..", "..", "Services", "Messages");

        public LoadMessagesTest() : base(new Mock<IUnitOfWorkWritable>(MockBehavior.Strict))
        {
            var environmentMock = new Mock<IHostingEnvironment>(MockBehavior.Strict);
            environmentMock.SetupGet(x => x.ContentRootPath).Returns(testFolder);
            translateToModel = new Mock<ITranslationEntity>(MockBehavior.Strict);
            messagesService = new LoadMessagesService(environmentMock.Object, contextManagerMock.Object, translateToModel.Object);
        }

        public static IEnumerable<object[]> LocalizationData()
        {
            yield return new object[]
            {
                new List<Localization>
                    {new Localization {LanguageCode = "fi", Modified = new DateTime(2019, 04, 1)}}
            };
            yield return new object[]
            {
                new List<Localization>
                {
                    new Localization {LanguageCode = "fi", Modified = new DateTime(2019, 04, 1)},
                    new Localization {LanguageCode = "sv", Modified = new DateTime(2019, 06, 1)}
                }
            };
            yield return new object[]
            {
                new List<Localization>
                {
                    new Localization {LanguageCode = "fi", Modified = new DateTime(2019, 06, 1)},
                    new Localization {LanguageCode = "sv", Modified = new DateTime(2019, 06, 12)}
                }
            };
        }

        [Theory]
        [InlineData("fi", new [] { 1 })]
        [InlineData("fi", new [] { 1, 2 })]
        [InlineData("fi", new [] { 1, 3 })]
        [InlineData("fi", new [] { 3, 4 })]
        [InlineData("fi", new [] { 1, 2, 3, 4 })]
        [InlineData("fi", new int[] {})]  
        [InlineData("sv", new [] { 1 })]
        [InlineData("en", new [] { 1, 2 })]
        public void GetAllTest(string languages, int[] indexes)
        {
            var directory = new DirectoryInfo(Path.Combine(testFolder, "wwwroot", "localization"));
            directory.Exists.Should().BeTrue();
            
            var files = new[] {"fi", "en"};
            var sourceFiles = files
                .Select(language => new
                {
                    language,
                    data = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(directory.FullName, $"{language}.json")))
                })
                .ToDictionary(x => x.language, x => x.data);
            
            var localizationData = modelGenerator.CreateList(languages, language => 
                new VmLanguageMessages
                {
                    LanguageCode = language,
                    Texts = indexes
                        .Select(index => new { Key = $"key{index}", Text = $"text {index} {languages}"})
                        .ToDictionary(x => x.Key, x => x.Text),
                    
                }
            ).ToList();
            var dbDataForCheck =
                localizationData.ToDictionary(x => x.LanguageCode, x => x.Texts.ToDictionary(t => t.Key, t => t.Value));
            
            SetupContextManager<object, Dictionary<string, IVmLanguageMessages>>();
            RegisterRepository<ILocalizationRepository, Localization>(new TestDbSet<Localization>(new List<Localization>()));
            translateToModel
                .Setup(x => x.TranslateAll<Localization, IVmLanguageMessages>(It.IsAny<IQueryable<Localization>>()))
                .Returns(localizationData);
            
            var messages = messagesService.Load();
            
            messages.Should().HaveCount(files.Length);
            var messagesByLanguage = messages.ToDictionary(x => x.LanguageCode);
            sourceFiles.ForEach(pair =>
            {
                var (language, fileTexts) = pair;
                messagesByLanguage.Should().ContainKey(language);
                if (dbDataForCheck.ContainsKey(language))
                {
                    var languageTextFromDb = dbDataForCheck[language];
                    messagesByLanguage[language].Texts.Should().ContainKeys(fileTexts.Keys);
                    if (languageTextFromDb.Count > 0)
                    {
                        messagesByLanguage[language].Texts.Should().ContainKeys(languageTextFromDb.Keys);
                        messagesByLanguage[language].Texts.Should().ContainValues(languageTextFromDb.Values);

                        fileTexts.ForEach(x =>
                            {
                                var (key, value) = x;
                                if (languageTextFromDb.ContainsKey(key))
                                {
                                    // text replaced
                                    messagesByLanguage[language].Texts.Should().NotContainValue(value);
                                }
                                else
                                {
                                    // original text used
                                    messagesByLanguage[language].Texts.Should().ContainValue(value);
                                }
                            }
                        );
                    }
                    else
                    {
                        messagesByLanguage[language].Texts.Should().ContainValues(fileTexts.Values);
                    }
                }
                else
                {
                    messagesByLanguage[language].Texts.Should().ContainKeys(fileTexts.Keys);
                    messagesByLanguage[language].Texts.Should().ContainValues(fileTexts.Values);
                }
            });
            
        }
        
        [Theory]
        [MemberData(nameof(LocalizationDataWithDate))]
        internal void GetWithDateCheck(List<Localization> data)
        {
            var directory = new DirectoryInfo(Path.Combine(testFolder, "wwwroot", "localization"));
            directory.Exists.Should().BeTrue();
            
            var files = new[] {"fi", "en"};
            
            var localizationData = data
                .Select(x => new VmLanguageMessages { LanguageCode = x.LanguageCode, Texts = new Dictionary<string, string> { { x.LanguageCode, x.LanguageCode }}})
                .ToList();            

            SetupContextManager<object, Dictionary<string, IVmLanguageMessages>>();
            RegisterRepository<ILocalizationRepository, Localization>(new TestDbSet<Localization>(data));
            translateToModel
                .Setup(x => x.TranslateAll<Localization, IVmLanguageMessages>(It.IsAny<IQueryable<Localization>>()))
                .Returns(localizationData);

            var loadTime = new DateTime(2019, 5, 19);
            var messages = messagesService.Load(loadTime);
            if (data.Any(x => x.Modified > loadTime))
            {
                messages.Should().HaveCount(files.Length);    
            }
            else
            {
                messages.Should().BeNull();
            }
        }

        public static IEnumerable<object[]> LocalizationDataWithDate => 
            new List<object[]>
            {
                new object[] {new List<Localization> {new Localization {LanguageCode = "fi", Modified = new DateTime(2019, 04, 1)}}},
                new object[] {new List<Localization>
                {
                    new Localization {LanguageCode = "fi", Modified = new DateTime(2019, 04, 1)},
                    new Localization {LanguageCode = "sv", Modified = new DateTime(2019, 06, 1)}
                }},
                new object[] {new List<Localization>
                {
                    new Localization {LanguageCode = "fi", Modified = new DateTime(2019, 06, 1)},
                    new Localization {LanguageCode = "sv", Modified = new DateTime(2019, 06, 12)}
                }},
            };
        
    }
}