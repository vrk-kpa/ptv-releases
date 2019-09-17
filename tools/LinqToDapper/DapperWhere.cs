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
using System.Linq.Expressions;
using System.Text;

namespace LinqToDapper
{
    public class DapperWhere<T> : DapperQueryBase
    {
        private DapperWhere() : base(new StringBuilder()) { }
        
        public DapperWhere(DapperQueryBase previousQuery) : base(new StringBuilder(previousQuery.ToString()))
        {
        }

        public DapperWhere<T> Embrace<TOther>(Func<DapperWhere<T>, DapperWhere<TOther>> innerExpression)
        {
            base.Embrace(innerExpression(new DapperWhere<T>()));
            return this;
        }
        
        public DapperWhere<TOther> Using<TOther>()
        {
            return new DapperWhere<TOther>(this);
        }
        
        public DapperWhere<T> AddCondition(string tableAlias, Expression<Func<T, object>> key, string condition, 
            string booleanJoin = null)
        {
            var tableName = GetTableNameOrAlias<T>(tableAlias);
            var columnName = GetColumnName(key);
            
            Append($"{booleanJoin} {tableName}.{columnName} {condition} ");
            
            return new DapperWhere<T>(this);
        }

        public DapperWhere<T> AddCondition<TOther>(string firstAlias, Expression<Func<T, object>> firstKey, 
            string condition, string secondAlias, Expression<Func<TOther, object>> secondKey, string booleanJoin = null)
        {
            var firstTableOrAlias = GetTableNameOrAlias<T>(firstAlias);
            var secondTableOrAlias = GetTableNameOrAlias<TOther>(secondAlias);

            var firstMember = GetColumnName(firstKey);
            var secondMember = GetColumnName(secondKey);
            
            Append($"{booleanJoin} {firstTableOrAlias}.{firstMember} {condition} {secondTableOrAlias}.{secondMember} ");
            
            return new DapperWhere<T>(this);
        }

        public DapperWhere<T> And(Expression<Func<T, object>> key, string condition)
        {
            return And(null, key, condition);
        }

        public DapperWhere<T> And(string tableAlias, Expression<Func<T, object>> key, string condition)
        {
            return AddCondition(tableAlias, key, condition, "AND");
        }

        public DapperWhere<T> And<TOther>(string firstAlias, Expression<Func<T, object>> firstKey, string condition,
            string secondAlias, Expression<Func<TOther, object>> secondKey)
        {
            return AddCondition(firstAlias, firstKey, condition, secondAlias, secondKey, "AND");
        }

        public DapperWhere<T> And(Func<DapperWhere<T>, DapperWhere<T>> innerCondition)
        {
            Append("AND ( ");
            Append(innerCondition(new DapperWhere<T>()).ToString());
            Append(") ");
            return this;
        }

        public DapperWhere<T> Or(Expression<Func<T, object>> key, string condition)
        {
            return Or(null, key, condition);
        }

        public DapperWhere<T> Or(string tableAlias, Expression<Func<T, object>> key, string condition)
        {
            return AddCondition(tableAlias, key, condition, "OR");
        }

        public DapperWhere<T> Or<TOther>(string firstAlias, Expression<Func<T, object>> firstKey, string condition,
            string secondAlias, Expression<Func<TOther, object>> secondKey)
        {
            return AddCondition(firstAlias, firstKey, condition, secondAlias, secondKey, "OR");
        }

        public DapperWhere<T> Or(Func<DapperWhere<T>, DapperWhere<T>> innerCondition)
        {
            Append("OR ( ");
            Append(innerCondition(new DapperWhere<T>()).ToString());
            Append(") ");
            return this;
        }

        public DapperWhere<T> Exists<TOther>(Func<DapperSelect<TOther>, DapperQueryBase> query)
        {
            var existsQuery = query(new DapperSelect<TOther>("SELECT 1 ")).ToString();
            Append($"EXISTS ({existsQuery})");
            return this;
        }

        public DapperWhere<T> Like(string tableAlias, Expression<Func<T, object>> key, string pattern)
        {
            var tableName = GetTableNameOrAlias<T>(tableAlias);
            var columnName = GetColumnName(key);
            
            Append($"LOWER({tableName}.{columnName}) LIKE {pattern} ");
            return this;
        }

        public DapperOrdering<T> OrderBy(string tableAlias, Expression<Func<T, object>> key, bool descending = false)
        {
            var tableName = GetTableNameOrAlias<T>(tableAlias);
            var columnName = GetColumnName(key);
            var direction = descending ? "DESC" : "ASC";
            
            Append($"ORDER BY {tableName}.{columnName} {direction} ");
            return new DapperOrdering<T>(this);
        }

        public DapperOrdering<T> OrderByCustom(string customColumn, bool descending = false)
        {
            var direction = descending ? "DESC" : "ASC";
            Append($"ORDER BY {customColumn} {direction} ");
            return new DapperOrdering<T>(this);
        }
    }
}
