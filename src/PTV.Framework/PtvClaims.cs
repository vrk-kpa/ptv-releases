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
using System.Text;

namespace PTV.Framework
{
    public class PtvClaims
    {
        public const string UserAccessRights = "user_access_rights";
        //public const string UserOrganizations = "user_organizations";

        public const string HandlingPrefixAction = "for_action_needed_";
        public const string HandlingPrefixUser = "user_has_assigned_";
    }

    public class PahaClaims
    {
        // common
        public const string ActiveOrganizationId = "activeOrganizationId";
        public const string Email = "email";
        public const string Expiration = "exp";
        public const string FirstName = "firstName";
        public const string Id = "id";
        public const string LastName = "lastName";
        public const string Roles = "roles";
        
        // ver 1
        public const string ApiUserOrganization = "apiUserOrganization";
        public const string ClientId = "client_id";
        public const string Organizations = "organizations";
        public const string Scope = "scope";
        public const string UserName = "userName";
        
        // ver 2
        public const string OrganizationNameEnglish = "activeOrganizationNameEn";
        public const string OrganizationNameFinnish = "activeOrganizationNameFi";
        public const string OrganizationNameSwedish = "activeOrganizationNameSv";
        public const string AvailableServices = "availServices";
        public const string ExpiresIn = "expiresIn";
        public const string Issuer = "iss";
        public const string LoginStyle = "loginStyle";
        public const string SamlIssuer = "samlIssuer";
        public const string Sub = "sub";
        public const string VrkServices = "vrkServices";
        
        // unused
        public const string Iat = "iat";
        public const string Jti = "jti";
    }

    public class PahaRolesDto
    {
        public bool active { get; set; }
        public List<string> Ptv { get; set; }
    }

    public class PahaOrganizationDto
    {
        public PahaOrganizationDto(PahaOrganizationInternalDto source)
        {
            this.Id = source.Id;
            this.FromTokenId = source.Id;
            this.Name = source.Name;
            this.Role = source.Roles != null ? source.Roles.FirstOrDefault() : string.Empty;
        }
        public Guid Id { get; set; }
        public Guid FromTokenId { get; set; }

        public string Name { get; set; }
        public string Role { get; set; }
    }

    public class PahaOrganizationInternalDto
    {
        public PahaOrganizationInternalDto() { }
        public PahaOrganizationInternalDto(PahaOrganizationDto source)
        {
            this.Id = source.Id;
            this.Name = source.Name;
            this.Roles = new List<string> {source.Role};
        }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> Roles { get; set; }
    }

    public class PahaGlobalRolesDto
    {
        // because of inconsistent naming used by PAHA
        // ReSharper disable once InconsistentNaming
        public List<string> Vrk_Ptv { get; set; }
    }
}
