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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models.Interfaces.Localization;
using PTV.Domain.Model.Models.Localization;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IMessagesService), RegisterType.Transient)]
    public class MessagesService : IMessagesService
    {
        private readonly IHostingEnvironment environment;
        private readonly string devRootDirectory = @"..\PTV.Application.Web";
        private string jsonFileExtension = ".json";

        public MessagesService(IHostingEnvironment environment)
        {
            this.environment = environment;
        }

        /// <summary>
        /// Gets ui localized text for application
        /// </summary>
        /// <returns></returns>
        public IVmLocalizationMessages GetMessages()
        {
            var localizationDir = new DirectoryInfo(environment.GetFilePath(devRootDirectory, Path.Combine("wwwroot", "localization")));
            var result = new VmLocalizationMessages();
            foreach (var file in localizationDir.GetFiles($"*{jsonFileExtension}"))
            {
                LoadMessages(file, result, file.Name.Replace(jsonFileExtension, string.Empty));
            }

            return result;
        }

        private static void LoadMessages(FileInfo file, VmLocalizationMessages result, string language)
        {
            if (file.Exists)
            {
                var json = File.ReadAllText(file.FullName);
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                result.Translations.Add
                (
                    new VmLanguageMessages
                    {
                        LanguageCode = language.ToLower(),
                        Texts = data
                    }
                );
            }
            else
            {
                result.Translations.Add(new VmLanguageMessages
                {
                    LanguageCode = language,
                    Texts = new Dictionary<string, string> { { file.Name, file.FullName } }
                });
            }
        }
    }
}