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
            throw new NotSupportedException();
        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(ImportStatutoryServiceGeneralDescription vModel)
        {
            Console.Write("#");
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => !string.IsNullOrEmpty(i.ReferenceCode), i => o => o.ReferenceCode == i.ReferenceCode,
                    def =>
                    {
                        def.UseDataContextCreate(i => true, o => o.Id, i => Guid.NewGuid());
                        def.AddSimple(i => typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString()), o => o.PublishingStatusId);
                    });
                //.UseVersioning<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(o => o);
            var generalDescription = definition.GetFinal();

            definition.AddCollection(i => GetNames(i, generalDescription), o => o.Names)
                .AddCollection(i => GetDescriptions(i, generalDescription), o => o.Descriptions)
                .AddSimple(i => typesCache.Get<ServiceType>(i.ServiceType, ServiceTypeEnum.Service.ToString()), o => o.TypeId)
                .AddSimple(i => string.IsNullOrEmpty(i.ChargeType) ? (Guid?)null : typesCache.Get<ServiceChargeType>(i.ChargeType), o => o.ChargeTypeId)
                .AddCollection(i => i.ServiceRestrictions.Select(x => ownerReferenceLogic.GetWithOwnerReference(x, generalDescription.Id)), o => o.StatutoryServiceRequirements)
                .AddNavigation(i => i.ReferenceCode, o => o.ReferenceCode)
                .AddCollection(i => i.UniqueLaws, o => o.StatutoryServiceLaws)
                .AddCollection(i => i.TargetGroups, o => o.TargetGroups)
                .AddCollection(i => i.ServiceClasses.Distinct(new ImportFintoItemComparer()), o => o.ServiceClasses)
                .AddCollection(i => i.OntologyTerms, o => o.OntologyTerms)
                .AddCollection(i => i.LifeEvents, o => o.LifeEvents)
                .AddCollection(i => i.IndustrialClasses, o => o.IndustrialClasses);
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
            return
                descriptions.Select(
                    x =>
                        new VmDescription
                        {
                            Description = x.Label,
                            TypeId = typeId,
                            OwnerReferenceId = generalDescription.Id,
                            LocalizationId = languageCache.Get(x.Lang)
                        });
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