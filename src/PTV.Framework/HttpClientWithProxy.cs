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
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace PTV.Framework
{
    /// <summary>
    /// Creates new HttpClient object including proxy settings
    /// </summary>
    public static class HttpClientWithProxy
    {
        private static WebProxy GetProxy(ProxyServerSettings proxySettings)
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

            return proxy;
        }

        private static async Task<T> UseAsync<T>(ProxyServerSettings proxySettings, X509Certificate2 clientCertificate, Func<HttpClient, Task<T>> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            WebProxy proxy = GetProxy(proxySettings);
            using (HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy,
                UseProxy = proxy != null
            })
            {
                if (ignoreServerCertificate)
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true;
                }
                if (clientCertificate != null)
                {
                    httpClientHandler.ClientCertificates.Add(clientCertificate);
                    httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                }
                httpClientHandler.SslProtocols = SslProtocols.Tls11 | SslProtocols.Tls12;
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(timeoutSecs);
                    httpClient.CancelPendingRequests();
                    return await action(httpClient);
                }
            }
        }

        private static T Use<T>(ProxyServerSettings proxySettings, X509Certificate2 clientCertificate, Func<HttpClient, T> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            WebProxy proxy = GetProxy(proxySettings);
            using (HttpClientHandler httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy,
                UseProxy = proxy != null
            })
            {
                if (ignoreServerCertificate)
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true;
                }
                if (clientCertificate != null)
                {
                    httpClientHandler.ClientCertificates.Add(clientCertificate);
                    httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                }
                httpClientHandler.SslProtocols = SslProtocols.Tls11 | SslProtocols.Tls12;
                using (var httpClient = new HttpClient(httpClientHandler))
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(timeoutSecs);
                    httpClient.CancelPendingRequests();
                    return action(httpClient);
                }
            }
        }
        

        /// <summary>
        /// Create HttpClient with proxy
        /// </summary>
        /// <param name="proxySettings"></param>
        /// <param name="action"></param>
        /// <param name="timeoutSecs"></param>
        /// <param name="ignoreServerCertificate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Use<T>(ProxyServerSettings proxySettings, Func<HttpClient, T> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            return Use(proxySettings, (X509Certificate2)null, action, timeoutSecs, ignoreServerCertificate);
        }
        
        /// <summary>
        /// Create HttpClient with proxy
        /// </summary>
        /// <param name="proxySettings"></param>
        /// <param name="clientPfxCertificateFile"></param>
        /// <param name="action"></param>
        /// <param name="timeoutSecs"></param>
        /// <param name="ignoreServerCertificate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Use<T>(ProxyServerSettings proxySettings, string clientPfxCertificateFile, Func<HttpClient, T> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            return Use(proxySettings, new X509Certificate2(clientPfxCertificateFile), action, timeoutSecs, ignoreServerCertificate);
        }
        
        /// <summary>
        /// Create HttpClient with proxy
        /// </summary>
        /// <param name="proxySettings"></param>
        /// <param name="clientPfxCertificateData"></param>
        /// <param name="action"></param>
        /// <param name="timeoutSecs"></param>
        /// <param name="ignoreServerCertificate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Use<T>(ProxyServerSettings proxySettings, byte[] clientPfxCertificateData, Func<HttpClient, T> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            return Use(proxySettings, new X509Certificate2(clientPfxCertificateData), action, timeoutSecs, ignoreServerCertificate);
        }

        /// <summary>
        /// Create HttpClient with proxy
        /// </summary>
        /// <param name="proxySettings"></param>
        /// <param name="action"></param>
        /// <param name="timeoutSecs"></param>
        /// <param name="ignoreServerCertificate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> UseAsync<T>(ProxyServerSettings proxySettings, Func<HttpClient, Task<T>> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            return await UseAsync(proxySettings, (X509Certificate2)null, action, timeoutSecs, ignoreServerCertificate);
        }
        
        /// <summary>
        /// Create HttpClient with proxy
        /// </summary>
        /// <param name="proxySettings"></param>
        /// <param name="clientPfxCertificateFile"></param>
        /// <param name="action"></param>
        /// <param name="timeoutSecs"></param>
        /// <param name="ignoreServerCertificate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> UseAsync<T>(ProxyServerSettings proxySettings, string clientPfxCertificateFile, Func<HttpClient, Task<T>> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            return await UseAsync(proxySettings, new X509Certificate2(clientPfxCertificateFile), action, timeoutSecs, ignoreServerCertificate);
        }
        
        /// <summary>
        /// Create HttpClient with proxy
        /// </summary>
        /// <param name="proxySettings"></param>
        /// <param name="clientPfxCertificateData"></param>
        /// <param name="action"></param>
        /// <param name="timeoutSecs"></param>
        /// <param name="ignoreServerCertificate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> UseAsync<T>(ProxyServerSettings proxySettings, byte[] clientPfxCertificateData, Func<HttpClient, Task<T>> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            return await UseAsync(proxySettings, new X509Certificate2(clientPfxCertificateData), action, timeoutSecs, ignoreServerCertificate);
        }
        
        /// <summary>
        /// Create HttpClient with proxy
        /// </summary>
        /// <param name="proxySettings"></param>
        /// <param name="action"></param>
        /// <param name="timeoutSecs"></param>
        /// <param name="ignoreServerCertificate"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static void Use(ProxyServerSettings proxySettings, Action<HttpClient> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            Use(proxySettings, client => {
                action(client);
                return (object)null;
            }, timeoutSecs, ignoreServerCertificate);
        }
        
        /// <summary>
        /// Create HttpClient with proxy
        /// </summary>
        /// <param name="proxySettings"></param>
        /// <param name="action"></param>
        /// <param name="timeoutSecs"></param>
        /// <param name="ignoreServerCertificate"></param>
        /// <returns></returns>
        public static async void UseAsync(ProxyServerSettings proxySettings, Func<HttpClient, Task> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            await UseAsync(proxySettings, async client => {
                await action(client);
                return (object)null;
            }, timeoutSecs, ignoreServerCertificate);
        }
        
        /// <summary>
        /// Call Post action and get result as string
        /// </summary>
        /// <param name="proxyServerSettings"></param>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static HttpCallResponse CallPostToString(ProxyServerSettings proxyServerSettings, string requestUri, HttpContent content)
        {
            return RequestCallToString(proxyServerSettings, c => c.PostAsync(requestUri, content));
        }
        
        /// <summary>
        /// Call Get action and get result as string
        /// </summary>
        /// <param name="proxyServerSettings"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public static HttpCallResponse CallGetToString(ProxyServerSettings proxyServerSettings, string requestUri)
        {
            return RequestCallToString(proxyServerSettings, c => c.GetAsync(requestUri));
        }

        private static HttpCallResponse RequestCallToString(ProxyServerSettings proxyServerSettings, Func<HttpClient,Task<HttpResponseMessage>> action)
        {
            return Asyncs.HandleAsyncInSync(() => UseAsync(proxyServerSettings, async client =>
            {
                try
                {
                    var response = await action(client);
                    if (response?.Content == null)
                    {
                        return new HttpCallResponse();
                    }
                    var data = await response.Content.ReadAsStringAsync();
                    return new HttpCallResponse(data, response.StatusCode);
                }
                catch (Exception e)
                {
                    return new HttpCallResponse(e);
                }
            }));
        }
    }

    public class HttpCallResponse
    {
        public string Data { get; private set; } = string.Empty;
        public HttpStatusCode StatusCode { get; private set; } = HttpStatusCode.NoContent;
        public Exception Exception { get; private set; } = null;

        internal HttpCallResponse()
        {
        }
        
        internal HttpCallResponse(string data, HttpStatusCode statusCode)
        {
            this.Data = data;
            this.StatusCode = statusCode;
        }
        
        internal HttpCallResponse(Exception exception)
        {
            this.Exception = exception;
        }

        public void ThrowExceptionIfOccured()
        {
            if (Exception != null)
            {
                throw Exception;
            }
        }
    }
}