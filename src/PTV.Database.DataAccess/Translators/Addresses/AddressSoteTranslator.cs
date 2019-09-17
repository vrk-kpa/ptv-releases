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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<Address, VmJsonSoteAddress>), RegisterType.Transient)]
    internal class AddressSoteTranslator : Translator<Address, VmJsonSoteAddress>
    {
        private readonly ITypesCache typesCache;

        public AddressSoteTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
         typesCache = cacheManager.TypesCache;
        }

        public override VmJsonSoteAddress TranslateEntityToVm(Address entity)
        {
            throw new NotImplementedException("Translator Address -> VmJsonSoteAddress is not implemented");
        }

        public override Address TranslateVmToEntity(VmJsonSoteAddress vModel)
        {
            if (vModel == null) return null;
            
            var exists = vModel.Id.IsAssigned();
            
// SOTE has been disabled (SFIPTV-1177)            
//            var extraTypeId = typesCache.Get<ExtraType>(ExtraTypeEnum.Sote.ToString());

            var translation = CreateViewModelEntityDefinition<Address>(vModel)
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => i.Id == o.Id);

            translation.AddPartial(i => new VmAddressSimple
            {
                Id = i.Id,
                StreetType = "Street",
                AddressCharacter = AddressCharacterEnum.Visiting,
                StreetNumber = i.StreetAddress?.StreetNumber,
                Street = new VmStreet { Id = i.StreetAddress?.StreetId ?? Guid.Empty },
                StreetNumberRange = new VmStreetNumber { Id = i.StreetAddress?.StreetRangeId ?? Guid.Empty },
                PostalCode = new VmPostalCode {Id = i.PostalCodeId ?? Guid.Empty},
                Municipality = i.MunicipalityId
            }, o => o);

/* SOTE has been disabled (SFIPTV-1177)            
            var extraTypes = new List<VmExtraType> {new VmExtraType {Id = extraTypeId}};
            translation.Propagation((i, o) => {extraTypes.ForEach(et => et.OwnerReferenceId = o.Id); });
            translation.AddCollectionWithRemove(
                    i => extraTypes, 
                    o => o.ExtraTypes, 
                    x => true);
*/           
            return translation.GetFinal();
        }
    }
}