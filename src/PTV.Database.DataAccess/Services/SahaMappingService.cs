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
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(ISahaMappingService), RegisterType.Transient)]
    internal class SahaMappingService : ISahaMappingService
    {
        private readonly IContextManager contextManager;
        private readonly ICommonServiceInternal commonService;
        private readonly IVersioningManager versioningManager;
        private readonly ITypesCache typesCache;

        public SahaMappingService(IContextManager contextManager, ICommonServiceInternal commonService, IVersioningManager versioningManager,ICacheManager cacheManager)
        {
            this.contextManager = contextManager;
            this.commonService = commonService;
            this.versioningManager = versioningManager;
            this.typesCache = cacheManager.TypesCache;
        }


        public VmSahaMappings GetAllMappings()
        {
            return contextManager.ExecuteReader(unitOfWork =>
                {
                    return new VmSahaMappings(unitOfWork.CreateRepository<ISahaOrganizationInformationRepository>().All().Select(i => new SahaMappingDefinition {SahaId = i.SahaId, PtvOrganizationId = i.OrganizationId, SahaOrganizationName = i.Name}).ToList());
                });
        }

        public VmSahaMappings GetMappings(List<Guid> ptvIds)
        {

            return contextManager.ExecuteReader(unitOfWork =>
            {
                var organizationIds = ptvIds.ToDictionary(id => id,
                    id => versioningManager.GetLastVersion<OrganizationVersioned>(unitOfWork, id).EntityId);
                var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
                var organizations = organizationRep
                    .All()
                    .Include(x => x.LanguageAvailabilities)
                    .Include(x=>x.OrganizationNames)
                    .Where(x => organizationIds.Values.Contains(x.Id))
                    .ToList();
                var organizationLangs =
                    commonService.GetLanguageAvailabilites<OrganizationVersioned, OrganizationLanguageAvailability>(
                        organizations);
                var names = commonService.GetEntityNames(organizations);

                var results = unitOfWork.CreateRepository<ISahaOrganizationInformationRepository>().All()
                    .Where(i=> ptvIds.Contains(i.OrganizationId))
                    .ToList()
                    .Select(i => new SahaMappingDefinition
                    {
                        SahaId = i.SahaId,
                        PtvOrganizationId = i.OrganizationId,
                        SahaOrganizationName = i.Name,
                        OrganizationName = names.ContainsKey(i.OrganizationId) ? names[i.OrganizationId] : new Dictionary<string, string>(),
                        LanguagesAvailabilities = organizationLangs.ContainsKey(i.OrganizationId) ? organizationLangs[i.OrganizationId].ToList() : new List<VmLanguageAvailabilityInfo>(),
                        EntityId = organizationIds[i.OrganizationId],
                        OrganizationTypeId = organizations.First(x=>x.UnificRootId == i.OrganizationId).TypeId.Value,
                        ServiceCount = GetServiceCount(unitOfWork, organizationIds[i.OrganizationId]),
                        ChannelCount = GetChannelCount(unitOfWork, organizationIds[i.OrganizationId])
                    }).ToList();

                return new VmSahaMappings(results);
            });
        }

        private int GetChannelCount(IUnitOfWork unitOfWork, Guid id)
        {
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var psRemoved = typesCache.Get<PublishingStatusType>(PublishingStatus.Removed.ToString());

            var organization = organizationRep.All().Where(x => x.Id == id);
            return organization.Select(x => x.UnificRoot.OrganizationServiceChannelsVersioned.Count(j =>
                    j.PublishingStatusId != psDeleted &&
                    j.PublishingStatusId != psOldPublished &&
                    j.PublishingStatusId != psRemoved)).First();
        }
        private int GetServiceCount(IUnitOfWork unitOfWork, Guid id)
        {
            var organizationRep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var psDeleted = typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString());
            var psOldPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.OldPublished.ToString());
            var psRemoved = typesCache.Get<PublishingStatusType>(PublishingStatus.Removed.ToString());

            var organization = organizationRep.All().Where(x => x.Id == id);

            return /*organization.Select(x => x.UnificRoot.OrganizationServices.Count(j =>
                            j.ServiceVersioned.PublishingStatusId != psDeleted &&
                            j.ServiceVersioned.PublishingStatusId != psOldPublished &&
                            j.ServiceVersioned.PublishingStatusId != psRemoved)).First() +
                    organization.Select(x => x.UnificRoot.ServiceProducerOrganizations.Count(j =>
                            j.ServiceProducer.ServiceVersioned.PublishingStatusId != psDeleted &&
                            j.ServiceProducer.ServiceVersioned.PublishingStatusId != psOldPublished &&
                            j.ServiceProducer.ServiceVersioned.PublishingStatusId != psRemoved)).First() +*/
                    organization.Select(x => x.UnificRoot.OrganizationServicesVersioned.Count(j =>
                            j.PublishingStatusId != psDeleted &&
                            j.PublishingStatusId != psOldPublished &&
                            j.PublishingStatusId != psRemoved)).First();

        }

        public void MapSahaIdToPtvId(SahaMappingDefinition mappingDefinition)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<ISahaOrganizationInformationRepository>();
                var mapping = rep.All().FirstOrDefault(i => i.SahaId == mappingDefinition.SahaId);
                if (mapping == null)
                {
                    throw new ArgumentException("SaHaId not exists in PTV!");
                }
                rep.Remove(mapping);
                rep.Add(new SahaOrganizationInformation
                {
                    Name = mapping?.Name,
                    OrganizationId = mappingDefinition.PtvOrganizationId,
                    SahaId = mappingDefinition.SahaId,
                    SahaParentId = mapping.SahaParentId
                });
                unitOfWork.Save();
            });
        }

        public void RemoveSahaIdMapping(SahaMappingDefinitionBase model)
        {
            contextManager.ExecuteWriter(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<ISahaOrganizationInformationRepository>();
                var mapping = rep.All().FirstOrDefault(i => i.SahaId == model.SahaId);
                rep.Remove(mapping);
                unitOfWork.Save();
            });
        }
    }
}
