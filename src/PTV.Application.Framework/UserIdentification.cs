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
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;

namespace PTV.Application.Framework
{
    [RegisterService(typeof(IUserIdentification), RegisterType.Scope)]
    public class UserIdentification : IUserIdentification, IThreadUserInterface
    {
        private readonly IHttpContextAccessor contextAccessor;
        private ApplicationConfiguration appConfiguration;
        private ILogger<UserIdentification> logger;
        private const string UniqueClaimIdentifier = "email";
        private string userName;

        /// <summary>
        /// Status flag if userinfo is already fetched or set by IThreadUserInterface.SetUserName
        /// </summary>
        private bool userinfoFetched = false;

        /// <summary>
        /// Userinfo event for logging.
        /// </summary>
        public static EventId UserinfoEvent => new EventId(5995, "Userinfo"); // on purpose always returning new, same pattern used in .net (core) framework

        public string UserName
        {
            get
            {
                // have we tried to fetch userinfo or is it set with IThreadUserInterface.SetUserName already
                if (!userinfoFetched)
                {
                    // just set it to true already, if there is an exception it should also fail on next calls in this scope
                    // it would be really weird situation that during the same request this might fail and succeed in different calling code paths
                    userinfoFetched = true;

                    var uniqueClaim = contextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(i => i.Type == UniqueClaimIdentifier);

                    if ((uniqueClaim == null) && (!string.IsNullOrEmpty(appConfiguration.StsAddress)))
                    {
                        string bearer = string.Empty;
                        string userEmail = string.Empty;

                        var authHeader = contextAccessor?.HttpContext?.Request?.Headers["Authorization"];

                        if (authHeader != default(StringValues?))
                        {
                            bearer = string.Join("", authHeader).Split(' ').LastOrDefault() ?? string.Empty;
                        }

                        if (string.IsNullOrEmpty(bearer))
                        {
                            logger.LogWarning("No token found for requested user name!");
                            return string.Empty;
                        }

                        CoreExtensions.RunWithRetries(3, () =>
                        {
                            try
                            {
                                // TODO: Performance wise the HttpClient should be re-used for all requests, create own STS client that wraps the HttpClient
                                // and re-uses the instance: https://blogs.msdn.microsoft.com/shacorn/2016/10/21/best-practices-for-using-httpclient-on-services/

                                // don't follow redirects, HttpClient disposes this handler
                                HttpClientHandler clientHandler = new HttpClientHandler();
                                clientHandler.AllowAutoRedirect = false;

                                using (var client = new HttpClient(clientHandler, true))
                                {
                                    // just add the user-agent so it is easier for us to see from IIS logs these entries
                                    client.DefaultRequestHeaders.Add("User-Agent", "Tieto UserIdentification Client 1.0");
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

                                    // have a short time out
                                    client.Timeout = TimeSpan.FromSeconds(15);

                                    // needs to be done like this so that we can check the return HTTP status
                                    // currently if the bearer token is invalid STS propertly responses with Unauthorized 401
                                    // as specified in http://openid.net/specs/openid-connect-core-1_0.html#rfc.section.5.3.3 BUT
                                    // STS app intercepts and does a redirect to the STS login page and in those cases we would get HTML for login page
                                    // so don't follow redirects and check the response

                                    // if this throws - then use the retry logic (mainly timeout)
                                    var responseMessage = client.GetAsync(appConfiguration.StsAddress + "/connect/userinfo").Result;

                                    if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                                    {
                                        // status OK (200) if token could be validated and JSON data is returned
                                        // http://openid.net/specs/openid-connect-core-1_0.html#rfc.section.5.3.2

                                        try
                                        {
                                            // read JSON data
                                            string jsonData = responseMessage.Content.ReadAsStringAsync().Result;

                                            var userIdentification = JsonConvert.DeserializeObject<UserIdentityResponse>(jsonData);

                                            userEmail = userIdentification.Email ?? userIdentification.Sub ?? string.Empty;
                                        }
                                        catch (Exception ex)
                                        {
                                            // so if we fail to read the data, then the retry is most likely going to end-up repeating the same error so don't retry
                                            logger.LogError(UserIdentification.UserinfoEvent, ex, "Error reading or deserializing STS Userinfo response.");
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        // invalid token or some other error, we don't know because STS app catches the unauthorized and responses with redirect to login page (which returns HTML)
                                        // so this part currently doesn't work as specified in : http://openid.net/specs/openid-connect-core-1_0.html#rfc.section.5.3.3
                                        logger.LogError(UserIdentification.UserinfoEvent, $"Unable to get Userinfo with bearer token. STS userinfo response was not OK, response HTTP status: {responseMessage.StatusCode}.");

                                        // don't retry
                                        return true;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                logger.LogError(UserIdentification.UserinfoEvent, "UserIdentification.UserName - Get users userinfo from STS caused an exception: " + CoreExtensions.ExtractAllInnerExceptions(e));
                                return false;
                            }
                            return true;
                        });

                        userName = userEmail;
                    }
                    else
                    {
                        userName = uniqueClaim?.Value ?? string.Empty;
                    }

                }

                return userName;
            }
            private set
            {
                if(string.IsNullOrEmpty(value))
                {
                    // assume the value is reset and should be fetched in get so set the state to not fetched
                    userinfoFetched = false;
                }
                else
                {
                    // set to fetched as the value is explicitly set
                    userinfoFetched = true;
                }

                userName = value;
            }
        }

        public UserIdentification(IHttpContextAccessor contextAccessor, ApplicationConfiguration appConfiguration, ILogger<UserIdentification> logger)
        {
            this.contextAccessor = contextAccessor;
            this.appConfiguration = appConfiguration;
            this.logger = logger;
        }

        void IThreadUserInterface.SetUserName(string user)
        {
            UserName = user;
        }
    }

    public class UserIdentityResponse
    {
        public string Email { get; set; }
        public string Sub { get; set; }
    }
}
