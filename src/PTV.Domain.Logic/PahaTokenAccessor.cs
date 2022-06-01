using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;
using PTV.Framework.Paha;

namespace PTV.Domain.Logic
{
    public interface IPahaTokenAccessor
    {
        AccessRightEnum AccessRights { get; }
        Guid ActiveOrganizationId { get; }
        string ErrorMessageFlat { get; }
        bool IsUserPresent { get; }
        string UserName { get; }
        UserRoleEnum UserRole { get; }

        IPahaTokenAccessor ProcessToken(string token = null);
    }

    [RegisterService(typeof(IPahaTokenAccessor), RegisterType.Transient)]
    public class PahaTokenAccessor : IPahaTokenAccessor
    {
        private readonly IResolveManager resolveManager;

        public PahaTokenAccessor(IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
        }

        private IPahaToken GetToken()
        {
            using (var scope = resolveManager.CreateScope())
            {
                var pahaTokenProcessor = scope.ServiceProvider.GetService<IPahaTokenProcessorInternal>();
                return pahaTokenProcessor.GetToken();
            }
        }

        private VmUserAccessRights GetUserAccessRights()
        {
            using (var scope = resolveManager.CreateScope())
            {
                var pahaTokenProcessor = scope.ServiceProvider.GetService<IPahaTokenProcessorInternal>();
                return pahaTokenProcessor.GetUserAccessRights();
            }
        }

        private Guid GetActiveOrganizationId()
        {
            using (var scope = resolveManager.CreateScope())
            {
                var pahaTokenProcessor = scope.ServiceProvider.GetService<IPahaTokenProcessor>();
                return pahaTokenProcessor.ActiveOrganization;
            }
        }

        private List<string> GetErrorMessages()
        {
            using (var scope = resolveManager.CreateScope())
            {
                var pahaTokenProcessor = scope.ServiceProvider.GetService<IPahaTokenProcessor>();
                return pahaTokenProcessor.ErrorMessages;
            }
        }

        public AccessRightEnum AccessRights => GetUserAccessRights().AccessRights;
        public Guid ActiveOrganizationId => GetActiveOrganizationId();
        public string ErrorMessageFlat => string.Join(" // ", GetErrorMessages());
        public bool IsUserPresent => !string.IsNullOrEmpty(UserName);
        public string UserName => GetToken()?.Email;
        public UserRoleEnum UserRole => GetUserAccessRights().UserRole;
        public IPahaTokenAccessor ProcessToken(string token = null)
        {
            using (var scope = resolveManager.CreateScope())
            {
                var pahaTokenProcessor = scope.ServiceProvider.GetService<IPahaTokenProcessor>();
                pahaTokenProcessor.ProcessToken(token);
                return this;
            }
        }

        public static IPahaToken ExtractPahaToken(IEnumerable<Claim> claims, Dictionary<Guid,Guid> guidMappings = null)
        {
            return PahaTokenBase.ExtractPahaToken(claims.ToList(), guidMappings);
        }
    }
}
