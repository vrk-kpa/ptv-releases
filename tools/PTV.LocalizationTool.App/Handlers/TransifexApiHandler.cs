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
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.LocalizationTool.App.Handlers
{
   
    [RegisterService(typeof(TransifexApiHandler), RegisterType.Transient)]
    internal class TransifexApiHandler
    {
        Dictionary<string, string> languageMapping = new Dictionary<string, string>
        {
            { "en", "en_GB"},
            { "fi", "fi_FI"},
            { "sv", "sv_SE"},
            { "af", "af"}
        };
        
        public TransifexApiHandler(IConfigurationRoot configuration)
        {
            Configuration = configuration.Get<TransifexConfiguration>();
        }

        public TransifexConfiguration Configuration { get; private set; }

        private string GetLocalization(string languageCode)
        {
            Console.WriteLine(languageCode);
            if (languageMapping.TryGetValue(languageCode, out string localization))
            {
                return localization;
            }
            throw new ArgumentOutOfRangeException($"Language code should be one of {string.Join(',', languageMapping.Values)}, but was {languageCode}");
        }

        public string DownloadTranslations(string languageCode, string authorization)
        {

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authorization)));
                return Download(client, languageCode);
            }

        }

        public string DownloadTranslations(string languageCode, string userName, string password)
        {
            using (var handler = new HttpClientHandler())
            {
                handler.Credentials = new NetworkCredential
                {
                    UserName = userName,
                    Password = password
                };

                using (var client = new HttpClient(handler))
                {
                    return Download(client, languageCode);
                }
            }
        }

        private string Download(HttpClient client, string languageCode)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string localization = GetLocalization(languageCode);
            if (string.IsNullOrEmpty(localization))
            {
                return null;
            }

//            var url = $"https://www.transifex.com/api/2/project/{Project}/resource/fijson/translation/{localization}?mode=onlytranslated";
            var url = Configuration.GetUrl(localization);
            Console.WriteLine($"download transifex from '{url}'");
            var result = client
                    .GetStringAsync(
                        url)
//                        "https://www.transifex.com/api/2/project/fsc-dev/resource/fijson/translation/en_GB")
                ;
            return result.Result;
        }
    }
    
    internal class TransifexConfiguration 
    {
        public string Project { get; set; }
        public string Url { get; set; }
        public Dictionary<string, TransifexProject> Projects { get; set; } = new Dictionary<string, TransifexProject>();

        public string GetUrl(string localization)
        {         
            var project = Projects.TryGet(Project) ?? throw new ArgumentException($"No projects found for {Project} in {string.Join(", ", Projects.Keys)}.");
            return Url
                .Replace("{project}", project.Name)
                .Replace("{resource}", project.Resource)
                .Replace("{localization}", localization);
        }
    
    }

    internal class TransifexProject
    {
        public string Name { get; set; }
        public string Resource { get; set; }
    }

    [RegisterService(typeof(CommandHandlerManager), RegisterType.Singleton)]
    internal class CommandHandlerManager
    {
        private Dictionary<string, Type> handlers;
        private IResolveManager resolveManager;

        public CommandHandlerManager(IEnumerable<ITranslationDataHandler> handlers, IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
            this.handlers = handlers.ToDictionary(x => x.Key, x => x.GetType());
//            Console.WriteLine($"Register handlers: {string.Join(",", this.handlers.Select(x => $"{x.Key} - {x.Value}" ))}");
        }

        public ITranslationDataHandler Get(string key)
        {
//            Console.WriteLine("get handler: " + key);
            var type = handlers.TryGet(key);
            return type != null ? resolveManager.Resolve(type) as ITranslationDataHandler : null;
        }
        
        public T Get<T>(Action<T> initAction) where T : class, ITranslationDataHandler
        {
//            Console.WriteLine("get handler: " + key);
            
            var handler = resolveManager.Resolve<T>();
            if (handler != null)
            {
                initAction?.Invoke(handler);
            }

            return handler;
        }
    }
}