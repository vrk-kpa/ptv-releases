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
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql;

namespace PTV.Database.DataAccess.ApplicationDbContext
{
    /// <summary>
    /// Application database context extensions
    /// </summary>
    internal static class PtvDbContextExtensions
    {
        const int MaxRetriesAttempts = 3;

        public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = entity.Name.Split('.').Last();
            }
        }

        public static bool EnsureCreated(this PtvDbContext context)
        {
            int retries = 0;
            Exception ex = new Exception("Max retries exceeded");
            while (retries++ < MaxRetriesAttempts)
            {
                try
                {
                    return context.Database.EnsureCreated();
                }
                catch (Exception e)
                {
                    ex = new Exception($"Max retries exceeded, HashCode {context.GetHashCode()}, Original message {e.Message}");
                    try
                    {
                        context.Database.CloseConnection();
                    }
                    catch
                    {
                    }
                    try
                    {
                        context.Database.OpenConnection();
                    }
                    catch
                    {
                    }
                }
            }
            throw ex;
        }

        public static Dictionary<Type, PropertyInfo> BuildSetMap(this DbContext context)
        {
            return context.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(i => i.PropertyType.GetTypeInfo().IsGenericType && i.PropertyType.GetTypeInfo().GetGenericTypeDefinition() == typeof(DbSet<>)).ToDictionary(i => i.PropertyType.GetTypeInfo().GetGenericArguments().First(), i => i);
        }

        /// <summary>
        /// Get entitySet name (original db name of table)
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="context">object context</param>
        /// <returns></returns>
        public static string GetEntitySetName<T>(this DbContext context)
        {
            var table = context.Model.FindEntityType(typeof(T)).Npgsql();
            return table.TableName;
        }


        private static List<Type> batchInsertTypes = new List<Type>()
        {
            typeof(int),
            typeof(int?),
            typeof(decimal),
            typeof(decimal?),
            typeof(double),
            typeof(double?),
            typeof(string),
            typeof(DateTime),
            typeof(DateTime?),
            typeof(Guid),
            typeof(Guid?),
            typeof(bool),
            typeof(bool?)
        };

        private const int maxParams = 2000; // SQL server limitation

        /// <summary>
        /// Insert list of entities in batches. Note: Only primitive types of properties are processed, navigation properties are ignored.
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="dbContext">DbContext</param>
        /// <param name="batch">List of entities to be inserted</param>
        public static void BatchInsert<T>(this DbContext dbContext, IEnumerable<T> batch) where T : class
        {
            var tableName = dbContext.GetEntitySetName<T>();

            var entityType = typeof(T);
            var entityProperties = entityType.GetProperties().Where(i => batchInsertTypes.Contains(i.PropertyType)).ToList();
            int paramIndex = 0;
            List<object> paramValues = new List<object>();
            StringBuilder batchInsertCmdBuilder = new StringBuilder();
            int batchSize = maxParams / entityProperties.Count;
            int processed = 0;
            foreach (var item in batch)
            {
                processed++;
                var lineCmd = "INSERT INTO \"{0}\" ({1}) VALUES ({2});";
                var columnsList = entityProperties.Select(prop => $"\"{prop.Name}\"").ToList();
                var paramList = new List<string>();
                for (int i = 0; i < entityProperties.Count; i++)
                {
                    paramList.Add("@p" + paramIndex++ + "");
                }
                paramValues.AddRange(entityProperties.Select(prop => prop.GetValue(item)));
                var insertCmd = string.Format(lineCmd, tableName, string.Join(",", columnsList), string.Join(",", paramList));
                batchInsertCmdBuilder.AppendLine(insertCmd);
                if (processed >= batchSize)
                {
                    ProcessCmd(dbContext, batchInsertCmdBuilder, paramValues);
                    processed = 0;
                    batchInsertCmdBuilder.Clear();
                    paramValues.Clear();
                    paramIndex = 0;
                }
            }
            ProcessCmd(dbContext, batchInsertCmdBuilder, paramValues);
        }

        private static bool IsVirtual(this PropertyInfo self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            bool? found = null;
            foreach (MethodInfo method in self.GetAccessors())
            {
                if (found.HasValue)
                {
                    if (found.Value != method.IsVirtual)
                        return false;
                }
                else
                {
                    found = method.IsVirtual;
                }
            }
            return found ?? false;
        }

        private static void ProcessCmd(DbContext dbContext, StringBuilder batchInsertCmdBuilder, List<object> paramValues)
        {
            var directCmd = batchInsertCmdBuilder.ToString();
            if (!string.IsNullOrEmpty(directCmd))
            {
                dbContext.Database.ExecuteSqlCommand(directCmd, paramValues.ToArray());
            }
        }

        /// <summary>
        /// Batch delete according to the specified expresion(column) and its value
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="dbContext">db context</param>
        /// <param name="e">expression</param>
        /// <param name="value">value</param>
        public static void DeleteWhere<T>(this DbContext dbContext, Expression<Func<T, object>> e, object value) where T : class
        {
            var tableName = dbContext.GetEntitySetName<T>();

            MemberExpression body = e.Body as MemberExpression ?? ((UnaryExpression)e.Body).Operand as MemberExpression;
            if (body == null)
            {
                return;
            }

            string propertyName = body.Member.Name;

            var listOfValuesInt32 = value as IEnumerable<int>;
            var listOfValuesInt64 = value as IEnumerable<Int64>;
            var listOfValuesString = value as IEnumerable<string>;
            var listOfValuesGuid = value as IEnumerable<Guid>;

            if (listOfValuesInt32 != null || listOfValuesInt64 != null || listOfValuesString != null || listOfValuesGuid != null)
            {
                string deleteSql = "DELETE FROM \"{0}\" WHERE \"{1}\" IN ({2})";
                IEnumerable<string> listOfValues = listOfValuesInt32?.Select(i => i.ToString()).ToList() ??
                                                   (listOfValuesInt64?.Select(i => i.ToString()).ToList() ??
                                                   (listOfValuesGuid?.Select(i => $"'{i}'").ToList() ??
                                                   (listOfValuesString ?? new List<string>())));
                string paramSide = String.Join(",", listOfValues.ToArray());
                if (!string.IsNullOrEmpty(paramSide))
                {
                    deleteSql = string.Format(deleteSql, tableName, propertyName, paramSide);
                    dbContext.Database.ExecuteSqlCommand(deleteSql);
                }
            }
            else
            {
                string deleteSql = "DELETE FROM \"{0}\" WHERE \"{1}\" = @value";
                deleteSql = string.Format(deleteSql, tableName, propertyName);
                var parameter = new NpgsqlParameter("value", value);
                dbContext.Database.ExecuteSqlCommand(deleteSql, parameter);
            }
        }

        public static void RegisterDbSetsEntityTypes(this DbContext dbContext)
        {
            var dbSets = dbContext.BuildSetMap();
            var a = dbSets;
        }
    }
}
