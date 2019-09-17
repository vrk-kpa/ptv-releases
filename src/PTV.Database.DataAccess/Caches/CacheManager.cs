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
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums.Security;
using PTV.Framework;
using PTV.Framework.Enums;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Caches
{
    [RegisterService(typeof(ICacheManager), RegisterType.Singleton)]
    internal class CacheManager : ICacheManager
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;
        private readonly IPublishingStatusCache publishingStatusCache;
        private readonly ILanguageOrderCache languageOrderCache;
        private readonly IUserAccessRightsCache userAccessRightsCache;
        private readonly ILanguageStateCultureCache languageStateCultureCache;
        private readonly IResolveManager resolveManager;
        private readonly IEntityTreesCache entityTreesCache;
        private IPostalCodeCache postalCodeCache;

        public CacheManager(ILanguageCache languageCache,
            IPublishingStatusCache publishingStatusCache,
            ITypesCache typesCache,
            ILanguageOrderCache languageOrderCache,
            IUserAccessRightsCache userAccessRightsCache,
            ILanguageStateCultureCache languageStateCultureCache,
            IResolveManager resolveManager,
            IEntityTreesCache entityTreesCache,
            IPostalCodeCache postalCodeCache)
        {
            this.languageCache = languageCache;
            this.publishingStatusCache = publishingStatusCache;
            this.typesCache = typesCache;
            this.languageOrderCache = languageOrderCache;
            this.userAccessRightsCache = userAccessRightsCache;
            this.languageStateCultureCache = languageStateCultureCache;
            this.resolveManager = resolveManager;
            this.entityTreesCache = entityTreesCache;
            this.postalCodeCache = postalCodeCache;
            Init();
        }

        public ILanguageCache LanguageCache => languageCache;
        public IPostalCodeCache PostalCodeCache => postalCodeCache;
        public ILanguageOrderCache LanguageOrderCache => languageOrderCache;

        public IPublishingStatusCache PublishingStatusCache => publishingStatusCache;

        public ITypesCache TypesCache => typesCache;
        public IEntityTreesCache EntityTreesCache => entityTreesCache;

        public IUserAccessRightsCache UserAccessRightsCache => userAccessRightsCache;

        public ILanguageStateCultureCache LanguageStateCultureCache => languageStateCultureCache;

        private void Init()
        {
            var contextManager = resolveManager.Resolve<IContextManager>();
            contextManager.ExecuteReader(unitOfWork =>
            {
                var rep = unitOfWork.CreateRepository<IUserAccessRightsGroupRepository>();
                var data = rep.All().ToList();
                var storeData = data.ToDictionary(i => i.Code.ToLower(),
                    i => new VmUserAccessRights() { EntityId = i.Id, GroupCode = i.Code, UserRole = Enum.Parse<UserRoleEnum>(i.UserRole, true), AccessRights = (AccessRightEnum)i.AccessRightFlag });
                UserAccessRightsCache.Init(storeData);
            });
        }
    }
    
    internal interface ICacheManager
    {
        ILanguageCache LanguageCache { get; }
        ILanguageOrderCache LanguageOrderCache { get; }
        IPublishingStatusCache PublishingStatusCache { get; }
        ITypesCache TypesCache { get; }
        IEntityTreesCache EntityTreesCache { get; }
        IUserAccessRightsCache UserAccessRightsCache { get; }

        ILanguageStateCultureCache LanguageStateCultureCache { get; }
        IPostalCodeCache PostalCodeCache { get; }
    }
}
