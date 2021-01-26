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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums.Security;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Extensions;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Services
{
    internal interface IServiceUtilities
    {
        UserRoleEnum UserHighestRole();
        IList<Guid> GetUserOrganizations();
        Guid GetUserMainOrganization();
        IList<Guid> GetAllUserOrganizations();

        /// <summary>
        /// Get the identification of current user for ExternalSource table.
        /// </summary>
        /// <returns></returns>
        string GetRelationIdForExternalSource(bool throwExceptionIfNotExists = true);

        WebPage GetWebPageByUrl(string url, string language, IUnitOfWork unitOfWork);
        IVmEntityBase LockEntityRoot<T>(Guid id) where T : IEntityIdentifier, IVersionedRoot;
        IVmEntityBase LockEntityVersioned<T, T2>(Guid id, bool isLockDisAllowedForArchived = false) where T : class, IEntityIdentifier, IVersionedVolume<T2> where T2 : IVersionedRoot;
        void CheckIsEntityConnectable<T>(Guid id, IUnitOfWork unitOfWork) where T : class, IVersionedVolume;
        IVmEntityBase UnLockEntityRoot<T>(Guid id) where T : IEntityIdentifier, IVersionedRoot;
        IVmEntityBase UnLockEntityVersioned<T, T2>(Guid id) where T : class, IEntityIdentifier, IVersionedVolume<T2> where T2 : IVersionedRoot;
        IVmEntityBase UnLockEntityVersioned<T, T2>(T entity) where T : class, IEntityIdentifier, IVersionedVolume<T2> where T2 : IVersionedRoot;
        IVmEntityBase GetEntityEditableInfo<TVersioned, TRoot, TLanguages>(Guid id)
            where TVersioned : class, IEntityIdentifier, IVersionedVolume<TRoot>
            where TRoot : IVersionedRoot
            where TLanguages : ILanguageAvailabilityBase;
        IVmEntityBase GetEntityEditableInfo<TVersioned, TRoot, TLanguages>(Guid id, IUnitOfWork unitOfWork)
            where TVersioned : class, IEntityIdentifier, IVersionedVolume<TRoot>
            where TRoot : IVersionedRoot
            where TLanguages : ILanguageAvailabilityBase;
        IVmEntityLockBase EntityLockedBy(Guid id);
        IVmLastModifiedInfo GetEntityLastModifiedInfo<T, T2>(Guid id, IUnitOfWork unitOfWork) where T : class, IEntityIdentifier, IVersionedVolume<T2> where T2 : IVersionedRoot;

        Dictionary<Guid, List<String>> GetEntityLanguages<TEntity, TLanguageAvail>(List<Guid> serviceUnificRootIds,
            IUnitOfWork unitOfWork)
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability;

        List<(Guid mainEntityId, Guid connectedEntityId)> ProcessConnectionCommonLanguage(DomainEnum mainType, Dictionary<Guid, IEnumerable<Guid>> ids,
            IUnitOfWork unitOfWork);
        
        /// <summary>
        /// Check Id according to global OID regex
        /// </summary>
        /// <param name="id">OID</param>
        void CheckIdFormat(string id);
    }

    [RegisterService(typeof(IServiceUtilities), RegisterType.Transient)]
    internal class ServiceUtilities : IServiceUtilities
    {
        private readonly IPahaTokenAccessor pahaTokenAccessor;
        private readonly ILockingManager lockingManager;
        private readonly IContextManager contextManager;
        private readonly IUserOrganizationService userOrganizationService;
        private IVersioningManager versioningManager;
        private IUserOrganizationChecker userOrganizationChecker;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        //private readonly ITranslationService translationService;

        public ServiceUtilities
        (
            IPahaTokenAccessor pahaTokenAccessor,
            ILockingManager lockingManager,
            IContextManager contextManager,
            IUserOrganizationService userOrganizationService,
            IVersioningManager versioningManager,
            //ITranslationService translationService,
            IUserOrganizationChecker userOrganizationChecker,
            ICacheManager cacheManager)
        {
            this.pahaTokenAccessor = pahaTokenAccessor;
            this.lockingManager = lockingManager;
            this.contextManager = contextManager;
            this.userOrganizationService = userOrganizationService;
            this.versioningManager = versioningManager;
            this.userOrganizationChecker = userOrganizationChecker;
            this.typesCache = cacheManager.TypesCache;
            this.languageCache = cacheManager.LanguageCache;
            //this.translationService = translationService;
        }

        public UserRoleEnum UserHighestRole()
        {
            var userOrgs = userOrganizationService.GetOrganizationsAndRolesForLoggedUser();
            if (userOrgs.IsNullOrEmpty()) return UserRoleEnum.Shirley;
            return userOrgs.Select(i => i.Role).OrderBy(i => i).FirstOrDefault();
        }


        public IList<Guid> GetUserOrganizations()
        {
            return userOrganizationService.GetOrganizationsAndRolesForLoggedUser().Select(i => i.OrganizationId).ToList();
        }

        public Guid GetUserMainOrganization()
        {
            var orgs = userOrganizationService.GetOrganizationsAndRolesForLoggedUser();
            return (orgs.FirstOrDefault(i => i.IsMain) ?? orgs.FirstOrDefault()).OrganizationId;
        }

        /// <summary>
        /// Selects all draft, published and modified user organizations.
        /// </summary>
        /// <returns></returns>
        public IList<Guid> GetAllUserOrganizations()
        {
            var allowedPublishingStatuses = new List<PublishingStatus>
            {
                PublishingStatus.Published, PublishingStatus.Draft, PublishingStatus.Modified
            };

            return userOrganizationService.GetAllUserOrganizationIds(allowedPublishingStatuses);
        }

        /// <summary>
        /// Get the identification of current user for ExternalSource table.
        /// </summary>
        /// <returns></returns>
        public string GetRelationIdForExternalSource(bool throwExceptionIfNotExists = true)
        {
            var relationId = pahaTokenAccessor.UserName;
            if (throwExceptionIfNotExists && string.IsNullOrEmpty(relationId))
            {
                throw new Exception(CoreMessages.OpenApi.RelationIdNotFound);
            }
            return relationId;
        }

        public WebPage GetWebPageByUrl(string url, string language, IUnitOfWork unitOfWork)
        {
            var webpageRep = unitOfWork.CreateRepository<IServiceWebPageRepository>();
            return webpageRep.All()
                .Include(x => x.WebPage)
                .Include(x => x.Localization)
                .FirstOrDefault(webPage => webPage.WebPage.Url == url && webPage.Localization.Code == language)?.WebPage;
        }

        public IVmEntityBase LockEntityRoot<T>(Guid id) where T : IEntityIdentifier, IVersionedRoot
        {
            var lockResult = lockingManager.LockEntity<T>(id);
            if (lockResult.LockStatus != EntityLockEnum.LockedForCurrent)
            {
                throw new LockException("",new List<string> { lockResult.LockedBy });
            }
            return new VmEntityLockBase { Id = id, EntityLockedForMe = lockResult.LockStatus == EntityLockEnum.LockedForCurrent, LockedBy = lockResult.LockedBy };
        }

        public IVmEntityBase LockEntityVersioned<T, T2>(Guid id, bool isLockDisAllowedForArchived = false) where T : class, IEntityIdentifier, IVersionedVolume<T2> where T2 : IVersionedRoot
        {
            Guid? rootId = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                rootId = unitOfWork.GetSet<T>().Where(i => i.Id == id).Select(i => i.UnificRootId).FirstOrDefault();
                switch (versioningManager.IsAllowedForEditing<T>(unitOfWork, id))
                {
                    case VersionEditableReason.ModefiedExist:
                        throw new ModifiedExistsException("Modified version already exists", new List<string>());
                    case VersionEditableReason.NotAllowed:
                        throw new LockNotAllowedException();
                }
            });
            if (rootId == null)
            {
                throw new LockException("", new List<string> { CoreMessages.EntityNotFoundToUpdate });
            }

            if (!CheckEntityAccess<T, T2>(rootId.Value, id))
            {
                throw new Exception(CoreMessages.EntityAccessDenied);
            }
            return LockEntityRoot<T2>(rootId.Value);
        }

        public void CheckIsEntityConnectable<T>(Guid id, IUnitOfWork unitOfWork) where T : class, IVersionedVolume
        {
            if (versioningManager.IsEntityArchived<T>(unitOfWork, id))
            {
                throw new LockNotAllowedException();
            }
        }

        public IVmEntityBase UnLockEntityRoot<T>(Guid id) where T : IEntityIdentifier, IVersionedRoot
        {
            var lockResult = lockingManager.UnLockEntity(id);
            if (lockResult != EntityLockEnum.Unlocked)
            {
                throw new LockException("", new List<string> { lockResult.ToString() });
            }
            return new VmEntityLockBase { Id = id, EntityLockedForMe = false, LockedBy = string.Empty };
        }

        public IVmEntityBase UnLockEntityVersioned<T, T2>(Guid id) where T : class, IEntityIdentifier, IVersionedVolume<T2> where T2 : IVersionedRoot
        {
            Guid? rootId = null;
            contextManager.ExecuteReader(unitOfWork =>
            {
                rootId = unitOfWork.GetSet<T>().Where(i => i.Id == id).Select(i => i.UnificRootId).FirstOrDefault();
            });
            if (rootId == null)
            {
                throw new LockException("", new List<string> { CoreMessages.EntityNotFoundToUpdate });
            }
            return UnLockEntityRoot<T2>(rootId.Value);
        }

        public IVmEntityBase UnLockEntityVersioned<T, T2>(T entity) where T : class, IEntityIdentifier, IVersionedVolume<T2> where T2 : IVersionedRoot
        {
            return UnLockEntityRoot<T2>(entity.UnificRootId);
        }

