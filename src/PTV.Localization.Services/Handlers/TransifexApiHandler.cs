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
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Localization.Services.Model;

namespace PTV.Localization.Services.Handlers
{

    [RegisterService(typeof(TransifexApiHandler), RegisterType.Transient)]
    public class TransifexApiHandler
    {
        Dictionary<string, string> languageMapping = new Dictionary<string, string>
        {
            { "en", "en_GB"},
            { "fi", "fi_FI"},
            { "sv", "sv_SE"},
            { "af", "af"}
        };

//        private TransifexConfiguration configuration;

        private ProxyServerSettings proxySettings;

        public TransifexApiHandler(IOptions<TransifexConfiguration> configuration, IOptions<ProxyServerSettings> proxySettings)
        {
            Configuration = configuration.Value;
            this.proxySettings = proxySettings.Value;
            this.proxySettings.OverrideBy(Configuration?.ProxyServerSettings);
        }

        private TransifexConfiguration Configuration { get; set; }
//        public TransifexConfiguration Configuration
//        {
//            get { return configuration = configuration ?? appConfiguration.GetConfiguration<TransifexConfiguration>("transifex"); }
//            set => configuration = value;
//        }

        private string GetLocalization(string languageCode)
        {
            Console.WriteLine(languageCode);
            if (languageMapping.TryGetValue(languageCode, out string localization))
            {
                return localization;
            }
            throw new ArgumentOutOfRangeException($"Language code should be one of {string.Join(',', languageMapping.Values)}, but was {languageCode}");
        }

        public string DownloadTranslations(string languageCode)
        {

//            return HttpClientWithProxy.Use(null, client =>
//            {
//                SetHeader(client);
//                var url = GetUrl(client, languageCode);
//
//            });
            return CallTransifexApi(languageCode, (client, url) =>
            {
                Console.WriteLine($"download transifex from '{url}'");
                var result = client.GetStringAsync(url);
                return result.Result;
            });
        }

        private T CallTransifexApi<T>(string languageCode, Func<HttpClient, string, T> callAction)
        {
            return HttpClientWithProxy.Use(proxySettings, client =>
            {
                SetHeader(client);
                var url = GetUrl(languageCode);
                Console.WriteLine($"url: {url}");
                return callAction(client, url);
            });
        }
        public string UploadTranslations(string languageCode, string content)
        {
            Console.WriteLine($"Upload for {languageCode}");
            Console.WriteLine($"Content: '{content.Length}'");
            var response = CallTransifexApi
            (
                languageCode,
                (client, url) =>
                {
                    Console.WriteLine($"upload transifex to '{url}'");
                    var result = client.PutAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));
                    return result.Result;
                }
            );
            var putResult = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(putResult);
            return putResult;
        }

//        public string DownloadTranslations(string languageCode, string userName, string password)
//        {
//            using (var handler = new HttpClientHandler())
//            {
//                handler.Credentials = new NetworkCredential
//                {
//                    UserName = userName,
//                    Password = password
//                };
//
//                using (var client = new HttpClient(handler))
//                {
//                    return Download(client, languageCode);
//                }
//            }
//        }

        private string GetUrl(string languageCode)
        {
            string localization = GetLocalization(languageCode);
            if (string.IsNullOrEmpty(localization))
            {
                return null;
            }

            return Configuration.GetUrl(localization, ParamType);
        }

        public string ParamType { get; set; }

        private void SetHeader(HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(Configuration.Authorization)));

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
