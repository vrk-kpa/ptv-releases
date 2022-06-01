using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;
using PTV.Framework.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Next.Security
{
    public interface IAccessChecker
    {
        UserRoleEnum Role { get; }
        IPahaTokenAccessor Token { get; }
        bool IsPtvAdmin { get; }
        string UserName { get; }

        bool IsUserOrganization(Guid targetOrganizationId);
        bool AreUserOrganizations(List<Guid> targetOrganizationIds);

        void CheckDelete(DomainEnum domain, Guid targetOrganizationId, string forbiddenErrorMessage = null);
        void CheckCreate(DomainEnum domain, Guid targetOrganizationId, string forbiddenErrorMessage = null);
        void CheckUpdate(DomainEnum domain, Guid targetOrganizationId, string forbiddenErrorMessage = null);
    }

    [RegisterService(typeof(IAccessChecker), RegisterType.Transient)]
    public class AccessChecker: IAccessChecker
    {
        public AccessChecker(IPahaTokenAccessor token, IUserInfoService userInfoService)
        {
            Token = token;
            UserInfoService = userInfoService;
        }

        public IPahaTokenAccessor Token { get; set; }

        public IUserInfoService UserInfoService { get; set; }

        public UserRoleEnum Role => Token.UserRole;
        public string UserName => Token.UserName;
        public bool IsPtvAdmin => Role == UserRoleEnum.Eeva;

        public bool IsUserOrganization(Guid targetOrganizationId)
        {
            return Token.ActiveOrganizationId == targetOrganizationId;
        }

        public bool AreUserOrganizations(List<Guid> targetOrganizationIds)
        {
            return targetOrganizationIds.All(x => x == Token.ActiveOrganizationId);
        }

        public void CheckUpdate(DomainEnum domain,
            Guid targetOrganizationId,
            string forbiddenErrorMessage = null)
        {
            Check(domain, PermisionEnum.Update, targetOrganizationId, forbiddenErrorMessage);
        }

        public void CheckDelete(DomainEnum domain,
            Guid targetOrganizationId,
            string forbiddenErrorMessage = null)
        {
            Check(domain, PermisionEnum.Delete, targetOrganizationId, forbiddenErrorMessage);
        }

        public void CheckCreate(DomainEnum domain,
            Guid targetOrganizationId,
            string forbiddenErrorMessage = null)
        {
            Check(domain, PermisionEnum.Create, targetOrganizationId, forbiddenErrorMessage);
        }

        private void Check(DomainEnum domainToCheck, 
            PermisionEnum permissionToCheck, 
            Guid targetOrganizationId,
            string forbiddenErrorMessage)
        {
            var domain = domainToCheck.ToCamelCase();
            var allPermissions = UserInfoService.GetPermissions();
            var permission = allPermissions.TryGet(domain);
            if (permission == null)
            {
                throw new OperationForbiddenException($"No permissions for domain {domain} for user {Token.UserName}");
            }

            ThrowIfNoPermission(domainToCheck,
                IsUserOrganization(targetOrganizationId) ? permission.RulesOwn : permission.RulesAll, permissionToCheck,
                targetOrganizationId, forbiddenErrorMessage);
        }

        private void ThrowIfNoPermission(DomainEnum domain,
            PermisionEnum rule, 
            PermisionEnum permissionToCheck,
            Guid targetOrganizationId,
            string forbiddenErrorMessage)
        {
            if (rule.HasFlag(permissionToCheck))
            {
                return;
            }

            var isOwnOrg = IsUserOrganization(targetOrganizationId);

            if (string.IsNullOrEmpty(forbiddenErrorMessage))
            {
                var msg = $"User {UserName} has no rights to {permissionToCheck} using domain {domain} against org {targetOrganizationId} (Is own org = {isOwnOrg})";
                throw new OperationForbiddenException(msg);
            }

            throw new OperationForbiddenException(forbiddenErrorMessage);
        }
    }
}
