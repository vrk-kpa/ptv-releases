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
* THE SOFTWARE.C:\Projects\PTV_TEST\src\PTV.Database.DataAccess\Services\Security\
*/
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Services.Validation.Common
{
    [RegisterService(typeof(ILoadingValidationChecker<Organization>), RegisterType.Transient)]
    internal class OrganizationStatusValidationChecker : BaseLoadingValidationChecker<OrganizationVersioned>, ILoadingValidationChecker<Organization>
    {
        private readonly ITypesCache typesCache;

        public OrganizationStatusValidationChecker(ICacheManager cacheManager, IResolveManager resolveManager) : base(cacheManager, resolveManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        protected override OrganizationVersioned FetchEntity(Guid id, IUnitOfWork unitOfWork)
        {
            var rep = unitOfWork.CreateRepository<IOrganizationVersionedRepository>();
            var organization = rep.All().FirstOrDefault(x => x.UnificRootId == id && x.PublishingStatusId == typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString()));
            // if organization is ok (exists published organization), return something, otherwise returns null
            return organization;
        }

        protected override void ValidateEntityInternal(Guid? language)
        {
            // check if published organization exists
            NotBeTrue("organization", x => x == null, ValidationErrorTypeEnum.PublishedMandatoryField);
        }

        protected override ValidationPath GetCurrentPath(ValidationPath defaultPath)
        {
            return null;
        }
    }
}
