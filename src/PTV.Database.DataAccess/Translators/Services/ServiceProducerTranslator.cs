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
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceProducer, VmServiceProducer>), RegisterType.Transient)]
    internal class ServiceProducerTranslator : Translator<ServiceProducer, VmServiceProducer>
    {
        private readonly ITypesCache typesCache;
        public ServiceProducerTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmServiceProducer TranslateEntityToVm(ServiceProducer entity)
        {
            var definition = CreateEntityViewModelDefinition<VmServiceProducer>(entity)
                .AddSimple(input => input.Id, output => output.Id)
                .AddSimple(input => input.ProvisionTypeId, output => output.ProvisionTypeId)
                .AddSimple(input => input.OrderNumber, output => output.Order)
                .AddSimple(input => input.ServiceVersionedId, output => output.OwnerReferenceId);
                
            var provisionType = typesCache.GetByValue<ProvisionType>(entity.ProvisionTypeId);
            var provisionTypeEnum = Enum.Parse(typeof(ProvisionTypeEnum), provisionType);
            switch (provisionTypeEnum)
            {
                case ProvisionTypeEnum.SelfProduced:
                    definition.AddSimpleList(input => input.Organizations.Select(o => o.OrganizationId).ToList(), output => output.Organizers);
                    break;
                case ProvisionTypeEnum.PurchaseServices:
                    definition.AddLocalizable(input => input.AdditionalInformations, output => output.AdditionalInformation);
                    var purchaseServiceOrganization = entity.Organizations.FirstOrDefault();
                    if (purchaseServiceOrganization != null)
                    {
                        definition.AddSimple(input => purchaseServiceOrganization.OrganizationId, output => output.OrganizationId);
                    }
                    break;
                case ProvisionTypeEnum.Other:
                    definition.AddLocalizable(input => input.AdditionalInformations, output => output.AdditionalInformation);
                    var organization = entity.Organizations.FirstOrDefault();
                    if (organization != null)
                    {
                        definition.AddSimple(input => organization.OrganizationId, output => output.OrganizationId);
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
                .AddSimple(input => input.ProvisionTypeId, output => output.ProvisionTypeId)
                .AddSimple(input => input.Order, output => output.OrderNumber);

            ProvisionTypeEnum provisionType;
            if (!Enum.TryParse(typesCache.GetByValue<ProvisionType>(vModel.ProvisionTypeId), out provisionType))
            {
                return definition.GetFinal();
            }

            switch (provisionType)
            {
                case ProvisionTypeEnum.SelfProduced:
                    definition.AddCollectionWithRemove(i => i.Organizers?.Select(x => new VmListItem { Id = x, OwnerReferenceId = i.Id, }), o => o.Organizations, r => true);
                    break;

                case ProvisionTypeEnum.PurchaseServices:

                    var organizations = vModel.OrganizationId.IsAssigned()
                        ? new List<VmListItem> {new VmListItem {Id = vModel.OrganizationId.Value, OwnerReferenceId = vModel.Id}}
                        : new List<VmListItem>();

                    definition.AddCollectionWithRemove(i => organizations, o => o.Organizations, r => true);

                    if (!string.IsNullOrEmpty(vModel.AdditionalInformation))
                    {
                        definition.AddNavigationOneMany(i => i, o => o.AdditionalInformations);
                    }
                    else
                    {
                        definition.AddCollectionWithRemove(i => new List<VmServiceProducer>(), o => o.AdditionalInformations, TranslationPolicy.FetchData, r => true);
                    }
                    break;

                case ProvisionTypeEnum.Other:
                    if (vModel.OrganizationId.IsAssigned())
                    {
                        definition.AddCollectionWithRemove(i => new List<VmListItem> { new VmListItem { Id = i.OrganizationId.Value, OwnerReferenceId = i.Id } }, o => o.Organizations, r => true);
                        definition.AddCollectionWithRemove(i => new List<VmServiceProducer>(), o => o.AdditionalInformations, TranslationPolicy.FetchData, r => true);
                    }
                    else
                    {
                        definition.AddCollectionWithRemove(i => new List<VmListItem>(), o => o.Organizations, TranslationPolicy.FetchData, r => true);
                        if (string.IsNullOrEmpty(vModel.AdditionalInformation))
                        {
                            definition.AddCollectionWithRemove(i => new List<VmServiceProducer>(), o => o.AdditionalInformations, TranslationPolicy.FetchData, r => true);
                        }
                        else
                        {
                            definition.AddNavigationOneMany(i => i, o => o.AdditionalInformations);
                        }
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(provisionType), provisionType, null);
            }

            return definition.GetFinal();
        }
    }
}
