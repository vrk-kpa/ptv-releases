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
    public abstract class DapperQueryBase
    {
        private readonly StringBuilder builder;

        protected DapperQueryBase(StringBuilder builder)
        {
            this.builder = builder;
        }

        protected void Append(string text)
        {
            builder.Append(text);
        }

        protected void Append(DapperQueryBase expression)
        {
            builder.Append(expression);
        }

        public override string ToString()
        {
            return builder.ToString();
        }

        protected void Limit(int limit, int? offset = null)
        {
            builder.Append($"LIMIT {limit} ");
            if (offset != null)
            {
                builder.Append($"OFFSET {offset}");
            }
        }

        protected static string GetTableNameOrAlias<T>(string tableAlias)
        {
            return tableAlias ?? typeof(T).Name.WithQuotes();
        }

        protected static string GetColumnName<T>(Expression<Func<T, object>> expression)
        {
            var result = string.Empty;
            switch (expression.Body)
            {
                case MemberExpression memberExpression:
                    result = memberExpression.Member.Name;
                    break;
                case UnaryExpression unaryExpression:
                {
                    var operand = unaryExpression.Operand;
                    result = (operand as MemberExpression)?.Member?.Name ?? "";
                    break;
                }
            }

            return result.WithQuotes();
        }

        protected void Embrace(DapperQueryBase innerExpression)
        {
            Append("( ");
            Append(innerExpression.ToString());
            Append(") ");
        }

        protected void And()
        {
            Append("AND ");
        }

        protected void Or()
        {
            Append("OR ");
        }

        protected void Equal<T>(string tableAlias, Expression<Func<T,object>> selector, object expectedValue)
        {
            var tableName = GetTableNameOrAlias<T>(tableAlias);
            var columnName = GetColumnName(selector);
            Append($"{tableName}.{columnName} = {expectedValue} ");
        }

        protected void NotEqual<T>(string tableAlias, Expression<Func<T,object>> selector, object expectedValue)
        {
            var tableName = GetTableNameOrAlias<T>(tableAlias);
            var columnName = GetColumnName(selector);
            Append($"{tableName}.{columnName} != {expectedValue} ");
        }
    }
}
