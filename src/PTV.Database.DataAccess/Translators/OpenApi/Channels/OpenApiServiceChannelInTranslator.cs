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
using PTV.Framework.Interfaces;
using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Framework;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    abstract class OpenApiServiceChannelInTranslator<TVmOpenApiServiceChannelIn> : Translator<ServiceChannelVersioned, TVmOpenApiServiceChannelIn> where TVmOpenApiServiceChannelIn : class, IVmOpenApiServiceChannelIn
    {
        ITypesCache typesCache;
        protected OpenApiServiceChannelInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override TVmOpenApiServiceChannelIn TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceChannelInTranslator!");
        }
        public override ServiceChannelVersioned TranslateVmToEntity(TVmOpenApiServiceChannelIn vModel)
        {
            return CreateVmToChannelDefinitions(vModel)
                .GetFinal();
        }

        protected ITranslationDefinitions<TVmOpenApiServiceChannelIn, ServiceChannelVersioned> CreateVmToChannelDefinitions(TVmOpenApiServiceChannelIn vModel)
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

            // Set available languages
            var languages = GetAvailableLanguages(vModel);
            if (languages.Count > 0)
            {
                definition.AddCollection(i => languages, o => o.LanguageAvailabilities, true);
            }

            // organization
            if (!string.IsNullOrEmpty(vModel.OrganizationId))
            {
                definition.AddSimple(i => Guid.Parse(i.OrganizationId), o => o.OrganizationId);
            }

            // names
            if (vModel.ServiceChannelNames != null && vModel.ServiceChannelNames.Count > 0)
            {
                definition.AddCollection(i => i.ServiceChannelNames.Select(n => new VmOpenApiLocalizedListItem()
                { Value = n.Value, Language = n.Language, Type = NameTypeEnum.Name.ToString(), OwnerReferenceId = vModel.Id }).ToList(), o => o.ServiceChannelNames);
            }

            // descriptions
            if (vModel.ServiceChannelDescriptions != null && vModel.ServiceChannelDescriptions.Count > 0)
            {
                definition.AddCollection(i => i.ServiceChannelDescriptions, o => o.ServiceChannelDescriptions, true);
            }

            // area type
            if (!string.IsNullOrEmpty(vModel.AreaType))
            {
                definition.AddSimple(i => typesCache.Get<AreaInformationType>(vModel.AreaType), o => o.AreaInformationTypeId);
            }

            // Set areas
            if (vModel.Areas.Count > 0)
            {
                var municipalityAreas = vModel.Areas.Where(a => a.Type == AreaTypeEnum.Municipality.ToString()).ToList();
                var otherAreas = vModel.Areas.Where(a => a.Type != AreaTypeEnum.Municipality.ToString()).ToList();
                if (municipalityAreas.Count > 0)
                {
                    var municipalities = new List<VmOpenApiStringItem>();
                    municipalityAreas.ForEach(area =>
                    {
                        area.AreaCodes.ForEach(m => municipalities.Add(new VmOpenApiStringItem { Value = m, OwnerReferenceId = vModel.Id }));
                    });
                    definition.AddCollection(i => municipalities, o => o.AreaMunicipalities, false); // Update municipalities
                    if (otherAreas.Count == 0)
                    {
                        definition.AddCollection(i => new List<VmOpenApiArea>(), o => o.Areas, false); // Remove possible old areas
                    }
                }
                if (otherAreas.Count > 0)
                {
                    var areas = new List<VmOpenApiArea>();
                    otherAreas.ForEach(area =>
                    {
                        area.AreaCodes.ForEach(a => areas.Add(new VmOpenApiArea { Type = area.Type, Code = a, OwnerReferenceId = vModel.Id }));
                    });
                    definition.AddCollection(i => areas, o => o.Areas, false); // Update areas
                    if (municipalityAreas.Count == 0)
                    {
                        definition.AddCollection(i => new List<VmOpenApiStringItem>(), o => o.AreaMunicipalities, false); // Remove possible old municipalities
                    }
                }
            }
            else if (vModel.Id.IsAssigned() && !vModel.AreaType.IsNullOrEmpty() && vModel.AreaType != AreaInformationTypeEnum.AreaType.ToString())
            {
                // Area type has been changed into WholeCountry or WholeCountryExceptAlandIslands so we need to remove possible old municipalities and areas.
                definition.AddCollection(i => new List<VmOpenApiStringItem>(), o => o.AreaMunicipalities, false);
                definition.AddCollection(i => new List<VmOpenApiArea>(), o => o.Areas, false);
            }

            // Set connection type
            var connectionType = ServiceChannelConnectionTypeEnum.NotCommon;
            if (vModel.IsVisibleForAll)
            {
                connectionType = ServiceChannelConnectionTypeEnum.CommonForAll;
            }
            var connectionTypeId = typesCache.Get<ServiceChannelConnectionType>(connectionType.ToString());
            definition.AddSimple(i => connectionTypeId, o => o.ConnectionTypeId);

            // languages
            if (vModel.Languages != null && vModel.Languages.Count > 0)
            {
                var order = 0;
                var languageList = new List<VmOpenApiStringItem>();
                vModel.Languages.ForEach(l => languageList.Add(new VmOpenApiStringItem
                {
                    Value = l,
                    Order = order++,
                    OwnerReferenceId = vModel.Id
                }));
                definition.AddCollection(i => languageList, o => o.Languages);
            }

            // web pages
            if (vModel.DeleteAllWebPages || vModel.WebPages?.Count > 0)
            {
                definition.AddCollection(i => i.WebPages, o => o.WebPages, false);
            }

            // service hours
            if (vModel.DeleteAllServiceHours || vModel.ServiceHours?.Count > 0)
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

            // support phones
            if (vModel.DeleteAllSupportPhones || vModel.SupportPhones?.Count > 0)
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

            // support emails
            if (vModel.DeleteAllSupportEmails || vModel.SupportEmails?.Count > 0)
            {
                var emails = new List<V4VmOpenApiEmail>();
                var i = 1;
                vModel.SupportEmails.ForEach(e => emails.Add(new V4VmOpenApiEmail()
                { Value = e.Value, Language = e.Language, OwnerReferenceId = e.OwnerReferenceId, ExistsOnePerLanguage = false, OrderNumber = i++ }));
                definition.AddCollection(input => emails, output => output.Emails, false);
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
