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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.Publishing;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;

namespace PTV.Database.DataAccess.Services.Publishing
{
    [RegisterService(typeof(IBasePublishingChecker<ServiceVersioned>), RegisterType.Transient)]
    internal class ServiceVersionedPublishingChecker : BasePublishingChecker<ServiceVersioned>
    {
        private readonly ITypesCache typesCache;
        
        public ServiceVersionedPublishingChecker(ICacheManager cacheManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override ServiceVersioned FetchEntity(Guid id, IUnitOfWork unitOfWork)
        {
            return GetEntity<ServiceVersioned>(id, unitOfWork,
                q => q.Include(x => x.ServiceDescriptions));
        }

        public override bool ValidateEntity()
        {
            NotEmptyGuid(x => x.FundingTypeId); //TODO Add check fundingTypes
            NotEmptyString(x => x.ServiceDescriptions.Where(y => y.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString())).Select(y => y.Description).FirstOrDefault());
            NotEmptyString(x => x.ServiceDescriptions.Where(y => y.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString())).Select(y => y.Description).FirstOrDefault());
            NotEmptyList(x => x.ServiceLanguages);
            NotEmptyList(x => x.ServiceTargetGroups);
            return true;
        }

    }
}
