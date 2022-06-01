using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Framework;
using System;
using System.Collections.Generic;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Database.DataAccess.Interfaces.Repositories;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next.ServiceChannels
{
    internal interface IGdServiceChannelQuery
    {
        List<GdServiceChannelModel> GetForGeneralDescription(List<Guid> serviceChannelUnificRootIds);
    }

    [RegisterService(typeof(IGdServiceChannelQuery), RegisterType.Transient)]
    internal class GdServiceChannelQuery : IGdServiceChannelQuery
    {
        private readonly IContextManager contextManager;
        private readonly ITypesCache typesCache;
        private readonly IGdServiceChannelMapper mapper;

        public GdServiceChannelQuery(ITypesCache typesCache, 
            IContextManager contextManager,
            IGdServiceChannelMapper mapper)
        {
            this.typesCache = typesCache;
            this.contextManager = contextManager;
            this.mapper = mapper;
        }

        public List<GdServiceChannelModel> GetForGeneralDescription(List<Guid> serviceChannelUnificRootIds)
        {
            var channels = GetByUnificRootIds(serviceChannelUnificRootIds);
            return mapper.Map(channels);
        }

        private List<ServiceChannelVersioned> GetByUnificRootIds(List<Guid> unificRootIds)
        {
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var allowedStatuses = GetAllowedPublishingStatuses();

            return contextManager.ExecuteReader(unitOfWork => 
            { 
                var connectionsRepository = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
                var serviceChannelRepository = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();

                var channels = serviceChannelRepository.All()
                    .Include(x => x.ServiceChannelNames)
                    .Include(j => j.LanguageAvailabilities)
                    .Include(j => j.DisplayNameTypes)
                    .Include(j => j.Type)
                    .Where(x => unificRootIds.Contains(x.UnificRootId) && allowedStatuses.Contains(x.PublishingStatusId))
                    .ToList()
                    .GroupBy(x => x.UnificRootId).Select(x =>
                        x.OrderBy(y =>
                            y.PublishingStatusId == publishingStatusId ? 0 :
                            y.PublishingStatusId == draftStatusId ? 1 :
                            y.PublishingStatusId == modifiedStatusId ? 2 : 3).FirstOrDefault())
                    .ToDictionary(x => x.UnificRootId, y => y);

                return channels.Select(x => x.Value).ToList();
            });
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
