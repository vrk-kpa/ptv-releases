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

using System;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Enums;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ServiceChannelTranslationDefinitionHelper), RegisterType.Scope)]
    internal class ServiceChannelTranslationDefinitionHelper : EntityDefinitionHelper
    {
        private ILanguageCache languageCache;
        private ITypesCache typesCache;

        public ServiceChannelTranslationDefinitionHelper(ICacheManager cacheManager)
        {
            this.languageCache = cacheManager.LanguageCache;
            this.typesCache = cacheManager.TypesCache;
        }

        public ITranslationDefinitions<TIn, ServiceChannelVersioned> AddWebPagesDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannelVersioned> definition, IWebPages model, Guid? referenceId) where TIn : IWebPages
        {
            model.WebPages?.ForEach(i => i.OwnerReferenceId = referenceId);
            return definition.AddCollection(i => i.WebPages.Where(j => !string.IsNullOrEmpty(j.UrlAddress)), o => o.WebPages, true);
        }

        public ITranslationDefinitions<ServiceChannelVersioned, TOut> AddWebPagesDefinition<TOut>(ITranslationDefinitions<ServiceChannelVersioned, TOut> definition, LanguageCode languageCode) where TOut : IWebPages
        {
            return definition.AddCollection(i => i.WebPages.OrderBy(x => x.WebPage.OrderNumber).ThenBy(x=>x.WebPage.Created).Where(x => languageCache.Get(languageCode.ToString()) == x.WebPage.LocalizationId), o => o.WebPages);

        }

        public ITranslationDefinitions<TIn, ServiceChannelVersioned> AddAttachmentsDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannelVersioned> definition, IAttachments model, Guid? referenceId) where TIn : IAttachments
        {
            var order = 1;
            model.UrlAttachments?.ForEach(i =>
            {
                i.OrderNumber = order++;
                i.OwnerReferenceId = referenceId;
            });

            return definition.AddCollection(input => input.UrlAttachments, output => output.Attachments);
        }

        public ITranslationDefinitions<ServiceChannelVersioned, TOut> AddAttachmentsDefinition<TOut>(ITranslationDefinitions<ServiceChannelVersioned, TOut> definition, LanguageCode requestLanguageCode) where TOut : IAttachments
        {
            return definition.AddCollection(input => languageCache.FilterCollection(input.Attachments.Select(i => i.Attachment).OrderBy(i => i.OrderNumber).ThenBy(i => i.Modified), requestLanguageCode), output => output.UrlAttachments);
        }

        public ITranslationDefinitions<TIn, ServiceChannelVersioned> AddChannelDescriptionsDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannelVersioned> definition) where TIn : IVmChannelDescription
        {
            return definition.AddPartial<IVmChannelDescription>(input => input);
        }

        public ITranslationDefinitions<ServiceChannelVersioned, TOut> AddChannelDescriptionsDefinition<TOut>(ITranslationDefinitions<ServiceChannelVersioned, TOut> definition) where TOut : IVmChannelDescription
        {
            return definition.AddPartial(input => input, output => output as IVmChannelDescription);
        }
        
        public ITranslationDefinitions<TIn, ServiceChannelVersioned> AddPhonesDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannelVersioned> definition) where TIn : IPhoneNumbers
        {
            return definition.AddCollection(input =>
                    {
                        var number = 1;
                        input.PhoneNumbers.ForEach(phoneNumber => phoneNumber.OrderNumber = number++);
                        return input.PhoneNumbers;
                    }, output => output.Phones);
        }
        public ITranslationDefinitions<TIn, ServiceChannelVersioned> AddEmailsDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannelVersioned> definition) where TIn : IEmails
        {
            return definition.AddCollection(input =>
                    {
                        var number = 1;
                        input.Emails.ForEach(email => email.OrderNumber = number++);
                        return input.Emails;
                    }, output => output.Emails);
        }
    
        public ITranslationDefinitions<ServiceChannelVersioned, TOut> AddPhonesDefinition<TOut>(ITranslationDefinitions<ServiceChannelVersioned, TOut> definition, LanguageCode languageCode) where TOut : IPhoneNumbers
        {
            return definition.AddCollection(input => languageCache.FilterCollection(input.Phones.Select(x => x.Phone), languageCode).OrderBy(x=>x.OrderNumber).ThenBy(x=>x.Created),
                                               output => output.PhoneNumbers);
        }

        public ITranslationDefinitions<ServiceChannelVersioned, TOut> AddPhonesPhoneTypeDefinition<TOut>(ITranslationDefinitions<ServiceChannelVersioned, TOut> definition, LanguageCode languageCode) where TOut : IPhoneNumbers
        {
            return definition.AddCollection(input => languageCache.FilterCollection(input.Phones.Where(x=>x.Phone.TypeId == typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString())).Select(x => x.Phone), languageCode).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created),
                                               output => output.PhoneNumbers);
        }
        public ITranslationDefinitions<ServiceChannelVersioned, TOut> AddPhonesFaxTypeDefinition<TOut>(ITranslationDefinitions<ServiceChannelVersioned, TOut> definition, LanguageCode languageCode) where TOut : IFaxNumbers
        {
            return definition.AddCollection(input => languageCache.FilterCollection(input.Phones.Where(x => x.Phone.TypeId == typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString())).Select(x => x.Phone), languageCode).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created),
                                               output => output.FaxNumbers);
        }

        public ITranslationDefinitions<ServiceChannelVersioned, TOut> AddEmailsDefinition<TOut>(ITranslationDefinitions<ServiceChannelVersioned, TOut> definition, LanguageCode languageCode) where TOut : IEmails
        {
            return definition.AddCollection(input => languageCache.FilterCollection(input.Emails.Select(x => x.Email), languageCode).OrderBy(x=>x.OrderNumber).ThenBy(x=>x.Created),
                                               output => output.Emails);           
        }

        public ITranslationDefinitions<TIn, ServiceChannelVersioned> AddPhoneDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannelVersioned> definition) where TIn : IPhoneNumber
        {
            return definition.AddPartial<IPhoneNumber>(input => input);
        }

        public ITranslationDefinitions<ServiceChannelVersioned, TOut> AddPhoneDefinition<TOut>(ITranslationDefinitions<ServiceChannelVersioned, TOut> definition) where TOut : IPhoneNumber
        {
            return definition.AddPartial(input => input, output => output as IPhoneNumber);
        }

        public ITranslationDefinitions<TIn, ServiceChannelVersioned> AddLanguagesDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannelVersioned> definition) where TIn : ILanguages
        {
            var languageCounter = 0;
            return definition.AddCollection(
              i => i.Languages.Select(x => new VmListItem {
                Id = x,
                OrderNumber = languageCounter++,
                OwnerReferenceId = i.Id
              }),
              o => o.Languages
            );
        }

        public ITranslationDefinitions<ServiceChannelVersioned, TOut> AddLanguagesDefinition<TOut>(ITranslationDefinitions<ServiceChannelVersioned, TOut> definition) where TOut : ILanguages
        {
            return definition.AddSimpleList(
                input => input.Languages
                    .OrderBy(x => x.Order)
                    .Select(x => x.LanguageId),
                output => output.Languages
            );
        }

        public ITranslationDefinitions<TIn, ServiceChannelVersioned> AddAllAreasDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannelVersioned> definition,Guid? AreaInformationTypeId, Guid? id) where TIn : IVmAreaInformation
        {
            if (typesCache.Compare<AreaInformationType>(AreaInformationTypeId, AreaInformationTypeEnum.AreaType.ToString()))
            {
                return definition
                      .AddCollection(
                          i => i.AreaBusinessRegions.Union(i.AreaHospitalRegions).Union(i.AreaProvince).Select(x => new VmListItem
                          {
                              Id = x,
                              OwnerReferenceId = id
                          }), o => o.Areas)
                    .AddCollection(
                          i => i.AreaMunicipality.Select(x => new VmListItem
                          {
                              Id = x,
                              OwnerReferenceId = id
                          }), o => o.AreaMunicipalities);
            }
            else
            {
                definition.AddCollection(i => new List<VmListItem>() {}, o => o.Areas, TranslationPolicy.FetchData);
                definition.AddCollection(i => new List<VmListItem>() {}, o => o.AreaMunicipalities, TranslationPolicy.FetchData);
            }
            return definition;
        }

        public ITranslationDefinitions<ServiceChannelVersioned, TOut> AddAllAreasDefinition<TOut>(ITranslationDefinitions<ServiceChannelVersioned, TOut> definition) where TOut : IVmAreaInformation
        {
            return definition
                .AddSimpleList(
                    input => input.Areas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString()))
                        .Select(x => x.AreaId),
                    output => output.AreaProvince)
                .AddSimpleList(
                    input => input.Areas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.BusinessRegions.ToString()))
                        .Select(x => x.AreaId),
                    output => output.AreaBusinessRegions)
                .AddSimpleList(
                    input => input.Areas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.HospitalRegions.ToString()))
                        .Select(x => x.AreaId),
                    output => output.AreaHospitalRegions)
                .AddSimpleList(
                    input => input.AreaMunicipalities.Select(x => x.MunicipalityId),
                    output => output.AreaMunicipality
            );
        }
        
        public ITranslationDefinitions<TIn, ServiceChannelServiceHours> AddOpeningHoursDefinition<TIn>(ITranslationDefinitions<TIn, ServiceChannelServiceHours> definition) where TIn : VmHours
        {
            return definition.AddPartial<VmHours>(input => input);
        }

        public ITranslationDefinitions<ServiceChannelServiceHours, TOut> AddOpeningHoursDefinition<TOut>(ITranslationDefinitions<ServiceChannelServiceHours, TOut> definition) where TOut : VmHours
        {
            return definition.AddPartial(input => input, output => output as VmHours);
        }
    }

    [RegisterService(typeof(EntityDefinitionHelper), RegisterType.Scope)]
    internal class EntityDefinitionHelper
    {
        public ITranslationDefinitionsForVersioning<TIn, TOut> GetDefinitionWithCreateOrUpdate<TIn, TOut>(ITranslationDefinitionsForContextUsage<TIn, TOut> definition) where TIn : VmEntityBase where TOut : IEntityIdentifier
        {
            return definition
                .DisableAutoTranslation()
                .UseDataContextCreate(input => !input.Id.IsAssigned(), output => output.Id, input => Guid.NewGuid())
                .UseDataContextUpdate(input => input.Id.IsAssigned(), input => output => input.Id == output.Id);
        }
    }
}
