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
using System.Linq;
using Dapper;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.DirectRaw;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Models;
using PTV.Domain.Model;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Framework;
using PTV.Framework.ServiceManager;

namespace PTV.Database.DataAccess.Services
{
    [RegisterService(typeof(IStreetService), RegisterType.Transient)]
    internal class StreetService : ServiceBase, IStreetService
    {
        private readonly IDatabaseRawContext rawContext;
        private readonly Guid defaultLanguageId;

        public StreetService(
            ITranslationEntity translationEntToVm,
            ITranslationViewModel translationManagerToEntity, 
            IPublishingStatusCache publishingStatusCache,
            ILanguageCache languageCache,
            IUserOrganizationChecker userOrganizationChecker,           
            IDatabaseRawContext rawContext,
            IVersioningManager versioningManager)
            : base(translationEntToVm, translationManagerToEntity, publishingStatusCache, userOrganizationChecker,
                versioningManager)
        {
            this.rawContext = rawContext;
            var defaultLanguageCode = DomainConstants.DefaultLanguage;
            defaultLanguageId = languageCache.Get(defaultLanguageCode);
        }

        public IVmListItemsData<IVmStreet> GetStreets(
            string searchedExpression, 
            Guid? postalCodeId, 
            Guid languageId,
            bool onlyValid = true)
        {
            var lowerSearchedExpression = searchedExpression?.ToLower();
            var name3 = lowerSearchedExpression.SafeSubstring(0, 3);
            var streetDictionary = new Dictionary<Guid, StreetRelation>();

            var sqlQuery = GetSearchStreetsSelect(postalCodeId.HasValue, onlyValid);

            var splitOn = SplitOnColumns;
                             
            var commandParams =  new
            {
                Name3 = new DbString
                {
                    Value = name3,
                    IsFixedLength = true,
                    Length = 3
                },
                SearchedExpression = new DbString
                {
                    Value = lowerSearchedExpression,
                    IsFixedLength = false
                },
                PostalCode = postalCodeId,
                Language = languageId,
                DefaultLanguageId = defaultLanguageId
            };

            var streets = rawContext.ExecuteReader(db => db.SelectListWithInclude(
                sqlQuery,
                commandParams, 
                GetConnectFunc(streetDictionary), 
                splitOn));
            
            var translations = TranslationManagerToVm.TranslateAll<ClsAddressStreet, VmStreet>(streets);
            return new VmListItemsData<IVmStreet>(translations ?? new List<VmStreet>());
        }

        #region DAPPER HELPERS
        private static class DBNames
        {
            public static class Street
            {
                public const string TableName = nameof(ClsAddressStreet);
                public const string Id = nameof(ClsAddressStreet.Id);
                public const string IsValid = nameof(ClsAddressStreet.IsValid);
                public const string MunicipalityId = nameof(ClsAddressStreet.MunicipalityId);
                public const string NonCls = nameof(ClsAddressStreet.NonCls);
            }

            public static class StreetName
            {
                public const string TableName = nameof(ClsAddressStreetName);
                public const string StreetId = nameof(ClsAddressStreetName.ClsAddressStreetId);
                public const string Name = nameof(ClsAddressStreetName.Name);
                public const string Name3 = nameof(ClsAddressStreetName.Name3);
                public const string LocalizationId = nameof(ClsAddressStreetName.LocalizationId);
                public const string NonCls = nameof(ClsAddressStreetName.NonCls);
            }
            
            public static class StreetNumber
            {
                public const string TableName = nameof(ClsAddressStreetNumber);
                public const string StreetId = nameof(ClsAddressStreetNumber.ClsAddressStreetId);
                public const string PostalCodeId = nameof(ClsAddressStreetNumber.PostalCodeId);
                public const string IsValid = nameof(ClsAddressStreetNumber.IsValid);
                public const string Id = nameof(ClsAddressStreetNumber.Id);
                public const string IsEven = nameof(ClsAddressStreetNumber.IsEven);
                public const string StartNumber = nameof(ClsAddressStreetNumber.StartNumber);
                public const string EndNumber = nameof(ClsAddressStreetNumber.EndNumber);
                public const string NonCls = nameof(ClsAddressStreetNumber.NonCls);
            }

