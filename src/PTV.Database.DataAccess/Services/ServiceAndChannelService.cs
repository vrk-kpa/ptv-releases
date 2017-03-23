/**
 * The MIT License
 * Copyright (c) 2016 Population Register Centre (VRK)
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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Enums;
using Microsoft.Extensions.Logging;
using PTV.Database.Model.Models.Base;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Logic;
using PTV.Domain.Model.Models.OpenApi;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Utils;
using PTV.Domain.Logic.Services;
using PTV.Domain.Model.Models.OpenApi.V2;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof (IServiceAndChannelService), RegisterType.Transient)]
    internal class ServiceAndChannelService : ServiceBase, IServiceAndChannelService
    {
        private readonly IContextManager contextManager;
        private ILogger logger;
        private DataUtils dataUtils;
        private ServiceUtilities utilities;
        private ICommonService commonService;
        private IServiceService serviceService;
        private IChannelService channelService;
        private IVersioningManager versioningManager;

        public ServiceAndChannelService(IContextManager contextManager,
                                       ITranslationEntity translationEntToVm,
                                       ITranslationViewModel translationVmtoEnt,
                                       ILogger<OrganizationService> logger,
                                       ServiceUtilities utilities,
                                       DataUtils dataUtils,
                                       ICommonService commonService,
                                       IServiceService serviceService,
                                       IChannelService channelService,
                                       IPublishingStatusCache publishingStatusCache,
                                       IVersioningManager versioningManager,
                                       IUserOrganizationChecker userOrganizationChecker)
            : base(translationEntToVm, translationVmtoEnt, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.commonService = commonService;
            this.dataUtils = dataUtils;
            this.serviceService = serviceService;
            this.channelService = channelService;
            this.versioningManager = versioningManager;
        }

        public List<IVmBase> SaveServiceAndChannels(VmRelations relations)
        {
            var result = new List<IVmBase>();
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SetTranslatorLanguage(relations);
                relations.ServiceAndChannelRelations = relations.ServiceAndChannelRelations.Distinct(new RelationComparer()).ToList();
                relations.ServiceAndChannelRelations.ForEach(i => i.ChannelRelations = i.ChannelRelations.Distinct(new RelationChannelsComparer()).ToList());
                var relationData = TranslationManagerToEntity.TranslateAll<VmServiceChannelRelation, ServiceVersioned>(relations.ServiceAndChannelRelations, unitOfWork);
                var serviceVersion = unitOfWork.TranslationCloneCache.GetFromCachedSet<ServiceVersioned>().ToDictionary(i => i.OriginalEntity.Id, i => i.ClonedEntity.Id);

                foreach (var service in relationData)
                {
                    service.ServiceServiceChannels = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceServiceChannels, query => query.ServiceVersionedId == service.Id, channel => channel.ServiceChannelId);
                    UpdateDigitalAuthorizationCollection(unitOfWork, service.ServiceServiceChannels);
                }

                unitOfWork.Save();

                var serviceCloneData = new Dictionary<Guid, string>();
                relations.ServiceAndChannelRelations.ForEach(service =>
                {
                    if (serviceVersion.ContainsKey(service.Id.Value))
                    {
                        serviceCloneData.Add(serviceVersion[service.Id.Value], service.ConnectedServiceId);
                    }
                });
                var cloneServiceIds = serviceCloneData.Select(y => y.Key).ToList();

                var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var resultTemp = unitOfWork.ApplyIncludes(serviceRep.All().Where(x => cloneServiceIds.Contains(x.Id)), q =>
                    q.Include(i => i.ServiceNames).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Versions).ThenInclude(i => i.ServiceChannelNames).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Versions).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDescriptions).ThenInclude(i => i.Type)
                    .Include(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDigitalAuthorizations).ThenInclude(i => i.DigitalAuthorization)
                    );

                var cloneServices = TranslationManagerToVm.TranslateAll<ServiceVersioned, VmServiceRelationListItem>(resultTemp).Cast<IVmServiceListItem>().ToList();

                cloneServices.ForEach(service =>
                     {
                         if (serviceCloneData.ContainsKey(service.Id))
                         {
                             result.Add(new VmConnectedService() { UiId = serviceCloneData[service.Id], Service = service });
                         }
                     });
                });
            return result;
        }

        private void UpdateDigitalAuthorizationCollection(IUnitOfWorkWritable unitOfWork, ICollection<ServiceServiceChannel> relations)
        {
            foreach (var relation in relations)
            {
                relation.ServiceServiceChannelDigitalAuthorizations = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, relation.ServiceServiceChannelDigitalAuthorizations, query => query.ServiceId == relation.ServiceVersionedId && query.ServiceChannelId == relation.ServiceChannelId, da => da.DigitalAuthorizationId);
            }
        }

        public IList<string> SaveServicesAndChannels(List<V2VmOpenApiServiceAndChannel> serviceAndChannelRelations)
        {
            var list = new List<string>();
            foreach (var service in serviceAndChannelRelations)
            {
                try
                {
                    list.Add(SaveServiceServiceChannel(service));
                }
                catch(Exception ex)
                {
                    logger.LogError(ex.Message);
                    list.Add(ex.Message);
                }
            }
            return list;
        }

        private string SaveServiceServiceChannel(V2VmOpenApiServiceAndChannel serviceServiceChannel)
        {
            Guid channelId, serviceId;
            serviceServiceChannel.ChannelGuid = Guid.TryParse(serviceServiceChannel.ServiceChannelId, out channelId) ? channelId : Guid.Empty;
            serviceServiceChannel.ServiceGuid = Guid.TryParse(serviceServiceChannel.ServiceId, out serviceId) ? serviceId : Guid.Empty;

            if (!serviceService.ServiceExists(serviceServiceChannel.ServiceGuid))
            {
                return string.Format(CoreMessages.OpenApi.EntityNotFound, "Service", serviceId);
            }

            if (!channelService.ChannelExists(serviceServiceChannel.ChannelGuid))
            {
                return string.Format(CoreMessages.OpenApi.EntityNotFound, "Service channel", channelId);
            }

            var msg = string.Empty;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Get the published version of service (or Draft) and use the version id
                var publishedService = versioningManager.GetSpecificVersionByRoot<ServiceVersioned>(unitOfWork, serviceServiceChannel.ServiceGuid, PublishingStatus.Published);
                if (publishedService == null)
                {
                    publishedService = versioningManager.GetSpecificVersionByRoot<ServiceVersioned>(unitOfWork, serviceServiceChannel.ServiceGuid, PublishingStatus.Draft);
                }
                if (publishedService == null)
                {
                    msg = string.Format(CoreMessages.OpenApi.SuitableVersionNotFound, "Service", serviceServiceChannel.ServiceGuid);
                }
                else
                {
                    serviceServiceChannel.ServiceGuid = publishedService.Id;
                    var result = TranslationManagerToEntity.Translate<V2VmOpenApiServiceAndChannel, ServiceServiceChannel>(serviceServiceChannel, unitOfWork);
                    unitOfWork.Save();
                    msg = string.Format(CoreMessages.OpenApi.ServiceServiceChannelAdded, channelId, serviceId);
                }
            });

            return msg;
        }

        public VmChannelRelation GetRelationDetail(VmGetRelationDetail model)
        {
            VmChannelRelation result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                TranslationManagerToVm.SetLanguage(model.Language);
                var relationRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();

                var resultTemp = unitOfWork.ApplyIncludes(relationRep.All().Where(x => (x.ServiceVersionedId == model.ServiceId.Value && x.ServiceChannelId == model.ChannelId.Value)), i => i
                    .Include(j => j.ServiceChannel).ThenInclude(j => j.Versions).ThenInclude(j => j.ServiceChannelNames).ThenInclude(j => j.Type)
                    .Include(j => j.ServiceChannel).ThenInclude(j => j.Versions).ThenInclude(j => j.Type)
                    .Include(j => j.ServiceServiceChannelDescriptions).ThenInclude(j => j.Type)
                    .Include(j => j.ServiceServiceChannelDigitalAuthorizations).ThenInclude(j => j.DigitalAuthorization),
                    true).FirstOrDefault();

                if (resultTemp != null)
                {
                    result = TranslationManagerToVm.Translate<ServiceServiceChannel, VmChannelRelation>(resultTemp);
                    result.Id = model.ChannelRelationId;
                }
            });

            return result;
        }

        private List<PublishingAffectedResult> PublishEntities<TEntity>(IUnitOfWorkWritable unitOfWork, List<Guid> entitiesIds) where TEntity : class, IEntityIdentifier, IVersionedVolume, new()
        {
            var entityRep = unitOfWork.CreateRepository<IRepository<TEntity>>();
            var entities = entityRep.All().Where(x => entitiesIds.Contains(x.Id)).ToList();
            return entities.Select(service => versioningManager.PublishVersion(unitOfWork, service)).SelectMany(i => i).ToList();
        }

        public VmPublishServiceAndChannelResult PublishRelations(VmPublishServiceAndChannel model)
        {
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var resultServices = PublishEntities<ServiceVersioned>(unitOfWork, model.Services);
                var resultChannels = PublishEntities<ServiceChannelVersioned>(unitOfWork, model.Channels);
                unitOfWork.Save();
                return new VmPublishServiceAndChannelResult()
                {
                    Channels = resultChannels.Select(i => new VmEntityStatusBase() {Id = i.Id, PublishingStatusId = i.PublishingStatusNew}).ToList<IVmEntityBase>(),
                    Services = resultServices.Select(i => new VmEntityStatusBase() {Id = i.Id, PublishingStatusId = i.PublishingStatusNew}).ToList<IVmEntityBase>()
                };
            });
        }

        class RelationComparer : IEqualityComparer<VmServiceChannelRelation>
        {
            public bool Equals(VmServiceChannelRelation x, VmServiceChannelRelation y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(VmServiceChannelRelation obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        class RelationChannelsComparer : IEqualityComparer<VmChannelRelation>
        {
            public bool Equals(VmChannelRelation x, VmChannelRelation y)
            {
                return x.ConnectedChannel?.RootId == y.ConnectedChannel?.RootId;
            }

            public int GetHashCode(VmChannelRelation obj)
            {
                return obj.ConnectedChannel?.RootId.GetHashCode() ?? obj.Id.GetHashCode();
            }
        }
    }
}
