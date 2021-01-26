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

using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using PTV.Domain.Model.Models.OpenApi;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<Model.Models.AccessibilityRegister, VmOpenApiAccessibilityContactInfo>), RegisterType.Transient)]
    internal class OpenApiAccessibilityContactInfoTranslator : Translator<Model.Models.AccessibilityRegister, VmOpenApiAccessibilityContactInfo>
    {
        public OpenApiAccessibilityContactInfoTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiAccessibilityContactInfo TranslateEntityToVm(Model.Models.AccessibilityRegister entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => i.ContactEmail, o => o.Email)
                .AddNavigation(i => i.ContactPhone, o => o.Phone)
                .AddNavigation(i => i.ContactUrl, o => o.Url)
                .GetFinal();
        }

        public override Model.Models.AccessibilityRegister TranslateVmToEntity(VmOpenApiAccessibilityContactInfo vModel)
        {
            throw new NotImplementedException("Translator VmOpenApiAccessibilityContactInfo -> AccessibilityRegister is not implemented.");
        }
    }
}
