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
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<AddressPostOfficeBox, VmAddressSimple>), RegisterType.Transient)]
    internal class AddressPostBoxOfficeTranslator : Translator<AddressPostOfficeBox, VmAddressSimple>
    {
        public AddressPostBoxOfficeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives)
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmAddressSimple TranslateEntityToVm(AddressPostOfficeBox entity)
        {
            return CreateEntityViewModelDefinition(entity)
                //.AddPartial(i => i, o => o as VmAddressSimpleBase)
                .AddSimple(input => input.MunicipalityId, output => output.Municipality)
                .AddNavigation(input => input.PostalCode != null && input.PostalCode.Code != "Undefined" ? input.PostalCode : null, output => output.PostalCode)
                //.AddLocalizable(input => input.PostOfficeBoxNames, output => output.PoBox)
                .AddDictionary(
                    i => i.PostOfficeBoxNames,
                    o => o.PoBox,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .GetFinal();
        }

        public override AddressPostOfficeBox TranslateVmToEntity(VmAddressSimple vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned())
                .UseDataContextUpdate(i => i.Id.IsAssigned(), i => o => i.Id == o.AddressId, def => def.UseDataContextCreate(i => true, i => i.AddressId, i => i.Id))
                .AddSimple(i => i.PostalCode?.Id, o => o.PostalCodeId)
                .AddCollection(i => i.PoBox?.Select(pair => new VmLocalizedPostOfficeBox
                {
                    PostOfficeBox = pair.Value,
                    LocalizationId = languageCache.Get(pair.Key),
                    OwnerReferenceId = i.Id
                }), o => o.PostOfficeBoxNames, true)
                .AddSimple(i => i.Municipality, o => o.MunicipalityId)
                .GetFinal();
        }
    }
}
