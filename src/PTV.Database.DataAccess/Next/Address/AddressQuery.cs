using System;
using System.Collections.Generic;
using System.Linq;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Next;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Next.Street;
using PTV.Database.Model.Models;
using PTV.Framework;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Next.Address
{
    public interface IAddressQuery
    {
        StreetSearchResult Search(StreetSearchModel model);
        StreetNameResult SearchStreetName(StreetNameSearchModel model);
        StreetModel ValidateStreetAddress(StreetValidateModel model);
    }

    [RegisterService(typeof(IAddressQuery), RegisterType.Transient)]
    internal class AddressQuery : IAddressQuery
    {
        private readonly IStreetService service;
        private readonly ITypesCache typesCache;
        private readonly IPostalCodeCache postalCodeCache;
        private readonly IAddressMapper mapper;
        private readonly IEnumQueries enumQueries;
        
        public AddressQuery(IStreetService service,
            ITypesCache typesCache,
            IPostalCodeCache postalCodeCache,
            IEnumQueries enumQueries,
            IAddressMapper mapper)
        {
            this.service = service;
            this.typesCache = typesCache;
            this.postalCodeCache = postalCodeCache;
            this.enumQueries = enumQueries;
            this.mapper = mapper;
        }

        public StreetSearchResult Search(StreetSearchModel model)
        {
            var languageId = typesCache.Get<Language>(model.Language.ToString());
            var postalCodeId = string.IsNullOrEmpty(model.PostalCode)
                ? (Guid?) null
                : postalCodeCache.GuidByCode(model.PostalCode);
            var result = service.SearchStreets(model.SearchText, postalCodeId, languageId, model.Offset,
                model.OnlyValid);
            return mapper.Map(result);
        }

        public StreetNameResult SearchStreetName(StreetNameSearchModel model)
        {
            if (model.SearchText.IsNullOrEmpty()) return new StreetNameResult();
            var result = service.SearchStreetNames(model.SearchText, model.OnlyValid);
            return mapper.Map(result);
        }


        public StreetModel ValidateStreetAddress(StreetValidateModel model)
        { 
            if (model.StreetName.IsNullOrWhitespace() || model.PostalCode.IsNullOrWhitespace())
            {
                return new StreetModel
                {
                    IsValid = false,
                };
            }

            var postalCode = enumQueries.GetAllPostalCodes().FirstOrDefault(x => x.Code.Equals(model.PostalCode));
            var street = service.GetNonUserCreatedStreet(model.StreetName, model.StreetNumber, postalCode);
            return mapper.Map(street);
        }
    }
}