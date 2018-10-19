using System;
using System.Collections.Generic;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework;

namespace PTV.Database.DataAccess.Interfaces.Services.Validation
{
    public interface IInternalValidation
    {
        /// <summary>
        /// Check required fields on entity by id, language and with validation paths.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="validationPaths"></param>
        /// <param name="languageId"></param>
        /// <param name="entityLanguages"></param>
        /// <returns></returns>
        Dictionary<Guid, List<ValidationMessage>> CheckEntity<T>(T entity, List<ValidationPath> validationPaths,
            Guid? languageId, List<Guid> entityLanguages);

        /// <summary>
        /// Check required fields on entity by id, language and with validation paths.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="unitOfWork"></param>
        /// <param name="validationPaths"></param>
        /// <param name="languageId"></param>
        /// <param name="entityLanguages"></param>
        /// <returns></returns>
        Dictionary<Guid, List<ValidationMessage>> CheckEntity<T>(Guid id, IUnitOfWork unitOfWork, List<ValidationPath> validationPaths, Guid? languageId, List<Guid> entityLanguages);
    }
}
