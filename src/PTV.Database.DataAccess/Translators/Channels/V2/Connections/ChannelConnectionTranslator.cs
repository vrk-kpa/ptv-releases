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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using VmDigitalAuthorization = PTV.Domain.Model.Models.V2.Common.Connections.VmDigitalAuthorization;
using PTV.Domain.Model.Enums;
using System.Linq;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.V2.Channel.OpeningHours;
using PTV.Domain.Model.Models.V2.Common.Connections;

namespace PTV.Database.DataAccess.Translators.Channels.V2
{
    [RegisterService(typeof(ITranslator<ServiceChannel, VmConnectionsInput>), RegisterType.Transient)]
    internal class ChannelConnectionsInputTranslator : Translator<ServiceChannel, VmConnectionsInput>
    {
        public ChannelConnectionsInputTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager,
            translationPrimitives)
        {
        }

        public override VmConnectionsInput TranslateEntityToVm(ServiceChannel entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceChannel TranslateVmToEntity(VmConnectionsInput vModel)
        {
            var order = 1;
            vModel.SelectedConnections.ForEach(connection =>
            {
                connection.MainEntityId = connection.ConnectedEntityId;
                connection.ConnectedEntityId = vModel.UnificRootId;
                connection.OrderNumber = order++;
            });
            return CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddCollectionWithRemove(
                    i => i.SelectedConnections, 
                    o => o.ServiceServiceChannels, 
                    r => vModel.IsAsti ? r.IsASTIConnection : !r.IsASTIConnection)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceChannel, VmChannelConnectionsOutput>), RegisterType.Transient)]
    internal class ChannelConnectionsOutputTranslator : Translator<ServiceChannel, VmChannelConnectionsOutput>
    {
        public ChannelConnectionsOutputTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager,
            translationPrimitives)
        {
        }

        public override VmChannelConnectionsOutput TranslateEntityToVm(ServiceChannel entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddCollection(input => input.ServiceServiceChannels.OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.Connections)
                .GetFinal();
        }

        public override ServiceChannel TranslateVmToEntity(VmChannelConnectionsOutput vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmChannelConnectionOutput>), RegisterType.Transient)]
    internal class ChannelConnectionOutputTranslator : Translator<ServiceServiceChannel, VmChannelConnectionOutput>
    {
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private ILanguageOrderCache languageOrderCache;

        public ChannelConnectionOutputTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager,
            translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
            this.languageOrderCache = cacheManager.LanguageOrderCache;
        }

        public override VmChannelConnectionOutput TranslateEntityToVm(ServiceServiceChannel entity)
        {
            var service = VersioningManager.ApplyPublishingStatusFilterFallback(entity.Service.Versions);
            if (service != null)
            {
                var serviceTypeId = service.StatutoryServiceGeneralDescriptionId.IsAssigned()
                    ? VersioningManager.ApplyPublishingStatusFilterFallback(service.StatutoryServiceGeneralDescription.Versions).TypeId
                    : service.TypeId;
                var connectionId = entity.ServiceId.ToString() + entity.ServiceChannelId.ToString();
                return CreateEntityViewModelDefinition(entity)
                    .AddNavigation(i => connectionId, o => o.ConnectionId)
                    .AddSimple(i => service.Id, o => o.Id)
                    .AddSimple(i => service.UnificRootId, o => o.UnificRootId)
                    .AddCollection<ILanguageAvailability, VmLanguageAvailabilityInfo>(i => service.LanguageAvailabilities.OrderBy(x => languageOrderCache.Get(x.LanguageId)), o => o.LanguagesAvailabilities)
                    .AddSimple(i => serviceTypeId.Value, o => o.ServiceType)
                    .AddSimple(i => service.OrganizationId, o => o.OrganizationId)
                    .AddSimple(i => i.Modified, o => o.Modified)
                    .AddNavigation(i => i.ModifiedBy, o => o.ModifiedBy)
                    .AddDictionary(i => service.ServiceNames.Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())), o => o.Name,
                        i => languageCache.GetByValue(i.LocalizationId))
                    .AddNavigation(i => i, o => o.BasicInformation)
                    .AddNavigation(i => i, o => o.DigitalAuthorization)
                    .AddNavigation(i => i, o => o.AstiDetails)
                    .AddNavigation(i => i, o => o.ContactDetails)
                    .AddNavigation(i => i, o => o.OpeningHours)
                    .GetFinal();
            }
            return new VmChannelConnectionOutput();
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmChannelConnectionOutput vModel)
        {
            throw new NotImplementedException();
        }
    }
    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmAstiDetails>), RegisterType.Transient)]
    internal class AstiDetailsTranslator : Translator<ServiceServiceChannel, VmAstiDetails>
    {
        public AstiDetailsTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives
        ) : base(
            resolveManager,
            translationPrimitives
        )
        { }

        public override VmAstiDetails TranslateEntityToVm(ServiceServiceChannel entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddCollection(i => i.ServiceServiceChannelExtraTypes, o => o.AstiTypeInfos)
                .AddSimple(i => i.IsASTIConnection, o => o.IsASTIConnection)
                .GetFinal();
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmAstiDetails vModel)
        {
            throw new NotImplementedException();
        }
    }
    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmContactDetails>), RegisterType.Transient)]
    internal class ContactInformationTranslator : Translator<ServiceServiceChannel, VmContactDetails>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;
        private readonly EntityDefinitionHelper entityDefinitionHelper;

        public ContactInformationTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager,
            EntityDefinitionHelper entityDefinitionHelper
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
            this.entityDefinitionHelper = entityDefinitionHelper;
        }

        public override VmContactDetails TranslateEntityToVm(ServiceServiceChannel entity)
        {
            var faxTypeId = typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString());
            var phoneTypeId = typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString());
            var definition = CreateEntityViewModelDefinition(entity)
                .AddCollection(
                    input => input.ServiceServiceChannelAddresses,
                    output => output.PostalAddresses
                );

            entityDefinitionHelper
                .AddOrderedDictionaryList(
                    definition,
                    i => i.ServiceServiceChannelEmails.Select(x => x.Email),
                    o => o.Emails,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .AddOrderedDictionaryList(
                    definition,
                    i => i.ServiceServiceChannelWebPages.Select(x => x.WebPage),
                    o => o.WebPages,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .AddOrderedDictionaryList(
                    definition,
                    i => i.ServiceServiceChannelPhones
                        .Where(x => x.Phone.TypeId == phoneTypeId)
                        .Select(x => x.Phone),
                    o => o.PhoneNumbers,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .AddOrderedDictionaryList(
                    definition,
                    i => i.ServiceServiceChannelPhones
                        .Where(x => x.Phone.TypeId == faxTypeId)
                        .Select(x => x.Phone),
                    o => o.FaxNumbers,
                    k => languageCache.GetByValue(k.LocalizationId)
                );

            return definition.GetFinal();
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmContactDetails vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmOpeningHours>), RegisterType.Transient)]
    internal class ServiceHoursTranslator : Translator<ServiceServiceChannel, VmOpeningHours>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;
        public ServiceHoursTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpeningHours TranslateEntityToVm(ServiceServiceChannel entity)
        {
            var serviceHours = entity.ServiceServiceChannelServiceHours.Select(x => x.ServiceHours).ToList();

            return CreateEntityViewModelDefinition(entity)
                .AddCollection(i => GetHoursByType(serviceHours, ServiceHoursTypeEnum.Standard), o => o.StandardHours)
                .AddCollection(i => GetHoursByType(serviceHours, ServiceHoursTypeEnum.Exception), o => o.ExceptionHours)
                .AddCollection(i => GetHoursByType(serviceHours, ServiceHoursTypeEnum.Special), o => o.SpecialHours)
                .GetFinal();
        }
        private IEnumerable<ServiceHours> GetHoursByType(ICollection<ServiceHours> openingHours, ServiceHoursTypeEnum type)
        {
            return openingHours
                .Where(j => typesCache.Compare<ServiceHourType>(j.ServiceHourTypeId, type.ToString()))
                .OrderBy(x => x.OrderNumber)
                .ThenBy(x => x.Created);
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmOpeningHours vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceServiceChannelExtraType, VmAstiTypeInfo>), RegisterType.Transient)]
    internal class AstiTypeInfoTranslator : Translator<ServiceServiceChannelExtraType, VmAstiTypeInfo>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public AstiTypeInfoTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override VmAstiTypeInfo TranslateEntityToVm(ServiceServiceChannelExtraType entity)

        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.ExtraSubTypeId, o => o.AstiTypeId)
                .AddDictionary
                (
                    i => i.ServiceServiceChannelExtraTypeDescriptions,
                    o => o.AstiDescription,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .GetFinal();
        }

        public override ServiceServiceChannelExtraType TranslateVmToEntity(VmAstiTypeInfo vModel)
        {
            throw new NotImplementedException();
        }
    }
    [RegisterService(typeof(ITranslator<ServiceServiceChannelExtraTypeDescription, string>), RegisterType.Transient)]
    internal class AstiDescriptionStringTranslator : Translator<ServiceServiceChannelExtraTypeDescription, string>
    {
        public AstiDescriptionStringTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives
        ) : base(
            resolveManager,
            translationPrimitives
        )
        { }

        public override string TranslateEntityToVm(ServiceServiceChannelExtraTypeDescription entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => input.Description, output => output)
                .GetFinal();
        }

        public override ServiceServiceChannelExtraTypeDescription TranslateVmToEntity(string vModel)
        {
            throw new NotImplementedException();
        }
    }
    [RegisterService(typeof(ITranslator<ExtraSubTypeName, string>), RegisterType.Transient)]
    internal class AstiNameStringTranslator : Translator<ExtraSubTypeName, string>
    {
        public AstiNameStringTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives
        ) : base(
            resolveManager,
            translationPrimitives
        )
        { }

        public override string TranslateEntityToVm(ExtraSubTypeName entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => input.Name, output => output)
                .GetFinal();
        }

        public override ExtraSubTypeName TranslateVmToEntity(string vModel)
        {
            throw new NotImplementedException();
        }
    }
}
