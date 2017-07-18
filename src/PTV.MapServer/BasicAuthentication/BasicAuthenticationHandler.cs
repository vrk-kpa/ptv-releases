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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;


namespace PTV.MapServer.BasicAuthentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private const char CredentialDelimiter = ':';

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return Task.FromResult(AuthenticateResult.Fail("No authorization header."));
            }

            if (!authorizationHeader.StartsWith(Options.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.Fail("Unsupported authentication"));
            }

            var credentials = authorizationHeader.Substring(Options.AuthenticationScheme.Length).Trim();
            if (string.IsNullOrEmpty(credentials))
            {
                return Task.FromResult(AuthenticateResult.Fail("No credetials"));
            }

            try
            {
                credentials = DecodeCredentials(credentials);
                var delimiterIndex = credentials.IndexOf(CredentialDelimiter);
                if (delimiterIndex < 0)
                {
                    return Task.FromResult(AuthenticateResult.Fail("Invalid credentials, missing delimiter."));
                }

                var username = credentials.Substring(0, delimiterIndex);
                var password = credentials.Substring(delimiterIndex + 1);

                if (username != Options.WfsUserName && password != Options.WfsPassword)
                {
                    Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(
                        new ClaimsPrincipal(),
                        new AuthenticationProperties(),
                        Options.AuthenticationScheme)));
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim(ClaimTypes.Name, username)
                };

                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(
                        new ClaimsPrincipal(new ClaimsIdentity(claims, Options.AuthenticationScheme)),
                        new AuthenticationProperties(),
                        Options.AuthenticationScheme)));
            }
            catch (Exception ex)
            {
                return Task.FromResult(AuthenticateResult.Fail($"Authentication failed: {ex.Message}"));
            }

        }

        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            Response.StatusCode = 401;

            var headerValue = Options.AuthenticationScheme + $" realm=\"{Options.Realm}\"";
            Response.Headers.Append(HeaderNames.WWWAuthenticate, headerValue);

            return Task.FromResult(true);
        }

        protected override Task<bool> HandleForbiddenAsync(ChallengeContext context)
        {
            Response.StatusCode = 403;
            return Task.FromResult(true);
        }

        private static string DecodeCredentials(string credentials)
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(credentials));
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to decode credentials : {credentials}", ex);
            }
        }
    }
}
