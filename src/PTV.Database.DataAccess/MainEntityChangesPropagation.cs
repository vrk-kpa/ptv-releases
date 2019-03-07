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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Models;
using PTV.Framework;

namespace PTV.Database.DataAccess
{
    internal abstract class MainEntityChangesPropagation
    {
        protected bool finalized = false;

        public abstract void SetFinal(object final);

        public abstract IUnitOfWorkWritable UnitOfWork { get; }
    }
    internal abstract class MainEntityChangesPropagation<TProcessed, TMarked> : MainEntityChangesPropagation where TProcessed : class where TMarked : class
    {
        private readonly TProcessed processedEntity;
        private IEnumerable<TMarked> markedEntities;
        protected readonly IUnitOfWorkWritable unitOfWorkWritable;
        public override IUnitOfWorkWritable UnitOfWork => unitOfWorkWritable;

        protected MainEntityChangesPropagation(IUnitOfWorkWritable unitOfWorkWritable, TProcessed entity)
        {
            this.unitOfWorkWritable = unitOfWorkWritable;
            this.processedEntity = entity;
        }

        public IEnumerable<TMarked> GetMarkedEntities() => markedEntities;

        public override void SetFinal(object final)
        {
            markedEntities = final as IEnumerable<TMarked>;
            if (markedEntities == null)
            {
                throw new Exception("Fatal error in Final object, not convertible to desired target. Fix the Path definition");
            }

            finalized = true;
        }
        
        public MainEntityChangesPropagationPath<TProcessed> CreateDefinition()
        {
            return new MainEntityChangesPropagationPath<TProcessed>(this, processedEntity);
        }

    }

    internal class MainEntityChangesPropagationDefiner<TEntity, TTarget> : MainEntityChangesPropagation<TEntity, TTarget> where TEntity : class where TTarget : class
    {
        public MainEntityChangesPropagationDefiner(IUnitOfWorkWritable unitOfWorkWritable, TEntity entity) : base(unitOfWorkWritable, entity)
        {
        }
    }


    internal interface IEntityModifiedPropagationCreator
    {
        MainEntityChangesPropagationDefiner<TEntity, TTarget> Create<TEntity, TTarget>(IUnitOfWorkWritable unitOfWorkWritable, TEntity entity) where TEntity : class where TTarget : class;
    }

    [RegisterService(typeof(IEntityModifiedPropagationCreator), RegisterType.Singleton)]
    internal class EntityModifiedPropagationCreator : IEntityModifiedPropagationCreator
    {
        public MainEntityChangesPropagationDefiner<TEntity, TTarget> Create<TEntity, TTarget>(IUnitOfWorkWritable unitOfWorkWritable, TEntity entity) where TEntity : class where TTarget : class
        {
            return new MainEntityChangesPropagationDefiner<TEntity, TTarget>(unitOfWorkWritable, entity);
        }
    }

    internal class MainEntityChangesPropagationPath<TEntity> : IMainEntityChangesPropagationPath<TEntity> where TEntity : class
    {
        private readonly IEnumerable<TEntity> entities;
        private readonly MainEntityChangesPropagation holder;
        
        public MainEntityChangesPropagationPath(MainEntityChangesPropagation holder, IEnumerable<TEntity> entities)
        {
            this.holder = holder;
            this.entities = entities;
        }
        
        public MainEntityChangesPropagationPath(MainEntityChangesPropagation holder, TEntity entity)
        {
            this.holder = holder;
            this.entities = new List<TEntity>() { entity };
        }
        
        public IMainEntityChangesPropagationPath<T> AddPath<T>(Expression<Func<TEntity, T>> path) where T : class
        {
            var entity = entities.FirstOrDefault();
            var propValue = path.Compile()(entity);
            if (propValue == null)
            {
                holder.UnitOfWork.LoadNavigation(entity, path);
                propValue = path.Compile()(entity);
            }
            return new MainEntityChangesPropagationPath<T>(holder, propValue);
        }
        
        public IMainEntityChangesPropagationPath<T> AddPath<T>(Expression<Func<TEntity, ICollection<T>>> path) where T : class
        {
            var entity = entities.FirstOrDefault();
            var propValue = path.Compile()(entity);
            if (propValue == null)
            {
                holder.UnitOfWork.LoadCollection(entity, path);
                propValue = path.Compile()(entity);
            }
            return new MainEntityChangesPropagationPath<T>(holder, propValue);
        }

        public void Final()
        {
            holder.SetFinal(entities ?? new List<TEntity>());
        }
    }
}