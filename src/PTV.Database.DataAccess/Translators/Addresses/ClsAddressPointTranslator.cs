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
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<ClsAddressPoint, VmAddressPoint>), RegisterType.Transient)]
    internal class ClsAddressPointTranslator : Translator<ClsAddressPoint, VmAddressPoint>
    {
        public ClsAddressPointTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) 
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmAddressPoint TranslateEntityToVm(ClsAddressPoint entity)
        {
            var translation = CreateEntityViewModelDefinition<VmAddressPoint>(entity)
                .AddSimple(i => i.IsValid, o => o.IsValid)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.Municipality, o => o.Municipality)
                .AddNavigation(i => i.AddressStreet, o => o.Street)
                .AddNavigation(i => i.AddressStreetNumber, o => o.AddressStreetNumber)
                .AddNavigation(i => i.StreetNumber, o => o.StreetNumber)
                .AddNavigation(i => i.PostalCode, o => o.PostalCode);

            return translation.GetFinal();
        }

        public override ClsAddressPoint TranslateVmToEntity(VmAddressPoint vModel)
        {
            var translation = CreateViewModelEntityDefinition<ClsAddressPoint>(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.Id)
                .AddSimple(i => i.IsValid, o => o.IsValid)
                .AddSimple(i => i.Municipality?.Id ?? Guid.Empty, o => o.MunicipalityId)
                //.AddSimple(i => i.Street?.Id ?? Guid.Empty, o => o.AddressStreetId)
                //.AddSimple(i => i.AddressStreetNumber?.Id ?? Guid.Empty, o => o.AddressStreetNumberId)
                .AddNavigation(i => i.StreetNumber, o => o.StreetNumber)
                .AddNavigation(i => i.Street, o => o.AddressStreet)
                .AddSimple(i => i.PostalCode?.Id ?? Guid.Empty, o => o.PostalCodeId);

            return translation.GetFinal();
        }
    }
}