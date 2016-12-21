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
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<Service, VmServiceListItem>), RegisterType.Transient)]
    internal class ServiceListItemTranslator : Translator<Service, VmServiceListItem>
    {
        private ITypesCache typesCache;
        public ServiceListItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmServiceListItem TranslateEntityToVm(Service entity)
        {
            return CreateEntityViewModelDefinition<VmServiceListItem>(entity)
                .AddLocalizable(i => i.ServiceNames.Where(k => k.Type.Code == NameTypeEnum.Name.ToString()), o => o.Name)
                .AddSimple(i => i.PublishingStatusId ?? Guid.Empty, o => o.PublishingStatus)
                .AddSimple(i => i.TypeId, o => o.ServiceTypeId)
                .AddNavigation(i => i.Type.Code, o => o.ServiceType)
                .AddNavigation(i =>
                {
                    var orgsNamesAll = i.OrganizationServices.Where(org => org.Organization != null).Select(k => k.Organization.OrganizationNames);
                    var orgsNames = orgsNamesAll.Where(j => j.Any(k => k.Type.Code == NameTypeEnum.Name.ToString()));
                    var names = orgsNames.Select(j => j.First(k => k.Type.Code == NameTypeEnum.Name.ToString()).Name).ToList();
                    var result = names.Any() ? names.Aggregate((o1, o2) => o1 + ", " + o2) : string.Empty;
                    return result;
                }
                , o => o.OrganizationName)
                .AddNavigation(i =>
                {
                    var names = i.StatutoryServiceGeneralDescription != null
                        ? i.ServiceOntologyTerms.Select(x => x.OntologyTerm.Label)
                            .Union(i.StatutoryServiceGeneralDescription.OntologyTerms.Select(x => x.OntologyTerm.Label))
                        : i.ServiceOntologyTerms.Select(x => x.OntologyTerm.Label);
                    return names.Any() ? names.Aggregate((o1, o2) => o1 + ", " + o2) : string.Empty;
                }
                , o => o.OntologyTerms)
                .AddSimple(i => i.ServiceServiceChannels.Count(x => x.ServiceChannel.PublishingStatusId != typesCache.Get<PublishingStatusType>(PublishingStatus.Deleted.ToString())), o => o.ConnectedChannelsCount)
                .AddSimple(i => i.Modified, o => o.Modified)
                .AddNavigation(i =>
                    {
                        var serviceClasses = i.StatutoryServiceGeneralDescription != null
                            ? i.ServiceServiceClasses.Select(s => s.ServiceClass.Label)
                                .Union(i.StatutoryServiceGeneralDescription.ServiceClasses.Select(s => s.ServiceClass.Label))
                            : i.ServiceServiceClasses.Select(s => s.ServiceClass.Label);
                        return serviceClasses.Any() ? serviceClasses.Aggregate((o1, o2) => o1 + ", " + o2) : string.Empty;
                    }, o => o.ServiceClassName).GetFinal();
        }

        public override Service TranslateVmToEntity(VmServiceListItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
