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
using PTV.Domain.Model.Enums;

using Microsoft.Extensions.Logging;
using PTV.Domain.Model.Models.OpenApi;
using System.Linq.Expressions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.DirectRaw;
using PTV.Framework.Exceptions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Database.DataAccess.Utils.OpenApi;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework.ServiceManager;
using PTV.Framework.Extensions;
using PTV.Domain.Model.Models.OpenApi.V11;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IChannelService), RegisterType.Transient)]
    internal class ChannelService : ServiceBase, IChannelService
    {
        private IContextManager contextManager;
        private ILogger logger;
        private IServiceUtilities utilities;
        private ICommonServiceInternal commonService;
        private IAddressService addressService;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private ILanguageOrderCache languageOrderCache;
        private readonly IPostalCodeCache postalCodeCache;
        private readonly PTV.Database.DataAccess.Interfaces.Services.V2.IAccessibilityRegisterService accessibilityRegisterService;
        private readonly IUrlService urlService;
        private readonly IExpirationService expirationService;
        private readonly IDatabaseRawContext rawContext;
        private readonly ITextManager textManager;

        public ChannelService(
            IContextManager contextManager,
            ITranslationEntity translationManagerToVm,
            ITranslationViewModel translationManagerToEntity,
            ILogger<ChannelService> logger,
            IServiceUtilities utilities,
            ICommonServiceInternal commonService,
            IAddressService addressService,
            ICacheManager cacheManager,
            IPublishingStatusCache publishingStatusCache,
            IVersioningManager versioningManager,
            IUserOrganizationChecker userOrganizationChecker,
            ILanguageOrderCache languageOrderCache,
            PTV.Database.DataAccess.Interfaces.Services.V2.IAccessibilityRegisterService accessibilityRegisterService,
            IUrlService urlService,
            IExpirationService expirationService,
            IDatabaseRawContext rawContext,
            ITextManager textManager) :
            base(translationManagerToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker,
                versioningManager)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.commonService = commonService;
            this.addressService = addressService;
            this.typesCache = cacheManager.TypesCache;
            this.languageCache = cacheManager.LanguageCache;
            this.languageOrderCache = languageOrderCache;
            this.accessibilityRegisterService = accessibilityRegisterService;
            this.postalCodeCache = cacheManager.PostalCodeCache;
            this.urlService = urlService;
            this.expirationService = expirationService;
            this.rawContext = rawContext;
            this.textManager = textManager;
        }


        #region OpenApi

        public IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServiceChannels(DateTime? date, int pageNumber, int pageSize, EntityStatusExtendedEnum status = EntityStatusExtendedEnum.Published, DateTime? dateBefore = null, IList<Guid> organizationIds = null, bool isVisibleForAll = false)
        {
            var handler = new ServiceChannelsByOrganizationPagingHandler(isVisibleForAll, organizationIds, status, null, date, dateBefore, PublishingStatusCache, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        public IVmOpenApiModelWithPagingBase<VmOpenApiExpiringTask> GetTaskServiceChannels(int pageNumber, int pageSize, List<Guid> entityIds, int expirationMonths, List<Guid> publishingStatusIds)
        {
            var handler = new ExpiringTasksPagingHandler<ServiceChannelVersioned, ServiceChannelName, ServiceChannelLanguageAvailability>(entityIds, publishingStatusIds, TranslationManagerToVm, expirationMonths, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }
        
        public IVmOpenApiModelWithPagingBase<VmOpenApiNotUpdatedTask> GetNotUpdatedServiceChannels(int pageNumber, int pageSize, List<Guid> entityIds, List<Guid> publishingStatusIds)
        {
            var handler = new NotUpdatedTasksPagingHandler<ServiceChannelVersioned, ServiceChannelName, ServiceChannelLanguageAvailability>(entityIds, publishingStatusIds, TranslationManagerToVm, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        public IVmOpenApiModelWithPagingBase<VmOpenApiTask> GetTaskServiceChannels(int pageNumber, int pageSize, List<Guid> entityIds, List<Guid> publishingStatusIds)
        {
            var handler = new TasksPagingHandler<ServiceChannelVersioned, ServiceChannelName, ServiceChannelLanguageAvailability>(entityIds, publishingStatusIds, TranslationManagerToVm, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        private IVmOpenApiModelWithPagingBase<IVmOpenApiServiceChannel> GetPageWithAllData(IEntitiesWithAllDataPagingHandler<IVmOpenApiServiceChannel> pageHandler, int openApiVersion, bool showHeader)
        {
            if (pageHandler.PageNumber <= 0) return pageHandler.GetModel();

            IVmOpenApiModelWithPagingBase<IVmOpenApiServiceChannel> model = null;
            contextManager.ExecuteReader(unitOfWork =>
                {
                    var totalCount = pageHandler.Search(unitOfWork);
                    model = pageHandler.GetModel();
                    if (totalCount > 0 && pageHandler.EntityVersionIds?.Count > 0)
                    {
                        model.ItemList = GetServiceChannelsWithDetails(unitOfWork, pageHandler.EntityVersionIds, openApiVersion, showHeader);
                    }
                });

            return model;
        }

        private List<Guid> GetArchivedChannelIds(V11VmOpenApiGetArchivedServiceChannels parameters)
        {
            var sql = @"            
            -- Group rows by UnificRootId and rank them by VersionMajor. Row where rank = 1 is the latest

            WITH ranked AS (
                SELECT 
                sv.""Id"", 
                sv.""UnificRootId"", 
                sv.""PublishingStatusId"", 
                sv.""OrganizationId"", 
                sv.""Modified"", 
                sv.""ModifiedBy"", 
                rank() OVER (PARTITION BY v.""UnificRootId"" ORDER BY v.""VersionMajor"" desc) rank
                FROM ""ServiceChannelVersioned"" sv
                LEFT JOIN ""Versioning"" v ON sv.""VersioningId"" = v.""Id""
                WHERE sv.""OrganizationId"" = @organizationId
                ORDER BY sv.""UnificRootId""
            )
            
            -- Take only those rows which are lates (rank = 1) and status = Archived gives us
            -- latest archived row per service

            SELECT ranked.""Id""
            FROM ranked
            LEFT JOIN ""PublishingStatusType"" pt ON ranked.""PublishingStatusId"" = pt.""Id""

            /**where**/
            
            order by ranked.""UnificRootId"" limit @limit offset @offset";

            var builder = new SqlBuilder();
            var template = builder.AddTemplate(sql, new
            {
                organizationId = parameters.OrganizationId, 
                limit = parameters.Take, 
                offset = parameters.Skip
            });

            builder.Where("rank = 1");

            var publishStatuses = new List<string> {PublishingStatus.Deleted.ToString(), PublishingStatus.OldPublished.ToString()};
            builder.Where(@"pt.""Code"" = ANY(@publishStatuses)", new {publishStatuses});

            if (parameters.MinArchivingDate.HasValue)
            {
                builder.Where(@"ranked.""Modified"" > @minDate", new {minDate = parameters.MinArchivingDate.Value});
            }

            if (parameters.MaxArchivingDate.HasValue)
            {
                builder.Where(@"ranked.""Modified"" < @maxDate", new {maxDate = parameters.MaxArchivingDate.Value});
            }

            builder.Where(
                parameters.ArchivingType == ArchivingType.Automatic
                    ? @"lower(ranked.""ModifiedBy"") = @modifiedBy"
                    : @"lower(ranked.""ModifiedBy"") <> @modifiedBy", new {modifiedBy = CoreConstants.PtvAppUserName.ToLowerInvariant()});

            return rawContext.ExecuteReader(db => db.SelectList<Guid>(template.RawSql, template.Parameters).ToList());
        }

        public IList<VmOpenApiArchivedServiceChannelBase> GetArchivedChannels(V11VmOpenApiGetArchivedServiceChannels parameters, int openApiVersion)
        {
            var channelIds = GetArchivedChannelIds(parameters);
            if (!channelIds.Any())
            {
                return new List<VmOpenApiArchivedServiceChannelBase>();
            }

            return contextManager.ExecuteReader(unitOfWork =>
            {
                var repo = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();

                var query = repo.AllPure()
                    .Include(x => x.ServiceChannelNames).ThenInclude(a => a.Localization)
                    .Include(x => x.Type)
                    .Where(e => channelIds.Contains(e.Id))
                    .OrderBy(x => x.UnificRootId);

                var channels = query.ToList();

                TranslationManagerToVm.SetValue(openApiVersion, false);
                return TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiArchivedServiceChannelBase>(channels).ToList();
            });
        }

        public IList<IVmOpenApiServiceChannel> GetServiceChannels(List<Guid> idList, int openApiVersion, bool showHeader)
        {
            if (idList.IsNullOrEmpty())
            {
                return null;
            }

            IList<Expression<Func<ServiceChannelVersioned, bool>>> filters = new List<Expression<Func<ServiceChannelVersioned, bool>>>
            {
                c => idList.Contains(c.UnificRootId)
            };

            try
            {
                return contextManager.ExecuteReader(unitOfWork => GetServiceChannelsWithDetails(unitOfWork, filters, openApiVersion, showHeader));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occured while getting service channels");
                throw;
            }
        }

        public IVmOpenApiModelWithPagingBase<IVmOpenApiServiceChannel> GetServiceChannelsWithAllDataByOrganization(IList<Guid> organizationIds, int openApiVersion, int pageNumber, bool showHeader)
        {
            var handler = new ServiceChannelsWithAllDataByOrganizationPagingHandler(organizationIds, PublishingStatusCache, pageNumber);
            return GetPageWithAllData(handler, openApiVersion, showHeader);
        }

        public IVmOpenApiModelWithPagingBase<IVmOpenApiServiceChannel> GetServiceChannelsWithAllDataByMunicipality(Guid municipalityId, bool includeWholeCountry, int openApiVersion, int pageNumber, bool showHeader)
        {
            var handler = new ServiceChannelsWithAllDataByMunicipalityPagingHandler(municipalityId, includeWholeCountry, typesCache, PublishingStatusCache, pageNumber);
            return GetPageWithAllData(handler, openApiVersion, showHeader);
        }

        public IVmOpenApiModelWithPagingBase<IVmOpenApiServiceChannel> GetServiceChannelsWithAllDataByArea(Guid areaId, bool includeWholeCountry, int openApiVersion, int pageNumber, bool showHeader)
        {
            var handler = new ServiceChannelsWithAllDataByAreaPagingHandler(areaId, includeWholeCountry, typesCache, PublishingStatusCache, pageNumber);
            return GetPageWithAllData(handler, openApiVersion, showHeader);
        }

        public IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServiceChannelsByMunicipality(Guid municipalityId, bool includeWholeCountry, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null)
        {
            var handler = new ServiceChannelsByMunicipalityPagingHandler(municipalityId, includeWholeCountry, date, dateBefore, PublishingStatusCache, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        public IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServiceChannelsByArea(Guid areaId, bool includeWholeCountry, DateTime? date, int pageNumber, int pageSize, DateTime? dateBefore = null)
        {
            var handler = new ServiceChannelsByAreaPagingHandler(areaId, includeWholeCountry, typesCache, date, dateBefore, PublishingStatusCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        public IVmOpenApiServiceChannel GetServiceChannelById(Guid id, int openApiVersion, VersionStatusEnum status, bool showHeader = false)
        {
            return contextManager.ExecuteReader(unitOfWork => GetServiceChannelById(unitOfWork, id, openApiVersion, status, showHeader));
        }

        public IVmOpenApiServiceChannel GetServiceChannelByIdSimple(Guid id, bool getOnlyPublished = true)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                try
                {
                    var entityId = getOnlyPublished ? VersioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, PublishingStatus.Published)
                                                : VersioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, null, false);

                    if (entityId.IsAssigned())
                    {
                        return GetServiceChannelWithSimpleDetails(unitOfWork, entityId.Value);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error occured while getting a channel with id {id}");
                    throw;
                }

                return null;
            });
        }

        public PublishingStatus? GetLatestVersionPublishingStatus(Guid id)
        {
            return contextManager.ExecuteReader(unitOfWork => VersioningManager.GetLatestVersionPublishingStatus<ServiceChannelVersioned>(unitOfWork, id));
        }

        private IVmOpenApiServiceChannel GetServiceChannelById(IUnitOfWork unitOfWork, Guid id, int openApiVersion, VersionStatusEnum status, bool showHeader = false)
        {
            try
            {
                Guid? entityId = null;
                switch (status)
                {
                    case VersionStatusEnum.Published:
                        entityId = VersioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, PublishingStatus.Published);
                        break;
                    case VersionStatusEnum.Latest:
                        entityId = VersioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, null, false);
                        break;
                    case VersionStatusEnum.LatestActive:
                        entityId = VersioningManager.GetVersionId<ServiceChannelVersioned>(unitOfWork, id, null, true);
                        break;
                    default:
                        break;
                }
                return (entityId.IsAssigned()) ? GetServiceChannelWithDetails(unitOfWork, entityId.Value, openApiVersion, showHeader,status == VersionStatusEnum.Published) : null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occured while getting a channel with id {id}");
                throw;
            }
        }

        public IVmOpenApiServiceChannel GetServiceChannelBySource(string sourceId)
        {
            var userId = utilities.GetRelationIdForExternalSource();
            try
            {
                var rootId = contextManager.ExecuteReader(unitOfWork => GetPTVId<ServiceChannel>(sourceId, userId, unitOfWork));
                return rootId.IsAssigned() ? GetServiceChannelByIdSimple(rootId, false) : null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occured while getting services by source id {sourceId}");
                throw;
            }
        }

        public IList<IVmOpenApiServiceChannel> GetServiceChannelsWithDetailsByType(ServiceChannelTypeEnum type, DateTime? date, int openApiVersion, bool showHeader)
        {
            var typeId = typesCache.Get<ServiceChannelType>(type.ToString());
            IList<Expression<Func<ServiceChannelVersioned, bool>>> filters = new List<Expression<Func<ServiceChannelVersioned, bool>>>
            {
                c => c.TypeId.Equals(typeId)
            };
            if (date.HasValue)
            {
                filters.Add(c => c.Modified > date.Value);
            }

            try
            {
                return contextManager.ExecuteReader(unitOfWork => GetServiceChannelsWithDetails(unitOfWork, filters, openApiVersion, showHeader));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occured while getting service channels of type {type}");
                throw;
            }
        }

        public IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServiceChannelsByType(ServiceChannelTypeEnum type, DateTime? date, int pageNumber, int pageSize, bool getOnlyPublished = true, DateTime? dateBefore = null)
        {
            var typeId = typesCache.Get<ServiceChannelType>(type.ToString());

            var handler = new ServiceChannelsByTypePagingHandler(typeId, getOnlyPublished ? EntityStatusExtendedEnum.Published : EntityStatusExtendedEnum.Active, date, dateBefore, PublishingStatusCache, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        public IList<IVmOpenApiServiceChannel> GetServiceChannelsByOrganization(Guid organizationId, DateTime? date, int openApiVersion, bool showHeader,  ServiceChannelTypeEnum? type = null)
        {
            IList<Expression<Func<ServiceChannelVersioned, bool>>> filters = new List<Expression<Func<ServiceChannelVersioned, bool>>>
            {
                c => c.OrganizationId.Equals(organizationId)
            };
            if (date.HasValue)
            {
                filters.Add(c => c.Modified > date.Value);
            }
            if (type.HasValue)
            {
                var typeId = typesCache.Get<ServiceChannelType>(type.Value.ToString());
                // Filter by service channel type
                filters.Add(c => c.TypeId == typeId);
            }
            try
            {
                return contextManager.ExecuteReader(unitOfWork => GetServiceChannelsWithDetails(unitOfWork, filters, openApiVersion, showHeader));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occured while getting service channels for organization {organizationId}");
                throw;
            }
        }

        public IVmOpenApiModelWithPagingBase<VmOpenApiItem> GetServiceChannelsByOrganization(Guid organizationId, DateTime? date, int pageNumber, int pageSize, ServiceChannelTypeEnum? type = null, DateTime? dateBefore = null)
        {
            Guid? typeId = null;
            if (type.HasValue)
            {
                typeId = typesCache.Get<ServiceChannelType>(type.Value.ToString());
            }
            var handler = new ServiceChannelsByOrganizationPagingHandler(false, new List<Guid> { organizationId }, EntityStatusExtendedEnum.Published, typeId, date, dateBefore, PublishingStatusCache, typesCache, pageNumber, pageSize);
            return GetPage(contextManager, handler);
        }

        public VmOpenApiConnectionChannels CheckChannels(List<Guid> idList, List<Guid> userOrganizations = null)
        {
            if (idList == null || idList?.Count == 0) { return null; }

            var result = new VmOpenApiConnectionChannels();
            var serviceLocationChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString());
            contextManager.ExecuteReader(unitOfWork =>
            {
                var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
                var query = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>().All().Where(s => idList.Contains(s.UnificRootId) && s.PublishingStatusId == publishedId);
                if (userOrganizations.HasData())
                {
                    var visibleForAll = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.CommonForAll.ToString());
                    query = query.Where(s => s.ConnectionTypeId == visibleForAll || userOrganizations.Contains(s.OrganizationId));
                }
                var existingChannels = query.Select(s => new { Id = s.UnificRootId, TypeId = s.TypeId }).ToList();
                var existingChannelsIds = existingChannels.Select(i => i.Id).ToList();
                result.NotExistingChannels = idList.Where(i => !existingChannelsIds.Contains(i)).ToList();
                result.ServiceLocationChannels = existingChannels.Where(i => i.TypeId == serviceLocationChannelTypeId)?.Select(i => i.Id).ToList();
            });
            return result;
        }

        private IVmOpenApiServiceChannel GetServiceChannelWithDetails(IUnitOfWork unitOfWork, Guid versionId, int openApiVersion, bool showHeader, bool getOnlyPublished = true)
        {
            IVmOpenApiServiceChannel result = null;
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var query = serviceChannelRep.All().Where(s => s.Id == versionId);
            if (getOnlyPublished)
            {
                query = query.Where(c => c.LanguageAvailabilities.Any(l => l.StatusId == publishedId));
            }

            var channel = unitOfWork.ApplyIncludes(query, GetServiceChannelIncludeChain()).FirstOrDefault();

            if (channel != null)
            {
                var list = new List<ServiceChannelVersioned> { channel };
                IncludeCommonDetails(unitOfWork, list);
                IncludeAddressesAndAreas(unitOfWork, list);
                
                TranslationManagerToVm.SetValue(openApiVersion, showHeader);

                if (channel.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.EChannel.ToString()))
                {
                    IncludeEChannelDetails(unitOfWork, channel);
                    IncludeServiceHours(unitOfWork, list);

                    // Filter out not published language versions
                    FilterOutNotPublishedLanguageVersions(channel, publishedId, getOnlyPublished);

                    result = TranslationManagerToVm.Translate<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(channel);
                }
                else if (channel.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.Phone.ToString()))
                {
                    IncludeServiceHours(unitOfWork, list);

                    // Filter out not published language versions
                    FilterOutNotPublishedLanguageVersions(channel, publishedId, getOnlyPublished);

                    result = TranslationManagerToVm.Translate<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(channel);
                }
                else if (channel.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString()))
                {
                    IncludeServiceHours(unitOfWork, list);
                    IncludeAccessibilityRegisters(unitOfWork, channel);
                    IncludeSocialHealthCenters(unitOfWork, channel);

                    // Filter out not published language versions
                    FilterOutNotPublishedLanguageVersions(channel, publishedId, getOnlyPublished);

                    // Set accessibility sentences. PTV-2481 & PTV-4237
                    if (channel.Addresses.HasData() && channel.UnificRoot?.AccessibilityRegisters.HasData() == true)
                    {
                        channel.Addresses.ForEach(a =>
                        {
                            if (a.Address.UniqueId != Guid.Empty)
                            {
                                var ar = channel.UnificRoot?.AccessibilityRegisters.FirstOrDefault(r => r.Address.UniqueId == a.Address.UniqueId);
                                if (ar != null)
                                {
                                    a.Address.AccessibilityRegisterEntrances = ar.Entrances;
                                }
                            }
                        });
                    }
                    result = TranslationManagerToVm.Translate<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(channel);
                }
                else if (channel.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.PrintableForm.ToString()))
                {
                    IncludePrintableFormDetails(unitOfWork, channel);

                    // Filter out not published language versions
                    FilterOutNotPublishedLanguageVersions(channel, publishedId, getOnlyPublished);

                    result = TranslationManagerToVm.Translate<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(channel);
                }
                else if (channel.TypeId == typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.WebPage.ToString()))
                {
                    IncludeWebPageDetails(unitOfWork, channel);

                    // Filter out not published language versions
                    FilterOutNotPublishedLanguageVersions(channel, publishedId, getOnlyPublished);

                    result = TranslationManagerToVm.Translate<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(channel);
                }
            }
            if (result == null)
            {
                return null;
            }
            // Find only published services for a service channel - let's do this outside of translator
            // Manually map connection data.
            result.Services = GetChannelServices(new List<Guid> { channel.UnificRootId }, unitOfWork);
            result.ServiceCollections = GetServiceCollections(new List<Guid> {channel.UnificRootId}, unitOfWork);
            result.OntologyTerms = GetAllRelatedOntologyTerms(channel.UnificRootId, unitOfWork);
            //if area not set by user set area default by areas of connected published services
            if (channel.AreaInformationTypeId == null && result.Services != null)
            {
                AddInheritedAreaInformation(result.Services.Select(x => x.Service.Id).OfType<Guid>().ToList(), result, unitOfWork);
            }

            return GetServiceChannelByOpenApiVersion(unitOfWork, result, openApiVersion);
        }

        private void AddInheritedAreaInformation(List<Guid> connectedServiceRootIds, IVmOpenApiServiceChannel result, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            var connectedPublishedServiceIds = new List<Guid>();
            foreach (var connectedServiceRootId in connectedServiceRootIds)
            {
                var serviceVersionedId = VersioningManager.GetVersionId<ServiceVersioned>(unitOfWork, connectedServiceRootId, PublishingStatus.Published);
                if (serviceVersionedId.IsAssigned())
                {
                    connectedPublishedServiceIds.Add(serviceVersionedId.Value);
                }
            }

            if (connectedPublishedServiceIds.Any())
            {
                GetAllVmInheritedAreaInformation(connectedPublishedServiceIds, result, unitOfWork);
            }
        }

        private void GetAllVmInheritedAreaInformation(List<Guid> connectedPublishedServiceIds, IVmOpenApiServiceChannel result, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var connectedServiceWithAreas =
                serviceRep.All().Where(s => connectedPublishedServiceIds.Contains(s.Id))
                    .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaNames)
                    .Include(i => i.Areas).ThenInclude(i => i.Area).ThenInclude(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                    .Include(i => i.AreaMunicipalities).ThenInclude(i => i.Municipality).ThenInclude(i => i.MunicipalityNames)
                    .ToList();

            var wholeCountryId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
            var wholeCountryExceptAlandId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString());
            var areaTypeId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.AreaType.ToString());

            if (connectedServiceWithAreas.Any(x => x.AreaInformationTypeId == areaTypeId))
            {
                var areaMunicipalities = connectedServiceWithAreas.SelectMany(x => x.AreaMunicipalities).Select(y => y.Municipality).DistinctBy(z => z.Id).ToList();
                result.AreaMunicipalities = TranslationManagerToVm.TranslateAll<Municipality, VmOpenApiMunicipality>(areaMunicipalities).ToList();

                var areas = connectedServiceWithAreas.SelectMany(x => x.Areas).Select(y => y.Area).DistinctBy(z => z.Id).ToList();
                result.Areas = TranslationManagerToVm.TranslateAll<Area, VmOpenApiArea>(areas).ToList();
                result.AreaType = AreaInformationTypeEnum.AreaType.ToString().GetOpenApiEnumValue<AreaInformationTypeEnum>();
            }
            else if (connectedServiceWithAreas.Any(x => x.AreaInformationTypeId == wholeCountryExceptAlandId))
            {
                result.AreaType = AreaInformationTypeEnum.WholeCountryExceptAlandIslands.ToString().GetOpenApiEnumValue<AreaInformationTypeEnum>();
            }
            else if (connectedServiceWithAreas.Any(x => x.AreaInformationTypeId == wholeCountryId))
            {
                result.AreaType =  AreaInformationTypeEnum.WholeCountry.ToString().GetOpenApiEnumValue<AreaInformationTypeEnum>();
            }
        }

        private Func<IQueryable<ServiceChannelVersioned>, IQueryable<ServiceChannelVersioned>> GetServiceChannelIncludeChain()
        {
            return q =>
                q.Include(i => i.ServiceChannelNames)
                .Include(i => i.DisplayNameTypes)
                .Include(i => i.ServiceChannelDescriptions)
                .Include(i => i.Type)
                .Include(i => i.Attachments).ThenInclude(i => i.Attachment)
                .Include(i => i.Languages).ThenInclude(i => i.Language)
                .Include(i => i.LanguageAvailabilities)
                .Include(i => i.AreaMunicipalities)
                .Include(i => i.UnificRoot)
                .Include(i => i.AccessibilityClassifications).ThenInclude(i => i.AccessibilityClassification);
        }

        private void IncludeEChannelDetails(IUnitOfWork unitOfWork, ServiceChannelVersioned entity)
        {
            if (unitOfWork == null || entity == null)
            {
                return;
            }

            var channelRepo = unitOfWork.CreateRepository<IElectronicChannelRepository>();

            var channels = channelRepo.All()
                .Include(x => x.LocalizedUrls).ThenInclude(x => x.WebPage)
                .Where(i => i.ServiceChannelVersionedId == entity.Id)
                .ToList();

            entity.ElectronicChannels = channels;
        }

        private void IncludePrintableFormDetails(IUnitOfWork unitOfWork, ServiceChannelVersioned entity)
        {
            if (unitOfWork == null || entity == null)
            {
                return;
            }

            var channelRepo = unitOfWork.CreateRepository<IPrintableFormChannelRepository>();

            var channels = channelRepo.All()
                .Include(x => x.ChannelUrls).ThenInclude(x => x.WebPage)
                .Include(x => x.FormIdentifiers)
                .Where(i => i.ServiceChannelVersionedId == entity.Id)
                .ToList();

            entity.PrintableFormChannels = channels;
        }

        private void IncludeWebPageDetails(IUnitOfWork unitOfWork, ServiceChannelVersioned entity)
        {
            if (unitOfWork == null || entity == null)
            {
                return;
            }

            var channelRepo = unitOfWork.CreateRepository<IWebpageChannelRepository>();

            var channels = channelRepo.All()
                .Include(x => x.LocalizedUrls).ThenInclude(x => x.WebPage)
                .Where(i => i.ServiceChannelVersionedId == entity.Id)
                .ToList();

            entity.WebpageChannels = channels;
        }

        private void IncludeCommonDetails(IUnitOfWork unitOfWork, IList<ServiceChannelVersioned> entities)
        {
            if (unitOfWork == null || entities == null || entities.Count == 0)
            {
                return;
            }

            var ids = entities.Select(e => e.Id).ToList();

            var channelEmailRepo = unitOfWork.CreateRepository<IServiceChannelEmailRepository>();
            var channelPhoneRepo = unitOfWork.CreateRepository<IServiceChannelPhoneRepository>();
            var channelWebRepo = unitOfWork.CreateRepository<IServiceChannelWebPageRepository>();

            var emails = channelEmailRepo.All()
                .Include(x => x.Email)
                .Where(i => ids.Contains(i.ServiceChannelVersionedId))
                .ToList();
            var phones = channelPhoneRepo.All()
                .Include(x => x.Phone).ThenInclude(x => x.PrefixNumber).ThenInclude(x => x.Country)
                .ThenInclude(x => x.CountryNames)
                .Where(i => ids.Contains(i.ServiceChannelVersionedId))
                .ToList();
            var webPages = channelWebRepo.All()
                .Include(x => x.WebPage)
                .Where(x => ids.Contains(x.ServiceChannelVersionedId))
                .ToList();

            entities.ForEach(entity =>
            {
                entity.Emails = emails.Where(a => a.ServiceChannelVersionedId == entity.Id).ToList();
                entity.Phones = phones.Where(a => a.ServiceChannelVersionedId == entity.Id).ToList();
                entity.WebPages = webPages.Where(a => a.ServiceChannelVersionedId == entity.Id).ToList();
            });

        }

        private void IncludeAddressesAndAreas(IUnitOfWork unitOfWork, IList<ServiceChannelVersioned> entities)
        {
            if (unitOfWork == null || entities == null || entities.Count == 0)
            {
                return;
            }

            var channelAddressRepo = unitOfWork.CreateRepository<IServiceChannelAddressRepository>();
            var addressRepo =  unitOfWork.CreateRepository<IAddressRepository>();
            var clsAddressPointRepo =  unitOfWork.CreateRepository<IClsAddressPointRepository>();
            var addressPostOfficeBoxRepo = unitOfWork.CreateRepository<IAddressPostOfficeBoxRepository>();
            var addressForeignRepo = unitOfWork.CreateRepository<IAddressForeignRepository>();
            var addressOtherRepo = unitOfWork.CreateRepository<IAddressOtherRepository>();
            var countryRepo = unitOfWork.CreateRepository<ICountryRepository>();
            var addressAdditionalInformationRepo = unitOfWork.CreateRepository<IAddressAdditionalInformationRepository>();
            var addressCoordinateRepo = unitOfWork.CreateRepository<IAddressCoordinateRepository>();
            var addressReceiverRepo = unitOfWork.CreateRepository<IAddressReceiverRepository>();
            var clsAddressStreetRepo = unitOfWork.CreateRepository<IClsAddressStreetRepository>();
            var clsAddressStreetNumberRepo = unitOfWork.CreateRepository<IClsAddressStreetNumberRepository>();
            
            var ids = entities.Select(e => e.Id).ToList();
            var addresses = channelAddressRepo.All()
                .Where(x => ids.Contains(x.ServiceChannelVersionedId))
                .ToList();
            
            addresses.ForEach(address =>
            {
                address.Address = addressRepo.All().FirstOrDefault(x => x.Id == address.AddressId);
                if (address.Address != null)
                {
                    var clsAddressPoints = clsAddressPointRepo.All()
                        .Include(i => i.AddressStreetNumber)
                        .Include(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                        .Where(y => y.AddressId == address.AddressId)
                        .ToList();
                    
                    clsAddressPoints.ForEach(clsAddressPoint =>
                    {
                        var clsAddressStreet = clsAddressStreetRepo.All()
                            .Include(i => i.StreetNames)
                            .FirstOrDefault(x => x.Id == clsAddressPoint.AddressStreetId);

                        if (clsAddressStreet != null)
                        {
                            clsAddressStreet.StreetNumbers = clsAddressStreetNumberRepo.All()
                                .Include(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                                .Where(x => x.ClsAddressStreetId == clsAddressStreet.Id)
                                .ToList();
                            
                            clsAddressPoint.AddressStreet = clsAddressStreet;
                        }
                    });
                    
                    address.Address.ClsAddressPoints = clsAddressPoints;
                    
                    var addressPostOfficeBoxes = addressPostOfficeBoxRepo.All()
                        .Include(i => i.PostOfficeBoxNames)
                        .Include(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                        .Where(y => y.AddressId == address.AddressId)
                        .ToList();
                    address.Address.AddressPostOfficeBoxes = addressPostOfficeBoxes;

                    var addressForeigns = addressForeignRepo.All()
                        .Include(i => i.ForeignTextNames)
                        .Where(y => y.AddressId == address.AddressId)
                        .ToList();
                    address.Address.AddressForeigns = addressForeigns;

                    var addressOthers = addressOtherRepo.All()
                        .Include(i => i.PostalCode).ThenInclude(i => i.PostalCodeNames)
                        .Where(y => y.AddressId == address.AddressId)
                        .ToList();
                    address.Address.AddressOthers = addressOthers;

                    var country = countryRepo.All()
                        .Include(i => i.CountryNames)
                        .FirstOrDefault(y => y.Id == address.Address.CountryId);
                    address.Address.Country = country;
                    
                    var addressAdditionalInformations = addressAdditionalInformationRepo.All()
                        .Where(y => y.AddressId == address.AddressId)
                        .ToList();
                    address.Address.AddressAdditionalInformations = addressAdditionalInformations;

                    var coordinates = addressCoordinateRepo.All()
                        .Where(y => y.RelatedToId == address.AddressId)
                        .ToList();
                    address.Address.Coordinates = coordinates;
                    
                    var receivers = addressReceiverRepo.All()
                        .Where(y => y.AddressId == address.AddressId)
                        .ToList();
                    address.Address.Receivers = receivers;

                    address.Address.ClsAddressPoints.ForEach(x => x.Address = address.Address);
                    address.Address.AddressPostOfficeBoxes.ForEach(x => x.Address = address.Address);
                    address.Address.AddressForeigns.ForEach(x => x.Address = address.Address);
                    address.Address.AddressOthers.ForEach(x => x.Address = address.Address);
                }
            });

            var channelAreasRepo = unitOfWork.CreateRepository<IServiceChannelAreaRepository>();
            var channelAreas = channelAreasRepo.All()
                .Include(i => i.Area).ThenInclude(i => i.AreaNames)
                .Include(i => i.Area).ThenInclude(i => i.AreaMunicipalities)
                .Where(i => ids.Contains(i.ServiceChannelVersionedId))
                .ToList();

            // Get the related municipalities
            var allMunicipalityIds = addresses.Select(a => a.Address).SelectMany(a => a.ClsAddressPoints).Select(m => m.MunicipalityId).Distinct().ToList();
            var postofficeMunicipalities = addresses.Select(a => a.Address).SelectMany(a => a.AddressPostOfficeBoxes).Where(p => p.MunicipalityId != null).Select(m => m.MunicipalityId.Value).Distinct().ToList();
            allMunicipalityIds.AddRange(postofficeMunicipalities.Except(allMunicipalityIds));
            var areaMunicipalities = channelAreas.Select(a => a.Area).SelectMany(a => a.AreaMunicipalities).Select(m => m.MunicipalityId).Distinct().ToList();
            allMunicipalityIds.AddRange(areaMunicipalities.Except(allMunicipalityIds));
            var entityAreaMunicipalities = entities.SelectMany(e => e.AreaMunicipalities).Select(m => m.MunicipalityId).Distinct().ToList();
            allMunicipalityIds.AddRange(entityAreaMunicipalities.Except(allMunicipalityIds));

            var municipalityRepo = unitOfWork.CreateRepository<IMunicipalityRepository>();
            var municipalities = municipalityRepo.All()
                .Include(m => m.MunicipalityNames)
                .Where(x => allMunicipalityIds.Contains(x.Id))
                .ToList();

            addresses.ForEach(a =>
            {
                a.Address.ClsAddressPoints?.ForEach(p => p.Municipality = municipalities.FirstOrDefault(m => m.Id == p.MunicipalityId));
                a.Address.AddressPostOfficeBoxes?.ForEach(p => p.Municipality = municipalities.FirstOrDefault(m => m.Id == p.MunicipalityId));
            });
            channelAreas.ForEach(a => a.Area.AreaMunicipalities?.ForEach(am =>
            {
                if (am.Municipality == null)
                {
                    am.Municipality = municipalities.FirstOrDefault(m => m.Id == am.MunicipalityId);
                }
            }));

            entities.ForEach(entity =>
            {
                entity.Addresses = addresses.Where(a => a.ServiceChannelVersionedId == entity.Id).ToList();
                entity.Areas = channelAreas.Where(a => a.ServiceChannelVersionedId == entity.Id).ToList();
                entity.AreaMunicipalities.ForEach(o =>
                {
                    if (o.Municipality == null)
                    {
                        o.Municipality = municipalities.FirstOrDefault(m => m.Id == o.MunicipalityId);
                    }
                });
            });
        }

        private void IncludeAccessibilityRegisters(IUnitOfWork unitOfWork, ServiceChannelVersioned entity)
        {
            if (unitOfWork == null || entity == null)
            {
                return;
            }

            var arRepo = unitOfWork.CreateRepository<IAccessibilityRegisterRepository>();
            var accessibilityRegisters = arRepo.All()
                .Include(i => i.Address)
                .Include(i => i.Entrances).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
                .Include(i => i.Entrances).ThenInclude(i => i.Names)
                .Include(i => i.Entrances).ThenInclude(i => i.SentenceGroups).ThenInclude(i => i.Values)
                .Include(i => i.Entrances).ThenInclude(i => i.SentenceGroups).ThenInclude(i => i.Sentences).ThenInclude(i => i.Values)
                .Where(x => x.ServiceChannelId == entity.UnificRootId)
                .ToList();

            entity.UnificRoot.AccessibilityRegisters = accessibilityRegisters;
        }

        private void IncludeServiceHours(IUnitOfWork unitOfWork, IList<ServiceChannelVersioned> entities)
        {
            if (unitOfWork == null || entities == null || entities.Count == 0)
            {
                return;
            }

            var ids = entities.Select(e => e.Id).ToList();

            var serviceHoursRepo = unitOfWork.CreateRepository<IServiceChannelServiceHoursRepository>();

            var serviceHours = serviceHoursRepo.All()
                .Include(i => i.ServiceHours).ThenInclude(i => i.DailyOpeningTimes)
                .Include(i => i.ServiceHours).ThenInclude(i => i.AdditionalInformations)
                .Include(i => i.ServiceHours).ThenInclude(i => i.HolidayServiceHour).ThenInclude(i => i.Holiday).ThenInclude(i => i.Names)
                .Include(i => i.ServiceHours).ThenInclude(i => i.HolidayServiceHour).ThenInclude(i => i.Holiday).ThenInclude(i => i.HolidayDates)
                .Where(x => ids.Contains(x.ServiceChannelVersionedId))
                .ToList();

            entities.ForEach(entity => entity.ServiceChannelServiceHours = serviceHours.Where(a => a.ServiceChannelVersionedId == entity.Id).ToList());
        }

        private void IncludeSocialHealthCenters(IUnitOfWork unitOfWork, ServiceChannelVersioned entity)
        {
            if (unitOfWork == null || entity == null)
            {
                return;
            }

            var socialHealthCenterRepo = unitOfWork.CreateRepository<IServiceChannelSocialHealthCenterRepository>();
            var healthCenters = socialHealthCenterRepo.All()
                .Where(x => x.ServiceChannelId == entity.UnificRootId)
                .ToList();

            entity.UnificRoot.SocialHealthCenters = healthCenters;
        }

        private void FilterOutNotPublishedLanguageVersions(ServiceChannelVersioned channel, Guid publishedId, bool getOnlyPublished)
        {
            // Filter out not published language versions
            if (getOnlyPublished)
            {
                // Filter out not published language versions
                var notPublishedLanguageVersions = channel.LanguageAvailabilities.Where(i => i.StatusId != publishedId).Select(i => i.LanguageId).ToList();
                if (notPublishedLanguageVersions.HasData())
                {
                    channel.ServiceChannelNames = channel.ServiceChannelNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    channel.ServiceChannelDescriptions = channel.ServiceChannelDescriptions.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    channel.WebPages = channel.WebPages.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    if (channel.ServiceChannelServiceHours.HasData())
                    {
                        var hours = channel.ServiceChannelServiceHours.Select(i => i.ServiceHours).ToList();
                        hours.ForEach(hour =>
                        {
                            hour.AdditionalInformations = hour.AdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        });
                    }

                    channel.Attachments = channel.Attachments.Where(i => !notPublishedLanguageVersions.Contains(i.Attachment.LocalizationId)).ToList();
                    channel.Emails = channel.Emails.Where(i => !notPublishedLanguageVersions.Contains(i.Email.LocalizationId)).ToList();
                    channel.Phones = channel.Phones.Where(i => !notPublishedLanguageVersions.Contains(i.Phone.LocalizationId)).ToList();

                    // Electronic channel
                    channel.ElectronicChannels.ForEach(c =>
                    {
                        c.LocalizedUrls = c.LocalizedUrls.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    });

                    // Web page channel
                    channel.WebpageChannels.ForEach(c =>
                    {
                        c.LocalizedUrls = c.LocalizedUrls.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    });

                    // Printable form channel
                    channel.PrintableFormChannels.ForEach(c =>
                    {
                        c.FormIdentifiers = c.FormIdentifiers.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        c.ChannelUrls = c.ChannelUrls.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                    });

                    // channel addresses
                    // (printable channels and service location channels)
                    channel.Addresses.ForEach(address =>
                    {
                        address.Address.AddressPostOfficeBoxes.ForEach(j =>
                        {
                            j.PostOfficeBoxNames = j.PostOfficeBoxNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        });
                        address.Address.AddressForeigns.ForEach(j =>
                        {
                            j.ForeignTextNames = j.ForeignTextNames.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        });
                        address.Address.AddressAdditionalInformations = address.Address.AddressAdditionalInformations.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                        address.Address.AccessibilityRegisterEntrances.ForEach(e =>
                        {
                            e.Names = e.Names.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                            e.SentenceGroups.ForEach(g =>
                            {
                                g.Values = g.Values.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList();
                                g.Sentences.ForEach(s => s.Values = s.Values.Where(i => !notPublishedLanguageVersions.Contains(i.LocalizationId)).ToList());
                            });
                        });
                    });
                }
            }
        }

        private IVmOpenApiServiceChannel GetServiceChannelByOpenApiVersion(IUnitOfWork unitOfWork, IVmOpenApiServiceChannel baseVersion, int openApiVersion)
        {
            // Get the sourceId if user is logged in
            var userId = utilities.GetRelationIdForExternalSource(false);
            if (!string.IsNullOrEmpty(userId))
            {
                baseVersion.SourceId = GetSourceId<ServiceChannel>(baseVersion.Id.Value, userId, unitOfWork);
            }
            return GetEntityByOpenApiVersion(baseVersion, openApiVersion);
        }

        private IVmOpenApiServiceChannel GetServiceChannelWithDetails(Guid versionId, int openApiVersion, bool showHeader, bool getOnlyPublished = true)
        {
            IVmOpenApiServiceChannel result = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                result = GetServiceChannelWithDetails(unitOfWork, versionId, openApiVersion, showHeader, getOnlyPublished);
            });
            return result;
        }

        private IList<IVmOpenApiServiceChannel> GetServiceChannelsWithDetails(IUnitOfWork unitOfWork, IList<Expression<Func<ServiceChannelVersioned, bool>>> filters, int openApiVersion, bool showHeader)
        {
            var resultList = new List<IVmOpenApiServiceChannel>();
            var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            // Get only published items - filter out items that do not have any language versions published.
            var query = serviceChannelRep.All().Where(e => e.PublishingStatusId == publishedId && e.LanguageAvailabilities.Any(l => l.StatusId == publishedId));
            filters.ForEach(a => query = query.Where(a));

            var totalCount = query.Count();
            if (totalCount > 100 && openApiVersion > 8) // The amount checking should only be done for version 8+ (SFIPTV-568).
            {
                throw new Exception(CoreMessages.OpenApi.TooManyItems);
            }
            if (totalCount == 0)
            {
                return null;
            }
            var serviceChannels = unitOfWork.ApplyIncludes(query, GetServiceChannelIncludeChain()).ToList();
            IncludeCommonDetails(unitOfWork, serviceChannels);
            IncludeAddressesAndAreas(unitOfWork, serviceChannels);
            IncludeServiceHours(unitOfWork, serviceChannels);

            return GetServiceChannelsWithDetails(unitOfWork, serviceChannels, openApiVersion, showHeader);
        }

        private IList<IVmOpenApiServiceChannel> GetServiceChannelsWithDetails(IUnitOfWork unitOfWork, IList<Guid> versionIds, int openApiVersion, bool showHeader)
        {
            if (versionIds == null || versionIds.Count == 0)
            {
                return null;
            }

            var query = unitOfWork.CreateRepository<IRepository<ServiceChannelVersioned>>().All()
                .Where(e => versionIds.Contains(e.Id));

            var serviceChannels = unitOfWork.ApplyIncludes(query, GetServiceChannelIncludeChain()).ToList();
            IncludeCommonDetails(unitOfWork, serviceChannels);
            IncludeAddressesAndAreas(unitOfWork, serviceChannels);
            IncludeServiceHours(unitOfWork, serviceChannels);

            return GetServiceChannelsWithDetails(unitOfWork, serviceChannels, openApiVersion, showHeader);
        }

        private IList<IVmOpenApiServiceChannel> GetServiceChannelsWithDetails(IUnitOfWork unitOfWork, IList<ServiceChannelVersioned> serviceChannels, int openApiVersion, bool showHeader)
        {
            var resultList = new List<IVmOpenApiServiceChannel>();
            List<V11VmOpenApiServiceChannelService> allConnections = null;
            List<VmOpenApiServiceServiceCollection> allCollections = null;

            if (serviceChannels.HasData())
            {
                var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
                TranslationManagerToVm.SetValue(openApiVersion, showHeader);

                // E channels
                var eChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.EChannel.ToString());
                var eChannels = serviceChannels.Where(s => s.TypeId == eChannelTypeId).ToList();
                if (eChannels.HasData())
                {
                    var eChannelIds = eChannels.Select(i => i.Id).ToList();
                    var eChannelQuery = unitOfWork.CreateRepository<IElectronicChannelRepository>().All().Where(c => eChannelIds.Contains(c.ServiceChannelVersionedId));
                    var eChannelsWithData = unitOfWork.ApplyIncludes(eChannelQuery, q =>
                        q.Include(i => i.LocalizedUrls).ThenInclude(i => i.WebPage)).ToList();
                    eChannelsWithData.ForEach(e =>
                    {
                        var channel = eChannels.FirstOrDefault(i => i.Id == e.ServiceChannelVersionedId);
                        if (channel != null)
                        {
                            channel.ElectronicChannels.Add(e);
                        }
                    });
                    eChannels.ForEach(channel => FilterOutNotPublishedLanguageVersions(channel, publishedId, true));
                    resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiElectronicChannelVersionBase>(eChannels));
                }

                // Phone channels
                var phoneChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.Phone.ToString());
                var phoneChannels = serviceChannels.Where(s => s.TypeId == phoneChannelTypeId).ToList();
                if (phoneChannels.HasData())
                {
                    phoneChannels.ForEach(channel => FilterOutNotPublishedLanguageVersions(channel, publishedId, true));
                    resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiPhoneChannelVersionBase>(phoneChannels));
                }

                // Service location channels
                var serviceLocationChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString());
                var locationChannels = serviceChannels.Where(s => s.TypeId == serviceLocationChannelTypeId).ToList();
                if (locationChannels.HasData())
                {
                    // Get accessibility application sentences
                    var slChannelIds = locationChannels.Select(l => l.UnificRootId).ToList();
                    var arQuery = unitOfWork.CreateRepository<IAccessibilityRegisterRepository>().All()
                        .Where(ar => slChannelIds.Contains(ar.ServiceChannelId));
                    var registers = unitOfWork.ApplyIncludes(arQuery, q =>
                        q.Include(i => i.Address)
                         .Include(i => i.Entrances).ThenInclude(i => i.Names)
                         .Include(i => i.Entrances).ThenInclude(i => i.Address).ThenInclude(i => i.Coordinates)
                         .Include(i => i.Entrances).ThenInclude(i => i.SentenceGroups).ThenInclude(i => i.Values)
                         .Include(i => i.Entrances).ThenInclude(i => i.SentenceGroups).ThenInclude(i => i.Sentences).ThenInclude(i => i.Values)
                        ).ToList();
                    locationChannels.ForEach(channel =>
                    {
                        // Map the entrances
                        channel.Addresses.ForEach(a =>
                        {
                            var ar = registers.FirstOrDefault(r => r.Address.UniqueId == a.Address.UniqueId);
                            if (ar != null)
                            {
                                a.Address.AccessibilityRegisterEntrances = ar.Entrances;
                            }
                        });
                        FilterOutNotPublishedLanguageVersions(channel, publishedId, true);
                    });

                    resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiServiceLocationChannelVersionBase>(locationChannels));
                }

                // Printable form channels
                var formChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.PrintableForm.ToString());
                var formChannels = serviceChannels.Where(s => s.TypeId == formChannelTypeId).ToList();
                if (formChannels.HasData())
                {
                    var formChannelIds = formChannels.Select(i => i.Id).ToList();
                    var formChannelQuery = unitOfWork.CreateRepository<IPrintableFormChannelRepository>().All().Where(c => formChannelIds.Contains(c.ServiceChannelVersionedId));
                    var formChannelsWithData = unitOfWork.ApplyIncludes(formChannelQuery, q =>
                        q.Include(i => i.FormIdentifiers)
                         .Include(i => i.ChannelUrls).ThenInclude(i => i.WebPage)
                        ).ToList();
                    formChannelsWithData.ForEach(f =>
                    {
                        var channel = formChannels.FirstOrDefault(i => i.Id == f.ServiceChannelVersionedId);
                        if (channel != null)
                        {
                            channel.PrintableFormChannels.Add(f);
                        }
                    });
                    formChannels.ForEach(channel => FilterOutNotPublishedLanguageVersions(channel, publishedId, true));
                    resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiPrintableFormChannelVersionBase>(formChannels));
                }

                // Web page channels
                var webpageChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.WebPage.ToString());
                var webpageChannels = serviceChannels.Where(s => s.TypeId == webpageChannelTypeId).ToList();
                if (webpageChannels.HasData())
                {
                    var webpageChannelIds = webpageChannels.Select(i => i.Id).ToList();
                    var webpageChannelQuery = unitOfWork.CreateRepository<IWebpageChannelRepository>().All().Where(c => webpageChannelIds.Contains(c.ServiceChannelVersionedId));
                    var webpageChannelsWithData = unitOfWork.ApplyIncludes(webpageChannelQuery, q =>
                        q.Include(i => i.LocalizedUrls).ThenInclude(i => i.WebPage)).ToList();
                    webpageChannelsWithData.ForEach(w =>
                    {
                        var channel = webpageChannels.Where(i => i.Id == w.ServiceChannelVersionedId).FirstOrDefault();
                        if (channel != null)
                        {
                            channel.WebpageChannels.Add(w);
                        }
                    });
                    webpageChannels.ForEach(channel => FilterOutNotPublishedLanguageVersions(channel, publishedId, true));
                    resultList.AddRange(TranslationManagerToVm.TranslateAll<ServiceChannelVersioned, VmOpenApiWebPageChannelVersionBase>(webpageChannels));
                }

                // Find only published services for channels
                List<Guid> rootIds = resultList.Where(c => c.Id != null).Select(c => c.Id.Value).Distinct().ToList();
                allConnections = GetChannelServices(rootIds, unitOfWork);
                allCollections = GetServiceCollections(rootIds, unitOfWork);
                // Map all related ontologyterms for channels
                MapRelatedOntologyTerms(resultList, unitOfWork);
            }

            // Map connections.
            // Set the right version for service channels
            var versionList = new List<IVmOpenApiServiceChannel>();
            resultList.ForEach(channel =>
            {
                // Map service collections
                if (allCollections.HasData())
                {
                    var collections = allCollections.Where(c => c.OwnerReferenceId == channel.Id).ToList();
                    if (collections?.Count > 0)
                    {
                        channel.ServiceCollections = collections;
                    }
                }
                
                if (allConnections.HasData())
                {
                    var connections = allConnections.Where(s => s.OwnerReferenceId == channel.Id).ToList();
                    if (connections.HasData())
                    {
                        channel.Services = connections;
                        //if area not set by user set area default by areas of connected published services
                        if (channel.AreaType == null && channel.Services != null)
                        {
                            AddInheritedAreaInformation(channel.Services.Select(x => x.Service.Id).OfType<Guid>().ToList(), channel, unitOfWork);
                        }
                    }
                }
                versionList.Add(GetServiceChannelByOpenApiVersion(unitOfWork, channel, openApiVersion));
            });

            return versionList;
        }

        private IList<V4VmOpenApiOntologyTerm> GetAllRelatedOntologyTerms(Guid unificRootId, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }
            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var services = serviceRep.All()
                .Where(c => c.UnificRoot.ServiceServiceChannels.Any(x => x.ServiceChannelId == unificRootId))
                .Where(c => c.PublishingStatusId == publishedId)
                .Include(c => c.ServiceOntologyTerms).ThenInclude(x => x.OntologyTerm).ThenInclude(x => x.Names)
                .ToList();
            var serviceOntologyTerms = services.SelectMany(x => x.ServiceOntologyTerms.Select(y => y.OntologyTerm)).ToList();

            var gdUnificRootIds = services.Where(x => x.StatutoryServiceGeneralDescriptionId.HasValue)
                .Select(x => x.StatutoryServiceGeneralDescriptionId)
                .ToList();
            if (gdUnificRootIds.Any())
            {
                var gdRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                var gdOntologyTerms = gdRep.All()
                    .Where(c => gdUnificRootIds.Contains(c.UnificRootId))
                    .Where(c => c.PublishingStatusId == publishedId)
                    .Include(c => c.OntologyTerms).ThenInclude(x => x.OntologyTerm).ThenInclude(x => x.Names)
                    .ToList()
                    .SelectMany(x => x.OntologyTerms.Select(y => y.OntologyTerm));
                serviceOntologyTerms.AddRange(gdOntologyTerms);
            }
            return TranslationManagerToVm.TranslateAll<OntologyTerm, V4VmOpenApiOntologyTerm>(serviceOntologyTerms.DistinctBy(x => x.Id)).ToList();
        }

        private void MapRelatedOntologyTerms(IList<IVmOpenApiServiceChannel> channels, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            if (channels == null || channels.Count == 0)
            {
                return;
            }

            var rootIds = channels.Select(c => c.Id).ToList();

            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);
            var connections = unitOfWork.CreateRepository<IServiceServiceChannelRepository>().All()
                .Where(c => rootIds.Contains(c.ServiceChannelId))
                .Where(c => c.Service.Versions.Any(v => v.PublishingStatusId == publishedId))
                .ToList();

            if (connections == null || connections.Count == 0)
            {
                return;
            }

            var serviceIds = connections.Select(s => s.ServiceId).Distinct().ToList();

            var connectionDict = connections
                .GroupBy(c => c.ServiceChannelId)
                .ToDictionary(i => i.Key, i => i.Select(x => x.ServiceId).ToList());
            var serviceRep = unitOfWork.CreateRepository<IServiceVersionedRepository>();
            var services = serviceRep.All()
                .Where(c => serviceIds.Contains(c.UnificRootId))
                .Where(c => c.PublishingStatusId == publishedId)
                .Include(c => c.ServiceOntologyTerms)
                .ToList();
            var termIds = services.SelectMany(x => x.ServiceOntologyTerms).Select(x => x.OntologyTermId).Distinct().ToList();

            var gdUnificRootIds = services.Where(x => x.StatutoryServiceGeneralDescriptionId.HasValue)
                .Select(x => x.StatutoryServiceGeneralDescriptionId)
                .ToList();

            List<StatutoryServiceGeneralDescriptionVersioned> gds = null;
            if (gdUnificRootIds.Any())
            {
                var gdRep = unitOfWork.CreateRepository<IStatutoryServiceGeneralDescriptionVersionedRepository>();
                gds = gdRep.All()
                    .Where(c => gdUnificRootIds.Contains(c.UnificRootId))
                    .Where(c => c.PublishingStatusId == publishedId)
                    .Include(c => c.OntologyTerms)
                    .ToList();

                termIds.AddRange(gds.SelectMany(x => x.OntologyTerms).Select(x => x.OntologyTermId).Distinct().ToList());
            }

            if (termIds == null || termIds.Count == 0)
            {
                return;
            }

            var ots = unitOfWork.CreateRepository<IOntologyTermRepository>().All()
                .Where(o => termIds.Contains(o.Id))
                .Include(x => x.Names).ToList();

            if (ots?.Count > 0)
            {
                var ontologyTerms = TranslationManagerToVm.TranslateAll<OntologyTerm, V4VmOpenApiOntologyTerm>(ots).ToList();

                channels.ForEach(channel =>
                {
                    if (connectionDict.TryGetValue(channel.Id.Value, out var channelServiceIds))
                    {
                        var channelServices = services.Where(s => channelServiceIds.Contains(s.UnificRootId));
                        var channelTermIds = channelServices.SelectMany(x => x.ServiceOntologyTerms).Select(x => x.OntologyTermId).Distinct().ToList();
                        if (gds?.Count > 0)
                        {
                            var channelServiceGds = channelServices.Where(c => c.StatutoryServiceGeneralDescriptionId != null).Select(c => c.StatutoryServiceGeneralDescriptionId.Value).ToList();
                            if (channelServiceGds?.Count > 0)
                            {
                                channelTermIds.AddRange(gds.Where(g => channelServiceGds.Contains(g.UnificRootId)).SelectMany(x => x.OntologyTerms).Select(x => x.OntologyTermId).ToList());
                            }

                        }

                        channel.OntologyTerms = ontologyTerms.Where(o => channelTermIds.Contains(o.Id)).Distinct().ToList();
                    }
                });
            }
        }
        
        private List<VmOpenApiServiceServiceCollection> GetServiceCollections(List<Guid> rootIds, IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            // Find only published service collections for a service (which have at least one published language version)
            var serviceCollectionRep = unitOfWork.CreateRepository<IServiceCollectionServiceChannelRepository>();
            var serviceCollectionVersionsRep = unitOfWork.CreateRepository<IServiceCollectionVersionedRepository>();
            var allCollections = serviceCollectionRep.All()
                .Where(c => rootIds.Contains(c.ServiceChannelId) 
                            && c.ServiceCollection.Versions.Any(x => x.PublishingStatusId == publishedId) 
                            && c.ServiceCollection.Versions.Any(t => t.LanguageAvailabilities.Any(l => l.StatusId == publishedId)))
                .ToList();

            var collectionIds = allCollections.Select(i => i.ServiceCollectionId).ToList();
            var collectionVersions = serviceCollectionVersionsRep.All()
                .Where(i => collectionIds.Contains(i.UnificRootId) && i.PublishingStatusId == publishedId)
                .Include(j => j.ServiceCollectionNames)
                .ToList();

            return allCollections.Select(collection =>
                new VmOpenApiServiceServiceCollection
                {
                    Id = collection.ServiceCollectionId,
                    Name = TranslationManagerToVm.TranslateAll<ServiceCollectionName, VmOpenApiLanguageItem>(
                        collectionVersions
                            .FirstOrDefault(i => i.UnificRootId == collection.ServiceCollectionId)?.ServiceCollectionNames)
                        .InclusiveToList(),
                    OwnerReferenceId = collection.ServiceChannelId
                }).ToList();
        }

        private List<V11VmOpenApiServiceChannelService> GetChannelServices(List<Guid> rootIds, IUnitOfWork unitOfWork)
        {
            var serviceList = new List<V11VmOpenApiServiceChannelService>();

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            var publishedId = PublishingStatusCache.Get(PublishingStatus.Published);

            var connectionRep = unitOfWork.CreateRepository<IServiceServiceChannelRepository>();
            var connectionQuery = connectionRep.All().Where(c => rootIds.Contains(c.ServiceChannelId) &&
                c.Service.Versions.Any(v => v.PublishingStatusId == publishedId &&
                v.LanguageAvailabilities.Any(l => l.StatusId == publishedId)));
            var connections = connectionQuery.ToList();

            if (connections.HasData())
            {
                // Fill with service names
                var serviceRootIds = connections.Select(s => s.ServiceId).ToList();
                commonService.IncludeConnectionAddresses(unitOfWork, connections);
                commonService.IncludeConnectionCommonDetails(unitOfWork, connections);
                commonService.IncludeConnectionContactDetails(unitOfWork, connections);

                var services = unitOfWork.ApplyIncludes(
                    unitOfWork.CreateRepository<IServiceVersionedRepository>().All().Where(i => serviceRootIds.Contains(i.UnificRootId) && i.PublishingStatusId == publishedId),
                    q => q.Include(i => i.UnificRoot).Include(i => i.ServiceNames).Include(i => i.LanguageAvailabilities)).ToList();
                connections.ForEach(c =>
                {
                    string name = null;
                    var service = services.FirstOrDefault(i => i.UnificRootId == c.ServiceId);
                    if (service != null)
                    {
                        var version = service.UnificRoot.Versions.FirstOrDefault();
                        if (version != null)
                        {
                            // Get published name for service (PTV-3689).
                            name = GetNameWithFallback(
                                version.ServiceNames,
                                version.LanguageAvailabilities.Where(i => i.StatusId == publishedId).Select(i => i.LanguageId).ToList(),
                                typesCache,
                                languageCache);
                        }
                    }
                    V11VmOpenApiServiceChannelService vm = new V11VmOpenApiServiceChannelService
                    {
                        OwnerReferenceId = c.ServiceChannelId,
                        Service = new VmOpenApiItem { Id = c.ServiceId, Name = name },
                        Modified = c.Modified
                    };

                    // map base connection data
                    MapConnection(c, vm, typesCache, languageCache, textManager);

                    // extra types
                    vm.ExtraTypes = GetExtraTypes(c, typesCache, languageCache);

                    // contactdetails
                    vm.ContactDetails = GetContactDetails<V9VmOpenApiContactDetails>(c, typesCache, languageCache);

                    // digitalAuthorizations
                    vm.DigitalAuthorizations = GetDigitalAuthorizations(c, languageCache);

                    serviceList.Add(vm);
                });

                return serviceList;
            }

            return serviceList;
        }

        private VmOpenApiServiceChannel GetServiceChannelWithSimpleDetails(IUnitOfWork unitOfWork, Guid versionId)
        {
            if (!versionId.IsAssigned()) return null;

            return GetModel<ServiceChannelVersioned, VmOpenApiServiceChannel>(GetEntity<ServiceChannelVersioned>(versionId, unitOfWork,
                    q => q.Include(x => x.LanguageAvailabilities)
                    .Include(i => i.ElectronicChannels)
                    .Include(i => i.Addresses)
                    .Include(i => i.WebpageChannels)
                    .Include(i => i.PrintableFormChannels)
                    .Include(i => i.ServiceChannelNames)
                    .Include(i => i.ServiceChannelDescriptions)), unitOfWork);
        }

        public IVmOpenApiServiceChannel AddServiceChannel<TVmChannelIn>(TVmChannelIn vm, int openApiVersion, string userName = null)
           where TVmChannelIn : class, IVmOpenApiServiceChannelIn
        {
            var saveMode = SaveMode.Normal;
            var userId = userName ?? utilities.GetRelationIdForExternalSource();
            var useOtherEndPoint = false;
            var serviceChannel = new ServiceChannelVersioned();
            var streetId = typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString());

            ProcessStreetAddresses(vm);
            ProcessNewUrls(vm);

            contextManager.ExecuteWriter(unitOfWork =>
            {
                // Check if the external source already exists. Let's not throw the excpetion here to avoid contextManager to catch the exception.
                useOtherEndPoint = ExternalSourceExists<ServiceChannel>(vm.SourceId, userId, unitOfWork);
                if (!useOtherEndPoint)
                {
                    // Set user name which is used within language availabilities and check the publishing status (SFIPTV-190)
                    vm.UserName = unitOfWork.GetUserNameForAuditing();
                    if (vm.ValidFrom.HasValue && vm.ValidFrom > DateTime.UtcNow)
                    {
                        // For timed publishing the version created needs to be set as draft
                        vm.PublishingStatus = PublishingStatus.Draft.ToString();
                    }

                    // Check address related municipalities
                    if (vm is VmOpenApiPrintableFormChannelInVersionBase)
                    {
                        (vm as VmOpenApiPrintableFormChannelInVersionBase).DeliveryAddresses.ForEach(a => CheckAddress(unitOfWork, a));
                    }
                    else if (vm is VmOpenApiServiceLocationChannelInVersionBase)
                    {
                        (vm as VmOpenApiServiceLocationChannelInVersionBase).Addresses.ForEach(a => CheckAddress(unitOfWork, a));
                    }

                    serviceChannel = TranslationManagerToEntity.Translate<TVmChannelIn, ServiceChannelVersioned>(vm, unitOfWork);

                    ValidateExpirationTime(unitOfWork, serviceChannel, DateTime.UtcNow);
                    // SFIPTV-1963: At the moment the Oid property is only a placeholder and will not be saved into DB!
                    // HandleSocialHealthCenter(serviceChannel.UnificRootId, vm, unitOfWork);

                    var serviceChannelRep = unitOfWork.CreateRepository<IServiceChannelVersionedRepository>();
                    serviceChannelRep.Add(serviceChannel);

                    // Create the mapping between external source id and PTV id
                    if (!string.IsNullOrEmpty(vm.SourceId))
                    {
                        SetExternalSource(serviceChannel.UnificRoot, vm.SourceId, userId, unitOfWork);
                    }

                    expirationService.SetExpirationDate(unitOfWork, serviceChannel);
                    commonService.CreateHistoryMetaData<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannel, setByEntity:true);
                    unitOfWork.Save(saveMode, userName: userName);// We need to save the item - otherwise when adding related services we are getting error "Sequence contains no elements" (SFIPTV-529)
                }
            });

            if (useOtherEndPoint)
            {
                throw new ExternalSourceExistsException(string.Format(CoreMessages.OpenApi.ExternalSourceExists, vm.SourceId));
            }
            
            // Add postal addresses
            ProcessPostalAddresses(vm, serviceChannel.Id);

            // Add related services
            if (vm.ServiceChannelServices.HasData())
            {
                //SFIPTV-1861
                vm.ServiceChannelServices =
                    ProcessConnectionsCommonLanguage(vm.ServiceChannelServices, serviceChannel.UnificRootId);
                foreach (var serviceId in vm.ServiceChannelServices)
                {
                    contextManager.ExecuteWriter(unitOfWork =>
                    {
                        var connection = new V11VmOpenApiServiceServiceChannelAstiInBase
                        {
                            ServiceGuid = serviceId,
                            ChannelGuid = serviceChannel.UnificRootId
                        };
                        TranslationManagerToEntity
                            .Translate<V11VmOpenApiServiceServiceChannelAstiInBase, ServiceServiceChannel>(connection,
                                unitOfWork);
                        unitOfWork.Save(saveMode, userName: userName);
                    });
                }
                contextManager.ExecuteWriter(unitOfWork =>
                {
                    TranslationManagerToEntity
                        .TranslateAll<V11VmOpenApiServiceServiceChannelAstiInBase, ServiceServiceChannel>(
                            vm.ServiceChannelServices.Select(serviceId => new V11VmOpenApiServiceServiceChannelAstiInBase
                                {ServiceGuid = serviceId, ChannelGuid = serviceChannel.UnificRootId}), unitOfWork);
                    unitOfWork.Save(saveMode, userName: userName);
                });
            }

            // Update the map coordinates for addresses
            if (serviceChannel.Addresses.HasData())
            {
                var addresses = serviceChannel.Addresses
                    .Where(a => a.Address.TypeId == streetId)
                    .Select(x => x.AddressId).ToList();
                addressService.UpdateAddress(addresses);
            }

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult =
                    commonService
                        .PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability
                        >(serviceChannel.Id, i => i.ServiceChannelVersionedId == serviceChannel.Id);
                var ids = new List<Guid> {serviceChannel.Id};
                expirationService.SetExpirationDateForPublishing<ServiceChannelVersioned>(contextManager, ids);
            }

            return GetServiceChannelWithDetails(serviceChannel.Id, openApiVersion, showHeader: true,false);
        }

        private IList<Guid> ProcessConnectionsCommonLanguage(IList<Guid> connections, Guid channelUnificRootId)
        {
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var removedConnections = utilities.ProcessConnectionCommonLanguage(DomainEnum.Channels,
                    new Dictionary<Guid, IEnumerable<Guid>>
                        {{channelUnificRootId, connections}}, unitOfWork);
                if (removedConnections.Any())
                {
                    var removedIds = removedConnections.Select(x => x.Item2);
                    return connections.Where(x => !removedIds.Contains(x)).ToList();
                }

                return connections;
            });
        }
        
        // SFIPTV-1919
        private void ValidateExpirationTime(IUnitOfWorkWritable unitOfWork, ServiceChannelVersioned channel, DateTime lastChangeDate)
        {
            var expirationTime = expirationService.GetExpirationDate(unitOfWork, channel, lastChangeDate: lastChangeDate);
            if (channel.LanguageAvailabilities.Any(la => la.PublishAt > expirationTime))
            {
                throw new PtvAppException("Publishing date cannot be scheduled after automatic archiving date.", "Channel.ScheduleException.LateDate");
            }
        }

        public IVmOpenApiServiceChannel SaveServiceChannel<TVmChannelIn>(TVmChannelIn vm, int openApiVersion, string userName = null)
            where TVmChannelIn : class, IVmOpenApiServiceChannelIn
        {
            if (vm == null) return null;

            var saveMode = SaveMode.Normal;
            var serviceChannel = new ServiceChannelVersioned();

            ProcessStreetAddresses(vm);
            ProcessNewUrls(vm);

            contextManager.ExecuteWriter(unitOfWork =>
            {
                if (vm.VersionId.IsAssigned())
                {
                    // Set user name which is used within language availabilities and check the publishing status (SFIPTV-190)
                    vm.UserName = unitOfWork.GetUserNameForAuditing();
                    var currentTime = DateTime.UtcNow;
                    if ((vm.ValidFrom.HasValue && vm.ValidFrom.Value > currentTime) || (vm.ValidTo.HasValue && vm.ValidTo.Value > currentTime))
                    {
                        if (vm.ValidFrom.HasValue)
                        {
                            // For timed publishing the version created needs to be set as modified
                            vm.PublishingStatus = PublishingStatus.Modified.ToString();
                        }
                        // We need to get the available languages to be able update the publishing and archiving dates for different language versions
                        var allAvailableLanguages = unitOfWork.CreateRepository<IRepository<ServiceChannelLanguageAvailability>>().All()
                            .Where(x => x.ServiceChannelVersionedId == vm.VersionId).Select(i => i.LanguageId).Select(x => languageCache.GetByValue(x)).ToHashSet();
                        vm.AvailableLanguages.ForEach(l => allAvailableLanguages.Add(l));
                        vm.AvailableLanguages = allAvailableLanguages.ToList();
                    }

                    if (vm.PublishingStatus == PublishingStatus.Deleted.ToString())
                    {
                        serviceChannel = DeleteChannel(unitOfWork, vm.VersionId);
                    }
                    else
                    {
                        // Entity needs to be restored?
                        if (vm.CurrentPublishingStatus == PublishingStatus.Deleted.ToString())
                        {
                            if (vm.PublishingStatus == PublishingStatus.Modified.ToString() || vm.PublishingStatus == PublishingStatus.Published.ToString())
                            {
                                // We need to restore already archived item
                                var publishingResult = commonService.RestoreArchivedEntity<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, vm.VersionId.Value);
                            }
                        }

                        // Check address related municipalities
                        if (vm is VmOpenApiPrintableFormChannelInVersionBase)
                        {
                            (vm as VmOpenApiPrintableFormChannelInVersionBase).DeliveryAddresses.ForEach(a => CheckAddress(unitOfWork, a));
                        }
                        else if (vm is VmOpenApiServiceLocationChannelInVersionBase)
                        {
                            (vm as VmOpenApiServiceLocationChannelInVersionBase).Addresses.ForEach(a => CheckAddress(unitOfWork, a));
                        }

                        serviceChannel = TranslationManagerToEntity.Translate<TVmChannelIn, ServiceChannelVersioned>(vm, unitOfWork);

                        ValidateExpirationTime(unitOfWork, serviceChannel, DateTime.UtcNow);

                        // Update the mapping between external source id and PTV id
                        if (!string.IsNullOrEmpty(vm.SourceId))
                        {
                            UpdateExternalSource<ServiceChannel>(serviceChannel.UnificRootId, vm.SourceId, utilities.GetRelationIdForExternalSource(), unitOfWork);
                        }
                    }

                    if (vm is VmOpenApiServiceLocationChannelInVersionBase)
                    {
                        if (serviceChannel.Addresses.HasData())
                        {
                            HandleAccessibilityRegisterAddress(unitOfWork, vm.VersionId, serviceChannel);
                        }

                        // SFIPTV-1963: At the moment the Oid property is only a placeholder and will not be saved into DB!
                        //var vmServiceLocation = vm as VmOpenApiServiceLocationChannelInVersionBase;
                        //if ((vmServiceLocation.DeleteOid && vmServiceLocation.Oid.IsNullOrEmpty()) || !vmServiceLocation.Oid.IsNullOrEmpty())
                        //{
                        //    HandleSocialHealthCenter(serviceChannel.UnificRootId, vmServiceLocation, unitOfWork);
                        //}
                    }

                    expirationService.SetExpirationDate(unitOfWork, serviceChannel);
                    commonService.CreateHistoryMetaData<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(serviceChannel, setByEntity: true);
                    unitOfWork.Save(saveMode, PreSaveAction.Standard, serviceChannel, userName);
                }
            });

            if (serviceChannel == null) return null;

            // Update the map coordinates for addresses and postal addresses
            if (vm.PublishingStatus != PublishingStatus.Deleted.ToString())
            {
                ProcessPostalAddresses(vm, serviceChannel.Id);
                var locationChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.ServiceLocation.ToString());
                var printableChannelTypeId = typesCache.Get<ServiceChannelType>(ServiceChannelTypeEnum.PrintableForm.ToString());

                if (serviceChannel.TypeId == locationChannelTypeId || serviceChannel.TypeId == printableChannelTypeId)
                {
                    var addresses = serviceChannel.Addresses?.Select(x => x.AddressId);
                    if (addresses != null)
                    {
                        addressService.UpdateAddress(addresses.ToList());
                    }
                }
            }

            // Publish all language versions
            if (vm.PublishingStatus == PublishingStatus.Published.ToString())
            {
                var publishingResult =
                    commonService
                        .PublishAllAvailableLanguageVersions<ServiceChannelVersioned, ServiceChannelLanguageAvailability
                        >(serviceChannel.Id, i => i.ServiceChannelVersionedId == serviceChannel.Id);
                var ids = new List<Guid> {serviceChannel.Id};
                expirationService.SetExpirationDateForPublishing<ServiceChannelVersioned>(contextManager, ids);
                if (vm.VersionId.HasValue && !vm.IsVisibleForAll)
                {
                    // If service channel is changed to not common, remove connected services
                    // from other organizations
                    contextManager.ExecuteWriter(unitOfWork =>
                    {
                        commonService.RemoveNotCommonConnections(new List<Guid> {vm.VersionId.Value}, unitOfWork,
                            false);
                        unitOfWork.Save(saveMode, PreSaveAction.Standard, serviceChannel, userName);
                    });
                }
            }

            return GetServiceChannelWithDetails(serviceChannel.Id, openApiVersion, showHeader: true,false);
        }

        private void ProcessNewUrls<TVmChannelIn>(TVmChannelIn vm) where TVmChannelIn : class, IVmOpenApiServiceChannelIn
        {
            var urlsToAdd = vm?.WebPages?.Select(x => x.Url).ToList() ?? new List<string>();
            switch (vm)
            {
                case VmOpenApiElectronicChannelInVersionBase ec:
                    urlsToAdd.AddRange(ec.Attachments?.Select(x => x.Url) ?? new List<string>());
                    break;
                case VmOpenApiPrintableFormChannelInVersionBase pf:
                    urlsToAdd.AddRange(pf.Attachments?.Select(x => x.Url) ?? new List<string>());
                    urlsToAdd.AddRange(pf.ChannelUrls?.Select(x => x.Value) ?? new List<string>());
                    break;
            }
            contextManager.ExecuteWriter(unitOfWork =>
            {
                urlService.AddNewUrls(unitOfWork, urlsToAdd);
                unitOfWork.Save();
            });
        }

        private void HandleAccessibilityRegisterAddress(IUnitOfWorkWritable unitOfWork, Guid? versioningId, ServiceChannelVersioned serviceChannel)
        {
            if (!serviceChannel.UnificRootId.IsAssigned()) return;
            if (!versioningId.IsAssigned()) return;
            var visitingAddressId = typesCache.Get<AddressCharacter>(AddressCharacterEnum.Visiting.ToString());

            var accessibilityRegister = unitOfWork.CreateRepository<IAccessibilityRegisterRepository>()
                .All()
                .Where(ar => ar.ServiceChannelId == serviceChannel.UnificRootId)
                .Include(ar => ar.Address).ThenInclude(a => a.ClsAddressPoints).ThenInclude(i => i.AddressStreet).ThenInclude(i => i.StreetNames)
                .FirstOrDefault();

            var arAddressEntityPoint = accessibilityRegister?.Address?.ClsAddressPoints?.SingleOrDefault();
            if (arAddressEntityPoint == null) return;

            // get info for compare
            var arAddressToCompare = new
            {
                StreetName = arAddressEntityPoint.AddressStreet?.Names?.SingleOrDefault(sn => sn.LocalizationId == accessibilityRegister.AddressLanguageId)?.Name,
                StreetNumber = arAddressEntityPoint.StreetNumber,
                MunicipalityId = (Guid?) arAddressEntityPoint.MunicipalityId,
                PostalCodeId = (Guid?) arAddressEntityPoint.PostalCodeId
            };

            // check, that address to compare has all needed values
            if (arAddressToCompare.StreetName.IsNullOrEmpty() || !arAddressToCompare.PostalCodeId.IsAssigned() || !arAddressToCompare.MunicipalityId.IsAssigned()) return;

            // get list of visiting addresses
            var visitingAddresses = serviceChannel.Addresses
                .Where(x => x.CharacterId == visitingAddressId && x.Address?.ClsAddressPoints.FirstOrDefault() != null)
                .Select(x => new {address = x.Address, clsAddressPoint = x.Address.ClsAddressPoints.FirstOrDefault()})
                .ToDictionary(key => key.address, value => new
                {
                    StreetName = value.clsAddressPoint.AddressStreet?.Names?.SingleOrDefault(sn => sn.LocalizationId == accessibilityRegister.AddressLanguageId)?.Name,
                    StreetNumber = value.clsAddressPoint.StreetNumber,
                    MunicipalityId = value.clsAddressPoint.MunicipalityId.IsAssigned() ? value.clsAddressPoint.MunicipalityId : value.clsAddressPoint.Municipality?.Id,
                    PostalCodeId = value.clsAddressPoint.PostalCodeId.IsAssigned() ? value.clsAddressPoint.PostalCodeId : value.clsAddressPoint.PostalCode?.Id
                });

            // check, that visiting addresses have filled in needed values
            var notComparableVisitingAddress = visitingAddresses
                .Where(x => x.Value.StreetName.IsNullOrEmpty() || !x.Value.PostalCodeId.IsAssigned() || !x.Value.MunicipalityId.IsAssigned())
                .Select(x => x.Key)
                .ToList();
            if (notComparableVisitingAddress.Any()) notComparableVisitingAddress.ForEach(a => visitingAddresses.Remove(a));
            if (!visitingAddresses.Any()) return;

            // NOTE: return back ID comparing when problem with addresses is fixed
//            var theSameAddress = serviceChannel.Addresses.Where(a => a.Address?.ClsAddressPoints?.SingleOrDefault()?.AddressStreet?.Id == addressEntityPoint.AddressStreetId
//                                                                     && a.Address?.ClsAddressPoints?.SingleOrDefault()?.PostalCode?.Id == addressEntityPoint.PostalCodeId
//                                                                     && a.Address?.ClsAddressPoints?.SingleOrDefault()?.StreetNumber == addressEntityPoint.StreetNumber)
//                .Select(a => a.Address).FirstOrDefault();


            var theSameAddress = visitingAddresses.FirstOrDefault(a => a.Value.StreetName == arAddressToCompare.StreetName
                                                                       && a.Value.StreetNumber == arAddressToCompare.StreetNumber
                                                                       && a.Value.MunicipalityId == arAddressToCompare.MunicipalityId
                                                                       && a.Value.PostalCodeId == arAddressToCompare.PostalCodeId)
                .Key;

            //if (theSameAddress != null) theSameAddress.UniqueId = arAddressEntity.UniqueId;
            if (theSameAddress != null) theSameAddress.UniqueId = accessibilityRegister.Address.UniqueId;
            else accessibilityRegisterService.DeleteAccessibilityRegister(unitOfWork, accessibilityRegister.Id, false);
        }

        #endregion

        private ServiceChannelVersioned DeleteChannel(IUnitOfWorkWritable unitOfWork, Guid? entityId)
        {
            if (entityId.HasValue)
            {
                commonService.DeleteServiceChannelConnections(unitOfWork, entityId.Value);
            }
            return commonService.ChangeEntityToDeleted<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(unitOfWork, entityId.Value, HistoryAction.Delete);
        }



        /// <summary>
        /// Checks if a service channel with given identifier exists in the system.
        /// </summary>
        /// <param name="channelId">guid of the channel</param>
        /// <returns>true if a channel exists otherwise false</returns>
        public bool ChannelExists(Guid channelId)
        {
            return channelId.IsAssigned() && contextManager.ExecuteReader(unitOfWork => unitOfWork.CreateRepository<IServiceChannelRepository>().All().Any(s => s.Id == channelId));
        }

        private void ProcessStreetAddresses(IVmOpenApiServiceChannelIn model)
        {
            var newStreets = new Dictionary<(Guid, string), Guid>();
            var streetAddresses = model switch
            {
                VmOpenApiServiceLocationChannelInVersionBase serviceLocation => serviceLocation.Addresses
                    ?.Where(x => x.StreetAddress != null).Select(x => x.StreetAddress),
                VmOpenApiPrintableFormChannelInVersionBase printableForm => printableForm.DeliveryAddresses
                    ?.Where(x => x.StreetAddress != null).Select(x => x.StreetAddress),
                _ => new List<VmOpenApiAddressStreetIn>()
            };

            var vmOpenApiAddressStreetIns = streetAddresses?.ToList() ?? new List<VmOpenApiAddressStreetIn>();
            if (!vmOpenApiAddressStreetIns.Any())
            {
                return;
            }
            
            var addresses = vmOpenApiAddressStreetIns.Select(x => new VmAddressSimple
            {
                StreetType = AddressTypeEnum.Street.ToString(),
                StreetName = x.Street.ToDictionary(k=>k.Language, v=>v.Value.Trim().FirstCharToUpper()),
                PostalCode = new VmPostalCode
                {
                    Id = postalCodeCache.GuidByCode(x.PostalCode),
                    Code = x.PostalCode,
                    MunicipalityId = postalCodeCache.MunicipalityIdForCode(x.PostalCode)
                },
                Municipality = postalCodeCache.MunicipalityIdForCode(x.PostalCode),
                StreetNumber = x.StreetNumber,
                Coordinates = new List<VmCoordinate>()
            });
            contextManager.ExecuteWriter(unitOfWork =>
            {
                addressService.AddNewStreetAddresses(unitOfWork, addresses, newStreets);
                unitOfWork.Save();
            });
            contextManager.ExecuteWriter(unitOfWork =>
            {
                addressService.AddNewStreetAddressNumbers(unitOfWork, addresses, newStreets);
                unitOfWork.Save();
            });
        }

        private void ProcessPostalAddresses(IVmOpenApiServiceChannelIn model, Guid channelId)
        {
            IEnumerable<VmOpenApiAddressPostOfficeBoxIn> postalAddresses = model switch
            {
                VmOpenApiServiceLocationChannelInVersionBase serviceLocation => serviceLocation.Addresses
                    ?.Where(x => x.PostOfficeBoxAddress != null).Select(x => x.PostOfficeBoxAddress) ?? new List<VmOpenApiAddressPostOfficeBoxIn>(),
                VmOpenApiPrintableFormChannelInVersionBase printableForm => printableForm.DeliveryAddresses
                    ?.Where(x => x.PostOfficeBoxAddress != null).Select(x => x.PostOfficeBoxAddress) ?? new List<VmOpenApiAddressPostOfficeBoxIn>(),
                _ => new List<VmOpenApiAddressPostOfficeBoxIn>()
            };

            if (!postalAddresses.Any())
            {
                return;
            }
            
            contextManager.ExecuteWriter(unitOfWork =>
            {
                addressService.AddServiceChannelPostalAddresses(unitOfWork, postalAddresses, channelId);
                unitOfWork.Save();
            });
        }

        private void HandleSocialHealthCenter(Guid? unificRootId, IVmOpenApiServiceChannelIn vm, IUnitOfWorkWritable unitOfWork)
        {
            if (!unificRootId.IsAssigned()) return;
            if (!(vm is VmOpenApiServiceLocationChannelInVersionBase locationChannel)) return;
            commonService.HandleSocialHealthCenter(new VmStringItem {OwnerReferenceId = unificRootId, Value = locationChannel.Oid}, unitOfWork);
        }
    }
}
