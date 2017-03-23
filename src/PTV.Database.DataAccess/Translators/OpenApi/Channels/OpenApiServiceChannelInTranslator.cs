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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework.Interfaces;
using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V1;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    abstract class OpenApiServiceChannelInTranslator<TVmOpenApiServiceChannelIn> : OpenApiServiceChannelBaseTranslator<TVmOpenApiServiceChannelIn> where TVmOpenApiServiceChannelIn : class, IVmOpenApiServiceChannelIn
    {
        protected OpenApiServiceChannelInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {}

        public override TVmOpenApiServiceChannelIn TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            return base.TranslateEntityToVm(entity);
        }
        public override ServiceChannelVersioned TranslateVmToEntity(TVmOpenApiServiceChannelIn vModel)
        {
            return CreateVmToChannelDefinitions(vModel)//base.TranslateVmToEntity(vModel);
                .GetFinal();
        }

        protected ITranslationDefinitions<TVmOpenApiServiceChannelIn, ServiceChannelVersioned> CreateVmToChannelDefinitions(TVmOpenApiServiceChannelIn vModel)
        {
            var definition = CreateBaseVmEntityDefinitions(vModel);

            // Set available languages
            var languages = GetAvailableLanguages(vModel);
            if (languages.Count > 0)
            {
                definition.AddCollection(i => languages, o => o.LanguageAvailabilities, true);
            }

            if (!string.IsNullOrEmpty(vModel.OrganizationId))
            {
                definition.AddSimple(i => Guid.Parse(i.OrganizationId), o => o.OrganizationId);
            }
            if (vModel.ServiceChannelNames != null && vModel.ServiceChannelNames.Count > 0)
            {
                definition.AddCollection(i => i.ServiceChannelNames.Select(n => new VmOpenApiLocalizedListItem()
                { Value = n.Value, Language = n.Language, Type = NameTypeEnum.Name.ToString(), OwnerReferenceId = vModel.Id }).ToList(), o => o.ServiceChannelNames);
            }

            // We have to check if collections need to be removed in this translator because in base translator/model 'DeleteAllServiceHour'
            // kind of properties do not exist.
            if (vModel.DeleteAllServiceHours && vModel.ServiceHours?.Count == 0)
            {
                definition.AddCollection(i => i.ServiceHours, o => o.ServiceHours, false);
            }
            if (vModel.DeleteAllSupportEmails && vModel.SupportEmails?.Count == 0)
            {
                definition.AddCollection(i => new List<V4VmOpenApiEmail>(), o => o.Emails, false);
            }
            if (vModel.DeleteAllSupportPhones && vModel.SupportPhones?.Count == 0)
            {
                definition.AddCollection(i => new List<VmOpenApiPhoneWithType>(), o => o.Phones, false);
            }
            if (vModel.DeleteAllWebPages && vModel.WebPages?.Count == 0)
            {
                definition.AddCollection(i => i.WebPages, o => o.WebPages, false);
            }

            return definition;
        }

        private List<VmOpenApiLanguageAvailability> GetAvailableLanguages(TVmOpenApiServiceChannelIn vModel)
        {
            var languages = new List<VmOpenApiLanguageAvailability>();
            var currentPublishingStatus = !string.IsNullOrEmpty(vModel.CurrentPublishingStatus) ? vModel.CurrentPublishingStatus : PublishingStatus.Draft.ToString();
            vModel.AvailableLanguages.ForEach(item =>
            {
                if (!languages.Select(l => l.Language).ToList().Contains(item))
                {
                    languages.Add(new VmOpenApiLanguageAvailability() { Language = item, OwnerReferenceId = vModel.Id, PublishingStatus = currentPublishingStatus });
                }
            });
            
            return languages;
        }
    }
}
