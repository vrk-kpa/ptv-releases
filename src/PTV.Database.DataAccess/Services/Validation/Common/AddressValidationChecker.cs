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
* THE SOFTWARE.C:\Projects\PTV_TEST\src\PTV.Database.DataAccess\Services\Security\
*/
using System;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Services.Validation.Common
{
    [RegisterService(typeof(IValidationChecker<Address>), RegisterType.Transient)]
    internal class AddressValidationChecker : BaseValidationChecker<Address>
    {
        private readonly ITypesCache typesCache;

        public AddressValidationChecker(ICacheManager cacheManager, IResolveManager resolveManager) : base(cacheManager, resolveManager)
        {
            typesCache = cacheManager.TypesCache;
        }

        protected Address FetchEntity(Guid id, IUnitOfWork unitOfWork)
        {
            return GetEntity<Address>(id, unitOfWork, x => x);
        }
        protected override void ValidateEntityInternal(Guid? language)
        {
            if (!language.IsAssigned())
            {
                throw new ArgumentNullException(nameof(language), "language must be defined.");
            }

            var addressType = typesCache.GetByValue<AddressType>(entity.TypeId);
            var addressTypeEnum = Enum.Parse(typeof(AddressTypeEnum), addressType);

            switch (addressTypeEnum)
            {
                case AddressTypeEnum.Street:
                    NotBeTrue("StreetsAddressPostalCodeId", x => x.ClsAddressPoints.Any(y => !y.PostalCodeId.IsAssigned()));
                    NotBeTrue("StreetAddressStreetId", x => x.ClsAddressPoints.Any(y => !y.AddressStreetId.IsAssigned()));
                    NotBeTrue("StreetAddressMunicipalityId", x => x.ClsAddressPoints.Any(y => !y.MunicipalityId.IsAssigned()));
                    break;

                case AddressTypeEnum.PostOfficeBox:
                    NotBeTrue("PostOfficeBoxAddressName", x => x.AddressPostOfficeBoxes.Any(z => string.IsNullOrEmpty(z.PostOfficeBoxNames.Where(sn => sn.LocalizationId == language.Value).Select(n => n.Name).FirstOrDefault())));
                    NotBeTrue("PostOfficeBoxAddressPostalCodeId", x => x.AddressPostOfficeBoxes.Any(y => !y.PostalCodeId.IsAssigned()));
                    break;

                case AddressTypeEnum.Foreign:
                    NotBeTrue("ForeignAddressTextName", x => x.AddressForeigns.Any(z => string.IsNullOrEmpty(z.ForeignTextNames.Where(sn => sn.LocalizationId == language.Value).Select(n => n.Name).FirstOrDefault())));
                    break;
                case AddressTypeEnum.Other:
                    NotBeTrue("OtherAddressAdditionalInformation", x => string.IsNullOrEmpty(x.AddressAdditionalInformations.Where(sn => sn.LocalizationId == language.Value).Select(n => n.Text).FirstOrDefault()));
                    NotBeTrue("OtherAddressCoordinates", x => !x.Coordinates.Any());
                    break;
                default:
                    break;
            }
        }
    }
}
