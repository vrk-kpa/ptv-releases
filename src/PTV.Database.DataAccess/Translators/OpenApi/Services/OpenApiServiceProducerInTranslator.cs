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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<ServiceProducer, VmOpenApiServiceProducerIn>), RegisterType.Transient)]
    internal class OpenApiServiceProducerInTranslator : Translator<ServiceProducer, VmOpenApiServiceProducerIn>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceProducerInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiServiceProducerIn TranslateEntityToVm(ServiceProducer entity)
        {
            throw new NotImplementedException("Translator ServiceProducer -> VmOpenApiServiceProducerIn is not implemented");
        }

        public override ServiceProducer TranslateVmToEntity(VmOpenApiServiceProducerIn vModel)
        {
            if (vModel == null)
            {
                return null;
            }

            var exists = vModel.Id.IsAssigned();
            vModel.OwnerReferenceId = vModel.OwnerReferenceId ?? Guid.Empty;

            var definition = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextUpdate(input => exists, input => output => (input.Id == output.Id) && 
                        (!input.OwnerReferenceId.IsAssigned() || output.ServiceVersionedId == vModel.OwnerReferenceId));

            definition.AddSimple(i => i.OrderNumber, o => o.OrderNumber);
            definition.AddSimple(i => typesCache.Get<ProvisionType>(i.ProvisionType), o => o.ProvisionTypeId);
            definition.AddCollection(i => i.Organizations?.Select(oId => new VmOpenApiServiceProducerOrganizationIn{OrganizationId = oId, OwnerReferenceId = vModel.OwnerReferenceId}), o => o.Organizations);
            definition.AddCollection(i => i.AdditionalInformation, o => o.AdditionalInformations);
            return definition.GetFinal();
        }
    }
}
