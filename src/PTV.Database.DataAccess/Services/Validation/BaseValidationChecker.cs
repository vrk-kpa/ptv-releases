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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Exceptions;
using PTV.Database.DataAccess.Interfaces.DbContext;
using PTV.Database.DataAccess.Interfaces.Repositories;
using PTV.Database.DataAccess.Interfaces.Services.Security;
using PTV.Database.DataAccess.Interfaces.Services.Validation;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces.OpenApi.V6;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Domain.Model.Models.V2.Common;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Framework.ServiceManager;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;

namespace PTV.Database.DataAccess.Services.Validation
{
    internal abstract class BaseLoadingValidationChecker<T> : ValidationCheckerBase<T>, ILoadingValidationChecker<T>
    {
        protected BaseLoadingValidationChecker(ICacheManager cacheManager, IResolveManager resolveManager) : base(cacheManager, resolveManager)
        {
        }

        public void Init(Guid id, IUnitOfWork unitOfWork, List<ValidationPath> parentPath)
        {
            this.entity = FetchEntity(id, unitOfWork);
            SetPath(id, parentPath);
        }
        protected abstract T FetchEntity(Guid id, IUnitOfWork unitOfWork);
        
    }

    internal abstract class BaseValidationChecker<T> : ValidationCheckerBase<T>, IValidationChecker<T>
    {
        protected BaseValidationChecker(ICacheManager cacheManager, IResolveManager resolveManager) : base(cacheManager, resolveManager)
        {
        }

        public void Init(T entity, List<ValidationPath> parentPath)
        {
            var entityIdentifier = entity as IEntityIdentifier;
            this.entity = entity;
            if (entityIdentifier != null && entityIdentifier.Id.IsAssigned())
            {
                SetPath(entityIdentifier.Id, parentPath);
            }
        }
    }
    
    internal abstract class ValidationCheckerBase<T> 
    {

        protected T entity;
        private readonly IResolveManager resolveManager;
        private IValidationManager validationManager;
        protected List<Guid> entityOrPublishedLanguagesAvailabilityIds;
        private Dictionary<Guid, List<ValidationMessage>> validationMessagesDictionary;
        
        private ITextManager textManager;
        private readonly ITypesCache typesCache;
        private Guid validationLanguageId;
        private List<ValidationPath> validationPaths;

        public IEnumerable<Guid> ValidationLanguages 
        {
            get
            {
                if (validationLanguageId.IsAssigned())
                {
                    yield return validationLanguageId;
                }
                else
                {
                    foreach (var t in entityOrPublishedLanguagesAvailabilityIds)
                    {
                        yield return t;
                    }
                }
            }   
        }

        public ValidationCheckerBase(ICacheManager cacheManager, IResolveManager resolveManager)
        {
            this.validationMessagesDictionary = new Dictionary<Guid, List<ValidationMessage>>();
            this.entityOrPublishedLanguagesAvailabilityIds = new List<Guid>();
            this.validationPaths = new List<ValidationPath>();

            this.typesCache = cacheManager.TypesCache;
            this.resolveManager = resolveManager;
            this.textManager = resolveManager.Resolve<ITextManager>();
            this.validationManager = resolveManager.Resolve<IValidationManager>();
        }

        public void SetLanguages(ILanguagesAvailabilities languageAvailability)
        {
            var entityLanguages = entity as IMultilanguagedEntity;

            this.entityOrPublishedLanguagesAvailabilityIds = languageAvailability == null && entityLanguages != null
                ? entityLanguages.LanguageAvailabilitiesReference.Select(x => x.LanguageId).ToList()
                : languageAvailability.LanguagesAvailabilities
                    .Where(x => x.StatusId ==
                                typesCache.Get<PublishingStatusType>(PublishingStatus.Published.ToString()))
                    .Select(x => x.LanguageId).ToList();
        }
        
        public void SetLanguages(List<Guid> languages)
        {
            if (languages != null)
            {
                entityOrPublishedLanguagesAvailabilityIds = languages;
            }
        }

        public void SetValidationLanguage(Guid languageId)
        {
            this.validationLanguageId = languageId;
        }

