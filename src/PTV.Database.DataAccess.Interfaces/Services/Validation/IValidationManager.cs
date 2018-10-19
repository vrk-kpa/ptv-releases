using System;
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework;

namespace PTV.Database.DataAccess.Interfaces.Services.Validation
{
    public interface IValidationManager
    {
        /// <summary>
        /// Check required fields on id of entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="languageAvailability"></param>
        /// <returns></returns>
        Dictionary<Guid, List<ValidationMessage>> CheckEntity<T>(Guid id, IUnitOfWork unitOfWork,
            ILanguagesAvailabilities languageAvailability = null);

        /// <summary>
        /// Check required fields on entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="languageAvailability"></param>
        /// <returns></returns>
        Dictionary<Guid, List<ValidationMessage>> CheckEntity<T>(T entity,
            ILanguagesAvailabilities languageAvailability = null);


        /// <summary>
        /// Check required fields on entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        Dictionary<Guid, List<ValidationMessage>> CheckEntity<T>(T entity, Guid? languageId);

        /// <summary>
        /// Check required fields on entity by id and language.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="languageId"></param>
        /// <returns></returns>
        Dictionary<Guid, List<ValidationMessage>> CheckEntity<T>(Guid id, IUnitOfWork unitOfWork, Guid? languageId);

    }
}
