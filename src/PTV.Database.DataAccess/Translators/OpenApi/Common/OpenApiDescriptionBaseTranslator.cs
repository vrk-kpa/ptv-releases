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

using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;
using System;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    internal abstract class OpenApiDescriptionBaseTranslator<TEntity> :
        Translator<TEntity, VmOpenApiLanguageItem> where TEntity : class, IDescription, ILocalizable
    {
        private bool convertToMarkDown;

        protected OpenApiDescriptionBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager, bool convertToMarkDown = false) : base(resolveManager, translationPrimitives)
        {
            this.convertToMarkDown = convertToMarkDown;
        }

        public override VmOpenApiLanguageItem TranslateEntityToVm(TEntity entity)
        {
            var description = convertToMarkDown ? textManager.ConvertToMarkDown(entity.Description) : entity.Description;
            return CreateEntityViewModelDefinition<VmOpenApiLanguageItem>(entity)
                .AddNavigation(i => string.IsNullOrEmpty(description) ? null : description, o => o.Value)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .GetFinal();
        }
    }

    internal abstract class OpenApiDescriptionWithTypeBaseTranslator<TEntity> :
        Translator<TEntity, VmOpenApiLocalizedListItem> where TEntity : class, IDescription, ILocalizable
    {
        private readonly ITypesCache typesCache;

        protected OpenApiDescriptionWithTypeBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiLocalizedListItem TranslateEntityToVm(TEntity entity)
        {
            var description = IsTextEditorField(entity.TypeId) ? textManager.ConvertToMarkDown(entity.Description) : entity.Description;
            // Description type ShortDescription changed into Summary(PTV-2184)
            var type = typesCache.GetByValue<DescriptionType>(entity.TypeId);
            return CreateEntityViewModelDefinition<VmOpenApiLocalizedListItem>(entity)
                .AddNavigation(i => string.IsNullOrEmpty(description) ? null : description, o => o.Value)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .AddNavigation(i => string.IsNullOrEmpty(type) ? null : type.GetOpenApiEnumValue<DescriptionTypeEnum>(), o => o.Type)
                .GetFinal();
        }

        protected bool IsTextEditorField(Guid typeId)
        {
            return (typesCache.Get<DescriptionType>(DescriptionTypeEnum.Description.ToString()) == typeId ||
                    typesCache.Get<DescriptionType>(DescriptionTypeEnum.ServiceUserInstruction.ToString()) == typeId ||
                    typesCache.Get<DescriptionType>(DescriptionTypeEnum.BackgroundDescription.ToString()) == typeId);
        }
    }
}
