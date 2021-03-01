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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ILoadMessagesService), RegisterType.Transient)]
    internal class LoadMessagesService : ILoadMessagesService
    {
        private readonly IWebHostEnvironment environment;
        private readonly string devRootDirectory = Path.Combine("..", "PTV.Application.Web");

        private IContextManager contextManager;
        private ITranslationEntity translationToViewModel;
        private const string JsonFileExtension = ".json";

        public LoadMessagesService(IWebHostEnvironment environment, IContextManager contextManager, ITranslationEntity translationToViewModel)
        {
            this.environment = environment;
            this.contextManager = contextManager;
            this.translationToViewModel = translationToViewModel;
        }

        public IEnumerable<IVmLanguageMessages> Load(DateTime? loadTime = null)
        {
            var dbMessages = LoadMessagesFromDatabase(loadTime);
            if (dbMessages == null)
            {
                return null;
            }
            var localizationDir = new DirectoryInfo(environment.GetDirectoryPath(devRootDirectory, Path.Combine("wwwroot", "localization")));
            var fileMessages = localizationDir
                .GetFiles($"*{JsonFileExtension}")
                .Select(
                    file => LoadMessagesFromFile(file, file.Name.Replace(JsonFileExtension, string.Empty)));

            var result = MergeTexts(dbMessages, fileMessages);
            return result;
            ;
        }

        private IEnumerable<IVmLanguageMessages> MergeTexts(IDictionary<string, IVmLanguageMessages> databaseMessages,
            IEnumerable<IVmLanguageMessages> fileMessages)
        {
            return fileMessages.Select(fm =>
            {
                var dbLanguageMessages = databaseMessages.TryGet(fm.LanguageCode);
                if (dbLanguageMessages == null)
                {
                    return fm;
                }

                if (fm.Modified > dbLanguageMessages.Modified)
                {
                    return fm;
                }

                if (!fm.Texts.Keys.Except(dbLanguageMessages.Texts.Keys).Any())
                {
                    return dbLanguageMessages;
                }

                var missingKeys = fm.Texts.Keys.Except(dbLanguageMessages.Texts.Keys).ToList();
                missingKeys.ForEach(key => dbLanguageMessages.Texts[key] = fm.Texts[key]);

                return dbLanguageMessages;
            }).ToList();
        }

        private static IVmLanguageMessages LoadMessagesFromFile(FileInfo file, string language)
        {
            if (file.Exists)
            {
                var json = File.ReadAllText(file.FullName);
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                var messages = new VmLanguageMessages
                {
                    LanguageCode = language.ToLower(),
                    Texts = data,
                    Modified = DateTime.MinValue
                };
                if (long.TryParse(data.TryGet("timestamp"), out long timestamp))
                {
                    messages.Modified = timestamp.FromEpochTime();
                }
                return messages;
            }
            return null;
        }

        private IDictionary<string, IVmLanguageMessages> LoadMessagesFromDatabase(DateTime? loadTime)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<ILocalizationRepository>();
                var query = rep.All();
                if (loadTime.HasValue && !query.Any(x => x.Modified > loadTime))
                {
                    return null;
                }

                return translationToViewModel.TranslateAll<Localization, IVmLanguageMessages>(query)
                    .ToDictionary(x => x.LanguageCode);
            });
        }
    }
}