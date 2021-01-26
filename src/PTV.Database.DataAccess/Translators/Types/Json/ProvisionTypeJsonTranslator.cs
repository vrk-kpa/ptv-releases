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
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models.Import;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Types.Json
{
    [RegisterService(typeof(ITranslator<ProvisionType, VmJsonTypeItem>), RegisterType.Scope)]
    internal class ProvisionTypeJsonTranslator : Translator<ProvisionType, VmJsonTypeItem>
    {
        public ProvisionTypeJsonTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }
        
        public override VmJsonTypeItem TranslateEntityToVm(ProvisionType entity)
        {
            throw new NotSupportedException();
        }

        public override ProvisionType TranslateVmToEntity(VmJsonTypeItem vModel)
        {
            var orderMapping = new Dictionary<string, int?>();
            
            var definition = CreateViewModelEntityDefinition<ProvisionType>(vModel)
                .UseDataContextLocalizedUpdate(i => true, i => o => i.Code == o.Code, def =>
                {
                    def.UseDataContextCreate(i => true, o => o.Id, i => i.Code.GetGuid<ProvisionType>())
                        .AddNavigation(input => input.Code, output => output.Code);
                })
                .AddSimple(input => orderMapping.TryGet(input.Code) ?? input.OrderNumber, output => output.OrderNumber);;
            var instance = definition.GetFinal();
            vModel.Names.ForEach(i => i.TypeId = instance.Id);
            definition.AddCollection(input => input.Names, output => output.Names);
            return definition.GetFinal();
        }
    }
}
