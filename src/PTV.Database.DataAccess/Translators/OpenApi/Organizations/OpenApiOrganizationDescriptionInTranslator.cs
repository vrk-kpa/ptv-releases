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
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Enums;
using System;
using PTV.Database.DataAccess.Caches;

namespace PTV.Database.DataAccess.Translators.OpenApi.Organizations
{
    [RegisterService(typeof(ITranslator<OrganizationDescription, VmOpenApiLanguageItem>), RegisterType.Transient)]
    internal class OpenApiOrganizationDescriptionInTranslator : Translator<OrganizationDescription, VmOpenApiLanguageItem>
    {
        private ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public OpenApiOrganizationDescriptionInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiLanguageItem TranslateEntityToVm(OrganizationDescription entity)
        {
            throw new NotImplementedException();
        }

        public override OrganizationDescription TranslateVmToEntity(VmOpenApiLanguageItem vModel)
        {
            var typeDescription = DescriptionTypeEnum.Description.ToString();
            var typeDescriptionId = typesCache.Get<DescriptionType>(typeDescription);

            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.OwnerReferenceId.HasValue)
                .UseDataContextUpdate(i => i.OwnerReferenceId.HasValue, i => o => typeDescriptionId == o.TypeId && languageCache.Get(i.Language) == o.LocalizationId && i.OwnerReferenceId == o.OrganizationId, def => def.UseDataContextCreate(i => i.OwnerReferenceId.IsAssigned()))
                .AddNavigation(i => i.Value, o => o.Description)
                .AddNavigation(i => i.Language, o => o.Localization)
                .AddNavigation(i => typeDescription, o => o.Type)
                .GetFinal();
        }
    }
}
