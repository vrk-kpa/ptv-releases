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

using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using System;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.OpenApi.Services
{
    [RegisterService(typeof(ITranslator<StatutoryServiceDescription, VmOpenApiLocalizedListItem>), RegisterType.Transient)]
    internal class OpenApiStatutoryServiceDescriptionTranslator : Translator<StatutoryServiceDescription, VmOpenApiLocalizedListItem>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public OpenApiStatutoryServiceDescriptionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiLocalizedListItem TranslateEntityToVm(StatutoryServiceDescription entity)
        {
            return CreateEntityViewModelDefinition<VmOpenApiLocalizedListItem>(entity)
                .AddNavigation(i => IsTextEditorField(entity.TypeId) ? textManager.ConvertToMarkDown(i.Description) : i.Description, o => o.Value)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .AddNavigation(i => typesCache.GetByValue<DescriptionType>(i.TypeId), o => o.Type)
                .GetFinal();
        }

        public override StatutoryServiceDescription TranslateVmToEntity(VmOpenApiLocalizedListItem vModel)
        {
            return CreateViewModelEntityDefinition<StatutoryServiceDescription>(vModel)
                .UseDataContextUpdate(i => i.OwnerReferenceId.HasValue, i => o => o.Type.Code == i.Type &&
                    o.LocalizationId == languageCache.Get(i.Language) && i.OwnerReferenceId == o.StatutoryServiceGeneralDescriptionVersionedId, name => name.UseDataContextCreate(x => true))
                .AddNavigation(i => IsTextEditorField(typesCache.Get<DescriptionType>(vModel.Type)) ? textManager.ConvertMarkdownToJson(i.Value) : i.Value, o => o.Description)
                .AddNavigation(i => i.Language, o => o.Localization)
                .AddSimple(i => typesCache.Get<DescriptionType>(i.Type), o => o.TypeId)
                .GetFinal();
        }

        private bool IsTextEditorField(Guid typeId)
        {
            return (typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()) == typeId ||
                    typesCache.Get<DescriptionType>(DescriptionTypeEnum.ServiceUserInstruction.ToString()) == typeId ||
                    typesCache.Get<DescriptionType>(DescriptionTypeEnum.BackgroundDescription.ToString()) == typeId);
        }
    }
}
