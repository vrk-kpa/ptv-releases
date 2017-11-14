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
using VmDigitalAuthorization = PTV.Domain.Model.Models.V2.Common.VmDigitalAuthorization;
using PTV.Domain.Model.Enums;
using System.Linq;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models;
using System.Collections.Generic;
using PTV.Domain.Model.Models.V2.Common;

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
                .AddCollectionWithRemove(i => i.SelectedConnections, o => o.ServiceServiceChannels, r => true)
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

        public ChannelConnectionOutputTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager,
            translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmChannelConnectionOutput TranslateEntityToVm(ServiceServiceChannel entity)
        {
            var service = VersioningManager.ApplyPublishingStatusFilterFallback(entity.Service.Versions);
            var serviceTypeId = service.StatutoryServiceGeneralDescriptionId.IsAssigned()
                ? VersioningManager.ApplyPublishingStatusFilterFallback(service.StatutoryServiceGeneralDescription.Versions).TypeId
                : service.TypeId;
            var connectionId = entity.ServiceId.ToString() + service.UnificRootId.ToString();
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => connectionId, o => o.ConnectionId)
                .AddSimple(i => service.Id, o => o.Id)
                .AddSimple(i => service.UnificRootId, o => o.UnificRootId)
                .AddCollection<ILanguageAvailability, VmLanguageAvailabilityInfo>(i => service.LanguageAvailabilities, o => o.LanguagesAvailabilities)
                .AddSimple(i => serviceTypeId.Value, o => o.ServiceTypeId)
                .AddNavigation(i => typesCache.GetByValue<ServiceType>(serviceTypeId.Value), o => o.ServiceType)
                .AddSimple(i => service.OrganizationId, o => o.OrganizationId)
                .AddSimple(i => i.Modified, o => o.Modified)
                .AddNavigation(i => i.ModifiedBy, o => o.ModifiedBy)
                .AddDictionary(i => service.ServiceNames.Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())), o => o.Name,
                    i => languageCache.GetByValueEnum(i.LocalizationId).ToString())
                .AddNavigation(i => i, o => o.BasicInformation)
                .AddNavigation(i => i, o => o.DigitalAuthorization)
                .AddNavigation(i => i, o => o.AstiDetails)
                .GetFinal();
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmChannelConnectionOutput vModel)
        {
            throw new NotImplementedException();
        }
    }


    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmAstiDetails>), RegisterType.Transient)]
    internal class AstiDetailsTranslator : Translator<ServiceServiceChannel, VmAstiDetails>
    {
        public AstiDetailsTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmAstiDetails TranslateEntityToVm(ServiceServiceChannel entity)
        {
            var def = CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.IsASTIConnection, o => o.IsASTIConnection);
            return def.GetFinal();
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmAstiDetails vModel)
        {
            throw new NotImplementedException();
        }
    }
}