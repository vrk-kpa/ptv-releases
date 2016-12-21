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

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<Service, VmService>), RegisterType.Transient)]
    internal class ServiceTranslator : Translator<Service, VmService>
    {
        private ITypesCache typesCache;
        public ServiceTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmService TranslateEntityToVm(Service entity)
        {
            throw new NotImplementedException();
        }

        public override Service TranslateVmToEntity(VmService vModel)
        {
            var transaltionDefinition = CreateViewModelEntityDefinition<Service>(vModel)
                .UseDataContextCreate(input => !input.Id.HasValue, output => output.Id, input => Guid.NewGuid())
                .UseDataContextLocalizedUpdate(input => input.Id.HasValue, input => output => input.Id.Value == output.Id);

            if (vModel.PublishingStatus != null)
            {
                transaltionDefinition.AddSimple(i => i.PublishingStatus, o => o.PublishingStatusId);
            }

            if (vModel.Step1Form != null)
            {
                SetStep1Translation(transaltionDefinition, vModel);
            }
            if (vModel.Step2Form != null)
            {
                SetStep2Translation(transaltionDefinition);
            }
            if (vModel.Step3Form != null)
            {
                SetStep3Translation(transaltionDefinition, vModel);
            }
            if (vModel.Step4Form != null)
            {
                SetStep4Translation(transaltionDefinition);
            }
            var entity = transaltionDefinition.GetFinal();
            return entity;
        }

        private void SetStep1Translation(ITranslationDefinitions<VmService, Service> definition, VmService service)
        {
            var model = service.Step1Form;
            var names = new List<VmName>()
            {
                new VmName {Name = model.ServiceName, TypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString()), OwnerReferenceId = service.Id},
                new VmName {Name = model.AlternateServiceName, TypeId = typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString()), OwnerReferenceId = service.Id}
            };

            var descriptions = new List<VmDescription>()
            {
                new VmDescription { Description = model.ShortDescriptions, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString()), OwnerReferenceId = service.Id },
                new VmDescription { Description = model.Description, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()), OwnerReferenceId = service.Id},
                new VmDescription { Description = model.UserInstruction, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ServiceUserInstruction.ToString()), OwnerReferenceId = service.Id },
                new VmDescription { Description = model.AdditionalInformation, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString()), OwnerReferenceId = service.Id }
            };

            if (!typesCache.Compare<ServiceType>(model.ServiceTypeId, ServiceTypeEnum.Service.ToString()))
            {
                descriptions.Add(new VmDescription { Description = model.AdditionalInformationDeadLine, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.DeadLineAdditionalInfo.ToString()), OwnerReferenceId = service.Id });
                descriptions.Add(new VmDescription { Description = model.AdditionalInformationProcessingTime, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString()), OwnerReferenceId = service.Id });
                descriptions.Add(new VmDescription { Description = model.AdditionalInformationTasks, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.TasksAdditionalInfo.ToString()), OwnerReferenceId = service.Id });
                descriptions.Add(new VmDescription { Description = model.AdditionalInformationValidityTime, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString()), OwnerReferenceId = service.Id });
            }
            else
            {
                if (service.Id.IsAssigned())
                {
                    descriptions.Add(new VmDescription { TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.DeadLineAdditionalInfo.ToString()), OwnerReferenceId = service.Id });
                    descriptions.Add(new VmDescription { TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString()), OwnerReferenceId = service.Id });
                    descriptions.Add(new VmDescription { TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.TasksAdditionalInfo.ToString()), OwnerReferenceId = service.Id });
                    descriptions.Add(new VmDescription { TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString()), OwnerReferenceId = service.Id });
                }
            }

            if (typesCache.Compare<ServiceCoverageType>(service.Step1Form.ServiceCoverageTypeId, CoverageTypeEnum.Local.ToString()))
            {
                definition.AddCollection(i => i.Step1Form.Municipalities.Select(x => new VmListItem { Id = x }), o => o.ServiceMunicipalities);
            }

            definition
                .AddSimple(i => i.Step1Form.GeneralDescription?.Id, o => o.StatutoryServiceGeneralDescriptionId)
                .AddSimple(i => i.Step1Form.ServiceTypeId, o => o.TypeId)
                .AddSimple(i => i.Step1Form.ChargeType, o => o.ServiceChargeTypeId)
                .AddSimple(i => i.Step1Form.ServiceCoverageTypeId, o => o.ServiceCoverageTypeId)
                .AddCollection(i => names, o => o.ServiceNames)
                .AddCollection(i => descriptions, o => o.ServiceDescriptions)
                .AddCollection(
                        i => new List<VmServiceRequirement> { new VmServiceRequirement { Requirement = model.ServiceUsage, Id = service.Id } },
                        o => o.ServiceRequirements)
                .AddCollection(i => i.Step1Form.Languages.Select(x => new VmListItem { Id = x }), o => o.ServiceLanguages);

        }

        private void SetStep2Translation(ITranslationDefinitions<VmService, Service> definition)
        {
            definition
                .AddCollection(i => i.Step2Form.TargetGroups.Select(x => new VmListItem { Id = x}), o => o.ServiceTargetGroups)
                .AddCollection(i => i.Step2Form.ServiceClasses, o => o.ServiceServiceClasses)
                .AddCollection(i => i.Step2Form.OntologyTerms, o => o.ServiceOntologyTerms)
                .AddCollection(i => i.Step2Form.LifeEvents, o => o.ServiceLifeEvents)
                .AddCollection(i => i.Step2Form.IndustrialClasses, o => o.ServiceIndustrialClasses)
                .AddCollection(i =>  i.Step2Form.KeyWords.Select(x => new VmKeywordItem() { Id = x }).Concat(i.Step2Form.NewKeyWords), o => o.ServiceKeywords);
        }

        private void SetStep3Translation(ITranslationDefinitions<VmService, Service> definition, VmService model)
        {
            var entity = definition.AddCollection(i => i.Step3Form.ServiceProducers, o => o.OrganizationServices).GetFinal();
            var result = new List<OrganizationService>(entity.OrganizationServices);

            definition.AddCollection(i => i.Step3Form.OrganizersItems, o => o.OrganizationServices);
            result.AddRange(entity.OrganizationServices);
            entity.OrganizationServices = result;

        }


        private void SetStep4Translation(ITranslationDefinitions<VmService, Service> definition)
        {
            definition
                .AddCollection(i => i.Step4Form.Select( x => new VmChannelListItem() { Id = x }), o => o.ServiceServiceChannels);
        }
    }
}
