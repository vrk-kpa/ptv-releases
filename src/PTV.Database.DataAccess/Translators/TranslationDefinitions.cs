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
using System.Reflection;
using System.Threading.Tasks;
using PTV.Database.DataAccess.ApplicationDbContext;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Enums;
using Microsoft.EntityFrameworkCore;
using PTV.Database.DataAccess.Services;
using PTV.Database.Model.Models;

namespace PTV.Database.DataAccess.Translators
{
    internal class TranslationDefinitions<TIn, TOut> : ITranslationDefinitionsEntityToVModel<TIn, TOut>, ITranslationDefinitionsVModelToEntity<TIn, TOut> where TIn : class where TOut : class
    {
        private static List<Type> nonEnumerableTypes = new List<Type> {typeof(string)};
        private bool identicalPropertiesAutoTranslation = true;

        private readonly IResolveManager resolveManager;
        private TIn sourceInstance;
        private TOut targetInstance;
        private readonly ITranslationPrimitives translationPrimitives;
        private readonly Type typeToInstantiate;
        private readonly TranslationDirection directionDefinition;
        private readonly ITranslationUnitOfWork unitOfWork;
        private readonly LanguageCode languageCode;
        private ILanguageCache languageCache;

        public TranslationDefinitions(IResolveManager resolveManager,  ITranslationPrimitives translationPrimitives, EntityNavigationsMap entityNavigationsMap, ITranslationUnitOfWork unitOfWork, TranslationDirection directionDefinition, Type typeToInstantiate = null, TOut predefinedTarget = null, LanguageCode languageCode = LanguageCode.fi)
        {
            this.resolveManager = resolveManager;
            this.translationPrimitives = translationPrimitives;
            this.directionDefinition = directionDefinition;
            this.typeToInstantiate = typeToInstantiate ?? typeof (TOut);
            this.unitOfWork = unitOfWork;
            this.targetInstance = predefinedTarget;
            this.languageCode = languageCode;
            this.languageCache = resolveManager.Resolve<ICacheManager>().LanguageCache;
        }
        public void SetTnit(TIn source)
        {
            this.sourceInstance = source;
        }

        public ITranslationDefinitions<TIn, TOut> DisableAutoTranslation()
        {
            this.identicalPropertiesAutoTranslation = false;
            return this;
        }

        private bool EnsureCreateTargetInstance()
        {
            if ((sourceInstance != null) && (targetInstance == null))
            {
                if (typeof(TOut) == typeof(string))
                {
                    targetInstance = (TOut) (object) string.Empty;
                }
                else
                {
                    targetInstance = (TOut) Activator.CreateInstance(typeToInstantiate);
                }
                if (identicalPropertiesAutoTranslation)
                {
                    BaseTranslator.Translate<TIn, TOut>(sourceInstance, targetInstance);
                }
                //var newInstance = targetInstance;
                //targetInstance = (duplicatesHandler?.ProcessInstance(sourceInstance, targetInstance)) ?? targetInstance;
                //var newCreated = newInstance == targetInstance;
                return true;// newCreated;
            }
            return false;
        }

        private void EnsureCreateTargetInstance(TOut target)
        {
            targetInstance = target;
            if (identicalPropertiesAutoTranslation)
            {
                BaseTranslator.Translate<TIn, TOut>(sourceInstance, targetInstance);
            }
        }

