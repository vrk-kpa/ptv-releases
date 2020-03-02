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
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<ServiceWebPage, V9VmOpenApiServiceVoucher>), RegisterType.Transient)]
    internal class OpenApiServiceVoucherServiceWebPageTranslator : Translator<ServiceWebPage, V9VmOpenApiServiceVoucher>
    {

        private readonly ILanguageCache languageCache;

        public OpenApiServiceVoucherServiceWebPageTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override V9VmOpenApiServiceVoucher TranslateEntityToVm(ServiceWebPage entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
               .AddSimple(input => input.ServiceVersionedId, output => output.OwnerReferenceId);

            if (entity.WebPage != null)
            {
                definition
                    .AddSimple(i => i.OrderNumber.GetValueOrDefault(), o => o.OrderNumber)
                    .AddNavigation(i => i.Name, o => o.Value)
                    .AddNavigation(i => i.WebPage, o => o.Url)
                    .AddNavigation(i => i.Description, o => o.AdditionalInformation)
                    .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), output => output.Language);
            }

            return definition.GetFinal();
        }

        public override ServiceWebPage TranslateVmToEntity(V9VmOpenApiServiceVoucher vModel)
        {
            if (vModel == null)
            {
                return null;
            }
            var exists = vModel.Id.IsAssigned();
            vModel.OwnerReferenceId = vModel.OwnerReferenceId ?? Guid.Empty;

            var translation = CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextLocalizedUpdate(input => exists, input => output => (input.Id == output.WebPageId) &&
                                                                                   (!input.OwnerReferenceId.IsAssigned() || output.ServiceVersionedId == vModel.OwnerReferenceId));

            return translation
                .AddNavigation(input => input, output => output.WebPage)
                .GetFinal();
        }
    }
}
