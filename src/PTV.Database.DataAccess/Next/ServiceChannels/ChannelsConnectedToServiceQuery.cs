using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Framework;
using PTV.Next.Model;
using System;
using System.Collections.Generic;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Database.DataAccess.Interfaces.Repositories;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PTV.Database.DataAccess.Next.ServiceChannels
{
    internal interface IChannelsConnectedToServiceQuery
    {
        List<ConnectableChannelModel> GetChannelsConnectedToService(Guid serviceUnificRootId);
        List<ConnectableChannelModel> GetChannelsConnectedToService(IUnitOfWork unitOfWork, Guid serviceUnificRootId);
    }

    [RegisterService(typeof(IChannelsConnectedToServiceQuery), RegisterType.Transient)]
    internal class ChannelsConnectedToServiceQuery : IChannelsConnectedToServiceQuery
    {
        private readonly IContextManager contextManager;
        private readonly ITypesCache typesCache;
        private readonly IConnectableChannelMapper mapper;

        public ChannelsConnectedToServiceQuery(IContextManager contextManager,
            ITypesCache typesCache,
            IConnectableChannelMapper mapper)
        {
            this.contextManager = contextManager;
            this.typesCache = typesCache;
            this.mapper = mapper;
        }

        public List<ConnectableChannelModel> GetChannelsConnectedToService(IUnitOfWork unitOfWork, Guid serviceUnificRootId)
        {
            var connections = GetConnections(unitOfWork, serviceUnificRootId);
            var serviceChannelUnificRootIds = connections.Select(x => x.ServiceChannelId).ToList();
            var channels = GetChannels(unitOfWork, serviceChannelUnificRootIds);

            var result = new List<ConnectableChannelModel>();

            foreach(var connection in connections)
            {
                if (channels.TryGetValue(connection.ServiceChannelId, out var channel))
                {
                    result.Add(mapper.Map(channel, connection));
                }
            }

            return result;
        }

        public List<ConnectableChannelModel> GetChannelsConnectedToService(Guid serviceUnificRootId)
        {
            return contextManager.ExecuteReader(unitOfWork => 
            {
                return GetChannelsConnectedToService(unitOfWork, serviceUnificRootId);
            });
        }

        public Dictionary<Guid, ServiceChannelVersioned> GetChannels(IUnitOfWork unitOfWork, List<Guid> serviceChannelUnificRootIds)
        {
            // Following logic is quite similar as in ConnectionsService.GetAllServiceRelations

            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var allowedStatuses = GetAllowedPublishingStatuses();

            var serviceChannelRepository = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();                

            var channels = serviceChannelRepository.All()
                .Include(x => x.ServiceChannelNames)
                .Include(j => j.LanguageAvailabilities)
                .Include(j => j.DisplayNameTypes)
                .Include(j => j.Type)
                .Where(x => serviceChannelUnificRootIds.Contains(x.UnificRootId) && allowedStatuses.Contains(x.PublishingStatusId))
                .ToList()
                .GroupBy(x => x.UnificRootId).Select(x =>
                    x.OrderBy(y =>
                        y.PublishingStatusId == publishingStatusId ? 0 :
                        y.PublishingStatusId == draftStatusId ? 1 :
                        y.PublishingStatusId == modifiedStatusId ? 2 : 3).FirstOrDefault())
                .ToDictionary(x => x.UnificRootId, y => y);

            return channels;
        }

        private List<ServiceServiceChannel> GetConnections(IUnitOfWork unitOfWork, Guid serviceUnificRootId)
        {
            var connectionsRepository = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            return connectionsRepository.All().Where(x => x.ServiceId == serviceUnificRootId).ToList();
        }

        private List<Guid> GetAllowedPublishingStatuses()
        {
            return new List<Guid> {
                typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString()),
                typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString()),
                typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString())
            };
        }
    }
}
