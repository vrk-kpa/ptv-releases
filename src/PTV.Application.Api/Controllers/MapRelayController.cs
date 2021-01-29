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
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PTV.Framework;

namespace PTV.Application.Api.Controllers
{
    /// <summary>
    /// Relaying controller for map calls
    /// </summary>
    [Route("api/maprelay")]
    [AllowAnonymous]
    [Controller]
    public class MapRelayController
    {
        private Dictionary<string, Uri> MapUri { get; }
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<MapRelayController> logger;

        /// <summary>
        /// Initialize controller
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <param name="mapAddresses"></param>
        /// <param name="logger"></param>
        public MapRelayController(IHttpContextAccessor httpContextAccessor, IOptions<MapDNSes> mapAddresses, ILogger<MapRelayController> logger)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
            this.MapUri = new Dictionary<string, Uri>
            {
                { "fi", new Uri(mapAddresses.Value.Fi) },
                { "sv", new Uri(mapAddresses.Value.Sv) },
                { "en", new Uri(mapAddresses.Value.En) },
            };
        }


        /// <summary>
        /// Method providing relaying for map data
        /// </summary>
        /// <returns></returns>
        [HttpGet("first/{language}")]
        [HttpPost("first/{language}")]
        [AllowAnonymous]
        public HttpResponseMessage MapRelay(string language)
        {
            var languageUri = MapUri.TryGet(language.ToLower());
            if (languageUri == default(Uri))
            {
                logger.LogCritical($"Configuration for Map relay not found! Language: '{language}'");
            }
            var queryString = httpContextAccessor.HttpContext.Request.QueryString;
            var uri = new Uri(languageUri, queryString.Value ?? string.Empty);
            switch (httpContextAccessor.HttpContext.Request.Method.ToUpper())
            {
                case "GET":
                {
                    using (var httpClient = new HttpClient())
                    {
                        var request = new HttpRequestMessage(HttpMethod.Get, uri);
                        using (var content = new StringContent(string.Empty, Encoding.UTF8, "text/html,application/xhtml+xml,application/xml"))
                        {
                            request.Content = content;
                            var response = httpClient.SendAsync(request).Result;
                            var firstResult = response.Content.ReadAsStringAsync().Result;
//                                var newUri = response.RequestMessage.RequestUri;
//                                response = httpClient.GetAsync(newUri).Result;
//                                var secondResult = response.Content.ReadAsStringAsync().Result;
                            Console.WriteLine("--------------");
                            Console.WriteLine(firstResult);
                            Console.WriteLine("--------------");
//                                Console.WriteLine(secondResult);
//                                Console.WriteLine("--------------");
                            return response;}



                    }
                }
                case "POST":
                {
                    using (var httpClient = new HttpClient())
                    {
                        HttpContent content = new StreamContent(httpContextAccessor.HttpContext.Request.Body);
                        var response = httpClient.PostAsync(uri, content).Result;
                        return response;
                    }
                }
                default: return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
            }
        }

        /// <summary>
        /// Method providing relaying for map data
        /// </summary>
        /// <returns></returns>
        [HttpGet("next")]
        [HttpPost("next")]
        [AllowAnonymous]
        public HttpResponseMessage MapRelayNext()
        {
            var languageUri = new Uri("http://www.paikkatietoikkuna.fi");
            var queryString = httpContextAccessor.HttpContext.Request.QueryString;
            var uri = new Uri(languageUri, queryString.Value ?? string.Empty);
            switch (httpContextAccessor.HttpContext.Request.Method.ToUpper())
            {
                case "GET":
                {
                    using (var httpClient = new HttpClient())
                    {
                        var response = httpClient.GetAsync(uri).Result;
                        return response;
                    }
                }
                case "POST":
                {
                    using (var httpClient = new HttpClient())
                    {
                        HttpContent content = new StreamContent(httpContextAccessor.HttpContext.Request.Body);
                        var response = httpClient.PostAsync(uri, content).Result;
                        return response;
                    }
                }
                default: return new HttpResponseMessage(HttpStatusCode.MethodNotAllowed);
            }
        }
    }
}
