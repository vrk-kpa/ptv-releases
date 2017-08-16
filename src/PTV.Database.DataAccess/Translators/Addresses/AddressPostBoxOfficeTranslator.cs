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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<AddressPostOfficeBox, VmAddressSimple>), RegisterType.Transient)]
    internal class AddressPostBoxOfficeTranslator : Translator<AddressPostOfficeBox, VmAddressSimple>
    {
        private readonly ITypesCache typesCache;

        public AddressPostBoxOfficeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager,
            translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmAddressSimple TranslateEntityToVm(AddressPostOfficeBox entity)
        {
            return CreateEntityViewModelDefinition(entity)
                //.AddPartial(i => i, o => o as VmAddressSimpleBase)
                .AddSimple(input => input.MunicipalityId, output => output.MunicipalityId)
                .AddNavigation(input => input.PostalCode != null && input.PostalCode.Code != "Undefined" ? input.PostalCode : null, output => output.PostalCode)
                .AddLocalizable(input => input.PostOfficeBoxNames, output => output.PoBox)
                .GetFinal();
        }

        public override AddressPostOfficeBox TranslateVmToEntity(VmAddressSimple vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !CoreExtensions.IsAssigned((Guid?) i.Id))
                .UseDataContextUpdate(i => CoreExtensions.IsAssigned((Guid?) i.Id), i => o => i.Id == o.AddressId, def => def.UseDataContextCreate(i => true, i => i.AddressId, i => i.Id))
                .AddNavigation(i => i.PostalCode, o => o.PostalCode)
                .AddLocalizable(i => i, o => o.PostOfficeBoxNames)
                .AddSimple(i => i.MunicipalityId, o => o.MunicipalityId)
                .GetFinal();
        }
    }
}