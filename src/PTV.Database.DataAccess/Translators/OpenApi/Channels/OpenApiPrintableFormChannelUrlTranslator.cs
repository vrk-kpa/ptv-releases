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


namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<PrintableFormChannelUrl, VmOpenApiLocalizedListItem>), RegisterType.Transient)]
    internal class OpenApiPrintableFormChannelUrlTranslator : Translator<PrintableFormChannelUrl, VmOpenApiLocalizedListItem>
    {
        private readonly ILanguageCache languageCache;
        private readonly ITypesCache typesCache;

        public OpenApiPrintableFormChannelUrlTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiLocalizedListItem TranslateEntityToVm(PrintableFormChannelUrl entity)
        {
            return CreateEntityViewModelDefinition<VmOpenApiLocalizedListItem>(entity)
                .AddNavigation(i => typesCache.GetByValue<PrintableFormChannelUrlType>(i.TypeId), o => o.Type)
                .AddNavigation(i => i.Url, o => o.Value)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .GetFinal();
        }

        public override PrintableFormChannelUrl TranslateVmToEntity(VmOpenApiLocalizedListItem vModel)
        {
            var exists = vModel.OwnerReferenceId.IsAssigned();
            var typeid = typesCache.Get<PrintableFormChannelUrlType>(vModel.Type);

            return CreateViewModelEntityDefinition< PrintableFormChannelUrl>(vModel)
                .UseDataContextCreate(input => !exists)
                .UseDataContextUpdate(input => exists,
                    input => output => (input.OwnerReferenceId.Value == output.PrintableFormChannelId && typeid == output.TypeId &&
                    languageCache.Get(input.Language) == output.LocalizationId && input.Value == output.Url),
                    e => e.UseDataContextCreate(x => true))
                .AddSimple(i => typesCache.Get<PrintableFormChannelUrlType>(i.Type), o => o.TypeId)
                .AddNavigation(i => i.Value, o => o.Url)
                .AddNavigation(i => i.Language, o => o.Localization)
                .GetFinal();
        }
    }
}
