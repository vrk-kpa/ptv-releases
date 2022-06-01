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
using System.Linq.Expressions;
using System.Text;

namespace LinqToDapper
{
    public class DapperFrom<T> : DapperQueryBase
    {
        public DapperFrom(DapperQueryBase previousQuery) 
            : base(new StringBuilder(previousQuery.ToString()))
        {
        }

        public DapperFrom<T> Embrace(Func<DapperFrom<T>, DapperFrom<T>> innerExpression)
        {
            base.Embrace(innerExpression(this));
            return this;
        }
        
        public DapperFrom<TOther> InnerJoin<TOther>(Expression<Func<T, object>> firstKey,
            Expression<Func<TOther, object>> secondKey)
        {
            return InnerJoinAs(null, firstKey, null, secondKey);
        }

        public DapperFrom<TOther> InnerJoinAs<TOther>(string firstAlias, Expression<Func<T, object>> firstKey,
            string secondAlias, Expression<Func<TOther, object>> secondKey)
        {
            var firstTableOrAlias = GetTableNameOrAlias<T>(firstAlias);
            var secondTable = GetTableNameOrAlias<TOther>(null);

            var firstMember = GetColumnName(firstKey);
            var secondMember = GetColumnName(secondKey);

            Append(string.IsNullOrWhiteSpace(secondAlias)
                ? $"INNER JOIN {secondTable} ON {firstTableOrAlias}.{firstMember} = {secondTable}.{secondMember} "
                : $"INNER JOIN {secondTable} AS {secondAlias} ON {firstTableOrAlias}.{firstMember} = {secondAlias}.{secondMember} ");

            return new DapperFrom<TOther>(this);
        }

        public DapperFrom<TOther> Using<TOther>()
        {
            return new DapperFrom<TOther>(this);
        }

        public DapperWhere<T> Where(Expression<Func<T, object>> key, string condition)
        {
            return Where(null, key, condition);
        }

        public DapperWhere<T> Where(string tableAlias, Expression<Func<T, object>> key, string condition)
        {
            var tableName = GetTableNameOrAlias<T>(tableAlias);
            var columnName = GetColumnName(key);
            
            Append($"WHERE {tableName}.{columnName} {condition} ");
            
            return new DapperWhere<T>(this);
        }

        public DapperWhere<T> Where<TOther>(string firstAlias, Expression<Func<T, object>> firstKey, string condition, string secondAlias, Expression<Func<TOther, object>> secondKey)
        {
            var firstTableOrAlias = GetTableNameOrAlias<T>(firstAlias);
            var secondTableOrAlias = GetTableNameOrAlias<TOther>(secondAlias);

            var firstMember = GetColumnName(firstKey);
            var secondMember = GetColumnName(secondKey);
            
            Append($"WHERE {firstTableOrAlias}.{firstMember} {condition} {secondTableOrAlias}.{secondMember} ");
            
            return new DapperWhere<T>(this);
        }

        public DapperFrom<T> With(string withAlias, DapperQueryBase withQuery, string withKey, string tableAlias, Expression<Func<T, object>> entityKey)
        {
            var tableName = GetTableNameOrAlias<T>(tableAlias);
            var columnName = GetColumnName(entityKey);
            
            var with = new DapperSelect<T>($"WITH {withAlias} AS ( ");
            with.Append(withQuery.ToString());
            with.Append(") ");
            with.Append(this.ToString());
            with.Append($"JOIN {withAlias} ON {withAlias}.{withKey.WithQuotes()} = {tableName}.{columnName} ");
            return new DapperFrom<T>(with);
        }
    }
}
