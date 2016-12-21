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

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
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
            SetLanguage(LanguageCode.fi);
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddLocalizable(i => i.Names.Where(j => typesCache.Compare<NameType>(j.TypeId, NameTypeEnum.Name.ToString())), o => o.Name)
                .AddLocalizable(i => i.Names.Where(j => typesCache.Compare<NameType>(j.TypeId, NameTypeEnum.AlternateName.ToString())), o => o.AlternateName)
                .AddLocalizable(i => i.Descriptions.Where(j => typesCache.Compare<DescriptionType>(j.TypeId, DescriptionTypeEnum.Description.ToString())), o => o.Description)
                .AddLocalizable(i => i.Descriptions.Where(j => typesCache.Compare<DescriptionType>(j.TypeId, DescriptionTypeEnum.ShortDescription.ToString())), o => o.ShortDescription)
                .AddLocalizable(i => i.Descriptions.Where(j => typesCache.Compare<DescriptionType>(j.TypeId, DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString())), o => o.ChargeTypeAdditionalInfo)
                .AddLocalizable(i => i.Descriptions.Where(j => typesCache.Compare<DescriptionType>(j.TypeId, DescriptionTypeEnum.DeadLineAdditionalInfo.ToString())), o => o.DeadLineAdditionalInfo)
                .AddLocalizable(i => i.Descriptions.Where(j => typesCache.Compare<DescriptionType>(j.TypeId, DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString())), o => o.ProcessingTimeAdditionalInfo)
                .AddLocalizable(i => i.Descriptions.Where(j => typesCache.Compare<DescriptionType>(j.TypeId, DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString())), o => o.ValidityTimeAdditionalInfo)
                .AddSimpleList(i => i.TargetGroups.Select(j => j.TargetGroupId), o => o.TargetGroups)
                .AddSimpleList(i => i.ServiceClasses.Select(j => j.ServiceClassId), o => o.ServiceClasses)
                .AddCollection(i => i.IndustrialClasses.Select(x => x.IndustrialClass as IFintoItem), o => o.IndustrialClasses)
                .AddCollection(i => i.LifeEvents.Select(x => x.LifeEvent as IFintoItem), o => o.LifeEvents)
                .AddCollection(i => i.OntologyTerms.Select(x => x.OntologyTerm), o => o.OntologyTerms)
                .AddLocalizable(i => i.StatutoryServiceRequirements.Cast<IText>(), o => o.ConditionOfServiceUsage)
                .AddSimple(i => i.TypeId, o => o.TypeId)
                .AddNavigation(i => i.Type.Code, o => o.Type)
                .AddSimple(i => i.ChargeTypeId, o => o.ChargeTypeId)
                .AddNavigation(i => i.ChargeType?.Code, o => o.ChargeType)
                .GetFinal();

        }

        public override StatutoryServiceGeneralDescription TranslateVmToEntity(VmGeneralDescription vModel)
        {
            throw new NotImplementedException();
        }
    }
}
