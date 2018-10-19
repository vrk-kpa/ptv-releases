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
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceProducer, VmServiceProducer>), RegisterType.Transient)]
    internal class ServiceProducerTranslator : Translator<ServiceProducer, VmServiceProducer>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;
        public ServiceProducerTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = cacheManager.LanguageCache;
            this.typesCache = cacheManager.TypesCache;
        }

        public override VmServiceProducer TranslateEntityToVm(ServiceProducer entity)
        {
            var definition = CreateEntityViewModelDefinition<VmServiceProducer>(entity)
                .AddSimple(input => input.Id, output => output.Id)
                .AddSimple(input => input.ProvisionTypeId, output => output.ProvisionType)
                .AddPartial(i => i as IOrderable, o => o as IVmOrderable)
                .AddSimple(input => input.ServiceVersionedId, output => output.OwnerReferenceId);
                
            var provisionType = typesCache.GetByValue<ProvisionType>(entity.ProvisionTypeId);
            var provisionTypeEnum = Enum.Parse(typeof(ProvisionTypeEnum), provisionType);
            switch (provisionTypeEnum)
            {
                case ProvisionTypeEnum.SelfProduced:
                    definition.AddSimpleList(input => input.Organizations.Select(o => o.OrganizationId).ToList(), output => output.SelfProducers);
                    break;
                case ProvisionTypeEnum.PurchaseServices:
                    definition.AddDictionary(input => input.AdditionalInformations, output => output.AdditionalInformation, k => languageCache.GetByValue(k.LocalizationId));
                    var purchaseServiceOrganization = entity.Organizations.FirstOrDefault();
                    if (purchaseServiceOrganization != null)
                    {
                        definition.AddSimple(input => purchaseServiceOrganization.OrganizationId, output => output.Organization);
                    }
                    break;
                case ProvisionTypeEnum.Other:
                    definition.AddDictionary(input => input.AdditionalInformations, output => output.AdditionalInformation, k => languageCache.GetByValue(k.LocalizationId));
                    var otherOrganization = entity.Organizations.FirstOrDefault();
                    if (otherOrganization != null)
                    {
                        definition.AddSimple(input => otherOrganization.OrganizationId, output => output.Organization);
                    }
                    break;
            }

            return definition.GetFinal();
        }

        public override ServiceProducer TranslateVmToEntity(VmServiceProducer vModel)
        {
            var exists = vModel.Id.IsAssigned();

            var definition = CreateViewModelEntityDefinition<ServiceProducer>(vModel)
                .UseDataContextCreate(input => !exists, output => output.Id, input => Guid.NewGuid())
                .UseDataContextLocalizedUpdate(input => exists, input => output => input.Id == output.Id)
                .AddSimple(input => input.ProvisionType, output => output.ProvisionTypeId)
                .AddPartial(i => i as IVmOrderable, o => o as IOrderable);

            ProvisionTypeEnum provisionType;
            if (!Enum.TryParse(typesCache.GetByValue<ProvisionType>(vModel.ProvisionType), out provisionType))
            {
                return definition.GetFinal();
            }

            switch (provisionType)
            {
                case ProvisionTypeEnum.SelfProduced:
                    definition.AddCollectionWithRemove(i => i.SelfProducers?.Select(x => new VmListItem { Id = x, OwnerReferenceId = i.Id, }), o => o.Organizations, r => true);
                    break;
                case ProvisionTypeEnum.PurchaseServices:
                case ProvisionTypeEnum.Other:

                    if (vModel.Organization.IsAssigned())
                    {
                        definition.AddCollectionWithRemove(
                            i => new List<VmListItem>{new VmListItem {Id = i.Organization.Value, OwnerReferenceId = i.Id}}, o => o.Organizations, r => true);

                        definition.AddCollectionWithRemove(i => new List<VmServiceProducerAdditionalInformation>(),
                            o => o.AdditionalInformations, TranslationPolicy.FetchData, r => true);
                    }
                    else
                    {
                        definition.AddCollectionWithRemove(i => new List<VmListItem>(), o => o.Organizations,
                            TranslationPolicy.FetchData, r => true);

                        definition.AddCollectionWithRemove(i => i.AdditionalInformation
                            ?.Select(
                                pair => new VmServiceProducerAdditionalInformation()
                                {
                                    ServiceProducerId = i.Id,
                                    Text = pair.Value,
                                    LocalizationId = languageCache.Get(pair.Key)
                                }), o => o.AdditionalInformations, r => true);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(provisionType), provisionType, null);
            }

            return definition.GetFinal();
        }
    }
}
