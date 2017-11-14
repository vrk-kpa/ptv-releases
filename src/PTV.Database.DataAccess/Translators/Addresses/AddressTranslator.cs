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
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Razor.TagHelpers;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.V2.Common;

namespace PTV.Database.DataAccess.Translators.Addresses
{
    [RegisterService(typeof(ITranslator<Address, VmAddressSimple>), RegisterType.Transient)]
    internal class AddressTranslator : Translator<Address, VmAddressSimple>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        public AddressTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager,
            translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        private IList<Coordinate> GetCoordinates(ICollection<Coordinate> coordinates)
        {
            var mainCoordinateType = typesCache.Get<CoordinateType>(CoordinateTypeEnum.Main.ToString());
            var mainCoordinates = coordinates.Where(x => x.TypeId == mainCoordinateType).ToList();
            var mainCoordinate = mainCoordinates.Count > 1
                ? mainCoordinates.FirstOrDefault(x => x.CoordinateState == CoordinateStates.Ok.ToString()) ??
                  mainCoordinates.FirstOrDefault()
                : mainCoordinates.FirstOrDefault();
            var filteredCoordinates = coordinates.Except(mainCoordinates).ToList();
            if (mainCoordinate != null)
            {
                filteredCoordinates.Add(mainCoordinate);
            }

            return filteredCoordinates;
        }

        public override VmAddressSimple TranslateEntityToVm(Address entity)
        {
            AddressTypeEnum type = GetAddressType(entity);
            var definition = CreateEntityViewModelDefinition<VmAddressSimple>(entity)
                .AddPartial(i => i, o => o as VmAddressSimpleBase)
                .AddSimple(input => input.Id, output => output.Id)
                .AddNavigation(input => input.Country != null ? input.Country : null, output => output.Country)
                .AddCollection(input => GetCoordinates(input.Coordinates), output => output.Coordinates)
                .AddNavigation(input => type.ToString(), output => output.StreetType);

            switch (type)
            {
                case AddressTypeEnum.PostOfficeBox:
                    definition.AddDictionary(i => i.AddressAdditionalInformations,o => o.AdditionalInformation, k => languageCache.GetByValue(k.LocalizationId));
                    definition.AddPartial(i => i.AddressPostOfficeBoxes.FirstOrDefault(), o => o);
                    break;
                case AddressTypeEnum.Street:
                    definition.AddDictionary(i => i.AddressAdditionalInformations, o => o.AdditionalInformation, k => languageCache.GetByValue(k.LocalizationId));
                    definition.AddPartial(i => i.AddressStreets.FirstOrDefault(), o => o);
                    break;
                case AddressTypeEnum.Foreign:
                    definition.AddPartial(i => i.AddressForeigns.FirstOrDefault(), o => o);
                    break;
                case AddressTypeEnum.NoAddress:
                    definition.AddDictionary(i => i.AddressAdditionalInformations,o => o.NoAddressAdditionalInformation, k => languageCache.GetByValue(k.LocalizationId)
                    );
                    break;
                default:
                    break;
            }
            var model = definition.GetFinal();
            return model;
        }

