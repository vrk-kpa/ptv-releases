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
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PTV.Framework;

namespace PTV.LocalAuthentication
{
    public interface ITokenService
    {
        TokenOutputData CreateToken(TokenInputParams inputParams);
    }

    [RegisterService(typeof(ITokenService), RegisterType.Scope)]
    internal class TokenService : ITokenService
    {
        public TokenOutputData CreateToken(TokenInputParams inputParams)
        {
            var userClaims = new List<Claim>()
            {
                new Claim(PahaClaims.UserName, inputParams.UserName),
                new Claim(PahaClaims.UserOrganization, inputParams.OrganizationId.ToString()),
                new Claim(PahaClaims.FirstName, inputParams.UserName.Split('@').FirstOrDefault() ?? string.Empty),
                new Claim(PahaClaims.LastName, $"{inputParams.UserAccessRightsGroup}-{inputParams.UserName.Split('@').LastOrDefault() ?? string.Empty}")
            };

            var tokenJwt = new JwtSecurityToken(
                issuer: inputParams.TokenServiceUrl,
                audience: inputParams.TokenServiceUrl,
                claims: userClaims,

                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Guid.NewGuid().ToByteArray()), SecurityAlgorithms.HmacSha256)
            );
            var orgs = JsonConvert.SerializeObject(new List<PahaOrganizationInternalDto>()
            {
                new PahaOrganizationInternalDto() { Id = inputParams.OrganizationId, Name  = inputParams.OrganizationId.ToString(), Roles = new List<string>() { inputParams.UserAccessRightsGroup } }
            });
            
            tokenJwt.Payload.Add(PahaClaims.Organizations, orgs);
            //var pahaGlobalRole = new PahaGlobalRolesDto(){ Vrk_Ptv = new List<string>() { "PTV_ADMINISTRATOR" }};
            //tokenJwt.Payload.Add(PahaClaims.PahaRoles,  JsonConvert.SerializeObject(pahaGlobalRole));
            return new TokenOutputData(new JwtSecurityTokenHandler().WriteToken(tokenJwt), tokenJwt.ValidTo);
        }
    }

    public class TokenOutputData
    {
        public TokenOutputData(string token, DateTime validTo)
        {
            this.AccessToken = token;
            this.ValidTo = validTo;
        }
        public string AccessToken { get; }
        public DateTime ValidTo { get; }
    }


    public class TokenInputParams
    {
        public TokenInputParams(string userName, string userAccessRightsGroup, Guid organizationId, string tokenServiceUrl)
        {
            this.UserName = userName;
            this.UserAccessRightsGroup = userAccessRightsGroup;
            this.OrganizationId = organizationId;
            this.TokenServiceUrl = tokenServiceUrl;
        }


        public string UserName { get; }
        public string UserAccessRightsGroup { get; }
        public Guid OrganizationId { get; }
        public string TokenServiceUrl { get; }

    }
}
