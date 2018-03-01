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

using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.Interfaces.OpenApi;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelVersioned, VmOpenApiPhoneChannelInVersionBase>), RegisterType.Transient)]
    internal class OpenApiPhoneChannelInTranslator : OpenApiServiceChannelInTranslator<VmOpenApiPhoneChannelInVersionBase>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        public OpenApiPhoneChannelInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives, cacheManager)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }
        public override VmOpenApiPhoneChannelInVersionBase TranslateEntityToVm(ServiceChannelVersioned entity)
        {
            return base.TranslateEntityToVm(entity);
        }

        public override ServiceChannelVersioned TranslateVmToEntity(VmOpenApiPhoneChannelInVersionBase vModel)
        {
            if (vModel.WebPage != null)
            {
                var i = 1;
                vModel.WebPage.ForEach(u => vModel.WebPages.Add(new VmOpenApiWebPageWithOrderNumber()
                {
                    Url = u.Value,
                    Language = u.Language,
                    ExistsOnePerLanguage = true,
                    OwnerReferenceId = vModel.Id,
                    OrderNumber = (i++).ToString()
                }));
            }

            var counter = 1;
            vModel.PhoneNumbers.ForEach(p => {
                p.OwnerReferenceId = vModel.Id;
                p.ExistsOnePerLanguage = false;
                p.OrderNumber = counter++;
            });

            var definitions = CreateVmToChannelDefinitions(vModel)
                .DisableAutoTranslation()
                .AddNavigation(i => ServiceChannelTypeEnum.Phone.ToString(), o => o.Type);

            if (vModel.PhoneNumbers?.Count > 0)
            {
                definitions.AddCollection(i => vModel.PhoneNumbers, o => o.Phones, false);
            }

            return definitions.GetFinal();
        }
    }
}
