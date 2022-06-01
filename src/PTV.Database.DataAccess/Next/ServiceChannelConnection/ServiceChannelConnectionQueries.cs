using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Next.Model;
using System;
using PTV.Database.DataAccess.Next.ServiceChannels;
using PTV.Framework;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Database.DataAccess.Interfaces.Repositories;
using System.Linq;
using PTV.Database.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Next.ServiceChannelConnection
{
    [RegisterService(typeof(IServiceChannelConnectionQueries), RegisterType.Transient)]
    internal class ServiceChannelConnectionQueries: IServiceChannelConnectionQueries
    {
        private readonly IContextManager contextManager;
        private readonly IServiceChannelQuery getServiceChannelQuery;
        private readonly IGetConnection getConnectionQuery;
        private readonly IConnectionMapper mapper;
        private readonly IConnectionAccessCheck accessCheck;
        private readonly IChannelsConnectedToServiceQuery channelsConnectedToServiceQuery;
        private readonly IServiceChannelQuery serviceChannelQuery;

        public ServiceChannelConnectionQueries(IContextManager contextManager,
            IServiceChannelQuery getServiceChannelQuery,
            IGetConnection getConnectionQuery,
            IConnectionMapper mapper,
            IConnectionAccessCheck accessCheck,
            IChannelsConnectedToServiceQuery channelsConnectedToServiceQuery,
            IServiceChannelQuery serviceChannelQuery)
        {
            this.contextManager = contextManager;
            this.getServiceChannelQuery = getServiceChannelQuery;
            this.getConnectionQuery = getConnectionQuery;
            this.mapper = mapper;
            this.accessCheck = accessCheck;
            this.channelsConnectedToServiceQuery = channelsConnectedToServiceQuery;
            this.serviceChannelQuery = serviceChannelQuery;
        }

        public ConnectionModel GetConnectionForService(Guid serviceId, Guid serviceChannelUnificRootId)
        {
            return contextManager.ExecuteReader(unitOfWork => 
            {
                var service = GetService(unitOfWork, serviceId);
                if (service == null)
                {
                    return null;
                }

                var connection = getConnectionQuery.GetWithAllDetails(unitOfWork, service.UnificRootId, serviceChannelUnificRootId);
                var channel = getServiceChannelQuery.Get(unitOfWork, serviceChannelUnificRootId);
                if (connection == null || channel == null)
                {
                    return null;
                }

                return mapper.Map(connection, channel, service);
            });
        }

        public void RemoveConnection(Guid serviceId, Guid serviceChannelUnificRootId)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var service = GetService(unitOfWork, serviceId);
                if (service == null)
                {
                    return;
                }

                var repository = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
                var connection = repository.All().SingleOrDefault(x => x.ServiceId == service.UnificRootId && x.ServiceChannelId == serviceChannelUnificRootId);
                var channel = getServiceChannelQuery.Get(unitOfWork, serviceChannelUnificRootId);
                if (connection == null || channel == null)
                {
                    return;
                }

                accessCheck.CanRemoveConnectionFromService(service, channel, connection);

                repository.Remove(connection);
                unitOfWork.Save();
            });
        }

        public List<ConnectableChannelModel> ConnectServiceToChannels(ConnectToChannelsModel model)
        {
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var service = GetService(unitOfWork, model.ServiceId);
                if (service == null)
                {
                    throw new Exception($"Cannot connect service to channels. Service {model.ServiceId} not found");
                }

                var repository = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
                var channels = GetChannelsToBeConnectedToService(unitOfWork, repository, model, service);

                if (channels.Any())
                {
                    accessCheck.CanConnectServiceToChannels(service, channels);

                    foreach (var ch in channels)
                    {
                        repository.Add(new ServiceServiceChannel
                        {
                            ServiceId = service.UnificRootId,
                            ServiceChannelId = ch.UnificRootId,
                        });
                    }

                    unitOfWork.Save();
                 }

                return channelsConnectedToServiceQuery.GetChannelsConnectedToService(unitOfWork, service.UnificRootId);                
            });
        }

        private List<ServiceChannelVersioned> GetChannelsToBeConnectedToService(IUnitOfWork unitOfWork,
            IServiceServiceChannelRepository repository,
            ConnectToChannelsModel model,
            ServiceVersioned service)
        {
            var currentConnections = repository.All().Where(x => x.ServiceId == service.UnificRootId).ToList();

            var newChannelIds = model.ServiceChannelUnificRootIds.Where(serviceChannelUnificRootId =>
            {
                return currentConnections.FirstOrDefault(c => c.ServiceChannelId == serviceChannelUnificRootId) == null;
            }).ToList();

            return serviceChannelQuery.Get(unitOfWork, newChannelIds);
        }

        private ServiceVersioned GetService(IUnitOfWork unitOfWork, Guid serviceId)
        {
            var repository = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            return repository.All()
                .Include(x => x.LanguageAvailabilities)
                .FirstOrDefault(x => x.Id == serviceId);
        }
    }
}