            public static class PostalCode
            {
                public const string TableName = nameof(Model.Models.PostalCode);
                public const string Id = nameof(Model.Models.PostalCode.Id);
                public const string Code = nameof(Model.Models.PostalCode.Code);
                public const string MunicipalityId = nameof(Model.Models.PostalCode.MunicipalityId);
                public const string IsValid = nameof(Model.Models.PostalCode.IsValid);
            }

            public static class PostalCodeName
            {
                public const string TableName = nameof(Model.Models.PostalCodeName);
                public const string PostalCodeId = nameof(Model.Models.PostalCodeName.PostalCodeId);
                public const string LocalizationId = nameof(Model.Models.PostalCodeName.LocalizationId);
                public const string Name = nameof(Model.Models.PostalCodeName.Name);
            }

            public static class Municipality
            {
                public const string TableName = nameof(Model.Models.Municipality);
                public const string Id = nameof(Model.Models.Municipality.Id);
                public const string Code = nameof(Model.Models.Municipality.Code);
                public const string IsValid = nameof(Model.Models.Municipality.IsValid);
            }
        }
        
        private static string SplitOnColumns =>
            string.Join(",",
                nameof(ClsAddressStreetName.ClsAddressStreetId),
                nameof(ClsAddressStreetNumber.Id),
                nameof(PostalCode.Id),
                nameof(PostalCodeName.PostalCodeId),
                nameof(Municipality.Id));

