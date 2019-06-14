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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Database.DataAccess.Utils.OpenApi
{
    internal class TranslationPageHandler : GuidPageFilteredByDateHandlerBase<VmOpenApiTranslationItemsPage, VmOpenApiTranslationItem, TranslationOrder>
    {
        private ILanguageCache languageCache;
        private ITypesCache typesCache;
        private Dictionary<Guid, Guid> services;
        private Dictionary<Guid, Guid> channels;
        private Dictionary<Guid, List<TranslationOrderState>> translationStates;

        public TranslationPageHandler(
            ILanguageCache languageCache,
            ITypesCache typesCache,
            DateTime? date,
            DateTime? dateBefore,
            int pageNumber,
            int pageSize
            ) :base(date, dateBefore, pageNumber, pageSize)
        {
            this.languageCache = languageCache;
            this.typesCache = typesCache;
        }

        public override int Search(IUnitOfWork unitOfWork)
        {
            var count = base.Search(unitOfWork);
            if (count > 0)
            {
                // Get the related services
                services = unitOfWork.CreateRepository<IRepository<ServiceTranslationOrder>>().All().Where(o => EntityIds.Contains(o.TranslationOrderId))
                    .ToDictionary(i => i.TranslationOrderId, i => i.ServiceId);

                // Get the related channels
                channels = unitOfWork.CreateRepository<IRepository<ServiceChannelTranslationOrder>>().All().Where(o => EntityIds.Contains(o.TranslationOrderId))
                    .ToDictionary(i => i.TranslationOrderId, i => i.ServiceChannelId);

                // Get translation statuses
                translationStates = unitOfWork.CreateRepository<IRepository<TranslationOrderState>>().All().Where(o => EntityIds.Contains(o.TranslationOrderId))
                    .GroupBy(i => i.TranslationOrderId).ToDictionary(i => i.Key, i => i.OrderByDescending(x => x.SendAt).ToList());
            }

            return count;
        }

        protected override VmOpenApiTranslationItem GetItemData(TranslationOrder entity)
        {
            string itemType = null;
            Guid? itemId = services.TryGetOrDefault(entity.Id, Guid.Empty);
            if (itemId.IsAssigned())
            {
                itemType = typeof(Service).Name;
            }
            else
            {
                itemId = channels.TryGetOrDefault(entity.Id, Guid.Empty);
                if (itemId.IsAssigned())
                {
                    itemType = typeof(ServiceChannel).Name;
                }
            }

            var latestStateItem = translationStates.TryGetOrDefault(entity.Id, new List<TranslationOrderState>()).FirstOrDefault();
            // Order date - the date when order has been sent
            var orderDateItem = translationStates.TryGetOrDefault(entity.Id, new List<TranslationOrderState>()).FirstOrDefault(i => i.TranslationStateId == typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.Sent.ToString()));
            // Order resolved date - the date when order has been received from translation company (arrived)
            var resolvedDateItem = translationStates.TryGetOrDefault(entity.Id, new List<TranslationOrderState>()).FirstOrDefault(i => i.TranslationStateId == typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.Arrived.ToString()));

            return new VmOpenApiTranslationItem
            {
                OrganizationId = entity.OrganizationIdentifier,
                OrganizationName = entity.OrganizationName,
                BusinessCode = entity.OrganizationBusinessCode,
                Orderer = entity.SenderEmail,
                OrderId = entity.OrderIdentifier,
                SourceLanguage = languageCache.GetByValue(entity.SourceLanguageId),
                SourceLanguageCharAmount = entity.SourceLanguageCharAmount,
                TargetLanguage = languageCache.GetByValue(entity.TargetLanguageId),
                Type = itemType,
                ItemId = itemId.IsAssigned() ? itemId : null,
                ItemName = entity.SourceEntityName,
                OrderState = latestStateItem != null ? typesCache.GetByValue<TranslationStateType>(latestStateItem.TranslationStateId) : null,
                OrderDate = orderDateItem != null ? orderDateItem.SendAt : (DateTime?)null,
                OrderResolvedDate = resolvedDateItem != null ? resolvedDateItem.SendAt : (DateTime?)null
            };
        }
    }
}
