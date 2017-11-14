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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Collections.Generic;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Translators.ServiceAndChannels
{

    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmChannelRelation>), RegisterType.Transient)]
    internal class ChannelRelationsTranslator : Translator<ServiceServiceChannel, VmChannelRelation>
    {
        private ITypesCache typesCache;
        private ILanguageCache languageCache;

        public ChannelRelationsTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmChannelRelation TranslateEntityToVm(ServiceServiceChannel entity)
        {
            var model = CreateEntityViewModelDefinition<VmChannelRelation>(entity)
                .AddSimple(i => i.ServiceId, o => o.Service)
                .AddSimple(i => i.ChargeTypeId, o => o.ChargeType)
                .AddSimple(i => true, o => o.isLoaded)
                .AddCollection(i => i.ServiceServiceChannelDigitalAuthorizations.Select(x => x.DigitalAuthorization as IFintoItemBase), output => output.DigitalAuthorizations)
                .AddNavigation(i => VersioningManager.ApplyPublishingStatusFilterFallback(i.ServiceChannel.Versions), o => o.ConnectedChannel)
//                .AddLocalizable(i => GetDescription(i), output => output.Description)
//                .AddLocalizable(i => GetDescription(i), output => output.ChargeTypeAdditionalInformation)
                .GetFinal();

            model.Texts = GetDescriptions(entity);
            return model;
        }

        private IDictionary<string, VmChannelRelationTexts> GetDescriptions(ServiceServiceChannel serviceServiceChannel)
        {
            return serviceServiceChannel.ServiceServiceChannelDescriptions.GroupBy(x => x.LocalizationId).ToDictionary(x => languageCache.GetByValue(x.Key), x => GetDescription(x.Select(i => i)));
        }

        private VmChannelRelationTexts GetDescription(IEnumerable<ServiceServiceChannelDescription> descriptions)
        {
            return new VmChannelRelationTexts
            {
                Description = descriptions.FirstOrDefault(x => x.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()))?.Description,
                ChargeTypeAdditionalInformation = descriptions.FirstOrDefault(x => x.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString()))?.Description,
            };
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmChannelRelation model)
        {
            model.Descriptions.ForEach(x => {
                x.TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString());
                x.OwnerReferenceId = model.Service;
                x.OwnerReferenceId2 = model.ConnectedChannel.RootId;
            });

            model.ChargeTypeAdditionalInformations.ForEach(x => {
                x.TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString());
                x.OwnerReferenceId = model.Service;
                x.OwnerReferenceId2 = model.ConnectedChannel.RootId;
            });

            var descriptions = model.Descriptions.Concat(model.ChargeTypeAdditionalInformations);

            return CreateViewModelEntityDefinition<ServiceServiceChannel>(model)
                .UseDataContextUpdate(input => true, input => output => input.Service == output.ServiceId && input.ConnectedChannel.RootId == output.ServiceChannelId,
                def => def.UseDataContextCreate(input => true).AddSimple(input => input.Service, output => output.ServiceId)
                )
                .AddSimple(input => input.ConnectedChannel.RootId, output => output.ServiceChannelId)
                .AddSimple(input => input.ChargeType, output => output.ChargeTypeId)
                .AddCollection(input => descriptions, o => o.ServiceServiceChannelDescriptions)
                .AddCollection(input => input.DigitalAuthorizations.Select(x => new VmDigitalAuthorization() { Id = x.Id, ServiceId = input.Service, ChannelId = input.ConnectedChannel.RootId }), o => o.ServiceServiceChannelDigitalAuthorizations)
                .GetFinal();
        }
    }
}
