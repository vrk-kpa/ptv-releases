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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Translators.Common;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<PostalCode, VmListItem>), RegisterType.Transient)]
    internal class PostalCodeVmListItemTranslator : Translator<PostalCode, VmListItem>
    {
        private TranslatedItemDefinitionHelper definitionHelper;

        public PostalCodeVmListItemTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, TranslatedItemDefinitionHelper definitionHelper) : base(resolveManager, translationPrimitives)
        {
            this.definitionHelper = definitionHelper;
        }

        public override VmListItem TranslateEntityToVm(PostalCode entity)
        {
            var definition = CreateEntityViewModelDefinition(entity)
                 .AddNavigation(i => i.Code, o => o.Code)
                 .AddSimple(i => i.Id, o => o.Id); ;

            definitionHelper.AddTranslations(definition, entity?.Code);
            return definition.GetFinal();
        }

        public override PostalCode TranslateVmToEntity(VmListItem vModel)
        {
            throw new NotImplementedException();
        }
    }
}