        private string GetSearchStreetsSelect(bool hasPostalCode, bool onlyValid)
        {
            const string withAlias = "x";
            const string withStreetAlias = "xStr";
            const string withNameAlias = "xNam";
            const string withNumberAlias = "xNum";
            const string withPostalCodeAlias = "xPC";

            var withSubquery = "WITH " + withAlias + " AS (" +
               "SELECT DISTINCT " + GetColumn(withStreetAlias, nameof(DBNames.Street.Id), true) + " " +
               "FROM " + DBNames.Street.TableName.WithQuotes() + " AS " + withStreetAlias + " " +
               JoinClauseAs(withStreetAlias, DBNames.StreetName.TableName, withNameAlias, DBNames.Street.Id, DBNames.StreetName.StreetId) + " " +
               JoinClauseAs(withStreetAlias, DBNames.StreetNumber.TableName, withNumberAlias, DBNames.Street.Id, DBNames.StreetNumber.StreetId) + " " +
               JoinClauseAs(withNumberAlias, DBNames.PostalCode.TableName, withPostalCodeAlias, DBNames.StreetNumber.PostalCodeId, DBNames.PostalCode.Id) + " " +
               "WHERE " + GetColumn(withStreetAlias, DBNames.Street.IsValid, true) + " = TRUE " +
               "AND " + GetColumn(withNumberAlias, DBNames.StreetNumber.IsValid, true) + " = TRUE " +
               "AND " + GetColumn(withNameAlias, DBNames.StreetName.Name3, true) + " = @Name3 " +
               "AND LOWER(" + GetColumn(withNameAlias, DBNames.StreetName.Name, true) + ") LIKE CONCAT(@SearchedExpression, '%') " +
               (hasPostalCode
                   ? "AND " + GetColumn(withPostalCodeAlias, DBNames.PostalCode.Id, true) + " = @PostalCode" + " "
                   : "") +
               "LIMIT 100) ";

            var whereSubquery = "WHERE (" + GetColumn(DBNames.StreetName.TableName, DBNames.StreetName.LocalizationId) + " = @Language " +
                "OR " + GetColumn(DBNames.StreetName.TableName, DBNames.StreetName.LocalizationId) + " = @DefaultLanguageId) " + 
                (onlyValid
                    ? "AND " + GetColumn(DBNames.Street.TableName, DBNames.Street.IsValid) + " = TRUE" + " " + 
                      "AND " + GetColumn(DBNames.StreetNumber.TableName, DBNames.StreetNumber.IsValid) + " = TRUE "
                    : "");

            return withSubquery +
                   // street
                "SELECT DISTINCT " + GetColumn(DBNames.Street.TableName, DBNames.Street.Id) + ", " + 
                GetColumn(DBNames.Street.TableName, DBNames.Street.MunicipalityId) + ", " + 
                GetColumn(DBNames.Street.TableName, DBNames.Street.IsValid) + ", " + 
                GetColumn(DBNames.Street.TableName, DBNames.Street.NonCls) + ", " + 
                   // street name
                GetColumn(DBNames.StreetName.TableName, DBNames.StreetName.StreetId) + ", " +
                GetColumn(DBNames.StreetName.TableName, DBNames.StreetName.LocalizationId) + ", " +
                GetColumn(DBNames.StreetName.TableName, DBNames.StreetName.Name) + ", " +
                GetColumn(DBNames.StreetName.TableName, DBNames.StreetName.Name3) + ", " +
                GetColumn(DBNames.StreetName.TableName, DBNames.StreetName.NonCls) + ", " +
                // street number
                GetColumn(DBNames.StreetNumber.TableName, DBNames.StreetNumber.Id) + ", " + 
                GetColumn(DBNames.StreetNumber.TableName, DBNames.StreetNumber.StreetId) + ", " + 
                GetColumn(DBNames.StreetNumber.TableName, DBNames.StreetNumber.PostalCodeId) + ", " + 
                GetColumn(DBNames.StreetNumber.TableName, DBNames.StreetNumber.IsEven) + ", " + 
                GetColumn(DBNames.StreetNumber.TableName, DBNames.StreetNumber.StartNumber) + ", " + 
                GetColumn(DBNames.StreetNumber.TableName, DBNames.StreetNumber.EndNumber) + ", " + 
                GetColumn(DBNames.StreetNumber.TableName, DBNames.StreetNumber.IsValid) + ", " + 
                GetColumn(DBNames.StreetNumber.TableName, DBNames.StreetNumber.NonCls) + ", " +
                   // postal code
                GetColumn(DBNames.PostalCode.TableName, DBNames.PostalCode.Id) + ", " + 
                GetColumn(DBNames.PostalCode.TableName, DBNames.PostalCode.Code) + ", " + 
                GetColumn(DBNames.PostalCode.TableName, DBNames.PostalCode.MunicipalityId) + ", " + 
                GetColumn(DBNames.PostalCode.TableName, DBNames.PostalCode.IsValid) + ", " +
                   // postal code name
                GetColumn(DBNames.PostalCodeName.TableName, DBNames.PostalCodeName.PostalCodeId) + ", " +
                GetColumn(DBNames.PostalCodeName.TableName, DBNames.PostalCodeName.LocalizationId) + ", " +
                GetColumn(DBNames.PostalCodeName.TableName, DBNames.PostalCodeName.Name) + ", " +
                // municipality
                GetColumn(DBNames.Municipality.TableName, DBNames.Municipality.Id) + ", " + 
                GetColumn(DBNames.Municipality.TableName, DBNames.Municipality.Code) + ", " +  
                GetColumn(DBNames.Municipality.TableName, DBNames.Municipality.IsValid) + " " +
                "FROM " + DBNames.Street.TableName.WithQuotes() + " " +
                "JOIN " + withAlias + " ON " + GetColumn(withAlias, DBNames.Street.Id, true) + " = " + GetColumn(DBNames.Street.TableName, DBNames.Street.Id) + " " +
                JoinClause(DBNames.Street.TableName, DBNames.StreetName.TableName, DBNames.Street.Id, DBNames.StreetName.StreetId) + " " +
                JoinClause(DBNames.Street.TableName, DBNames.StreetNumber.TableName, DBNames.Street.Id, DBNames.StreetNumber.StreetId) + " " +
                JoinClause(DBNames.StreetNumber.TableName, DBNames.PostalCode.TableName, DBNames.StreetNumber.PostalCodeId, DBNames.PostalCode.Id) + " " +
                JoinClause(DBNames.PostalCode.TableName, DBNames.PostalCodeName.TableName, DBNames.PostalCode.Id, DBNames.PostalCodeName.PostalCodeId) + " " +
                JoinClause(DBNames.Street.TableName, DBNames.Municipality.TableName, DBNames.Street.MunicipalityId, DBNames.Municipality.Id) + " " +
                whereSubquery + 
                "ORDER BY " + GetColumn(DBNames.StreetName.TableName, DBNames.StreetName.Name) + ", " +
                GetColumn(DBNames.PostalCodeName.TableName, DBNames.PostalCodeName.Name);
        }

