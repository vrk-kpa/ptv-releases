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

        public ServiceAndChannelService(IContextManager contextManager,
                                       ITranslationEntity translationEntToVm,
                                       ITranslationViewModel translationVmtoEnt,
                                       ILogger<OrganizationService> logger,
                                       ServiceUtilities utilities,
                                       DataUtils dataUtils,
                                       ICommonService commonService,
                                       IServiceService serviceService,
                                       IChannelService channelService,
                                       IPublishingStatusCache publishingStatusCache) : base(translationEntToVm, translationVmtoEnt, publishingStatusCache)
        {
            this.contextManager = contextManager;
            this.logger = logger;
            this.utilities = utilities;
            this.commonService = commonService;
            this.dataUtils = dataUtils;
            this.serviceService = serviceService;
            this.channelService = channelService;
        }

        public IVmEntityBase SaveServiceAndChannels(VmRelations relations)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                SetTranslatorLanguage(relations);
                var result= TranslationManagerToEntity.TranslateAll<VmServiceChannelRelation, Service>(relations.ServiceAndChannelRelations, unitOfWork);
                foreach (var service in result)
                {
                    service.ServiceServiceChannels = dataUtils.UpdateCollectionForReferenceTable(unitOfWork, service.ServiceServiceChannels, query => query.ServiceId == service.Id, channel => channel.ServiceChannelId);
                }

                unitOfWork.Save();
            });
            return new VmEntityBase();
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

            contextManager.ExecuteWriter(unitOfWork =>
            {
                var result = TranslationManagerToEntity.Translate<V2VmOpenApiServiceAndChannel, ServiceServiceChannel>(serviceServiceChannel, unitOfWork);
                unitOfWork.Save();
            });

            return string.Format(CoreMessages.OpenApi.ServiceServiceChannelAdded, channelId, serviceId);
        }
    }
}
