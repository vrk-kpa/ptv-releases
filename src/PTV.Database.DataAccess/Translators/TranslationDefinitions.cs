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
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal;
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Framework;
using PTV.Framework.Interfaces;
using PTV.Database.DataAccess.Interfaces;
using PTV.Database.Model.Interfaces;
using PTV.Domain.Model.Enums;
using PTV.Database.Model.Models.Base;

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
        private Guid? requestlanguageIdAssigned;
        private ILanguageCache languageCache;
        private readonly IVersioningManager versioningManager;
        private readonly IPublishingStatusCache publishingStatusCache;
        private bool clonedTargetApplied = false;
        private bool autoTranslationExplicitlySet = false;
        private bool requestLanguageExplicitlySet = false;
        private TranslationPolicy translationPolicies;

        public TranslationDefinitions(IResolveManager resolveManager,
            ITranslationPrimitives translationPrimitives,
            ITranslationUnitOfWork unitOfWork,
            TranslationDirection directionDefinition,
            IVersioningManager versioningManager,
            Type typeToInstantiate = null,
            TOut predefinedTarget = null,
            Guid? requestlanguageId = null,
            TranslationPolicy translationPolicies = TranslationPolicy.Defaults)
        {
            this.resolveManager = resolveManager;
            this.translationPrimitives = translationPrimitives;
            this.directionDefinition = directionDefinition;
            this.typeToInstantiate = typeToInstantiate ?? typeof (TOut);
            this.unitOfWork = unitOfWork;
            this.targetInstance = predefinedTarget;
            this.requestlanguageIdAssigned = requestlanguageId;
            var cacheManager = resolveManager.Resolve<ICacheManager>();
            this.languageCache = cacheManager.LanguageCache;
            this.publishingStatusCache = cacheManager.PublishingStatusCache;
            this.versioningManager = versioningManager;
            this.translationPolicies = translationPolicies;
            if (this.unitOfWork != null && this.targetInstance != null)
            {
                var cached = unitOfWork.TranslationCloneCache.GetFromCachedSet<TOut>();
                clonedTargetApplied = cached.Any(i => i.ClonedEntity == this.targetInstance);
            }
        }

        private Guid requestlanguageId => requestlanguageIdAssigned ?? (requestlanguageIdAssigned = languageCache.Get(LanguageCode.fi)).Value;

        public void SetTnit(TIn source)
        {
            this.sourceInstance = source;
        }

        public ITranslationDefinitions<TIn, TOut> DisableAutoTranslation()
        {
            this.identicalPropertiesAutoTranslation = false;
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> EnableAutoTranslation()
        {
            this.identicalPropertiesAutoTranslation = true;
            this.autoTranslationExplicitlySet = true;
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
                ResetTrackedVersioning(targetInstance);
                return true;
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

        private void ResetTrackedVersioning(TOut entity)
        {
            (entity as IVersionedTrackedEntity).SafeCall(i => i.VersioningApplied = false);
        }

        public ITranslationDefinitions<TIn, TOut> ApplyMarkAsKept<TProcess>(Func<TProcess, bool> markingCondition) where TProcess : class
        {
            if (unitOfWork?.TranslationCloneCache == null) return this;
            unitOfWork.TranslationCloneCache.MarkAllAsMustBeKept<TProcess>(markingCondition);
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> UseVersioning<TOutVersioned, TOutRoot>(Expression<Func<TOut, TOutVersioned>> versioned, VersioningMode versioningMode = VersioningMode.Standard, Func<TIn, bool> versioningCondition = null) where TOutRoot : class, IVersionedRoot, new()  where TOutVersioned : class, TOut, IVersionedVolume<TOutRoot>, new()
        {
            if (unitOfWork == null) return this;
            if (sourceInstance == null) return this;
            if (targetInstance == null)
            {
                throw new Exception(CoreMessages.TranslatorWrongVersioningUsage);
            }
            if ((versioningCondition != null) && (!versioningCondition(sourceInstance)))
            {
                return this;
            }
            var tracked = targetInstance as IVersionedTrackedEntity;
            if (tracked?.VersioningApplied == true) return this;
            var versionedEntity = versioned.Compile()(targetInstance);
            var newTarget = versioningManager.CreateModifiedVersion(unitOfWork, versionedEntity, versioningMode);
            clonedTargetApplied = newTarget != versionedEntity;
            versionedEntity = newTarget;
            versioningManager.EnsureUnificRoot(versionedEntity);
            targetInstance = versionedEntity;
            tracked.SafeCall(i => i.VersioningApplied = true);
            if (clonedTargetApplied)
            {
                translationPolicies |= TranslationPolicy.MergeOutputCollections;
                unitOfWork.TranslationCloneCache.MarkAsProcessedByTranslator(newTarget);
                if (requestLanguageExplicitlySet)
                {
                    ApplyMarkAsKept<ILocalizable>(i => i.LocalizationId != requestlanguageId);
                }
            }
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddRequestLanguage<TOutLoc>(Expression<Func<TOut, TOutLoc>> localizable) where TOutLoc :class, TOut, ILocalizable
        {
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            var localizableTarget = targetInstance as TOutLoc;
            localizableTarget.SafeCall(i => i.LocalizationId = requestlanguageId);
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddLanguageAvailability<TOutLang>(Func<TOut, IMultilanguagedEntity<TOutLang>> languageAvailable) where TOutLang : class,ILanguageAvailability, new ()
        {
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            var languagedTarget = languageAvailable(targetInstance);
            if ((!clonedTargetApplied))
            {
                unitOfWork.LoadNavigationProperty(targetInstance, i => languageAvailable(i).LanguageAvailabilities);
            }
            var availability = languagedTarget.LanguageAvailabilities.FirstOrDefault(i => i.LanguageId == requestlanguageId) ??
                               languagedTarget.LanguageAvailabilities.AddAndReturn(new TOutLang() {LanguageId = requestlanguageId });
            if (availability.StatusId == Guid.Empty)
            {
                availability.StatusId = publishingStatusCache.Get(PublishingStatus.Draft);
            }
            if (availability.StatusId == publishingStatusCache.Get(PublishingStatus.Published))
            {
                availability.StatusId = publishingStatusCache.Get(PublishingStatus.Modified);
            }
            return this;
        }
        public ITranslationDefinitions<TIn, TOut> AddPartial<TInProperty>(Func<TIn, TInProperty> fromProperty) where TInProperty : class
        {
            AddPartial(fromProperty, t => t);
            return this;
        }
        public ITranslationDefinitions<TIn, TOut> AddPartial<TInProperty, TExplicitTarget>(Func<TIn, TInProperty> fromProperty, Func<TOut, TExplicitTarget> explicitTarget) where TInProperty : class where TExplicitTarget : class
        {
            var sourcePropertyType = typeof(TInProperty);
            if (sourcePropertyType.IsEnumerable(nonEnumerableTypes))
            {
                throw new Exception(string.Format(CoreMessages.UseAddCollectionInstead, typeof(TIn).Name, typeof(TOut).Name, typeof(TInProperty).Name));
            }
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            CallTranslation(fromProperty(sourceInstance), explicitTarget(targetInstance));
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
            var fromPropertyInstance = languageCache.Filter(fromProperty(sourceInstance), requestlanguageId);
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
            if (!clonedTargetApplied)
            {
                unitOfWork?.LoadNavigationProperty(targetInstance, toProperty);
            }
            
            var targetPropertyInstance = toProperty.Compile()(targetInstance);
            var filteredOutput = languageCache.FilterCollection(targetPropertyInstance, requestlanguageId);
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

        public ITranslationDefinitions<TIn, TOut> AddCollection<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty) where TInProperty : class where TOutProperty : class
        {
            AddCollectionInternal<TInProperty, TOutProperty>(fromProperty, toProperty, keepUntouchedAndMerge: translationPolicies.HasFlag(TranslationPolicy.MergeOutputCollections), fetchData: translationPolicies.HasFlag(TranslationPolicy.FetchData));
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddCollection<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty, TranslationPolicy translationPolicy) where TInProperty : class where TOutProperty : class
        {
            AddCollectionInternal<TInProperty, TOutProperty>(fromProperty, toProperty, keepUntouchedAndMerge: translationPolicy.HasFlag(TranslationPolicy.MergeOutputCollections), fetchData: translationPolicy.HasFlag(TranslationPolicy.FetchData));
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddCollection<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty, bool keepUntouchedAndMerge) where TInProperty : class where TOutProperty : class
        {
            AddCollectionInternal<TInProperty, TOutProperty>(fromProperty, toProperty, keepUntouchedAndMerge: keepUntouchedAndMerge);
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddCollectionWithKeep<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty, Func<TOutProperty, bool> mergeAndKeepTheseSelector) where TInProperty : class where TOutProperty : class
        {
            AddCollectionInternal<TInProperty, TOutProperty>(fromProperty, toProperty, keepUntouchedAndMerge: false, fetchData: false, mergeAndKeepTheseSelector: mergeAndKeepTheseSelector);
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddCollectionWithKeep<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty, TranslationPolicy translationPolicy, Func<TOutProperty, bool> mergeAndKeepTheseSelector) where TInProperty : class where TOutProperty : class
        {
            AddCollectionInternal<TInProperty, TOutProperty>(fromProperty, toProperty, keepUntouchedAndMerge: translationPolicy.HasFlag(TranslationPolicy.MergeOutputCollections), fetchData: translationPolicy.HasFlag(TranslationPolicy.FetchData), mergeAndKeepTheseSelector: mergeAndKeepTheseSelector);
            return this;
        }


        public ITranslationDefinitions<TIn, TOut> AddCollectionWithRemove<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty, Func<TOutProperty, bool> whatRemoveSelector) where TInProperty : class where TOutProperty : class
        {
            AddCollectionInternal<TInProperty, TOutProperty>(fromProperty, toProperty, keepUntouchedAndMerge: false, fetchData: false, mergeAndKeepTheseSelector: null, whatToRemoveSelector: whatRemoveSelector);
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddCollectionWithRemove<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty, TranslationPolicy translationPolicy, Func<TOutProperty, bool> whatRemoveSelector) where TInProperty : class where TOutProperty : class
        {
            AddCollectionInternal<TInProperty, TOutProperty>(fromProperty, toProperty, keepUntouchedAndMerge: translationPolicy.HasFlag(TranslationPolicy.MergeOutputCollections), fetchData: translationPolicy.HasFlag(TranslationPolicy.FetchData), mergeAndKeepTheseSelector: null, whatToRemoveSelector: whatRemoveSelector);
            return this;
        }

        private ITranslationDefinitions<TIn, TOut> AddCollectionInternal<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty, Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty, bool keepUntouchedAndMerge = false, bool fetchData = false, Func<TOutProperty, bool> mergeAndKeepTheseSelector = null, Func<TOutProperty, bool> whatToRemoveSelector = null) where TInProperty : class where TOutProperty : class
        {
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            if (translationPrimitives.TranslatePrimitive(sourceInstance, ref targetInstance, fromProperty, toProperty)) return this;
            var inputEnumerable = fromProperty(sourceInstance);
            if (inputEnumerable == null)
            {
                return this;
            }
            var existingOutput = toProperty.Compile()(targetInstance)?.ToList() ?? new List<TOutProperty>();
            if (!clonedTargetApplied && fetchData && unitOfWork != null)
            {
                unitOfWork.LoadNavigationProperty<TOut, TOutProperty>(targetInstance, toProperty);
                existingOutput = toProperty.Compile()(targetInstance)?.Except(existingOutput)?.Union(existingOutput)?.ToList() ?? new List<TOutProperty>();
            }
            var outputList = inputEnumerable.Select(itemToTranslate => CallTranslation<TInProperty, TOutProperty>(itemToTranslate)).ToList();
            if (!existingOutput.IsNullOrEmpty() && (unitOfWork != null))
            {
                if (keepUntouchedAndMerge)
                {
                    outputList = existingOutput.Except(outputList).Union(outputList).ToList();
                    if (clonedTargetApplied)
                    {
                        var toKeep = unitOfWork.TranslationCloneCache.GetTouchedEntities(outputList, new List<object>() {targetInstance});
                        var toDetach = outputList.Except(toKeep).ToList();
                        toDetach.ForEach(i => unitOfWork.DetachEntity(i));
                    }
                }
                else
                {
                    var keepEntities = mergeAndKeepTheseSelector != null ? existingOutput.Where(mergeAndKeepTheseSelector).ToList() : new List<TOutProperty>();
                    var removeEntities = whatToRemoveSelector != null ? existingOutput.Where(whatToRemoveSelector).ToList() : new List<TOutProperty>();
                    unitOfWork.TranslationCloneCache.MarkAllAsMustBeKept(keepEntities);
                    unitOfWork.DetachOrRemoveEntities(unitOfWork.TranslationCloneCache.MarkAllAsDeletable(removeEntities.Except(outputList).ToList(), true));
                    List<TOutProperty> toDetach = new List<TOutProperty>();
                    if (clonedTargetApplied)
                    {
                        if (whatToRemoveSelector != null)
                        {
                            toDetach = removeEntities.Except(outputList).ToList();
                        }
                        else
                        {
                            toDetach = existingOutput.Except(keepEntities).Except(outputList).ToList();
                        }
                        var toDetachAll = unitOfWork.TranslationCloneCache.GetEntitiesCascade(toDetach.Cast<object>().ToList()).Union(toDetach).ToList();
                        toDetachAll.ForEach(i => unitOfWork.DetachOrRemoveEntity(i));
                    }
                    outputList = existingOutput.Except(outputList).Union(outputList).Except(keepEntities).Union(keepEntities).Except(toDetach).ToList();
                }
            }
            targetInstance.SetPropertyValue(toProperty, outputList);
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddDictionary<TInProperty, TOutProperty, TKeyProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, Dictionary<TKeyProperty, TOutProperty>>> toProperty,
            Func<TInProperty, TKeyProperty> keyProperty) where TInProperty : class where TOutProperty : class where TKeyProperty : class
        {
            AddDictionary<TInProperty, TOutProperty, TKeyProperty>(fromProperty, toProperty, keyProperty, translationPolicies.HasFlag(TranslationPolicy.MergeOutputCollections));
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> AddDictionary<TInProperty, TOutProperty, TKeyProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, Dictionary<TKeyProperty, TOutProperty>>> toProperty,
            Func<TInProperty, TKeyProperty> keyProperty, bool keepUntouchedAndMerge) where TInProperty : class where TOutProperty : class where TKeyProperty : class
        {
            if (sourceInstance == null) return this;
            EnsureCreateTargetInstance();
            if (translationPrimitives.TranslatePrimitive(sourceInstance, ref targetInstance, fromProperty, toProperty)) return this;
            var inputEnumerable = fromProperty(sourceInstance);
            if (inputEnumerable == null)
            {
                return this;
            }
            var outputList = inputEnumerable.Select(itemToTranslate => new KeyValuePair<TKeyProperty, TOutProperty>(keyProperty(itemToTranslate), CallTranslation<TInProperty, TOutProperty>(itemToTranslate))).ToDictionary(x => x.Key, x => x.Value);
            var existingOutput = toProperty.Compile()(targetInstance);
            if (!existingOutput.IsNullOrEmpty() && keepUntouchedAndMerge)
            {
                outputList = existingOutput.Except(outputList).Union(outputList).ToDictionary(x => x.Key, x => x.Value);
            }
            targetInstance.SetPropertyValue(toProperty, outputList);
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> UseDataContextCreate(Func<TIn, bool> callCondition = null)
        {
            if ((callCondition != null) && SkipCallCondition(callCondition)) return this;
            if (!autoTranslationExplicitlySet)
            {
                DisableAutoTranslation();
            }
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
                    var langId = requestlanguageId;
                    return (entitySet as IQueryable<ILocalizable>).Where(i => i.LocalizationId == langId).Cast<TOut>();
                }
                return entitySet;
            };
            UseDataContextUpdateInternal(callCondition, entitySelector, internalAction, actionIfNotFound);
            return this;
        }

        private void UseDataContextUpdateInternal(Func<TIn, bool> callCondition, Func<TIn, Expression<Func<TOut, bool>>> entitySelector, Func<IQueryable<TOut>, IQueryable<TOut>> internalAction, Action<ITranslationDefinitions<TIn, TOut>> actionIfNotFound = null)
        {
            if (SkipCallCondition(callCondition)) return;
            if (!autoTranslationExplicitlySet)
            {
                DisableAutoTranslation();
            }
            if (unitOfWork == null)
            {
                EnsureCreateTargetInstance();
                return;
            }
            IQueryable<TOut> entitySet = unitOfWork.GetSet<TOut>();
            Expression<Func<TOut, bool>> query = entitySelector(sourceInstance);
            var cached = unitOfWork.TranslationCloneCache.GetFromCachedSet<TOut>();

            entitySet = internalAction?.Invoke(entitySet) ?? entitySet;
            TOut dbEntity = entitySet.FirstOrDefault(query);

            if (dbEntity != null)
            {
                var cachedTrace = cached.FirstOrDefault(i => i.OriginalEntity == dbEntity);
                if (cachedTrace != null)
                {
                    clonedTargetApplied = true;
                    cachedTrace.ProcessedByTranslator = true;
                    dbEntity = cachedTrace.ClonedEntity;
                }
                else
                {
                    ResetTrackedVersioning(dbEntity);
                }
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


        public ITranslationDefinitions<TIn, TOut> Propagation(Action<TIn, TOut> propagationAction)
        {
            var originalNonCloned = unitOfWork.TranslationCloneCache.GetFromCachedSet<TOut>().FirstOrDefault(i => i.ClonedEntity == targetInstance)?.OriginalEntity;
            propagationAction(sourceInstance, originalNonCloned ?? targetInstance);
            return this;
        }

        public ITranslationDefinitions<TIn, TOut> Propagation(Action<TIn, TOut, TOut> propagationAction)
        {
            Propagation((i, o) => propagationAction(i, o, targetInstance));
            return this;
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
                       
                        translator.SetTranslationPolicy(translationPolicies);
                        var assignableTranslator = translator as ITranslationDefinitionAccessor<TFrom, TTo>;
                        assignableTranslator.SetLanguageInternaly(requestlanguageId, requestLanguageExplicitlySet);
                        assignableTranslator?.SetTargetInstance(targetPredInstance);
                        return translator.TranslateEntityToVm(originalInstance);

                    }
                case TranslationDirection.ViewModelToEntity:
                    {
                        var translator = resolveManager.Resolve<ITranslator<TTo, TFrom>>();
                        translator.UnitOfWork = unitOfWork;
                        translator.SetTranslationPolicy(translationPolicies);
                        var assignableTranslator = translator as ITranslationDefinitionAccessor<TTo, TFrom>;
                        assignableTranslator.SetLanguageInternaly(requestlanguageId, requestLanguageExplicitlySet);
                        assignableTranslator?.SetTargetInstance(targetPredInstance);
                        return translator.TranslateVmToEntity(originalInstance);

                    }
                default:
                    throw new Exception();
            }
        }

        void ITranslationDefinitionAccessor<TIn, TOut>.SetTargetInstance(TOut instance)
        {
            this.EnsureCreateTargetInstance(instance);
        }

        void ITranslationDefinitionAccessor<TIn, TOut>.SetTargetInstance(TIn instance)
        {
            this.SetTnit(instance);
        }

        void ITranslationDefinitionAccessor<TIn, TOut>.SetLanguageInternaly(Guid languageId, bool explicitlySet)
        {
            this.requestlanguageIdAssigned = languageId;
            this.requestLanguageExplicitlySet = explicitlySet;
        }
    }
}
