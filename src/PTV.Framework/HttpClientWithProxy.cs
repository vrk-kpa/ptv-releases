using System;
using System.Net;
using System.Net.Http;
using PTV.Framework.Extensions;

namespace PTV.Framework
{
    /// <summary>
    /// Creates new HttpClient object including proxy settings
    /// </summary>
    public static class HttpClientWithProxy
    {
        public static void Use(ProxyServerSettings proxySettings, Action<HttpClient> action)
        {
            Use(proxySettings, client => {
                action(client);
                return (object)null;
            });
        }

        public static T Use<T>(ProxyServerSettings proxySettings, Func<HttpClient, T> action)
        {
            WebProxy proxy = null;
            if ((proxySettings != null) && (!string.IsNullOrEmpty(proxySettings.Address)))
            {
                string proxyUri = string.Format("{0}:{1}", proxySettings.Address, proxySettings.Port);
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
                using (var httpclient = new HttpClient(httpClientHandler))
                {
                    return action(httpclient);
                }
        }
    }
}