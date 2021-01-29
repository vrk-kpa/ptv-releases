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
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using System;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Tests.TestHelpers
{
    internal static class EntityGenerator
    {
        public static List<AreaMunicipality> GetAreaMunicipalityList(Guid? areaId = null, Guid? municipalityId = null)
        {
            return new List<AreaMunicipality>
            {
                new AreaMunicipality
                {
                    AreaId = areaId.IsAssigned() ? areaId.Value : Guid.NewGuid(),
                    MunicipalityId = municipalityId.IsAssigned() ? municipalityId.Value : Guid.NewGuid()
                }
            };
        }

        public static List<ServiceChannelVersioned> GetServiceChannelEntityList(int count, IPublishingStatusCache publishingStatusCache)
        {
            return CreateEntityList<ServiceChannelVersioned, ServiceChannel, ServiceChannelLanguageAvailability>(count, publishingStatusCache);
        }

        public static List<ServiceVersioned> GetServiceEntityList(int count, IPublishingStatusCache publishingStatusCache)
        {
            var list = CreateEntityList<ServiceVersioned, Service, ServiceLanguageAvailability>(count, publishingStatusCache);
            // set main responsible organization
            list.ForEach(o =>
            {
                o.Organization = new Organization();
            });
            return list;
        }

        public static List<StatutoryServiceGeneralDescriptionVersioned> GetGeneralDescriptionEntityList(int count, IPublishingStatusCache publishingStatusCache)
        {
            return CreateEntityList<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription, GeneralDescriptionLanguageAvailability>(count, publishingStatusCache);
        }

        /// <summary>
        /// Creates a list of organizations that includes each publishing status type 'count' times.
        /// For example if count is 2, the list will include 2 * 5 items (because we have 5 different publishing statuses).
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvailability"></typeparam>
        /// <param name="count">Indicates how many of each publishing status type items the list will include.</param>
        /// <returns></returns>
        public static List<OrganizationVersioned> GetOrganizationEntityList(int count, IPublishingStatusCache publishingStatusCache, bool attachServices = true, bool attachChannels = true)
        {
            var list = CreateEntityList<OrganizationVersioned, Organization, OrganizationLanguageAvailability>(count, publishingStatusCache);
            // set service and/or channels
            if (attachServices || attachChannels)
            {
                list.ForEach(o =>
                {
                    if (attachServices)
                    {
                        o.UnificRoot.OrganizationServicesVersioned = new List<ServiceVersioned> { new ServiceVersioned
                        {
                            OrganizationId = o.UnificRootId,
                            PublishingStatusId = o.PublishingStatusId
                        } };
                    }
                    if (attachChannels)
                    {
                        o.UnificRoot.OrganizationServiceChannelsVersioned = new List<ServiceChannelVersioned> { new ServiceChannelVersioned
                        {
                            OrganizationId = o.UnificRootId,
                            PublishingStatusId = o.PublishingStatusId
                        }};
                    }
                });
            }
            return list;
        }

        /// <summary>
        /// Creates an entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvailability"></typeparam>
        /// <returns></returns>
        public static TEntity CreateEntity<TEntity, TRootEntity, TLanguageAvailability>(Guid statusId, Guid? rootId = null, Guid? versionId = null, Guid? languageId = null) where TEntity :
            EntityIdentifierBase, IVersionedVolume<TRootEntity>, IPublishingStatus, IMultilanguagedEntity<TLanguageAvailability>, new()
            where TRootEntity : VersionedRoot<TEntity>, new()
            where TLanguageAvailability : LanguageAvailability, new()
        {
            var uniqueRootId = rootId.IsAssigned() ? rootId.Value : Guid.NewGuid();
            var entity = new TEntity
            {
                Id = versionId.IsAssigned() ? versionId.Value : Guid.NewGuid(),
                UnificRootId = uniqueRootId,
                PublishingStatusId = statusId,
                LanguageAvailabilities = new List<TLanguageAvailability>
                {
                    new TLanguageAvailability { StatusId = statusId, LanguageId = languageId ?? Guid.NewGuid() }
                },
                Created = DateTime.Now.AddDays(-1),
                Modified = DateTime.Now.AddDays(-1),
                UnificRoot = new TRootEntity { Id = uniqueRootId }
            };
            entity.UnificRoot.Versions = new List<TEntity> { entity };
            return entity;
        }

        /// <summary>
        /// Creates a list of entities that includes each publishing status type 'count' times.
        /// For example if count is 2, the list will include 2 * 5 items (because we have 5 different publishing statuses).
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvailability"></typeparam>
        /// <param name="count">Indicates how many of each publishing status type items the list will include.</param>
        /// <returns></returns>
        private static List<TEntity> CreateEntityList<TEntity, TRootEntity, TLanguageAvailability>(int count, IPublishingStatusCache publishingStatusCache) where TEntity :
            EntityIdentifierBase, IVersionedVolume<TRootEntity>, IPublishingStatus, IMultilanguagedEntity<TLanguageAvailability>, new()
            where TRootEntity : VersionedRoot<TEntity>, new()
            where TLanguageAvailability : LanguageAvailability, new()
        {
            var list = new List<TEntity>();
            for(var i = 0; i < count; i++)
            {
                list.Add(CreateEntity<TEntity, TRootEntity, TLanguageAvailability>(publishingStatusCache.Get(PublishingStatus.Published)));
                list.Add(CreateEntity<TEntity, TRootEntity, TLanguageAvailability>(publishingStatusCache.Get(PublishingStatus.Draft)));
                list.Add(CreateEntity<TEntity, TRootEntity, TLanguageAvailability>(publishingStatusCache.Get(PublishingStatus.Modified)));
                list.Add(CreateEntity<TEntity, TRootEntity, TLanguageAvailability>(publishingStatusCache.Get(PublishingStatus.OldPublished)));
                list.Add(CreateEntity<TEntity, TRootEntity, TLanguageAvailability>(publishingStatusCache.Get(PublishingStatus.Deleted)));
            }

            return list;
        }
    }
}
