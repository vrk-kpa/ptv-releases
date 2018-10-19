using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PTV.Database.DataAccess.Interfaces
{
    public interface IMainEntityChangesPropagationPath<TEntity> where TEntity : class
    {
        IMainEntityChangesPropagationPath<T> AddPath<T>(Expression<Func<TEntity, T>> path) where T : class;
        IMainEntityChangesPropagationPath<T> AddPath<T>(Expression<Func<TEntity, ICollection<T>>> path) where T : class;
        void Final();
    }
}