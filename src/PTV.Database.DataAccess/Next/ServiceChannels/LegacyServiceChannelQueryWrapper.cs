using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Models.V2.Channel;
using PTV.Framework;
using PTV.Next.Model;
using System;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Database.DataAccess.Interfaces.Repositories;
using System.Linq;

namespace PTV.Database.DataAccess.Next.ServiceChannels
{
    [RegisterService(typeof(ILegacyServiceChannelQueryWrapper), RegisterType.Transient)]
    internal class LegacyServiceChannelQueryWrapper : ILegacyServiceChannelQueryWrapper
    {
        private readonly IChannelService channelService;
        private readonly IContextManager contextManager;
        private readonly ITypesCache typesCache;
        private readonly ILegacyServiceChannelMapper mapper;

        public LegacyServiceChannelQueryWrapper(IChannelService channelService,
            ITypesCache typesCache,
            IContextManager contextManager,
            ILegacyServiceChannelMapper mapper)
        {
            this.mapper = mapper;
            this.channelService = channelService;
            this.typesCache = typesCache;
            this.contextManager = contextManager;
        }

        public ConnectableChannelSearchResultModel GetConnectableChannels(ConnectableChannelSearchModel searchParameters)
        {
            var result = channelService.GetConnectableChannels(this.mapper.Map(searchParameters));
            return mapper.Map(result);
        }

        public ServiceChannelModel GetChannelLatestVersionByUnificRootId(Guid unificRootId, bool includeConnections)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var repository = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                var channel = repository.All().OrderByDescending(x => x.Versioning.VersionMajor)
                    .ThenByDescending(x => x.Versioning.VersionMinor)
                    .FirstOrDefault(x => x.UnificRootId == unificRootId);
                return channel == null ? null : GetChannel(channel, includeConnections);
            });
        }

        private ServiceChannelModel GetChannel(ServiceChannelVersioned channel, bool includeConnections)
        {
            var channelType = typesCache.GetByValue<ServiceChannelType>(channel.TypeId).ToEnum<ServiceChannelTypeEnum>();
            var parameters = new VmChannelBasic{ Id = channel.Id, IncludeConnections = includeConnections};

            if (channelType == ServiceChannelTypeEnum.EChannel)
            {
                var vm = channelService.GetElectronicChannel(parameters);
                return mapper.Map(vm, channel);
            }

            if (channelType == ServiceChannelTypeEnum.Phone)
            {
                var vm = channelService.GetPhoneChannel(parameters);
                return mapper.Map(vm, channel);
            }

            if (channelType == ServiceChannelTypeEnum.PrintableForm)
            {
                var vm = channelService.GetPrintableFormChannel(parameters);
                return mapper.Map(vm, channel);
            }

            if (channelType == ServiceChannelTypeEnum.ServiceLocation)
            {
                var vm = channelService.GetServiceLocationChannel(parameters);
                return mapper.Map(vm, channel);
            }

            if (channelType == ServiceChannelTypeEnum.WebPage)
            {
                var vm = channelService.GetWebPageChannel(parameters);
                return mapper.Map(vm, channel);
            }

            throw new Exception($"Cannot get channel with id {channel.Id} because channel type {channelType} is not supported");            
        }
    }
}
