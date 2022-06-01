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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.V2.TranslationOrder.Json;

namespace PTV.Database.DataAccess.Translators.TranslationOrder.Json
{
    [RegisterService(typeof(ITranslator<StatutoryServiceDescription, VmJsonTranslationText>), RegisterType.Transient)]
    internal class JsonGeneralDescriptionDescriptionTranslator : Translator<StatutoryServiceDescription, VmJsonTranslationText>
    {
        private readonly ITypesCache typesCache;

        public JsonGeneralDescriptionDescriptionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmJsonTranslationText TranslateEntityToVm(StatutoryServiceDescription entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(GetDescription, o => o.Text)
                .GetFinal();
        }

        public override StatutoryServiceDescription TranslateVmToEntity(VmJsonTranslationText vModel)
        {
            throw new NotImplementedException();
        }

        private string GetDescription(StatutoryServiceDescription entity)
        {
            if (typesCache.Compare<DescriptionType>(entity.TypeId, DescriptionTypeEnum.Description.ToString())
                || typesCache.Compare<DescriptionType>(entity.TypeId, DescriptionTypeEnum.ServiceUserInstruction.ToString())
                || typesCache.Compare<DescriptionType>(entity.TypeId, DescriptionTypeEnum.BackgroundDescription.ToString())
                || typesCache.Compare<DescriptionType>(entity.TypeId, DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString())
                || typesCache.Compare<DescriptionType>(entity.TypeId, DescriptionTypeEnum.DeadLineAdditionalInfo.ToString())
                || typesCache.Compare<DescriptionType>(entity.TypeId, DescriptionTypeEnum.ProcessingTimeAdditionalInfo.ToString())
                || typesCache.Compare<DescriptionType>(entity.TypeId, DescriptionTypeEnum.ValidityTimeAdditionalInfo.ToString())
                || typesCache.Compare<DescriptionType>(entity.TypeId, DescriptionTypeEnum.GeneralDescriptionTypeAdditionalInformation.ToString()))
            {
                return textManager.ConvertToMarkDown(entity.Description, true);
            }

            return entity.Description;
        }
    }
}
