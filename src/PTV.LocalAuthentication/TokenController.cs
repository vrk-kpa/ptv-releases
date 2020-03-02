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
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Extensions;
using IAuthorizationService = PTV.Database.DataAccess.Interfaces.Services.IAuthorizationService;

namespace PTV.LocalAuthentication
{
    [Controller]
    public class TokenController : Controller
    {
        public class AtsTokenRequest
        {
            public string UserName { get; set; }
            public Guid OrganizationId { get; set; }
            public Guid UserAccessRightsGroupId { get; set; }
            public string UserAccessRightsGroup { get; set; }
            public string AccessToken { get; set; }
            public string AcrValues { get; set; }
            public string Assertion { get; set; }
            public string ClaimsLocales { get; set; }
            public string ClientAssertion { get; set; }
            public string ClientAssertionType { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string Code { get; set; }
            public string CodeChallenge { get; set; }
            public string CodeChallengeMethod { get; set; }
            public string CodeVerifier { get; set; }
            public string Display { get; set; }
            public string GrantType { get; set; }
            public string IdentityProvider { get; set; }
            public string IdTokenHint { get; set; }
            public string LoginHint { get; set; }
            public long? MaxAge { get; set; }
            public string Nonce { get; set; }
            public string Password { get; set; }
            public string PostLogoutRedirectUri { get; set; }
            public string Prompt { get; set; }
            public string RedirectUri { get; set; }
            public string RefreshToken { get; set; }
            public string Request { get; set; }
            public string RequestId { get; set; }
            public string RequestUri { get; set; }
            public string Resource { get; set; }
            public string ResponseMode { get; set; }
            public string ResponseType { get; set; }
            public string Scope { get; set; }
            public string State { get; set; }
            public string Token { get; set; }
            public string TokenTypeHint { get; set; }
            public string UiLocales { get; set; }
        }

        private readonly ApplicationConfiguration configuration;
        private IHttpContextAccessor contextAccessor;
        private string TokenServiceUrl = "http://localhost:42621";
        private UserManager<StsUser> userManager;
        private IUserOrganizationService userOrganizationService;
        private RoleManager<StsRole> roleManager;
        private IAuthorizationService authorizationService;
        private ILogger<TokenController> logger;
        private ITokenService tokenService;
        private ITokenStore tokenStore;


        public TokenController(ITokenStore tokenStore, ITokenService tokenService, IHttpContextAccessor contextAccessor, ApplicationConfiguration configuration, UserManager<StsUser> userManager,
            RoleManager<StsRole> roleManager, IUserOrganizationService userOrganizationService, IAuthorizationService authorizationService, ILogger<TokenController> logger)
        {
            this.tokenStore = tokenStore;
            this.tokenService = tokenService;
            this.contextAccessor = contextAccessor;
            this.configuration = configuration;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.userOrganizationService = userOrganizationService;
            this.authorizationService = authorizationService;
            this.logger = logger;
            TokenServiceUrl = configuration.GetTokenServiceUrl();
        }


        [HttpPost("~/.well-known/openid-configuration"),
         HttpGet("~/.well-known/openid-configuration"),
         Produces("application/json")]
        public IActionResult OpenIdConfiguration()
        {
            return Ok(new
            {
                issuer = TokenServiceUrl,
                jwks_uri = $"{TokenServiceUrl}/.well-known/openid-configuration/jwks",
                token_endpoint = $"{TokenServiceUrl}/connect/token",
                userinfo_endpoint = $"{TokenServiceUrl}/connect/userinfo",
                introspection_endpoint = $"{TokenServiceUrl}/connect/introspect"
            });
        }


