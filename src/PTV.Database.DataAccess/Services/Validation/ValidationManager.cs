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
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Interfaces;

namespace PTV.Database.DataAccess.Services.Validation
{
    [RegisterService(typeof(IValidationManager), RegisterType.Transient)]
    internal class ValidationManager : IValidationManager, IInternalValidation
    {
        private readonly IResolveManager resolveManager;

        public ValidationManager(IResolveManager resolveManager)
        {
            this.resolveManager = resolveManager;
        }

        Dictionary<Guid, List<ValidationMessage>> IValidationManager.CheckEntity<T>(Guid id, IUnitOfWork unitOfWork, ILanguagesAvailabilities languageAvailability)
        {
            var result = new Dictionary<Guid, List<ValidationMessage>>();

            if (!id.IsAssigned()) return result;

            var checker = resolveManager.Resolve<IBaseValidationChecker<T>>();
            if (checker != null)
            {
                checker.Init(id, unitOfWork);
                checker.SetLanguages(languageAvailability);
                result = checker.ValidateEntity(null);
            }
            return result;
        }

        Dictionary<Guid, List<ValidationMessage>> IValidationManager.CheckEntity<T>(T entity, ILanguagesAvailabilities languageAvailability)
        {
            var result = new Dictionary<Guid, List<ValidationMessage>>();
            if (entity == null) return result;

            var checker = resolveManager.Resolve<IBaseValidationChecker<T>>();
            if (checker != null)
            {
                checker.Init(entity);
                checker.SetLanguages(languageAvailability);
                result = checker.ValidateEntity(null);
            }
            return result;
        }

        Dictionary<Guid, List<ValidationMessage>> IValidationManager.CheckEntity<T>(Guid id, IUnitOfWork unitOfWork, Guid? languageId)
        {
            var result = new Dictionary<Guid, List<ValidationMessage>>();

            if (!id.IsAssigned()) return result;

            var checker = resolveManager.Resolve<IBaseValidationChecker<T>>();
            if (checker != null)
            {
                if (languageId.HasValue)
                {
                    checker.SetValidationLanguage(languageId.Value);
                }
                checker.Init(id, unitOfWork);
                result = checker.ValidateEntity(languageId);
            }
            return result;
        }

        Dictionary<Guid, List<ValidationMessage>> IValidationManager.CheckEntity<T>(T entity, Guid? languageId)
        {
            var result = new Dictionary<Guid, List<ValidationMessage>>();
            if (entity == null) return result;

            var checker = resolveManager.Resolve<IBaseValidationChecker<T>>();
            if (checker != null)
            {
                if (languageId.HasValue)
                {
                    checker.SetValidationLanguage(languageId.Value);
                }
                checker.Init(entity);
                result = checker.ValidateEntity(languageId);
            }
            return result;
        }

        Dictionary<Guid, List<ValidationMessage>> IInternalValidation.CheckEntity<T>(T entity, List<ValidationPath> validationPaths, Guid? languageId)
        {
            var result = new Dictionary<Guid, List<ValidationMessage>>();
            if (entity == null) return result;

            var checker = resolveManager.Resolve<IBaseValidationChecker<T>>();
            if (checker != null)
            {
                if (languageId.HasValue)
                {
                    checker.SetValidationLanguage(languageId.Value);
                }

                checker.Init(entity, validationPaths);
                result = checker.ValidateEntity(languageId);
            }
            return result;
        }


    }
}