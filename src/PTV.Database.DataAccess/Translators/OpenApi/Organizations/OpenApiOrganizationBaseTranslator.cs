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
using PTV.Framework.Interfaces;
using System.Linq;
using System;
using System.Collections.Generic;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Database.DataAccess.Caches;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    internal abstract class OpenApiOrganizationBaseTranslator<TVmOpenApiOrganization> : Translator<Organization, TVmOpenApiOrganization> where TVmOpenApiOrganization : class, IVmOpenApiOrganizationBase
    {
        private readonly ITypesCache typesCache;

        protected OpenApiOrganizationBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override TVmOpenApiOrganization TranslateEntityToVm(Organization entity)
        {
            return CreateBaseEntityVmDefinitions(entity).GetFinal();
        }

        public override Organization TranslateVmToEntity(TVmOpenApiOrganization vModel)
        {
            return CreateBaseVmEntityDefinitions(vModel).GetFinal();
        }

        protected ITranslationDefinitions<Organization, TVmOpenApiOrganization> CreateBaseEntityVmDefinitions(Organization entity)
        {
            return CreateEntityViewModelDefinition<TVmOpenApiOrganization>(entity)
                .AddNavigation(i => i.Oid, o => o.Oid)
                .AddNavigation(i => i.Municipality?.Name, o => o.Municipality)
                .AddNavigation(i => i.Business?.Code, o => o.BusinessCode)
                .AddNavigation(i => i.Business?.Name, o => o.BusinessName)
                .AddNavigation(i => typesCache.GetByValue<OrganizationType>(i.TypeId), o => o.OrganizationType)
                .AddCollection(i => i.OrganizationNames, o => o.OrganizationNames)
                .AddNavigation(i => i.PublishingStatus, o => o.PublishingStatus)
                .AddNavigation(i => typesCache.GetByValue<NameType>(i.DisplayNameTypeId), o => o.DisplayNameType);
        }

        protected ITranslationDefinitions<TVmOpenApiOrganization, Organization> CreateBaseVmEntityDefinitions(TVmOpenApiOrganization vModel)
        {
            // Update the organizationId for needed properties if we are updating organization data
            if (vModel.Id.HasValue)
            {
                vModel.OrganizationNames.ForEach(n => n.OwnerReferenceId = vModel.Id);
            }
            var translationDefinition = CreateViewModelEntityDefinition<Organization>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(input => !input.Id.HasValue, output => output.Id, input => Guid.NewGuid())
                .UseDataContextUpdate(input => input.Id.HasValue, input => output => input.Id.Value == output.Id);

            if (vModel.OrganizationNames != null)
            {
                translationDefinition.AddCollection(i => i.OrganizationNames, o => o.OrganizationNames);
            }
            if (!string.IsNullOrEmpty(vModel.PublishingStatus))
            {
                translationDefinition.AddNavigation(i => i.PublishingStatus, o => o.PublishingStatus);
            }
            if (!string.IsNullOrEmpty(vModel.Oid))
            {
                translationDefinition.AddNavigation(i => i.Oid, o => o.Oid);
            }
            if (!string.IsNullOrEmpty(vModel.Municipality))
            {
                translationDefinition.AddNavigation(i => i.Municipality, o => o.Municipality);
            }
            if (!string.IsNullOrEmpty(vModel.OrganizationType))
            {
                translationDefinition.AddSimple(i => typesCache.Get<OrganizationType>(i.OrganizationType), o => o.TypeId);
            }
            if (!string.IsNullOrEmpty(vModel.DisplayNameType))
            {
                translationDefinition.AddSimple(i => typesCache.Get<NameType>(i.DisplayNameType), o => o.DisplayNameTypeId);
            }
            if (!string.IsNullOrEmpty(vModel.BusinessCode) || !string.IsNullOrEmpty(vModel.BusinessName))
            {
                var vmBusiness = new VmOpenApiBusiness()
                {
                    Code = vModel.BusinessCode,
                    Name = vModel.BusinessName,
                    Id = vModel.BusinessId.HasValue ? vModel.BusinessId.Value : Guid.Empty
                };

                translationDefinition.AddNavigation(i => vmBusiness, o => o.Business);
            }

            return translationDefinition;
        }
    }
}
