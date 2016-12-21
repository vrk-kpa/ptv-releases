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
using System.Threading.Tasks;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.Model.Models.Base;
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
        public EntityLockResult LockEntity(EntityIdentifierBase entity)
        {
            return LockEntity(entity.Id, entity.GetType().Name);
        }

        /// <summary>
        /// Lock specified entity identified by Id
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity which should be locked</typeparam>
        /// <param name="id">Id of entity which should be locked</param>
        /// <returns>Result of locking if succeeded or failed</returns>
        public EntityLockResult LockEntity<TEntityType>(Guid id) where TEntityType : EntityIdentifierBase
        {
            return LockEntity(id, typeof(TEntityType).Name);
        }

        private EntityLockResult LockEntity(Guid id, string tableName)
        {
            var userName = userIdentification.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                return new EntityLockResult(EntityLockResultEnum.LockFailNoUser);
            }
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var lockingRep = unitOfWork.CreateRepository<ILockingRepository>();
                var entityLock = lockingRep.All().FirstOrDefault(i => i.LockedEntityId == id);
                if (entityLock == null)
                {
                    lockingRep.Add(new Model.Models.Locking() { Id = Guid.NewGuid(), LockedEntityId = id, LockedBy = userName, LockedAt = DateTime.UtcNow, TableName = tableName });
                    unitOfWork.Save();
                    return new EntityLockResult(EntityLockResultEnum.LockedNowForCurrent, userName);
                }
                if (entityLock.LockedBy != userName)
                {
                    return new EntityLockResult(EntityLockResultEnum.AlreadyLockedForAnother, entityLock.LockedBy);
                }
                return new EntityLockResult(EntityLockResultEnum.LockedNowForCurrent, userName);
            });
        }

        /// <summary>
        /// Unlock specified entity
        /// </summary>
        /// <param name="entity">Reference to entity which should be unlocked</param>
        /// <returns>Result of unlocking if succeeded or failed</returns>
        public EntityLockResultEnum UnLockEntity(EntityIdentifierBase entity)
        {
            return UnLockEntity(entity.Id);
        }

        /// <summary>
        /// Unlock specified entity
        /// </summary>
        /// <param name="id">Id of entity which should be unlocked</param>
        /// <returns>Result of unlocking if succeeded or failed</returns>
        public EntityLockResultEnum UnLockEntity(Guid id)
        {
            var userName = userIdentification.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                return EntityLockResultEnum.LockFailNoUser;
            }
            return contextManager.ExecuteWriter(unitOfWork =>
            {
                var lockingRep = unitOfWork.CreateRepository<ILockingRepository>();
                var entityLock = lockingRep.All().FirstOrDefault(i => i.LockedEntityId == id);
                if (entityLock != null)
                {
                    if (entityLock.LockedBy == userName)
                    {
                        lockingRep.Remove(entityLock);
                        unitOfWork.Save();
                        return EntityLockResultEnum.Unlocked;
                    }
                    return EntityLockResultEnum.AlreadyLockedForAnother;
                }
                return EntityLockResultEnum.Unlocked;
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
            var userName = userIdentification.UserName;
            if (string.IsNullOrEmpty(userName))
            {
                return true;
            }
            return contextManager.ExecuteReader(unitOfWork =>
            {
                var lockingRep = unitOfWork.CreateRepository<ILockingRepository>();
                return lockingRep.All().Any(i => i.LockedEntityId == id && i.LockedBy != userName);
            });
        }
    }

    /// <summary>
    /// Result holder of locking/unlocking action
    /// </summary>
    public class EntityLockResult
    {
        public EntityLockResult(EntityLockResultEnum status, string userName = null)
        {
            this.LockStatus = status;
            this.LockedBy = userName;
        }
        public EntityLockResultEnum LockStatus { get; private set; }
        public string LockedBy { get; private set; }
    }

    /// <summary>
    /// Status of lock
    /// </summary>
    public enum EntityLockResultEnum
    {
        LockedNowForCurrent,
        AlreadyLockedForAnother,
        LockFailNoUser,
        Unlocked
    }
}
