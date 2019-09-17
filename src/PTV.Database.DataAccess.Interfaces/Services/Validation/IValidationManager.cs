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
