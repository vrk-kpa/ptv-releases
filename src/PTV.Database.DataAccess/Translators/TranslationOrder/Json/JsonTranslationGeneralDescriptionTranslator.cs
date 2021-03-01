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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.V2.TranslationOrder.Json;

namespace PTV.Database.DataAccess.Translators.TranslationOrder.Json
{
    [RegisterService(typeof(ITranslator<StatutoryServiceGeneralDescriptionVersioned, VmJsonGeneralDescriptionTranslation>), RegisterType.Transient)]
    internal class JsonTranslationGeneralDescriptionTranslator : Translator<StatutoryServiceGeneralDescriptionVersioned, VmJsonGeneralDescriptionTranslation>
    {
        private readonly ITypesCache typesCache;

        public JsonTranslationGeneralDescriptionTranslator(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager,
            translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmJsonGeneralDescriptionTranslation TranslateEntityToVm(StatutoryServiceGeneralDescriptionVersioned entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddNavigation(input => GetName(input, NameTypeEnum.Name, RequestLanguageId), output => output.Name)
                .AddNavigation(input => GetName(input, NameTypeEnum.AlternateName, RequestLanguageId), output => output.AlternateName)
                .AddNavigation(input => GetDescription(input, DescriptionTypeEnum.Description, RequestLanguageId), output => output.Description)
                .AddNavigation(input => GetDescription(input, DescriptionTypeEnum.ShortDescription, RequestLanguageId), output => output.Summary)
                .AddNavigation(input => GetConditionsAndCriteria(input, RequestLanguageId), output => output.ConditionsAndCriteria)
                .AddNavigation(input => GetDescription(input, DescriptionTypeEnum.ServiceUserInstruction, RequestLanguageId), output => output.UserInstructions)
                .AddNavigation(input => GetDescription(input, DescriptionTypeEnum.ChargeTypeAdditionalInfo, RequestLanguageId), output => output.ChargeAdditionalInformation)
                .AddNavigation(input => GetDescription(input, DescriptionTypeEnum.DeadLineAdditionalInfo, RequestLanguageId), output => output.Deadline)
                .AddNavigation(input => GetDescription(input, DescriptionTypeEnum.ProcessingTimeAdditionalInfo, RequestLanguageId), output => output.ProcessingTime)
                .AddNavigation(input => GetDescription(input, DescriptionTypeEnum.ValidityTimeAdditionalInfo, RequestLanguageId), output => output.PeriodOfValidity)
                .AddNavigation(input => GetDescription(input, DescriptionTypeEnum.GeneralDescriptionTypeAdditionalInformation, RequestLanguageId), output => output.GeneralDescriptionTypeAdditionalInformation)
                .AddNavigation(input => GetDescription(input, DescriptionTypeEnum.BackgroundDescription, RequestLanguageId), output => output.BackgroundDescription);
            var model = definition.GetFinal();
            modelUtility.SetMaxLengthToTranslationText(model);

            return model;
        }

        public override StatutoryServiceGeneralDescriptionVersioned TranslateVmToEntity(VmJsonGeneralDescriptionTranslation vModel)
        {
            var names = new List<VmName>();
            names.AddExceptNull(CreateName(vModel.Id, RequestLanguageId, vModel.Name?.Text, NameTypeEnum.Name));
            names.AddExceptNull(CreateName(vModel.Id, RequestLanguageId, vModel.AlternateName?.Text, NameTypeEnum.AlternateName));

            var descriptions = new List<VmDescription>();
            descriptions.AddExceptNull(CreateDescription(vModel.Id, RequestLanguageId, vModel.Summary?.Text, DescriptionTypeEnum.ShortDescription));
            descriptions.AddExceptNull(CreateDescription(vModel.Id, RequestLanguageId, vModel.Description?.Text, DescriptionTypeEnum.Description, true));
            descriptions.AddExceptNull(CreateDescription(vModel.Id, RequestLanguageId, vModel.UserInstructions?.Text, DescriptionTypeEnum.ServiceUserInstruction, true));
            descriptions.AddExceptNull(CreateDescription(vModel.Id, RequestLanguageId, vModel.ChargeAdditionalInformation?.Text, DescriptionTypeEnum.ChargeTypeAdditionalInfo, true));
            descriptions.AddExceptNull(CreateDescription(vModel.Id, RequestLanguageId, vModel.Deadline?.Text, DescriptionTypeEnum.DeadLineAdditionalInfo, true));
            descriptions.AddExceptNull(CreateDescription(vModel.Id, RequestLanguageId, vModel.ProcessingTime?.Text, DescriptionTypeEnum.ProcessingTimeAdditionalInfo, true));
            descriptions.AddExceptNull(CreateDescription(vModel.Id, RequestLanguageId, vModel.PeriodOfValidity?.Text, DescriptionTypeEnum.ValidityTimeAdditionalInfo, true));
            descriptions.AddExceptNull(CreateDescription(vModel.Id, RequestLanguageId, vModel.GeneralDescriptionTypeAdditionalInformation?.Text, DescriptionTypeEnum.GeneralDescriptionTypeAdditionalInformation));
            descriptions.AddExceptNull(CreateDescription(vModel.Id, RequestLanguageId, vModel.BackgroundDescription?.Text, DescriptionTypeEnum.BackgroundDescription, true));

            var transaltionDefinition = CreateViewModelEntityDefinition<StatutoryServiceGeneralDescriptionVersioned>(vModel)
                .UseDataContextUpdate(i => true, i => o => vModel.Id == o.Id)
                .UseVersioning<StatutoryServiceGeneralDescriptionVersioned, StatutoryServiceGeneralDescription>(o => o)
                .AddLanguageAvailability(i => i)
                .AddCollection(i => names, o => o.Names)
                .AddCollection(i => descriptions, o => o.Descriptions);

            var entity = transaltionDefinition.GetFinal();
            return entity;
        }

        public StatutoryServiceRequirement GetConditionsAndCriteria(StatutoryServiceGeneralDescriptionVersioned generalDescriptionVersioned, Guid localizationId)
        {
            return generalDescriptionVersioned.StatutoryServiceRequirements.Where(x => x.LocalizationId == localizationId).Select(x => x).FirstOrDefault();
        }

        private StatutoryServiceName GetName(StatutoryServiceGeneralDescriptionVersioned generalDescriptionVersioned, NameTypeEnum type, Guid localizationId)
        {
            return generalDescriptionVersioned.Names.FirstOrDefault(x => x.LocalizationId == localizationId && typesCache.Compare<NameType>(x.TypeId, type.ToString()));
        }

        private StatutoryServiceDescription GetDescription(StatutoryServiceGeneralDescriptionVersioned generalDescriptionVersioned, DescriptionTypeEnum type, Guid localizationId)
        {
            return generalDescriptionVersioned.Descriptions.Where(x => x.LocalizationId == localizationId && typesCache.Compare<DescriptionType>(x.TypeId, type.ToString())).Select(x => x).FirstOrDefault();
        }

        private VmName CreateName(Guid entityId, Guid languageId, string value, NameTypeEnum typeEnum)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            return new VmName
            {
                Name = value,
                TypeId = typesCache.Get<NameType>(typeEnum.ToString()),
                OwnerReferenceId = entityId,
                LocalizationId = languageId
            };
        }

        private VmDescription CreateDescription(Guid entityId, Guid languageId, string value, DescriptionTypeEnum typeEnum, bool isMarkDown = false)
        {
            if (string.IsNullOrEmpty(value)) return null;

            return new VmDescription
            {
                Description = isMarkDown ? textManager.ConvertMarkdownToJson(value) : value,
                TypeId = typesCache.Get<DescriptionType>(typeEnum.ToString()),
                OwnerReferenceId = entityId,
                LocalizationId = languageId
            };
        }
    };
}
