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
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Repositories;

namespace PTV.Database.DataAccess.Services.Validation
{
    [RegisterService(typeof(ILoadingValidationChecker<ServiceCollectionVersioned>), RegisterType.Transient)]
    internal class ServiceCollectionValidationChecker : BaseLoadingValidationChecker<ServiceCollectionVersioned>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private IVersioningManager VersioningManager;
        private IUnitOfWork unitOfWork;

        public ServiceCollectionValidationChecker(ICacheManager cacheManager, IResolveManager resolveManager) : base(cacheManager, resolveManager)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
            this.VersioningManager = resolveManager.Resolve<IVersioningManager>();
        }

        protected override ServiceCollectionVersioned FetchEntity(Guid id, IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            return GetEntity<ServiceCollectionVersioned>(id, unitOfWork,
                q => q.Include(i => i.ServiceCollectionNames)
                        .Include(i => i.LanguageAvailabilities)                                             
                );
        }

        protected override void ValidateEntityInternal(Guid? language)
        {

            foreach (var entitylanguageId in entityOrPublishedLanguagesAvailabilityIds)
            {
                SetValidationLanguage(entitylanguageId);

                CheckEntityWithMergeResult<Organization>(entity.OrganizationId, unitOfWork);
                                
                NotEmptyString("name", x => x.ServiceCollectionNames
                    .Where(y => y.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()) && y.LocalizationId == entitylanguageId)
                    .Select(y => y.Name)
                    .FirstOrDefault());
            }
        }
    }
}
