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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Framework.Extensions;

namespace PTV.Database.DataAccess.Translators.OpenApi.ServiceAndChannels
{
    [RegisterService(typeof(ITranslator<ServiceServiceChannelDescription, VmOpenApiLocalizedListItem>), RegisterType.Transient)]
    internal class OpenApiServiceServiceChannelDescriptionTranslator : Translator<ServiceServiceChannelDescription, VmOpenApiLocalizedListItem>
    {
        private readonly ITypesCache typesCache;

        public OpenApiServiceServiceChannelDescriptionTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiLocalizedListItem TranslateEntityToVm(ServiceServiceChannelDescription entity)
        {    
            var description = IsTextEditorField(entity.TypeId) ? textManager.ConvertToMarkDown(entity.Description) : entity.Description;
            return CreateEntityViewModelDefinition<VmOpenApiLocalizedListItem>(entity)
                .AddNavigation(i => string.IsNullOrEmpty(description) ? null : description, o => o.Value)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .AddNavigation(i => typesCache.GetByValue<DescriptionType>(i.TypeId), o => o.Type)
                .GetFinal();
        }

        public override ServiceServiceChannelDescription TranslateVmToEntity(VmOpenApiLocalizedListItem vModel)
        {
            var typeId = typesCache.Get<DescriptionType>(vModel.Type.GetEnumValueByOpenApiEnumValue<DescriptionTypeEnum>());
            var languageId = languageCache.Get(vModel.Language);
            return CreateViewModelEntityDefinition<ServiceServiceChannelDescription>(vModel)
                .UseDataContextUpdate(i => true, i => o => (i.OwnerReferenceId == o.ServiceId) && (i.OwnerReferenceId2 == o.ServiceChannelId) && (typeId == o.TypeId) && languageId == o.LocalizationId,
                    def => def.UseDataContextCreate(i => true))
                .AddNavigation(i => IsTextEditorField(typeId) ? textManager.ConvertMarkdownToJson(i.Value) : i.Value, o => o.Description)
                .AddSimple(i => languageId, o => o.LocalizationId)
                .AddSimple(i => typeId, o => o.TypeId)
                .GetFinal();
        }

        private bool IsTextEditorField(Guid typeId)
        {
            return (typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()) == typeId ||
                    typesCache.Get<DescriptionType>(DescriptionTypeEnum.ChargeTypeAdditionalInfo.ToString()) == typeId);
        }
    }
}
