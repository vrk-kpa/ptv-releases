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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NLog.Web.LayoutRenderers;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.TaskScheduler.Configuration;

namespace PTV.TaskScheduler.Controllers
{
    /// <summary>
    /// Controller is providing relay point(s) for another service calls
    /// </summary>
    [Route("relaying")]
    public class RelayController : Controller
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly RelayingTranslationOrderConfiguration relayingOptions;
        private readonly ProxyServerSettings proxySettings;
        private const string ContentType = "text/xml";

        public RelayController(IHttpContextAccessor contextAccessor, IOptions<RelayingTranslationOrderConfiguration> relayingOptions, IOptions<ProxyServerSettings> proxyServerSettings)
        {
            this.contextAccessor = contextAccessor;
            this.relayingOptions = relayingOptions.Value;
            this.proxySettings = proxyServerSettings.Value;
        }

        /// <summary>
        /// Relaying point for Lingsoft, translation orders
        /// </summary>
        /// <returns></returns>
        [HttpPost("LingSoftRelay")]
        public ObjectResult LingSoftRelay()
        {
            return HttpClientWithProxy.Use(proxySettings, httpClient =>
            {
                using (HttpContent content = new StreamContent(contextAccessor.HttpContext.Request.Body))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
                    var credence = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{relayingOptions.UserName}:{relayingOptions.Password}"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credence);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType));
                    string soapAction = contextAccessor.HttpContext.Request.Headers["SOAPAction"].FirstOrDefault();
                    httpClient.DefaultRequestHeaders.Add("SOAPAction", soapAction);
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "XXX");
                    httpClient.BaseAddress = new Uri(relayingOptions.UrlBase);
                    var targetResponse = httpClient.PostAsync(relayingOptions.UrlBase, content).Result;
                    Stream serviceResponse = targetResponse.Content.ReadAsStreamAsync().Result;
                    contextAccessor.HttpContext.Response.Headers["Content-Type"] = ContentType;
                    return new ObjectResult(serviceResponse);
                }
            });
        }
    }
}
