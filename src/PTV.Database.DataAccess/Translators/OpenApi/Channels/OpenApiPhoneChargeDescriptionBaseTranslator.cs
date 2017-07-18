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

using PTV.Domain.Model.Models.OpenApi;
using PTV.Database.Model.Interfaces;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Caches;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    internal abstract class OpenApiPhoneChargeDescriptionBaseTranslator<T> : Translator<T, VmOpenApiLanguageItem> where T : class, IPhoneChargeDescription
    {
        private ILanguageCache languageCache;

        protected OpenApiPhoneChargeDescriptionBaseTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILanguageCache languageCache) : base(resolveManager, translationPrimitives)
        {
            this.languageCache = languageCache;
        }

        public override VmOpenApiLanguageItem TranslateEntityToVm(T entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.Description, o => o.Value)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .GetFinal();
        }

        protected ITranslationDefinitions<VmOpenApiLanguageItem, T> AddDefaultTransaltion(ITranslationDefinitions<VmOpenApiLanguageItem, T> definition)
        {
            return definition
                .AddNavigation(i => i.Value, o => o.Description)
                .AddNavigation(i => i.Language, o => o.Localization);
        }

        public override T TranslateVmToEntity(VmOpenApiLanguageItem vModel)
        {
            return AddDefaultTransaltion(CreateViewModelEntityDefinition(vModel)).GetFinal();
        }
    }
}
