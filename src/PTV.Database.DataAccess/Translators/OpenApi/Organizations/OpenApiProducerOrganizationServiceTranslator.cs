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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.OpenApi.V10;
using System;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    [RegisterService(typeof(ITranslator<ServiceProducerOrganization, V10VmOpenApiOrganizationService>), RegisterType.Transient)]
    internal class OpenApiProducerOrganizationServiceTranslator : Translator<ServiceProducerOrganization, V10VmOpenApiOrganizationService>
    {
        private ITypesCache typeCache;

        public OpenApiProducerOrganizationServiceTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typeCache = cacheManager.TypesCache;
        }

        public override V10VmOpenApiOrganizationService TranslateEntityToVm(ServiceProducerOrganization entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => "Producer", o => o.RoleType)
                .AddNavigation(i => typeCache.GetByValue<ProvisionType>(i.ServiceProducer.ProvisionTypeId), o => o.ProvisionType)
                .AddNavigation(i => i.ServiceProducer.ServiceVersioned, o => o.Service)
                .GetFinal();
        }

        public override ServiceProducerOrganization TranslateVmToEntity(V10VmOpenApiOrganizationService vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiProducerOrganizationServiceTranslator.");
        }
    }
}
