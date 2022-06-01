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
using System.Diagnostics;
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
    public static class PtvHttpClient
    {
        private static async Task<T> UseAsync<T>(X509Certificate2 clientCertificate, Func<HttpClient, Task<T>> action, int timeoutSecs = 180, bool ignoreServerCertificate = false, bool allowAutoRedirect = true)
        {
            using (HttpClientHandler httpClientHandler = new HttpClientHandler{AllowAutoRedirect = allowAutoRedirect})
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

        private static T Use<T>(X509Certificate2 clientCertificate, Func<HttpClient, T> action, int timeoutSecs = 180, bool ignoreServerCertificate = false, bool allowAutoRedirect = true)
        {
            using (HttpClientHandler httpClientHandler = new HttpClientHandler{AllowAutoRedirect = allowAutoRedirect})
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
        
        public static T Use<T>(Func<HttpClient, T> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            return Use((X509Certificate2)null, action, timeoutSecs, ignoreServerCertificate);
        }

        public static T UseWithoutAutoRedirect<T>(Func<HttpClient, T> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            return Use((X509Certificate2)null, action, timeoutSecs, ignoreServerCertificate, false);
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
        public static T Use<T>(string clientPfxCertificateFile, Func<HttpClient, T> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            return Use(new X509Certificate2(clientPfxCertificateFile), action, timeoutSecs, ignoreServerCertificate);
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
        public static T Use<T>(byte[] clientPfxCertificateData, Func<HttpClient, T> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            return Use(new X509Certificate2(clientPfxCertificateData), action, timeoutSecs, ignoreServerCertificate);
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
        public static async Task<T> UseAsync<T>(Func<HttpClient, Task<T>> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            return await UseAsync((X509Certificate2)null, action, timeoutSecs, ignoreServerCertificate);
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
        public static async Task<T> UseAsync<T>(string clientPfxCertificateFile, Func<HttpClient, Task<T>> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            return await UseAsync(new X509Certificate2(clientPfxCertificateFile), action, timeoutSecs, ignoreServerCertificate);
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
        public static async Task<T> UseAsync<T>(byte[] clientPfxCertificateData, Func<HttpClient, Task<T>> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            return await UseAsync(new X509Certificate2(clientPfxCertificateData), action, timeoutSecs, ignoreServerCertificate);
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
        public static void Use(Action<HttpClient> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            Use(client => {
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
        public static async void UseAsync(Func<HttpClient, Task> action, int timeoutSecs = 180, bool ignoreServerCertificate = false)
        {
            await UseAsync(async client => {
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
        public static HttpCallResponse CallPostToString(string requestUri, HttpContent content)
        {
            return RequestCallToString(c => c.PostAsync(requestUri, content));
        }

        /// <summary>
        /// Call Get action and get result as string
        /// </summary>
        /// <param name="proxyServerSettings"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public static HttpCallResponse CallGetToString(string requestUri)
        {
            return RequestCallToString(c => c.GetAsync(requestUri));
        }

        private static HttpCallResponse RequestCallToString(Func<HttpClient,Task<HttpResponseMessage>> action)
        {
            return Asyncs.HandleAsyncInSync(() => UseAsync(async client =>
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
