using System;
using System.Collections.Generic;

namespace PTV.Database.DataAccess.Interfaces
{
    public interface IDefinitionDeepClause<TEntity> : IDefinitionDeepClause
    {
        DefinitionDeepClause<TLast> Check<TLast>(Func<TEntity, TLast> selector);
        DefinitionDeepClause<TLast> Check<TLast>(Func<TEntity, ICollection<TLast>> selector, Func<TLast, bool> where = null);
        DefinitionDeepClause<TEntity> Is(TEntity value);
        DefinitionDeepClause<TEntity> Not(TEntity value);
    }

    public interface IDefinitionDeepClause { }
}