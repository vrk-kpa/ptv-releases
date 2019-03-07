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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<ServiceProducerOrganization, VmOpenApiServiceProducerOrganizationIn>), RegisterType.Transient)]
    internal class OpenApiServiceProducerOrganizationInTranslator : Translator<ServiceProducerOrganization, VmOpenApiServiceProducerOrganizationIn>
    {
        public OpenApiServiceProducerOrganizationInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {}

        public override VmOpenApiServiceProducerOrganizationIn TranslateEntityToVm(ServiceProducerOrganization entity)
        {
            throw new NotImplementedException("Translator ServiceProducerOrganization -> VmOpenApiServiceProducerOrganizationIn is not implemented.");
        }

        public override ServiceProducerOrganization TranslateVmToEntity(VmOpenApiServiceProducerOrganizationIn vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => vModel.OwnerReferenceId.IsAssigned(), i => o => i.OwnerReferenceId == o.ServiceProducerId && i.OrganizationId == o.OrganizationId,
                    s => s.UseDataContextCreate(x => true));
            definition.AddSimple(i => i.OrganizationId, o => o.OrganizationId);
            return definition.GetFinal();
        }
    }
}
