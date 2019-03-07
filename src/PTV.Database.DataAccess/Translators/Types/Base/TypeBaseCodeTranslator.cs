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
using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models.Base;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Types.Base
{
    internal abstract class TypeBaseCodeTranslator<T> : Translator<T, string> where T : TypeBase
    {
        private ILogger logger;

        protected TypeBaseCodeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILogger logger) : base(resolveManager, translationPrimitives)
        {
            this.logger = logger;
        }

        public override string TranslateEntityToVm(T entity)
        {
            logger.LogWarning("This is obsolete. Use type cache instead of this.");
            return CreateEntityViewModelDefinition<string>(entity)
                .AddNavigation(i=>i.Code, o=>o)
                .GetFinal();
        }

        public override T TranslateVmToEntity(string vModel)
        {
            logger.LogWarning("This is obsolete. Use type cache instead of this.");
            return CreateViewModelEntityDefinition<T>(vModel)
                .UseDataContextLocalizedUpdate(input => true, input => output => input == output.Code)
                .GetFinal();
        }
    }
}
