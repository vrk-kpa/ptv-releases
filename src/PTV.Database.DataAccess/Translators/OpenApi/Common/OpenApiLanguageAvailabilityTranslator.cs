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
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<ILanguageAvailability, VmOpenApiLanguageItem>), RegisterType.Transient)]
    internal class OpenApiLanguageAvailabilityTranslator : Translator<ILanguageAvailability, VmOpenApiLanguageItem>
    {
        private readonly IPublishingStatusCache publishingStatusCache;
        private readonly ILanguageCache languageCache;

        public OpenApiLanguageAvailabilityTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            publishingStatusCache = cacheManager.PublishingStatusCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmOpenApiLanguageItem TranslateEntityToVm(ILanguageAvailability entity)
        {
            return CreateEntityViewModelDefinition<VmOpenApiLanguageItem>(entity)
                .AddNavigation(i => publishingStatusCache.GetByValue(i.StatusId), o => o.Value)
                .AddNavigation(i => languageCache.GetByValue(i.LanguageId), o => o.Language)
                .GetFinal();
        }

        public override ILanguageAvailability TranslateVmToEntity(VmOpenApiLanguageItem vModel)
        {
            throw new NotImplementedException();
        }
    }

    internal class OpenApiLanguageAvailabilityBaseTranslator<TEntity> : Translator<TEntity, VmOpenApiLanguageItem> where TEntity : class, ILanguageAvailability
    {
        private readonly IPublishingStatusCache publishingStatusCache;
        private readonly ILanguageCache languageCache;

        public OpenApiLanguageAvailabilityBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            publishingStatusCache = cacheManager.PublishingStatusCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmOpenApiLanguageItem TranslateEntityToVm(TEntity entity)
        {
            return CreateEntityViewModelDefinition<VmOpenApiLanguageItem>(entity)
                .AddNavigation(i => publishingStatusCache.GetByValue(i.StatusId), o => o.Value)
                .AddNavigation(i => languageCache.GetByValue(i.LanguageId), o => o.Language)
                .GetFinal();
        }

        public override TEntity TranslateVmToEntity(VmOpenApiLanguageItem vModel)
        {
            throw new NotImplementedException();
        }
    }

    [RegisterService(typeof(ITranslator<ServiceLanguageAvailability, VmOpenApiLanguageItem>), RegisterType.Transient)]
    internal class OpenApiTaskServiceLanguageAvailabilityTranslator : OpenApiLanguageAvailabilityBaseTranslator<ServiceLanguageAvailability>
    {
        public OpenApiTaskServiceLanguageAvailabilityTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives, cacheManager)
        {
        }
    }

    [RegisterService(typeof(ITranslator<ServiceChannelLanguageAvailability, VmOpenApiLanguageItem>), RegisterType.Transient)]
    internal class OpenApiTaskServiceChannelLanguageAvailabilityTranslator : OpenApiLanguageAvailabilityBaseTranslator<ServiceChannelLanguageAvailability>
    {
        public OpenApiTaskServiceChannelLanguageAvailabilityTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives, cacheManager)
        {
        }
    }
}
