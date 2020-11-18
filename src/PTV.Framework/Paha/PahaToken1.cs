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
using PTV.Framework.Enums;

namespace PTV.Framework.Paha
{
    public class PahaToken1 : PahaTokenBase
    {
        public override string ActiveOrganizationName
        {
            get
            {
                if (Organizations == null || 
                    !ActiveOrganizationId.IsAssigned() && !ApiUserOrganization.IsAssigned())
                {
                    return null;
                }

                var searchId = ApiUserOrganization.IsAssigned() ? ApiUserOrganization : ActiveOrganizationId;
                return Organizations.Where(x => x.Id == searchId).Select(x => x.Name).FirstOrDefault();
            }
            set => throw new NotImplementedException();
        }

        public override UserAccessRightsGroupEnum PtvRole
        {
            get
            {
                var vrkRole = GetUserRole<UserAccessRightsGroupEnum>(VrkRoleKeys);
                var userRoleNew = GetUserRole<PahaUserRole>(PtvRoleKeys);
                var userRole = userRoleNew == null ? null : UserRoleMap[userRoleNew.Value] as UserAccessRightsGroupEnum?;
                var orgRole = GetRoleFromActiveOrganization();

                return vrkRole ?? userRole ?? orgRole ?? UserAccessRightsGroupEnum.PTV_VIEWER;
            }
            set => throw new NotImplementedException();
        }

        private T? GetUserRole<T>(string[] roleKeys) where T : struct
        {
            var usedRoleKey = Roles?.Keys.FirstOrDefault(k => roleKeys.Contains(k.ToLower()));
                
            if (Roles == null || usedRoleKey.IsNullOrWhitespace())
            {
                return null;
            }

            var firstRole = Roles[usedRoleKey].FirstOrDefault();
            if (Enum.TryParse<T>(firstRole, true, out var result))
            {
                return result;
            }
            return null;
        }

        private UserAccessRightsGroupEnum? GetRoleFromActiveOrganization()
        {
            if (!ActiveOrganizationId.IsAssigned() || Organizations == null)
            {
                return null;
            }

            var activeRole = Organizations.FirstOrDefault(o => o.Id == ActiveOrganizationId)?.Role;
            return activeRole.IsNullOrWhitespace() 
                ? null
                : Enum.Parse<UserAccessRightsGroupEnum>(activeRole, true) as UserAccessRightsGroupEnum?;
        }

        public override string Email
        {
            get => base.Email ?? UserName;
            set => base.Email = value;
        }

        public Guid? ApiUserOrganization { get; set; }
        // ReSharper disable once InconsistentNaming
        public Guid Client_id { get; set; }
        public List<PahaOrganizationDto> Organizations { get; set; }
        public string Scope { get; set; }
        public string UserName { get; set; }

        public override void Validate()
        {
            InternalErrorMessages = new List<string>();
            InternalWarningMessages = new List<string>();
            
            if (!ActiveOrganizationId.IsAssigned() && !ApiUserOrganization.IsAssigned())
            {
                InternalErrorMessages.Add("PahaToken: User's active organization is not set!");
            }
            
            base.Validate();
        }

        public override void FillInClaims(List<Claim> claims, Dictionary<Guid, Guid> ptvSahaMapping)
        {
            ActiveOrganizationId = ParseGuid(GetClaimValue(claims, PahaClaims.ActiveOrganizationId));
            Email = GetClaimValue(claims, PahaClaims.Email);
            Exp = ParseLong(GetClaimValue(claims, PahaClaims.Expiration));
            FirstName = GetClaimValue(claims, PahaClaims.FirstName);
            Id = ParseGuid(GetClaimValue(claims, PahaClaims.Id));
            LastName = GetClaimValue(claims, PahaClaims.LastName);
            Roles = DeserializeJsonSafe<Dictionary<string, List<string>>>(GetClaimValue(claims, PahaClaims.Roles));
            ApiUserOrganization = ParseGuid(GetClaimValue(claims, PahaClaims.ApiUserOrganization));
            Client_id = ParseGuid(GetClaimValue(claims, PahaClaims.ClientId));
            Organizations = GetOrganizationClaims(claims);
            Scope = GetClaimValue(claims, PahaClaims.Scope);
            UserName = GetClaimValue(claims, PahaClaims.UserName);
        }

        /// <summary>
        /// There is an issue with the organization claims. Sometimes they are in proper JSON
        /// array, but sometimes in a string containing the actual JSON. Therefore, we need to
        /// attempt to deserialize both options and choose whichever one comes populated.
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        private List<PahaOrganizationDto> GetOrganizationClaims(List<Claim> claims)
        {
            var listOrganizations = DeserializeJsonListSafe<PahaOrganizationInternalDto>(
                    GetClaimValueList(claims, PahaClaims.Organizations))
                .Select(x => new PahaOrganizationDto(x))
                .ToList();

            if (listOrganizations.Any())
            {
                return listOrganizations;
            }
            
            return DeserializeJsonSafe(
                    GetClaimValue(claims, PahaClaims.Organizations), new List<PahaOrganizationInternalDto>())
                .Select(x => new PahaOrganizationDto(x))
                .ToList();
        }
    }
}
