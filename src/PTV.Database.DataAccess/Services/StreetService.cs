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
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using LinqToDapper;
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

        public IVmSearchBase GetStreets(
            string searchedExpression,
            Guid? postalCodeId,
            Guid languageId,
            int offset,
            bool onlyValid = true)
        {
            var lowerSearchedExpression = searchedExpression?.ToLower();
            var name3 = lowerSearchedExpression.SafeSubstring(0, 3);
            var streetDictionary = new Dictionary<Guid, StreetRelation>();
            var pageSize = CoreConstants.MaximumNumberOfAllItems;

            var countQuery = GetSearchStreetsCount(postalCodeId.HasValue);
            var sqlQuery = GetSearchStreetsSelect(postalCodeId.HasValue, onlyValid, pageSize, offset);

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
                DefaultLanguage = defaultLanguageId
            };

            var totalCount = rawContext.ExecuteReader(db => db.SelectOne<int>(countQuery, commandParams));
            var streets = totalCount < offset
                ? new List<ClsAddressStreet>()
                : rawContext.ExecuteReader(db => db.SelectListWithInclude(sqlQuery, commandParams,
                    GetConnectFunc(streetDictionary), splitOn));

            var translations = TranslationManagerToVm.TranslateAll<ClsAddressStreet, VmStreet>(streets);
            var maxPageCount = (totalCount - 1).PositiveOrZero() / pageSize;
            var pageNumber = (offset - 1).PositiveOrZero() / pageSize;
            var nextOffset = offset + pageSize;

            var returnData = new VmSearchResult<VmStreet>
            {
                SearchResult = translations,
                Count = totalCount,
                Skip = nextOffset,
                MoreAvailable = totalCount > nextOffset,
                // Pages starts at 1, not 0, therefore we are adding +1
                PageNumber = pageNumber + 1,
                MaxPageCount = maxPageCount + 1
            };

            return returnData;
        }

        #region DAPPER SELECTORS
        // A string of column names which decide when mapping to a new object type starts in dapper multi-object mapping
        // https://dapper-tutorial.net/result-multi-mapping#example-query-multi-mapping-one-to-many
        private static string SplitOnColumns =>
            string.Join(",",
                nameof(ClsAddressStreetName.ClsAddressStreetId),
                nameof(ClsAddressStreetNumber.Id),
                nameof(PostalCode.Id),
                nameof(PostalCodeName.PostalCodeId),
                nameof(Municipality.Id));

        private string GetSearchStreetsCount(bool hasPostalCode)
        {
            const string innerSelectAlias = "x";

            var unorderedWithSelect = GetOrderedWithSelect(hasPostalCode);

            var countSelect = DapperSelect<ClsAddressStreet>
                .Count(innerSelectAlias)
                //.CountDistinct(innerSelectAlias, s => s.Id)
                .FromCustom(unorderedWithSelect, innerSelectAlias);

            return countSelect.ToString();
        }

        private string GetSearchStreetsSelect(bool hasPostalCode, bool onlyValid, int limit, int offset)
        {
            const string innerSelectAlias = "x";

            var orderedWithSelect = GetOrderedWithSelect(hasPostalCode, limit, offset);

            var mainSelect = DapperSelect<ClsAddressStreet>
                .SelectDistinct(s => s.Id, s => s.MunicipalityId, s => s.IsValid, s => s.NonCls)
                .ThenSelect<ClsAddressStreetName>(sn => sn.ClsAddressStreetId, sn => sn.LocalizationId, sn => sn.Name,
                    sn => sn.Name3, sn => sn.NonCls)
                .ThenSelect<ClsAddressStreetNumber>(sn => sn.Id, sn => sn.ClsAddressStreetId, sn => sn.PostalCodeId,
                    sn => sn.IsEven, sn => sn.StartNumber, sn => sn.EndNumber, sn => sn.IsValid, sn => sn.NonCls)
                .ThenSelect<PostalCode>(pc => pc.Id, pc => pc.Code, pc => pc.MunicipalityId, pc => pc.IsValid)
                .ThenSelect<PostalCodeName>(pn => pn.PostalCodeId, pn => pn.LocalizationId, pn => pn.Name)
                .ThenSelect<Municipality>(m => m.Id, m => m.Code, m => m.IsValid)
                .From()
                .InnerJoin<ClsAddressStreet>(m => m.Id, s => s.MunicipalityId)
                .With(innerSelectAlias, orderedWithSelect, "Id", null, s => s.Id)
                .InnerJoin<ClsAddressStreetName>(s => s.Id, sn => sn.ClsAddressStreetId)
                .Using<ClsAddressStreet>()
                .InnerJoin<ClsAddressStreetNumber>(s => s.Id, sn => sn.ClsAddressStreetId)
                .InnerJoin<PostalCode>(sn => sn.PostalCodeId, pc => pc.Id)
                .InnerJoin<PostalCodeName>(pc => pc.Id, pcn => pcn.PostalCodeId);

            if (!onlyValid)
            {
                return mainSelect.ToString();
            }

            return mainSelect.Using<ClsAddressStreet>().Where(s => s.IsValid, " = TRUE")
                .Using<ClsAddressStreetNumber>().And(sn => sn.IsValid, " = TRUE")
                .ToString();
        }

        private DapperOrdering<PostalCodeName> GetOrderedWithSelect(bool hasPostalCode, int? limit = null, int? offset = null)
        {
            const string withStreetAlias = "xStr";
            const string orderingStreetNameAlias = "xOrdering";
            const string withPostalCodeNameAlias = "xPCN";
            const string withPostalCodeAlias = "xPC";
            const string withNumberAlias = "xNum";
            const string searchStreetNameAlias = "xSearch";
            const string withCoordinatesAlias = "xCoord";

            var streetNameSelection = GetStreetNameSubquery(withStreetAlias);
            var postalCodeNameSelection = GetPostalCodeNameSubquery(withPostalCodeAlias);

            var withSelect = DapperSelect<ClsAddressStreet>
                .SelectDistinctAs(withStreetAlias, s => s.Id)
                .ThenSelectAs<ClsAddressStreetName>(orderingStreetNameAlias, sn => sn.Name)
                .ThenSelectAs<PostalCodeName>(withPostalCodeNameAlias, pcn => pcn.Name)
                .From(withPostalCodeNameAlias)
                .InnerJoinAs<PostalCode>(withPostalCodeNameAlias, pcn => pcn.PostalCodeId, withPostalCodeAlias,
                    pc => pc.Id)
                .InnerJoinAs<ClsAddressStreetNumber>(withPostalCodeAlias, pc => pc.Id, withNumberAlias,
                    sn => sn.PostalCodeId)
                .InnerJoinAs<ClsAddressStreetName>(withNumberAlias, sn => sn.ClsAddressStreetId, searchStreetNameAlias,
                    sn => sn.ClsAddressStreetId)
                .InnerJoinAs<ClsAddressStreet>(searchStreetNameAlias, sn => sn.ClsAddressStreetId, withStreetAlias,
                    s => s.Id)
                .InnerJoinAs<ClsAddressStreetName>(withStreetAlias, sn => sn.Id, orderingStreetNameAlias,
                    sn => sn.ClsAddressStreetId)
                .Where(searchStreetNameAlias, sn => sn.Name3, " = @Name3")
                .And(q => q.Like(searchStreetNameAlias, s => s.Name, "CONCAT(@SearchedExpression, '%')"))
                .And(q => q.Embrace(e => e.AddCondition(searchStreetNameAlias, sn => sn.NonCls, " = FALSE")
                        .Using<ClsAddressStreetNumber>().And(withNumberAlias, sn => sn.NonCls, " = FALSE"))
                    .Or(w => w.Exists<ClsStreetNumberCoordinate>(coord => coord
                        .From(withCoordinatesAlias)
                        .Where<ClsAddressStreetNumber>(withCoordinatesAlias, c => c.RelatedToId, "=",
                            withNumberAlias, sn => sn.Id))))
                .And(orderingStreetNameAlias, sn => sn.LocalizationId, $" = ({streetNameSelection})")
                .Using<PostalCodeName>()
                .And(withPostalCodeNameAlias, pcn => pcn.LocalizationId, $" = ({postalCodeNameSelection})");

            if (hasPostalCode)
            {
                withSelect = withSelect.Using<PostalCode>().And(withPostalCodeAlias, pc => pc.Id, " = @PostalCode")
                    .Using<PostalCodeName>();
            }

            if (limit == null)
            {
                return new DapperOrdering<PostalCodeName>(withSelect);
            }

            var withOrdering = withSelect.Using<ClsAddressStreetName>().OrderBy(orderingStreetNameAlias, sn => sn.Name)
                .Using<PostalCodeName>().OrderBy(withPostalCodeNameAlias, pcn => pcn.Name)
                .Limit(limit.Value, offset);
            return withOrdering;
        }

        private DapperFrom<PostalCodeName> GetPostalCodeNameSubquery(string withPostalCodeAlias)
        {
            const string nameOrderAlias = "NameOrder";
            const string innerPostalCodeNameAlias = "innerPCN";
            const string innerSubqeryAlias = "xSubquery";

            var postalCodeNameSubquery = DapperSelect<PostalCodeName>
                .SelectAs(innerPostalCodeNameAlias, pcn => pcn.LocalizationId)
                .Case(innerPostalCodeNameAlias, pcn => pcn.LocalizationId, nameOrderAlias,
                    GetLanguageOrderDictionary(), 2)
                .From(innerPostalCodeNameAlias)
                .Where<PostalCode>(innerPostalCodeNameAlias, pcn => pcn.PostalCodeId, " = ", withPostalCodeAlias,
                    pc => pc.Id)
                .OrderByCustom(nameOrderAlias)
                .Limit(1);
            var postalCodeNameSelection = DapperSelect<PostalCodeName>
                .SelectAs(innerSubqeryAlias, pcn => pcn.LocalizationId)
                .FromCustom(postalCodeNameSubquery, innerSubqeryAlias);
            return postalCodeNameSelection;
        }

        private DapperFrom<ClsAddressStreetName> GetStreetNameSubquery(string withStreetAlias)
        {
            const string innerStreetNameAlias = "innerOrdering";
            const string nameOrderAlias = "NameOrder";
            const string innerSubqeryAlias = "xSubquery";

            var streetNameSubquery = DapperSelect<ClsAddressStreetName>
                .SelectAs(innerStreetNameAlias, sn => sn.LocalizationId)
                .Case(innerStreetNameAlias, sn => sn.LocalizationId, nameOrderAlias,
                    GetLanguageOrderDictionary(), 2)
                .From(innerStreetNameAlias)
                .Where<ClsAddressStreet>(innerStreetNameAlias, sn => sn.ClsAddressStreetId, " = ", withStreetAlias,
                    s => s.Id)
                .OrderByCustom(nameOrderAlias)
                .Limit(1);
            var streetNameSelection = DapperSelect<ClsAddressStreetName>
                .SelectAs(innerSubqeryAlias, sn => sn.LocalizationId)
                .FromCustom(streetNameSubquery, innerSubqeryAlias);
            return streetNameSelection;
        }

        private Dictionary<string, int> GetLanguageOrderDictionary()
        {
            return new Dictionary<string, int>
            {
                {"@Language", 0},
                {"@DefaultLanguage", 1}
            };
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

        #endregion

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

        public static int? ParseStreetNumber(string streetNumber)
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
