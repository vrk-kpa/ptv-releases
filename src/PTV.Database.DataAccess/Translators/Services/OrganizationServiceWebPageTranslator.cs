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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<OrganizationServiceWebPage, VmServiceProducerDetail>), RegisterType.Transient)]
    internal class OrganizationServiceWebPageTranslator : Translator<OrganizationServiceWebPage, VmServiceProducerDetail>
    {
        private ILanguageCache languageCache;
        public OrganizationServiceWebPageTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override VmServiceProducerDetail TranslateEntityToVm(OrganizationServiceWebPage entity)
        {
            throw new NotImplementedException();
        }

        public override OrganizationServiceWebPage TranslateVmToEntity(VmServiceProducerDetail vModel)
        {
            Guid languageId = languageCache.Get(RequestLanguageCode.ToString());
            return CreateViewModelEntityDefinition<OrganizationServiceWebPage>(vModel)
                .UseDataContextUpdate(input => input.OwnerReferenceId.IsAssigned(),
                        input => output => output.OrganizationServiceId == input.OwnerReferenceId && output.WebPage.LocalizationId == languageId,
                        definition => definition.UseDataContextCreate(input => true))

                .AddNavigation(input => input, output => output.WebPage)
                .AddNavigation(input => WebPageTypeEnum.HomePage.ToString(), output => output.Type)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<WebPage, VmServiceProducerDetail>), RegisterType.Transient)]
    internal class ServiceProduceWebPageTranslator : Translator<WebPage, VmServiceProducerDetail>
    {
        public ServiceProduceWebPageTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmServiceProducerDetail TranslateEntityToVm(WebPage entity)
        {
            throw new NotImplementedException();
        }

        public override WebPage TranslateVmToEntity(VmServiceProducerDetail vModel)
        {
            return CreateViewModelEntityDefinition<WebPage>(vModel)
                .DisableAutoTranslation()
                .UseDataContextLocalizedUpdate(input => input.OwnerReferenceId.IsAssigned(),
                        input => output => output.OrganizationServiceWebPages.Any(x => x.OrganizationServiceId == input.OwnerReferenceId),
                        definition => definition.UseDataContextCreate(input => true, output => output.Id, input => Guid.NewGuid()))

                .AddPartial(input => input.Link)
                .GetFinal();
        }
    }
}
