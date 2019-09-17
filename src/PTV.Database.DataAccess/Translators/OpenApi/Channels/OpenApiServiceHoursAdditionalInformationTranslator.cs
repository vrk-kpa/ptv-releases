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
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Translators.OpenApi.Common;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceHoursAdditionalInformation, VmOpenApiLanguageItem>), RegisterType.Transient)]
    internal class OpenApiServiceHoursAdditionalInformationTranslator : OpenApiTextBaseTranslator<ServiceHoursAdditionalInformation>
    {
        private ILanguageCache languageCache;

        public OpenApiServiceHoursAdditionalInformationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives,
            ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
            this.languageCache = cacheManager.LanguageCache;
        }

        public override VmOpenApiLanguageItem TranslateEntityToVm(ServiceHoursAdditionalInformation entity)
        {
            return base.TranslateEntityToVm(entity);
        }

        public override ServiceHoursAdditionalInformation TranslateVmToEntity(VmOpenApiLanguageItem vModel)
        {
            var definition = CreateViewModelEntityDefinition<ServiceHoursAdditionalInformation>(vModel)
                .UseDataContextUpdate(i => i.OwnerReferenceId.HasValue, i => o => o.LocalizationId == languageCache.Get(i.Language) &&
                    o.ServiceHoursId == i.OwnerReferenceId, d => d.UseDataContextCreate(x => true))
                .AddNavigation(i => i.Value, o => o.Text)
                .AddRequestLanguage(i => i);

            if (!string.IsNullOrEmpty(vModel.Language))
            {
                definition.AddSimple(i => languageCache.Get(i.Language), o => o.LocalizationId);
            }

            return definition.GetFinal();
        }
    }
}
