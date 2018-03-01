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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.V7;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;

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
                .AddNavigationOneMany(i => i, o => o.AddressStreets);                

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

    [RegisterService(typeof(ITranslator<AddressStreet, VmOpenApiAddressStreetWithCoordinatesIn>), RegisterType.Transient)]
    internal class OpenApiStreetInTranslator : Translator<AddressStreet, VmOpenApiAddressStreetWithCoordinatesIn>
    {
        public OpenApiStreetInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmOpenApiAddressStreetWithCoordinatesIn TranslateEntityToVm(AddressStreet entity)
        {
            throw new NotImplementedException();
        }

        public override AddressStreet TranslateVmToEntity(VmOpenApiAddressStreetWithCoordinatesIn vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.OwnerReferenceId.IsAssigned())
                .UseDataContextUpdate(i => i.OwnerReferenceId.IsAssigned(), i => o => i.OwnerReferenceId == o.AddressId, def => def.UseDataContextCreate(i => true))
                .AddCollection(i => i.Street, o => o.StreetNames)
                .AddNavigation(i => i.StreetNumber, o => o.StreetNumber)
                .AddNavigation(i => i.PostalCode, o => o.PostalCode)
                .AddNavigation(i => i.Municipality, o => o.Municipality)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<AddressForeign, V7VmOpenApiAddressWithForeignIn>), RegisterType.Transient)]
    internal class OpenApiAddressForeignTranslator : Translator<AddressForeign, V7VmOpenApiAddressWithForeignIn>
    {
        public OpenApiAddressForeignTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override V7VmOpenApiAddressWithForeignIn TranslateEntityToVm(AddressForeign entity)
        {
            throw new NotImplementedException();
        }

        public override AddressForeign TranslateVmToEntity(V7VmOpenApiAddressWithForeignIn vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.OwnerReferenceId.IsAssigned())
                .UseDataContextUpdate(i => i.OwnerReferenceId.IsAssigned(), i => o => i.OwnerReferenceId == o.AddressId, def => def.UseDataContextCreate(i => true))
                .AddCollection(i => i.ForeignAddress, o => o.ForeignTextNames, false)
                .GetFinal();
        }
    }

    [RegisterService(typeof(ITranslator<Address, V7VmOpenApiAddressWithForeignIn>), RegisterType.Transient)]
    internal class OpenApiAddressInTranslator : Translator<Address, V7VmOpenApiAddressWithForeignIn>
    {
        private readonly ITypesCache typesCache;

        public OpenApiAddressInTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager, translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
        }

        public override V7VmOpenApiAddressWithForeignIn TranslateEntityToVm(Address entity)
        {
            throw new NotImplementedException("No translation implemented in OpenApiAddressWithTypeTranslator");
        }
        public override Address TranslateVmToEntity(V7VmOpenApiAddressWithForeignIn vModel)
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
                .AddSimple(i => i.OrderNumber, o => o.OrderNumber)
                .AddSimple(i => typesCache.Get<AddressType>(i.SubType), o => o.TypeId);

            switch (vModel.SubType.Parse<AddressTypeEnum>())
            {
                case AddressTypeEnum.PostOfficeBox:
                    vModel.PostOfficeBoxAddress.Id = vModel.Id;
                    definition.AddPartial(i => i.PostOfficeBoxAddress);
                    break;
                case AddressTypeEnum.Street:
                case AddressTypeEnum.Moving:// Moving addresses are saved into AddressStreet
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
}
