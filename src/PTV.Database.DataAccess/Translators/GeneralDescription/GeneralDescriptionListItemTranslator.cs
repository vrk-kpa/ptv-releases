﻿/**
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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.GeneralDescription
{
    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionListItem>), RegisterType.Transient)]
    internal class GeneralDescriptionListItemTranslator : Translator<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionListItem>
    {
        private readonly ITypesCache typesCache;

        public GeneralDescriptionListItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) 
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        // seems not used anymore
        public override VmGeneralDescriptionListItem TranslateEntityToVm(StatutoryServiceGeneralDescriptionVersioned entity)
        {
            var publishedLanguageIds = entity.LanguageAvailabilities.Where(x => x.StatusId == typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString())).Select(x => x.LanguageId).ToList();

            return CreateEntityViewModelDefinition(entity)
                .AddDictionary(i => i.Names.Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString()) && publishedLanguageIds.Contains(j.LocalizationId)), o => o.Name, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()) && publishedLanguageIds.Contains(j.LocalizationId)), o => o.Description, k => languageCache.GetByValue(k.LocalizationId))
                .AddDictionary(i => i.Descriptions.Where(j => j.TypeId == typesCache.Get <DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString()) && publishedLanguageIds.Contains(j.LocalizationId)), o => o.ShortDescription, k => languageCache.GetByValue(k.LocalizationId))
                .AddSimpleList(i => i.TargetGroups.Select(m => m.TargetGroupId), o => o.TargetGroups)
                .AddSimpleList(i => i.ServiceClasses.Select(j => j.ServiceClassId), o => o.ServiceClasses)
                .AddNavigation(i => i.Versioning, o => o.Version)
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddCollection<ILanguageAvailability, VmLanguageAvailabilityInfo>(i => i.LanguageAvailabilities.OrderBy(x => x.Language.OrderNumber), o => o.LanguagesAvailabilities)
                .GetFinal();
        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(VmGeneralDescriptionListItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
