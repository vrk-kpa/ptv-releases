/**
 * The MIT License
 * Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators.Channels.V2;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Translators.GeneralDescription.V2
{
    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionBase>), RegisterType.Transient)]
    internal class GeneralDescriptionVersionedTranslator : Translator<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionBase>
    {
        private readonly ITypesCache typesCache;
        private readonly EntityDefinitionHelper definitionHelper;

        public GeneralDescriptionVersionedTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager,
            EntityDefinitionHelper entityHelper
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
            typesCache = cacheManager.TypesCache;
            definitionHelper = entityHelper;
        }

        public override VmGeneralDescriptionBase TranslateEntityToVm(StatutoryServiceGeneralDescriptionVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddSimple(i => i.TypeId, o => o.ServiceType)
                .AddSimple(i => i.UnificRootId, o => o.UnificRootId)
                .AddSimple(i => i.TypeId, o => o.ServiceType)
                .AddSimple(i => i.GeneralDescriptionTypeId, o => o.GeneralDescriptionType)
                .AddPartial(input => input, output => output as VmGeneralDescriptionHeader)
                .AddDictionary(i => GetDescription(i, DescriptionTypeEnum.Description), o => o.Description, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(i => GetDescription(i, DescriptionTypeEnum.ShortDescription), o => o.ShortDescription, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(i => GetDescription(i, DescriptionTypeEnum.BackgroundDescription), o => o.BackgroundDescription, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(i => GetDescription(i, DescriptionTypeEnum.DeadLineAdditionalInfo), o => o.DeadLineInformation, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(i => GetDescription(i, DescriptionTypeEnum.ProcessingTimeAdditionalInfo), o => o.ProcessingTimeInformation, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(i => GetDescription(i, DescriptionTypeEnum.ServiceUserInstruction), o => o.UserInstruction, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(i => GetDescription(i, DescriptionTypeEnum.ValidityTimeAdditionalInfo), o => o.ValidityTimeInformation, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(i => GetDescription(i, DescriptionTypeEnum.GeneralDescriptionTypeAdditionalInformation), o => o.GeneralDescriptionTypeAdditionalInformation, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(input => input.StatutoryServiceRequirements, output => output.ConditionOfServiceUsage, k => languageCache.GetByValue(k.LocalizationId))
                //.AddCollection(i => i.StatutoryServiceLaws.Select(j => j.Law), o => o.Laws)
                .AddSimpleList(i => i.TargetGroups.Select(j => j.TargetGroupId), o => o.TargetGroups)
                .AddNavigation(input => input as IChargeType, output => output.ChargeType);

            definitionHelper.AddOrderedCollection(
                definition,
                i => i.StatutoryServiceLaws.Select(x => x.Law),
                o => o.Laws
            );

            return definition.GetFinal();
        }

        private IEnumerable<IDescription> GetDescription(StatutoryServiceGeneralDescriptionVersioned generalDescriptionVersioned, DescriptionTypeEnum type)
        {
            return generalDescriptionVersioned.Descriptions.Where(x => typesCache.Compare<DescriptionType>(x.TypeId, type.ToString()));
        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(VmGeneralDescriptionBase vModel)
        {
            var names = new List<VmName>();
            names.AddNullRange(vModel.Name?.Select(pair => CreateName(pair.Key, pair.Value, vModel, NameTypeEnum.Name)));
            var serviceTypeService = typesCache.Get<ServiceType>(ServiceTypeEnum.Service.ToString());
            var descriptions = new List<VmDescription>();
            descriptions.AddNullRange(vModel.ShortDescription?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.ShortDescription)));
            descriptions.AddNullRange(vModel.Description?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.Description)));
            descriptions.AddNullRange(vModel.BackgroundDescription?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.BackgroundDescription)));
            descriptions.AddNullRange(vModel.DeadLineInformation?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.DeadLineAdditionalInfo, serviceTypeService == vModel.ServiceType)));
            descriptions.AddNullRange(vModel.ProcessingTimeInformation?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.ProcessingTimeAdditionalInfo, serviceTypeService == vModel.ServiceType)));
            descriptions.AddNullRange(vModel.GeneralDescriptionTypeAdditionalInformation?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.GeneralDescriptionTypeAdditionalInformation)));
            descriptions.AddNullRange(vModel.ValidityTimeInformation?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.ValidityTimeAdditionalInfo, serviceTypeService == vModel.ServiceType)));
            descriptions.AddNullRange(vModel.UserInstruction?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.ServiceUserInstruction)));
            descriptions.AddNullRange(vModel.ChargeType?.AdditionalInformation?.Select(pair => CreateDescription(pair.Key, pair.Value, vModel, DescriptionTypeEnum.ChargeTypeAdditionalInfo)));

            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .AddSimple(i => i?.ChargeType.ChargeType, o => o.ChargeTypeId)
                .AddSimple(i => i.ServiceType.HasValue ? i.ServiceType.Value : throw new PtvArgumentException("Service type has to be provided"), o => o.TypeId)
                .AddSimple(i => i.GeneralDescriptionType.HasValue ? i.GeneralDescriptionType.Value : throw new PtvArgumentException("General description type has to be provided"), o => o.GeneralDescriptionTypeId)
                .AddLanguageAvailability(i => i, o => o)
                .AddCollection(i => names, o => o.Names, true)
                .AddCollection(i => descriptions, o => o.Descriptions, true)
                .AddCollection(i => i.ConditionOfServiceUsage?.Select(
                    pair => new VmServiceRequirement { Id = i.Id, Requirement = pair.Value, LocalizationId = languageCache.Get(pair.Key) }), o => o.StatutoryServiceRequirements, true)
                //.AddCollectionWithRemove(i => (i.Laws ?? new List<VmLaw>()).Select(
                //    (l, order) =>
                //        {
                //            l.OwnerReferenceId = i.Id;
                //            l.OrderNumber = order;
                //            return l;
                //        }), output => output.StatutoryServiceLaws, c => true)
                .AddCollectionWithRemove(i => (i.TargetGroups?.Select(x => new VmTargetGroupListItem { Id = x, OwnerReferenceId = i.Id }) ?? new List<VmTargetGroupListItem>()), o => o.TargetGroups, x => true);

            definitionHelper
                .AddOrderedCollectionWithRemove(
                    definition,
                    i => i.Laws ?? new List<VmLaw>(),
                    o => o.StatutoryServiceLaws,
                    x => true,
                    (i, o) => o.OwnerReferenceId = i.Id
                );

            return definition.GetFinal();
        }

        private VmName CreateName(string language, string value, VmGeneralDescriptionBase vModel, NameTypeEnum typeEnum)
        {
            return new VmName
            {
                Name = value,
                TypeId = typesCache.Get<NameType>(typeEnum.ToString()),
                OwnerReferenceId = vModel.Id,
                LocalizationId = languageCache.Get(language)
            };
        }

        private VmDescription CreateDescription(string language, string value, VmGeneralDescriptionBase vModel, DescriptionTypeEnum typeEnum, bool isEmpty = false)
        {
            return new VmDescription
            {
                Description = isEmpty ? null : value,
                TypeId = typesCache.Get<DescriptionType>(typeEnum.ToString()),
                OwnerReferenceId = vModel.Id,
                LocalizationId = languageCache.Get(language)
            };
        }
    }

    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionOutput>), RegisterType.Transient)]
    internal class GeneralDescriptionVersionedOutputTranslator : Translator<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionOutput>
    {
        public GeneralDescriptionVersionedOutputTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmGeneralDescriptionOutput TranslateEntityToVm(StatutoryServiceGeneralDescriptionVersioned entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddPartial(i => i, o => o as VmGeneralDescriptionBase)
                .AddCollection(input => input.ServiceClasses.Select(x => x.ServiceClass as IFintoItemBase).OrderBy(x => x.Uri), output => output.ServiceClasses)
                .AddCollection(input => input.IndustrialClasses.Select(x => x.IndustrialClass as IFintoItemBase).OrderBy(x => x.Uri), output => output.IndustrialClasses)
                .AddCollection(input => input.OntologyTerms.Select(x => x.OntologyTerm as IFintoItemBase).OrderBy(x => x.Uri), output => output.OntologyTerms)
                .AddCollection(input => input.LifeEvents.Select(x => x.LifeEvent as IFintoItemBase).OrderBy(x => x.Uri), output => output.LifeEvents)
                .AddDictionaryList(input => input.Keywords.Select(x => x.Keyword), output => output.Keywords, k => languageCache.GetByValue(k.LocalizationId))
                .GetFinal();
        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(VmGeneralDescriptionOutput vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionInput>), RegisterType.Transient)]
    internal class ServiceSaveTranslator : Translator<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionInput>
    {
        private readonly IEntityTreesCache entityTreesCache;

        public ServiceSaveTranslator(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            entityTreesCache = cacheManager.EntityTreesCache;
        }

        public override VmGeneralDescriptionInput TranslateEntityToVm(StatutoryServiceGeneralDescriptionVersioned entity)
        {
            throw new NotImplementedException();
        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(VmGeneralDescriptionInput vModel)
        {
            var definition = CreateViewModelEntityDefinition(vModel)
                .DefineEntitySubTree(i => i.Include(j => j.StatutoryServiceLaws).ThenInclude(j => j.Law).ThenInclude(j => j.WebPages).ThenInclude(j => j.WebPage))
                .DefineEntitySubTree(i => i.Include(j => j.StatutoryServiceLaws).ThenInclude(j => j.Law).ThenInclude(j => j.Names))
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => o.Id == i.Id)
                .UseVersioning<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(o => o)
                .AddPartial(i => i as VmGeneralDescriptionBase)
                .AddCollectionWithRemove(i => i.ServiceClasses?.Select(x => new VmListItem { Id = x, OwnerReferenceId = i.Id }), o => o.ServiceClasses, x => true)
                .AddCollectionWithRemove(i => i.OntologyTerms?.Select(x => new VmListItem { Id = x, OwnerReferenceId = i.Id }), o => o.OntologyTerms, x => true)
                .AddCollectionWithRemove(i => IsAssignedTargetGroup(i, "KR1") ? i.LifeEvents?.Select(x => new VmListItem { Id = x, OwnerReferenceId = i.Id }) : new List<VmListItem>(), o => o.LifeEvents, x => true)
                .AddCollectionWithRemove(i => IsAssignedTargetGroup(i, "KR2") ? i.IndustrialClasses?.Select(x => new VmListItem { Id = x, OwnerReferenceId = i.Id }) : new List<VmListItem>(), o => o.IndustrialClasses, x => true)
                .AddCollectionWithRemove(i =>
                    {
                        var existingOnes = i.Keywords.SelectMany(pair =>
                        {
                            return pair.Value.Select(kw =>
                                new VmKeywordItem {Id = kw, OwnerReferenceId = i.Id});
                        });

                        var newKeywords = i.NewKeywords?.SelectMany(pair =>
                        {
                            var localizationId = languageCache.Get(pair.Key);
                            return pair.Value?.Select(
                                       x => new VmKeywordItem
                                           {Name = x, OwnerReferenceId = i.Id, LocalizationId = localizationId}) ??
                                   new List<VmKeywordItem>();
                        });

                        return existingOnes.Concat(newKeywords ?? new List<VmKeywordItem>());
                    }, o => o.Keywords,
                    x => true);

            return definition.GetFinal();
        }

        private bool IsAssignedTargetGroup(VmGeneralDescriptionInput model, string code)
        {
            Guid tgId = entityTreesCache.Get<TargetGroup>(code);
            return model.TargetGroups.Contains(tgId);
        }

    }
}