        public override Address TranslateVmToEntity(VmAddressSimple vModel)
        {
            if (vModel == null) return null;

            bool exists = vModel.Id.IsAssigned();
            AddressTypeEnum type;
            if (!Enum.TryParse(vModel.StreetType, true, out type))
            {
                type = AddressTypeEnum.Street;
            }
            var translationDefinition = CreateViewModelEntityDefinition<Address>(vModel)
                .UseDataContextCreate(input => !exists, output => output.Id, input => Guid.NewGuid())
                .UseDataContextUpdate(input => exists, input => output => input.Id == output.Id)
                .AddSimple(input => input.OrderNumber, output => output.OrderNumber)
                .AddSimple(i => typesCache.Get<AddressType>(type.ToString()), o => o.TypeId)
                .AddSimple(input => input.OrderNumber, output => output.OrderNumber)
                .AddCollection(i => i.AdditionalInformation?.Select(pair => new VmLocalizedAdditionalInformation()
                {
                    Content = pair.Value,
                    LocalizationId = languageCache.Get(pair.Key),
                    OwnerReferenceId = i.Id
                }),
                o => o.AddressAdditionalInformations, true)
                .AddSimple(i => typesCache.Get<AddressType>(type.ToString()), o => o.TypeId);

            if (type == AddressTypeEnum.Foreign)
            {
                if (vModel.Country != null && vModel.Country.Id.IsAssigned())
                {
                    translationDefinition.AddSimple(input => input.Country.Id, output => output.CountryId);
                }
                else
                {
                    translationDefinition.AddSimple(input => (Guid?)null, output => output.CountryId);
                }
            }
            else
            {
                translationDefinition.AddNavigation(input => CountryCode.FI.ToString(), output => output.Country);
            }

            switch (type)
            {
                case AddressTypeEnum.PostOfficeBox:
                    if (vModel.PostalCode != null)
                    {
                        translationDefinition.AddNavigationOneMany(i => i, o => o.AddressPostOfficeBoxes);
                    }
                    else
                    {
                        translationDefinition.AddCollection(i => new List<AddressPostOfficeBox>(), o => o.AddressPostOfficeBoxes, false);
                    }
                    translationDefinition.AddCollection(i => new List<AddressStreet>(), o => o.AddressStreets, false);
                    translationDefinition.AddCollection(i => new List<AddressForeign>(), o => o.AddressForeigns, false);
                    break;
                case AddressTypeEnum.Street:
                    if (vModel.PostalCode != null)
                    {
                        translationDefinition.AddNavigationOneMany(i => i, o => o.AddressStreets);
                    }
                    else
                    {
                        translationDefinition.AddCollection(i => new List<AddressStreet>(), o => o.AddressStreets, false);
                    }
                    translationDefinition.AddCollection(i => new List<AddressPostOfficeBox>(), o => o.AddressPostOfficeBoxes, false);
                    translationDefinition.AddCollection(i => new List<AddressForeign>(), o => o.AddressForeigns, false);
                    break;
                case AddressTypeEnum.Foreign:
                    translationDefinition.AddNavigationOneMany(i => i, o => o.AddressForeigns);
                    translationDefinition.AddCollection(i => new List<AddressStreet>(), o => o.AddressStreets, false);
                    translationDefinition.AddCollection(i => new List<AddressPostOfficeBox>(), o => o.AddressPostOfficeBoxes, false);
                    vModel.Coordinates?.Clear();
                    break;
                case AddressTypeEnum.NoAddress:
                    translationDefinition.AddCollection(i => new List<AddressStreet>(), o => o.AddressStreets, false);
                    translationDefinition.AddCollection(i => new List<AddressForeign>(), o => o.AddressForeigns, false);
                    translationDefinition.AddCollection(i => new List<AddressPostOfficeBox>(), o => o.AddressPostOfficeBoxes, false);
                    vModel.Coordinates?.Clear();
                    translationDefinition.AddCollection(i => i.NoAddressAdditionalInformation?.Select(
                            pair => new VmLocalizedAdditionalInformation()
                            {
                                Content = pair.Value,
                                LocalizationId = languageCache.Get(pair.Key),
                                OwnerReferenceId = i.Id
                            }),
                        o => o.AddressAdditionalInformations, true);

                    break;
                default:
                    break;
            }

            var entity = translationDefinition.GetFinal();

            vModel.Coordinates?.ForEach(x => x.OwnerReferenceId = entity.Id);

            // TODO: apply the remove flag on coordinates
            // save main coordinate with "Loading" state
            translationDefinition.AddCollectionWithRemove(
                i => vModel.Coordinates ?? new List<VmCoordinate>
                {
                    new VmCoordinate
                    {
                        OwnerReferenceId = entity.Id,
                        TypeId = typesCache.Get<CoordinateType>(CoordinateTypeEnum.Main.ToString()),
                        CoordinateState = CoordinateStates.Loading.ToString(),
                        Latitude = 0,
                        Longitude = 0
                    }
                }, o => o.Coordinates, TranslationPolicy.FetchData, r => true);


            return entity;
        }

