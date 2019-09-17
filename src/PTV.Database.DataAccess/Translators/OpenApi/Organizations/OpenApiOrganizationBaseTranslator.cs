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
using PTV.Framework.Interfaces;
using System.Linq;
using System;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Enums;
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    internal abstract class OpenApiOrganizationBaseTranslator<TVmOpenApiOrganization> : Translator<OrganizationVersioned, TVmOpenApiOrganization> where TVmOpenApiOrganization : class, IVmOpenApiOrganizationBase
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        protected OpenApiOrganizationBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override TVmOpenApiOrganization TranslateEntityToVm(OrganizationVersioned entity)
        {
            return CreateBaseEntityVmDefinitions(entity).GetFinal();
        }

        public override OrganizationVersioned TranslateVmToEntity(TVmOpenApiOrganization vModel)
        {
            return CreateBaseVmEntityDefinitions(vModel).GetFinal();
        }

        protected ITranslationDefinitions<OrganizationVersioned, TVmOpenApiOrganization> CreateBaseEntityVmDefinitions(OrganizationVersioned entity)
        {
            var aiType = typesCache.GetByValue<AreaInformationType>(entity.AreaInformationTypeId);
            return CreateEntityViewModelDefinition<TVmOpenApiOrganization>(entity)
                .AddNavigation(i => i.Oid, o => o.Oid)
                .AddNavigation(i => i.Business?.Code, o => o.BusinessCode)
                .AddNavigation(i => i.Business?.Name, o => o.BusinessName)
                .AddNavigation(i => i.TypeId.HasValue ? typesCache.GetByValue<OrganizationType>(i.TypeId.Value) : null, o => o.OrganizationType)
                .AddCollection(i => i.OrganizationNames, o => o.OrganizationNames)
                .AddCollection(i => i.OrganizationDisplayNameTypes, o => o.DisplayNameType)
                .AddCollection(i => i.LanguageAvailabilities.Select(l => languageCache.GetByValue(l.LanguageId)).ToList(), o => o.AvailableLanguages)
                // Area information types changed into Nationwide, NationwideExceptAlandIslands and LimitedType (PTV-2184)
                .AddNavigation(i => string.IsNullOrEmpty(aiType) ? null : aiType.GetOpenApiEnumValue<AreaInformationTypeEnum>(), o => o.AreaType);
        }

        protected ITranslationDefinitions<TVmOpenApiOrganization, OrganizationVersioned> CreateBaseVmEntityDefinitions(TVmOpenApiOrganization vModel)
        {
            var exists = vModel.VersionId.IsAssigned();

            // Update the organizationId for needed properties if we are updating organization data
            if (exists)
            {
                vModel.OrganizationNames.ForEach(n => n.OwnerReferenceId = vModel.VersionId.Value);
                vModel.DisplayNameType.ForEach(n => n.OwnerReferenceId = vModel.VersionId.Value);
            }
            
            // Set default values for AreaType
            if (vModel.AreaType.IsNullOrEmpty() && (vModel.OrganizationType == OrganizationTypeEnum.Municipality.ToString() ||
                vModel.OrganizationType == OrganizationTypeEnum.TT1.ToString() ||
                vModel.OrganizationType == OrganizationTypeEnum.TT2.ToString()))
            {
                vModel.AreaType = AreaInformationTypeEnum.WholeCountry.ToString();
            }

            var translationDefinition = CreateViewModelEntityDefinition<OrganizationVersioned>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(input => !exists, output => output.Id, input => Guid.NewGuid())
                .UseDataContextUpdate(input => exists, input => output => input.VersionId.Value == output.Id)
                .UseVersioning<OrganizationVersioned, Organization>(o => o);

            if (!exists || !vModel.AreaType.IsNullOrEmpty())
            {
                //Let's use default value for area type if no are type is given
                var typeID = vModel.AreaType.IsNullOrEmpty() ? typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString()) : typesCache.Get<AreaInformationType>(vModel.AreaType);
                translationDefinition.AddSimple(i => typeID, o => o.AreaInformationTypeId);
            }
           
            if (vModel.OrganizationNames != null)
            {
                translationDefinition.AddCollectionWithKeep(i => i.OrganizationNames, o => o.OrganizationNames, x => true);
            }
            
            if (!string.IsNullOrEmpty(vModel.Oid))
            {
                translationDefinition.AddNavigation(i => i.Oid, o => o.Oid);
            }

            if (!string.IsNullOrEmpty(vModel.OrganizationType))
            {
                translationDefinition.AddSimple(i => typesCache.Get<OrganizationType>(i.OrganizationType), o => o.TypeId);
            }

            if (vModel.DisplayNameType?.Count > 0)
            {
                translationDefinition.AddCollectionWithKeep(i => i.DisplayNameType, o => o.OrganizationDisplayNameTypes, x => true);
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