        [HttpPost("~/connect/userinfo"), HttpGet("~/connect/userinfo"), Produces("application/json")]
        public IActionResult UserInfo()
        {
            var bearer = contextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("bearer", string.Empty, true, CultureInfo.InvariantCulture).Replace(" ", string.Empty); // skip Bearer header
            try
            {
                var encodedToken = new JwtSecurityToken(bearer);
                var pahaToken = PahaTokenAccessor.ExtractPahaToken(encodedToken.Claims);
                return Ok(pahaToken);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost("~/connect/introspect"), HttpGet("~/connect/introspect"), Produces("application/json")]
        public IActionResult Introspection(string token)
        {
            try
            {
                var hash = token.GetSha256Hash();
                var knownToken = tokenStore.Get(hash)?.AccessToken;
                bool active = (token == knownToken) && (knownToken != null);
                var encodedToken = new JwtSecurityToken(token);
                var pahaToken = PahaTokenAccessor.ExtractPahaToken(encodedToken.Claims);
                var dateTime = DateTime.UtcNow;
                active &= encodedToken.ValidFrom <= dateTime && encodedToken.ValidTo > dateTime;
                pahaToken.Active = active;
                return Ok(new PahaTokenIntrospect(pahaToken));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private int FlagMatches(AccessRightEnum enumA, AccessRightEnum enumB)
        {
            int matches = 0;
            var allValues = Enum.GetValues(typeof(AccessRightEnum)).Cast<AccessRightEnum>();
            foreach (var enumValue in allValues)
            {
                if (enumA.HasFlag(enumValue) && enumB.HasFlag(enumValue))
                {
                    matches++;
                }
            }

            return matches;
        }


        private bool MapOldIdentityClaimsToUserGroup(AtsTokenRequest request, string stsRole, string accessRightClaim)
        {
            var accessRightsSet = accessRightClaim.Split(',').Select(i => i.Trim().ConvertToEnum<AccessRightEnum>()).WhereNotNull().Cast<AccessRightEnum>().ToList();
            if (!accessRightsSet.Any())
            {
                accessRightsSet = Enum.GetValues(typeof(AccessRightEnum)).Cast<AccessRightEnum>().ToList();
            }

            AccessRightEnum accessRights = accessRightsSet.Aggregate((i, j) => i | j);
            var userGroups = authorizationService.GetUserAccessGroupsFull();
            stsRole = stsRole.ToLowerInvariant();
            var perRoleGroups = userGroups.Where(i => i.UserRole.ToString().ToLowerInvariant() == stsRole).ToList();
            var bestGroup = perRoleGroups.OrderByDescending(i => FlagMatches(i.AccessRightFlag, accessRights)).FirstOrDefault();
            if (bestGroup == null)
            {
                logger.LogWarning($"TokenService> User '{request.UserName}' has no matching user group. Assigned role: {stsRole}, access rights: {accessRightClaim}");
                return false;
            }

            request.UserAccessRightsGroup = bestGroup.Code;
            return true;
        }


        [HttpPost("~/connect/token"), Produces("application/json")]
        public IActionResult Exchange(AtsTokenRequest data)
        {
            var response = IssueToken(data);
            if (response.StatusCode == 0)
            {
                return Ok(new
                {
                    access_token = response.AccessToken,
                    expiration = response.Expiration
                });
            }

            return CoreExtensions.ReturnStatusCode(response.StatusCode, response.TokenError);
        }


        private AuthenticationResponse IssueToken(AtsTokenRequest request)
        {
            if (!configuration.UsePAHAAuthentication && !configuration.FakeAuthorization)
            {
                StsUser dbUser = userManager.FindByNameAsync(request.UserName).Result;
                if (dbUser == null || !userManager.CheckPasswordAsync(dbUser, request.Password).Result)
                {
                    logger.LogWarning($"TokenService> User '{request.UserName}', credentails are wrong.");
                    return new AuthenticationResponse {StatusCode = 403, TokenError = new VmTokenError {Error = TokenErrorEnum.WrongCredentials, Message = $"TokenService> User '{request.UserName}', credentails are wrong."}};
                }

                var allUserOrgsRoles = userOrganizationService.GetOrganizationRolesForUser(dbUser.Id);
                var allRoles = roleManager.Roles.ToList();
                var eevaRole = allRoles.First(i => i.Name.ToLowerInvariant() == UserRoleEnum.Eeva.ToString().ToLowerInvariant());
                var peteRole = allRoles.First(i => i.Name.ToLowerInvariant() == UserRoleEnum.Pete.ToString().ToLowerInvariant());
                var shirleyRole = allRoles.First(i => i.Name.ToLowerInvariant() == UserRoleEnum.Shirley.ToString().ToLowerInvariant());
                if (allUserOrgsRoles.All(i => !i.IsMain))
                {
                    var toBeMain = (allUserOrgsRoles.FirstOrDefault(i => i.RoleId == eevaRole.Id) ?? allUserOrgsRoles.FirstOrDefault(i => i.RoleId == peteRole.Id)) ?? allUserOrgsRoles.FirstOrDefault();
                    toBeMain.SafeCall(i => i.IsMain = true);
                }

                if (!allUserOrgsRoles.Any())
                {
                    logger.LogWarning($"TokenService> User '{request.UserName}' has no organization assigned.");
                    return new AuthenticationResponse {StatusCode = 403, TokenError = new VmTokenError {Error = TokenErrorEnum.MissingOrganization, Message = $"TokenService> User '{request.UserName}' has no organization assigned."}};
                }

                var mainOrg = allUserOrgsRoles.First(i => i.IsMain);
                var stsRole = allRoles.First(i => i.Id == mainOrg.RoleId).Name;
                request.OrganizationId = mainOrg.OrganizationId;
                var accessRightClaim = userManager.GetClaimsAsync(dbUser).Result.FirstOrDefault(i => i.Type == "user_access_rights")?.Value ?? string.Empty;

                if (!MapOldIdentityClaimsToUserGroup(request, stsRole, accessRightClaim))
                {
                    logger.LogWarning($"TokenService> User '{request.UserName}' mapping to user group has failed.");
                    return new AuthenticationResponse {StatusCode = 403, TokenError = new VmTokenError {Error = TokenErrorEnum.UserNotMapped, Message = $"TokenService> User '{request.UserName}' mapping to user group has failed."}};
                }
            }

            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.UserAccessRightsGroup) || !request.OrganizationId.IsAssigned())
            {
                logger.LogWarning($"TokenService> User '{request.UserName}' has no organization or user group assigned");
                return new AuthenticationResponse {StatusCode = 403, TokenError = new VmTokenError {Error = TokenErrorEnum.NoOrgOrGroup, Message = $"TokenService> User '{request.UserName}' has no organization or user group assigned"}};
            }

            //Fake token from Production
            //var accessToken = new TokenOutputData("eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE1NzE3Njk2MTEsInNjb3BlIjoibG9naW4iLCJjbGllbnRfaWQiOiI1NzVmYjhlMy1mOGEwLTQzZmUtYmE3Yy01OTRmMGYzMmNiOTkiLCJ1c2VybmFtZSI6InRlcmhpLnR1b2trb2xhQHZyay5maSIsImZpcnN0TmFtZSI6IlRlcmhpIiwibGFzdE5hbWUiOiJUdW9ra29sYSIsImVtYWlsIjoidGVyaGkudHVva2tvbGFAdnJrLmZpIiwib3JnYW5pemF0aW9ucyI6W3siaWQiOiJkZjJmN2NlNy1lNzdhLTQ3ZTctYTkyYy0wMjBmNjZlYjRiY2YiLCJuYW1lIjoiU3VvbWkuZmlfdmVya2tvdG9pbWl0dXMgIiwic3ViT3JnIjpmYWxzZSwicm9sZXMiOlsiUFRWX0FETUlOSVNUUkFUT1IiLCJQVFZfTUFJTl9VU0VSIl19LHsiaWQiOiJjMjBmZDdkMC1hOTRiLTRjMmYtYTZhYS0wOTNhZTRlOGU5YjkiLCJuYW1lIjoiVsOkZXN0w7ZyZWtpc3RlcmlrZXNrdXMgSUNUIiwic3ViT3JnIjpmYWxzZSwicm9sZXMiOlsiUFRWX0FETUlOSVNUUkFUT1IiLCJQVFZfTUFJTl9VU0VSIl19LHsiaWQiOiI3ZThkMTdhMi1jNGI5LTQ0MzYtODAyMC0yZmQ2ZjljNmQ5YjciLCJuYW1lIjoiRWxpbmtlaW5vLSwgbGlpa2VubmUtIGphIHltcMOkcmlzdMO2a2Vza3VzdGVuIHNla8OkIHR5w7YtIGphIGVsaW5rZWlub3RvaW1pc3RvamVuIGtlaGl0dMOkbWlzLSBqYSBoYWxsaW50b2tlc2t1cyBFTFkta2Vza3VzIiwic3ViT3JnIjpmYWxzZSwicm9sZXMiOlsiUFRWX0FETUlOSVNUUkFUT1IiLCJQVFZfTUFJTl9VU0VSIl19XSwicm9sZXMiOnsicHR2IjpbImFkbWluIl0sInNlbWEiOlsiYWRtaW4iXSwic2loYSI6WyJ3ZWItZWRpdG9yIl0sInZya19wdHYiOlsiUFRWX0FETUlOSVNUUkFUT1IiXSwidnJrX3NlbWEiOlsidnJrYWRtaW4iXX0sImlkIjoiNTc1ZmI4ZTMtZjhhMC00M2ZlLWJhN2MtNTk0ZjBmMzJjYjk5IiwiYWN0aXZlT3JnYW5pemF0aW9uSWQiOiJjMjBmZDdkMC1hOTRiLTRjMmYtYTZhYS0wOTNhZTRlOGU5YjkiLCJhcGlVc2VyT3JnYW5pemF0aW9uIjpmYWxzZSwiaWF0IjoxNTcxNzI2NDEyfQ.iLig5XsNGolt7jOF2ijPuHjpCKtE3JsPCNfc3CIKHQxymfw0a0HpVNzdnFb2LiXWs3tHx1XT1ivUtuRPUqm4a-Wzk4n9gC8KSMuDFttxzsHza7GVa8VP8cZK4CVvkXnb005L8XAy8Cw1R-coPzGBT_WSePYbQBoqaoG-CmEHgsoKBZcuBVXwlXtDOXMa8Ihy2yi8Ve6VXXCsivganVp8UY6SNDpnU1NsO4se_ff6tCSRfe8wAb-if0djNHVjBT64tfUwzsYknfYcQposE3KR7oI3S5ahiiZntS1tdoiizt4VShycVpOrxdDfWIQJJURPkhn24BZj6_dOAguC9VAvQw", DateTime.Now.AddDays(2));

            var accessToken = tokenService.CreateToken(new TokenInputParams(request.UserName, request.UserAccessRightsGroup, request.OrganizationId, TokenServiceUrl));
            tokenStore.Add(accessToken);
            return new AuthenticationResponse
            {
                AccessToken = accessToken.AccessToken,
                Expiration = accessToken.ValidTo
            };
        }


        public class PahaStyleRequest
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        public class AuthenticationResponse
        {
            public string AccessToken { get; set; }
            public DateTime Expiration { get; set; }
            public int StatusCode { get; set; } = 0;
            public VmTokenError TokenError { get; set; }
        }

        [HttpPost("~/api/auth/api-login"), Produces("application/json")]
        public IActionResult PahaLikeTokenRequest([FromBody] PahaStyleRequest request)
        {
            if (configuration.UsePAHAAuthentication || configuration.FakeAuthorization)
            {
                return StatusCode(400, "Not supported, not configured for this type of request.");
            }

            var response = IssueToken(new AtsTokenRequest {UserName = request.UserName, Password = request.Password, GrantType = "password"});
            if (response.StatusCode == 0)
            {
                return Ok(new
                {
                    ptvToken = response.AccessToken
                });
            }
            return CoreExtensions.ReturnStatusCode(response.StatusCode, response.TokenError);
        }
    }
}
