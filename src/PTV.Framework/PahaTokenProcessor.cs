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
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Routing.Tree;
using Newtonsoft.Json;

namespace PTV.Framework
{
    public class PahaTokenIntrospect : PahaToken
    {
        public PahaTokenIntrospect(PahaToken pahaToken)
        {
            Active = pahaToken.Active;
            FirstName = pahaToken.FirstName;
            LastName = pahaToken.LastName;
            Hetu = pahaToken.Hetu;
            Client_id = pahaToken.Client_id;
            Exp = pahaToken.Exp;
            Jti = pahaToken.Jti;
            Email = pahaToken.Email;
            UserName = pahaToken.UserName;
            Id = pahaToken.Id;
            ApiUserOrganization = pahaToken.ApiUserOrganization;
            ActiveOrganizationId = pahaToken.ActiveOrganizationId;
            AllOrganizations = pahaToken.AllOrganizations;
            GlobalPtvRole = pahaToken.GlobalPtvRole;
        }

        // Needed for token introspection
        public string Organizations => JsonConvert.SerializeObject(AllOrganizations.Values.Select(i => new PahaOrganizationInternalDto(i)).ToList());
        public string Roles => JsonConvert.SerializeObject(new PahaGlobalRolesDto() {Vrk_Ptv = new List<string>() {GlobalPtvRole}});
    }


    public class PahaToken
    {
        private string userName;

