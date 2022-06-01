using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.Model.Models;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Next.Model;
using System;
using System.Collections.Generic;
using System.Linq;
namespace PTV.Database.DataAccess.Next.ServiceChannelConnection
{
    internal interface IAddressMapper
    {
        AddressModel Map(ServiceServiceChannelAddress serviceChannelAddress, List<Guid> languages);
    }

    [RegisterService(typeof(IAddressMapper), RegisterType.Transient)]
    internal class AddressMapper : IAddressMapper
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;
        private readonly IPostalCodeCache postalCodeCache;

        public AddressMapper(ITypesCache typesCache, 
            ILanguageCache languageCache,
            IPostalCodeCache postalCodeCache)
        {
            this.typesCache = typesCache;
            this.languageCache = languageCache;
            this.postalCodeCache = postalCodeCache;
        }

        public AddressModel Map(ServiceServiceChannelAddress serviceChannelAddress, List<Guid> languageIds)
        {
            var source = serviceChannelAddress.Address;
            var addressType = typesCache.GetByValue<AddressType>(source.TypeId).ToEnum<AddressTypeEnum>();

            var model = new AddressModel();
            model.Type = addressType;
            model.Character =  typesCache.GetByValue<AddressCharacter>(serviceChannelAddress.CharacterId).ToEnum<AddressCharacterEnum>(); // Visiting, postal, delivery
            model.OrderNumber = source.OrderNumber;
            
            if (addressType == AddressTypeEnum.Street)
            {
                FillStreetAddress(serviceChannelAddress, languageIds, model);
            }
            else if (addressType == AddressTypeEnum.PostOfficeBox)
            {
                FillPostOfficeBox(serviceChannelAddress, languageIds, model);
            }
            else if (addressType == AddressTypeEnum.Foreign)
            {
                FillForeignAddress(serviceChannelAddress, languageIds, model);
            }
            else
            {
                throw new Exception($"No suport for mapping address of type {addressType} for connection address {source.Id}");
            }

            return model;            
        }

        private void FillForeignAddress(ServiceServiceChannelAddress source,  List<Guid> languageIds, AddressModel target)
        {
            var foreign = source.Address.AddressForeigns.FirstOrDefault();
            target.CountryCode = source.Address.Country.Code;

            var languages = Helpers.ToLanguageList(languageCache, languageIds);
            target.LanguageVersions = languages
                .Select(x => MapForeignLv(source, x.LanguageId))
                .ToDictionary(x => x.Language);
        }

        private void FillPostOfficeBox(ServiceServiceChannelAddress source, List<Guid> languageIds, AddressModel target)
        {
            var poBox = source.Address.AddressPostOfficeBoxes.FirstOrDefault();
           
            target.PostalCode = poBox.PostalCode.Code;
            target.CountryCode = source.Address.Country.Code;

            var languages = Helpers.ToLanguageList(languageCache, languageIds);
            target.LanguageVersions = languages
                .Select(x => MapPostOfficeBoxLv(source, poBox, x.LanguageId))
                .ToDictionary(x => x.Language);
        }

        private void FillStreetAddress(ServiceServiceChannelAddress source, List<Guid> languageIds, AddressModel target)
        {
            var point = source.Address.ClsAddressPoints.FirstOrDefault();
            target.PostalCode = point.PostalCode.Code;
            target.CountryCode = source.Address.Country.Code;
            target.StreetNumber = point.StreetNumber.NullToEmpty();

            // The connection can have e.g. languages FI and EN but the street name itself can have separate languages.
            var names = point.AddressStreet.Names.Select(x => 
                new {
                    Language = languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), 
                    Value = x.Name
                    }
                ).ToDictionary(x => x.Language, x => x.Value);
            
            target.StreetName = names;
            target.StreetNumberRange = MapStreetNumberRange(point.AddressStreetNumber);

            target.Street = MapStreet(point);

            var languages = Helpers.ToLanguageList(languageCache, languageIds);
            target.LanguageVersions = languages
                .Select(x => MapStreetLv(source, x.LanguageId))
                .ToDictionary(x => x.Language);
        }

        private StreetModel MapStreet(ClsAddressPoint point)
        {
            var streetNumbers = point.AddressStreet.StreetNumbers
                .Where(x => x.IsValid)
                .Select(MapStreetNumberRange)
                .ToList();

            var names = point.AddressStreet.Names
                .Select(x => new {Language = languageCache.GetByValue(x.LocalizationId).ToEnum<LanguageEnum>(), Value = x.Name})
                .ToDictionary(x => x.Language, x => x.Value);

            var street = new StreetModel
            {
                Id = point.AddressStreet.Id,
                Names = names,
                MunicipalityCode = point.Municipality.Code,
                IsValid = point.IsValid,
                StreetNumbers = streetNumbers
            };

            return street;
        }

        private StreetNumberModel MapStreetNumberRange(ClsAddressStreetNumber source)
        {
            if (source == null) return null;

            var postalCode = postalCodeCache.GetById(source.PostalCodeId);

            return new StreetNumberModel
            {
                Id = source.Id,
                StreetId = source.ClsAddressStreetId,
                StartNumber = source.StartNumber,
                EndNumber = source.EndNumber,
                IsEven = source.IsEven,
                PostalCode = postalCode.Code,
                IsValid = source.IsValid
            };
        }

        private AddressLvModel MapStreetLv(ServiceServiceChannelAddress serviceChannelAddress, Guid languageId)
        {
            var lv = new AddressLvModel();
            lv.Language = Helpers.MapLanguage(languageCache, languageId);
            lv.AdditionalInformation = MapInfo(serviceChannelAddress, languageId);
            lv.PoBox = string.Empty;
            lv.ForeignAddress = string.Empty;
            return lv;
        }

        private AddressLvModel MapPostOfficeBoxLv(ServiceServiceChannelAddress serviceChannelAddress, 
            AddressPostOfficeBox poBox,
            Guid languageId)
        {
            var lv = new AddressLvModel();
            lv.Language = Helpers.MapLanguage(languageCache, languageId);

            var name = poBox.PostOfficeBoxNames.FirstOrDefault(x => x.LocalizationId == languageId);
            lv.PoBox = name?.Name.NullToEmpty();
            lv.AdditionalInformation = MapInfo(serviceChannelAddress, languageId);
            lv.ForeignAddress = string.Empty;
            return lv;
        }

        private AddressLvModel MapForeignLv(ServiceServiceChannelAddress serviceChannelAddress, Guid languageId)
        {
            var lv = new AddressLvModel();
            lv.Language = Helpers.MapLanguage(languageCache, languageId);
            var foreign = serviceChannelAddress.Address.AddressForeigns.FirstOrDefault();
            var text = foreign.ForeignTextNames.FirstOrDefault(x => x.LocalizationId == languageId);
            lv.ForeignAddress = text?.Name.NullToEmpty();
            lv.AdditionalInformation = string.Empty;
            lv.PoBox= string.Empty;
            return lv;
        }
        private string MapInfo(ServiceServiceChannelAddress serviceChannelAddress, Guid languageId)
        {
            var info = serviceChannelAddress.Address.AddressAdditionalInformations.FirstOrDefault(x => x.LocalizationId == languageId);
            return info?.Text.NullToEmpty();
        }
    }
}
