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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services.Providers
{
    [RegisterService(typeof(TranslationOrderServiceProvider), RegisterType.Singleton)]
    public class TranslationOrderServiceProvider
    {
        private readonly ITranslationService translationService;

        public TranslationOrderServiceProvider(ITranslationService translationService)
        {
            this.translationService = translationService;
        }

        public async Task DownloadTranslationOrderFile(string url, Guid translationOrderId, ProxyServerSettings proxySettings)
        {
            try
            {
                var result = await DownloadTranslationOrderJson(url, proxySettings);
                translationService.ProcessTranslatedFile(translationOrderId, result);
            }
            catch (AggregateException ae)
            {
                throw new PtvTranslationException($"Download file error: {CoreExtensions.ExtractAllInnerExceptions(ae)}", ae.Flatten());
            }
            catch (Exception e)
            {
                throw new PtvTranslationException("Download file error", e);
            }
        }

        internal async Task<string> DownloadTranslationOrderJson(string url, ProxyServerSettings proxySettings)
        {
            return await HttpClientWithProxy.UseAsync(proxySettings, async client =>
            {
                client.DefaultRequestHeaders.Add("User-Agent", "XXX");
                return await client.GetStringAsync(new Uri(url));
            });
        }
    }
}