//        internal IVmEntityBase CheckIsEntityLocked<T, T2>(Guid id) where T : class, IEntityIdentifier, IVersionedVolume<T2> where T2 : IVersionedRoot
//        {
//            if (CheckEntityAccess<T, T2>(id))
//            {
//                var lockResult = lockingManager.IsLockedBy(id);
//
//                var result = new VmEntityLockBase
//                {
//                    Id = id,
//                    EntityLockStatus = lockResult.LockStatus,
//                    EntityLockedForMe = lockResult.LockStatus == EntityLockEnum.LockedForCurrent,
//                    LockedBy = lockResult.LockedBy
//
//                };
//                if (lockResult.LockStatus == EntityLockEnum.LockedForAnother)
//                {
//                    throw new LockException("", new List<string>() { lockResult.LockedBy }, result);
//                }
//
//                return result;
//            }
//            return null;
//        }

        public IVmEntityBase GetEntityEditableInfo<TVersioned, TRoot, TLanguages>(Guid id)
            where TVersioned : class, IEntityIdentifier, IVersionedVolume<TRoot>
            where TRoot : IVersionedRoot
            where TLanguages : ILanguageAvailabilityBase
        {
            return contextManager.ExecuteReader(unitOfWork => GetEntityEditableInfo<TVersioned, TRoot, TLanguages>(id, unitOfWork));
        }

        public IVmEntityBase GetEntityEditableInfo<TVersioned, TRoot, TLanguages>(Guid id, IUnitOfWork unitOfWork)
            where TVersioned : class, IEntityIdentifier, IVersionedVolume<TRoot>
            where TRoot : IVersionedRoot
            where TLanguages : ILanguageAvailabilityBase
        {
            Guid? rootId = null;
            rootId = versioningManager.GetUnificRootId<TVersioned>(unitOfWork, id);

            var result = new VmEntityEditableBase();

            if (!rootId.HasValue) return null;

            var lastModified = versioningManager.GetLastModifiedVersion<TVersioned>(unitOfWork, rootId.Value);
            var lastPublished = versioningManager.GetLastPublishedVersion<TVersioned>(unitOfWork, rootId.Value);
            result = new VmEntityEditableBase
            {
                Id = id,
                UnificRootId = rootId.Value,
                LastModifiedId = lastModified?.EntityId,
                ModifiedOfLastModified = lastModified?.Modified,
                LastPublishedId = lastPublished?.EntityId,
                ModifiedOfLastPublished = lastPublished?.Modified
            };
            if (lastPublished != null)
            {
                var languageAvailabilityRep = unitOfWork.CreateRepository<IRepository<TLanguages>>();
                var eId = lastPublished.EntityId;
                var statusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                result.LastPublishedLanguages = languageAvailabilityRep.All()
                    .WithEntityId(eId)
                    .Where(x => x.StatusId == statusId)
                    .Select(x => x.LanguageId)
                    .ToList();
            }
            if (lastModified != null)
            {
                var languageAvailabilityRep = unitOfWork.CreateRepository<IRepository<TLanguages>>();
                var eId = lastModified.EntityId;
                var statusModifiedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
                var statusDraftId = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
                result.LastModifiedLanguages = languageAvailabilityRep.All()
                    .WithEntityId(eId)
                    .Where(x => x.StatusId == statusModifiedId || x.StatusId == statusDraftId)
                    .Select(x => x.LanguageId)
                    .ToList();
            }

            if (CheckEntityAccess<TVersioned, TRoot>(rootId.Value, id, unitOfWork))
            {
                result.IsEditable = versioningManager.IsAllowedForEditing<TVersioned>(unitOfWork, id) ==
                                    VersionEditableReason.Editable;
            }
            return result;
        }


        public IVmLastModifiedInfo GetEntityLastModifiedInfo<T, T2>(Guid id, IUnitOfWork unitOfWork) where T : class, IEntityIdentifier, IVersionedVolume<T2> where T2 : IVersionedRoot
        {
            var rootId = versioningManager.GetUnificRootId<T>(unitOfWork, id);

            if (rootId.HasValue)
            {
                var lastModified = versioningManager.GetLastModifiedVersion<T>(unitOfWork, rootId.Value);
                var result = new VmLastModifiedInfo
                {
                    Id = id,
                    UnificRootId = rootId.Value,
                    LastModifiedId = lastModified?.EntityId,
                };

                return result;
            }
            return null;
        }

        private bool CheckEntityAccess<T, T2>(Guid rootId, Guid versionId) where T : class, IEntityIdentifier, IVersionedVolume<T2> where T2 : IVersionedRoot
        {
            var access = true;
            var userOrgs =  userOrganizationService.GetOrganizationsAndRolesForLoggedUser();
            if (userOrgs.All(i => i.Role != UserRoleEnum.Eeva))
            {
                contextManager.ExecuteReader(unitOfWork =>
                {
                    var entity = unitOfWork.GetSet<T>().FirstOrDefault(i => i.UnificRootId == rootId && i.Id == versionId);
                    var security = userOrganizationChecker.CheckEntity(entity, unitOfWork);
                    access = security.IsOwnOrganization;
                });
            }
            return access;
        }

        private bool CheckEntityAccess<T, T2>(Guid rootId, Guid versionId, IUnitOfWork unitOfWork) where T : class, IEntityIdentifier, IVersionedVolume<T2> where T2 : IVersionedRoot
        {
            var access = true;
            var userOrgs =  userOrganizationService.GetOrganizationsAndRolesForLoggedUser();
            if (userOrgs.All(i => i.Role != UserRoleEnum.Eeva))
            {
                var entity = unitOfWork.GetSet<T>().FirstOrDefault(i => i.UnificRootId == rootId && i.Id == versionId);
                var security = userOrganizationChecker.CheckEntity(entity, unitOfWork);
                access = security.IsOwnOrganization;

            }
            return access;
        }

        public IVmEntityLockBase EntityLockedBy(Guid id)
        {
            var lockedBy = lockingManager.IsLockedBy(id);
            return new VmEntityLockBase
            {
                Id = id,
                EntityLockStatus = lockedBy.LockStatus,
                EntityLockedForMe = lockedBy.LockStatus == EntityLockEnum.LockedForCurrent,
                LockedBy = lockedBy.LockedBy
            };
        }

        public void CheckIdFormat(string id)
        {
            if (!string.IsNullOrEmpty(id) && !Regex.IsMatch(id, DomainConstants.OidParser))
            {
                throw new IdFormatException("", id);
            }
        }
        /// <summary>
        /// Find all connections which has no common language
        /// </summary>
        /// <param name="mainType"></param>
        /// <param name="ids"></param>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public List<(Guid mainEntityId, Guid connectedEntityId)> ProcessConnectionCommonLanguage(DomainEnum mainType, Dictionary<Guid, IEnumerable<Guid>> ids, IUnitOfWork unitOfWork)
        {
            var languages = new Dictionary<Guid, List<String>>();
            var result = new List<(Guid mainEntityId, Guid connectedEntityId)>();
            
            switch (mainType)
            {
                case DomainEnum.Services:
                    GetEntityLanguages<ServiceVersioned, ServiceLanguageAvailability>(ids.Keys.ToList(),
                            unitOfWork)
                        .ForEach(x => languages.Add(x.Key, x.Value));
                    GetEntityLanguages<ServiceChannelVersioned, ServiceChannelLanguageAvailability>(
                            ids.SelectMany(x => x.Value).Distinct().ToList(), unitOfWork)
                        .ForEach(x => languages.Add(x.Key, x.Value));
                    break;
                case DomainEnum.Channels:
                    GetEntityLanguages<ServiceChannelVersioned, ServiceChannelLanguageAvailability
                    >(ids.Keys.ToList(), unitOfWork).ForEach(x => languages.Add(x.Key, x.Value));
                    GetEntityLanguages<ServiceVersioned, ServiceLanguageAvailability>(ids
                        .SelectMany(x => x.Value)
                        .Distinct().ToList(), unitOfWork).ForEach(x => languages.Add(x.Key, x.Value));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mainType), mainType, null);
            }
            
            foreach (var connection in ids)
            {
                foreach (var child in connection.Value)
                {
                    if (languages.ContainsKey(connection.Key) && languages.ContainsKey(child) &&
                        !languages[connection.Key].Intersect(languages[child]).Any())
                    {
                        result.Add((connection.Key,child));
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// return languages for root ids of entities
        /// </summary>
        /// <param name="serviceUnificRootIds"></param>
        /// <param name="unitOfWork"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TLanguageAvail"></typeparam>
        /// <returns></returns>
        public Dictionary<Guid, List<String>> GetEntityLanguages<TEntity, TLanguageAvail>(List<Guid> serviceUnificRootIds, IUnitOfWork unitOfWork) 
            where TEntity : class, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvail>, new()
            where TLanguageAvail : class, ILanguageAvailability

        {
            var allowedPublishingStatusTypes = new List<Guid>
            {
                typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString()),
                typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString()),
                typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString())
            };
            var publishingStatusId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var draftStatusId =typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            var modifiedStatusId =typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            var serviceRepo = unitOfWork.CreateRepository<IRepository<TEntity>>();
              
            return serviceRepo.All()
                .Include(j => j.LanguageAvailabilities)
                .Where(x => serviceUnificRootIds.Contains(x.UnificRootId) && allowedPublishingStatusTypes.Contains(x.PublishingStatusId))
                .ToList()
                .GroupBy(x => x.UnificRootId).Select(x =>
                    x.OrderBy(y =>
                        y.PublishingStatusId == publishingStatusId ? 0 :
                        y.PublishingStatusId == draftStatusId ? 1 :
                        y.PublishingStatusId == modifiedStatusId ? 2 : 3).FirstOrDefault())
                .ToDictionary(x => x.UnificRootId, y => y.LanguageAvailabilities.Select(z=>languageCache.GetByValue(z.LanguageId)).ToList());
            
        }
    }

}
