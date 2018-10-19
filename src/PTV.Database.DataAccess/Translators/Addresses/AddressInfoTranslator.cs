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
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<Address, AddressInfo>), RegisterType.Transient)]
    internal class AddressInfoTranslator : Translator<Address, AddressInfo>
    {
        private readonly ITypesCache typesCache;

        public AddressInfoTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public IName GetAddressTextName(Address entity)
        {
            if (entity.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString()))
            {
                return entity.AddressStreets.FirstOrDefault()?.StreetNames.OrderBy(x => x.Localization.OrderNumber).FirstOrDefault();
            }
            if (entity.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.PostOfficeBox.ToString()))
            {
                return entity.AddressPostOfficeBoxes.FirstOrDefault()?.PostOfficeBoxNames.OrderBy(x => x.Localization.OrderNumber).FirstOrDefault();
            }
            if (entity.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.Foreign.ToString()))
            {
                return entity.AddressForeigns.FirstOrDefault()?.ForeignTextNames.OrderBy(x => x.Localization.OrderNumber).FirstOrDefault();
            }
            return null;
        }


        public string GetAddressStreetNumber(Address entity)
        {
            if (entity.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString()))
            {
                return entity.AddressStreets.FirstOrDefault()?.StreetNumber;
            }
            return null;
        }

        public PostalCode GetAddressPostalCode(Address entity)
        {
            if (entity.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString()))
            {
                return entity.AddressStreets.FirstOrDefault()?.PostalCode;
            }
            if (entity.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.PostOfficeBox.ToString()))
            {
                return entity.AddressPostOfficeBoxes.FirstOrDefault()?.PostalCode;
            }
            return null;
        }

        public Municipality GetAddressMunicipality(Address entity)
        {
            if (entity.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString()))
            {
                return entity.AddressStreets.FirstOrDefault()?.Municipality;
            }
            if (entity.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.PostOfficeBox.ToString()))
            {
                return entity.AddressPostOfficeBoxes.FirstOrDefault()?.Municipality;
            }
            return null;
        }


        public override AddressInfo TranslateEntityToVm(Address entity)
        {
            var definition = CreateEntityViewModelDefinition<AddressInfo>(entity)
                .AddSimple(i => i.Id, o => o.Id)
                .AddNavigation(i => GetAddressTextName(i)?.Name ?? string.Empty, o => o.Street)
                .AddNavigation(i => GetAddressStreetNumber(i), o => o.StreetNumber);

            var postalCode = GetAddressPostalCode(entity);
            if (postalCode?.Municipality != null)
            {
                definition.AddNavigation(i => postalCode.Municipality.Code, o => o.MunicipalityCode);
            }
            else
            {
                definition.AddNavigation(i => GetAddressMunicipality(i)?.Code, o => o.MunicipalityCode);
            }
            return definition.GetFinal();
        }

        public override Address TranslateVmToEntity(AddressInfo vModel)
        {

            var definition = CreateViewModelEntityDefinition(vModel)
                .DisableAutoTranslation()
                .UseDataContextUpdate(i => true, i => o => i.Id == o.Id);

            if (!string.IsNullOrEmpty(vModel.StreetNumber))
            {
                //definition.AddNavigation(i => i.StreetNumber, o => o.StreetNumber);
            }

            if (!string.IsNullOrEmpty(vModel.Street))
            {
                //definition.AddLocalizable(i => new VmAddressSimple() { Street = i.Street, Id = i.Id }, o => o.StreetNames);
            }

            var vmCoordinate = CreateCoordinateModel(vModel);
            if (vmCoordinate != null)
            {
                definition.AddCollection(i => new List<VmCoordinate> { vmCoordinate }, o => o.Coordinates);
            }

            return definition.GetFinal();
        }

        private VmCoordinate CreateCoordinateModel(AddressInfo address)
        {
            if (address == null) return null;
            return new VmCoordinate
            {
                OwnerReferenceId = address.Id,
                Latitude = address.Latitude ?? 0,
                Longitude = address.Longitude ?? 0,
                CoordinateState = address.State.ToString(),
                TypeId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.Main.ToString())
            };
        }
    }
}
