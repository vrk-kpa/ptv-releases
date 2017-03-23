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
using System.Collections;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceVersioned, VmServiceStep1>), RegisterType.Transient)]
    internal class ServiceStep1Translator : Translator<ServiceVersioned, VmServiceStep1>
    {
        private ITypesCache typesCache;
        public ServiceStep1Translator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmServiceStep1 TranslateEntityToVm(ServiceVersioned entity)
        {
            var step = CreateEntityViewModelDefinition<VmServiceStep1>(entity)
                .AddSimple(input => input.Id, output => output.Id)
                .AddSimple(input => input.UnificRootId, output => output.UnificRootId)
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
                // TODO sarzijan 18_10_2016_1: Remove the default value once the id is set as mandatory in database
                .AddSimple(input => input.ServiceCoverageTypeId ?? typesCache.Get<ServiceCoverageType>(ServiceCoverageTypeEnum.Nationwide.ToString()), output => output.ServiceCoverageTypeId)
                .AddSimple(input => input.ServiceChargeTypeId, output => output.ChargeType)
                .AddSimpleList(input => input.ServiceMunicipalities.Select(x => x.MunicipalityId), output => output.Municipalities)
                .AddSimpleList(input => input.ServiceLanguages.OrderBy(x => x.Order).Select(x => x.LanguageId), output => output.Languages)
                .AddCollection(input => input.ServiceLaws.Select(x => x.Law), output => output.Laws)
                .AddSimpleList(input => input.ServiceTargetGroups.Where(x => x.Override).Select(x => x.TargetGroupId), output => output.OverrideTargetGroups);

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
