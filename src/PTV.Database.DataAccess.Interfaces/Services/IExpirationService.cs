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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    internal interface IExpirationService
    {
        DateTime? GetExpirationDate<TEntity>(IUnitOfWork unitOfWork, TEntity entity, DateTime? utcNow = null, DateTime? lastChangeDate = null)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IExpirable, new();

        void SetExpirationDatesForDraft<TEntity>(IUnitOfWorkWritable unitOfWork, IEnumerable<Guid> ids, bool allowAnonymous = false)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IExpirable, new();

        void SetExpirationDate<TEntity>(IUnitOfWork unitOfWork, TEntity entity, ActionTypeEnum actionType, bool isEeva)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IExpirable, new();

        DateTime GetNonEvaModifiedDate(DateTime modified, LastOperationType lastOperationType, Guid? versioningId, IUnitOfWork unitOfWork);

        DateTime? GetExpirationDate<TEntity>(IUnitOfWork unitOfWork, Guid entityId, DateTime? utcNow = null)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IExpirable, new();

        bool GetIsWarningVisible<TEntity, TLanguageAvailability>(IUnitOfWork unitOfWork, TEntity entity, DateTime? utcNow = null)
            where TEntity : class, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability;

        List<VmTaskEntity> GetExpirationTasks<TEntity, TLanguageAvailability>
        (IUnitOfWork unitOfWork, Guid publishingStatusId, IList<Guid> forOrganizations,
            IEnumerable<Guid> definedEntities = null, int? skip = null)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume,
            IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability;

        IEnumerable<TEntity> GetEntityIdsByExpirationDate<TEntity, TLanguageAvailability>(IUnitOfWork unitOfWork, PublishingStatus publishingStatus, DateTime utcNow)
            where TEntity : class, IAuditing, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability;

        Dictionary<Guid, VmExpirationOfEntity> GetExpirationInformation<TEntity, TLanguageAvailability>(IUnitOfWork unitOfWork, Guid unificRootId, Guid publishingStatusId, IList<Guid> forOrganizations)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability;

        void SetExpirationDateForPublishing<TEntity>(IUnitOfWorkWritable unitOfWork, IEnumerable<Guid> ids, bool isEeva, bool allowAnonymous = false)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IExpirable, new();

        void PolyfillExpirationDate<TEntity, TLanguageAvailability>(IUnitOfWorkWritable unitOfWork, TEntity entity,
            DateTime utcNow)
            where TEntity : class, IAuditing, IVersionedVolume, IMultilanguagedEntity<TLanguageAvailability>, IExpirable, new()
            where TLanguageAvailability : ILanguageAvailability;

        void SetExpirationDateForPublishing<TEntity>(IContextManager contextManager, IEnumerable<Guid> ids, bool isEeva, bool allowAnonymous = false)
            where TEntity : class, IAuditing, IOrganizationInfo, IEntityIdentifier, IVersionedVolume, IExpirable, new();
    }
}