        private bool MergeToValidationMessagesDictionary(Dictionary<Guid, List<ValidationMessage>> secondDictionary)
        {
            if (!secondDictionary.Any())
            {
                return false;
            }
            
            secondDictionary.ForEach(language =>
            {
                AddRangeValidationMessagesToDictionary(language.Key, language.Value);
                    
            });
            return true;
        }
        
        #region ValidationHelpers

        protected bool NotEmptyGuid<TOutProperty>(string key, Expression<Func<T, TOutProperty>> property)
        {
            var val = property.Compile()(entity);
            var value = val as Guid?;

            if (value.IsAssigned()) return false;

            if (key != null)
            {
                AddValidationMessageToDictionary(key);
            }

            return true;
        }

        protected bool NotEmptyGuid<TOutProperty>(Expression<Func<T, TOutProperty>> property)
        {
            return NotEmptyGuid(null, property);
        }

        protected bool NotEmptyString<TOutProperty>(string key, Expression<Func<T, TOutProperty>> property)
        {
            var val = property.Compile()(entity);
            var value = val as string;
            if (!string.IsNullOrEmpty(value)) return false;

            if (key != null)
            {
                AddValidationMessageToDictionary(key);
            }

            return true;
        }

        protected bool NotEmptyString<TOutProperty>(Expression<Func<T, TOutProperty>> property)
        {
            return NotEmptyString(null, property);
        }

        protected bool NotEmptyStringFunc<TOutProperty>(string key, Func<T, TOutProperty> property)
        {
            var val = property(entity);
            var value = val as string;
            if (!string.IsNullOrEmpty(value)) return false;

            if (key != null)
            {
                AddValidationMessageToDictionary(key);
            }

            return true;
        }

        protected bool NotEmptyStringFunc<TOutProperty>(Func<T, TOutProperty> property)
        {
            return NotEmptyStringFunc(null, property);
        }
        
        protected bool NotEmptyTextEditorString<TOutProperty>(string key, Expression<Func<T, TOutProperty>> property)
        {
            var val = property.Compile()(entity);
            var value = val as string;

            if (!string.IsNullOrEmpty(textManager.ConvertToPureText(value))) return false;

            if (key != null)
            {
                AddValidationMessageToDictionary(key);
            }

            return true;
        }

        protected bool NotEmptyTextEditorString<TOutProperty>(Expression<Func<T, TOutProperty>> property)
        {
           return NotEmptyTextEditorString(null, property);
        }

        protected bool NotEmptyList<TOutProperty>(string key, Expression<Func<T, TOutProperty>> property) where TOutProperty : IEnumerable<object>
        {
            var val = property.Compile()(entity);

            if (val != null && val.Any()) return false;

            if (key != null)
            {
                AddValidationMessageToDictionary(key);
            }
            return true;
        }

        protected bool NotEmptyList<TOutProperty>(Expression<Func<T, TOutProperty>> property) where TOutProperty : IEnumerable<object>
        {
            return NotEmptyList(null, property);
        }
        
        protected bool NotEmptyListFunc<TOutProperty>(string key, Func<T, TOutProperty> property) where TOutProperty : IEnumerable<object>
        {
            var val = property(entity);
            if (val != null && val.Any()) return false;

            if (key != null)
            {
                AddValidationMessageToDictionary(key);
            }
            return true;
        }

        protected bool NotEmptyListFunc<TOutProperty>(Func<T, TOutProperty> property) where TOutProperty : IEnumerable<object>
        {
            return NotEmptyListFunc(null, property);
        }


        protected bool NotBeTrue<TOutProperty>(string key, Expression<Func<T, TOutProperty>> property, ValidationErrorTypeEnum errorType = ValidationErrorTypeEnum.MandatoryField)
        {
            var val = property.Compile()(entity);
            var value = val as bool?;

            if (!value.HasValue || !value.Value) return false;

            if (key != null)
            {
                AddValidationMessageToDictionary(key, errorType);
            }
            return true;
        }

        protected bool NotBeTrueFunc<TOutProperty>(string key, Func<T, TOutProperty> property)
        {
            var val = property(entity);
            var value = val as bool?;
            if (value.HasValue && value.Value)
            {
                AddValidationMessageToDictionary(key);
                return true;
            }
            return false;
        }

