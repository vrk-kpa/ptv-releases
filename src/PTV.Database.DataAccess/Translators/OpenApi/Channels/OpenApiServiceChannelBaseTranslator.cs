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
            var resultDefinition = CreateEntityViewModelDefinition(entity)
                .AddCollection(i => i.ServiceChannelDescriptions, o => o.ServiceChannelDescriptions)
                .AddCollection(i => i.Languages.Select(j => j.Language.Code).ToList(), o => o.Languages)
                .AddCollection(i => i.WebPages, o => o.WebPages)
                .AddCollection(i => i.ServiceHours, o => o.ServiceHours)
                // For support phones we only match items with type Phone.
                .AddCollection(i => i.Phones.Where(p => p.Phone.TypeId == typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString())), o => o.SupportPhones)
                .AddCollection(i => i.Emails, o => o.SupportEmails)
                .AddNavigation(i => i.PublishingStatus, o => o.PublishingStatus)
                .AddCollection(i => i.LanguageAvailabilities.Select(l => languageCache.GetByValue(l.LanguageId)).ToList(), o => o.AvailableLanguages);
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
            if (vModel.Languages != null && vModel.Languages.Count > 0)
            {
                var languages = new List<VmOpenApiStringItem>();
                vModel.Languages.ForEach(l => languages.Add(new VmOpenApiStringItem { Value = l, OwnerReferenceId = vModel.Id }));
                definition.AddCollection(i => languages, o => o.Languages);
            }
            if (vModel.WebPages != null && vModel.WebPages.Count > 0)
            {
                definition.AddCollection(i => i.WebPages, o => o.WebPages, false);
            }
            if (vModel.ServiceHours != null && vModel.ServiceHours.Count > 0)
            {
                definition.AddCollection(i => i.ServiceHours, o => o.ServiceHours, false);
            }
            if (vModel.SupportPhones != null && vModel.SupportPhones.Count > 0)
            {
                var phones = new List<V4VmOpenApiPhoneWithType>();
                vModel.SupportPhones.ForEach(p => phones.Add(new V4VmOpenApiPhoneWithType()
                {
                    Number = p.Number,
                    PrefixNumber = p.PrefixNumber,
                    Language = p.Language,
                    OwnerReferenceId = p.OwnerReferenceId,
                    AdditionalInformation = p.AdditionalInformation,
                    ChargeDescription = p.ChargeDescription,
                    ServiceChargeType = p.ServiceChargeType,
                    ExistsOnePerLanguage = true,
                    Type = PhoneNumberTypeEnum.Phone.ToString()
                }));

                definition.AddCollection(input => phones, output => output.Phones, false);
            }
            if (vModel.SupportEmails != null && vModel.SupportEmails.Count > 0)
            {
                var emails = new List<V4VmOpenApiEmail>();
                vModel.SupportEmails.ForEach(e => emails.Add(new V4VmOpenApiEmail()
                    { Value = e.Value, Language = e.Language, OwnerReferenceId = e.OwnerReferenceId, ExistsOnePerLanguage = true }));
                definition.AddCollection(input => emails, output => output.Emails, false);
            }

            return definition;
        }
    }
}
