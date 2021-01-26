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
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Domain.Model.Enums;

namespace PTV.Database.DataAccess.Translators.Channels
{
    [RegisterService(typeof(ITranslator<ServiceChannelAccessibilityClassification, VmAccessibilityClassification>), RegisterType.Transient)]
    internal class ServiceChannelAccessibilityClassificationTranslator : Translator<ServiceChannelAccessibilityClassification, VmAccessibilityClassification>
    {
        public ServiceChannelAccessibilityClassificationTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmAccessibilityClassification TranslateEntityToVm(ServiceChannelAccessibilityClassification entity)
        {;
            throw new NotImplementedException();
        }

        public override ServiceChannelAccessibilityClassification TranslateVmToEntity(VmAccessibilityClassification vModel)
        {
            if (vModel.LocalizationId.IsAssigned())
            {
                SetLanguage(vModel.LocalizationId.Value);
            }
            var translationDefinition = CreateViewModelEntityDefinition<ServiceChannelAccessibilityClassification>(vModel)
                .UseDataContextUpdate(
                    i => i.OwnerReferenceId.IsAssigned() && i.Id.IsAssigned(),
                    i => o =>
                        (i.OwnerReferenceId == o.ServiceChannelVersionedId && o.AccessibilityClassificationId == i.Id) &&
                        (RequestLanguageId == o.AccessibilityClassification.LocalizationId)
                )
                .AddNavigation(input => input, output => output.AccessibilityClassification);

            var entity = translationDefinition.GetFinal();
            return entity;
        }
    }
}
