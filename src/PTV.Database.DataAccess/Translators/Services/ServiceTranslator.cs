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
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceVersioned, VmService>), RegisterType.Transient)]
    internal class ServiceTranslator : Translator<ServiceVersioned, VmService>
    {
        private readonly ITypesCache typesCache;
        public ServiceTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmService TranslateEntityToVm(ServiceVersioned entity)
        {
            throw new NotImplementedException();
        }

        public override ServiceVersioned TranslateVmToEntity(VmService vModel)
        {
            var transaltionDefinition = CreateViewModelEntityDefinition<ServiceVersioned>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(input => !input.Id.HasValue, output => output.Id, input => Guid.NewGuid())
                .UseDataContextLocalizedUpdate(input => input.Id.HasValue, input => output => input.Id.Value == output.Id)
                .UseVersioning<ServiceVersioned, Service>(o => o)
                .AddLanguageAvailability(o => o);
            if (vModel.Step1Form != null)
            {
                SetStep1Translation(transaltionDefinition, vModel);
            }
            if (vModel.Step2Form != null)
            {
                SetStep2Translation(transaltionDefinition, vModel);
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

        private void SetStep1Translation(ITranslationDefinitions<VmService, ServiceVersioned> definition, VmService service)
        {
            var model = service.Step1Form;
            model.Laws?.ForEach(i => i.OwnerReferenceId = model.Id);

            var serviceTypeId = model.GeneralDescription?.Id == null ? model.ServiceTypeId : model.GeneralDescription.TypeId;

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

            if (!typesCache.Compare<ServiceType>(serviceTypeId, ServiceTypeEnum.Service.ToString()))
            {
                descriptions.Add(new VmDescription { Description = model.AdditionalInformationDeadLine, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.DeadLineAdditionalInfo.ToString()), OwnerReferenceId = service.Id });
                descriptions.Add(new VmDescription { Description = model.AdditionalInformationProcessingTime, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString()), OwnerReferenceId = service.Id });
            //    descriptions.Add(new VmDescription { Description = model.AdditionalInformationTasks, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.TasksAdditionalInfo.ToString()), OwnerReferenceId = service.Id });
                descriptions.Add(new VmDescription { Description = model.AdditionalInformationValidityTime, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString()), OwnerReferenceId = service.Id });
            }
            else
            {
                if (service.Id.IsAssigned())
                {
                    descriptions.Add(new VmDescription { TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.DeadLineAdditionalInfo.ToString()), OwnerReferenceId = service.Id });
                    descriptions.Add(new VmDescription { TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString()), OwnerReferenceId = service.Id });
                    //descriptions.Add(new VmDescription { TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.TasksAdditionalInfo.ToString()), OwnerReferenceId = service.Id });
                    descriptions.Add(new VmDescription { TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString()), OwnerReferenceId = service.Id });
                }
            }

            if (typesCache.Compare<ServiceCoverageType>(service.Step1Form.ServiceCoverageTypeId, CoverageTypeEnum.Local.ToString()))
            {
                definition.AddCollection(i => i.Step1Form.Municipalities.Select(x => new VmListItem { Id = x, OwnerReferenceId = service.Id }), o => o.ServiceMunicipalities);
            }

            var languageCount = 0;
            definition
                .AddSimple(i => i.Step1Form.GeneralDescription?.Id, o => o.StatutoryServiceGeneralDescriptionId)
                .AddSimple(i => i.Step1Form.GeneralDescription?.Id == null ? i.Step1Form.ServiceTypeId : null, o => o.TypeId)
                .AddSimple(i => i.Step1Form.ChargeType, o => o.ServiceChargeTypeId)
                .AddSimple(i => i.Step1Form.ServiceCoverageTypeId, o => o.ServiceCoverageTypeId)
                .AddCollection(i => names, o => o.ServiceNames)
                .AddCollection(i => descriptions, o => o.ServiceDescriptions)
                .AddCollection(
                        i => new List<VmServiceRequirement> { new VmServiceRequirement { Requirement = model.ServiceUsage, Id = service.Id } },
                        o => o.ServiceRequirements)
                .AddCollection(i => i.Step1Form.Languages.Select(x => new VmListItem { Id = x, OrderNumber = languageCount++, OwnerReferenceId = i.Id }), o => o.ServiceLanguages)
				.AddCollection(i => i.Step1Form.Laws, output => output.ServiceLaws, false);
        }

        private void SetStep2Translation(ITranslationDefinitions<VmService, ServiceVersioned> definition, VmService model)
        {
            model.Step2Form?.ServiceClasses?.ForEach(i => i.OwnerReferenceId = model.Id);
            model.Step2Form?.OntologyTerms?.ForEach(i => i.OwnerReferenceId = model.Id);
            model.Step2Form?.LifeEvents?.ForEach(i => i.OwnerReferenceId = model.Id);
            model.Step2Form?.IndustrialClasses?.ForEach(i => i.OwnerReferenceId = model.Id);
            definition
                .AddCollection(i => i.Step2Form.TargetGroups.Select(x => new VmTargetGroupListItem { Id = x, OwnerReferenceId = model.Id }).Concat(
                                    i.Step2Form.OverrideTargetGroups.Select(x => new VmTargetGroupListItem { Id = x, Override = true, OwnerReferenceId = model.Id })),
                                    o => o.ServiceTargetGroups)
                .AddCollection(i => i.Step2Form.ServiceClasses, o => o.ServiceServiceClasses)
                .AddCollection(i => i.Step2Form.OntologyTerms, o => o.ServiceOntologyTerms)
                .AddCollection(i => i.Step2Form.LifeEvents, o => o.ServiceLifeEvents)
                .AddCollection(i => i.Step2Form.IndustrialClasses, o => o.ServiceIndustrialClasses)
                .AddCollection(i =>  i.Step2Form.KeyWords.Select(x => new VmKeywordItem() { Id = x, OwnerReferenceId = model.Id }).Concat(i.Step2Form.NewKeyWords), o => o.ServiceKeywords);
        }

        private void SetStep3Translation(ITranslationDefinitions<VmService, ServiceVersioned> definition, VmService model)
        {
//            var entity = definition.AddCollection(i => i.Step3Form.ServiceProducers, o => o.OrganizationServices).GetFinal();
//            var result = new List<OrganizationService>(entity.OrganizationServices);

            definition.AddCollection(i => i.Step3Form.OrganizersItems, o => o.OrganizationServices);
            definition.AddCollection(i => i.Step3Form.ServiceProducers, o => o.OrganizationServices, true);
//            result.AddRange(entity.OrganizationServices);
//            entity.OrganizationServices = result;

        }


        private void SetStep4Translation(ITranslationDefinitions<VmService, ServiceVersioned> definition)
        {
//            definition
//                .AddCollection(i => i.Step4Form.Select( x => new VmChannelListItem() { Id = x }), o => o.ServiceServiceChannels);
        }
    }
}
