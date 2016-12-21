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
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.DataAccess.Translators;
using PTV.Database.Model.Models;
using PTV.Framework.Interfaces;
using PTV.Framework;

namespace PTV.Database.DataAccess.EntityCloners
{
    /// <summary>
    /// Abstract base class for "cloner", ie. definitions for creating copy of entity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EntityCloner<TEntity> : IEntityCloner<TEntity> where TEntity : class, new()
    {
        private ITranslationUnitOfWork unitOfWork;
        private readonly EntityNavigationsMap entityNavigationsMap;
        private TEntity target;
        private TEntity source;
        private readonly IResolveManager resolveManager;
        private readonly EntityFrameworkEntityTools.EntityPropertiesDefinition entityPropsDefs;
        private readonly List<string> affectedForeignKeys = new List<string>();
        private DbSet<TEntity> entitySet;
        private readonly List<EntityFrameworkEntityTools.NameTypePairDefinition> foreignKeys;
        private readonly string primaryKeyName;

        /// <summary>
        /// Mehod defining list of properties which should be cloned
        /// </summary>
        public abstract void CloningDefinition();

        protected EntityCloner(IResolveManager resolveManager, EntityNavigationsMap entityNavigationsMap)
        {
            this.entityNavigationsMap = entityNavigationsMap;
            this.entityPropsDefs = entityNavigationsMap.NavigationsMap[typeof(TEntity)];
            this.resolveManager = resolveManager;
            this.foreignKeys = entityPropsDefs.NavigationsMap.SelectMany(i => i.ForeignKeys).ToList();
            this.primaryKeyName = entityPropsDefs.PrimaryKeysDefinition.First().Name;
        }

        /// <summary>
        /// Perform cloning of entity by calling spcified definitions
        /// </summary>
        /// <param name="unitOfWork">Unit of work instance for creating requests to DB</param>
        /// <param name="entity">Instance of entity which will be cloned</param>
        /// <param name="beforeAddAction">Optional additional action which can be called before entity is added to db set</param>
        /// <returns>Copy of entity</returns>
        public TEntity PerformCloning(ITranslationUnitOfWork unitOfWork, TEntity entity, Action<TEntity> beforeAddAction = null)
        {
            if (entity == null)
            {
                return null;
            }
            this.unitOfWork = unitOfWork;
            if (this.entitySet == null)
            {
                this.entitySet = unitOfWork.GetSet<TEntity>();
            }
            source = entity;
            target = new TEntity();
            BaseTranslator.TranslateWithExceptions(entity, target, foreignKeys.Select(i => i.Name));
            entityPropsDefs.PrimaryKeysDefinition.Where(i => !i.IsPrimaryAndForeign && i.Type == typeof(Guid)).ForEach(i =>
            {
                target.SetPropertyValue(i.Name, Guid.NewGuid());
            });
            CloningDefinition();
            BaseTranslator.TranslateProperties(entity, target, foreignKeys.Select(i => i.Name).Except(affectedForeignKeys));
            beforeAddAction?.Invoke(target);
            entitySet.Add(target);
            return target;
        }

        /// <summary>
        /// Definition specifying which property (navigation) of ICollection type (many of one-to-many side) will be cloned
        /// </summary>
        /// <typeparam name="TInnerType">Inner type of generic collection</typeparam>
        /// <param name="clonedProperty">Property selector which property should be cloned</param>
        /// <returns>The self instance</returns>
        protected EntityCloner<TEntity> AddClone<TInnerType>(Expression<Func<TEntity, ICollection<TInnerType>>> clonedProperty) where TInnerType : class, new()
        {
            var propertyName = ((clonedProperty.Body as MemberExpression)?.Member as PropertyInfo)?.Name;
            var collectionDefinition = entityPropsDefs.CollectionNavigationMap.First(i => i.Navigation.Name == propertyName);
            var propertyForeignKeyName = collectionDefinition.ForeignKeys.First().Name;
            var currentEntityPrimaryKeyName = entityPropsDefs.PrimaryKeysDefinition.First().Name;
            var currentId = source.GetPropertyObjectValue(currentEntityPrimaryKeyName);
            var selectCondition = CoreExtensions.CreateLambdaEqual<TInnerType>(propertyForeignKeyName, currentId);
            var propertyEntitySet = unitOfWork.GetSet<TInnerType>().AsNoTracking();
            var instancesToClone = propertyEntitySet.Where(selectCondition).ToList();
            var propertyCloner = resolveManager.Resolve<IEntityCloner<TInnerType>>();
            target.SetPropertyValue(clonedProperty, instancesToClone.Select(i => propertyCloner.PerformCloning(unitOfWork, i,
                tg => tg.SetPropertyValue(propertyForeignKeyName, target.GetPropertyObjectValue(primaryKeyName)))).ToList());
            return this;
        }

        /// <summary>
        /// Definition specifying which property (navigation) will be cloned
        /// </summary>
        /// <typeparam name="TProperty">Type of property, ie. entity type of oposite side</typeparam>
        /// <param name="clonedProperty">Property selector which property should be cloned</param>
        /// <returns>The self instance</returns>
        protected EntityCloner<TEntity> AddClone<TProperty>(Expression<Func<TEntity, TProperty>> clonedProperty) where TProperty : class, new()
        {
            var propertyName = ((clonedProperty.Body as MemberExpression)?.Member as PropertyInfo)?.Name;
            var foreignKeysForNavigation = entityPropsDefs.NavigationsMap.First(i => i.Navigation.Name == propertyName).ForeignKeys;
            if (foreignKeysForNavigation.IsNullOrEmpty()) return this;
            affectedForeignKeys.AddRange(foreignKeysForNavigation.Select(i => i.Name));
            var foreignKeyValue = source.GetPropertyObjectValue(foreignKeysForNavigation.First().Name);
            if (foreignKeyValue == null) return this;
            var propertyPrimaryKeyName = entityNavigationsMap.NavigationsMap[typeof(TProperty)].PrimaryKeysDefinition.First().Name;
            var propertyEntitySet = unitOfWork.GetSet<TProperty>().AsNoTracking();
            var selectCondition = CoreExtensions.CreateLambdaEqual<TProperty>(propertyPrimaryKeyName, foreignKeyValue);
            var entityOfProperty = propertyEntitySet.Where(selectCondition).FirstOrDefault();
            var propertyCloner = resolveManager.Resolve<IEntityCloner<TProperty>>();
            target.SetPropertyValue(clonedProperty, propertyCloner.PerformCloning(unitOfWork, entityOfProperty));
            return this;
        }
    }

