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
using System.Threading.Tasks;
using PTV.Framework;
using System.Linq.Expressions;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Domain.Model;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;

namespace PTV.Database.DataAccess.Utils
{
    public static class QueryUtils
    {
        public static VmSearchResult<T> ApplyPaging<T>(this IQueryable<T> query, int pageNumber = 0, int pageSize = CoreConstants.MaximumNumberOfAllItems) where T : class
        {
            var data = query.Skip(pageNumber.PositiveOrZero() * pageSize).Take(pageSize + 1).ToList();
            return new VmSearchResult<T>
            {
                SearchResult = data.Take(pageSize).ToList(),
                MoreAvailable = data.Count > pageSize
            };
        }

        public static VmSearchResult<TOut> ApplyPaging<T, TOut>(this IEnumerable<T> query, Func<IEnumerable<T>, IEnumerable<TOut>> function)
        {
            var entities = function(query).Take(CoreConstants.MaximumNumberOfAllItems + 1).ToList();

            return new VmSearchResult<TOut>
            {
                SearchResult = entities.Take(CoreConstants.MaximumNumberOfAllItems).ToList(),
                MoreAvailable = entities.Count > CoreConstants.MaximumNumberOfAllItems
            };
        }

        public static IEnumerable<TOut> ApplyPagination<T, TOut>(this IEnumerable<T> query, Func<IEnumerable<T>, IEnumerable<TOut>> function, int pageNumber)
        {
            return function(query).Skip(pageNumber.PositiveOrZero() * CoreConstants.MaximumNumberOfAllItems).Take(CoreConstants.MaximumNumberOfAllItems).ToList();
        }


        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int pageNumber)
        {
            return query.Skip(pageNumber.PositiveOrZero() * CoreConstants.MaximumNumberOfAllItems).Take(CoreConstants.MaximumNumberOfAllItems);
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, List<VmSortParam> sortParams)
        {
            IOrderedQueryable<T> orderedQuery = null;
            if (!sortParams.Any())
            {
                return query;
            }

            sortParams.Where(i => i.Column != null).OrderBy(x => x.Order).ForEach(sortParam =>
            {
                var param = Expression.Parameter(typeof(T));
                var memberExpression = Expression.PropertyOrField(param, sortParam.Column);

                if (sortParam.SortDirection == SortDirectionEnum.Desc)
                {
                    if (orderedQuery == null)
                    {
                        #region Expression types

                        if (memberExpression.Type == typeof(DateTime))
                        {
                            orderedQuery = query.OrderByDescending(GetDateTimeExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(DateTime?))
                        {
                            orderedQuery = query.OrderByDescending(GetNullableDateTimeExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(Guid))
                        {
                            orderedQuery = query.OrderByDescending(GetGuidExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(Guid?))
                        {
                            orderedQuery = query.OrderByDescending(GetNullableGuidExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(int))
                        {
                            orderedQuery = query.OrderByDescending(GetIntExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(int?))
                        {
                            orderedQuery = query.OrderByDescending(GetNullableIntExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(bool))
                        {
                            orderedQuery = query.OrderByDescending(GetBoolExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(bool?))
                        {
                            orderedQuery = query.OrderByDescending(GetNullableBoolExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(IEnumerable<int>))
                        {
                        }
                        else
                        {
                            orderedQuery = query.OrderByDescending(GetExpression<T>(param, memberExpression));
                        }

                        #endregion Expression types
                    }
                    else
                    {
                        #region Expression types
                        if (memberExpression.Type == typeof(DateTime))
                        {
                            orderedQuery = orderedQuery.ThenByDescending(GetDateTimeExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(DateTime?))
                        {
                            orderedQuery = orderedQuery.ThenByDescending(GetNullableDateTimeExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(Guid))
                        {
                            orderedQuery = orderedQuery.ThenByDescending(GetGuidExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(Guid?))
                        {
                            orderedQuery = orderedQuery.ThenByDescending(GetNullableGuidExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(int))
                        {
                            orderedQuery = orderedQuery.ThenByDescending(GetIntExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(int?))
                        {
                            orderedQuery = orderedQuery.ThenByDescending(GetNullableIntExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(bool))
                        {
                            orderedQuery = orderedQuery.ThenByDescending(GetBoolExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(bool?))
                        {
                            orderedQuery = orderedQuery.ThenByDescending(GetNullableBoolExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(IEnumerable<int>))
                        {
                        }
                        else
                        {
                            orderedQuery = orderedQuery.ThenByDescending(GetExpression<T>(param, memberExpression));
                        }
                        #endregion Expression types
                    }
                }
                else
                {
                    if (orderedQuery == null)
                    {
                        #region Expression types

                        if (memberExpression.Type == typeof(DateTime))
                        {
                            orderedQuery = query.OrderBy(GetDateTimeExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(DateTime?))
                        {
                            orderedQuery = query.OrderBy(GetNullableDateTimeExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(Guid))
                        {
                            orderedQuery = query.OrderBy(GetGuidExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(Guid?))
                        {
                            orderedQuery = query.OrderBy(GetNullableGuidExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(int))
                        {
                            orderedQuery = query.OrderBy(GetIntExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(int?))
                        {
                            orderedQuery = query.OrderBy(GetNullableIntExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(bool))
                        {
                            orderedQuery = query.OrderBy(GetBoolExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(bool?))
                        {
                            orderedQuery = query.OrderBy(GetNullableBoolExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(IEnumerable<int>))
                        {
                        }
                        else
                        {
                            orderedQuery = query.OrderBy(GetExpression<T>(param, memberExpression));
                        }

                        #endregion Expression types
                    }
                    else
                    {
                        #region Expression types

                        if (memberExpression.Type == typeof(DateTime))
                        {
                            orderedQuery = orderedQuery.ThenBy(GetDateTimeExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(DateTime?))
                        {
                            orderedQuery = orderedQuery.ThenBy(GetNullableDateTimeExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(Guid))
                        {
                            orderedQuery = orderedQuery.ThenBy(GetGuidExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(Guid?))
                        {
                            orderedQuery = orderedQuery.ThenBy(GetNullableGuidExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(int))
                        {
                            orderedQuery = orderedQuery.ThenBy(GetIntExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(int?))
                        {
                            orderedQuery = orderedQuery.ThenBy(GetNullableIntExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(bool))
                        {
                            orderedQuery = orderedQuery.ThenBy(GetBoolExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(bool?))
                        {
                            orderedQuery = orderedQuery.ThenBy(GetNullableBoolExpression<T>(param, memberExpression));
                        }
                        else if (memberExpression.Type == typeof(IEnumerable<int>))
                        {
                        }
                        else
                        {
                            orderedQuery = orderedQuery.ThenBy(GetExpression<T>(param, memberExpression));
                        }

                        #endregion Expression types
                    }
                }
            });

            return orderedQuery;
        }
        public static IQueryable<T> ApplySortingByVersions<T>(this IQueryable<T> query, List<VmSortParam> sortParams, VmSortParam defaultSortParam = null)
        {
            var versionSortingParams = new List<VmSortParam>
            {
                            new VmSortParam { Column = "UnificRootId", SortDirection = SortDirectionEnum.Asc, Order = 997},
                            new VmSortParam { Column = "VersionMajor", SortDirection = SortDirectionEnum.Desc, Order = 998},
                            new VmSortParam { Column = "VersionMinor", SortDirection = SortDirectionEnum.Desc, Order = 999},
                     };

            if (!sortParams.Any() && defaultSortParam == null)
            {
                return query;
            }
            else
            {
                sortParams = sortParams.Any() ? sortParams : new List<VmSortParam> { defaultSortParam };
            }

            return query.ApplySorting(sortParams.Union(versionSortingParams).ToList());
        }

        internal static VmSearchResult<T> LoadByStatusPriority<T>(this IQueryable<T> query, IPublishingStatusCache publishingStatusCache, int pageNumber = 0, int pageSize = CoreConstants.MaximumNumberOfAllItems)
            where T : IVersionedVolume
        {
            var draft = publishingStatusCache.Get(PublishingStatus.Published);
            var modified = publishingStatusCache.Get(PublishingStatus.Published);
            var published = publishingStatusCache.Get(PublishingStatus.Published);
            var result = new List<T>();
            var loadAdditional = 0;

            result.AddRange(query
                .Skip(pageNumber * pageSize)
                .Where(x => x.PublishingStatusId == published || x.PublishingStatusId == draft)
                .Take(pageSize + 1));
            loadAdditional = pageSize - result.Count;

            if (loadAdditional > 0)
            {
                var existingUnificRootIds = result.Select(y => y.UnificRootId).ToList();
                result.AddRange(query
                    .Skip(pageNumber * pageSize)
                    .Where(x => x.PublishingStatusId == modified && !existingUnificRootIds.Contains(x.UnificRootId))
                    .Take(pageSize + 1));
            }

            return new VmSearchResult<T>
            {
                SearchResult = result.Take(pageSize).ToList(),
                MoreAvailable = result.Count > pageSize
            };
        }

        #region Sort Expressions

        private static Expression<Func<T, object>> GetExpression<T>(ParameterExpression param, MemberExpression member)
        {
            return Expression.Lambda<Func<T, object>>(member, param);
        }

        private static Expression<Func<T, Guid>> GetGuidExpression<T>(ParameterExpression param, MemberExpression member)
        {
            return Expression.Lambda<Func<T, Guid>>(member, param);
        }

        private static Expression<Func<T, Guid?>> GetNullableGuidExpression<T>(ParameterExpression param, MemberExpression member)
        {
            return Expression.Lambda<Func<T, Guid?>>(member, param);
        }

        private static Expression<Func<T, int>> GetIntExpression<T>(ParameterExpression param, MemberExpression member)
        {
            return Expression.Lambda<Func<T, int>>(member, param);
        }

        private static Expression<Func<T, int?>> GetNullableIntExpression<T>(ParameterExpression param, MemberExpression member)
        {
            return Expression.Lambda<Func<T, int?>>(member, param);
        }

        private static Expression<Func<T, bool>> GetBoolExpression<T>(ParameterExpression param, MemberExpression member)
        {
            return Expression.Lambda<Func<T, bool>>(member, param);
        }

        private static Expression<Func<T, bool?>> GetNullableBoolExpression<T>(ParameterExpression param, MemberExpression member)
        {
            return Expression.Lambda<Func<T, bool?>>(member, param);
        }

        private static Expression<Func<T, DateTime>> GetDateTimeExpression<T>(ParameterExpression param, MemberExpression member)
        {
            return Expression.Lambda<Func<T, DateTime>>(member, param);
        }

        private static Expression<Func<T, DateTime?>> GetNullableDateTimeExpression<T>(ParameterExpression param, MemberExpression member)
        {
            return Expression.Lambda<Func<T, DateTime?>>(member, param);
        }

        #endregion Sort Expressions
    }
}
