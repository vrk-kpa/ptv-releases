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

namespace PTV.Database.DataAccess.Translators.ServiceAndChannels
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannel, VmChannelRelationListItem>), RegisterType.Transient)]
    internal class ChannelRelationsTranslator : Translator<ServiceServiceChannel, VmChannelRelationListItem>
    {
        private ITypesCache typesCache;

        public ChannelRelationsTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmChannelRelationListItem TranslateEntityToVm(ServiceServiceChannel entity)
        {
            return CreateEntityViewModelDefinition<VmChannelRelationListItem>(entity)
                .AddSimple(i => Guid.NewGuid(), o => o.Id)
                .AddSimple(i => i.ServiceId, o => o.Service)
                .AddSimple(i => i.ServiceChargeTypeId, o => o.ChargeType)
                .AddNavigation(i => i.ServiceChannel, o => o.ConnectedChannel)
                .AddLocalizable(i => GetDescription(i, DescriptionTypeEnum.Description), output => output.Description)
                .AddLocalizable(i => GetDescription(i, DescriptionTypeEnum.ChargeTypeAdditionalInfo), output => output.ChargeTypeAdditionalInformation)
                .GetFinal();
        }

        private ICollection<ServiceServiceChannelDescription> GetDescription(ServiceServiceChannel serviceServiceChannel, DescriptionTypeEnum type)
        {
            return serviceServiceChannel.ServiceServiceChannelDescriptions.Where(x => x.TypeId == typesCache.Get<DescriptionType>(type.ToString())).ToList();
        }

        public override ServiceServiceChannel TranslateVmToEntity(VmChannelRelationListItem model)
        {

            var descriptions = new List<VmDescription>()
            {
                new VmDescription { Description = model.Description, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()), OwnerReferenceId = model.Service, OwnerReferenceId2= model.ConnectedChannel.Id},
                new VmDescription { Description = model.ChargeTypeAdditionalInformation, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString()),  OwnerReferenceId = model.Service, OwnerReferenceId2= model.ConnectedChannel.Id},
            };

            return CreateViewModelEntityDefinition<ServiceServiceChannel>(model)
                .UseDataContextUpdate(input => true, input => output => input.Service == output.ServiceId && input.ConnectedChannel.Id == output.ServiceChannelId,
                def => def.UseDataContextCreate(input => true)
                )
                .AddSimple(input => input.Service, output => output.ServiceId)
                .AddSimple(input => input.ConnectedChannel.Id, output => output.ServiceChannelId)
                .AddSimple(input => input.ChargeType, output => output.ServiceChargeTypeId)
                .AddCollection(i => descriptions, o => o.ServiceServiceChannelDescriptions)
                .GetFinal();
        }
    }
}
