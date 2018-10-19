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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Framework;
using Microsoft.AspNetCore.Http;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Database.Model.Interfaces;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Domain.Model.Enums.Security;

namespace PTV.Database.DataAccess.Services.Security
{
    /// <summary>
    /// Service collection role checker
    /// </summary>
    [RegisterService(typeof(ServiceCollectionRoleChecker), RegisterType.Transient)]
    internal class ServiceCollectionRoleChecker : RoleCheckerBase
    {
        private readonly ITypesCache typesCache;
        private readonly IRoleCheckerManager roleChecker;

        public ServiceCollectionRoleChecker(IRoleCheckerManager roleChecker, ITypesCache typesCache, IUserOrganizationService userOrganizationService, IUserInfoService userInfoService)
        : base(userOrganizationService, userInfoService, typesCache)
        {
            this.typesCache = typesCache;
            this.roleChecker = roleChecker;
        }

        protected override DomainEnum Domain => DomainEnum.Services;

        public override void CheckEntity(IRoleBased entity, EntityEntry<IRoleBased> entityEntry, IUnitOfWorkCachedSearching unitOfWork)
        {
            var serviceCollection = entity as ServiceCollection;
            if (serviceCollection == null) return;

            var serviceCollectionVersioned = GetServiceCollectionVersioned(serviceCollection, unitOfWork);
            if (serviceCollectionVersioned != null)
            {
                roleChecker.CheckEntity(serviceCollectionVersioned, null, unitOfWork);
            }
        }

        private ServiceCollectionVersioned GetServiceCollectionVersioned(ServiceCollection service, IUnitOfWorkWritable unitOfWork)
        {
            var serviceCollectionVersionedRep = unitOfWork.CreateRepository<IServiceCollectionVersionedRepository>();
            var serviceCollectionVersionedList = serviceCollectionVersionedRep.All().Where(sv => sv.UnificRootId == service.Id);
            if (!serviceCollectionVersionedList.Any()) return null;

            // get published service 
            var psPublished = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
            var serviceCollectionVersioned = serviceCollectionVersionedList.FirstOrDefault(sv => sv.PublishingStatusId == psPublished);
            if (serviceCollectionVersioned != null) return serviceCollectionVersioned;

            // get modified service
            var psModified = typesCache.Get<PublishingStatusType>(PublishingStatus.Modified.ToString());
            serviceCollectionVersioned = serviceCollectionVersionedList.FirstOrDefault(sv => sv.PublishingStatusId == psModified);
            if (serviceCollectionVersioned != null) return serviceCollectionVersioned;

            // get draft service
            var psDraft = typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString());
            return serviceCollectionVersionedList.FirstOrDefault(sv => sv.PublishingStatusId == psDraft);
        }
    }
}
