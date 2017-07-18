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
using PTV.Domain.Model.Enums.Security;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V5;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V6;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof (IServiceAndChannelService), RegisterType.Transient)]
    internal class ServiceAndChannelService : ServiceBase, IServiceAndChannelService
    {
        private readonly IContextManager contextManager;
        private ILogger logger;
        private DataUtils dataUtils;
        private ServiceUtilities utilities;
        private IServiceService serviceService;
        private IChannelService channelService;
        private IVersioningManager versioningManager;
        private readonly ITypesCache typesCache;

        public ServiceAndChannelService(IContextManager contextManager,
                                       ITranslationEntity translationEntToVm,
                                       ITranslationViewModel translationVmtoEnt,
                                       ILogger<OrganizationService> logger,
                                       ServiceUtilities utilities,
                                       DataUtils dataUtils,
                                       IServiceService serviceService,
                                       IChannelService channelService,
                                       IPublishingStatusCache publishingStatusCache,
                                       IVersioningManager versioningManager,
                                       IUserOrganizationChecker userOrganizationChecker,
                                       ICacheManager cacheManager)
            : base(translationEntToVm, translationVmtoEnt, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.dataUtils = dataUtils;
            this.serviceService = serviceService;
            this.channelService = channelService;
            this.versioningManager = versioningManager;
            typesCache = cacheManager.TypesCache;
        }

        public List<IVmBase> SaveServiceAndChannels(VmRelations relations)
        {
            var result = new List<IVmBase>();

            Dictionary<Guid, Guid> serviceVersion = new Dictionary<Guid, Guid>();

            contextManager.ExecuteWriter(unitOfWork =>
            {
                IQueryable<Guid> supportedChannelIds = null;
                if (utilities.GetUserRole() != UserRoleEnum.Eeva)
                {
                    var schvRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                    var userOrg = utilities.GetUserOrganization(unitOfWork) ?? Guid.Empty;
                    var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                    var psCommonForAll = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.CommonForAll.ToString());
                    var channelIds = relations.ServiceAndChannelRelations.SelectMany(r => r.ChannelRelations).Select(ch => ch.ConnectedChannel.Id);
                    var channels = schvRep.All().Where(i => channelIds.Contains(i.Id));

                    var supportedChannels = channels.Where(ch => userOrg != Guid.Empty &&
                                                                 (ch.OrganizationId == userOrg ||
                                                                  (userOrg != ch.OrganizationId &&
                                                                   (ch.PublishingStatusId == psPublished &&
                                                                    ch.ConnectionTypeId == psCommonForAll))));

                    supportedChannelIds = supportedChannels.Select(ch => ch.UnificRootId);
                }

                SetTranslatorLanguage(relations);
                relations.ServiceAndChannelRelations = relations.ServiceAndChannelRelations.Distinct(new RelationComparer()).ToList();
                relations.ServiceAndChannelRelations.ForEach(i => i.ChannelRelations = i.ChannelRelations.Distinct(new RelationChannelsComparer()).ToList());
					
				var serviceVersionedRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
                var serviceVersionedIds = relations.ServiceAndChannelRelations.Select(sv => sv.Id).ToList();
                var unificRootIds = serviceVersionedRep.All().Where(sv => serviceVersionedIds.Contains(sv.Id)).ToDictionary(k => k.Id, v => v.UnificRootId);
                foreach (var item in relations.ServiceAndChannelRelations)
                {
                    if (!item.Id.IsAssigned() || !unificRootIds.ContainsKey(item.Id.Value)) continue;
                    item.UnificRootId = unificRootIds[item.Id.Value];
                    item.ChannelRelations.ForEach(chs => chs.Service = unificRootIds[item.Id.Value]);
                }
				
				
                var relationData = TranslationManagerToEntity.TranslateAll<VmServiceChannelRelation, Service>(relations.ServiceAndChannelRelations, unitOfWork);
                serviceVersion = unitOfWork.TranslationCloneCache.GetFromCachedSet<ServiceVersioned>().ToDictionary(i => i.OriginalEntity.Id, i => i.ClonedEntity.Id);

                foreach (var service in relationData)
                {
                    var supportedServiceServiceChannels = supportedChannelIds == null
						? service.ServiceServiceChannels
                        : service.ServiceServiceChannels.Where(ch => supportedChannelIds.Contains(ch.ServiceChannelId)).ToList();

                    service.ServiceServiceChannels = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, supportedServiceServiceChannels, query => query.ServiceId == service.Id, channel => channel.ServiceChannelId);
                    UpdateDigitalAuthorizationCollection(unitOfWork, service.ServiceServiceChannels);
                }

                unitOfWork.Save();
            });

            contextManager.ExecuteReader(unitOfWork =>
            { 
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
                    .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Versions).ThenInclude(i => i.ServiceChannelNames).ThenInclude(i => i.Type)
                    .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceChannel).ThenInclude(i => i.Versions).ThenInclude(i => i.Type)
                    .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDescriptions).ThenInclude(i => i.Type)
                    .Include(i => i.UnificRoot).ThenInclude(i => i.ServiceServiceChannels).ThenInclude(i => i.ServiceServiceChannelDigitalAuthorizations).ThenInclude(i => i.DigitalAuthorization)
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
                relation.ServiceServiceChannelDigitalAuthorizations = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, relation.ServiceServiceChannelDigitalAuthorizations, query => query.ServiceId == relation.ServiceId && query.ServiceChannelId == relation.ServiceChannelId, da => da.DigitalAuthorizationId);
            }
        }

        public IList<string> SaveServicesAndChannels(List<V2VmOpenApiServiceAndChannel> serviceAndChannelRelations, Guid? userOrganizationId)
        {
            var list = new List<string>();
            foreach (var service in serviceAndChannelRelations)
            {
                try
                {
                    list.Add(SaveServiceServiceChannel(service, userOrganizationId));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    list.Add(ex.Message);
                }
            }
            return list;
        }

        private string SaveServiceServiceChannel(V2VmOpenApiServiceAndChannel serviceServiceChannel, Guid? userOrganizationId)
        {
            if (!serviceServiceChannel.ChannelGuid.IsAssigned())
            {
                Guid channelId, serviceId;
                serviceServiceChannel.ChannelGuid = Guid.TryParse(serviceServiceChannel.ServiceChannelId, out channelId) ? channelId : Guid.Empty;
                serviceServiceChannel.ServiceGuid = Guid.TryParse(serviceServiceChannel.ServiceId, out serviceId) ? serviceId : Guid.Empty;
            }

            var currentVersion = serviceService.GetServiceById(serviceServiceChannel.ServiceGuid, 0, false);
            if (currentVersion == null || string.IsNullOrEmpty(currentVersion.PublishingStatus))
            {
                return string.Format(CoreMessages.OpenApi.EntityNotFound, "Service", serviceServiceChannel.ServiceGuid);
            }
            else if (currentVersion.PublishingStatus != PublishingStatus.Draft.ToString() && currentVersion.PublishingStatus != PublishingStatus.Published.ToString())
            {
                return $"Publishing status for service '{serviceServiceChannel.ServiceGuid}' is {currentVersion.PublishingStatus}. You cannot update service!";
            }

            var channel = channelService.GetServiceChannelById(serviceServiceChannel.ChannelGuid, 0);
            if (channel == null || !channel.Id.IsAssigned())
            {
                return string.Format(CoreMessages.OpenApi.EntityNotFound, "Service channel", serviceServiceChannel.ChannelGuid);
            }

            var msg = string.Empty;
            contextManager.ExecuteWriter(unitOfWork =>
            {
                if (!channel.IsVisibleForAll)
                {
                    if (!userOrganizationId.IsAssigned())
                    {
                        msg = string.Format(CoreMessages.OpenApi.ChannelNotVisibleForAll, serviceServiceChannel.ChannelGuid);
                    }
                    else
                    {
                        // Get the user organization tree and check the channel organization against the user organization tree (including the sub organizations). PTV-2299                        
                        var allUserOrganizations = GetOrganizationRootIdsFlatten(unitOfWork, userOrganizationId.Value);
                        
                        // Has user rights to add the relationship? In other word does the organization related to given channel belong to the user organization tree?
                        if (!allUserOrganizations.Contains(channel.OrganizationId))
                        {
                            msg = string.Format(CoreMessages.OpenApi.ChannelNotVisibleForAll, serviceServiceChannel.ChannelGuid);
                        }
                    }                    
                }
                if (string.IsNullOrEmpty(msg))
                {
                    var result = TranslationManagerToEntity.Translate<V2VmOpenApiServiceAndChannel, ServiceServiceChannel>(serviceServiceChannel, unitOfWork);
                    unitOfWork.Save();
                    msg = string.Format(CoreMessages.OpenApi.ServiceServiceChannelAdded, serviceServiceChannel.ChannelGuid, serviceServiceChannel.ServiceGuid);
                }                
            });

            return msg;
        }

        public IVmOpenApiServiceVersionBase SaveServiceAndChannels(V5VmOpenApiServiceAndChannelRelationInBase relations, int openApiVersion)
        {
            Service service;
            var rootID = relations.ServiceId.Value;
            try
            {
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    relations.ChannelRelations.ForEach(r => r.ServiceGuid = relations.ServiceId.Value);

                    service = TranslationManagerToEntity.Translate<V5VmOpenApiServiceAndChannelRelationInBase, Service>(relations, unitOfWork);
                    service.ServiceServiceChannels = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceServiceChannels, query => query.ServiceId == service.Id, channel => channel.ServiceChannelId);
                    UpdateDigitalAuthorizationCollection(unitOfWork, service.ServiceServiceChannels);

                    // Manually remove items from channel description collections
                    if (relations.ChannelRelations?.Count > 0)
                    {
                        service.ServiceServiceChannels.ForEach(relation => 
                        {
                            var updatedDescriptions = relation.ServiceServiceChannelDescriptions.ToList();
                            var rep = unitOfWork.CreateRepository<IServiceServiceChannelDescriptionRepository>();
                            var currentDescriptions = rep.All().Where(d => d.ServiceId == relation.ServiceId && d.ServiceChannelId == relation.ServiceChannelId).ToList();
                            var toRemove = currentDescriptions.Where(d => updatedDescriptions.FirstOrDefault(ud => ud.LocalizationId == d.LocalizationId && ud.TypeId == d.TypeId) == null).ToList();
                            toRemove.ForEach(d => rep.Remove(d));
                        });
                    }
                    
                    unitOfWork.Save();
                });
            }
            catch(Exception ex)
            {
                var errorMsg = $"Error occured while updating relations for a service with id {rootID}. {ex.Message}";
                logger.LogError(errorMsg + " " + ex.StackTrace);
                throw new Exception(errorMsg);
            }

            return serviceService.GetServiceById(rootID, openApiVersion, false);
        }

        public IList<string> SaveServicesAndChannelsBySource(List<VmOpenApiServiceServiceChannelBySource> serviceAndChannelRelations, Guid? userOrganizationId)
        {
            var list = new List<string>();
            var userId = utilities.GetRelationIdForExternalSource();
            foreach (var relation in serviceAndChannelRelations)
            {
                Guid? serviceId = null;
                Guid? serviceChannelId = null;
                    
                try
                {
                    contextManager.ExecuteReader(unitOfWork =>
                    {
                        try
                        {
                            serviceId = GetPTVId<Service>(relation.ServiceSourceId, userId, unitOfWork);
                            if (!serviceId.IsAssigned()) { list.Add(string.Format(CoreMessages.OpenApi.EntityNotFound, "Service", relation.ServiceSourceId)); }
                            serviceChannelId = GetPTVId<ServiceChannel>(relation.ServiceChannelSourceId, userId, unitOfWork);
                            if (!serviceChannelId.IsAssigned()) { list.Add(string.Format(CoreMessages.OpenApi.EntityNotFound, "ServiceChannel", relation.ServiceChannelSourceId)); }
                        }
                        catch(Exception ex)
                        {
                            list.Add(ex.Message);
                        }
                    });
                    if (serviceId.IsAssigned() && serviceChannelId.IsAssigned())
                    {
                        list.Add(SaveServiceServiceChannel(new V2VmOpenApiServiceAndChannel
                        {
                            ServiceGuid = serviceId.Value,
                            ChannelGuid = serviceChannelId.Value,
                            Description = relation.Description,
                            ServiceChargeType = relation.ServiceChargeType
                        }, userOrganizationId));
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    list.Add(ex.Message);
                }
            }
            return list;
        }


        public IVmOpenApiServiceVersionBase SaveServicesAndChannelsBySource(string serviceSourceId, V6VmOpenApiServiceAndChannelRelationBySourceInBase relationsBySource, Guid? userOrganizationId, int openApiVersion)
        {
            var errors = new List<string>();
            var userId = utilities.GetRelationIdForExternalSource();
            var relations = new V5VmOpenApiServiceAndChannelRelationInBase() { DeleteAllChannelRelations = relationsBySource.DeleteAllChannelRelations };
            contextManager.ExecuteReader(unitOfWork =>
            {
                try
                {
                    relations.ServiceId = GetPTVId<Service>(serviceSourceId, userId, unitOfWork);
                    foreach (var channel in relationsBySource.ChannelRelations)
                    {
                        var channelId = GetPTVId<ServiceChannel>(channel.ServiceChannelSourceId, userId, unitOfWork);
                        if (channelId.IsAssigned())
                        {
                            relations.ChannelRelations.Add(new V5VmOpenApiServiceServiceChannelInBase
                            {
                                ChannelGuid = channelId,
                                Description = channel.Description,
                                ServiceChargeType = channel.ServiceChargeType
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                }
            });
            if (errors.Count > 0)
            {
                throw new Exception(String.Join(", ", errors));
            }

            return SaveServiceAndChannels(relations, openApiVersion);
        }

        public VmChannelRelation GetRelationDetail(VmGetRelationDetail model)
        {
            VmChannelRelation result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                TranslationManagerToVm.SetLanguage(model.Language);
                var relationRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
                var resultTemp = unitOfWork.ApplyIncludes(relationRep.All().Where(x => (x.ServiceId == model.ServiceId.Value && x.ServiceChannelId == model.ChannelId.Value)), i => i
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
