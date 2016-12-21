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
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using PTV.Database.DataAccess.Caches;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    internal abstract class OpenApiOrganizationServiceBaseTranslator<TVmOpenApiOrganizationService> : Translator<OrganizationService, TVmOpenApiOrganizationService> where TVmOpenApiOrganizationService : class, IVmOpenApiOrganizationService
    {
        private readonly ITypesCache typesCache;

        protected OpenApiOrganizationServiceBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override TVmOpenApiOrganizationService TranslateEntityToVm(OrganizationService entity)
        {
            return CreateBaseEntityVmDefinitions(entity).GetFinal();
        }
        public override OrganizationService TranslateVmToEntity(TVmOpenApiOrganizationService vModel)
        {
            return CreateBaseVmEntityDefinitions(vModel).GetFinal();
        }

        protected ITranslationDefinitions<OrganizationService, TVmOpenApiOrganizationService> CreateBaseEntityVmDefinitions(OrganizationService entity)
        {
            return CreateEntityViewModelDefinition<TVmOpenApiOrganizationService>(entity)
                .AddNavigation(i => i.ServiceId.ToString(), o => o.ServiceId)
                .AddNavigation(i => typesCache.GetByValue<RoleType>(i.RoleTypeId), o => o.RoleType)
                .AddNavigation(i => i.ProvisionTypeId.HasValue ? typesCache.GetByValue<ProvisionType>(i.ProvisionTypeId.Value) : null, o => o.ProvisionType)
                .AddCollection(i => i.AdditionalInformations, o => o.AdditionalInformation)
                .AddCollection(i => i.WebPages, o => o.WebPages);
        }

        protected ITranslationDefinitions<TVmOpenApiOrganizationService, OrganizationService> CreateBaseVmEntityDefinitions(TVmOpenApiOrganizationService vModel)
        {
            Guid organizationId, serviceId;
            organizationId = Guid.TryParse(vModel.OrganizationId, out organizationId) ? organizationId : Guid.Empty;
            serviceId = Guid.TryParse(vModel.ServiceId, out serviceId) ? serviceId : Guid.Empty;

            var definition = CreateViewModelEntityDefinition<OrganizationService>(vModel)
                .DisableAutoTranslation();

            if (organizationId == Guid.Empty && string.IsNullOrEmpty(vModel.ProvisionType))
            {
                definition.UseDataContextUpdate(i => vModel.OwnerReferenceId.IsAssigned(), i => o => i.OwnerReferenceId == o.ServiceId && o.OrganizationId == null &&
                    i.RoleType == o.RoleType.Code && o.ProvisionTypeId == null, s => s.UseDataContextCreate(x => true));
            }
            else if (organizationId == Guid.Empty)
            {
                definition.UseDataContextUpdate(i => vModel.OwnerReferenceId.IsAssigned(), i => o => i.OwnerReferenceId == o.ServiceId && o.OrganizationId == null &&
                    i.RoleType == o.RoleType.Code && o.ProvisionType.Code == i.ProvisionType, s => s.UseDataContextCreate(x => true));
            }
            else if (string.IsNullOrEmpty(vModel.ProvisionType))
            {
                definition.UseDataContextUpdate(i => vModel.OwnerReferenceId.IsAssigned(), i => o => i.OwnerReferenceId == o.ServiceId && o.OrganizationId == organizationId &&
                    i.RoleType == o.RoleType.Code && o.ProvisionTypeId == null, s => s.UseDataContextCreate(x => true));
            }
            else
            {
                definition.UseDataContextUpdate(i => vModel.OwnerReferenceId.IsAssigned(), i => o => i.OwnerReferenceId == o.ServiceId && o.OrganizationId == organizationId &&
                    i.RoleType == o.RoleType.Code && o.ProvisionType.Code == i.ProvisionType, s => s.UseDataContextCreate(x => true));
            }

            if (serviceId.IsAssigned())
            {
                definition.AddSimple(i => serviceId, o => o.ServiceId);
                var entity = definition.GetFinal();
                if (entity.Created != DateTime.MinValue)
                {
                    vModel.AdditionalInformation.ForEach(a => a.OwnerReferenceId = entity.Id);
                    vModel.WebPages.ForEach(w => w.OwnerReferenceId = entity.Id);
                }
            }

            definition.AddNavigation(i => i.RoleType, o => o.RoleType);
            definition.AddNavigation(i => i.ProvisionType, o => o.ProvisionType);

            if (organizationId.IsAssigned())
            {
                definition.AddSimple(i => organizationId, o => o.OrganizationId);
            }

            definition.AddCollection(i => i.AdditionalInformation, o => o.AdditionalInformations);
            definition.AddCollection(i => i.WebPages, o => o.WebPages);

            return definition;
        }
    }
}