        private AddressTypeEnum GetAddressType(Address address)
        {
            if (address?.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString())) return AddressTypeEnum.Street;
            if (address?.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.PostOfficeBox.ToString())) return AddressTypeEnum.PostOfficeBox;
            if (address?.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.Foreign.ToString())) return AddressTypeEnum.Foreign;
            if (address?.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.NoAddress.ToString())) return AddressTypeEnum.NoAddress;
            return AddressTypeEnum.Foreign;
        }
    }


    // -------------------------------------- AddressStreet -------------------------------------- //
    [RegisterService(typeof(ITranslator<AddressStreet, VmAddressSimple>), RegisterType.Transient)]
    internal class AddressStreetTranslator : Translator<AddressStreet, VmAddressSimple>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        public AddressStreetTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager,
            translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmAddressSimple TranslateEntityToVm(AddressStreet entity)
        {
            return CreateEntityViewModelDefinition(entity)
                    //.AddPartial(i => i, o => o as VmAddressSimpleBase)
                    .AddSimple(input => input.MunicipalityId, output => output.Municipality)
                    .AddNavigation(input => input.PostalCode != null && input.PostalCode.Code != "Undefined" ? input.PostalCode : null, output => output.PostalCode)
                    .AddNavigation(input => input.StreetNumber, output => output.StreetNumber)
                    //.AddLocalizable(input => input.StreetNames, output => output.Street)
                    .AddDictionary(
                        i => i.StreetNames,
                        o => o.Street,
                        k => languageCache.GetByValue(k.LocalizationId)
                    )
                    .GetFinal();
        }

        public override AddressStreet TranslateVmToEntity(VmAddressSimple vModel)
    {
        return CreateViewModelEntityDefinition(vModel)
            .UseDataContextCreate(i => !i.Id.IsAssigned())
            .UseDataContextUpdate(
                i => i.Id.IsAssigned(),
                i => o => i.Id == o.AddressId,
                def => def.UseDataContextCreate(i => true, i => i.AddressId, i=> i.Id)
            )
            .AddNavigation(i => i.PostalCode, o => o.PostalCode)
            .AddCollection(i => i.Street?.Select(pair => new VmLocalizedStreetName()
            {
                Name = pair.Value,
                LocalizationId = languageCache.Get(pair.Key),
                OwnerReferenceId = i.Id
            }),
            o => o.StreetNames, true)
            .AddNavigation(i => i.StreetNumber, o => o.StreetNumber)
            .AddSimple(i => i.Municipality, o => o.MunicipalityId)
            .GetFinal();
    }
    }


    // -------------------------------------- AddressPostBoxOffice -------------------------------------- //
    [RegisterService(typeof(ITranslator<AddressPostOfficeBox, VmAddressSimple>), RegisterType.Transient)]
    internal class AddressPostBoxOfficeTranslator : Translator<AddressPostOfficeBox, VmAddressSimple>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        public AddressPostBoxOfficeTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager,
            translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
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
                .AddNavigation(i => i.PostalCode, o => o.PostalCode)
                .AddCollection(i => i.PoBox?.Select(pair => new VmLocalizedPostOfficeBox()
                 {
                     PostOfficeBox = pair.Value,
                     LocalizationId = languageCache.Get(pair.Key),
                     OwnerReferenceId = i.Id
                 }), o => o.PostOfficeBoxNames, true)              
                .AddSimple(i => i.Municipality, o => o.MunicipalityId)
                .GetFinal();
        }
    }


    // -------------------------------------- AddressForeignTranslator  -------------------------------------- //
    [RegisterService(typeof(ITranslator<AddressForeign, VmAddressSimple>), RegisterType.Transient)]
    internal class AddressForeignTranslator : Translator<AddressForeign, VmAddressSimple>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        public AddressForeignTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager,
            translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmAddressSimple TranslateEntityToVm(AddressForeign entity)
        {
            return CreateEntityViewModelDefinition(entity)
                //.AddLocalizable(input => input.ForeignTextNames, output => output.Street)
                .AddDictionary(
                    i => i.ForeignTextNames,
                    o => o.ForeignAddressText,
                    k => languageCache.GetByValue(k.LocalizationId)
                )
                .GetFinal();
        }

        public override AddressForeign TranslateVmToEntity(VmAddressSimple vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextUpdate(i=>CoreExtensions.IsAssigned((Guid?) i.Id), i=>o=>i.Id==o.AddressId, d=>d.UseDataContextCreate(c => true))
                //.AddLocalizable(i => i.ForeignAddress, o => o.ForeignTextNames)
                .AddCollection(i => i.ForeignAddressText?.Select(pair => new VmLocalizedForeignAddress()
                {
                    Text = pair.Value,
                    LocalizationId = languageCache.Get(pair.Key),
                    OwnerReferenceId = i.Id
                }),
                o => o.ForeignTextNames, true)
                .GetFinal();
        }
    }
    // AddressForeignTextName
    [RegisterService(typeof(ITranslator<AddressForeignTextName, VmLocalizedForeignAddress>), RegisterType.Transient)]
    internal class AddressForeignTextNameTranslator : Translator<AddressForeignTextName, VmLocalizedForeignAddress>
    {
        public AddressForeignTextNameTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) : base(resolveManager, translationPrimitives)
        {
        }

        public override VmLocalizedForeignAddress TranslateEntityToVm(AddressForeignTextName entity)
        {
            throw new NotImplementedException();
        }

        public override AddressForeignTextName TranslateVmToEntity(VmLocalizedForeignAddress vModel)
        {
            if (vModel.LocalizationId.IsAssigned())
            {
                SetLanguage(vModel.LocalizationId);
            }
            return CreateViewModelEntityDefinition<AddressForeignTextName>(vModel)              
                .UseDataContextCreate(i => !i.OwnerReferenceId.HasValue)
                .UseDataContextLocalizedUpdate(i => i.OwnerReferenceId.HasValue, i => o => (i.OwnerReferenceId == o.AddressForeignId), def => def.UseDataContextCreate(i => i.OwnerReferenceId.IsAssigned()))
                .AddNavigation(i => i.Text, o => o.Name)
                .AddRequestLanguage(output => output)
                .GetFinal();
        }
    }
}
