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

        /// <summary>
        /// Create HttpClient with proxy
        /// </summary>
        /// <param name="proxySettings"></param>
        /// <param name="action"></param>
        /// <param name="timeoutSecs"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Use<T>(ProxyServerSettings proxySettings, Func<HttpClient, T> action, int timeoutSecs = 180)
        {
            WebProxy proxy = null;
            if ((proxySettings != null) && (!string.IsNullOrEmpty(proxySettings.Address)) && (proxySettings.Disable.Not(bool.TrueString)))
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
                Proxy = proxy,
                UseProxy = proxy != null
            })
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(timeoutSecs);
                    httpClient.CancelPendingRequests();
                    return action(httpClient);
                }
        }
    }
}