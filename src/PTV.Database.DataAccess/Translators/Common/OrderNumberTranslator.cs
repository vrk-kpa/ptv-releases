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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace PTV.Database.DataAccess.Translators.Common
{
    [RegisterService(typeof(ITranslator<IOrderable, IVmOrderable>), RegisterType.Transient)]
    internal class OrderNumberTranslator : Translator<IOrderable, IVmOrderable>
    {
        public OrderNumberTranslator(
            IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives
        ) : base(
            resolveManager,
            translationPrimitives
        )
        {
        }

        public override IVmOrderable TranslateEntityToVm(IOrderable entity)
        {
            return CreateEntityViewModelDefinition<IVmOrderable>(entity)
                .AddSimple(i => i.OrderNumber ?? 0, o => o.OrderNumber)
                .GetFinal();
        }

        public override IOrderable TranslateVmToEntity(IVmOrderable vModel)
        {
            return CreateViewModelEntityDefinition<IOrderable>(vModel)
                .AddSimple(i => i.OrderNumber ?? 0, o => o.OrderNumber)
                .GetFinal();

        }
    }
}
