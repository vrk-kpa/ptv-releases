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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescription, ImportStatutoryServiceGeneralDescription>), RegisterType.Transient)]
    internal class ImportGeneralDescriptionTranslator : Translator<StatutoryServiceGeneralDescription, ImportStatutoryServiceGeneralDescription>
    {
        private ITypesCache typesCache;
        public ImportGeneralDescriptionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override ImportStatutoryServiceGeneralDescription TranslateEntityToVm(StatutoryServiceGeneralDescription entity)
        {
            throw new NotSupportedException();

        }

        public override StatutoryServiceGeneralDescription TranslateVmToEntity(ImportStatutoryServiceGeneralDescription vModel)
        {
            Console.Write("#");
            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => !string.IsNullOrEmpty(i.ReferenceCode), i => o => o.ReferenceCode == i.ReferenceCode,
                    def => def.UseDataContextCreate(i => true, o => o.Id, i => Guid.NewGuid()));
            var generalDescription = definition.GetFinal();

            definition.AddCollection(i => GetNames(i, generalDescription), o => o.Names)
                .AddCollection(i => GetDescriptions(i, generalDescription), o => o.Descriptions)
                .AddSimple(i => typesCache.Get<ServiceType>(i.ServiceType, ServiceTypeEnum.Service.ToString()), o => o.TypeId)
                .AddSimple(i => string.IsNullOrEmpty(i.ChargeType) ? (Guid?)null : typesCache.Get<ServiceChargeType>(i.ChargeType), o => o.ChargeTypeId)
                .AddLocalizable(i => new VmStringText { Id = generalDescription.Id, Text = i.ServiceRestrictions}, o => o.StatutoryServiceRequirements)
                .AddNavigation(i => i.ReferenceCode, o => o.ReferenceCode)
                .AddCollection(i => i.UniqueLaws, o => o.StatutoryServiceLaws)
                .AddCollection(i => i.TargetGroups, o => o.TargetGroups)
                .AddCollection(i => i.ServiceClasses.Distinct(new ImportFintoItemComparer()), o => o.ServiceClasses)
                .AddCollection(i => i.OntologyTerms, o => o.OntologyTerms)
                .AddCollection(i => i.LifeEvents, o => o.LifeEvents)
                .AddCollection(i => i.IndustrialClasses, o => o.IndustrialClasses)
                ;

            return generalDescription;
        }

        private IList<VmName> GetNames(ImportStatutoryServiceGeneralDescription model, StatutoryServiceGeneralDescription generalDescription)
        {
            return new List<VmName>()
            {
                new VmName {Name = model.Name, TypeId = typesCache.Get<NameType>(NameTypeEnum.Name.ToString()), OwnerReferenceId = generalDescription.Id}
            };
        }

        private IList<VmDescription> GetDescriptions(ImportStatutoryServiceGeneralDescription model, StatutoryServiceGeneralDescription generalDescription)
        {
            var descriptions = new List<VmDescription>()
            {
                new VmDescription { Description = model.ShortDescription, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString()), OwnerReferenceId = generalDescription.Id },
                new VmDescription { Description = model.Description, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()), OwnerReferenceId = generalDescription.Id},
                new VmDescription { Description = model.UserInstructions, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ServiceUserInstruction.ToString()), OwnerReferenceId = generalDescription.Id },
                new VmDescription { Description = model.ChargeTypeAdditionalInfo, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString()), OwnerReferenceId = generalDescription.Id },
                new VmDescription { Description = model.DeadLineAdditionalInfo, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.DeadLineAdditionalInfo.ToString()), OwnerReferenceId = generalDescription.Id },
                new VmDescription { Description = model.ProcessingTimeAdditionalInfo, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString()), OwnerReferenceId = generalDescription.Id },
                new VmDescription { Description = model.TasksAdditionalInfo, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.TasksAdditionalInfo.ToString()), OwnerReferenceId = generalDescription.Id },
                new VmDescription { Description = model.ValidityTimeAdditionalInfo, TypeId = typesCache.Get<DescriptionType>(DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString()), OwnerReferenceId = generalDescription.Id },
            };

            return descriptions;
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