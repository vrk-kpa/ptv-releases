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
using System.Linq;
using System.Security.Claims;
using Newtonsoft.Json;
using PTV.Framework.Enums;

namespace PTV.Framework.Paha
{
    public abstract class PahaTokenBase : IPahaToken
    {
        protected readonly string[] PtvRoleKeys = {"ptv", "servicecatalogue"};
        protected readonly string[] VrkRoleKeys = { "vrkptv", "vrk_ptv", "vrkservicecatalogue" };

        protected readonly Dictionary<PahaUserRole, UserAccessRightsGroupEnum> UserRoleMap = new Dictionary<PahaUserRole, UserAccessRightsGroupEnum>
        {
            { PahaUserRole.Unassigned, UserAccessRightsGroupEnum.DENY },
            { PahaUserRole.Viewer, UserAccessRightsGroupEnum.PTV_VIEWER },
            { PahaUserRole.User, UserAccessRightsGroupEnum.PTV_USER },
            { PahaUserRole.Admin, UserAccessRightsGroupEnum.PTV_MAIN_USER },
            { PahaUserRole.Vrkadmin, UserAccessRightsGroupEnum.PTV_ADMINISTRATOR },
            { PahaUserRole.Api, UserAccessRightsGroupEnum.API_USER },
            { PahaUserRole.Asti, UserAccessRightsGroupEnum.API_ASTI_USER }
        };
        
        public PahaTokenBase()
        {
            InternalWarningMessages = new List<string>();
            InternalErrorMessages = new List<string>();
        }
        
        public virtual Guid ActiveOrganizationId { get; set; }
        public virtual string ActiveOrganizationName { get; set; }
        public virtual string Email { get; set; }
        public virtual long Exp { get; set; }
        public virtual string FirstName { get; set; }
        public virtual long Iat { get; set; }
        public virtual Guid Id { get; set; }
        public virtual Guid? Jti { get; set; }
        public virtual string LastName { get; set; }
        public virtual UserAccessRightsGroupEnum PtvRole { get; set; }
        public virtual bool Active { get; set; }
        public Dictionary<string, List<string>> Roles { get; set; }
        public virtual List<string> InternalWarningMessages { get; set; }
        public virtual List<string> InternalErrorMessages { get; set; }
        public virtual void Validate()
        {
            if (ActiveOrganizationName.IsNullOrWhitespace())
            {
                InternalWarningMessages.Add($"PahaToken: Active user organization has invalid or missing Name! {ActiveOrganizationId}");
            }
            if (Email.IsNullOrWhitespace())
            {
                InternalErrorMessages.Add("PahaToken: User has no email!");
            }
            if (Exp <= 0)
            {
                InternalErrorMessages.Add("PahaToken: Expiration of token is invalid!");
            }
            if (FirstName.IsNullOrWhitespace())
            {
                InternalWarningMessages.Add("PahaToken: User's FirstName is not set!");
            }
            if (LastName.IsNullOrWhitespace())
            {
                InternalWarningMessages.Add("PahaToken: User's LastName is not set!");
            }
            if (PtvRole == UserAccessRightsGroupEnum.DENY || PtvRole == UserAccessRightsGroupEnum.PTV_VIEWER)
            {
                InternalErrorMessages.Add($"PahaToken: Active user's organization has invalid or missing role! {ActiveOrganizationId}:{ActiveOrganizationName ?? string.Empty}");
            }
        }

        public abstract void FillInClaims(List<Claim> claims, Dictionary<Guid, Guid> ptvSahaMapping);
        
        protected string GetClaimValue(IEnumerable<Claim> claims, string claimType, string defaultValue = null)
        {
            return claims.FirstOrDefault(i => i.Type.ToLowerInvariant() == claimType.ToLowerInvariant())?.Value ??
                   defaultValue;
        }

        protected List<string> GetClaimValueList(IEnumerable<Claim> claims, string claimType,
            List<string> defaultValue = null)
        {
            var result = claims.Where(i => i.Type.ToLowerInvariant() == claimType.ToLowerInvariant())
                .Select(i => i.Value).ToList();
            if (!result.Any() && defaultValue != null) return defaultValue;
            return result;
        }

        protected T DeserializeJsonSafe<T>(string json, T defaultValue = default)
        {
            try
            {
                return string.IsNullOrEmpty(json) ? defaultValue : JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
            }

            return defaultValue;
        }

        protected IEnumerable<T> DeserializeJsonListSafe<T>(List<string> json)
        {
            foreach (var item in json)
            {
                T converted;
                try
                {
                    converted = JsonConvert.DeserializeObject<T>(item);
                }
                catch (Exception e)
                {
                    yield break;
                }
                yield return converted;
            }
        }
        
        public static IPahaToken ExtractPahaToken(List<Claim> claims, Dictionary<Guid, Guid> guidMappings = null)
        {
            if (guidMappings == null) guidMappings = new Dictionary<Guid, Guid>();
            var result = ResolvePahaTokenVersion(claims);
            try
            {
                result.FillInClaims(claims, guidMappings);
                result.Validate();
            }
            catch (Exception e)
            {
                result.InternalErrorMessages.Add(
                    $"PahaToken: Exception occured during processing token! Exception {e.Message}");
            }

            return result;
        }

        private static IPahaToken ResolvePahaTokenVersion(List<Claim> claims)
        {
            if (claims.Any(x => x.Type == PahaClaims.LoginStyle && !x.Value.IsNullOrWhitespace()))
            {
                return new PahaToken2();
            }
            return new PahaToken1();
        }

        protected Guid ParseGuid(string claimValue)
        {
            if (Guid.TryParse(claimValue, out var result))
            {
                return result;
            }

            return default;
        }

        protected long ParseLong(string claimValue)
        {
            if (long.TryParse(claimValue, out var result))
            {
                return result;
            }

            return default;
        }
    }
}
