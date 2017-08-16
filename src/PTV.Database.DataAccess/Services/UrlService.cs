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
using Microsoft.Extensions.Options;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IUrlService), RegisterType.Transient)]
    internal class UrlService : IUrlService
    {
        private ApplicationConfiguration configuration;
        private readonly ProxyServerSettings proxySettings;

        public UrlService(ApplicationConfiguration configuration, IOptions<ProxyServerSettings> proxySettings)
        {
            this.configuration = configuration;
            this.proxySettings = proxySettings.Value;
        }

        public IVmUrlChecker CheckUrl(IVmUrlChecker model)
        {
            if (!configuration.IsInternetAvailable())
            {
                model.UrlExists = null;
//                return new VmUrlChecker() { UrlExists = null, Id = model.Id, UrlAddress = model.UrlAddress };
                return model;
            }

            WebProxy proxy = null;

            if ((proxySettings != null) && (!string.IsNullOrEmpty(proxySettings.Address)))
            {
                string proxyUri = string.Format("{0}:{1}", proxySettings.Address, proxySettings.Port);
                proxy = new WebProxy(proxyUri);

                if (!string.IsNullOrEmpty(proxySettings.UserName) && !string.IsNullOrEmpty(proxySettings.Password))
                {
                    NetworkCredential proxyCreds = new NetworkCredential(proxySettings.UserName, proxySettings.Password);
                    proxy.Credentials = proxyCreds;
                }
            }
            try
            {
                var uri = new UriBuilder(model.UrlAddress).Uri;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

                request.ContinueTimeout = 1000;
                request.Method = "HEAD";
                request.Proxy = proxy;

                var task = request.GetResponseAsync();
                task.Wait();
                using (var response = (HttpWebResponse)task.Result)
                {
                    model.UrlExists = response.StatusCode == HttpStatusCode.OK;
                    return model;
                }

            }
            catch (Exception)
            {
                model.UrlExists = false;
                return model;
            }
        }
    }
}