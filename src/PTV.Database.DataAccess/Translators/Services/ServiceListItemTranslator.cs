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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceVersioned, VmServiceListItem>), RegisterType.Transient)]
    internal class ServiceListItemTranslator : Translator<ServiceVersioned, VmServiceListItem>
    {
        private ITypesCache typesCache;

        public ServiceListItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache)
            : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmServiceListItem TranslateEntityToVm(ServiceVersioned entity)
        {

            var definition = CreateEntityViewModelDefinition<VmServiceListItem>(entity)
                .AddNavigation(i => VersioningManager.ApplyLanguageFilterFallback(i.ServiceNames.Where(k => k.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())), RequestLanguageId)?.Name ?? "N/A", o => o.Name)
                .AddSimple(i => i.PublishingStatusId, o => o.PublishingStatusId)
                .AddSimple(i => i.UnificRootId, o => o.UnificRootId)
                .AddSimple(i => i.TypeId ?? VersioningManager.ApplyPublishingStatusFilterFallback(i.StatutoryServiceGeneralDescription?.Versions).TypeId, o => o.ServiceTypeId)
                .AddNavigation(
                    i =>
                        typesCache.GetByValue<ServiceType>(i.TypeId ?? VersioningManager.ApplyPublishingStatusFilterFallback(i.StatutoryServiceGeneralDescription?.Versions).TypeId),
                    o => o.ServiceType)
                .AddCollection(i =>
                        (VersioningManager.ApplyPublishingStatusFilterFallback(i.StatutoryServiceGeneralDescription?.Versions) != null
                            ? i.ServiceOntologyTerms.Select(x => x.OntologyTerm)
                                .Union(
                                    VersioningManager.ApplyPublishingStatusFilterFallback(i.StatutoryServiceGeneralDescription?.Versions).OntologyTerms.Select(x => x.OntologyTerm))
                            : i.ServiceOntologyTerms.Select(x => x.OntologyTerm)).Cast<IFintoItemBase>()
                    , o => o.OntologyTerms)
                .AddSimple(i => i.Modified, o => o.Modified)
                .AddSimpleList(i =>
                        VersioningManager.ApplyPublishingStatusFilterFallback(i.StatutoryServiceGeneralDescription?.Versions) != null
                            ? i.ServiceServiceClasses.Select(s => s.ServiceClassId)
                                .Union(
                                    VersioningManager.ApplyPublishingStatusFilterFallback(i.StatutoryServiceGeneralDescription?.Versions)
                                        .ServiceClasses.Select(s => s.ServiceClassId))
                            : i.ServiceServiceClasses.Select(s => s.ServiceClassId)
                    , o => o.ServiceClasses)
                .AddCollection<ILanguageAvailability, VmLanguageAvailabilityInfo>(i => i.LanguageAvailabilities.OrderBy(x => x.Language.OrderNumber), o => o.LanguagesAvailabilities)
                .AddNavigation(i => i.Versioning, o => o.Version)
                .AddSimpleList(i => i.OrganizationServices.Select(x => x.OrganizationId).Union(new List<Guid> { i.OrganizationId }), o => o.Organizations)
                .AddSimpleList(i => i.ServiceProducers.Select(x => x.Id), o => o.Producers);

            if (entity.UnificRoot != null)
            {
                definition.AddSimple(i => i.UnificRoot.ServiceServiceChannels.Count(
                    x => x.ServiceChannel.Versions.Any(j => j.PublishingStatusId == typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString()) ||
                                                            j.PublishingStatusId == typesCache.Get<PublishingStatusType>(PublishingStatus.Draft.ToString()))),
                    o => o.ConnectedChannelsCount);
            }

            var model = definition.GetFinal();

            if (string.IsNullOrEmpty(model.Name))
            {
                definition.AddLocalizable(i => i.ServiceNames, o => o.Name);
            }

            return model;
        }

        public override ServiceVersioned TranslateVmToEntity(VmServiceListItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
