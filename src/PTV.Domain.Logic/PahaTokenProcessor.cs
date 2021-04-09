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
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PTV.Database.DataAccess.Interfaces;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;
using PTV.Framework.Paha;

namespace PTV.Domain.Logic
{

    public interface IPahaTokenProcessor
    {
        string UserName { get; }
        string FirstName { get; }
        string Surname { get; }
        string AccessRightGroup { get; }
        AccessRightEnum UserAccessRights { get; }
        UserRoleEnum UserRole { get; }
        Guid ActiveOrganization { get; }
        bool UserPresent { get; }

        // Dictionary<Guid, PahaOrganizationDto> UserOrganizations { get; }
        List<string> ErrorMessages { get; }
        List<string> WarningMessages { get; }
        string InternalToken { get; }

        void ProcessToken();

        void ProcessToken(string bearer);

        bool IsTokenPresent { get; }
        PahaOrganizationDto SelectedOrganization { get; }
        Guid OriginalTokenActiveOrganization { get; }
        string ErrorMessageFlat { get; }
        string WarningMessageFlat { get; }
        bool InvalidToken { get; }
    }

    internal interface IPahaTokenProcessorInternal : IPahaTokenProcessor
    {
        IPahaToken GetToken();
        VmUserAccessRights GetUserAccessRights();
    }

    [RegisterService(typeof(IPahaTokenProcessor), RegisterType.Scope)]
    [RegisterService(typeof(IPahaTokenProcessorInternal), RegisterType.Scope)]
    public class PahaTokenProcessor : IPahaTokenProcessorInternal
    {
        private const string InvalidTokenMsg = "Invalid token. Check error messages from processed token.";
        private const string TokenErrMsgMissingUserRole = "PahaToken: User access right group is missing!";
        private const string TokenErrMsgInvalidUserRole = "PahaToken: User access right group is invalid!";
        private const string TokenMsgTokenErrorMsgs = "Token is invalid. Error: {0}";

        private readonly IHttpContextAccessor contextAccessor;
        private readonly IUserAccessRightsCache userAccessRightsCache;
        private IPahaToken token;
        private VmUserAccessRights userAccessRights;
        private Guid userOrganization;
        private string tokenRaw;

        private readonly ILogger<PahaTokenProcessor> logger;
        private readonly IOrganizationTreeDataCache organizationTreeDataCache;
        // private readonly IResolveManager resolveManager;

        public PahaTokenProcessor(IHttpContextAccessor contextAccessor, IUserAccessRightsCache userAccessRightsCache,
            ILogger<PahaTokenProcessor> logger, IOrganizationTreeDataCache organizationTreeDataCache)
        {
            this.contextAccessor = contextAccessor;
            this.userAccessRightsCache = userAccessRightsCache;
            this.logger = logger;
            this.organizationTreeDataCache = organizationTreeDataCache;
            ProcessToken();
        }

        private void ClearToken()
        {
            token = new PahaToken2();
            userAccessRights = new VmUserAccessRights { UserRole  = UserRoleEnum.Shirley, AccessRights = 0, EntityId = Guid.Empty, GroupCode = "none"};
            userOrganization = new Guid();
            sahaIdChecked = false;
        }

        // public Dictionary<Guid, PahaOrganizationDto> UserOrganizations => token.AllOrganizations;

        public string InternalToken => tokenRaw;

        public void ProcessToken()
        {
            var httpRequest = contextAccessor.HttpContext?.Request;
            if (httpRequest == null || !httpRequest.Headers.ContainsKey("Authorization"))
            {
                ClearToken();
                return;
            }

            var authHeader = httpRequest.Headers["Authorization"].ToString();
            if (authHeader.ToLowerInvariant().StartsWith("bearer"))
            {
                var bearer = httpRequest.Headers["Authorization"].ToString().Replace("bearer", string.Empty, true, CultureInfo.InvariantCulture).Replace(" ", string.Empty); // skip Bearer header
                ProcessToken(bearer);
            }
            else
            {
                this.token = new PahaToken2();
            }

            if (InvalidToken)
            {
                logger?.LogError(string.Format(TokenMsgTokenErrorMsgs, ErrorMessageFlat));
            }
        }

