/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V10;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class ServiceCollectionsByOrganizationPagingHandler : EntitiesByStatusPagingHandler<V10VmOpenApiServiceCollectionsWithPaging, V10VmOpenApiServiceCollectionItem, ServiceCollectionVersioned, ServiceCollection, ServiceCollectionName, ServiceCollectionLanguageAvailability>
    {
        private const int pageSize = 100;

        private IList<Guid> organizationIds;
        private ILanguageCache languageCache;
        private ITypesCache typesCache;
        private ITranslationEntity translationManagerToVm;
        private Dictionary<Guid, List<VmOpenApiLanguageItem>> dictNames;
        private Dictionary<Guid, List<ServiceCollectionDescription>> dictDescriptions;
        private Dictionary<Guid, List<VmOpenApiServiceCollectionService>> dictServiceCollectionServices;
        private Dictionary<Guid, List<VmOpenApiServiceCollectionChannel>> dictServiceCollectionChannels;

        public ServiceCollectionsByOrganizationPagingHandler(
            IList<Guid> organizationIds,
            IPublishingStatusCache publishingStatusCache,
            ILanguageCache languageCache,
            ITypesCache typesCache,
            ITranslationEntity translationManagerToVm,
            int pageNumber
            ) : base(EntityStatusExtendedEnum.Published, null, null, publishingStatusCache, typesCache, pageNumber, pageSize)
        {
            this.organizationIds = organizationIds;
            this.languageCache = languageCache;
            this.typesCache = typesCache;
            this.translationManagerToVm = translationManagerToVm;
        }

        protected override IList<Expression<Func<ServiceCollectionVersioned, bool>>> GetFilters(IUnitOfWork unitOfWork)
        {
            var filters = base.GetFilters(unitOfWork);
            filters.Add(s => organizationIds.Contains(s.OrganizationId)); // Main responsible organization
            return filters;
        }

        protected override void SetReturnValues(IUnitOfWork unitOfWork)
        {
            // Get related names for service collections
            dictNames = unitOfWork.CreateRepository<IRepository<ServiceCollectionName>>().All().Where(x => EntityIds.Contains(x.ServiceCollectionVersionedId)).OrderBy(i => i.Localization.OrderNumber)
               .Select(i => new { id = i.ServiceCollectionVersionedId, i.Name, i.LocalizationId }).ToList().GroupBy(i => i.id)
               .ToDictionary(i => i.Key, i => i.ToList().Select(x => new VmOpenApiLanguageItem
               {
                   Value = x.Name,
                   Language = languageCache.GetByValue(x.LocalizationId),
               }).ToList());

            // Get related descriptions for service collections
            dictDescriptions = unitOfWork.CreateRepository<IRepository<ServiceCollectionDescription>>().All()
                .Where(x => EntityIds.Contains(x.ServiceCollectionVersionedId))
                .OrderBy(i => i.Localization.OrderNumber)
                .ToList()
                .GroupBy(i => i.ServiceCollectionVersionedId)
                .ToDictionary(i => i.Key, i => i.ToList());

            // Get related services
            var serviceCollectionServices = unitOfWork.CreateRepository<IRepository<ServiceCollectionService>>().All()
                .Where(s => UnificEntityIds.Contains(s.ServiceCollectionId))
                .ToList();
            var serviceCollectionChannels = unitOfWork.CreateRepository<IServiceCollectionServiceChannelRepository>()
                .All().Where(x => UnificEntityIds.Contains(x.ServiceCollectionId))
                .ToList();

            var serviceIds = serviceCollectionServices.Select(i => i.ServiceId).Distinct().ToList();
            var channelIds = serviceCollectionChannels.Select(i => i.ServiceChannelId).Distinct().ToList();
            
            // Filter out not published language versions
            var nameTypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            var descriptionTypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString());
            var vmServices = GetServiceDetails(unitOfWork, serviceIds, nameTypeId, descriptionTypeId);
            var vmChannels = GetChannelDetails(unitOfWork, channelIds, nameTypeId, descriptionTypeId);

            var serviceCollectionDict = serviceCollectionServices
                .GroupBy(i => i.ServiceCollectionId)
                .ToDictionary(i => i.Key, i => i.Select(d => d.ServiceId).ToList());
            var channelCollectionDict = serviceCollectionChannels
                .GroupBy(i => i.ServiceCollectionId)
                .ToDictionary(i => i.Key, i => i.Select(d => d.ServiceChannelId).ToList());
            
            FillServiceDictionary(serviceCollectionDict, vmServices);
            FillChannelDictionary(channelCollectionDict, vmChannels);
        }

        private void FillChannelDictionary(Dictionary<Guid, List<Guid>> channelCollectionDict, List<VmOpenApiServiceCollectionChannel> vmChannels)
        {
            dictServiceCollectionChannels = new Dictionary<Guid, List<VmOpenApiServiceCollectionChannel>>();
            foreach (var item in channelCollectionDict)
            {
                var serviceCollectionChannelList = vmChannels.Where(i => item.Value.Contains(i.Id.Value))?.ToList();
                if (serviceCollectionChannelList != null)
                {
                    dictServiceCollectionChannels.Add(item.Key, serviceCollectionChannelList);
                }
            }
        }

        private void FillServiceDictionary(Dictionary<Guid, List<Guid>> serviceCollectionDict, List<VmOpenApiServiceCollectionService> vmServices)
        {
            dictServiceCollectionServices = new Dictionary<Guid, List<VmOpenApiServiceCollectionService>>();
            foreach (var item in serviceCollectionDict)
            {
                var serviceCollectionServiceList = vmServices.Where(i => item.Value.Contains(i.Id.Value))?.ToList();
                if (serviceCollectionServiceList != null)
                {
                    dictServiceCollectionServices.Add(item.Key, serviceCollectionServiceList);
                }
            }
        }

        private List<VmOpenApiServiceCollectionChannel> GetChannelDetails(IUnitOfWork unitOfWork, List<Guid> channelIds, Guid nameTypeId, Guid descriptionTypeId)
        {
            var channels = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>().All()
                .Where(x => channelIds.Contains(x.UnificRootId) && x.PublishingStatusId == PublishedId)
                .Include(x => x.ServiceChannelNames)
                .Include(x => x.ServiceChannelDescriptions)
                .Include(x => x.LanguageAvailabilities)
                .Include(x => x.UnificRoot)
                .Include(x => x.Type)
                .ToList();
            
            channels.ForEach(s =>
            {
                var availableLanguages = s.LanguageAvailabilities.Where(i => i.StatusId == PublishedId).Select(i => i.LanguageId).ToList();
                s.ServiceChannelNames = s.ServiceChannelNames.Where(n => n.TypeId == nameTypeId && availableLanguages.Contains(n.LocalizationId)).ToList();
                s.ServiceChannelDescriptions = s.ServiceChannelDescriptions.Where(n => n.TypeId == descriptionTypeId && availableLanguages.Contains(n.LocalizationId)).ToList();
            });
            
            return channels.Select(s => new VmOpenApiServiceCollectionChannel
            {
                Id = s.UnificRootId,
                Name = s.ServiceChannelNames.Where(n => n.ServiceChannelVersionedId == s.Id).Select(x => new VmOpenApiLocalizedListItem
                {
                    Value = x.Name,
                    Type = typesCache.GetByValue<NameType>(x.TypeId),
                    Language = languageCache.GetByValue(x.LocalizationId)
                }).ToList(),
                Description = translationManagerToVm.TranslateAll<ServiceChannelDescription, VmOpenApiLocalizedListItem>(s.ServiceChannelDescriptions.Where(d => d.ServiceChannelVersionedId == s.Id))?.ToList(),
                ServiceChannelType = s.Type.Code
            }).ToList();
        }

        private List<VmOpenApiServiceCollectionService> GetServiceDetails(IUnitOfWork unitOfWork, List<Guid> serviceIds, Guid nameTypeId, Guid descriptionTypeId)
        {
            var services = unitOfWork.ApplyIncludes(unitOfWork.CreateRepository<IRepository<ServiceVersioned>>().All()
                        .Where(s => serviceIds.Contains(s.UnificRootId) && s.PublishingStatusId == PublishedId), 
                    q => q.Include(h => h.ServiceNames)
                        .Include(h => h.ServiceDescriptions)
                        .Include(h => h.LanguageAvailabilities)
                        .Include(h => h.UnificRoot))
                .ToList();
            
            services.ForEach(s =>
            {
                var availableLanguages = s.LanguageAvailabilities.Where(i => i.StatusId == PublishedId).Select(i => i.LanguageId).ToList();
                s.ServiceNames = s.ServiceNames.Where(n => n.TypeId == nameTypeId && availableLanguages.Contains(n.LocalizationId)).ToList();
                s.ServiceDescriptions = s.ServiceDescriptions.Where(n => n.TypeId == descriptionTypeId && availableLanguages.Contains(n.LocalizationId)).ToList();
            });
            
            return services.Select(s => new VmOpenApiServiceCollectionService
            {
                Id = s.UnificRootId,
                Name = s.ServiceNames.Where(n => n.ServiceVersionedId == s.Id).Select(x => new VmOpenApiLocalizedListItem
                {
                    Value = x.Name,
                    Type = typesCache.GetByValue<NameType>(x.TypeId),
                    Language = languageCache.GetByValue(x.LocalizationId)
                }).ToList(),
                Description = translationManagerToVm.TranslateAll<ServiceDescription, VmOpenApiLocalizedListItem>(s.ServiceDescriptions.Where(d => d.ServiceVersionedId == s.Id))?.ToList()
            }).ToList();
        }

        protected override V10VmOpenApiServiceCollectionItem GetItemData(ServiceCollectionVersioned entity)
        {
            return new V10VmOpenApiServiceCollectionItem
            {
                Id = entity.UnificRootId,
                ServiceCollectionNames = dictNames?.TryGetOrDefault(entity.Id, null),
                ServiceCollectionDescriptions = translationManagerToVm.TranslateAll<ServiceCollectionDescription, VmOpenApiLocalizedListItem>(dictDescriptions?.TryGetOrDefault(entity.Id, new List<ServiceCollectionDescription>()))?.ToList(),
                Services = dictServiceCollectionServices?.TryGetOrDefault(entity.UnificRootId, null),
                ServiceChannels = dictServiceCollectionChannels?.TryGetOrDefault(entity.UnificRootId, null)
            };
        }
    }
}
