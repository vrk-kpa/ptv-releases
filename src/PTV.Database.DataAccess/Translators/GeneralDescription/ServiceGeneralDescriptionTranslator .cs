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
    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescriptionVersioned, VmServiceGeneralDescription>), RegisterType.Transient)]
    internal class ServiceGeneralDescriptionVersionedTranslator : Translator<StatutoryServiceGeneralDescriptionVersioned, VmServiceGeneralDescription>
    {
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        public ServiceGeneralDescriptionVersionedTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmServiceGeneralDescription TranslateEntityToVm(StatutoryServiceGeneralDescriptionVersioned entity)
        {
            var publishedLanguageIds = entity.LanguageAvailabilities.Where(x => x.StatusId == typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString())).Select(x => x.LanguageId).ToList();

            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddSimple(i => i.PublishingStatusId, o => o.PublishingStatusId)
                .AddDictionary(i => i.Names.Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()) && publishedLanguageIds.Contains(j.LocalizationId)), o => o.Name, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()) && publishedLanguageIds.Contains(j.LocalizationId)), o => o.Description, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.BackgroundDescription.ToString()) && publishedLanguageIds.Contains(j.LocalizationId)), o => o.BackgroundDescription, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString()) && publishedLanguageIds.Contains(j.LocalizationId)), o => o.ShortDescription, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString()) && publishedLanguageIds.Contains(j.LocalizationId)), o => o.AdditionalInformation, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.DeadLineAdditionalInfo.ToString()) && publishedLanguageIds.Contains(j.LocalizationId)), o => o.AdditionalInformationDeadLine, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString()) && publishedLanguageIds.Contains(j.LocalizationId)), o => o.AdditionalInformationProcessingTime, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString()) && publishedLanguageIds.Contains(j.LocalizationId)), o => o.AdditionalInformationValidityTime, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ServiceUserInstruction.ToString()) && publishedLanguageIds.Contains(j.LocalizationId)), o => o.UserInstruction, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddSimpleList(i => i.TargetGroups.Select(j => j.TargetGroupId), o => o.TargetGroups)
                .AddCollection(i => i.ServiceClasses.Select(j => j.ServiceClass as IFintoItemBase), output => output.ServiceClasses)
                .AddCollection(i => i.StatutoryServiceLaws.Select(j => j.Law).OrderBy(k => k.OrderNumber).ThenBy(k => k.Modified), o => o.Laws)
                .AddCollection(i => i.IndustrialClasses.Select(x => x.IndustrialClass as IFintoItemBase), o => o.IndustrialClasses)
                .AddCollection(i => i.LifeEvents.Select(x => x.LifeEvent as IFintoItemBase), o => o.LifeEvents)
                .AddCollection(i => i.OntologyTerms.Select(x => x.OntologyTerm as IFintoItemBase), o => o.OntologyTerms)
                .AddDictionary(i => i.StatutoryServiceRequirements.Where(x => publishedLanguageIds.Contains(x.LocalizationId)).Cast<IText>(), o => o.ServiceUsage, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddSimple(i => i.TypeId, o => o.TypeId)
                .AddNavigation(i => i.Type.Code, o => o.Type)
                .AddSimple(i => i.ChargeTypeId, o => o.ChargeTypeId)
                .AddNavigation(i => i.ChargeType?.Code, o => o.ChargeType)
                .GetFinal();

        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(VmServiceGeneralDescription vModel)
        {
            throw new NotImplementedException();
        }
    }


    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescription, VmServiceGeneralDescription>), RegisterType.Transient)]
    internal class ServiceGeneralDescriptionTranslator : Translator<StatutoryServiceGeneralDescription, VmServiceGeneralDescription>
    {
        private ITypesCache typesCache;
        public ServiceGeneralDescriptionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ITypesCache typesCache) : base(resolveManager, translationPrimitives)
        {
            this.typesCache = typesCache;
        }

        public override VmServiceGeneralDescription TranslateEntityToVm(StatutoryServiceGeneralDescription entity)
        {
            if (entity == null) return null;
            SetLanguage(LanguageCode.fi);
            return CreateEntityViewModelDefinition(entity)
                .AddPartial(i => VersioningManager.ApplyPublishingStatusFilterFallback(i.Versions))
                .AddSimple(i => i.Id, o => o.Id)
                .GetFinal();

        }

        public override StatutoryServiceGeneralDescription TranslateVmToEntity(VmServiceGeneralDescription vModel)
        {
            throw new NotImplementedException();
        }
    }

}
