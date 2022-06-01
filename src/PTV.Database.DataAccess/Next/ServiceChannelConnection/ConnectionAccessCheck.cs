using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums.Security;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using System.Collections.Generic;
using PTV.Database.DataAccess.Next.Security;
using System;
using PTV.Framework.Exceptions;

namespace PTV.Database.DataAccess.Next.ServiceChannelConnection
{
    internal interface IConnectionAccessCheck
    {
        void CanRemoveConnectionFromService(ServiceVersioned service, ServiceChannelVersioned channel, ServiceServiceChannel connection);
        void CanConnectServiceToChannels(ServiceVersioned service, List<ServiceChannelVersioned> channels);
        void CanModifyConnection(Guid? serviceId, Guid servicePublishingStatus, Guid? serviceOrganizationId);
    }

    /// <summary>
    /// Checks that modifications to connections are allowed.
    /// See https://wiki.dvv.fi/display/SFIPTV/PTV+role+specific+rights
    /// </summary>
    [RegisterService(typeof(IConnectionAccessCheck), RegisterType.Transient)]
    internal class ConnectionAccessCheck : IConnectionAccessCheck
    {
        private readonly ITypesCache typesCache;
        private readonly IAccessChecker accessChecker;

        public ConnectionAccessCheck(ITypesCache typesCache, IAccessChecker accessChecker)
        {
            this.typesCache = typesCache;
            this.accessChecker = accessChecker;
        }

        public void CanModifyConnection(Guid? serviceId, Guid servicePublishingStatus, Guid? serviceOrganizationId)
        {
            if(!serviceId.HasValue)
                throw new OperationForbiddenException($"User {accessChecker.UserName} Cannot modify connection from service as serviceId is missing");
            if(!serviceOrganizationId.HasValue)
                throw new OperationForbiddenException($"User {accessChecker.UserName} Cannot modify connection from service {serviceId} as organizationId is missing");
            CheckServiceStatus(serviceId.Value, servicePublishingStatus);
            accessChecker.CheckUpdate(DomainEnum.Relations, serviceOrganizationId.Value);
        }

        public void CanRemoveConnectionFromService(ServiceVersioned service, 
            ServiceChannelVersioned channel,
            ServiceServiceChannel connection)
        {
			CheckServiceStatus(service);

            if (accessChecker.IsPtvAdmin)
            {
                accessChecker.CheckDelete(DomainEnum.Relations, service.OrganizationId);
                return;
            }
            
            if (connection.IsASTIConnection)
            {
                throw new OperationForbiddenException($"User {accessChecker.UserName} cannot remove ASTI connection from service {service.Id}");
            }

            // Service and channel belong to user's organization
            if (accessChecker.AreUserOrganizations(new List<Guid>{service.OrganizationId, channel.OrganizationId }))
            {
                accessChecker.CheckDelete(DomainEnum.Relations, service.OrganizationId);
                return;
            }

            // Service belongs to user's organization and channel is open
            if (accessChecker.IsUserOrganization(service.OrganizationId))
            {
                var connectionType = typesCache.GetByValue<ServiceChannelConnectionType>(channel.ConnectionTypeId).ToEnum<ServiceChannelConnectionTypeEnum>();
                if (connectionType == ServiceChannelConnectionTypeEnum.CommonForAll)
                {
                    accessChecker.CheckDelete(DomainEnum.Relations, service.OrganizationId);
                    return;
                }
            }

            throw new OperationForbiddenException($"User {accessChecker.UserName} cannot remove connection between service {service.Id} and channel {channel.Id}");
        }

        public void CanConnectServiceToChannels(ServiceVersioned service, List<ServiceChannelVersioned> channels)
        {
            channels.ForEach(x => CanConnectServiceToChannels(service, x));
        }

        private void CanConnectServiceToChannels(ServiceVersioned service, ServiceChannelVersioned channel)
        {
            CheckServiceStatus(service);

            if (accessChecker.IsPtvAdmin)
            {
                accessChecker.CheckCreate(DomainEnum.Relations, service.OrganizationId);
                return;
            }

            // Service and channel belong to user's organization
            if (accessChecker.AreUserOrganizations(new List<Guid>{service.OrganizationId, channel.OrganizationId }))
            {
                accessChecker.CheckCreate(DomainEnum.Relations, service.OrganizationId);
                return;
            }

            // Service belongs to user's organization and channel is open
            if (accessChecker.IsUserOrganization(service.OrganizationId))
            {
                var connectionType = typesCache.GetByValue<ServiceChannelConnectionType>(channel.ConnectionTypeId).ToEnum<ServiceChannelConnectionTypeEnum>();
                if (connectionType == ServiceChannelConnectionTypeEnum.CommonForAll)
                {
                    accessChecker.CheckCreate(DomainEnum.Relations, service.OrganizationId);
                    return;
                }
            }

            throw new OperationForbiddenException($"User {accessChecker.UserName} cannot create connection between service {service.Id} and channel {channel.Id}");
        }

        private void CheckServiceStatus(ServiceVersioned service)
        {
            CheckServiceStatus(service.Id, service.PublishingStatusId);
        }

        private void CheckServiceStatus(Guid serviceId, Guid servicePublishingStatus)
        {
            var status = typesCache.GetByValue<PublishingStatusType>(servicePublishingStatus).ToEnum<PublishingStatus>();

            if (status == PublishingStatus.Deleted ||
                status == PublishingStatus.OldPublished ||
                status == PublishingStatus.Removed)
            {
                throw new OperationForbiddenException($"User {accessChecker.UserName} Cannot modify connection from service {serviceId} because status is {status}");
            }
        }
    }
}
