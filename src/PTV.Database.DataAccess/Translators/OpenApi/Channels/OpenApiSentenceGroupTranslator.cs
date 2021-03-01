﻿/**
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
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Linq;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;

namespace PTV.Database.DataAccess.Translators.OpenApi.Channels
{
    [RegisterService(typeof(ITranslator<AccessibilityRegisterGroup, VmOpenApiAccessibilitySentence>), RegisterType.Transient)]
    internal class OpenApiSentenceGroupTranslator : Translator<AccessibilityRegisterGroup, VmOpenApiAccessibilitySentence>
    {
        public OpenApiSentenceGroupTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives) { }

        public override VmOpenApiAccessibilitySentence TranslateEntityToVm(AccessibilityRegisterGroup entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddCollection(i => i.Values, o => o.SentenceGroup)
                .AddCollection(i => i.Sentences.OrderBy(j => j.OrderNumber), o => o.Sentences)
                .GetFinal();
        }

        public override AccessibilityRegisterGroup TranslateVmToEntity(VmOpenApiAccessibilitySentence vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiSentenceGroupTranslator");
        }
    }

    [RegisterService(typeof(ITranslator<AccessibilityRegisterGroupValue, VmOpenApiLanguageItem>), RegisterType.Transient)]
    internal class OpenApiSentenceGroupValueTranslator : Translator<AccessibilityRegisterGroupValue, VmOpenApiLanguageItem>
    {
        public OpenApiSentenceGroupValueTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiLanguageItem TranslateEntityToVm(AccessibilityRegisterGroupValue entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(i => string.IsNullOrEmpty(i.Value) ? null : i.Value, o => o.Value)
                .AddNavigation(i => languageCache.GetByValue(i.LocalizationId), o => o.Language)
                .GetFinal();
        }

        public override AccessibilityRegisterGroupValue TranslateVmToEntity(VmOpenApiLanguageItem vModel)
        {
            throw new NotImplementedException("No translation implemented in OpenApiSentenceGroupValueTranslator");
        }
    }
}
