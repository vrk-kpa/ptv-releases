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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Services.Validation
{
    [RegisterService(typeof(ILoadingValidationChecker<List<ServiceHours>>), RegisterType.Transient)]
    internal class ServiceHoursListValidationChecker : BaseLoadingValidationChecker<List<ServiceHours>>
    {
        private readonly ITypesCache typesCache;

        public ServiceHoursListValidationChecker(ICacheManager cacheManager, IResolveManager resolveManager) : base(cacheManager, resolveManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        protected override List<ServiceHours> FetchEntity(Guid id, IUnitOfWork unitOfWork)
        {
            var rep = unitOfWork.CreateRepository<IServiceChannelServiceHoursRepository>();
            var serviceHours = rep.All()
                .Where
                (
                    x => x.ServiceChannelVersionedId == id &&
                         x.ServiceHours.ServiceHourTypeId == typesCache.Get<ServiceHourType>(ServiceHoursTypeEnum.Standard.ToString()) &&
                         !x.ServiceHours.DailyOpeningTimes.Any()
                )
                .Select(x => x.ServiceHours).ToList();
            // if organization is ok (exists published organization), return something, otherwise returns null
            return serviceHours;
        }

        protected override void ValidateEntityInternal(Guid? language)
        {
            entity.ForEach(sh => CheckEntityWithMergeResult(sh));
        }

        protected override ValidationPath GetCurrentPath(ValidationPath defaultPath)
        {
            return null;
        }
    }
}
