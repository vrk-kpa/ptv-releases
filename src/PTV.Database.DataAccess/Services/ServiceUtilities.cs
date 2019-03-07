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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.ServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Interfaces.Security;
using PTV.Domain.Model.Models.Security;
using PTV.Domain.Model.Models.V2.Service;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Services.V2;
using PTV.Domain.Model.Enums;
using PTV.Framework.Extensions;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Domain.Model;

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
        IVmEntityBase CheckIsEntityEditable<T, T2>(Guid id) where T : class, IEntityIdentifier, IVersionedVolume<T2> where T2 : IVersionedRoot;
        IVmEntityBase CheckIsEntityEditable<T, T2>(Guid id, IUnitOfWork unitOfWork) where T : class, IEntityIdentifier, IVersionedVolume<T2> where T2 : IVersionedRoot;
        IVmEntityLockBase EntityLockedBy(Guid id);
        
        /// <summary>
        /// Check Id according to global OID regex
        /// </summary>
        /// <param name="id">OID</param>
        void CheckIdFormat(string id);
    }

    [RegisterService(typeof(IServiceUtilities), RegisterType.Transient)]
    internal class ServiceUtilities : IServiceUtilities
    {
        private readonly IUserIdentification userIdentification;
        private readonly ILockingManager lockingManager;
        private readonly IContextManager contextManager;
        private readonly IUserOrganizationService userOrganizationService;
        private IVersioningManager versioningManager;
        private readonly IUserInfoService userInfoService;
        private IUserOrganizationChecker userOrganizationChecker;
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        //private readonly ITranslationService translationService;
        
        public ServiceUtilities
        (
            IUserIdentification userIdentification,
            ILockingManager lockingManager,
            IContextManager contextManager,
            IUserOrganizationService userOrganizationService,
            IVersioningManager versioningManager,
            IUserInfoService userInfoService,
            //ITranslationService translationService,
            IUserOrganizationChecker userOrganizationChecker,
            ICacheManager cacheManager)
        {
            this.userIdentification = userIdentification;
            this.lockingManager = lockingManager;
            this.contextManager = contextManager;
            this.userOrganizationService = userOrganizationService;
            this.versioningManager = versioningManager;
            this.userInfoService = userInfoService;
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

        public IList<Guid> GetAllUserOrganizations()
        {
            return userOrganizationService.GetAllUserOrganizationIds();
        }

        /// <summary>
        /// Get the identification of current user for ExternalSource table.
        /// </summary>
        /// <returns></returns>
        public string GetRelationIdForExternalSource(bool throwExceptionIfNotExists = true)
        {
            var relationId = userIdentification.UserName;
            if (throwExceptionIfNotExists && string.IsNullOrEmpty(relationId))
            {
                throw new Exception(CoreMessages.OpenApi.RelationIdNotFound);
            }
            return relationId;
        }

        public WebPage GetWebPageByUrl(string url, string language, IUnitOfWork unitOfWork)
        {
            var webpageRep = unitOfWork.CreateRepository<IWebPageRepository>();
            return webpageRep.All().FirstOrDefault(webPage => webPage.Url == url && webPage.Localization.Code == language);
        }

        public IVmEntityBase LockEntityRoot<T>(Guid id) where T : IEntityIdentifier, IVersionedRoot
        {
            var lockResult = lockingManager.LockEntity<T>(id);
            if (lockResult.LockStatus != EntityLockEnum.LockedForCurrent)
            {
                throw new LockException("",new List<string>() { lockResult.LockedBy });
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
                throw new LockException("", new List<string>() { CoreMessages.EntityNotFoundToUpdate });
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
                throw new LockException("", new List<string>() { lockResult.ToString() });
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
                throw new LockException("", new List<string>() { CoreMessages.EntityNotFoundToUpdate });
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

        public IVmEntityBase CheckIsEntityEditable<T, T2>(Guid id) where T : class, IEntityIdentifier, IVersionedVolume<T2> where T2 : IVersionedRoot
        {
            return contextManager.ExecuteReader(unitOfWork => CheckIsEntityEditable<T, T2>(id, unitOfWork));
        }

        public IVmEntityBase CheckIsEntityEditable<T, T2>(Guid id, IUnitOfWork unitOfWork) where T : class, IEntityIdentifier, IVersionedVolume<T2> where T2 : IVersionedRoot
        {
            Guid? rootId = null;
            rootId = versioningManager.GetUnificRootId<T>(unitOfWork, id);

            var result = new VmEntityEditableBase();

            if (rootId.HasValue && CheckEntityAccess<T, T2>(rootId.Value, id, unitOfWork))
            {
                var lastModified = versioningManager.GetLastModifiedVersion<T>(unitOfWork, rootId.Value);
                var lastPublished = versioningManager.GetLastPublishedVersion<T>(unitOfWork, rootId.Value);
                result = new VmEntityEditableBase
                {
                    Id = id,
                    UnificRootId = rootId.Value,
                    LastModifiedId = lastModified?.EntityId,
                    ModifiedOfLastModified = lastModified?.Modified,
                    LastPublishedId = lastPublished?.EntityId,
                    ModifiedOfLastPublished = lastPublished?.Modified,
                    IsEditable = versioningManager.IsAllowedForEditing<T>(unitOfWork, id) == VersionEditableReason.Editable
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
    }
    
}
