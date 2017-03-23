using System;
using System.Linq;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.Model.Interfaces;

namespace PTV.Database.DataAccess.Interfaces
{
    internal interface IUnitOfWorkCachedSearching : IUnitOfWorkWritable
    {
        T Find<T>(Guid entityId, Func<IQueryable<T>, IQueryable<T>> includeChain = null) where T : class, IEntityIdentifier;
    }
}