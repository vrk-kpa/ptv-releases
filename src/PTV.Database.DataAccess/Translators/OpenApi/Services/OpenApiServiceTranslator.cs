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
using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<ServiceVersioned, VmOpenApiServiceVersionBase>), RegisterType.Transient)]
    internal class OpenApiServiceTranslator : OpenApiServiceBaseTranslator<VmOpenApiServiceVersionBase>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives, cacheManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiServiceVersionBase TranslateEntityToVm(ServiceVersioned entity)
        {
            return base.CreateBaseEntityVmDefinitions(entity)
                // We have to use unique root id as id!
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddNavigation(i => i.ServiceChargeTypeId.HasValue ? typesCache.GetByValue<ServiceChargeType>(i.ServiceChargeTypeId.Value) : null, o => o.ServiceChargeType)
                .AddSimple(i => i.StatutoryServiceGeneralDescriptionId, o => o.StatutoryServiceGeneralDescriptionId)
                .AddCollection(i => i.ServiceMunicipalities, o => o.Municipalities)
                .AddCollection(i => i.ServiceServiceClasses.Select(j => j.ServiceClass).ToList(), o => o.ServiceClasses)
                .AddCollection(i => i.ServiceOntologyTerms.Select(j => j.OntologyTerm).ToList(), o => o.OntologyTerms)
                .AddCollection(i => i.ServiceTargetGroups.Select(j => j.TargetGroup).ToList(), o => o.TargetGroups)
                .AddCollection(i => i.ServiceLifeEvents.Select(j => j.LifeEvent).ToList(), o => o.LifeEvents)
                .AddCollection(i => i.ServiceIndustrialClasses.Select(j => j.IndustrialClass).ToList(), o => o.IndustrialClasses)
                .AddCollection(i => i.ServiceLaws.Select(j => j.Law), o => o.Legislation)
                //.AddSimple(i => i.ElectronicNotification, o => o.ElectronicNotification)   // These are not included in first release version
                //.AddSimple(i => i.ElectronicCommunication, o => o.ElectronicCommunication) // - maybe in some future release.
                .AddCollection(i => i.ServiceServiceChannels, o => o.ServiceChannels)
                .AddCollection(i => i.OrganizationServices, o => o.Organizations)
                .AddNavigation(i => i.TypeId.HasValue ? typesCache.GetByValue<ServiceType>(i.TypeId.Value) : null, o => o.Type)
                .GetFinal();
        }

        public override ServiceVersioned TranslateVmToEntity(VmOpenApiServiceVersionBase vModel)
        {
            throw new NotImplementedException();
        }
    }
}
