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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<Service, IV2VmOpenApiServiceInBase>), RegisterType.Transient)]
    internal class OpenApiServiceInTranslator : OpenApiServiceBaseTranslator<IV2VmOpenApiServiceInBase>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override IV2VmOpenApiServiceInBase TranslateEntityToVm(Service entity)
        {
            return base.TranslateEntityToVm(entity);
        }
        public override Service TranslateVmToEntity(IV2VmOpenApiServiceInBase vModel)
        {
            if (vModel.Id.IsAssigned())
            {
                vModel.ServiceOrganizations.ForEach(o => { o.ServiceId = vModel.Id.Value.ToString(); o.OwnerReferenceId = vModel.Id; });
                vModel.Keywords.ForEach(k => k.OwnerReferenceId = vModel.Id);
            }
            var definitions = base.CreateBaseVmEntityDefinitions(vModel);

            if (!string.IsNullOrEmpty(vModel.Type))
            {
                definitions.AddSimple(i => typesCache.Get<ServiceType>(i.Type), o => o.TypeId);
            }

            // set chargeType
            var chargeType = (string.Compare(vModel.ServiceChargeType, ServiceChargeTypeEnum.Other.ToString(), StringComparison.CurrentCultureIgnoreCase) == 0)
                    ? null
                    : vModel.ServiceChargeType;

            definitions.AddSimple(i => (chargeType == null) ? (Guid?) null : typesCache.Get<ServiceChargeType>(chargeType), o => o.ServiceChargeTypeId);

            if (vModel.ServiceClasses?.Count > 0)
            {
                definitions.AddCollection(i => i.ServiceClasses, o => o.ServiceServiceClasses);
            }

            if (vModel.OntologyTerms?.Count > 0)
            {
                definitions.AddCollection(i => i.OntologyTerms, o => o.ServiceOntologyTerms);
            }

            if (vModel.TargetGroups?.Count > 0)
            {
                definitions.AddCollection(i => i.TargetGroups, o => o.ServiceTargetGroups);
            }

            if (vModel.LifeEvents?.Count > 0)
            {
                definitions.AddCollection(i => i.LifeEvents, o => o.ServiceLifeEvents);
            }

            if (vModel.IndustrialClasses?.Count > 0)
            {
                definitions.AddCollection(i => i.IndustrialClasses, o => o.ServiceIndustrialClasses);
            }

            if (vModel.Keywords?.Count > 0)
            {
                definitions.AddCollection(i => i.Keywords, o => o.ServiceKeywords);
            }

            if (vModel.ServiceOrganizations?.Count > 0)
            {
                definitions.AddCollection(i => i.ServiceOrganizations, o => o.OrganizationServices);
            }

            if (!string.IsNullOrEmpty(vModel.StatutoryServiceGeneralDescriptionId))
            {
                Guid id;
                id = Guid.TryParse(vModel.StatutoryServiceGeneralDescriptionId, out id) ? id : Guid.Empty;
                definitions = definitions.AddSimple(i => id, o => o.StatutoryServiceGeneralDescriptionId);
            }

            return definitions.GetFinal();
        }
    }
}
