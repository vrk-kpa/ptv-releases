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
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.Framework;

namespace PTV.LocalAuthentication
{
    public interface ITokenIntrospecter
    {
        bool IntrospectToken(string token);
    }


    public interface IOAuthWellKnownConfigurationHolder
    {
        OAuthKnownConfiguration OAuthKnownConfiguration { get; }
    }

    [RegisterService(typeof(IOAuthWellKnownConfigurationHolder), RegisterType.Singleton)]
    public class OAuthWellKnownConfigurationHolder : IOAuthWellKnownConfigurationHolder
    {
        private OAuthKnownConfiguration configuration;
        private readonly ILogger<OAuthWellKnownConfigurationHolder> logger;
        private OAuthSettings options;

        public OAuthWellKnownConfigurationHolder(ILogger<OAuthWellKnownConfigurationHolder> logger, OAuthSettings options)
        {
            this.logger = logger;
            this.options = options;
        }
        
        public OAuthKnownConfiguration OAuthKnownConfiguration
        {
            get
            {
                if (configuration == null)
                {
                    HttpClientWithProxy.UseAsync(new ProxyServerSettings(), async http =>
                    {
                        if (string.IsNullOrEmpty(options.Authority))
                        {
                            logger.LogError("OAuth authority URL in settings missing!");
                        }

                        using (var httpResult =
                            await http.GetAsync(CoreExtensions.CombineUris(options.Authority,
                                ".well-known/openid-configuration")))
                        {
                            if (httpResult?.Content != null)
                            {
                                var stringResult = await httpResult.Content.ReadAsStringAsync();

                                var knownConfs =
                                    JsonConvert.DeserializeObject<OAuthKnownConfiguration>(
                                        stringResult ?? string.Empty);
                                if (string.IsNullOrEmpty(knownConfs.Introspection_endpoint))
                                {
                                    logger.LogError("OAuth Introspection_endpoint is missing!");
                                }
                                else
                                {
                                    configuration = knownConfs;
                                }
                            }
                        }
                    });
                }
                return configuration;
            }
        }
    }

    [RegisterService(typeof(ITokenIntrospecter), RegisterType.Transient)]
    public class TokenIntrospecter : ITokenIntrospecter
    {
        public TokenIntrospecter(ILogger<TokenIntrospecter> logger, IOAuthWellKnownConfigurationHolder oAuthWellKnownConfigurationHolder)
        {
            this.logger = logger;
            this.oAuthWellKnownConfigurationHolder = oAuthWellKnownConfigurationHolder;
        }
        
        private readonly ILogger<TokenIntrospecter> logger;
        private readonly IOAuthWellKnownConfigurationHolder oAuthWellKnownConfigurationHolder;

        public bool IntrospectToken(string token)
        {
            return Asyncs.HandleAsyncInSync(() => HttpClientWithProxy.UseAsync(new ProxyServerSettings(), async http =>
            {
                var url = oAuthWellKnownConfigurationHolder?.OAuthKnownConfiguration?.Introspection_endpoint;
                if (string.IsNullOrEmpty(url))
                {
                    logger.LogError("OAuth Introspection_endpoint is missing!");
                    return false;
                }
                logger.LogDebug($"Introspecting token on {url}, token {token}");
                
                try
                {
                    using (HttpContent content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                        {new KeyValuePair<string, string>("token", token)}))
                    {
                        using (var httpResponse = await http.PostAsync(url, content))
                        {
                            var httpContent = httpResponse?.Content == null
                                ? string.Empty
                                : await httpResponse.Content.ReadAsStringAsync();
                            IntrospectResponse response =
                                JsonConvert.DeserializeObject<IntrospectResponse>(httpContent ?? string.Empty);
                            logger.LogInformation($"Token is Active = {response.Active}");
                            return response.Active;
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.LogError("IntrospectToken failed in HTTP request", e);
                    return false;
                }
            }));
        }

        private class IntrospectRequest
        {
            public string Token { get; set; }
        }
        
        private class IntrospectResponse
        {
            public bool Active { get; set; }
        }
    }
    
    public class OAuthKnownConfiguration
    {
        public string Issuer { get; set; }
        public string Jwks_uri { get; set; }
        public string Token_endpoint { get; set; }
        public string Userinfo_endpoint { get; set; }
        public string Introspection_endpoint { get; set; }
        public string Revocation_endpoint { get; set; }
    }
}