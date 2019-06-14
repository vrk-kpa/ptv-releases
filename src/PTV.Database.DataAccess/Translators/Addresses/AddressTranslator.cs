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
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Framework;
using PTV.Framework.Extensions;
using PTV.Framework.Interfaces;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Domain.Model.Models.Interfaces;

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
                .AddNavigation(input => input.Country, output => output.Country)
                .AddCollection(input => GetCoordinates(input.Coordinates), output => output.Coordinates)
                .AddNavigation(input => type.ToString(), output => output.StreetType)
                .AddPartial(i => i as IOrderable, o => o as IVmOrderable)
                .AddDictionary(input => input.Receivers, output => output.Receivers, key => languageCache.GetByValue(key.LocalizationId))
                .AddSimpleList(i => i.ExtraTypes.Select(et => et.ExtraTypeId), o => o.ExtraTypes);

            switch (type)
            {
                case AddressTypeEnum.Other:
                    definition.AddDictionary(i => i.AddressAdditionalInformations,o => o.AdditionalInformation, k => languageCache.GetByValue(k.LocalizationId));
                    definition.AddPartial(i => i.AddressOthers.FirstOrDefault(), o => o);
                    break;
                case AddressTypeEnum.PostOfficeBox:
                    definition.AddDictionary(i => i.AddressAdditionalInformations,o => o.AdditionalInformation, k => languageCache.GetByValue(k.LocalizationId));
                    definition.AddPartial(i => i.AddressPostOfficeBoxes.FirstOrDefault(), o => o);
                    break;
                case AddressTypeEnum.Street:
                    definition.AddDictionary(i => i.AddressAdditionalInformations, o => o.AdditionalInformation, k => languageCache.GetByValue(k.LocalizationId));
                    definition.AddPartial(i => i.ClsAddressPoints.FirstOrDefault(), o => o);
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
            if (!Enum.TryParse(vModel.StreetType, true, out AddressTypeEnum type))
            {
                type = AddressTypeEnum.Street;
            }
            var translationDefinition = CreateViewModelEntityDefinition<Address>(vModel)
                .UseDataContextCreate(input => !exists, output => output.Id, input => Guid.NewGuid())
                .UseDataContextUpdate(input => exists, input => output => input.Id == output.Id)
                .AddPartial(i => i as IVmOrderable, o => o as IOrderable)
                .AddSimple(input => input.UniqueId == Guid.Empty ? Guid.NewGuid() : input.UniqueId, output => output.UniqueId)
                .AddSimple(i => typesCache.Get<AddressType>(type.ToString()), o => o.TypeId)
                .AddCollection(i => i.Receivers?.Select(pair => new VmLocalizedAddressReceiver
                    {
                        Receiver = pair.Value,
                        LocalizationId = languageCache.Get(pair.Key),
                        OwnerReferenceId = i.Id
                    }),
                    o => o.Receivers, true);

            //TODO NEED to refactor two vm fields go to same db fields 
            if (type == AddressTypeEnum.NoAddress)
            {
                translationDefinition.AddCollection(i => i.NoAddressAdditionalInformation?.Select(
                        pair => new VmLocalizedAdditionalInformation()
                        {
                            Content = pair.Value,
                            LocalizationId = languageCache.Get(pair.Key),
                            OwnerReferenceId = i.Id
                        }),
                    o => o.AddressAdditionalInformations, true);
            }
            else
            {
                translationDefinition.AddCollection(i => i.AdditionalInformation?.Select(pair =>
                        new VmLocalizedAdditionalInformation()
                        {
                            Content = pair.Value,
                            LocalizationId = languageCache.Get(pair.Key),
                            OwnerReferenceId = i.Id
                        }),
                    o => o.AddressAdditionalInformations, true);

            }


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

            var entity = translationDefinition.GetFinal();
            
            vModel.Coordinates?.ForEach(x => x.OwnerReferenceId = entity.Id);
            
            switch (type)
            {
                case AddressTypeEnum.Other:
                    translationDefinition.AddNavigationOneMany(i => i, o => o.AddressOthers);
                    translationDefinition.AddCollection(i => new List<ClsAddressPoint>(), o => o.ClsAddressPoints, false);
                    translationDefinition.AddCollection(i => new List<AddressPostOfficeBox>(), o => o.AddressPostOfficeBoxes, false);
                    translationDefinition.AddCollection(i => new List<AddressForeign>(), o => o.AddressForeigns, false);
                    var acceptableStatesForOtherAddress = new[] {CoordinateStates.EnteredByUser.ToString().ToLower(), CoordinateStates.EnteredByAR.ToString().ToLower()};
                    translationDefinition.AddCollectionWithRemove(
                        i => vModel.Coordinates?.Where(x => acceptableStatesForOtherAddress.Contains(x.CoordinateState.ToLower())), o => o.Coordinates, TranslationPolicy.FetchData, r => true);
                    
                    break;
                case AddressTypeEnum.PostOfficeBox:
                    translationDefinition.AddNavigationOneMany(i => i, o => o.AddressPostOfficeBoxes);
                    translationDefinition.AddCollection(i => new List<ClsAddressPoint>(), o => o.ClsAddressPoints, false);
                    translationDefinition.AddCollection(i => new List<AddressOther>(), o => o.AddressOthers, false);
                    translationDefinition.AddCollection(i => new List<AddressForeign>(), o => o.AddressForeigns, false);
                    break;
                case AddressTypeEnum.Street:
                    translationDefinition.AddNavigationOneMany(i => i, o => o.ClsAddressPoints);
                    translationDefinition.AddCollection(i => new List<AddressPostOfficeBox>(), o => o.AddressPostOfficeBoxes, false);
                    translationDefinition.AddCollection(i => new List<AddressOther>(), o => o.AddressOthers, false);
                    translationDefinition.AddCollection(i => new List<AddressForeign>(), o => o.AddressForeigns, false);
                    
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
                        break;
                case AddressTypeEnum.Foreign:
                    translationDefinition.AddNavigationOneMany(i => i, o => o.AddressForeigns);
                    translationDefinition.AddCollection(i => new List<ClsAddressPoint>(), o => o.ClsAddressPoints, false);
                    translationDefinition.AddCollection(i => new List<AddressPostOfficeBox>(), o => o.AddressPostOfficeBoxes, false);
                    translationDefinition.AddCollection(i => new List<AddressOther>(), o => o.AddressOthers, false);
                    vModel.Coordinates?.Clear();
                    break;
                case AddressTypeEnum.NoAddress:
                    translationDefinition.AddCollection(i => new List<ClsAddressPoint>(), o => o.ClsAddressPoints, false);
                    translationDefinition.AddCollection(i => new List<AddressOther>(), o => o.AddressOthers, false);
                    translationDefinition.AddCollection(i => new List<AddressForeign>(), o => o.AddressForeigns, false);
                    translationDefinition.AddCollection(i => new List<AddressPostOfficeBox>(), o => o.AddressPostOfficeBoxes, false);
                    vModel.Coordinates?.Clear();
                    break;
                default:
                    break;
            }

            return entity;
        }

        private AddressTypeEnum GetAddressType(Address address)
        {
            if (address?.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.Street.ToString())) return AddressTypeEnum.Street;
            if (address?.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.PostOfficeBox.ToString())) return AddressTypeEnum.PostOfficeBox;
            if (address?.TypeId == typesCache.Get<AddressType>(AddressTypeEnum.Other.ToString())) return AddressTypeEnum.Other;
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
                        o => o.OldStreetNames,
                        k => languageCache.GetByValue(k.LocalizationId)
                    )
                    // Old street addresses will not be valid anymore
                    .AddSimple(i => true, o => o.InvalidAddress)
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
            .AddSimple(i => i.PostalCode?.Id, o => o.PostalCodeId)
            .AddCollection(i => i.OldStreetNames?.Select(pair => new VmLocalizedStreetName()
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
    
    // -------------------------------------- AddressOther -------------------------------------- //
    [RegisterService(typeof(ITranslator<AddressOther, VmAddressSimple>), RegisterType.Transient)]
    internal class AddressOtherTranslator : Translator<AddressOther, VmAddressSimple>
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        public AddressOtherTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives, ICacheManager cacheManager) : base(resolveManager,
            translationPrimitives)
        {
            typesCache = cacheManager.TypesCache;
            languageCache = cacheManager.LanguageCache;
        }

        public override VmAddressSimple TranslateEntityToVm(AddressOther entity)
        {
            return CreateEntityViewModelDefinition(entity)
                    .AddNavigation(input => input.PostalCode != null && input.PostalCode.Code != "Undefined" ? input.PostalCode : null, output => output.PostalCode)
                    .GetFinal();
        }

        public override AddressOther TranslateVmToEntity(VmAddressSimple vModel)
        {
            return CreateViewModelEntityDefinition(vModel)
                .UseDataContextCreate(i => !i.Id.IsAssigned())
                .UseDataContextUpdate(
                    i => i.Id.IsAssigned(),
                    i => o => i.Id == o.AddressId,
                    def => def.UseDataContextCreate(i => true, i => i.AddressId, i=> i.Id)
                )
                .AddSimple(i => i.PostalCode?.Id, o => o.PostalCodeId)
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
                .AddSimple(i => i.PostalCode?.Id, o => o.PostalCodeId)
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

    // Cls Address Point - Vm Address Point
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
                .AddSimple(i => i.Street?.Id ?? Guid.Empty, o => o.AddressStreetId)
                .AddSimple(i => i.AddressStreetNumber?.Id ?? Guid.Empty, o => o.AddressStreetId)
                .AddNavigation(i => i.StreetNumber, o => o.StreetNumber)
                .AddSimple(i => i.PostalCode?.Id ?? Guid.Empty, o => o.PostalCodeId);

            return translation.GetFinal();
        }
    }
    
    // Cls Address Point - Vm Address Simple
    [RegisterService(typeof(ITranslator<ClsAddressPoint, VmAddressSimple>), RegisterType.Transient)]
    internal class ClsAddressPointSimpleTranslator : Translator<ClsAddressPoint, VmAddressSimple>
    {
        public ClsAddressPointSimpleTranslator(IResolveManager resolveManager, ITranslationPrimitives translationPrimitives) 
            : base(resolveManager, translationPrimitives)
        {
        }

        public override VmAddressSimple TranslateEntityToVm(ClsAddressPoint entity)
        {
            var translation = CreateEntityViewModelDefinition<VmAddressSimple>(entity)
                .AddSimple(i => !GetIsValidValue(entity), o => o.InvalidAddress)
                .AddSimple(i => i.Id, o => o.ClsPointId)
                .AddSimple(i => i.MunicipalityId, o => o.Municipality)
                .AddNavigation(i => i.AddressStreet, o => o.Street)
                .AddNavigation(i => i.AddressStreetNumber?.IsValid == true ? i.AddressStreetNumber : null, o => o.StreetNumberRange)
                .AddNavigation(i => i.StreetNumber, o => o.StreetNumber)
                .AddNavigation(i => i.PostalCode, o => o.PostalCode);

            return translation.GetFinal();
        }

        public override ClsAddressPoint TranslateVmToEntity(VmAddressSimple vModel)
        {
            var translation = CreateViewModelEntityDefinition<ClsAddressPoint>(vModel)
                .UseDataContextCreate(i => !i.ClsPointId.IsAssigned())
                .UseDataContextUpdate(i => i.ClsPointId.IsAssigned(), i => o => i.ClsPointId == o.Id)
                .AddSimple(i => !i.InvalidAddress, o => o.IsValid)
                .AddSimple(i => i.Municipality ?? Guid.Empty, o => o.MunicipalityId)
                .AddSimple(i => i.Street?.Id ?? Guid.Empty, o => o.AddressStreetId)
                .AddSimple(i => i.StreetNumberRange?.Id, o => o.AddressStreetNumberId)
                .AddNavigation(i => i.StreetNumber, o => o.StreetNumber)
                .AddSimple(i => i.PostalCode?.Id ?? Guid.Empty, o => o.PostalCodeId);

            return translation.GetFinal();
        }

        private bool GetIsValidValue(ClsAddressPoint addressPoint)
        {
            return addressPoint.IsValid
                   && (addressPoint.Municipality?.IsValid ?? false)
                   && (addressPoint.PostalCode?.IsValid ?? false)
                   && (addressPoint.AddressStreet?.IsValid ?? false)
                   && !string.IsNullOrWhiteSpace(addressPoint.StreetNumber)
                   && (addressPoint.AddressStreetNumber == null || addressPoint.AddressStreetNumber.IsValid);
        }
    }
}