        protected virtual ValidationPath GetCurrentPath(ValidationPath defaultPath)
        {
            return defaultPath;
        }

        protected void SetPath(Guid id, List<ValidationPath> parentPath = null)
        {
            var currentPath = GetCurrentPath(new ValidationPath() { Name = typeof(T).Name, Id = id });

            if (parentPath != null && parentPath.Any())
            {
                this.validationPaths = parentPath.ToList();
               
            }

            if (currentPath != null)
            {
                this.validationPaths.Add(currentPath);
            }
        }

        protected void AddValidationMessageToDictionary(string key, ValidationErrorTypeEnum errorType = ValidationErrorTypeEnum.MandatoryField)
        {
            foreach (var languageId in ValidationLanguages)
            {
                if (validationMessagesDictionary.ContainsKey(languageId))
                {
                    var usedValidationMessages = new List<ValidationMessage>();
                    if (validationMessagesDictionary.Any() &&
                        validationMessagesDictionary.TryGetValue(languageId, out usedValidationMessages))
                    {
                        usedValidationMessages.Add(new ValidationMessage()
                        {
                            Key = key,
                            ValidationPaths = validationPaths,
                            ErrorType = errorType.ToCamelCase()
                        });
                    }

                    validationMessagesDictionary[languageId] = usedValidationMessages;
                }
                else
                {
                    validationMessagesDictionary.Add(languageId,
                        new List<ValidationMessage>()
                        {
                            new ValidationMessage()
                            {
                                Key = key,
                                ValidationPaths = validationPaths,
                                ErrorType = errorType.ToCamelCase()
                            }
                        });
                }
            }
        }

        private void AddRangeValidationMessagesToDictionary(Guid languageId, List<ValidationMessage> validationMessageses)
        {
            if (!validationMessageses.Any()) return;

            if (validationMessagesDictionary.ContainsKey(languageId))
            {
                var usedValidationMessages = new List<ValidationMessage>();
                if (validationMessagesDictionary.Any() &&
                    validationMessagesDictionary.TryGetValue(languageId, out usedValidationMessages))
                {
                    usedValidationMessages.AddRange(validationMessageses);
                }

                validationMessagesDictionary[languageId] = usedValidationMessages;
            }
            else
            {
                validationMessagesDictionary.Add(languageId, validationMessageses);
            }
        }

        #endregion ValidationHelpers

        protected TEntity GetEntity<TEntity>(Guid? id, IUnitOfWork unitOfWork, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeChain) where TEntity : class, IEntityIdentifier
        {
            if (id.IsAssigned())
            {
                var repository = unitOfWork.CreateRepository<IRepository<TEntity>>();
                var entity = unitOfWork.ApplyIncludes(repository.All(), includeChain, true).SingleOrDefault(x => x.Id == id);
                if (entity == null)
                {
                    throw new EntityNotFoundException($"Entity {typeof(TEntity).Name} with id {id} not found", typeof(TEntity).Name, new[] { id.ToString() });
                }
                return entity;
            }
            return null;
        }

        protected bool CheckEntityWithMergeResult<TE>(TE entity)
        {
            var result = ((IInternalValidation)validationManager).CheckEntity(entity, validationPaths, validationLanguageId, entityOrPublishedLanguagesAvailabilityIds);

            return MergeToValidationMessagesDictionary(result);
        }

        protected bool CheckEntityWithMergeResult<TE>(Guid id, IUnitOfWork unitOfWork)
        {
            var result = CheckEntity<TE>(id, unitOfWork);

            return MergeToValidationMessagesDictionary(result);
        }

        protected Dictionary<Guid, List<ValidationMessage>> CheckEntity<TE>(Guid id, IUnitOfWork unitOfWork)
        {
            return ((IInternalValidation)validationManager).CheckEntity<TE>(id, unitOfWork, validationPaths, validationLanguageId, entityOrPublishedLanguagesAvailabilityIds);
        }

        public Dictionary<Guid, List<ValidationMessage>> ValidateEntity()
        {
            ValidateEntityInternal(validationLanguageId);
            return validationMessagesDictionary;
        }
        
        protected abstract void ValidateEntityInternal(Guid? language);
    }
}
