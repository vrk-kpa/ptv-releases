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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<OrganizationService, VmServiceProducer>), RegisterType.Transient)]
    internal class ServiceProducerTranslator : Translator<OrganizationService, VmServiceProducer>
    {
        private readonly ITypesCache typesCache;
        public ServiceProducerTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmServiceProducer TranslateEntityToVm(OrganizationService entity)
        {
            var definition = CreateEntityViewModelDefinition<VmServiceProducer>(entity)
                .AddSimple(input => input.Id, output => output.Id)
                .AddSimple(input => input.ProvisionTypeId, output => output.ProvisionTypeId);

            var provisionType = entity.ProvisionType;
            if (provisionType != null)
            {
                var type = (ProvisionTypeEnum) Enum.Parse(typeof(ProvisionTypeEnum), provisionType.Code);
                switch (type)
                {
                    case ProvisionTypeEnum.SelfProduced:
                        definition.AddNavigation(input => input, output => output.SelfProduced);
                        break;
                    case ProvisionTypeEnum.VoucherServices:
                        definition.AddNavigation(input => input, output => output.VoucherServices);
                        break;
                    case ProvisionTypeEnum.PurchaseServices:
                        definition.AddNavigation(input => input, output => output.PurchaseServices);
                        break;
                    case ProvisionTypeEnum.Other:
                        definition.AddNavigation(input => input, output => output.Other);
                        break;

                }
            }

            return definition.GetFinal();
        }

        public override OrganizationService TranslateVmToEntity(VmServiceProducer vModel)
        {
            bool exists = vModel.Id.IsAssigned();

            var definition = CreateViewModelEntityDefinition<OrganizationService>(vModel)
                .UseDataContextCreate(input => !exists, output => output.Id, input => Guid.NewGuid())
                .UseDataContextLocalizedUpdate(input => exists, input => output => input.Id == output.Id)
                .AddSimple(input => input.ProvisionTypeId, output => output.ProvisionTypeId)
                .AddSimple(input => typesCache.Get<RoleType>(RoleTypeEnum.Producer.ToString()), output => output.RoleTypeId);
//                .AddNavigation(input => input.Order, output => output.)

            ProvisionTypeEnum provisionType;
            if (vModel.ProvisionTypeId.HasValue && Enum.TryParse(typesCache.GetByValue<ProvisionType>(vModel.ProvisionTypeId.Value), out provisionType))
            {
                switch (provisionType)
                {
                    case ProvisionTypeEnum.SelfProduced:
                        definition.AddSimple(
                            input => input.SelfProduced.SubProducerOrganizationId ?? input.SelfProduced.ProducerOrganizationId,
                            output => output.OrganizationId);
                        break;
                    case ProvisionTypeEnum.VoucherServices:
                        definition.AddSimple(input => (Guid?)null, output => output.OrganizationId);
                        if (exists || !string.IsNullOrEmpty(vModel.VoucherServices.FreeDescription))
                        {
                            definition.AddCollection(input => GetUpdateModel(input.VoucherServices, vModel.Id), output => output.AdditionalInformations);
                        }
                        if (exists || !string.IsNullOrEmpty(vModel.VoucherServices.Link))
                        {
                            definition.AddCollection(input => GetUpdateModel(input.VoucherServices, vModel.Id), output => output.WebPages);
                        }
                        break;
                    case ProvisionTypeEnum.PurchaseServices:
                        definition.AddSimple(input => input.PurchaseServices.ProducerOrganizationId, output => output.OrganizationId);
                        if (!vModel.PurchaseServices.ProducerOrganizationId.HasValue && (exists || !string.IsNullOrEmpty(vModel.PurchaseServices.FreeDescription)))
                        {
                            definition.AddCollection(
                                input => GetUpdateModel(input.PurchaseServices, vModel.Id),
                                output => output.AdditionalInformations);
                        }
                        break;
                    case ProvisionTypeEnum.Other:
                        definition.AddSimple(input => input.Other.ProducerOrganizationId, output => output.OrganizationId);
                        break;

                }

                var entity = definition.GetFinal();
                return entity;
            }
            return null;
        }

        private IEnumerable<VmServiceProducerDetail> GetUpdateModel(VmServiceProducerDetail model, Guid? id)
        {
            model.OwnerReferenceId = id;
            return new List<VmServiceProducerDetail> { model };
        }
    }
}