        private Func<
            ClsAddressStreet, 
            ClsAddressStreetName, 
            ClsAddressStreetNumber, 
            PostalCode, 
            PostalCodeName,
            Municipality, 
            ClsAddressStreet> GetConnectFunc(
            Dictionary<Guid, StreetRelation> streetDictionary)
        {
            return (street, streetName, streetNumber, postalCode, postalCodeName, municipality) =>
            {
                if (!streetDictionary.TryGetValue(street.Id, out var streetRelation))
                {
                    streetRelation = (StreetRelation)street;
                    streetRelation.Street.Municipality = municipality;
                    streetRelation.Street.StreetNames = new List<ClsAddressStreetName>();
                    streetRelation.Street.StreetNumbers = new List<ClsAddressStreetNumber>();
                    streetDictionary.Add(street.Id, streetRelation);
                }

                if (!streetRelation.StreetNameLocalizationIds.Contains(streetName.LocalizationId))
                {
                    streetRelation.StreetNameLocalizationIds.Add(streetName.LocalizationId);
                    streetRelation.Street.StreetNames.Add(streetName);
                }

                if (!streetRelation.StreetNumberDictionary.TryGetValue(streetNumber.Id, out var streetNumberRelation))
                {
                    postalCode.PostalCodeNames = new List<PostalCodeName>();
                    streetNumber.PostalCode = postalCode;
                    streetNumberRelation = (StreetNumberRelation) streetNumber;
                    streetRelation.StreetNumberDictionary.Add(streetNumber.Id, streetNumberRelation);
                    streetRelation.Street.StreetNumbers.Add(streetNumber);
                }

                if (!streetNumberRelation.PostalCodeLocalizationIds.Contains(postalCodeName.LocalizationId))
                {
                    streetNumberRelation.PostalCodeLocalizationIds.Add(postalCodeName.LocalizationId);
                    streetNumberRelation.StreetNumber.PostalCode.PostalCodeNames.Add(postalCodeName);
                }

                return (ClsAddressStreet)streetRelation;
            };
        }

        private string GetColumn(string tableName, string columnName, bool isAlias = false)
        {
            var tableNameFormatted = isAlias ? tableName : tableName.WithQuotes();
            var columnNameFormatted = columnName == "*" ? "*" : columnName.WithQuotes();

            return tableNameFormatted + "." + columnNameFormatted;
        }

        private string JoinClause(string thisTableName, string otherTableName, string thisKey, string otherKey)
        {
            return
                $"JOIN {otherTableName.WithQuotes()} ON {thisTableName.WithQuotes()}.{thisKey.WithQuotes()} = {otherTableName.WithQuotes()}.{otherKey.WithQuotes()}";
        }

        private string JoinClauseAs(string thisTableAlias, string otherTableName, string otherTableAlias,
            string thisKey, string otherKey)
        {
            return
                $"JOIN {otherTableName.WithQuotes()} AS {otherTableAlias} ON {otherTableAlias}.{otherKey.WithQuotes()} = {thisTableAlias}.{thisKey.WithQuotes()}";
        }

        #endregion

