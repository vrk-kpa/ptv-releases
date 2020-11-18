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
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
//    [RegisterService(typeof(ITranslator<PostOfficeBoxName, VmAddressSimple>), RegisterType.Transient)]
//    internal class PostOfficeBoxNameTranslator : Translator<PostOfficeBoxName, VmAddressSimple>
//    {
//        public PostOfficeBoxNameTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
//        {
//        }
//
//        public override VmAddressSimple TranslateEntityToVm(PostOfficeBoxName entity)
//        {
//            return CreateEntityViewModelDefinition(entity)
//                .DisableAutoTranslation()
//                .AddNavigation(input => input.Name, output => output.PoBox)
//                .GetFinal();
//        }
//
//        public override PostOfficeBoxName TranslateVmToEntity(VmAddressSimple vModel)
//        {
//            return CreateViewModelEntityDefinition(vModel)
//                .DisableAutoTranslation()
//                .UseDataContextLocalizedUpdate(input => input.Id.IsAssigned(), input => output => output.AddressPostOfficeBoxId == input.Id, d => d.UseDataContextCreate(i => true))
//                .AddNavigation(input => input.PoBox, output => output.Name)
//                .AddRequestLanguage(output => output)
//                .GetFinal();
//        }
//    }

    [RegisterService(typeof(ITranslator<PostOfficeBoxName, string>), RegisterType.Transient)]
    internal class PostOfficeBoxNamesStringTranslator : Translator<PostOfficeBoxName, string>
    {
        public PostOfficeBoxNamesStringTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override string TranslateEntityToVm(PostOfficeBoxName entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => input.Name, output => output)
                .GetFinal();
        }

        public override PostOfficeBoxName TranslateVmToEntity(string vModel)
        {
            throw new NotImplementedException();
        }
    }
}
