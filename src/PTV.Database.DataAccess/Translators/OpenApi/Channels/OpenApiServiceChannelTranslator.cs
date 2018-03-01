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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Linq;
using System;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using System.Collections;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    internal abstract class OpenApiServiceChannelBaseTranslator<TVmOpenApiServiceChannel> : Translator<ServiceChannelVersioned, TVmOpenApiServiceChannel> where TVmOpenApiServiceChannel : class, IVmOpenApiServiceChannel
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        protected OpenApiServiceChannelBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override TVmOpenApiServiceChannel TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            return CreateChannelDefinitions(entity)
                .GetFinal();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(TVmOpenApiServiceChannel vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceChannelTranslator!");
        }

        protected ITranslationDefinitions<ServiceChannelVersioned, TVmOpenApiServiceChannel> CreateChannelDefinitions(ServiceChannelVersioned entity)
        {
            var commonForAllId = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.CommonForAll.ToString());
            var aiType = entity.AreaInformationTypeId.IsAssigned() ? typesCache.GetByValue<AreaInformationType>(entity.AreaInformationTypeId.Value) : null;

            return CreateEntityViewModelDefinition(entity)
                // We have to use unique root id as id!
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddCollection(i => i.ServiceChannelNames, o => o.ServiceChannelNames)
                .AddNavigation(i => typesCache.GetByValue<ServiceChannelType>(i.TypeId), o => o.ServiceChannelType)
                .AddCollection(i => i.ServiceChannelDescriptions, o => o.ServiceChannelDescriptions)
                .AddCollection(i => i.Languages.OrderBy(j => j.Order).Select(j => j.Language.Code), o => o.Languages)
                .AddCollection(i => i.WebPages, o => o.WebPages)
                .AddCollection(i => i.ServiceChannelServiceHours.Select(x => x.ServiceHours).ToList().OrderBy(x => x.OrderNumber), o => o.ServiceHours)
                // For support phones we only match items with type Phone.
                .AddCollection(i => i.Phones.Where(p => p.Phone.TypeId == typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString())).Select(p => p.Phone).OrderBy(p => p.OrderNumber).ToList(), o => o.SupportPhones)
                .AddCollection(i => i.Emails.Select(e => e.Email).OrderBy(e => e.OrderNumber), o => o.SupportEmails)
                .AddNavigation(i => typesCache.GetByValue<PublishingStatusType>(i.PublishingStatusId), o => o.PublishingStatus)
                .AddSimple(i => i.ConnectionTypeId == commonForAllId ? true : false, o => o.IsVisibleForAll)
                .AddCollection(i => i.LanguageAvailabilities.Select(l => languageCache.GetByValue(l.LanguageId)), o => o.AvailableLanguages)
                // Area information types changed into Nationwide, NationwideExceptAlandIslands and LimitedType (PTV-2184)
                .AddNavigation(i => string.IsNullOrEmpty(aiType) ? null : aiType.GetOpenApiEnumValue<AreaInformationTypeEnum>(), o => o.AreaType)
                .AddCollection(i => i.AreaMunicipalities.Select(m => m.Municipality), o => o.AreaMunicipalities)
                .AddCollection(i => i.Areas.Select(a => a.Area), o => o.Areas)
                .AddCollection(i => i.UnificRoot?.ServiceServiceChannels, o => o.Services)
                .AddSimple(i => i.Modified, o => o.Modified);
        }
    }

    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmOpenApiServiceChannel>), RegisterType.Transient)]
    internal class OpenApiServiceChannelTranslator : OpenApiServiceChannelBaseTranslator<VmOpenApiServiceChannel>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        public OpenApiServiceChannelTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives, cacheManager)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmOpenApiServiceChannel TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            var definition = base.CreateChannelDefinitions(entity);

            var type = typesCache.GetByValue<ServiceChannelType>(entity.TypeId);
            if (type == ServiceChannelTypeEnum.EChannel.ToString())
            {
                return definition.AddSimple(i => i.ElectronicChannels.FirstOrDefault()?.Id, o => o.ChannelId)
                    .GetFinal();
            }
            if (type == ServiceChannelTypeEnum.WebPage.ToString())
            {
                return definition.AddSimple(i => i.WebpageChannels.FirstOrDefault()?.Id, o => o.ChannelId)
                    .GetFinal();
            }
            if (type == ServiceChannelTypeEnum.PrintableForm.ToString())
            {
                return definition.AddSimple(i => i.PrintableFormChannels.FirstOrDefault()?.Id, o => o.ChannelId)
                    .GetFinal();
            }
            if (type == ServiceChannelTypeEnum.ServiceLocation.ToString())
            {
                return definition.AddSimple(i => i.ServiceLocationChannels.FirstOrDefault()?.Id, o => o.ChannelId)
                    .GetFinal();
            }

            return definition.GetFinal();
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmOpenApiServiceChannel vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceChannelTranslator!");
        }
    }
}
