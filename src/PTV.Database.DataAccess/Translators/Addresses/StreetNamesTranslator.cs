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
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<StreetName, VmAddressSimple>), RegisterType.Transient)]
    internal class StreetNamesTranslator : Translator<StreetName, VmAddressSimple>
    {
        public StreetNamesTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmAddressSimple TranslateEntityToVm(StreetName entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .DisableAutoTranslation()
                .AddNavigation(input => input.Text, output => output.Street)
                .GetFinal();
        }

        public override StreetName TranslateVmToEntity(VmAddressSimple vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(input => !input.Id.IsAssigned(), output => output.Id, input => Guid.NewGuid())
                .UseDataContextLocalizedUpdate(input => input.Id.IsAssigned(), input => output => output.AddressId == input.Id, d => d.UseDataContextCreate(i => true, output => output.Id, input => Guid.NewGuid()))
                .AddNavigation(input => input.Street, output => output.Text)
                .AddRequestLanguage(output => output)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<StreetName, string>), RegisterType.Transient)]
    internal class StreetNamesStringTranslator : Translator<StreetName, string>
    {
        public StreetNamesStringTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override string TranslateEntityToVm(StreetName entity)
        {
            return CreateEntityViewModelDefinition(entity)
                .AddNavigation(input => input.Text, output => output)
                .GetFinal();
        }

        public override StreetName TranslateVmToEntity(string vModel)
        {
            throw new NotImplementedException();
        }
    }

}
