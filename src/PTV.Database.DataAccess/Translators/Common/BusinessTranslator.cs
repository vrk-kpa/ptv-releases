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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Common
{
    [RegisterService(typeof(ITranslator<Business, VmBusiness>), RegisterType.Transient)]
    internal class BusinessTranslator : Translator<Business, VmBusiness>
    {
        public BusinessTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmBusiness TranslateEntityToVm(Business entity)
        {
            return CreateEntityViewModelDefinition<VmBusiness>(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.Name, o => o.Name)
                .AddNavigation(i => i.Code, o => o.Code)
                .GetFinal();
        }


        public override Business TranslateVmToEntity(VmBusiness vModel)
        {
            return CreateViewModelEntityDefinition<Business>(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned(), o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.Id)
                .AddNavigation(i => i.Code, o => o.Code)
                .AddNavigation(i => i.Name, o => o.Name)
                .GetFinal();
        }
    }
}