    [RegisterService(typeof(IEntityCloner<Service>), RegisterType.Transient)]
    internal class ServiceCloner : EntityCloner<Service>
    {
        public ServiceCloner(IResolveManager resolveManager, EntityNavigationsMap entityNavigationsMap) : base(resolveManager, entityNavigationsMap)
        {
        }

        public override void CloningDefinition()
        {
            AddClone(i => i.PublishingStatus);
            //AddClone(i => i.ServiceCoverageType);
            AddClone(i => i.ServiceNames);
        }
    }


    [RegisterService(typeof(IEntityCloner<PublishingStatusType>), RegisterType.Transient)]
    internal class PublishingStatusCloner : EntityCloner<PublishingStatusType>
    {
        public PublishingStatusCloner(IResolveManager resolveManager, EntityNavigationsMap entityNavigationsMap) : base(resolveManager, entityNavigationsMap)
        {
        }

        public override void CloningDefinition()
        {
        }
    }

    [RegisterService(typeof(IEntityCloner<ServiceCoverageType>), RegisterType.Transient)]
    internal class ServiceCoverageTypeCloner : EntityCloner<ServiceCoverageType>
    {
        public ServiceCoverageTypeCloner(IResolveManager resolveManager, EntityNavigationsMap entityNavigationsMap) : base(resolveManager, entityNavigationsMap)
        {
        }

        public override void CloningDefinition()
        {
        }
    }

    [RegisterService(typeof(IEntityCloner<ServiceName>), RegisterType.Transient)]
    internal class ServiceNameCloner : EntityCloner<ServiceName>
    {
        public ServiceNameCloner(IResolveManager resolveManager, EntityNavigationsMap entityNavigationsMap) : base(resolveManager, entityNavigationsMap)
        {
        }

        public override void CloningDefinition()
        {
        }
    }
}
