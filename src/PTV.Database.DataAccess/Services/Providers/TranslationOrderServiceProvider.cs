using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
                foreach (var e in ae.Flatten().InnerExceptions)
                {
                    throw new PtvTranslationException("Download file error", e);
                }
            }
            catch (Exception e)
            {
                throw new PtvTranslationException("Download file error", e);
            }
        }

        internal async Task<string> DownloadTranslationOrderJson(string url, ProxyServerSettings proxySettings)
        {
            WebProxy proxy = null;

            if ((proxySettings != null) && !proxySettings.Address.IsNullOrEmpty() && !proxySettings.Port.IsNullOrEmpty())
            {
                var proxyUri = string.Format("{0}:{1}", proxySettings.Address, proxySettings.Port);
                proxy = new WebProxy(proxyUri);

                if (!string.IsNullOrEmpty(proxySettings.UserName) && !string.IsNullOrEmpty(proxySettings.Password))
                {
                    proxy.Credentials = new NetworkCredential(proxySettings.UserName, proxySettings.Password);
                }
            }
            
            using (HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy
            })
            
            using (var client = new HttpClient(httpClientHandler))
            {
                client.DefaultRequestHeaders.Add("User-Agent", "XXX");
                return await client.GetStringAsync(new Uri(url));
            }
        }
    }
}
