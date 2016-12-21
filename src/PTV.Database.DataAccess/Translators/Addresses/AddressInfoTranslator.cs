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
using System.Linq;
using System.Threading.Tasks;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<Address, AddressInfo>), RegisterType.Transient)]
    internal class AddressInfoTranslator : Translator<Address, AddressInfo>
    {
        public AddressInfoTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override AddressInfo TranslateEntityToVm(Address entity)
        {
            return CreateEntityViewModelDefinition<AddressInfo>(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => i.StreetNames.FirstOrDefault(), o => o.Street)
                .AddNavigation(i => i.StreetNumber, o => o.StreetNumber)
                .AddNavigation(i => i.PostalCode?.Municipality?.Code, o => o.MunicipalityCode)
                .GetFinal();
        }

        public override Address TranslateVmToEntity(AddressInfo vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => true, i => o => i.Id == o.Id)
                .AddSimple(i => i.Latitude, o => o.Latitude)
                .AddSimple(i => i.Longtitude, o => o.Longtitude)
                .AddNavigation(i => i.StreetNumber, o => o.StreetNumber)
                .AddLocalizable(i => new VmAddressSimple() {Street = i.Street, Id = i.Id}, o => o.StreetNames)
                .AddNavigation(i => i.State.ToString(), o => o.CoordinateState)
                .GetFinal();
        }
    }
}
