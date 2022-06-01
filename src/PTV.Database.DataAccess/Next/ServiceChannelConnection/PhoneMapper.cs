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
    internal interface IPhoneMapper
    {
        Dictionary<LanguageEnum, List<FaxNumberLvModel>> MapFaxNumbers(ServiceServiceChannel connectionSource, 
            List<Guid> languageIds);
        Dictionary<LanguageEnum, List<PhoneNumberLvModel>> MapPhoneNumbers(ServiceServiceChannel connectionSource, 
            List<Guid> languageIds);
    }

    [RegisterService(typeof(IPhoneMapper), RegisterType.Transient)]
    internal class PhoneMapper: IPhoneMapper
    {
        private readonly ITypesCache typesCache;
        private readonly ILanguageCache languageCache;

        public PhoneMapper(ITypesCache typesCache,
            ILanguageCache languageCache)
        {
            this.typesCache = typesCache;
            this.languageCache = languageCache;

        }

        public Dictionary<LanguageEnum, List<PhoneNumberLvModel>> MapPhoneNumbers(ServiceServiceChannel connectionSource, 
            List<Guid> languageIds)
        {
            var result = new Dictionary<LanguageEnum, List<PhoneNumberLvModel>>();

            foreach(var languageId in languageIds)
            {
                var lang = Helpers.MapLanguage(languageCache, languageId);
                var phoneNumbers = CreatePhoneNumbers(connectionSource, languageId);
                if (phoneNumbers.Any())
                {
                    result.Add(lang, phoneNumbers);
                }
            }

            return result;

        }

        public Dictionary<LanguageEnum, List<FaxNumberLvModel>> MapFaxNumbers(ServiceServiceChannel connectionSource, 
            List<Guid> languageIds)
        {
            var result = new Dictionary<LanguageEnum, List<FaxNumberLvModel>>();

            foreach(var languageId in languageIds)
            {
                var lang = Helpers.MapLanguage(languageCache, languageId);
                var faxNumbers = CreateFaxNumbers(connectionSource, languageId);
                if (faxNumbers.Any())
                {
                    result.Add(lang, faxNumbers);
                }
            }

            return result;

        }

        private List<FaxNumberLvModel> CreateFaxNumbers(ServiceServiceChannel connectionSource, Guid languageId)
        {
            var result = new List<FaxNumberLvModel>();            

            var faxTypeId = typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Fax.ToString());

            var faxNumbers = connectionSource.ServiceServiceChannelPhones
                .Where(x => x.Phone.LocalizationId == languageId && x.Phone.TypeId == faxTypeId)
                .OrderBy(x => x.Phone.OrderNumber)
                .ToList();

            foreach(var faxNumber in faxNumbers)
            {
                result.Add(new FaxNumberLvModel
                {
                    Id = faxNumber.Phone.Id,
                    Number = faxNumber.Phone.Number.NullToEmpty(),
                    DialCodeId = faxNumber.Phone.PrefixNumber?.Id,
                    OrderNumber = faxNumber.Phone.OrderNumber.HasValue ? faxNumber.Phone.OrderNumber.Value : 0,
                    Type = typesCache.GetByValue<PhoneNumberType>(faxNumber.Phone.TypeId).ToEnum<PhoneNumberTypeEnum>()
                });
            }

            return result;
        }

        private List<PhoneNumberLvModel> CreatePhoneNumbers(ServiceServiceChannel connectionSource, Guid languageId)
        {
            var result = new List<PhoneNumberLvModel>();            

            var phoneTypeId = typesCache.Get<PhoneNumberType>(PhoneNumberTypeEnum.Phone.ToString());

            var phoneNumbers = connectionSource.ServiceServiceChannelPhones
                .Where(x => x.Phone.LocalizationId == languageId && x.Phone.TypeId == phoneTypeId)
                .OrderBy(x => x.Phone.OrderNumber)
                .ToList();

            foreach(var phoneNumber in phoneNumbers)
            {
                result.Add(new PhoneNumberLvModel
                {
                    Id = phoneNumber.Phone.Id,
                    Number = phoneNumber.Phone.Number.NullToEmpty(),
                    DialCodeId = phoneNumber.Phone.PrefixNumber?.Id,
                    AdditionalInformation = phoneNumber.Phone.AdditionalInformation.NullToEmpty(),
                    ChargeDescription = phoneNumber.Phone.ChargeDescription.NullToEmpty(),
                    OrderNumber = phoneNumber.Phone.OrderNumber.HasValue ? phoneNumber.Phone.OrderNumber.Value : 0,
                    ChargeType = typesCache.GetByValue<ServiceChargeType>(phoneNumber.Phone.ChargeTypeId).ToEnum<ServiceChargeTypeEnum>(),
                    Type = typesCache.GetByValue<PhoneNumberType>(phoneNumber.Phone.TypeId).ToEnum<PhoneNumberTypeEnum>()
                });
            }

            return result;
        }
    }
}
