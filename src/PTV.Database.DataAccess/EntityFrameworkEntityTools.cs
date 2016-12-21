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
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess
{
    public static class EntityFrameworkEntityTools
    {
        public static ContextInfo DataContextInfo;

        /// <summary>
        /// Create indexes on all primary and foreign keys of all entities
        /// </summary>
        /// <param name="modelBuilder"></param>
        public static void CreateIndexes(ModelBuilder modelBuilder)
        {
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
            }
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

        /// <summary>
        /// Holder class for relations between navigation and foreign key
        /// </summary>
        public class ForeignKeyNavigationPair
        {
            public List<NameTypePairDefinition> ForeignKeys;
            public NameTypePairDefinition Navigation;
        }

        public class CollectionNavigationDefinition : ForeignKeyNavigationPair
        {
            public NameTypePairDefinition DeclaringEntity;
            public NameTypePairDefinition PrincipalEntity;
        }

        public class EntityPropertiesDefinition
        {
            public List<ForeignKeyNavigationPair> NavigationsMap;
            public List<PrimaryKeyDefinition> PrimaryKeysDefinition;
            public List<CollectionNavigationDefinition> CollectionNavigationMap;
        }

        public class PrimaryKeyDefinition : NameTypePairDefinition
        {
            public bool IsPrimaryAndForeign;
        }

        public class NameTypePairDefinition
        {
            public string Name;
            public Type Type;
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
                var declaredEntity = it.InverseType.DeclaringEntityType;
                collectionNavDefs.Add(new CollectionNavigationDefinition()
                {
                    Navigation = new NameTypePairDefinition() { Name = it.i.Name, Type = it.i.ClrType },
                    ForeignKeys = inverseForeignKeys.Select(i => new NameTypePairDefinition() { Name = i.Name, Type = i.ClrType }).ToList(),
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
                    NavigationsMap =
                    entityType.GetNavigations()
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
            var inList = publishingStatuses.Cast<Guid?>().ToList();
            return query.Where(i => inList.Contains(i.PublishingStatusId));
        }
    }
}
