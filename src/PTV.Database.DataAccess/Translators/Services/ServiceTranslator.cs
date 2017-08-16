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
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.Interfaces;

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
            var translationDefinition = CreateViewModelEntityDefinition<ServiceVersioned>(vModel)
                .DisableAutoTranslation()
                .DefineEntitySubTree(i => i.Include(j => j.ServiceLaws).ThenInclude(j => j.Law).ThenInclude(j => j.WebPages).ThenInclude(j => j.WebPage))
                .DefineEntitySubTree(i => i.Include(j => j.ServiceLaws).ThenInclude(j => j.Law).ThenInclude(j => j.Names))
                .DefineEntitySubTree(i => i.Include(j => j.ServiceWebPages).ThenInclude(j => j.WebPage))
                .UseDataContextCreate(input => !input.Id.HasValue, output => output.Id, input => Guid.NewGuid())
                .UseDataContextLocalizedUpdate(input => input.Id.HasValue, input => output => input.Id.Value == output.Id)
                .UseVersioning<ServiceVersioned, Service>(o => o)
                .AddLanguageAvailability(o => o);
            if (vModel.Step1Form != null)
            {
                SetStep1Translation(translationDefinition, vModel);
            }
            if (vModel.Step2Form != null)
            {
                SetStep2Translation(translationDefinition, vModel);
            }
            if (vModel.Step3Form != null)
            {
                SetStep3Translation(translationDefinition);
            }
            if (vModel.Step4Form != null)
            {
                SetStep4Translation(translationDefinition);
            }

            var entity = translationDefinition.GetFinal();
            return entity;
        }

        private void SetStep1Translation(ITranslationDefinitions<VmService, ServiceVersioned> definition, VmService service)
        {
            var model = service.Step1Form;
            var order = 1;
            model.Laws?.ForEach(i =>
            {
                i.OrderNumber = order++;
                i.OwnerReferenceId = model.Id;
            });

            var serviceTypeId = model.GeneralDescription?.UnificRootId == null ? model.ServiceTypeId : model.GeneralDescription.TypeId;

            var names = new List<VmName>()
            {
                new VmName {Name = model.ServiceName, TypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString()), OwnerReferenceId = service.Id}
            };

            if (!string.IsNullOrEmpty(model.AlternateServiceName))
            {
                names.Add(new VmName { Name = model.AlternateServiceName, TypeId = typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString()), OwnerReferenceId = service.Id });
            }

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
            
            if (typesCache.Compare<AreaInformationType>(service.Step1Form.AreaInformationTypeId, AreaInformationTypeEnum.AreaType.ToString()))
            {
                var areas = service.Step1Form.AreaBusinessRegions.Union(service.Step1Form.AreaHospitalRegions).Union(service.Step1Form.AreaProvince);
                definition.AddCollectionWithRemove(i => areas.Select(x => new VmListItem { Id = x, OwnerReferenceId = service.Id }), o => o.Areas, r => true);
                definition.AddCollectionWithRemove(i => i.Step1Form.AreaMunicipality.Select(x => new VmListItem { Id = x, OwnerReferenceId = service.Id }), o => o.AreaMunicipalities, r => true);
            }
            else
            {   //Remove Areas 
                definition.AddCollectionWithRemove(i => new List<VmListItem>() {}, o => o.Areas, r => true);
                definition.AddCollectionWithRemove(i => new List<VmListItem>() {}, o => o.AreaMunicipalities, r => true);
            }

            var defaultAreaInformationTypeId = typesCache.Get<AreaInformationType>(AreaInformationTypeEnum.WholeCountry.ToString());
            var generalDescriptionUnificRootIdIsAssigned = model.GeneralDescription != null ? model.GeneralDescription.UnificRootId.IsAssigned() : false;
            var languageCount = 0;
            definition
                .AddSimple(i => generalDescriptionUnificRootIdIsAssigned ? model.GeneralDescription.UnificRootId : (Guid?) null, o => o.StatutoryServiceGeneralDescriptionId)
                .AddSimple(i => generalDescriptionUnificRootIdIsAssigned ? null : i.Step1Form.ServiceTypeId, o => o.TypeId)
                .AddSimple(i => i.Step1Form.ChargeType, o => o.ServiceChargeTypeId)
                .AddSimple(i => i.Step1Form.AreaInformationTypeId.IsAssigned() ? i.Step1Form.AreaInformationTypeId : defaultAreaInformationTypeId, output => output.AreaInformationTypeId)
                .AddCollectionWithKeep(i => names, o => o.ServiceNames, TranslationPolicy.FetchData, x => x.LocalizationId != RequestLanguageId)
                .AddCollection(i => descriptions, o => o.ServiceDescriptions)
                .AddCollection(
                    i => new List<VmServiceRequirement> {new VmServiceRequirement {Requirement = model.ServiceUsage, Id = service.Id}},
                    o => o.ServiceRequirements)
                .AddCollectionWithRemove(i => i.Step1Form.Languages.Select(x => new VmListItem {Id = x, OrderNumber = languageCount++, OwnerReferenceId = i.Id}), o => o.ServiceLanguages, r => true)
                //.AddCollection(i => i.Step1Form.Laws.Where(x => !string.IsNullOrEmpty(x.UrlAddress.TryGet(RequestLanguageCode.ToString()))), output => output.ServiceLaws, false)
                .AddCollectionWithKeep(i => i.Step1Form.Laws/*.Where(x => !string.IsNullOrEmpty(x.UrlAddress.TryGet(RequestLanguageCode.ToString())))*/, output => output.ServiceLaws, c =>
                {
                    c.Check(h => h.Law).Check(h => h.Names).Any();
                    c.Check(h => h.Law).Check(h => h.WebPages).Any();
                    c.Check(h => h.Law).Check(h => h.WebPages).Check(h => h.WebPage).Check(h => h.LocalizationId).Not(RequestLanguageId);
                })
                .AddCollectionWithRemove(i => i.Step1Form.Organizers.Select(x => new VmTreeItem {Id = x, OwnerReferenceId = i.Id}), o => o.OrganizationServices, r => true)
                .AddSimple(i => i.Step1Form.FundingTypeId, o => o.FundingTypeId)
                .AddSimple(i => i.Step1Form.OrganizationId, o => o.OrganizationId)
                .AddCollectionWithKeep(i => i.Step1Form.ServiceVouchers, o => o.ServiceWebPages, c => c.Check(h => h.WebPage).Check(h => h.LocalizationId).Not(RequestLanguageId));
              
            // handle selfProduced serviceProducers 
            if (service.Id.IsAssigned())
            {
                var selfProducedId = typesCache.Get<ProvisionType>(ProvisionTypeEnum.SelfProduced.ToString());
                var selfProducedOrganizations = service.Step1Form.ServiceProducers.Where(p => p.ProvisionTypeId == selfProducedId && p.Organizers != null).Select(x =>
                {
                    var organizers = service.Step1Form.Organizers;
                    if (service.Step1Form.OrganizationId.IsAssigned())
                    {
                        organizers.Add(service.Step1Form.OrganizationId);
                    }

                    x.Organizers = x.Organizers.Where(org => organizers.Contains(org)).ToList();
                    return x;
                }).ToList();
                definition.AddCollectionWithKeep(i => selfProducedOrganizations, o => o.ServiceProducers, r => r.ProvisionTypeId != selfProducedId);
            }

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

        private void SetStep3Translation(ITranslationDefinitions<VmService, ServiceVersioned> definition)
        {
            definition.AddCollectionWithRemove(i =>
            {
                var number = 1;
                i.Step3Form.ServiceProducers.ForEach(p => p.Order = number++);
                return i.Step3Form.ServiceProducers;
            }, o => o.ServiceProducers, r => true);
        }

        private void SetStep4Translation(ITranslationDefinitions<VmService, ServiceVersioned> definition)
        {
//            definition
//                .AddCollection(i => i.Step4Form.Select( x => new VmChannelListItem() { Id = x }), o => o.ServiceServiceChannels);
        }
    }
}
