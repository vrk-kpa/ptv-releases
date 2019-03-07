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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Services
{
    [RegisterService(typeof(ITranslator<ServiceProducerAdditionalInformation, VmServiceProducerAdditionalInformation>), RegisterType.Transient)]
    internal class ServiceProducerAdditionalInformationTranslator : Translator<ServiceProducerAdditionalInformation, VmServiceProducerAdditionalInformation>
    {
        private ILanguageCache languageCache;
        public ServiceProducerAdditionalInformationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            languageCache = cacheManager.LanguageCache;
        }

        public override VmServiceProducerAdditionalInformation TranslateEntityToVm(ServiceProducerAdditionalInformation entity)
        {
            throw new NotImplementedException("Translator ServiceProducerAdditionalInformation -> VmServiceProducer is not implemented.");
        }

        public override ServiceProducerAdditionalInformation TranslateVmToEntity(VmServiceProducerAdditionalInformation vModel)
        {
            if (vModel.LocalizationId.IsAssigned())
            {
                SetLanguage(vModel.LocalizationId.Value);
            }

            return CreateViewModelEntityDefinition<ServiceProducerAdditionalInformation>(vModel)
                .UseDataContextLocalizedUpdate(i => i.ServiceProducerId.HasValue, i => o => (i.ServiceProducerId == o.ServiceProducerId) && (i.LocalizationId == o.LocalizationId), def => def.UseDataContextCreate(i => true))
                .AddNavigation(input => input.Text, output => output.Text)
                .AddRequestLanguage(output => output)
                .GetFinal();
        }
    }
}
