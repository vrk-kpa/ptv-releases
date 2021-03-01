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

using Microsoft.Extensions.Logging;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Types
{
    [RegisterService(typeof(ITranslator<ProvisionType, string>), RegisterType.Transient)]
    internal class ProvisionTypeCodeTranslator : Translator<ProvisionType, string>
    {
        private readonly ILogger logger;
        
        public ProvisionTypeCodeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ILogger<ProvisionTypeCodeTranslator> logger) 
            : base(resolveManager, translationPrimitives)
        {
            this.logger = logger;
        }

        public override string TranslateEntityToVm(ProvisionType entity)
        {
            logger.LogWarning("This is obsolete. Use type cache instead of this.");
            return CreateEntityViewModelDefinition<string>(entity)
                .AddNavigation(i=>i.Code, o=>o)
                .GetFinal();
        }

        public override ProvisionType TranslateVmToEntity(string vModel)
        {
            logger.LogWarning("This is obsolete. Use type cache instead of this.");
            return CreateViewModelEntityDefinition<ProvisionType>(vModel)
                .UseDataContextLocalizedUpdate(input => true, input => output => input == output.Code)
                .GetFinal();
        }
    }
}