        public bool Active { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Hetu { get; set; }
        // Name comes from Paha
        // ReSharper disable once InconsistentNaming
        public string Client_id { get; set; }
        public decimal Exp { get; set; }
        public string Jti { get; set; }
        public string Email { get; set; }

        public string UserName
        {
            get => userName.IsNullOrWhitespace() ? Email : userName;
            set => userName = value;
        }

        public string Id { get; set; }
        public Guid ActiveOrganizationId { get; set; }
        
        // PAHA workaround solution for active organization because of their missing feature
        public Guid ApiUserOrganization { get; set; }

        [JsonIgnore]
        public Dictionary<Guid, PahaOrganizationDto> AllOrganizations { get; set; } = new Dictionary<Guid, PahaOrganizationDto>();

        [JsonIgnore]
        public List<string> InternalErrorMessages { get; set; } = new List<string>();

        [JsonIgnore]
        public List<string> InternalWarningMessages { get; set; } = new List<string>();
        
        [JsonIgnore]
        public string GlobalPtvRole { get; set; }

        private static string GetClaimValue(IEnumerable<Claim> claims, string claimType, string defaultValue)
        {
            return claims.FirstOrDefault(i => i.Type.ToLowerInvariant() == claimType.ToLowerInvariant())?.Value ?? defaultValue;
        }

        private static List<string> GetClaimValueList(IEnumerable<Claim> claims, string claimType, List<string> defaultValue = null)
        {
            var result = claims.Where(i => i.Type.ToLowerInvariant() == claimType.ToLowerInvariant()).Select(i => i.Value).ToList();
            if (!result.Any() && defaultValue != null) return defaultValue;
            return result;
        }

        private static T DeserializeJsonSafe<T>(string json)
        {
            try
            {
                return string.IsNullOrEmpty(json) ? default(T) : JsonConvert.DeserializeObject<T>(json);
            }
            catch(Exception){}

            return default(T);
        }


        public static PahaToken ExtractPahaToken(IEnumerable<Claim> claims, Dictionary<Guid,Guid> guidMappings = null)
        {
            if (guidMappings == null) guidMappings = new Dictionary<Guid, Guid>();
            var result = new PahaToken();
            try
            {
                var pahaTokenType = result.GetType();
                var tokenProperties = pahaTokenType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(i => i.PropertyType == typeof(string))
                    .Select(i => new {Name = i.Name.ToLowerInvariant(), Info = i}).ToDictionary(i => i.Name, i => i.Info);
                var inputClaims = claims.Select(i => new {Type = i.Type.ToLowerInvariant(), Value = i.Value}).ToList();
                foreach (var claim in inputClaims)
                {
                    var property = tokenProperties.TryGetOrDefault(claim.Type, null);
                    if (property != null)
                    {
                        property.SetValue(result, claim.Value);
                    }
                }

                result.GlobalPtvRole = DeserializeJsonSafe<PahaGlobalRolesDto>(GetClaimValue(claims, PahaClaims.PahaRoles, null))?.Vrk_Ptv?.FirstOrDefault();
                result.FirstName = GetClaimValue(claims, PahaClaims.FirstName, string.Empty);
                result.LastName = GetClaimValue(claims, PahaClaims.LastName, string.Empty);
                if (result.FirstName.IsNullOrWhitespace())
                {
                    result.InternalWarningMessages.Add("PahaToken: User's FirstName is not set!");
                }
                if (result.LastName.IsNullOrWhitespace())
                {
                    result.InternalWarningMessages.Add("PahaToken: User's LastName is not set!");
                }
                decimal.TryParse(GetClaimValue(claims, PahaClaims.Expiration, null) ?? "0", out decimal expiration);
                if (expiration <= 0)
                {
                    result.InternalErrorMessages.Add("PahaToken: Expiration of token is invalid!");
                }
                result.Exp = expiration;
                result.ActiveOrganizationId = GetClaimValue(claims, PahaClaims.UserOrganization, Guid.Empty.ToString()).ParseToGuid() ?? Guid.Empty;
                result.ApiUserOrganization = GetClaimValue(claims, PahaClaims.PahaSemiUserOrganization, Guid.Empty.ToString()).ParseToGuid() ?? Guid.Empty;
                if (!result.ActiveOrganizationId.IsAssigned() && result.ApiUserOrganization.IsAssigned())
                {
                    result.InternalErrorMessages.Add("PahaToken: User's active organization is not set!");
                }

                List<string> orgClaim = GetClaimValueList(claims, PahaClaims.Organizations);
                List <PahaOrganizationInternalDto > userOrgs = new List<PahaOrganizationInternalDto>();

                foreach (var orgDefItem in orgClaim)
                {
                    if (orgDefItem.StartsWith("["))
                    {
                        userOrgs.AddRange(JsonConvert.DeserializeObject<List<PahaOrganizationInternalDto>>(orgDefItem));
                    }
                    else if (orgDefItem.StartsWith("{"))
                    {
                        userOrgs.Add(JsonConvert.DeserializeObject<PahaOrganizationInternalDto>(orgDefItem));
                    }
                }

                userOrgs.ForEach(o =>
                {
                    if (!o.Id.IsAssigned())
                    {
                        result.InternalWarningMessages.Add($"PahaToken: One of user organization has invalid or missing ID! {o.Id}:{o.Name ?? string.Empty}");
                    }
                    if (string.IsNullOrEmpty(o.Name))
                    {
                        result.InternalWarningMessages.Add($"PahaToken: One of user organization has invalid or missing Name! {o.Id}");
                    }
                    if (o.Roles != null && !o.Roles.Any())
                    {
                        result.InternalWarningMessages.Add($"PahaToken: One of user organization has invalid or missing role! {o.Id}:{o.Name ?? string.Empty}");
                    }
                });

                
                List<Guid> possibleActiveOrganization = new List<Guid>();
                if (result.ApiUserOrganization.IsAssigned())
                {
                    possibleActiveOrganization.Add(result.ApiUserOrganization);
                    if (guidMappings.TryGetValue(result.ApiUserOrganization, out Guid altGuid))
                    {
                        possibleActiveOrganization.Add(altGuid);
                    }
                }
                else if (result.ActiveOrganizationId.IsAssigned())
                {
                    possibleActiveOrganization.Add(result.ActiveOrganizationId);
                    if (guidMappings.TryGetValue(result.ActiveOrganizationId, out Guid altGuid))
                    {
                        possibleActiveOrganization.Add(altGuid);
                    }
                }
                
                result.AllOrganizations = userOrgs.ToDictionary(i => i.Id, i => new PahaOrganizationDto(i));
                var activeOrganization = result.AllOrganizations.Select(i => i.Key).Intersect(possibleActiveOrganization).FirstOrDefault();
                
                if (!activeOrganization.IsAssigned())
                {
                    result.InternalErrorMessages.Add("PahaToken: User's active organization is wrong, not available in assigned organizations!");
                }
                else
                {
                    result.ActiveOrganizationId = activeOrganization;
                    var selectedOrg = result.AllOrganizations[result.ActiveOrganizationId];
                    if (!selectedOrg.Id.IsAssigned())
                    {
                        result.InternalErrorMessages.Add($"PahaToken: Active user's organization has invalid or missing ID! {selectedOrg.Id}:{selectedOrg.Name ?? string.Empty}");
                    }
                    if (string.IsNullOrEmpty(selectedOrg.Name))
                    {
                        result.InternalErrorMessages.Add($"PahaToken: Active user's organization has invalid or missing Name! {selectedOrg.Id}");
                    }
                    if (string.IsNullOrEmpty(selectedOrg.Role))
                    {
                        result.InternalErrorMessages.Add($"PahaToken: Active user's organization has invalid or missing role! {selectedOrg.Id}:{selectedOrg.Name ?? string.Empty}");
                    }
                }

                if (string.IsNullOrEmpty(result.UserName))
                {
                    result.InternalErrorMessages.Add("PahaToken: User has not username!");
                }

            }
            catch (Exception e)
            {
                result.InternalErrorMessages.Add($"PahaToken: Exception occured during processing token! Exception {e.Message}");
            }

            return result;
        }

        public static PahaToken ExtractPahaToken(string bearer)
        {
            var encodedToken = new JwtSecurityToken(bearer);
            return ExtractPahaToken(encodedToken.Claims);
        }
    }
}
