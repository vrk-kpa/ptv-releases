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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V4;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    internal abstract class OpenApiServiceChannelBaseTranslator<TVmOpenApiServiceChannel> : Translator<ServiceChannelVersioned, TVmOpenApiServiceChannel> where TVmOpenApiServiceChannel : class, IVmOpenApiServiceChannelBase
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
            return CreateBaseDefinitions<TVmOpenApiServiceChannel>(entity).GetFinal();
        }

        protected ITranslationDefinitions<ServiceChannelVersioned, TVmOpenApiServiceChannel> CreateBaseDefinitions<TInstantiate>(ServiceChannelVersioned entity) where TInstantiate : TVmOpenApiServiceChannel
        {
            var commonForAllId = typesCache.Get<ServiceChannelConnectionType>(ServiceChannelConnectionTypeEnum.CommonForAll.ToString());

            var resultDefinition = CreateEntityViewModelDefinition(entity)
                .AddCollection(i => i.ServiceChannelDescriptions, o => o.ServiceChannelDescriptions)
                .AddCollection(i => i.Languages.OrderBy(j => j.Order).Select(j => j.Language.Code).ToList(), o => o.Languages)
                .AddCollection(i => i.WebPages, o => o.WebPages)
                .AddCollection(i => i.ServiceHours.OrderBy(x => x.OrderNumber), o => o.ServiceHours)
                // For support phones we only match items with type Phone.
                .AddCollection(i => i.Phones.Where(p => p.Phone.TypeId == typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString())).Select(p => p.Phone).OrderBy(p => p.OrderNumber).ToList(), o => o.SupportPhones)
                .AddCollection(i => i.Emails.Select(e => e.Email).OrderBy(e => e.OrderNumber), o => o.SupportEmails)
                .AddNavigation(i => i.PublishingStatus, o => o.PublishingStatus)
                .AddSimple(i => i.ConnectionTypeId == commonForAllId ? true : false, o => o.IsVisibleForAll)
                .AddCollection(i => i.LanguageAvailabilities.Select(l => languageCache.GetByValue(l.LanguageId)).ToList(), o => o.AvailableLanguages)
                .AddNavigation(i => i.AreaInformationTypeId.IsAssigned() ? typesCache.GetByValue<AreaInformationType>(i.AreaInformationTypeId.Value) : null, o => o.AreaType);
            return resultDefinition;
        }

        public override ServiceChannelVersioned TranslateVmToEntity(TVmOpenApiServiceChannel vModel)
        {
            return CreateBaseVmEntityDefinitions(vModel).GetFinal();
        }

        protected ITranslationDefinitions<TVmOpenApiServiceChannel, ServiceChannelVersioned> CreateBaseVmEntityDefinitions(TVmOpenApiServiceChannel vModel)
        {
            if (vModel.Id.IsAssigned())
            {
                vModel.ServiceChannelDescriptions.ForEach(d => d.OwnerReferenceId = vModel.Id);
                vModel.ServiceHours.ForEach(h => h.OwnerReferenceId = vModel.Id);
                vModel.WebPages.ForEach(w => w.OwnerReferenceId = vModel.Id);
                vModel.SupportEmails.ForEach(e => e.OwnerReferenceId = vModel.Id);
                vModel.SupportPhones.ForEach(p => p.OwnerReferenceId = vModel.Id);
            }

            var definition = CreateViewModelEntityDefinition<ServiceChannelVersioned>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => i.Id.Value == o.Id)
                .UseVersioning<ServiceChannelVersioned, ServiceChannel>(o => o);

            if (vModel.ServiceChannelDescriptions != null && vModel.ServiceChannelDescriptions.Count > 0)
            {
                definition.AddCollection(i => i.ServiceChannelDescriptions, o => o.ServiceChannelDescriptions, true);
            }

            if (!string.IsNullOrEmpty(vModel.AreaType))
            {
                definition.AddSimple(i => typesCache.Get<AreaInformationType>(vModel.AreaType), o => o.AreaInformationTypeId);
            }

            if (vModel.Languages != null && vModel.Languages.Count > 0)
            {
                var languages = new List<VmOpenApiStringItem>();
                // Append ordering number for each item
                var index = 1;
                vModel.Languages.ForEach(l => languages.Add(new VmOpenApiStringItem { Value = l, OwnerReferenceId = vModel.Id, Order = index++ }));
                definition.AddCollection(i => languages, o => o.Languages);
            }
            if (vModel.WebPages != null && vModel.WebPages.Count > 0)
            {
                definition.AddCollection(i => i.WebPages, o => o.WebPages, false);
            }
            if (vModel.ServiceHours != null && vModel.ServiceHours.Count > 0)
            {
                // Set service hours order
                var sortedServiceHours = vModel.ServiceHours.OrderBy(x => x, new ServiceHourOrderComparer()).ToList();

                // Append ordering number for each item
                var index = 1;
                foreach (var serviceHour in sortedServiceHours)
                {
                    serviceHour.OrderNumber = index++;
                }

                definition.AddCollection(i => sortedServiceHours, o => o.ServiceHours, false);
            }
            if (vModel.SupportPhones != null && vModel.SupportPhones.Count > 0)
            {
                var phones = new List<V4VmOpenApiPhoneWithType>();
                var i = 1;
                vModel.SupportPhones.ForEach(p => phones.Add(new V4VmOpenApiPhoneWithType()
                {
                    Number = p.Number,
                    PrefixNumber = p.PrefixNumber,
                    Language = p.Language,
                    OwnerReferenceId = p.OwnerReferenceId,
                    AdditionalInformation = p.AdditionalInformation,
                    ChargeDescription = p.ChargeDescription,
                    ServiceChargeType = p.ServiceChargeType,
                    ExistsOnePerLanguage = false,
                    Type = PhoneNumberTypeEnum.Phone.ToString(),
                    OrderNumber = i++,
                }));

                definition.AddCollection(input => phones, output => output.Phones, false);
            }
            if (vModel.SupportEmails != null && vModel.SupportEmails.Count > 0)
            {
                var emails = new List<V4VmOpenApiEmail>();
                var i = 1;
                vModel.SupportEmails.ForEach(e => emails.Add(new V4VmOpenApiEmail()
                { Value = e.Value, Language = e.Language, OwnerReferenceId = e.OwnerReferenceId, ExistsOnePerLanguage = false, OrderNumber = i++ }));
                definition.AddCollection(input => emails, output => output.Emails, false);
            }

            return definition;
        }

        private class ServiceHourOrderComparer : IComparer<IVmOpenApiServiceHourBase>
        {
            // Type priority order for returning ServiceHours
            private readonly ServiceHoursTypeEnum[] serviceHourTypeOrder = new ServiceHoursTypeEnum[] { ServiceHoursTypeEnum.Standard, ServiceHoursTypeEnum.Special, ServiceHoursTypeEnum.Exception };

            public int Compare(IVmOpenApiServiceHourBase x, IVmOpenApiServiceHourBase y)
            {
                // Equal service hour types are ordered by Validity
                if (x.ServiceHourType == y.ServiceHourType)
                {
                    // Exception types are ordered by ValidFrom date
                    if (x.ServiceHourType == ServiceHoursTypeEnum.Exception.ToString())
                    {
                        return x.ValidFrom.Value.CompareTo(y.ValidFrom.Value);
                    }

                    // Other types are ordered by ValidForNow (with reverse boolean logic, thus y->x instead of x->y)
                    return y.ValidForNow.CompareTo(x.ValidForNow);
                }

                // Return in type priority order
                return Array.IndexOf(serviceHourTypeOrder, Enum.Parse(typeof(ServiceHoursTypeEnum), x.ServiceHourType))
                    .CompareTo(Array.IndexOf(serviceHourTypeOrder, Enum.Parse(typeof(ServiceHoursTypeEnum), y.ServiceHourType)));
            }
        }
    }
}
