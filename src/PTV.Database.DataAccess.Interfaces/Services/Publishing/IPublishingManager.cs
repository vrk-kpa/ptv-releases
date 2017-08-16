using System;
using PTV.Database.DataAccess.Interfaces.DbContext;

namespace PTV.Database.DataAccess.Interfaces.Services.Publishing
{
    public interface IPublishingManager
    {
        /// <summary>
        /// Check required fields on entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <param name="unitOfWork"></param>
        /// <returns></returns>
        bool CheckEntity<T>(T entity, Guid id, IUnitOfWork unitOfWork);
    }
}
