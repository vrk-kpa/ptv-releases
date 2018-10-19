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
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Domain.Model.Models.V2.Common.Connections;

namespace PTV.Database.DataAccess.Services.V2
{
    [Framework.RegisterService(typeof(IConnectionsService), RegisterType.Transient)]
    [Framework.RegisterService(typeof(IConnectionsServiceInternal), RegisterType.Transient)]
    internal class ConnectionsService : ServiceBase, IConnectionsService, IConnectionsServiceInternal
    {
        private readonly IContextManager contextManager;
        private ILanguageOrderCache languageOrderCache;
        private IVersioningManager versioningManager;

        public ConnectionsService(
           IContextManager contextManager,
           ITranslationEntity translationManagerToVm,
           ITranslationViewModel translationManagerToEntity,
           IPublishingStatusCache publishingStatusCache,
           IUserOrganizationChecker userOrganizationChecker,
           IVersioningManager versioningManager,
           ICacheManager cacheManager
            ) :
            base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker)
        {
            this.contextManager = contextManager;
            this.languageOrderCache = cacheManager.LanguageOrderCache;
            this.versioningManager = versioningManager;
        }


        public VmConnectionsPageOutput SaveRelations(VmConnectionsPageInput model)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SaveRelations(unitOfWork, model);
                unitOfWork.Save();
            });
            return GetRelations(model);
        }

        private void SaveRelations(IUnitOfWorkWritable unitOfWork, VmConnectionsPageInput model)
        {
            var connections = model.Connections.Select(connection =>
            {
                return new VmConnectionsInput
                {
                    Id = connection.MainEntity.Id,
                    UnificRootId = connection.MainEntity.UnificRootId,
                    SelectedConnections = connection.Childs
                };
            });
            switch (model.MainEntityType)
            {
                case DomainEnum.Services:
                    TranslationManagerToEntity.TranslateAll<VmConnectionsInput, Service>(connections, unitOfWork);
                    break;
                case DomainEnum.Channels:
                    TranslationManagerToEntity.TranslateAll<VmConnectionsInput, ServiceChannel>(connections, unitOfWork);
                    break;
                default:
                    break;
            }
        }

        private VmConnectionsPageOutput GetRelations(VmConnectionsPageInput model)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                return GetRelations(unitOfWork, model);
            });
        }
        private IQueryable<ServiceServiceChannel> IncludeAdditionalInformation(IQueryable<ServiceServiceChannel> query)
        {
            return query                
                .Include(j => j.ServiceServiceChannelDescriptions)
                .Include(j => j.ServiceServiceChannelDigitalAuthorizations)
                    .ThenInclude(j => j.DigitalAuthorization)
                .Include(j => j.ServiceServiceChannelEmails)
                    .ThenInclude(j => j.Email)
                // Attachments //
                .Include(j => j.ServiceServiceChannelWebPages)
                    .ThenInclude(j => j.WebPage)
                // FaxNumbers & PhoneNumbers //
                .Include(j => j.ServiceServiceChannelPhones)
                    .ThenInclude(j => j.Phone)
                    .ThenInclude(j => j.PrefixNumber)
                    .ThenInclude(j => j.Country)
                    .ThenInclude(j => j.CountryNames)
                // PostalAddresses //
                .Include(j => j.ServiceServiceChannelAddresses)
                    .ThenInclude(j => j.Address)
                    .ThenInclude(j => j.AddressAdditionalInformations)
                .Include(j => j.ServiceServiceChannelAddresses)
                    .ThenInclude(j => j.Address)
                    .ThenInclude(j => j.Country)
                    .ThenInclude(j => j.CountryNames)
                .Include(j => j.ServiceServiceChannelAddresses)
                    .ThenInclude(j => j.Address)
                    .ThenInclude(j => j.AddressForeigns)
                    .ThenInclude(j => j.ForeignTextNames)
                .Include(j => j.ServiceServiceChannelAddresses)
                    .ThenInclude(j => j.Address)
                    .ThenInclude(j => j.AddressPostOfficeBoxes)
                    .ThenInclude(j => j.PostOfficeBoxNames)
                .Include(j => j.ServiceServiceChannelAddresses)
                    .ThenInclude(j => j.Address)
                    .ThenInclude(j => j.AddressPostOfficeBoxes)
                    .ThenInclude(j => j.PostalCode)
                    .ThenInclude(j => j.PostalCodeNames)
                .Include(j => j.ServiceServiceChannelAddresses)
                    .ThenInclude(j => j.Address)
                    .ThenInclude(j => j.AddressStreets)
                    .ThenInclude(j => j.StreetNames)
                .Include(j => j.ServiceServiceChannelAddresses)
                    .ThenInclude(j => j.Address)
                    .ThenInclude(j => j.AddressStreets)
                    .ThenInclude(j => j.PostalCode)
                    .ThenInclude(j => j.PostalCodeNames)
                .Include(j => j.ServiceServiceChannelServiceHours)
                    .ThenInclude(j => j.ServiceHours)
                    .ThenInclude(j => j.DailyOpeningTimes)
                .Include(j => j.ServiceServiceChannelServiceHours)
                    .ThenInclude(j => j.ServiceHours)
                    .ThenInclude(j => j.AdditionalInformations)
                .Include(j => j.ServiceServiceChannelExtraTypes)
                    .ThenInclude(j => j.ServiceServiceChannelExtraTypeDescriptions);
        }
        private VmConnectionsPageOutput GetRelations(IUnitOfWork unitOfWork, VmConnectionsPageInput model)
        {
            var result = new VmConnectionsPageOutput();
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            if (model.MainEntityType == DomainEnum.Channels)
            {
                var serviceChannelIds = model.Connections.Select(i => i.MainEntity.Id).ToList();
                var channelLangAvailabilitiesRep = unitOfWork.CreateRepository<IServiceChannelLanguageAvailabilityRepository>();
                var channelLangAvailabilities = channelLangAvailabilitiesRep.All().Where(x => serviceChannelIds.Contains(x.ServiceChannelVersionedId)).ToList()
                    .GroupBy(i => i.ServiceChannelVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());

                result.Channels = new List<VmChannelConnectionsOutput>();

                var serviceChannelsUnificRootIds = model.Connections.Select(i => i.MainEntity.UnificRootId).ToList();
                var serviceChannels = GetAllServiceChannelRelations(unitOfWork, serviceChannelsUnificRootIds);

                foreach (var connection in model.Connections)
                {
                   var connectionResult = new VmChannelConnectionsOutput
                    {
                        Id = connection.MainEntity.Id,
                        Connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, VmChannelConnectionOutput>(
                            serviceChannels.TryGetOrDefault(connection.MainEntity.UnificRootId, new List<ServiceServiceChannel>()).OrderBy(x => x.OrderNumber)).ToList(),
                        LanguagesAvailabilities = TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            channelLangAvailabilities.TryGetOrDefault(connection.MainEntity.Id, new List<ServiceChannelLanguageAvailability>())),
                    };
                    result.Channels.Add(connectionResult);
                }
            }
            if (model.MainEntityType == DomainEnum.Services)
            {               
                result.Services = new List<VmConnectionsOutput>();

                var serviceIds = model.Connections.Select(i => i.MainEntity.Id).ToList();
                var serviceLangAvailabilitiesRep = unitOfWork.CreateRepository<IServiceLanguageAvailabilityRepository>();
                var serviceLangAvailabilities = serviceLangAvailabilitiesRep.All().Where(x => serviceIds.Contains(x.ServiceVersionedId)).ToList()
                    .GroupBy(i => i.ServiceVersionedId).ToDictionary(i => i.Key, i => i.OrderBy(x => languageOrderCache.Get(x.LanguageId)).ToList());

                var serviceUnificRootIds = model.Connections.Select(i => i.MainEntity.UnificRootId).ToList();

                var serviceChannels = GetAllServiceRelations(unitOfWork, serviceUnificRootIds);

                foreach (var connection in model.Connections)
                {
                    var connectionResult = new VmConnectionsOutput
                    {
                        Id = connection.MainEntity.Id,
                        Connections = TranslationManagerToVm.TranslateAll<ServiceServiceChannel, VmConnectionOutput>(
                            serviceChannels.TryGetOrDefault(connection.MainEntity.UnificRootId, new List<ServiceServiceChannel>()).OrderBy(x => x.OrderNumber)).ToList(),
                        LanguagesAvailabilities = TranslationManagerToVm.TranslateAll<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                            serviceLangAvailabilities.TryGetOrDefault(connection.MainEntity.Id, new List<ServiceLanguageAvailability>()))
                    };
                    result.Services.Add(connectionResult);
                }
            }
            return result;
        }

        public Dictionary<Guid, List<ServiceServiceChannel>> GetAllServiceRelations(IUnitOfWork unitOfWork, List<Guid> unificRootIds)
        {
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            return IncludeAdditionalInformation(serviceChannelRep.All()).Where(x => unificRootIds.Contains(x.ServiceId))
                                   .Include(j => j.ServiceChannel)
                                        .ThenInclude(j => j.Versions)
                                        .ThenInclude(j => j.ServiceChannelNames)
                                    .Include(j => j.ServiceChannel)
                                        .ThenInclude(j => j.Versions)
                                        .ThenInclude(j => j.LanguageAvailabilities)
                                   .ToList()
                                   .GroupBy(i => i.ServiceId).ToDictionary(i => i.Key, i => i.ToList());
        }
        public Dictionary<Guid, List<ServiceServiceChannel>> GetAllServiceChannelRelations(IUnitOfWork unitOfWork, List<Guid> unificRootIds)
        {
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            return IncludeAdditionalInformation(serviceChannelRep.All())
                                    .Include(j => j.Service)
                                        .ThenInclude(j => j.Versions)
                                        .ThenInclude(j => j.ServiceNames)
                                    .Include(j => j.Service)
                                        .ThenInclude(j => j.Versions)
                                        .ThenInclude(j => j.LanguageAvailabilities)
                                    .Include(j => j.Service)
                                        .ThenInclude(j => j.Versions)
                                        .ThenInclude(j => j.StatutoryServiceGeneralDescription)
                                        .ThenInclude(j => j.Versions)
                                   .Where(x => unificRootIds.Contains(x.ServiceChannelId)).ToList()
                                   .GroupBy(i => i.ServiceChannelId).ToDictionary(i => i.Key, i => i.ToList());
        }

        public List<Guid> GetServiceChannelRelationIds(IUnitOfWork unitOfWork, Guid unificRootId)
        {
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var rootIds = serviceChannelRep.All()
                      .Where(x => x.ServiceChannelId == unificRootId)
                      .Select(x => x.ServiceId).ToList();
            return rootIds.Select(x => versioningManager.GetLastPublishedModifiedDraftVersion<ServiceVersioned>(unitOfWork, x).EntityId).ToList();            
        }
    }
}
