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
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi.V5;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationService, V5VmOpenApiOrganizationService>), RegisterType.Transient)]
    internal class OpenApiOrganizationServiceTranslator : OpenApiOrganizationServiceBaseTranslator<V5VmOpenApiOrganizationService>
    {
        public OpenApiOrganizationServiceTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
        }

        public override V5VmOpenApiOrganizationService TranslateEntityToVm(OrganizationService entity)
        {
            var definition = base.CreateBaseEntityVmDefinitions(entity)
                .AddNavigation(i => i.OrganizationId.ToString(), o => o.OrganizationId);

            if (entity.ServiceVersioned != null)
            {
                definition
                    .AddNavigation(i => i.ServiceVersioned, o => o.Service);
            }

            return definition.GetFinal();
        }

        public override OrganizationService TranslateVmToEntity(V5VmOpenApiOrganizationService vModel)
        {
            return base.TranslateVmToEntity(vModel);
        }
    }
}
