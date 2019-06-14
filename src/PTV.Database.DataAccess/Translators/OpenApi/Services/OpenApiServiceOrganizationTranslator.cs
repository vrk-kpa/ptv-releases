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
using PTV.Database.DataAccess.Translators.OpenApi.Organizations;
using PTV.Framework;
using PTV.Database.Model.Models;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi.V6;
using System;
using System.Linq;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<OrganizationService, V6VmOpenApiServiceOrganization>), RegisterType.Transient)]
    internal class OpenApiServiceOrganizationTranslator : OpenApiOrganizationServiceBaseTranslator<V6VmOpenApiServiceOrganization>
    {

        private readonly ITypesCache typesCache;

        public OpenApiServiceOrganizationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V6VmOpenApiServiceOrganization TranslateEntityToVm(OrganizationService entity)
        {
            // These are all other responsible organizations
            var definition = CreateBaseEntityVmDefinitions(entity)
                .AddNavigation(i => "OtherResponsible", o => o.RoleType);

            if (entity.Organization != null && entity.Organization.Versions?.Count > 0)
            {
                var publishedId = typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString());
                definition.AddNavigation(i => i.Organization.Versions.FirstOrDefault(x => x.PublishingStatusId == publishedId), o => o.Organization);
            }
            else if (entity.OrganizationId.IsAssigned())
            {
                var organization = new OrganizationVersioned() { UnificRootId = entity.OrganizationId};
                definition.AddNavigation(i => organization, o => o.Organization);
            }
            else
            {
                definition.AddNavigation(i => (OrganizationVersioned)null, o => o.Organization);
            }

            return definition.GetFinal();
        }

        public override OrganizationService TranslateVmToEntity(V6VmOpenApiServiceOrganization vModel)
        {

            throw new NotImplementedException("Translator V6VmOpenApiServiceOrganization -> OrganizationService is not implemented");
        }
    }
}
