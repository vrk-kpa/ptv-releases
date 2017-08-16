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
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescription>), RegisterType.Transient)]
    internal class GeneralDescriptionVersionedTranslator : Translator<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescription>
    {
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        public GeneralDescriptionVersionedTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmGeneralDescription TranslateEntityToVm(StatutoryServiceGeneralDescriptionVersioned entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddSimple(i => i.UnificRootId, o => o.UnificRootId)
                .AddSimple(i => i.PublishingStatusId, o => o.PublishingStatusId)
                .AddDictionary(i => i.Names.Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())), o => o.Name, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString())), o => o.Description, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.BackgroundDescription.ToString())), o => o.BackgroundDescription, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString())), o => o.ShortDescription, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString())), o => o.AdditionalInformation, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.DeadLineAdditionalInfo.ToString())), o => o.AdditionalInformationDeadLine, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString())), o => o.AdditionalInformationProcessingTime, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString())), o => o.AdditionalInformationValidityTime, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ServiceUserInstruction.ToString())), o => o.UserInstruction, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddSimpleList(i => i.TargetGroups.Select(j => j.TargetGroupId), o => o.TargetGroups)
                .AddCollection(i => i.ServiceClasses.Select(x => x.ServiceClass as IFintoItemBase), output => output.ServiceClasses)
                .AddCollection(i => i.StatutoryServiceLaws.Select(j => j.Law).OrderBy(k => k.OrderNumber).ThenBy(k => k.Modified), o => o.Laws)
                .AddCollection(i => i.IndustrialClasses.Select(x => x.IndustrialClass as IFintoItemBase), o => o.IndustrialClasses)
                .AddCollection(i => i.LifeEvents.Select(x => x.LifeEvent as IFintoItemBase), o => o.LifeEvents)
                .AddCollection(i => i.OntologyTerms.Select(x => x.OntologyTerm as IFintoItemBase), o => o.OntologyTerms)
                .AddDictionary(i => i.StatutoryServiceRequirements.Cast<IText>(), o => o.ServiceUsage, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddSimple(i => i.TypeId, o => o.TypeId)
                .AddNavigation(i => i.Type.Code, o => o.Type)
                .AddSimple(i => i.ChargeTypeId, o => o.ChargeTypeId)
                .AddNavigation(i => i.ChargeType?.Code, o => o.ChargeType)
                .GetFinal();
        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(VmGeneralDescription vModel)
        {
            var translationDefinition = CreateViewModelEntityDefinition<StatutoryServiceGeneralDescriptionVersioned>(vModel)
                    .DisableAutoTranslation()
                    .UseDataContextCreate(input => !input.Id.HasValue, output => output.Id, input => Guid.NewGuid())
                    .UseDataContextLocalizedUpdate(input => input.Id.HasValue, input => output => input.Id.Value == output.Id)
                    .UseVersioning<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(o => o)
                    .AddLanguageAvailability(o => o);

            var order = 1;
            vModel.Laws?.ForEach(i =>
            {
                i.OrderNumber = order++;
                i.OwnerReferenceId = vModel.Id;
            });
            vModel.ServiceClasses?.ForEach(i => i.OwnerReferenceId = vModel.Id);
            vModel.OntologyTerms?.ForEach(i => i.OwnerReferenceId = vModel.Id);
            vModel.LifeEvents?.ForEach(i => i.OwnerReferenceId = vModel.Id);
            vModel.IndustrialClasses?.ForEach(i => i.OwnerReferenceId = vModel.Id);
            
            var names = new List<VmName>()
            {
                new VmName {Name = vModel.Name.TryGet(RequestLanguageCode.ToString()), TypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString()), OwnerReferenceId = vModel.Id}
            };

            var descriptions = new List<VmDescription>()
            {
                new VmDescription { Description = vModel.ShortDescription.TryGet(RequestLanguageCode.ToString()), TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString()), OwnerReferenceId = vModel.Id },
                new VmDescription { Description = vModel.Description.TryGet(RequestLanguageCode.ToString()), TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()), OwnerReferenceId = vModel.Id},
                new VmDescription { Description = vModel.BackgroundDescription.TryGet(RequestLanguageCode.ToString()), TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.BackgroundDescription.ToString()), OwnerReferenceId = vModel.Id},
                new VmDescription { Description = vModel.UserInstruction.TryGet(RequestLanguageCode.ToString()), TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ServiceUserInstruction.ToString()), OwnerReferenceId = vModel.Id },
                new VmDescription { Description = vModel.AdditionalInformation.TryGet(RequestLanguageCode.ToString()), TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString()), OwnerReferenceId = vModel.Id }

            };

            if (!typesCache.Compare<ServiceType>(vModel.TypeId, ServiceTypeEnum.Service.ToString()))
            {
                descriptions.Add(new VmDescription { Description = vModel.AdditionalInformationDeadLine.TryGet(RequestLanguageCode.ToString()), TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.DeadLineAdditionalInfo.ToString()), OwnerReferenceId = vModel.Id });
                descriptions.Add(new VmDescription { Description = vModel.AdditionalInformationProcessingTime.TryGet(RequestLanguageCode.ToString()), TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString()), OwnerReferenceId = vModel.Id });
                descriptions.Add(new VmDescription { Description = vModel.AdditionalInformationValidityTime.TryGet(RequestLanguageCode.ToString()), TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString()), OwnerReferenceId = vModel.Id });
            }
            else
            {
                if (vModel.Id.IsAssigned())
                {
                    descriptions.Add(new VmDescription { TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.DeadLineAdditionalInfo.ToString()), OwnerReferenceId = vModel.Id });
                    descriptions.Add(new VmDescription { TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString()), OwnerReferenceId = vModel.Id });
                    descriptions.Add(new VmDescription { TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString()), OwnerReferenceId = vModel.Id });
                }
            }
            
            translationDefinition
                    .AddSimple(i => i.TypeId, o => o.TypeId)
                    .AddSimple(i => i.ChargeTypeId, o => o.ChargeTypeId)
                    .AddCollection(i => names, o => o.Names)
                    .AddCollection(i => descriptions, o => o.Descriptions)
                    .AddCollection(i => new List<VmServiceRequirement> { new VmServiceRequirement { Requirement = vModel.ServiceUsage.TryGet(RequestLanguageCode.ToString()), Id = vModel.Id } }, o => o.StatutoryServiceRequirements)
                    .AddCollection(i => i.TargetGroups.Select(x => new VmTargetGroupListItem { Id = x, OwnerReferenceId = vModel.Id }), o => o.TargetGroups)
                    .AddCollection(i => i.ServiceClasses, o => o.ServiceClasses)
                    .AddCollectionWithKeep(i => i.Laws.Where(x => !string.IsNullOrEmpty(x.UrlAddress.TryGet(RequestLanguageCode.ToString()))), o => o.StatutoryServiceLaws, x => true)
                    .AddCollection(i => i.IndustrialClasses, o => o.IndustrialClasses)
                    .AddCollection(i => i.LifeEvents, o => o.LifeEvents)
                    .AddCollection(i => i.OntologyTerms, o => o.OntologyTerms);    

            var entity = translationDefinition.GetFinal();
            return entity;
        }
    }

    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescription, VmGeneralDescription>), RegisterType.Transient)]
    internal class GeneralDescriptionTranslator : Translator<StatutoryServiceGeneralDescription, VmGeneralDescription>
    {
        private ITypesCache typesCache;
        public GeneralDescriptionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmGeneralDescription TranslateEntityToVm(StatutoryServiceGeneralDescription entity)
        {
            if (entity == null) return null;
            SetLanguage(LanguageCode.fi);
            return CreateEntityViewModelDefinition(entity)
                .AddPartial(i => VersioningManager.ApplyPublishingStatusFilterFallback(i.Versions))
                .AddSimple(i => i.Id, o => o.Id)
                .GetFinal();

        }

        public override StatutoryServiceGeneralDescription TranslateVmToEntity(VmGeneralDescription vModel)
        {
            throw new NotImplementedException();
        }
    }
}