        public void ProcessToken(string bearer)
        {
            if (string.IsNullOrEmpty(bearer))
            {
                ClearToken();
                return;
            }
            this.token = ExtractPahaToken(bearer);
            this.userOrganization = token.ActiveOrganizationId;
            var globalOverridedGroup = token.PtvRole.ToString();
            VmUserAccessRights knownGroup = null;
            if (!string.IsNullOrEmpty(globalOverridedGroup) && ((knownGroup = userAccessRightsCache.Get(globalOverridedGroup)) != null))
            {
                this.userAccessRights = knownGroup;
                SelectedOrganization.SafeCall(i => i.Role = globalOverridedGroup);
            }
            else
            {
                var userAccessGroup = SelectedOrganization?.Role;
                if (!string.IsNullOrEmpty(userAccessGroup))
                {
                    knownGroup = userAccessRightsCache.Get(userAccessGroup);
                    if (knownGroup != null)
                    {
                        this.userAccessRights = knownGroup;
                    }
                    else
                    {
                        this.token.InternalErrorMessages.Add(TokenErrMsgInvalidUserRole);
                    }
                }
                else
                {
                    this.token.InternalErrorMessages.Add(TokenErrMsgMissingUserRole);
                }
            }
        }

        public bool IsTokenPresent => ((token != null) && (SelectedOrganization != null)) && (!string.IsNullOrEmpty(UserName)) && (!string.IsNullOrEmpty(AccessRightGroup));

        public PahaOrganizationDto SelectedOrganization
        {
            get
            {
                if (selectedOrganization?.Id != token.ActiveOrganizationId)
                {
                    selectedOrganization = new PahaOrganizationDto(new PahaOrganizationInternalDto
                    {
                        Id = token.ActiveOrganizationId,
                        Name = token.ActiveOrganizationName
                    });
                }

                return selectedOrganization;
            }
        } 

        public bool UserPresent => !string.IsNullOrEmpty(UserName);

        public string UserName => token?.Email;
        public string FirstName => token.FirstName;
        public string Surname => token.LastName;
        public string AccessRightGroup => userAccessRights?.GroupCode ?? throw new Exception(InvalidTokenMsg);
        public AccessRightEnum UserAccessRights => userAccessRights?.AccessRights ?? throw new Exception(InvalidTokenMsg);
        public UserRoleEnum UserRole => userAccessRights?.UserRole ?? throw new Exception(InvalidTokenMsg);

        public Guid OriginalTokenActiveOrganization => token?.ActiveOrganizationId ??  throw new Exception(InvalidTokenMsg);

        public Guid ActiveOrganization
        {
            get
            {
                if (!sahaIdChecked)
                {
                    var ptvOrgId = organizationTreeDataCache.FindBySahaId(this.userOrganization);
                    if (ptvOrgId != null)
                    {
                        var orgToUpdate = SelectedOrganization;
                        this.userOrganization = ptvOrgId.Value;
                        orgToUpdate.SafeCall(i => i.Id = ptvOrgId.Value);
                    }
                    sahaIdChecked = true;
                }
                return userOrganization;
            }
        }

        public List<string> ErrorMessages => token.InternalErrorMessages;

        public string ErrorMessageFlat => string.Join(" // ", ErrorMessages);

        public List<string> WarningMessages => token.InternalWarningMessages;

        public string WarningMessageFlat => string.Join(" // ", WarningMessages);

        public bool InvalidToken => ErrorMessages.Any();

        private bool sahaIdChecked = false;
        private PahaOrganizationDto selectedOrganization;

        private IPahaToken ExtractPahaToken(string bearer)
        {
            try
            {
                var encodedToken = new JwtSecurityToken(bearer);
                tokenRaw = encodedToken.RawData;
                return ExtractPahaToken(encodedToken.Claims, organizationTreeDataCache.SahaMappings());
            }
            catch (Exception)
            {
                return new PahaToken2();
            }
        }

        public static IPahaToken ExtractPahaToken(IEnumerable<Claim> claims, Dictionary<Guid,Guid> guidMappings = null)
        {
            return PahaTokenBase.ExtractPahaToken(claims.ToList(), guidMappings);
        }

        public IPahaToken GetToken()
        {
            return token;
        }

        public VmUserAccessRights GetUserAccessRights()
        {
            return userAccessRights;
        }
    }
}
