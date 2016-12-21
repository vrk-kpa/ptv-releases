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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannel, IVmChannelDescription>), RegisterType.Transient)]
    internal class ServiceChannelMainInfoStepTranslator : Translator<ServiceChannel, IVmChannelDescription>
    {
        private ITypesCache typesCache;

        public ServiceChannelMainInfoStepTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = cacheManager.TypesCache;
        }

        public override IVmChannelDescription TranslateEntityToVm(ServiceChannel entity)
        {
            var step = CreateEntityViewModelDefinition(entity)
                .AddLocalizable(input => GetName(input, NameTypeEnum.Name), output => output.Name)
                .AddLocalizable(input => GetDescription(input, DescriptionTypeEnum.ShortDescription), output => output.ShortDescription)
                .AddLocalizable(input => GetDescription(input, DescriptionTypeEnum.Description), output => output.Description)
                .AddSimple(input => input.OrganizationId, output => output.OrganizationId);

            return step.GetFinal();
        }

        private IEnumerable<IName> GetName(ServiceChannel serviceChannel, NameTypeEnum type)
        {
            return serviceChannel.ServiceChannelNames.Where(x => typesCache.Compare<NameType>(x.TypeId, type.ToString()));
        }

        private IEnumerable<IDescription> GetDescription(ServiceChannel serviceChannel, DescriptionTypeEnum type)
        {
            return serviceChannel.ServiceChannelDescriptions.Where(x => typesCache.Compare<DescriptionType>(x.TypeId, type.ToString()));
        }

        public override ServiceChannel TranslateVmToEntity(IVmChannelDescription vModel)
        {
            var names = new List<VmName>()
            {
                new VmName {Name = vModel.Name, TypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString()), OwnerReferenceId = vModel.Id },
            };
            var descriptions = new List<VmDescription>()
            {
                new VmDescription { Description = vModel.ShortDescription, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString()), OwnerReferenceId = vModel.Id },
                new VmDescription { Description = vModel.Description, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()), OwnerReferenceId = vModel.Id },
            };
            return CreateViewModelEntityDefinition(vModel)
                .AddSimple(i => i.OrganizationId ?? Guid.Empty, o => o.OrganizationId)
                .AddCollection(i => names, o => o.ServiceChannelNames)
                .AddCollection(i => descriptions, o => o.ServiceChannelDescriptions)
                .GetFinal();
        }
    }
}