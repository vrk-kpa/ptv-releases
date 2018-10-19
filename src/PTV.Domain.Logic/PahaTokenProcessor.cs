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

        Dictionary<Guid, PahaOrganizationDto> UserOrganizations { get; }
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

    [RegisterService(typeof(IPahaTokenProcessor), RegisterType.Scope)]
    public class PahaTokenProcessor : IPahaTokenProcessor
    {
        private const string InvalidTokenMsg = "Invalid token. Check error messages from processed token.";
        private const string TokenErrMsgMissingUserRole = "PahaToken: User access right group is missing!";
        private const string TokenErrMsgInvalidUserRole = "PahaToken: User access right group is invalid!";
        private const string TokenMsgNoToken = "No token available.";
        private const string TokenMsgTokenPresent = "Bearer token {0}";
        private const string TokenMsgTokenErrorMsgs = "Token is invalid. Error: {0}";

        private readonly IHttpContextAccessor contextAccessor;
        private readonly IUserAccessRightsCache userAccessRightsCache;
        private PahaToken token;
        private VmUserAccessRights userAccessRights;
        private Guid userOrganization;
        private string tokenRaw;
        private readonly IResolveManager resolveManager;

        public PahaTokenProcessor(IHttpContextAccessor contextAccessor, IUserAccessRightsCache userAccessRightsCache, IResolveManager resolveManager)
        {
            this.contextAccessor = contextAccessor;
            this.userAccessRightsCache = userAccessRightsCache;
            this.resolveManager = resolveManager;
            ProcessToken();
        }

        private void ClearToken()
        {
            token = new PahaToken();
            userAccessRights = new VmUserAccessRights() { UserRole  = UserRoleEnum.Shirley, AccessRights = 0, EntityId = Guid.Empty, GroupCode = "none"};
            userOrganization = new Guid();
            sahaIdChecked = false;
        }

        public Dictionary<Guid, PahaOrganizationDto> UserOrganizations => token.AllOrganizations;

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
                this.token = new PahaToken();
            }

            if (InvalidToken)
            {
                var logger = resolveManager.Resolve<ILogger<PahaTokenProcessor>>(true);
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
            var globalOverridedGroup = token.GlobalPtvRole;
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
        public PahaOrganizationDto SelectedOrganization => (UserOrganizations.Any() && OriginalTokenActiveOrganization.IsAssigned()) ? UserOrganizations.TryGetOrDefault(OriginalTokenActiveOrganization, null) : null;

        public bool UserPresent => !string.IsNullOrEmpty(UserName);

        public string UserName => token?.UserName;
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
                    var organizationTreeDataCache = resolveManager.Resolve<IOrganizationTreeDataCache>();
                    var ptvOrgId = organizationTreeDataCache.FindBySahaId(this.userOrganization);
                    if (ptvOrgId != null)
                    {
                        var orgToUpdate = this.UserOrganizations.TryGetOrDefault(this.userOrganization, null);
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

        private PahaToken ExtractPahaToken(string bearer)
        {
            try
            {
                var encodedToken = new JwtSecurityToken(bearer);
                tokenRaw = encodedToken.RawData;
                return ExtractPahaToken(encodedToken.Claims, resolveManager.Resolve<IOrganizationTreeDataCache>(true)?.SahaMappings());
            }
            catch (Exception)
            {
                return new PahaToken();
            }
        }

        public static PahaToken ExtractPahaToken(IEnumerable<Claim> claims, Dictionary<Guid,Guid> guidMappings = null)
        {
            return Framework.PahaToken.ExtractPahaToken(claims, guidMappings);
        }
    }
}