        public bool IsAddressPointValid(Guid streetId, Guid municipalityId, Guid? streetNumberId, Guid postalCodeId, IUnitOfWork unitOfWork)
        {
            if(streetId == Guid.Empty
               || municipalityId == Guid.Empty
               || postalCodeId == Guid.Empty)
                throw new PtvAppException("All address information must be filled in.", "Common.AddressException.InvalidInfo");
            
            var streetRepo = unitOfWork.CreateRepository<IClsAddressStreetRepository>();
            var street = streetRepo.All()
                .Include(s => s.Municipality)
                .Include(s => s.StreetNumbers)
                .ThenInclude(sn => sn.PostalCode)
                .SingleOrDefault(s => s.Id == streetId);

            if (street == null 
                || !street.IsValid 
                || street.MunicipalityId != municipalityId 
                || !street.Municipality.IsValid)
                return false;

            if (streetNumberId == null)
            {
                return street.StreetNumbers.Any(sn => sn.PostalCodeId == postalCodeId);
            }

            var streetNumberRange =
                street.StreetNumbers.SingleOrDefault(sn => sn.Id == streetNumberId);

            return streetNumberRange != null
                   && streetNumberRange.IsValid
                   && streetNumberRange.PostalCodeId == postalCodeId
                   && streetNumberRange.PostalCode.IsValid
                   && streetNumberRange.PostalCode.MunicipalityId == municipalityId;
        }

        public Guid? GetStreetNumberRangeId(IUnitOfWork unitOfWork, string streetNumber, Guid streetId)
        {
            var parsedStreetNumber = ParseStreetNumber(streetNumber);
            if (parsedStreetNumber == null)
            {
                return null;
            }

            var streetNumberRepo = unitOfWork.CreateRepository<IClsAddressStreetNumberRepository>();
            var streetNumberRange = streetNumberRepo.All()
                .FirstOrDefault(sn =>
                    IsAddressNumberInRange(parsedStreetNumber, sn.StartNumber, sn.EndNumber, sn.IsEven) 
                    && sn.ClsAddressStreetId == streetId);

            return streetNumberRange?.Id;
        }

        public static bool IsAddressNumberInRange(string input, int rangeStart, int rangeEnd, bool isEven)
        {
            var parsedNumber = ParseStreetNumber(input?.Trim());
            return IsAddressNumberInRange(parsedNumber, rangeStart, rangeEnd, isEven);
        }

        private static bool IsAddressNumberInRange(int? input, int rangeStart, int rangeEnd, bool isEven)
        {
            if (input % 2 != (isEven ? 0 : 1))
                return false;
            
            if (rangeEnd == 0)
                return input == rangeStart;
            return input >= rangeStart && input <= rangeEnd;
        }

        private static int? ParseStreetNumber(string streetNumber)
        {
            if (string.IsNullOrWhiteSpace(streetNumber))
            {
                return null;
            }
            
            var number = new string(streetNumber.TakeWhile(char.IsDigit).ToArray());
            if (int.TryParse(number, out var integer))
            {
                return integer;
            }

            return null;
        }

        private class StreetRelation
        {
            private StreetRelation()
            {
                StreetNameLocalizationIds = new HashSet<Guid>();
                StreetNumberDictionary = new Dictionary<Guid, StreetNumberRelation>();
            }
            
            public ClsAddressStreet Street { get; set; }
            
            public HashSet<Guid> StreetNameLocalizationIds { get; }
            
            public Dictionary<Guid, StreetNumberRelation> StreetNumberDictionary { get; }

            public static explicit operator ClsAddressStreet(StreetRelation relation)
            {
                return relation?.Street;
            }

            public static explicit operator StreetRelation(ClsAddressStreet street)
            {
                return new StreetRelation {Street = street};
            }
        }

        private class StreetNumberRelation
        {
            private StreetNumberRelation()
            {
                PostalCodeLocalizationIds = new HashSet<Guid>();
            }
            
            public ClsAddressStreetNumber StreetNumber { get; set; }
            
            public HashSet<Guid> PostalCodeLocalizationIds { get; }

            public static explicit operator ClsAddressStreetNumber(StreetNumberRelation relation)
            {
                return relation?.StreetNumber;
            }

            public static explicit operator StreetNumberRelation(ClsAddressStreetNumber streetNumber)
            {
                return new StreetNumberRelation {StreetNumber = streetNumber};
            }
        }
    }
}
