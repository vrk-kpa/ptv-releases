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
using System.Linq.Expressions;
using System.Text;

namespace LinqToDapper
{
    public class DapperSelect<T> : DapperQueryBase
    {
        internal DapperSelect(string queryStart = "")
            : base(new StringBuilder(queryStart))
        {
        }

        public new DapperSelect<T> Append(string query)
        {
            base.Append(query);
            return this;
        }
        
        public DapperSelect<T> Embrace(Func<DapperSelect<T>, DapperSelect<T>> innerExpression)
        {
            base.Embrace(innerExpression(this));
            return this;
        }

        public DapperSelect<TOther> ThenSelect<TOther>(params Expression<Func<TOther, object>>[] selectors)
        {
            Append(", ");
            return SelectInternal(this, null, selectors);
        }

        public DapperSelect<TOther> ThenSelectAs<TOther>(string tableAlias, params Expression<Func<TOther, object>>[] selectors)
        {
            Append(", ");
            return SelectInternal(this, tableAlias, selectors);
        }

        public DapperFrom<T> From(string tableAlias = null)
        {
            var tableName = typeof(T).Name.WithQuotes();
            Append($"FROM {tableName} ");
            if (tableAlias != null)
            {
                Append($"AS {tableAlias} ");
            }

            return new DapperFrom<T>(this);
        }

        public DapperFrom<T> FromCustom(DapperQueryBase innerDataSource, string dataSourceAlias)
        {
            Append($"FROM ({innerDataSource}) AS {dataSourceAlias}");
            return new DapperFrom<T>(this);
        }
        
        public static DapperSelect<T> Select(params Expression<Func<T, object>>[] selectors)
        {
            var query = new DapperSelect<T>("SELECT ");
            return SelectInternal(query, null, selectors);
        }
        
        public static  DapperSelect<T> SelectAs(string tableAlias, params Expression<Func<T, object>>[] selectors)
        {
            var query = new DapperSelect<T>("SELECT ");
            return SelectInternal(query, tableAlias, selectors);
        }
        
        public static  DapperSelect<T> SelectDistinct(params Expression<Func<T, object>>[] selectors)
        {
            var query = new DapperSelect<T>("SELECT DISTINCT ");
            return SelectInternal(query, null, selectors);
        }
        
        public static  DapperSelect<T> SelectDistinctAs(string tableAlias, params Expression<Func<T, object>>[] selectors)
        {
            var query = new DapperSelect<T>("SELECT DISTINCT ");
            return SelectInternal(query, tableAlias, selectors);
        }

        public static DapperSelect<T> SelectCustom(bool distinct, params string[] customColumns)
        {
            var query = new DapperSelect<T>(distinct ? "SELECT DISTINCT " : "SELECT ");
            query.Append(string.Join(", ", customColumns));
            query.Append(" ");
            return query;
        }

        public DapperSelect<T> ThenSelectCustom(params string[] customColumns)
        {
            Append(string.Join(", ", customColumns));
            Append(" ");
            return this;
        }

        public DapperSelect<T> Case<TCondition, TValue>(Expression<Func<T, object>> key, string caseAlias, Dictionary<TCondition, TValue> options, TValue defaultOption = default(TValue))
        {
            return Case(null, key, caseAlias, options, defaultOption);
        }

        public DapperSelect<T> Case<TCondition, TValue>(string tableAlias, Expression<Func<T, object>> key, string caseAlias, Dictionary<TCondition, TValue> options, TValue defaultOption = default(TValue))
        {
            var tableName = GetTableNameOrAlias<T>(tableAlias);
            var columnName = GetColumnName(key);

            Append($", CASE {tableName}.{columnName} ");
            foreach (var entry in options)
            {
                Append($"WHEN {entry.Key} THEN {entry.Value} ");
            }

            if (defaultOption != null && !defaultOption.Equals(default(TValue)))
            {
                Append($"ELSE {defaultOption} ");
            }

            Append($"END AS {caseAlias} ");
            return this;
        }

        public static DapperSelect<T> Count(string tableAlias = null)
        {
            var tableNameOrAlias = GetTableNameOrAlias<T>(tableAlias);
            
            var query = new DapperSelect<T>($"SELECT COUNT({tableNameOrAlias}.*) ");
            return query;
        }
        
        private static DapperSelect<TNext> SelectInternal<TPrevious, TNext>(DapperSelect<TPrevious> previousQuery, string tableAlias, params Expression<Func<TNext, object>>[] selectors)
        {
            var query = new DapperSelect<TNext>(previousQuery.ToString());
            var columns = new List<string>();
            
            foreach (var selector in selectors)
            {
                var columnName = GetColumnName(selector);
                var tableName = GetTableNameOrAlias<TNext>(tableAlias);
                var fullColumnName = $"{tableName}.{columnName} ";
                columns.Add(fullColumnName);
            }
            
            query.Append(string.Join(", ", columns));
            return query;
        }
    }
}
