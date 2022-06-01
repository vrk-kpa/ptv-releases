using System;
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Next.Model;

namespace PTV.Database.DataAccess.Interfaces.Services
{
    public interface IStreetService
    {
        /// <summary>
        /// Searches street names in given name.
        /// </summary>
        /// <param name="searchedExpression">Beginning of the street name, case insensitive.</param>
        /// <param name="postalCodeId">Postal code of the area where the street should occur.</param>
        /// <param name="languageId">Language in which the street name will be displayed.</param>
        /// <param name="offset">Number of items to be skipped.</param>
        /// <param name="onlyValid">Search only valid streets.</param>
        /// <returns></returns>
        IVmSearchBase GetStreets(string searchedExpression, Guid? postalCodeId, Guid languageId,
            int offset, bool onlyValid = true);

        VmSearchResult<VmStreet> SearchStreets(
            string searchedExpression,
            Guid? postalCodeId,
            Guid languageId,
            int offset,
            bool onlyValid = true);

        Guid? GetStreetNumberRangeId(IUnitOfWork unitOfWork,
            Guid postalCodeId,
            string unparsedStreetNumber,
            Guid streetId);

        List<VmLanguageText> SearchStreetNames(string modelSearchText, bool modelOnlyValid);

        VmStreet GetNonUserCreatedStreet(string modelStreetName, string modelStreetNumber, PostalCodeModel postalCode);
    }
}