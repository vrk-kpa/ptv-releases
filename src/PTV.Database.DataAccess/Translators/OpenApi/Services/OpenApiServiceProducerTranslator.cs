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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<ServiceProducer, VmOpenApiServiceProducer>), RegisterType.Transient)]
    internal class OpenApiServiceProducerTranslator : Translator<ServiceProducer, VmOpenApiServiceProducer>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceProducerTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiServiceProducer TranslateEntityToVm(ServiceProducer entity)
        {
            var provisionType = typesCache.GetByValue<ProvisionType>(entity.ProvisionTypeId);
            var definition = CreateEntityViewModelDefinition<VmOpenApiServiceProducer>(entity)
                .AddSimple(input => input.ServiceVersionedId, output => output.OwnerReferenceId);

            definition.AddSimple(i => i.OrderNumber ?? 0, o => o.OrderNumber);
            definition.AddNavigation(i => string.IsNullOrEmpty(provisionType) ? null : provisionType.GetOpenApiEnumValue<ProvisionTypeEnum>(), o => o.ProvisionType);
            definition.AddCollection(i => i.AdditionalInformations, o => o.AdditionalInformation);
            definition.AddCollection(i => i.Organizations, o => o.Organizations);
            return definition.GetFinal();
        }

        public override ServiceProducer TranslateVmToEntity(VmOpenApiServiceProducer vModel)
        {
            throw new NotImplementedException("Translator VmOpenApiServiceProducer -> ServiceProducer is not implemented");
        }
    }
}
