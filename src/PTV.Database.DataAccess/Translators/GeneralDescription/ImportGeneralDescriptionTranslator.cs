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
using PTV.Domain.Logic;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescriptionVersioned, ImportStatutoryServiceGeneralDescription>), RegisterType.Transient)]
    internal class ImportGeneralDescriptionTranslator : Translator<StatutoryServiceGeneralDescriptionVersioned, ImportStatutoryServiceGeneralDescription>
    {
        private ITypesCache typesCache;
        private VmOwnerReferenceLogic ownerReferenceLogic;
        private ILanguageCache languageCache;

        public ImportGeneralDescriptionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager, VmOwnerReferenceLogic ownerReferenceLogic) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
            this.ownerReferenceLogic = ownerReferenceLogic;
        }

        public override ImportStatutoryServiceGeneralDescription TranslateEntityToVm(StatutoryServiceGeneralDescriptionVersioned entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(input => typesCache.GetByValue<ServiceType>(input.TypeId), output => output.ServiceType)
                .AddNavigation(input => input.ChargeTypeId.HasValue ? typesCache.GetByValue<ServiceChargeType>(input.ChargeTypeId.Value) : null, output => output.ChargeType)
                .AddCollection(input => input.Names.Where(x => x.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())).Cast<IName>(), output => output.Name)
                .AddCollection(input => input.Descriptions.Where(x => x.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString())).Cast<IDescription>(), output => output.Description)
                .AddCollection(input => input.Descriptions.Where(x => x.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.BackgroundDescription.ToString())).Cast<IDescription>(), output => output.BackgroundDescription)
                .AddCollection(input => input.Descriptions.Where(x => x.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString())).Cast<IDescription>(), output => output.ShortDescription)
                .AddCollection(input => input.Descriptions.Where(x => x.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ServiceUserInstruction.ToString())).Cast<IDescription>(), output => output.UserInstructions)
                .AddCollection(input => input.Descriptions.Where(x => x.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString())).Cast<IDescription>(), output => output.ChargeTypeAdditionalInfo)
                .AddCollection(input => input.Descriptions.Where(x => x.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.DeadLineAdditionalInfo.ToString())).Cast<IDescription>(), output => output.DeadLineAdditionalInfo)
                .AddCollection(input => input.Descriptions.Where(x => x.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString())).Cast<IDescription>(), output => output.ProcessingTimeAdditionalInfo)
                .AddCollection(input => input.Descriptions.Where(x => x.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString())).Cast<IDescription>(), output => output.ValidityTimeAdditionalInfo)
                .AddCollection(input => input.StatutoryServiceRequirements, o => o.ServiceRestrictions)
                .AddCollection(i => i.StatutoryServiceLaws, o => o.Laws)
                .AddCollection(i => i.TargetGroups, o => o.TargetGroups)
                .AddCollection(i => i.ServiceClasses, o => o.ServiceClasses)
                .AddCollection(i => i.OntologyTerms, o => o.OntologyTerms)
                .AddCollection(i => i.LifeEvents, o => o.LifeEvents)
                .AddCollection(i => i.IndustrialClasses, o => o.IndustrialClasses)
                .AddNavigation(i => i.ReferenceCode, o => o.ReferenceCode)
                .GetFinal();
        }

        private ICollection<StatutoryServiceName> GetName(StatutoryServiceGeneralDescriptionVersioned gdVersioned, NameTypeEnum type)
        {
            return gdVersioned.Names.Where(x => x.TypeId == typesCache.Get<NameType>(type.ToString())).ToList();
        }

        private ICollection<StatutoryServiceDescription> GetDescription(StatutoryServiceGeneralDescriptionVersioned gdVersioned, DescriptionTypeEnum type)
        {
            return gdVersioned.Descriptions.Where(x => x.TypeId == typesCache.Get<DescriptionType>(type.ToString())).ToList();
        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(ImportStatutoryServiceGeneralDescription vModel)
        {
            Console.Write("#");
            bool isNew = false;
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => true, i => o => i.Id != null && (i.Id == o.Id),
                d => d.UseDataContextUpdate(i => true, i => o => (i.ReferenceCode != null && o.ReferenceCode == i.ReferenceCode),
                    def =>
                    {
                        isNew = true;
                        def.UseDataContextCreate(i => true, o => o.Id, i => i.Id.IsAssigned() ? i.Id : Guid.NewGuid());
                        def.AddSimple(i => typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString()), o => o.PublishingStatusId);
                    }));
                //.UseVersioning<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(o => o);
            var generalDescription = definition.GetFinal();

            definition
                .Propagation((i, o) =>
                {
                    i.UniqueLaws.ForEach(j => j.OwnerReferenceId = i.Id);
                    i.ServiceRestrictions.ForEach(j => j.OwnerReferenceId = i.Id);
                })
                .AddCollectionWithKeep(i => i.UniqueLaws, o => o.StatutoryServiceLaws, TranslationPolicy.FetchData, m => true);
            if (isNew)
            {
                definition
                    .AddCollectionWithKeep(i => GetNames(i, generalDescription), o => o.Names, TranslationPolicy.FetchData, m => true)
                    .AddCollectionWithKeep(i => GetDescriptions(i, generalDescription), o => o.Descriptions, TranslationPolicy.FetchData, m => true)
                    .AddSimple(i => typesCache.Get<ServiceType>(i.ServiceType, ServiceTypeEnum.Service.ToString()), o => o.TypeId)
                    .AddSimple(i => string.IsNullOrEmpty(i.ChargeType) ? (Guid?) null : typesCache.Get<ServiceChargeType>(i.ChargeType), o => o.ChargeTypeId)
                    .AddCollectionWithKeep(i => i.ServiceRestrictions.Select(x => ownerReferenceLogic.GetWithOwnerReference(x, generalDescription.Id)),
                        o => o.StatutoryServiceRequirements, TranslationPolicy.FetchData, m => true)
                    .AddNavigation(i => i.ReferenceCode, o => o.ReferenceCode)
                    .AddCollectionWithKeep(i => i.TargetGroups, o => o.TargetGroups, TranslationPolicy.FetchData, m => true)
                    .AddCollectionWithKeep(i => i.ServiceClasses.Distinct(new ImportFintoItemComparer()), o => o.ServiceClasses, TranslationPolicy.FetchData, m => true)
                    .AddCollectionWithKeep(i => i.OntologyTerms, o => o.OntologyTerms, TranslationPolicy.FetchData, m => true)
                    .AddCollectionWithKeep(i => i.LifeEvents, o => o.LifeEvents, TranslationPolicy.FetchData, m => true)
                    .AddCollectionWithKeep(i => i.IndustrialClasses, o => o.IndustrialClasses, TranslationPolicy.FetchData, m => true);
            }

            if (!generalDescription.UnificRootId.IsAssigned())
            {
                generalDescription.UnificRoot = new StatutoryServiceGeneralDescription() { Id = Guid.NewGuid() };
            }
            return generalDescription;
        }

        private IList<VmName> GetNames(ImportStatutoryServiceGeneralDescription model, StatutoryServiceGeneralDescriptionVersioned generalDescriptionVersioned)
        {
            var nameType = typesCache.Get<NameType>(NameTypeEnum.Name.ToString());
            return model.Name.Select(x => new VmName
            {
                Name = x.Label,
                TypeId = nameType,
                OwnerReferenceId = generalDescriptionVersioned.Id,
                LocalizationId = languageCache.Get(x.Lang)
            }).ToList();
        }

        private IList<VmDescription> GetDescriptions(ImportStatutoryServiceGeneralDescription model, StatutoryServiceGeneralDescriptionVersioned generalDescriptionVersioned)
        {
            var descriptions = new List<VmDescription>();
            descriptions.AddRange(GetDescription(model.ShortDescription, DescriptionTypeEnum.ShortDescription, generalDescriptionVersioned));
            descriptions.AddRange(GetDescription(model.Description, DescriptionTypeEnum.Description, generalDescriptionVersioned));
            descriptions.AddRange(GetDescription(model.BackgroundDescription, DescriptionTypeEnum.BackgroundDescription, generalDescriptionVersioned));
            descriptions.AddRange(GetDescription(model.UserInstructions, DescriptionTypeEnum.ServiceUserInstruction, generalDescriptionVersioned));
            descriptions.AddRange(GetDescription(model.ChargeTypeAdditionalInfo, DescriptionTypeEnum.ChargeTypeAdditionalInfo, generalDescriptionVersioned));
            descriptions.AddRange(GetDescription(model.DeadLineAdditionalInfo, DescriptionTypeEnum.DeadLineAdditionalInfo, generalDescriptionVersioned));
            descriptions.AddRange(GetDescription(model.ProcessingTimeAdditionalInfo, DescriptionTypeEnum.ProcessingTimeAdditionalInfo, generalDescriptionVersioned));
//            descriptions.AddRange(GetDescription(model.TasksAdditionalInfo, DescriptionTypeEnum.TasksAdditionalInfo, generalDescription));
            descriptions.AddRange(GetDescription(model.ValidityTimeAdditionalInfo, DescriptionTypeEnum.ValidityTimeAdditionalInfo, generalDescriptionVersioned));

            return descriptions;
        }

        private IEnumerable<VmDescription> GetDescription(IEnumerable<JsonLanguageLabel> descriptions, DescriptionTypeEnum type, StatutoryServiceGeneralDescriptionVersioned generalDescription)
        {
            var typeId = typesCache.Get<DescriptionType>(type.ToString());
            return descriptions?.Select(
                    x =>
                        new VmDescription
                        {
                            Description = x.Label,
                            TypeId = typeId,
                            OwnerReferenceId = generalDescription.Id,
                            LocalizationId = languageCache.Get(x.Lang)
                        })
                ?? new List<VmDescription>();
        }
    }

    internal class ImportFintoItemComparer : IEqualityComparer<ImportFintoItem>
    {
        public bool Equals(ImportFintoItem x, ImportFintoItem y)
        {
            return x.Url == y.Url || x.Name == y.Name;
        }

        public int GetHashCode(ImportFintoItem obj)
        {
            return obj.Url.GetHashCode();
        }
    }
}