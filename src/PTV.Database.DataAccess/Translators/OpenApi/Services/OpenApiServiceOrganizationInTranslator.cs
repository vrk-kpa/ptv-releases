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
using PTV.Framework;
using PTV.Database.Model.Models;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi.V4;
using System;
using PTV.Domain.Model.Models.OpenApi.V6;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<OrganizationService, V6VmOpenApiServiceOrganizationIn>), RegisterType.Transient)]
    internal class OpenApiServiceOrganizationInTranslator : Translator<OrganizationService, V6VmOpenApiServiceOrganizationIn>
    {
        private ITypesCache typesCache;

        public OpenApiServiceOrganizationInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V6VmOpenApiServiceOrganizationIn TranslateEntityToVm(OrganizationService entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiServiceOrganizationTranslator!");
        }

        public override OrganizationService TranslateVmToEntity(V6VmOpenApiServiceOrganizationIn vModel)
        {
            var organizationId = vModel.OrganizationId.ParseToGuid();

            var roleTypeId = typesCache.Get<RoleType>(vModel.RoleType);
            Guid? provisionTypeId = null;
            if (!string.IsNullOrEmpty(vModel.ProvisionType)) provisionTypeId = typesCache.Get<ProvisionType>(vModel.ProvisionType);

            var definition = CreateViewModelEntityDefinition<OrganizationService>(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => vModel.OwnerReferenceId.IsAssigned(), i => o => i.OwnerReferenceId == o.ServiceVersionedId && o.OrganizationId == organizationId &&
                                                                                           roleTypeId == o.RoleTypeId && o.ProvisionTypeId == provisionTypeId, s => s.UseDataContextCreate(x => true));

            if (organizationId.IsAssigned())
            {
                definition.AddSimple(i => organizationId, o => o.OrganizationId);
            }
            definition.AddNavigation(i => i.RoleType, o => o.RoleType);
            definition.AddNavigation(i => i.ProvisionType, o => o.ProvisionType);
            definition.AddCollection(i => i.AdditionalInformation, o => o.AdditionalInformations, false);
            return definition.GetFinal();
        }
    }
}
