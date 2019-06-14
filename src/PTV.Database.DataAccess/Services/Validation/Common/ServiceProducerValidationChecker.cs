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
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Services.Validation
{
    [RegisterService(typeof(ILoadingValidationChecker<ServiceProducer>), RegisterType.Transient)]
    internal class ServiceProducerValidationChecker : BaseLoadingValidationChecker<ServiceProducer>
    {
        private readonly ITypesCache typesCache;
        private IUnitOfWork unitOfWork;

        public ServiceProducerValidationChecker(ICacheManager cacheManager, IResolveManager resolveManager) : base(cacheManager, resolveManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        protected override ServiceProducer FetchEntity(Guid id, IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            return GetEntity<ServiceProducer>(id, unitOfWork,
                x => x.Include(i => i.Organizations)
                      .Include(i => i.AdditionalInformations));
        }

        protected override void ValidateEntityInternal(Guid? language)
        {
            if (!language.IsAssigned())
            {
                throw new ArgumentNullException(nameof(language), "language must be defined.");
            }

            var provisionType = typesCache.GetByValue<ProvisionType>(entity.ProvisionTypeId);
            var provisionTypeEnum = Enum.Parse(typeof(ProvisionTypeEnum), provisionType);

           foreach (var serviceProducerOrganization in entity.Organizations)
            {
                CheckEntityWithMergeResult<Organization>(serviceProducerOrganization.OrganizationId, unitOfWork);               
            }
           

            switch (provisionTypeEnum)
            {
                case ProvisionTypeEnum.SelfProduced:
                    NotBeTrue("selfProducers", x => !x.Organizations.Any());
                    break;

                case ProvisionTypeEnum.PurchaseServices:
                case ProvisionTypeEnum.Other:

                    if (NotEmptyString(x => x.AdditionalInformations
                            .Where(y => y.LocalizationId == language)
                            .Select(y => y.Text).FirstOrDefault()) &&
                        !entity.Organizations.Any())
                    {
                        AddValidationMessageToDictionary("additionalInformation");
                        AddValidationMessageToDictionary("organization");
                    }

                    break;
                default:
                    break;
            }
        }
    }
}