        public ITranslationDefinitions<TIn, TOut> AddRequestLanguage<TOutLoc>(Expression<Func<TOut, TOutLoc>> localizable) where TOutLoc :class, TOut, ILocalizable
        {
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            var localizableTarget = targetInstance as TOutLoc;
            localizableTarget.SafeCall(i => i.LocalizationId = languageCache.Get(languageCode.ToString()));
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddPartial<TInProperty>(Func<TIn, TInProperty> fromProperty) where TInProperty : class
        {
            var sourcePropertyType = typeof(TInProperty);
            if (sourcePropertyType.IsEnumerable(nonEnumerableTypes))
            {
                throw new Exception(string.Format(CoreMessages.UseAddCollectionInstead, typeof(TIn).Name, typeof(TOut).Name, typeof(TInProperty).Name));
            }
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            CallTranslation(fromProperty(sourceInstance), targetInstance);
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddSimple<TInProperty, TOutProperty>(Func<TIn, TInProperty> fromProperty, Expression<Func<TOut, TOutProperty>> toProperty) where TInProperty : struct where TOutProperty : struct
        {
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            translationPrimitives.TranslatePrimitive(sourceInstance, ref targetInstance, fromProperty, toProperty);
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddSimple<TInProperty, TOutProperty>(Func<TIn, List<TInProperty>> fromProperty, Expression<Func<TOut, IReadOnlyList<TOutProperty>>> toProperty) where TInProperty : struct where TOutProperty : struct
        {
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            translationPrimitives.TranslatePrimitive(sourceInstance, ref targetInstance, fromProperty, toProperty);
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddSimple<TInProperty, TOutProperty>(Func<TIn, TInProperty?> fromProperty, Expression<Func<TOut, TOutProperty?>> toProperty) where TInProperty : struct where TOutProperty : struct
        {
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            translationPrimitives.TranslatePrimitive(sourceInstance, ref targetInstance, fromProperty, toProperty);
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddSimple<TInProperty, TOutProperty>(Func<TIn, TInProperty> fromProperty, Expression<Func<TOut, TOutProperty?>> toProperty) where TInProperty : struct where TOutProperty : struct
        {
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            translationPrimitives.TranslatePrimitive(sourceInstance, ref targetInstance, fromProperty, toProperty);
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddSimpleList<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty, Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty) where TInProperty : struct where TOutProperty : struct
        {
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            if (translationPrimitives.TranslatePrimitive(sourceInstance, ref targetInstance, fromProperty, toProperty)) return this;
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddNavigation<TInProperty, TOutProperty>(Func<TIn, TInProperty> fromProperty, Expression<Func<TOut, TOutProperty>> toProperty, bool useTargetIfExisting = false)
            where TInProperty : class where TOutProperty : class
        {
            var sourcePropertyType = typeof(TInProperty);
            var targetPropertyType = typeof(TOutProperty);
            if (sourcePropertyType.IsEnumerable(nonEnumerableTypes) || targetPropertyType.IsEnumerable(nonEnumerableTypes))
            {
                throw new Exception(string.Format(CoreMessages.UseAddCollectionInstead, typeof(TIn).Name, typeof(TOut).Name, typeof(TInProperty).Name, typeof(TOutProperty).Name));
            }
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            if (translationPrimitives.TranslatePrimitive(sourceInstance, ref targetInstance, fromProperty, toProperty)) return this;
            var toPropertyInstance = useTargetIfExisting ? toProperty.Compile()(targetInstance) : null;
            targetInstance.SetPropertyValue(toProperty, CallTranslation<TInProperty, TOutProperty>(fromProperty(sourceInstance), toPropertyInstance));
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddLocalizable<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty, Expression<Func<TOut, TOutProperty>> toProperty, TOutProperty defaultValueIfNotFound = null)
            where TInProperty : class, ILocalizable where TOutProperty : class
        {
            var targetPropertyType = typeof(TOutProperty);
            if (targetPropertyType.IsEnumerable(nonEnumerableTypes))
            {
                throw new Exception(string.Format(CoreMessages.UseAddCollectionInstead, typeof(TIn).Name, typeof(TOut).Name, typeof(TInProperty).Name, typeof(TOutProperty).Name));
            }
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            if (translationPrimitives.TranslatePrimitive(sourceInstance, ref targetInstance, fromProperty, toProperty)) return this;
            var fromPropertyInstance = languageCache.Filter(fromProperty(sourceInstance), languageCode);
            targetInstance.SetPropertyValue(toProperty, fromPropertyInstance != default(TInProperty) ? CallTranslation<TInProperty, TOutProperty>(fromPropertyInstance) : defaultValueIfNotFound);
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddLocalizable<TInProperty, TOutProperty>(Func<TIn, TInProperty> fromProperty, Expression<Func<TOut, ICollection<TOutProperty>>> toProperty, Func<TIn, Func<TOutProperty, bool>> entitySelector = null)
             where TInProperty : class where TOutProperty : class, ILocalizable
        {
            var sourcePropertyType = typeof(TInProperty);
            if (sourcePropertyType.IsEnumerable(nonEnumerableTypes))
            {
                throw new Exception(string.Format(CoreMessages.UseAddCollectionInstead, typeof(TIn).Name, typeof(TOut).Name, typeof(TInProperty).Name, typeof(TOutProperty).Name));
            }
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            if (languageCache == null)
            {
                languageCache = resolveManager.Resolve<ICacheManager>().LanguageCache;
            }
            unitOfWork?.LoadNavigationProperty(targetInstance, toProperty);
            var targetPropertyInstance = toProperty.Compile()(targetInstance);
            var filteredOutput = languageCache.FilterCollection(targetPropertyInstance, languageCode);
            Func<TOutProperty, bool> query = (entitySelector != null) ? entitySelector(sourceInstance) : i => true;
            var toUpdate = filteredOutput.FirstOrDefault(query);

            var translated = CallTranslation<TInProperty, TOutProperty>(fromProperty(sourceInstance), toUpdate);
            if (toUpdate == null && translated != null)
            {
                targetPropertyInstance.Add(translated);
            }
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddNavigationOneMany<TInProperty, TOutProperty>(Func<TIn, TInProperty> fromProperty, Expression<Func<TOut, ICollection<TOutProperty>>> toProperty, bool useTargetIfExisting = false)
            where TInProperty : class where TOutProperty : class
        {
            var sourcePropertyType = typeof(TInProperty);
            var targetPropertyType = typeof(TOutProperty);
            if (sourcePropertyType.IsEnumerable(nonEnumerableTypes) || (targetPropertyType.IsEnumerable(nonEnumerableTypes)))
            {
                throw new Exception(string.Format(CoreMessages.IncorrectUsageTranslation, typeof(TIn).Name, typeof(TOut).Name, typeof(TInProperty).Name, typeof(TOutProperty).Name));
            }
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            TOutProperty toPropertyInstance = null;
            var targetCollection = toProperty.Compile()(targetInstance) ?? new HashSet<TOutProperty>();
            if (useTargetIfExisting)
            {
                toPropertyInstance = targetCollection?.FirstOrDefault();
                bool wasPresent = toPropertyInstance != null;
                toPropertyInstance = CallTranslation<TInProperty, TOutProperty>(fromProperty(sourceInstance), toPropertyInstance);
                if (!wasPresent && (toPropertyInstance != null))
                {
                    targetCollection.Add(toPropertyInstance);
                }
            }
            else
            {
                targetCollection.Clear();
                toPropertyInstance = CallTranslation<TInProperty, TOutProperty>(fromProperty(sourceInstance));
                targetCollection.Add(toPropertyInstance);
            }
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddCollection<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty, Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty) where TInProperty : class where TOutProperty : class
        {
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            if (translationPrimitives.TranslatePrimitive(sourceInstance, ref targetInstance, fromProperty, toProperty)) return this;
            var inputEnumerable = fromProperty(sourceInstance);
            var outputList = inputEnumerable.Select(itemToTranslate => CallTranslation<TInProperty, TOutProperty>(itemToTranslate)).ToList();
            targetInstance.SetPropertyValue(toProperty, outputList);
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> UseDataContextCreate(Func<TIn, bool> callCondition)
        {
            if (SkipCallCondition(callCondition)) return this;
            if (unitOfWork == null)
            {
                EnsureCreateTargetInstance();
                return this;
            }
            if (EnsureCreateTargetInstance())
            {
                if (targetInstance == null)
                {
                    throw new DbEntityNotFoundException($"{CoreMessages.EntityNotFoundToUpdate}. {typeof(TIn).Name} - {typeof(TOut).Name}");
                }
                var entitySet = unitOfWork.GetSet<TOut>();
                entitySet.Add(targetInstance);
            }
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> UseDataContextCreate<TKeyProperty>(Func<TIn, bool> callCondition, Expression<Func<TOut, TKeyProperty>> outKey, Func<TIn, TKeyProperty> valueForNew)
        {
            if (SkipCallCondition(callCondition)) return this;
            UseDataContextCreate(callCondition);
            targetInstance?.SetPropertyValue(outKey, valueForNew(sourceInstance));
            return this;
        }

        private bool SkipCallCondition(Func<TIn, bool> callCondition)
        {
            return (sourceInstance == null) ||
             (targetInstance != null) ||
            (!callCondition(sourceInstance));
        }

        public ITranslationDefinitions<TIn, TOut> UseDataContextUpdate(Func<TIn, bool> callCondition, Func<TIn, Expression<Func<TOut, bool>>> entitySelector,
            Action<ITranslationDefinitions<TIn, TOut>> actionIfNotFound = null)
        {
            UseDataContextUpdateInternal(callCondition, entitySelector, null, actionIfNotFound);
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> UseDataContextLocalizedUpdate(Func<TIn, bool> callCondition, Func<TIn, Expression<Func<TOut, bool>>> entitySelector, Action<ITranslationDefinitions<TIn, TOut>> actionIfNotFound = null)
        {
            Func<IQueryable<TOut>, IQueryable<TOut>> internalAction = (entitySet) =>
            {
                if (typeof(ILocalizable).IsAssignableFrom(typeof(TOut)))
                {
                    return (entitySet as IQueryable<ILocalizable>).Where(i => i.Localization.Code == languageCode.ToString()).Cast<TOut>();
                }
                return entitySet;
            };
            UseDataContextUpdateInternal(callCondition, entitySelector, internalAction, actionIfNotFound);
            return this;
        }

        private void UseDataContextUpdateInternal(Func<TIn, bool> callCondition, Func<TIn, Expression<Func<TOut, bool>>> entitySelector, Func<IQueryable<TOut>, IQueryable<TOut>> internalAction, Action<ITranslationDefinitions<TIn, TOut>> actionIfNotFound = null)
        {
            if (SkipCallCondition(callCondition)) return;
            if (unitOfWork == null)
            {
                EnsureCreateTargetInstance();
                return;
            }
            IQueryable<TOut> entitySet = unitOfWork.GetSet<TOut>();
            Expression<Func<TOut, bool>> query = entitySelector(sourceInstance);
            entitySet = internalAction?.Invoke(entitySet) ?? entitySet;
            var dbEntity = entitySet.FirstOrDefault(query);
            if (dbEntity != null)
            {
                EnsureCreateTargetInstance(dbEntity);
            }
            else
            {
                if (actionIfNotFound != null)
                {
                    actionIfNotFound(this);
                }
                else
                {
                    throw new DbEntityNotFoundException($"{CoreMessages.EntityNotFoundToUpdate}. {typeof(TIn).Name} - {typeof(TOut).Name}");
                }
            }
        }

        public TOut GetFinal(TOut defaultValue = default(TOut))
        {
            EnsureCreateTargetInstance();
            return targetInstance ?? defaultValue;
        }

        private TTo CallTranslation<TFrom, TTo>(TFrom originalInstance, TTo targetPredInstance = null) where TFrom : class where TTo : class
        {
            switch (directionDefinition)
            {
                case TranslationDirection.EntityToViewModel:
                    {
                        var translator = resolveManager.Resolve<ITranslator<TFrom, TTo>>();
                        translator.SetLanguage(languageCode);
                        var assignableTranslator = translator as ITranslationDefinitionAccessor<TFrom, TTo>;
                        assignableTranslator?.SetTargetInstance(targetPredInstance);
                        return translator.TranslateEntityToVm(originalInstance);

                    }
                case TranslationDirection.ViewModelToEntity:
                    {
                        var translator = resolveManager.Resolve<ITranslator<TTo, TFrom>>();
                        translator.SetLanguage(languageCode);
                        translator.UnitOfWork = unitOfWork;
                        var assignableTranslator = translator as ITranslationDefinitionAccessor<TTo, TFrom>;
                        assignableTranslator?.SetTargetInstance(targetPredInstance);
                        return translator.TranslateVmToEntity(originalInstance);

                    }
                default:
                    throw new Exception();
            }
        }
    }
}
