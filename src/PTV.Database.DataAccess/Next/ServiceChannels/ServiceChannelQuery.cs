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

namespace PTV.Database.DataAccess.Next.ServiceChannels
{
    internal interface IServiceChannelQuery
    {
        ServiceChannelVersioned Get(IUnitOfWork unitOfWork, Guid unificRootId);
        List<ServiceChannelVersioned> Get(IUnitOfWork unitOfWork, List<Guid> unificRootIds);
    }

    [RegisterService(typeof(IServiceChannelQuery), RegisterType.Transient)]
    internal class ServiceChannelQuery : IServiceChannelQuery
    {
        private readonly ITypesCache typesCache;

        public ServiceChannelQuery(ITypesCache typesCache)
        {
            this.typesCache = typesCache;
        }

        public List<ServiceChannelVersioned> Get(IUnitOfWork unitOfWork, List<Guid> unificRootIds)
        {
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var allowedStatuses = GetAllowedPublishingStatuses();

            var repository = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();

            var channels = repository.All()
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
            return channels.Values.ToList();
        }

        public ServiceChannelVersioned Get(IUnitOfWork unitOfWork, Guid unificRootId)
        {
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var draftStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var allowedStatuses = GetAllowedPublishingStatuses();

            var repository = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();

            var channel = repository.All()
                .Include(x => x.ServiceChannelNames)
                .Include(j => j.LanguageAvailabilities)
                .Include(j => j.DisplayNameTypes)
                .Include(j => j.Type)
                .Where(x => x.UnificRootId == unificRootId && allowedStatuses.Contains(x.PublishingStatusId))
                .ToList()
                .OrderBy(y =>
                    y.PublishingStatusId == publishingStatusId ? 0 :
                    y.PublishingStatusId == draftStatusId ? 1 :
                    y.PublishingStatusId == modifiedStatusId ? 2 : 3).FirstOrDefault();
            return channel;

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
