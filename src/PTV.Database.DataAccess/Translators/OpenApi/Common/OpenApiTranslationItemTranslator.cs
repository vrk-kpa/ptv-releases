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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<Model.Models.TranslationOrder, VmOpenApiTranslationItem>), RegisterType.Transient)]
    internal class OpenApiTranslationItemTranslator : Translator<Model.Models.TranslationOrder, VmOpenApiTranslationItem>
    {
        private ILanguageCache languageCache;
        private ITypesCache typesCache;

        public OpenApiTranslationItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiTranslationItem TranslateEntityToVm(Model.Models.TranslationOrder entity)
        {
            if (entity == null) return null;
            string itemType = null;
            Guid? itemId = null;
            if (entity.ServiceTranslationOrders?.Count > 0)
            {
                itemType = typeof(Service).Name;
                itemId = entity.ServiceTranslationOrders.OrderByDescending(x => x.Created).First().ServiceId;
            }
            else if (entity.ServiceChannelTranslationOrders?.Count > 0)
            {
                itemType = typeof(ServiceChannel).Name;
                itemId = entity.ServiceChannelTranslationOrders.OrderByDescending(x => x.Created).First().ServiceChannelId;
            }

            var definitions = CreateEntityViewModelDefinition(entity)
                // We have to use unique root id for the service!
                .AddSimple(i => i.OrganizationIdentifier, o => o.OrganizationId)
                .AddNavigation(i => i.OrganizationName, o => o.OrganizationName)
                .AddNavigation(i => i.OrganizationBusinessCode, O => O.BusinessCode)
                .AddNavigation(i => i.SenderEmail, o => o.Orderer)
                .AddSimple(i => i.OrderIdentifier, o => o.OrderId)
                .AddNavigation(i => languageCache.GetByValue(i.SourceLanguageId), o => o.SourceLanguage)
                .AddSimple(i => i.SourceLanguageCharAmount, o => o.SourceLanguageCharAmount)
                .AddNavigation(i => languageCache.GetByValue(i.TargetLanguageId), o => o.TargetLanguage)
                .AddNavigation(i => itemType, o => o.Type)
                .AddSimple(i => itemId, o => o.ItemId)
                .AddNavigation(i => i.SourceEntityName, o => o.ItemName);

            if (entity.TranslationOrderStates?.Count > 0)
            {
                definitions.AddNavigation(i => typesCache.GetByValue<TranslationStateType>(i.TranslationOrderStates.OrderByDescending(x => x.SendAt).First().TranslationStateId), o => o.OrderState);
                // Order date - the date when order has been sent
                var orderDateItem = entity.TranslationOrderStates.FirstOrDefault(i => i.TranslationStateId == typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.Sent.ToString()));
                if (orderDateItem != null)
                {
                    definitions.AddSimple(i => orderDateItem.SendAt, o => o.OrderDate);
                }

                // Order resolved date - the date when order has been received from translation company (arrived)
                var resolvedDateItem = entity.TranslationOrderStates.FirstOrDefault(i => i.TranslationStateId == typesCache.Get<TranslationStateType>(TranslationStateTypeEnum.Arrived.ToString()));
                if (resolvedDateItem != null)
                {
                    definitions.AddSimple(i => resolvedDateItem.SendAt, o => o.OrderResolvedDate);
                }
            }
            return definitions.GetFinal();
        }

        public override Model.Models.TranslationOrder TranslateVmToEntity(VmOpenApiTranslationItem vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiTranslationItemTranslator.");
        }
    }
}
