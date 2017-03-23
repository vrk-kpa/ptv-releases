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
using System.Linq.Expressions;
using System.Threading.Tasks;
using PTV.Database.DataAccess.ApplicationDbContext;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using PTV.Framework;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess
{
    public static partial class EntityFrameworkEntityTools
    {
        public static ContextInfo DataContextInfo;

        private static List<string> uniqualityMemory = new List<string>();

        private static bool IsUniqueName(string text)
        {
            bool exists = uniqualityMemory.Any(i => i == text);
            if (!exists)
            {
                uniqualityMemory.Add(text);
            }
            return !exists;
        }


        /// <summary>
        /// Create indexes on all primary and foreign keys of all entities
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void CreateIndexes(ModelBuilder modelBuilder)
        {
            Random randomGen = new Random();
            var entityTypes = modelBuilder.Model.GetEntityTypes();
            foreach (var entityType in entityTypes)
            {
                var primaryKey = entityType.FindPrimaryKey()?.Properties.ToList();
                if (primaryKey == null || !primaryKey.Any())
                {
                    throw new Exception(string.Format(CoreMessages.EntityHasNoPrimaryKey, entityType.Name));
                }
                var foreignKeys = entityType.GetProperties().SelectMany(entityType.FindForeignKeys).SelectMany(j => j.Properties).ToList();
                var existingIndexes = entityType.GetIndexes().SelectMany(i => i.Properties).Select(i => i.Name).ToList();
                var applyIndexes = primaryKey.Concat(foreignKeys).Distinct().Where(j => !existingIndexes.Contains(j.Name)).ToList();
                if (applyIndexes.Any())
                {
                    entityType.AddIndex(applyIndexes);
                }
                var finalIndexes = entityType.GetIndexes();
                finalIndexes.ForEach(i =>
                {
                    var indexNameParts = i.Npgsql().Name.Split('_');
                    var middleName = indexNameParts[1];
                    var finalName = middleName;
                    int abbChars = 3;
                    do
                    {
                        indexNameParts[1] = CreateAbbreviation(middleName, abbChars++);
                        if (abbChars > 255)
                        {
                            indexNameParts[1] += indexNameParts[1] + randomGen.Next();
                        }
                        finalName = string.Join("_", indexNameParts);
                    } while (!IsUniqueName(finalName));
                    i.Npgsql().Name  = finalName;
                });
            }
        }

        private static string CreateAbbreviation(string text, int charsInAbbr)
        {
            var u = 0;
            string finalName = string.Empty;
            foreach (var s in text)
            {
                if (char.IsLetter(s) && char.IsUpper(s))
                {
                    u = charsInAbbr;
                }
                if (u-- > 0)
                {
                    finalName += s;
                }
            }
            return finalName;
        }


        public static void BuildContextInfo(ModelBuilder modelBuilder)
        {
            var contextInfo = new ContextInfo
            {
                EntityTableName = new Dictionary<Type, string>(),
                Schema = modelBuilder.Model.Relational().DefaultSchema ?? "public"
            };
            var entities = modelBuilder.Model.GetEntityTypes();
            foreach (var entityType in entities)
            {
                var tableName = entityType.Relational().TableName;
                contextInfo.EntityTableName.Add(entityType.ClrType, tableName);
            }
            DataContextInfo = contextInfo;
        }

        public class ContextInfo
        {
            public string Schema;
            public Dictionary<Type, string> EntityTableName;
            public Dictionary<Type, PropertyInfo> EntitySetsMap;
        }



        private static List<PrimaryKeyDefinition> GetPrimaryKeyDefinition(IMutableEntityType entityType)
        {
            var keyProperties = entityType.FindPrimaryKey().Properties;
            var foreignKeys =  entityType.GetForeignKeys().SelectMany(i => i.Properties);
            return keyProperties.Select(i => new PrimaryKeyDefinition()
                {
                    Name = i.Name,
                    Type = i.ClrType,
                    IsPrimaryAndForeign = foreignKeys.Select(j => j.Name).Contains(i.Name)
                }).ToList();
        }

        private static List<CollectionNavigationDefinition> GetCollectionNavigationDefinition(IMutableEntityType entityType)
        {
            List<CollectionNavigationDefinition> collectionNavDefs = new List<CollectionNavigationDefinition>();
            var inverses =
                entityType.GetNavigations().Where(i => i.ClrType.IsCollection()).Select(i => new { i, InverseType = i.FindInverse() }).Where(i => i.InverseType != null).ToList();
            inverses.ForEach(it =>
            {
                var inverseForeignKeys = it.InverseType.ForeignKey.Properties;
                var principalKeys = it.InverseType.ForeignKey.PrincipalKey.Properties;
                var foreignKeyPairs = inverseForeignKeys.Zip(principalKeys,
                    (a, b) =>
                        new ForeignKeyPairDefinition()
                        {
                            DeclaringEntityColumn = new NameTypePairDefinition() {Name = a.Name, Type = a.ClrType},
                            PrincipalEntityColumn = new NameTypePairDefinition() {Name = b.Name, Type = b.ClrType}
                        }).ToList();
                var declaredEntity = it.InverseType.DeclaringEntityType;
                collectionNavDefs.Add(new CollectionNavigationDefinition()
                {
                    Navigation = new NameTypePairDefinition() { Name = it.i.Name, Type = it.i.ClrType },
                    ForeignKeys = foreignKeyPairs,
                    DeclaringEntity = new NameTypePairDefinition() { Name = declaredEntity.Name, Type = declaredEntity.ClrType },
                    PrincipalEntity = new NameTypePairDefinition() { Name = entityType.Name, Type = entityType.ClrType },
                });
            });
            return collectionNavDefs;
        }

        /// <summary>
        /// Build map between Navigation property and its Foreign keys
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <returns>Dictionary with navigation and foreign key map</returns>
        public static Dictionary<Type, EntityPropertiesDefinition> BuildEntityPropertiesMap(ModelBuilder modelBuilder)
        {
            return modelBuilder.Model.GetEntityTypes()
                .ToDictionary(entityType => entityType.ClrType, entityType => new EntityPropertiesDefinition() {
                    EntityType = entityType.ClrType,
                    NavigationsMap =
                    entityType.GetNavigations().Where(i => !i.ClrType.IsCollection())
                    .Select(navigation => new ForeignKeyNavigationPair()
                    {
                        Navigation = new NameTypePairDefinition() { Name = navigation.Name, Type = navigation.ClrType},
                        ForeignKeys = navigation.ForeignKey.Properties.Select(i => new NameTypePairDefinition() { Name = i.Name, Type = i.ClrType}).ToList()
                    }).ToList(),
                    PrimaryKeysDefinition = GetPrimaryKeyDefinition(entityType),
                    CollectionNavigationMap = GetCollectionNavigationDefinition(entityType)
                });
        }

        /// <summary>
        /// Get list of all entities of specific type
        /// </summary>
        /// <typeparam name="TBaseType"></typeparam>
        /// <param name="modelBuilder"></param>
        /// <returns>List of entities that are derived from specific type</returns>
        public static List<Type> GetEntitiesOfBaseType<TBaseType>(ModelBuilder modelBuilder)
        {
            return modelBuilder.Model.GetEntityTypes().Where(i => typeof(TBaseType).GetTypeInfo().IsAssignableFrom(i.ClrType)).Select(i => i.ClrType).ToList();
        }
        public static Dictionary<string, object> GetEntityKey<T>(this DbContext context, T entity) where T : class
        {
            var state = context.Entry(entity);
            var metadata = state.Metadata;
            var key = metadata.FindPrimaryKey();
            return key.Properties.Select(x => new {Name = x.Name, Value = x.GetGetter().GetClrValue(entity)}).ToDictionary(i => i.Name, i=> i.Value);
        }

        public static void Load<TSource, TDestination>(this EntityEntry<TSource> entry, Expression<Func<TSource, ICollection<TDestination>>> path) where TSource : class where TDestination : class
        {
            var entity = entry.Entity;
            var context = entry.Context;
            var keyValues = context.GetEntityKey(entity);
            var query = context.Set<TDestination>() as IQueryable<TDestination>;
            var parameter = Expression.Parameter(typeof(TDestination), "x");
            PropertyInfo foreignKeyProperty = typeof(TDestination).GetProperties().Single(p => p.PropertyType == typeof(TSource));

            foreach (var property in keyValues)
            {
                var expression = Expression.Lambda(
                        Expression.Equal(
                            Expression.Property(Expression.Property(parameter, foreignKeyProperty.Name), property.Key),
                            Expression.Constant(property.Value)),
                        parameter) as Expression<Func<TDestination, bool>>;

                query = query.Where(expression);
            }

            var fetchedData = query.ToList();

            var prop = (path.Body as MemberExpression).Member as PropertyInfo;
            prop.SetValue(entity, fetchedData);
        }

        internal static IQueryable<T> WherePublishingStatusIn<T>(this IQueryable<T> query, List<Guid> publishingStatuses) where T: class, IPublishingStatus
        {
            return query.Where(i => publishingStatuses.Contains(i.PublishingStatusId));
        }
    }
}
