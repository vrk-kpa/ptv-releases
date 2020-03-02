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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Domain.Model.Models.OpenApi.V9;
using PTV.Domain.Model.Models.OpenApi.V8;

namespace PTV.Database.DataAccess.Translators.OpenApi.Common
{
    [RegisterService(typeof(ITranslator<Address, VmOpenApiAddressPostOfficeBoxIn>), RegisterType.Transient)]
    internal class OpenApiAddressPostOfficeBoxTranslator : Translator<Address, VmOpenApiAddressPostOfficeBoxIn>
    {
        public OpenApiAddressPostOfficeBoxTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiAddressPostOfficeBoxIn TranslateEntityToVm(Address entity)
        {
            throw new NotImplementedException();
        }

        public override Address TranslateVmToEntity(VmOpenApiAddressPostOfficeBoxIn vModel)
        {
            var exists = vModel.Id.IsAssigned();

            if (exists)
            {
                vModel.PostOfficeBox.ForEach(s => s.OwnerReferenceId = vModel.Id);
                vModel.AdditionalInformation.ForEach(a => a.OwnerReferenceId = vModel.Id);
            }
            var definition = CreateViewModelEntityDefinition(vModel)
                .AddNavigationOneMany(i => i, o => o.AddressPostOfficeBoxes);

            if (vModel.AdditionalInformation?.Count > 0)
            {
                definition.AddCollection(i => i.AdditionalInformation, o => o.AddressAdditionalInformations);
            }

            return definition.GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<AddressPostOfficeBox, VmOpenApiAddressPostOfficeBoxIn>), RegisterType.Transient)]
    internal class OpenApiPostOfficeBoxInTranslator : Translator<AddressPostOfficeBox, VmOpenApiAddressPostOfficeBoxIn>
    {
        public OpenApiPostOfficeBoxInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiAddressPostOfficeBoxIn TranslateEntityToVm(AddressPostOfficeBox entity)
        {
            throw new NotImplementedException();
        }

        public override AddressPostOfficeBox TranslateVmToEntity(VmOpenApiAddressPostOfficeBoxIn vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.OwnerReferenceId.IsAssigned())
                .UseDataContextUpdate(i => i.OwnerReferenceId.IsAssigned(), i => o => i.OwnerReferenceId == o.AddressId, def => def.UseDataContextCreate(i => true))
                .AddCollection(i => i.PostOfficeBox, o => o.PostOfficeBoxNames)
                .AddNavigation(i => i.PostalCode, o => o.PostalCode)
                .AddNavigation(i => i.Municipality, o => o.Municipality)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<Address, VmOpenApiAddressOtherIn>), RegisterType.Transient)]
    internal class OpenApiAddressOtherInTranslator : Translator<Address, VmOpenApiAddressOtherIn>
    {
        private readonly ITypesCache typesCache;

        public OpenApiAddressOtherInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiAddressOtherIn TranslateEntityToVm(Address entity)
        {
            throw new NotImplementedException();
        }

        public override Address TranslateVmToEntity(VmOpenApiAddressOtherIn vModel)
        {
            if (vModel == null) return null;

            var exists = vModel.Id.IsAssigned();

            if (exists)
            {
                vModel.AdditionalInformation.ForEach(a => a.OwnerReferenceId = vModel.Id);
            }
            var definition = CreateViewModelEntityDefinition(vModel)
                .AddNavigationOneMany(i => i, o => o.AddressOthers);

            if (!string.IsNullOrEmpty(vModel.Latitude) && !string.IsNullOrEmpty(vModel.Longitude))
            {
                double latitude, longitude;

                if (double.TryParse(vModel.Latitude, NumberStyles.Number, CultureInfo.InvariantCulture, out latitude) &&
                    double.TryParse(vModel.Longitude, NumberStyles.Number, CultureInfo.InvariantCulture, out longitude))
                {
                    definition.AddCollection(
                    i => new List<VmCoordinate>
                    {
                        new VmCoordinate
                        {
                            OwnerReferenceId = definition.GetFinal().Id,
                            TypeId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.User.ToString()),
                            CoordinateState = CoordinateStates.EnteredByUser.ToString(),
                            Latitude = latitude,
                            Longitude = longitude
                        }
                    }, o => o.Coordinates);
                }
            }

            if (vModel.AdditionalInformation?.Count > 0)
            {
                definition.AddCollection(i => i.AdditionalInformation, o => o.AddressAdditionalInformations);
            }

            return definition.GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<AddressOther, VmOpenApiAddressOtherIn>), RegisterType.Transient)]
    internal class OpenApiOtherInTranslator : Translator<AddressOther, VmOpenApiAddressOtherIn>
    {
        public OpenApiOtherInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiAddressOtherIn TranslateEntityToVm(AddressOther entity)
        {
            throw new NotImplementedException();
        }

        public override AddressOther TranslateVmToEntity(VmOpenApiAddressOtherIn vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.OwnerReferenceId.IsAssigned())
                .UseDataContextUpdate(i => i.OwnerReferenceId.IsAssigned(), i => o => i.OwnerReferenceId == o.AddressId, def => def.UseDataContextCreate(i => true))
                .AddNavigation(i => i.PostalCode, o => o.PostalCode)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<Address, VmOpenApiAddressStreetWithCoordinatesIn>), RegisterType.Transient)]
    internal class OpenApiAddressStreetTranslator : Translator<Address, VmOpenApiAddressStreetWithCoordinatesIn>
    {
        private readonly ITypesCache typesCache;

        public OpenApiAddressStreetTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager)
            : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override VmOpenApiAddressStreetWithCoordinatesIn TranslateEntityToVm(Address entity)
        {
            throw new NotImplementedException();
        }

        public override Address TranslateVmToEntity(VmOpenApiAddressStreetWithCoordinatesIn vModel)
        {
            var exists = vModel.Id.IsAssigned();

            if (exists)
            {
                vModel.Street.ForEach(s => s.OwnerReferenceId = vModel.Id);
                vModel.AdditionalInformation.ForEach(a => a.OwnerReferenceId = vModel.Id);
            }

            var definition = CreateViewModelEntityDefinition(vModel)
                .AddNavigationOneMany(i => i, o => o.ClsAddressPoints);

            if (!string.IsNullOrEmpty(vModel.Latitude) && !string.IsNullOrEmpty(vModel.Longitude))
            {
                double latitude, longitude;

                if (double.TryParse(vModel.Latitude, NumberStyles.Number, CultureInfo.InvariantCulture, out latitude) &&
                    double.TryParse(vModel.Longitude, NumberStyles.Number, CultureInfo.InvariantCulture, out longitude))
                {
                    definition.AddCollection(
                    i => new List<VmCoordinate>
                    {
                        new VmCoordinate
                        {
                            OwnerReferenceId = definition.GetFinal().Id,
                            TypeId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.User.ToString()),
                            CoordinateState = CoordinateStates.EnteredByUser.ToString(),
                            Latitude = latitude,
                            Longitude = longitude
                        }
                    }, o => o.Coordinates);
                }
            }

            if (vModel.AdditionalInformation?.Count > 0)
            {
                definition.AddCollection(i => i.AdditionalInformation, o => o.AddressAdditionalInformations);
            }

            return definition.GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<ClsAddressPoint, VmOpenApiAddressStreetWithCoordinatesIn>), RegisterType.Transient)]
    internal class OpenApiStreetInTranslator : Translator<ClsAddressPoint, VmOpenApiAddressStreetWithCoordinatesIn>
    {
//        private readonly ILanguageCache languageCache;
//        private readonly ILanguageOrderCache languageOrderCache;

        public OpenApiStreetInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives/*, ICacheManager cacheManager*/) : base(resolveManager, translationPrimitives)
        {
//            this.languageCache = cacheManager.LanguageCache;
//            this.languageOrderCache = cacheManager.LanguageOrderCache;
        }

        public override VmOpenApiAddressStreetWithCoordinatesIn TranslateEntityToVm(ClsAddressPoint entity)
        {
            throw new NotImplementedException();
        }

        public override ClsAddressPoint TranslateVmToEntity(VmOpenApiAddressStreetWithCoordinatesIn vModel)
        {
            return CreateViewModelEntityDefinition<ClsAddressPoint>(vModel)
                .UseDataContextUpdate(
                    model => model.OwnerReferenceId.IsAssigned(),
                    model => entity => model.OwnerReferenceId == entity.AddressId,
                    def => def.UseDataContextCreate(i => true))
                .AddNavigation(i => i, o => o.AddressStreet)
                .Propagation((i, o) => i.ReferencedStreetId = o.AddressStreetId)
                .AddNavigation(i => i, o => o.AddressStreetNumber)
                .AddNavigation(i => i.StreetNumber?.Trim(), o => o.StreetNumber)
                .AddNavigation(i => i.PostalCode, o => o.PostalCode)
                .AddNavigation(i => i.Municipality, o => o.Municipality)
                .Propagation((i, o) =>
                {
                    if (o.AddressStreetNumber != null)
                    {
                        o.AddressStreetNumber.ClsAddressStreetId = o.AddressStreet?.Id ?? o.AddressStreetId;
                        o.AddressStreetNumber.PostalCodeId = o.PostalCode?.Id ?? o.PostalCodeId;
                    }
                })
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<AddressForeign, V9VmOpenApiAddressIn>), RegisterType.Transient)]
    internal class OpenApiAddressForeignTranslator : Translator<AddressForeign, V9VmOpenApiAddressIn>
    {
        public OpenApiAddressForeignTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override V9VmOpenApiAddressIn TranslateEntityToVm(AddressForeign entity)
        {
            throw new NotImplementedException();
        }

        public override AddressForeign TranslateVmToEntity(V9VmOpenApiAddressIn vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.OwnerReferenceId.IsAssigned())
                .UseDataContextUpdate(i => i.OwnerReferenceId.IsAssigned(), i => o => i.OwnerReferenceId == o.AddressId, def => def.UseDataContextCreate(i => true))
                .AddCollection(i => i.ForeignAddress, o => o.ForeignTextNames, false)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<Address, V9VmOpenApiAddressIn>), RegisterType.Transient)]
    internal class OpenApiAddressInTranslator : Translator<Address, V9VmOpenApiAddressIn>
    {
        private readonly ITypesCache typesCache;

        public OpenApiAddressInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V9VmOpenApiAddressIn TranslateEntityToVm(Address entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiAddressInTranslator");
        }
        public override Address TranslateVmToEntity(V9VmOpenApiAddressIn vModel)
        {
            if (vModel == null) return null;

            var exists = vModel.Id.IsAssigned();

            if (exists)
            {
                vModel.ForeignAddress.ForEach(a => a.OwnerReferenceId = vModel.Id);
            }

            var definition = CreateViewModelEntityDefinition<Address>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => (vModel.Id.Value == o.Id))
                .Propagation((i,o) => i.OwnerReferenceId = o.Id)
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
                .AddSimple(i => typesCache.Get<AddressType>(i.SubType), o => o.TypeId)
                .AddSimple(i => i.UniqueId.IsAssigned() ? i.UniqueId.Value : Guid.NewGuid(), output => output.UniqueId);

            switch (vModel.SubType.Parse<AddressTypeEnum>())
            {
                case AddressTypeEnum.PostOfficeBox:
                    vModel.PostOfficeBoxAddress.Id = vModel.Id;
                    definition.AddPartial(i => i.PostOfficeBoxAddress);
                    break;
                case AddressTypeEnum.Street:
                    vModel.StreetAddress.Id = vModel.Id;
                    definition.AddPartial(i => i.StreetAddress);
                    break;
                case AddressTypeEnum.Foreign:
                    definition.AddNavigationOneMany(i => i, o => o.AddressForeigns);
                    break;
                case AddressTypeEnum.NoAddress:
                    // Delivery address descriptions are saved into AddressAdditionalInformation table
                    definition.AddCollection(i => i.PostOfficeBoxAddress.AdditionalInformation, o => o.AddressAdditionalInformations);
                    break;
                case AddressTypeEnum.Other:
                    // Other address information is saved into new AddressOther table (PTV-4444)
                    definition.AddPartial(i => i.OtherAddress);
                    break;
                default:
                    break;
            }

            if (!exists && string.IsNullOrEmpty(vModel.Country))
            {
                definition.AddNavigation(i => CountryCode.FI.ToString(), o => o.Country);
            }
            else if (!string.IsNullOrEmpty(vModel.Country))
            {
                definition.AddNavigation(i => i.Country, o => o.Country);
            }

            return definition.GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<Address, V8VmOpenApiAddressDeliveryIn>), RegisterType.Transient)]
    internal class V8OpenApiAddressInTranslator : Translator<Address, V8VmOpenApiAddressDeliveryIn>
    {
        public V8OpenApiAddressInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        { }

        public override V8VmOpenApiAddressDeliveryIn TranslateEntityToVm(Address entity)
        {
            throw new NotImplementedException("No translation implemented in V8VmOpenApiAddressWithForeignIn");
        }
        public override Address TranslateVmToEntity(V8VmOpenApiAddressDeliveryIn vModel)
        {
            if (vModel == null) return null;
            var exists = vModel.Id.IsAssigned();
            Guid? addressId = null;
            var definition = CreateViewModelEntityDefinition<Address>(vModel)
                .DisableAutoTranslation()
                .UseDataContextCreate(i => !exists, o => o.Id, i => Guid.NewGuid())
                .UseDataContextUpdate(i => exists, i => o => (vModel.Id.Value == o.Id))
                .Propagation((i,o) => addressId = o.Id)
                .AddPartial(i => new V9VmOpenApiAddressIn
                {
                    OrderNumber = i.OrderNumber,
                    Type = AddressCharacterEnum.Delivery.ToString(),
                    SubType = i.SubType,
                    StreetAddress = i.StreetAddress == null
                                ? null
                                : new VmOpenApiAddressStreetWithCoordinatesIn
                                {
                                    Street = i.StreetAddress.Street,
                                    StreetNumber = i.StreetAddress.StreetNumber,
                                    PostalCode = i.StreetAddress.PostalCode,
                                    Municipality = i.StreetAddress.Municipality,
                                    AdditionalInformation = i.StreetAddress.AdditionalInformation,
                                    OwnerReferenceId = addressId
                                },
                    PostOfficeBoxAddress = i.SubType == AddressTypeEnum.NoAddress.ToString()
                                 ? new VmOpenApiAddressPostOfficeBoxIn
                                 {
                                     AdditionalInformation = i.DeliveryAddressInText
                                 }
                                 : i.PostOfficeBoxAddress
                }, o => o);

            if (!vModel.FormReceiver.IsNullOrEmpty())
            {
                vModel.FormReceiver.ForEach(r => r.OwnerReferenceId = vModel.Id);
                definition.AddCollection(i => i.FormReceiver, o => o.Receivers);
            }

            return definition.GetFinal();
        }
    }
}
