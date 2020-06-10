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
using Newtonsoft.Json.Converters;
using PTV.Framework.Enums;

namespace PTV.Framework.Paha
{
    public class PahaToken2 : PahaTokenBase
    {
        public override string ActiveOrganizationName
        {
            get => ActiveOrganizationNameFi;
            set => throw new NotImplementedException();
        }
        public override UserAccessRightsGroupEnum PtvRole
        {
            get
            {
                var vrkRole = GetUserRole(VrkServices, PtvRoleKeys);
                var userRole = GetUserRole(Roles, PtvRoleKeys);

                var newRole = vrkRole ?? userRole ?? PahaUserRole.Viewer;
                var oldRole = UserRoleMap[newRole];
                return oldRole;
            }
            set => throw new NotImplementedException();
        }

        private PahaUserRole? GetUserRole(Dictionary<string, List<string>> roles, string[] keys)
        {
            if (roles == null)
            {
                return null;
            }
            
            var usedRoleKey = roles.Keys.FirstOrDefault(k => keys.Contains(k.ToLower()));
            if (usedRoleKey.IsNullOrWhitespace())
            {
                return null;
            }

            var firstRole = roles[usedRoleKey].FirstOrDefault();
            if (Enum.TryParse<PahaUserRole>(firstRole, true, out var result))
            {
                return result;
            }

            return null;
        }

        public override void Validate()
        {
            InternalErrorMessages = new List<string>();
            InternalWarningMessages = new List<string>();
            
            if (!ActiveOrganizationId.IsAssigned())
            {
                InternalErrorMessages.Add("PahaToken: User's active organization is not set!");
            }
            
            base.Validate();
        }

        public string ActiveOrganizationNameEn { get; set; }
        public string ActiveOrganizationNameFi { get; set; }
        public string ActiveOrganizationNameSv { get; set; }
        public List<string> AvailServices { get; set; }
        public long ExpiresIn { get; set; }
        public string Iss { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public PahaLoginStyle LoginStyle { get; set; }
        public string SamlIssuer { get; set; }
        public string Sub { get; set; }
        public Dictionary<string, List<string>> VrkServices { get; set; }
        
        public override void FillInClaims(List<Claim> claims, Dictionary<Guid, Guid> ptvSahaMapping)
        {
            ActiveOrganizationId = ParseGuid(GetClaimValue(claims, PahaClaims.ActiveOrganizationId));
            Email = GetClaimValue(claims, PahaClaims.Email);
            Exp = ParseLong(GetClaimValue(claims, PahaClaims.Expiration));
            FirstName = GetClaimValue(claims, PahaClaims.FirstName);
            Id = ParseGuid(GetClaimValue(claims, PahaClaims.Id));
            LastName = GetClaimValue(claims, PahaClaims.LastName);
            Roles = DeserializeJsonSafe<Dictionary<string, List<string>>>(GetClaimValue(claims, PahaClaims.Roles));
            ActiveOrganizationNameEn = GetClaimValue(claims, PahaClaims.OrganizationNameEnglish);
            ActiveOrganizationNameFi = GetClaimValue(claims, PahaClaims.OrganizationNameFinnish);
            ActiveOrganizationNameSv = GetClaimValue(claims, PahaClaims.OrganizationNameSwedish);
            AvailServices = GetClaimValueList(claims, PahaClaims.AvailableServices, new List<string>());
            ExpiresIn = ParseLong(GetClaimValue(claims, PahaClaims.ExpiresIn));
            Iss = GetClaimValue(claims, PahaClaims.Issuer);
            LoginStyle = Enum.Parse<PahaLoginStyle>(GetClaimValue(claims, PahaClaims.LoginStyle), true);
            SamlIssuer = GetClaimValue(claims, PahaClaims.SamlIssuer);
            Sub = GetClaimValue(claims, PahaClaims.Sub);
            VrkServices = DeserializeJsonSafe<Dictionary<string, List<string>>>(GetClaimValue(claims, PahaClaims.VrkServices));
        }
    }
}
