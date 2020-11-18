﻿/**
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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums.Security;

namespace PTV.Database.DataAccess.Services
{
    /// <summary>
    /// Manager for handling lock of any entity, no relation needed
    /// </summary>
    internal interface ILockingManager
    {
        EntityLockResult LockEntity(IEntityIdentifier entity);
        EntityLockResult LockEntity<TEntityType>(Guid id) where TEntityType : IEntityIdentifier;
        EntityLockEnum UnLockEntity(EntityIdentifierBase entity);
        EntityLockEnum UnLockEntity(Guid id);
        Dictionary<Guid, EntityLockResult> UnLockEntities(IUnitOfWorkWritable unitOfWork, IList<Guid> ids);

        bool IsLocked(EntityIdentifierBase entity);
        bool IsLocked(Guid id);
        EntityLockResult IsLockedBy(EntityIdentifierBase entity);
        EntityLockResult IsLockedBy(Guid id);

        /// <summary>
        /// Check if list of ids of entities is locked by another user
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        Dictionary<Guid, bool> IsLocked(IUnitOfWork unitOfWork, List<Guid> ids);
    }
}
