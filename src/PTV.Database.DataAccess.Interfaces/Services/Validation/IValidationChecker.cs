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
    internal interface IValidationCheckerBase
    {
        /// <summary>
        /// Validate fields on entity.
        /// </summary>
        /// <returns></returns>
        Dictionary<Guid, List<ValidationMessage>> ValidateEntity();

        /// <summary>
        /// Set validation languages
        /// </summary>
        /// <param name="languageAvailability"></param>
        void SetLanguages(ILanguagesAvailabilities languageAvailability);

        /// <summary>
        /// Set validation languages
        /// </summary>
        /// <param name="languages"></param>
        void SetLanguages(List<Guid> languages);

        /// <summary>
        /// Set validation output language
        /// </summary>
        /// <param name="languageId"></param>
        void SetValidationLanguage(Guid languageId);
    }

    internal interface IValidationChecker<T> : IValidationCheckerBase
    {
        /// <summary>
        /// Init entity by entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="validationMessages"></param>
        void Init(T entity, List<ValidationPath> parentPath = null);
    }
    
    internal interface ILoadingValidationChecker<T> : IValidationCheckerBase
    {
        /// <summary>
        /// Init entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="unitOfWork"></param>
        void Init(Guid id, IUnitOfWork unitOfWork, List<ValidationPath> parentPath = null);
    }
}
