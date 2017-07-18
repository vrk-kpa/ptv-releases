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
using PTV.Database.DataAccess.Utils;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.V2.GeneralDescriptions;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.GeneralDescription.V2
{
    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionDialog>), RegisterType.Transient)]
    internal class GeneralDescriptionVersionedTranslator : Translator<StatutoryServiceGeneralDescriptionVersioned, VmGeneralDescriptionDialog>
    {
        private ITypesCache typesCache;
        private ILanguageCache languageCache;
        private DataUtils dataUtils;
        public GeneralDescriptionVersionedTranslator(IResolveManager resolveManager, DataUtils dataUtils, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
            this.dataUtils = dataUtils;
        }

        public override VmGeneralDescriptionDialog TranslateEntityToVm(StatutoryServiceGeneralDescriptionVersioned entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddSimple(i => i.UnificRootId, o => o.Id)
                .AddSimple(i => i.UnificRootId, o => o.UnificRootId)
                .AddDictionary(i => dataUtils.GetSpecificLanguageItem(i.Names.Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.Name.ToString())), RequestLanguageId).Select(x => x) , o => o.Name, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => dataUtils.GetSpecificLanguageItem(i.Names.Where(j => j.TypeId == typesCache.Get<NameType>(NameTypeEnum.AlternateName.ToString())), RequestLanguageId).Select(x => x) , o => o.AlternateName, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => dataUtils.GetSpecificLanguageItem(i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString())).Select(x => x), RequestLanguageId) , o => o.Description, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddDictionary(i => dataUtils.GetSpecificLanguageItem(i.Descriptions.Where(j => j.TypeId == typesCache.Get<DescriptionType>(DescriptionTypeEnum.ShortDescription.ToString())).Select(x => x), RequestLanguageId) , o => o.ShortDescription, k => languageCache.GetByValueEnum(k.LocalizationId).ToString())
                .AddSimpleList(i => i.TargetGroups.Select(j => j.TargetGroupId), o => o.TargetGroups)
                .AddSimpleList(i => i.ServiceClasses.Select(j => j.ServiceClassId), o => o.ServiceClasses)
                .AddCollection(i => i.OntologyTerms.Select(x => x.OntologyTerm), o => o.OntologyTerms)
                .GetFinal();
        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(VmGeneralDescriptionDialog vModel)
        {
            throw new NotImplementedException();
        }
    }
}
