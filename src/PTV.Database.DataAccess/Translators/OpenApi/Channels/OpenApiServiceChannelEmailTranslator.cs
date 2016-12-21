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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Caches;
using PTV.Domain.Model.Models.OpenApi.V2;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelEmail, VmOpenApiEmail>), RegisterType.Transient)]
    internal class OpenApiServiceChannelEmailTranslator :  Translator<ServiceChannelEmail, VmOpenApiEmail>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public OpenApiServiceChannelEmailTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiEmail TranslateEntityToVm(ServiceChannelEmail entity)
        {
            return CreateEntityViewModelDefinition<VmOpenApiEmail>(entity)
                    .AddNavigation(i => i.Email.Value, o => o.Value)
                    .AddNavigation(i => i.Email.Description, o => o.Description)
                    .AddNavigation(i => languageCache.GetByValue(i.Email.LocalizationId), o => o.Language)
                    .GetFinal();
        }

        public override ServiceChannelEmail TranslateVmToEntity(VmOpenApiEmail vModel)
        {
            if (vModel == null)
            {
                return null;
            }

            var exists = vModel.OwnerReferenceId.IsAssigned();

            var definition = CreateViewModelEntityDefinition(vModel).UseDataContextCreate(i => !exists);
            if (vModel.ExistsOnePerLanguage)
            {
                definition.UseDataContextUpdate(i => exists, i => o => i.OwnerReferenceId == o.ServiceChannelId &&
                    languageCache.Get(i.Language) == o.Email.LocalizationId, e => e.UseDataContextCreate(x => true));
            }
            else
            {
                definition.UseDataContextUpdate(input => exists,
                    input => output => (input.OwnerReferenceId == output.ServiceChannelId && input.Value == output.Email.Value),
                    e => e.UseDataContextCreate(x => true));
            }

            if (exists)
            {
                var entity = definition.GetFinal();
                if (entity.Created != DateTime.MinValue) // We are updating existing email
                {
                    vModel.Id = entity.EmailId;
                }
            }

            definition.AddNavigation(input => input, o => o.Email);
            return definition.GetFinal();
        }
    }
}
