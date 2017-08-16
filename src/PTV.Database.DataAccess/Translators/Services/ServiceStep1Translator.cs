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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Collections.Generic;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceVersioned, VmServiceStep1>), RegisterType.Transient)]
    internal class ServiceStep1Translator : Translator<ServiceVersioned, VmServiceStep1>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        public ServiceStep1Translator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmServiceStep1 TranslateEntityToVm(ServiceVersioned entity)
        {
            var step = CreateEntityViewModelDefinition<VmServiceStep1>(entity)
                .AddSimple(input => input.Id, output => output.Id)
                .AddSimple(input => input.UnificRootId, output => output.UnificRootId)
                .AddSimple(input => input.OrganizationId, output => output.OrganizationId)
                .AddLocalizable(input => GetName(input, NameTypeEnum.Name), output => output.ServiceName)
                .AddLocalizable(input => GetName(input, NameTypeEnum.AlternateName), output => output.AlternateServiceName)
                .AddLocalizable(input => GetDescription(input, DescriptionTypeEnum.ShortDescription), output => output.ShortDescriptions)
                .AddLocalizable(input => GetDescription(input, DescriptionTypeEnum.Description), output => output.Description)
                .AddLocalizable(input => GetDescription(input, DescriptionTypeEnum.ServiceUserInstruction), output => output.UserInstruction)
                .AddLocalizable(input => input.ServiceRequirements.ToList(), output => output.ServiceUsage)
                .AddLocalizable(input => GetDescription(input, DescriptionTypeEnum.ChargeTypeAdditionalInfo), output => output.AdditionalInformation)
                .AddLocalizable(input => GetDescription(input, DescriptionTypeEnum.DeadLineAdditionalInfo), output => output.AdditionalInformationDeadLine)
                .AddLocalizable(input => GetDescription(input, DescriptionTypeEnum.ProcessingTimeAdditionalInfo), output => output.AdditionalInformationProcessingTime)
                // .AddLocalizable(input => GetDescription(input, DescriptionTypeEnum.TasksAdditionalInfo), output => output.AdditionalInformationTasks)
                .AddLocalizable(input => GetDescription(input, DescriptionTypeEnum.ValidityTimeAdditionalInfo), output => output.AdditionalInformationValidityTime)
                .AddSimple(input => input.PublishingStatusId, output => output.PublishingStatusId)
                .AddNavigation(input => input.StatutoryServiceGeneralDescription, output => output.GeneralDescription)
                .AddSimple(input => input.TypeId, output => output.ServiceTypeId)
                .AddSimple(input => input.AreaInformationTypeId, output => output.AreaInformationTypeId)
                .AddSimple(input => input.ServiceChargeTypeId, output => output.ChargeType)
                .AddSimpleList(input => input.ServiceLanguages.OrderBy(x => x.Order).Select(x => x.LanguageId), output => output.Languages)
                .AddCollection(input => input.ServiceLaws.Select(x => x.Law).OrderBy(x => x.OrderNumber).ThenBy(x => x.Modified), output => output.Laws)
                .AddSimpleList(input => input.ServiceTargetGroups.Where(x => x.Override).Select(x => x.TargetGroupId), output => output.OverrideTargetGroups)
                .AddSimpleList(input => input.AreaMunicipalities.Select(x => x.MunicipalityId), output => output.AreaMunicipality)
                .AddSimpleList(input => input.Areas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.Province.ToString())).Select(x => x.AreaId), output => output.AreaProvince)
                .AddSimpleList(input => input.Areas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.BusinessRegions.ToString())).Select(x => x.AreaId), output => output.AreaBusinessRegions)
                .AddSimpleList(input => input.Areas.Where(x => x.Area.AreaTypeId == typesCache.Get<AreaType>(AreaTypeEnum.HospitalRegions.ToString())).Select(x => x.AreaId), output => output.AreaHospitalRegions)
                .AddSimpleList(input => input.OrganizationServices.Select(x => x.OrganizationId), output => output.Organizers)
                .AddCollection(input => input.ServiceProducers.OrderBy(x => x.OrderNumber).ThenBy(x => x.Modified), output => output.ServiceProducers)
                .AddSimple(input => input.FundingTypeId, output => output.FundingTypeId)
                .AddCollection(input => languageCache.FilterCollection(input.ServiceWebPages.Select(x => x.WebPage), RequestLanguageCode).OrderBy(x => x.OrderNumber).ThenBy(x => x.Created), output => output.ServiceVouchers)
                .AddCollection<ILanguageAvailability, VmLanguageAvailabilityInfo>(input => input.LanguageAvailabilities, o => o.LanguagesAvailabilities)
                ;

            return step.GetFinal();
        }

        private ICollection<ServiceName> GetName(ServiceVersioned serviceVersioned, NameTypeEnum type)
        {
            return serviceVersioned.ServiceNames.Where(x => x.TypeId == typesCache.Get<NameType>(type.ToString())).ToList();
        }

        private ICollection<ServiceDescription> GetDescription(ServiceVersioned serviceVersioned, DescriptionTypeEnum type)
        {
            return serviceVersioned.ServiceDescriptions.Where(x => x.TypeId == typesCache.Get<DescriptionType>(type.ToString())).ToList();
        }

        public override ServiceVersioned TranslateVmToEntity(VmServiceStep1 vModel)
        {
            throw new NotSupportedException();
        }

    }
}
