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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums.Security;
using PTV.Domain.Model.Models.Security;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services
{
    /// <summary>
    /// Manager for handling lock of any entity, no relation needed
    /// </summary>
    [RegisterService(typeof(ILockingManager), RegisterType.Transient)]
    internal class LockingManager : ILockingManager
    {
        private readonly IContextManager contextManager;
        private readonly IUserIdentification userIdentification;
        private const int entityLockTimeMinutes = 60;

        public LockingManager(IContextManager contextManager, IUserIdentification userIdentification)
        {
            this.contextManager = contextManager;
            this.userIdentification = userIdentification;
        }

        /// <summary>
        /// Lock specified entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Result of locking if succeeded or failed</returns>
        public EntityLockResult LockEntity(IEntityIdentifier entity)
        {
            return LockEntity(entity.Id, entity.GetType().Name);
        }

        /// <summary>
        /// Lock specified entity identified by Id
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which should be locked</typeparam>
        /// <param name="id">Id of entity which should be locked</param>
        /// <returns>Result of locking if succeeded or failed</returns>
        public EntityLockResult LockEntity<TEntityType>(Guid id) where TEntityType : IEntityIdentifier
        {
            return LockEntity(id, typeof(TEntityType).Name);
        }

        /// <summary>
        /// Lock specified entity identified by Id
        /// </summary>
        /// <param name="id">Id of entity</param>
        /// <param name="tableName">Table name of entity</param>
        /// <returns></returns>
        private EntityLockResult LockEntity(Guid id, string tableName)
        {
            var userName = userIdentification.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                return new EntityLockResult(EntityLockEnum.LockFailNoUser);
            }
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var lockingRep = unitOfWork.CreateRepository<ILockingRepository>();
                var entityLock = lockingRep.All().FirstOrDefault(i => i.LockedEntityId == id);
                if (entityLock == null)
                {
                    lockingRep.Add(new Model.Models.Locking() { Id = Guid.NewGuid(), LockedEntityId = id, LockedBy = userName, LockedAt = DateTime.UtcNow, TableName = tableName });
                    unitOfWork.Save();
                    return new EntityLockResult(EntityLockEnum.LockedForCurrent, userName);
                }
                if (entityLock.LockedAt.AddMinutes(entityLockTimeMinutes) < DateTime.UtcNow || entityLock.LockedBy == userName)
                {
                    entityLock.LockedAt = DateTime.UtcNow;
                    entityLock.LockedBy = userName;
                    unitOfWork.Save();
                    return new EntityLockResult(EntityLockEnum.LockedForCurrent, userName);
                }
                if (entityLock.LockedBy != userName)
                {
                    return new EntityLockResult(EntityLockEnum.LockedForAnother, entityLock.LockedBy);
                }
                return new EntityLockResult(EntityLockEnum.LockedForCurrent, userName);
            });
        }

        /// <summary>
        /// Unlock specified entity
        /// </summary>
        /// <param name="entity">Reference to entity which should be unlocked</param>
        /// <returns>Result of unlocking if succeeded or failed</returns>
        public EntityLockEnum UnLockEntity(EntityIdentifierBase entity)
        {
            return UnLockEntity(entity.Id);
        }

        /// <summary>
        /// Unlock specified entity
        /// </summary>
        /// <param name="id">Id of entity which should be unlocked</param>
        /// <returns>Result of unlocking if succeeded or failed</returns>
        public EntityLockEnum UnLockEntity(Guid id)
        {
            var userName = userIdentification.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                return EntityLockEnum.LockFailNoUser;
            }
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var result = UnLockEntity(unitOfWork, id);
                unitOfWork.Save();
                return result;
            });
        }

        /// <summary>
        /// Unlock specified entity
        /// </summary>
        /// <param name="id">Id of entity which should be unlocked</param>
        /// <param name="unitOfWork">unitOfWork</param>
        /// <returns>Result of unlocking if succeeded or failed</returns>
        public EntityLockEnum UnLockEntity(IUnitOfWorkWritable unitOfWork, Guid id)
        {
            var userName = userIdentification.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                return EntityLockEnum.LockFailNoUser;
            }
            var lockingRep = unitOfWork.CreateRepository<ILockingRepository>();
            var entityLock = lockingRep.All().FirstOrDefault(i => i.LockedEntityId == id);
            if (entityLock != null)
            {
                if (entityLock.LockedBy == userName || (entityLock.LockedAt.AddMinutes(entityLockTimeMinutes) < DateTime.UtcNow))
                {
                    lockingRep.Remove(entityLock);
                    return EntityLockEnum.Unlocked;
                }
                return EntityLockEnum.LockedForAnother;
            }
            return EntityLockEnum.Unlocked;
        }

        public Dictionary<Guid, EntityLockResult> UnLockEntities(IUnitOfWorkWritable unitOfWork, IList<Guid> ids)
        {
            ids = ids.Distinct().ToList();
            var userName = userIdentification.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                return ids.ToDictionary(i => i, i => new EntityLockResult(EntityLockEnum.LockFailNoUser));
            }
            var lockingRep = unitOfWork.CreateRepository<ILockingRepository>();
            var idsNullable = ids.Cast<Guid?>().ToList();
            var entityLocks = lockingRep.All().Where(i => idsNullable.Contains(i.LockedEntityId)).ToList();
            var currentTime = DateTime.UtcNow;
            return ids.ToDictionary(i => i, i =>
            {
                var lockInfo = entityLocks.FirstOrDefault(j => j.LockedEntityId == i);
                if (lockInfo == null) return new EntityLockResult(EntityLockEnum.Unlocked);
                if (lockInfo.LockedBy == userName || (lockInfo.LockedAt.AddMinutes(entityLockTimeMinutes) < currentTime))
                {
                    lockingRep.Remove(lockInfo);
                    return new EntityLockResult(EntityLockEnum.Unlocked);
                }
                return new EntityLockResult(EntityLockEnum.LockedForAnother, lockInfo.LockedBy);
            });
        }

        /// <summary>
        /// Check if entity is locked by another user
        /// </summary>
        /// <param name="entity">Entity to check locking status</param>
        /// <returns>True if locked by another user, false if not locked or locked by current user</returns>
        public bool IsLocked(EntityIdentifierBase entity)
        {
            return IsLocked(entity.Id);
        }

        /// <summary>
        /// Check if entity is locked by another user
        /// </summary>
        /// <param name="id">Id of entity which should be checked</param>
        /// <returns>True if locked by another user, false if not locked or locked by current user</returns>
        public bool IsLocked(Guid id)
        {
            return contextManager.ExecuteReader(unitOfWork => IsLocked(unitOfWork, id));
        }

        /// <summary>
        /// Check if entity is locked by another user
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="id">Id of entity which should be checked</param>
        /// <returns>True if locked by another user, false if not locked or locked by current user</returns>
        public bool IsLocked(IUnitOfWork unitOfWork, Guid id)
        {
            var userName = userIdentification.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                return true;
            }
            
            var currentTime = DateTime.UtcNow;
            var lockingRep = unitOfWork.CreateRepository<ILockingRepository>();
            return lockingRep.All().Any(i => i.LockedEntityId == id && i.LockedBy != userName && (i.LockedAt.AddMinutes(entityLockTimeMinutes) > currentTime));
        }


        /// <summary>
        /// Check if list of ids of entities is locked by another user
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Dictionary<Guid, bool> IsLocked(IUnitOfWork unitOfWork, List<Guid> ids)
        {
            var currentTime = DateTime.UtcNow;
            var lockingRep = unitOfWork.CreateRepository<ILockingRepository>();
            var userName = userIdentification.UserName;
            
            var result = ids.Distinct().ToDictionary(x => x, y => false);
            if (string.IsNullOrEmpty(userName))
            {
                return result;
            }
            
            var isLockedList = lockingRep
                .All()
                .Where(i => ids.Contains(i.LockedEntityId.Value) && i.LockedBy != userName && (i.LockedAt.AddMinutes(entityLockTimeMinutes) > currentTime))
                .Select(y => y.LockedEntityId)
                .ToList();

            if (isLockedList.Any())
            {
                isLockedList.ForEach(x =>
                {
                    if (x.HasValue && result.ContainsKey(x.Value))
                    result[x.Value] = true;
                });
            };

            return result;
        }

        /// <summary>
        /// Chceck if entity is locked
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Entity lock result</returns>
        public EntityLockResult IsLockedBy(EntityIdentifierBase entity)
        {
            return IsLockedBy(entity.Id);
        }

        /// <summary>
        /// Chceck if entity is locked
        /// </summary>
        /// <param name="id">Id of entity</param>
        /// <returns>Entity lock result</returns>
        public EntityLockResult IsLockedBy(Guid id)
        {
            var userName = userIdentification.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                return new EntityLockResult(EntityLockEnum.LockFailNoUser);
            }
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var lockingRep = unitOfWork.CreateRepository<ILockingRepository>();
                var result = lockingRep.All().FirstOrDefault(i => i.LockedEntityId == id);
                if (result != null && result.LockedAt.AddMinutes(entityLockTimeMinutes) > DateTime.UtcNow)
                {
                    return new EntityLockResult(result.LockedBy != userName ? EntityLockEnum.LockedForAnother : EntityLockEnum.LockedForCurrent, result.LockedBy);
                }
                return new EntityLockResult(EntityLockEnum.Unlocked, userName);
            });
        }
    }

    /// <summary>
    /// Result holder of locking/unlocking action
    /// </summary>
    public class EntityLockResult
    {
        public EntityLockResult(EntityLockEnum status, string userName = null)
        {
            this.LockStatus = status;
            this.LockedBy = userName;
        }
        public EntityLockEnum LockStatus { get; private set; }
        public string LockedBy { get; private set; }
    }
}
