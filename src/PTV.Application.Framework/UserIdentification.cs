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

        public string UserName
        {
            get
            {
                if (string.IsNullOrEmpty(userName))
                {
                    var uniqueClaim = contextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(i => i.Type == UniqueClaimIdentifier);
                    if ((uniqueClaim == null) && (!string.IsNullOrEmpty(appConfiguration.StsAddress)))
                    {
                        var bearer =
                            string.Join("",
                                contextAccessor?.HttpContext?.Request?.Headers["Authorization"] ??
                                new Microsoft.Extensions.Primitives.StringValues())
                                .Split(' ')
                                .LastOrDefault() ?? string.Empty;
                        if (string.IsNullOrEmpty(bearer))
                        {
                            logger.LogWarning("No token found for requested user name!");
                            return string.Empty;
                        }
                        string userEmail = string.Empty;
                        CoreExtensions.RunWithRetries(3, () =>
                        {
                            try
                            {
                                using (var client = new HttpClient())
                                {
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                                        "Bearer", bearer);
                                    var response =
                                        client.GetStringAsync(appConfiguration.StsAddress + "/connect/userinfo").Result;
                                    var userIdentification =
                                        JsonConvert.DeserializeObject<UserIdentityResponse>(response);
                                    userEmail = userIdentification.Email ?? userIdentification.Sub ?? string.Empty;
                                }
                            }
                            catch (Exception e)
                            {
                                logger.LogError("UserIdentification - GetUserName\n" +
                                                CoreExtensions.ExtractAllInnerExceptions(e));
                                throw;
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
            private set { userName = value; }
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
