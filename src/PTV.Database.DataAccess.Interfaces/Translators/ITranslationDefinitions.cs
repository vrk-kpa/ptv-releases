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
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;

namespace PTV.Database.DataAccess.Interfaces.Translators
{
    internal interface ITranslationDefinitions<TIn, TOut> : ITranslationDefinitionAccessor<TIn, TOut>
    {void SetTnit(TIn source);

        ITranslationDefinitions<TIn, TOut> AddPartial<TInProperty>(Func<TIn, TInProperty> fromProperty) where TInProperty : class;

        ITranslationDefinitions<TIn, TOut> AddPartial<TInProperty, TExplicitTarget>(Func<TIn, TInProperty> fromProperty, Func<TOut, TExplicitTarget> explicitTarget) where TInProperty : class where TExplicitTarget : class;

        ITranslationDefinitions<TIn, TOut> AddNavigation<TInProperty, TOutProperty>(Func<TIn, TInProperty> fromProperty, Expression<Func<TOut, TOutProperty>> toProperty) where TInProperty : class where TOutProperty : class;

        ITranslationDefinitions<TIn, TOut> AddNavigationOneMany<TInProperty, TOutProperty>(Func<TIn, TInProperty> fromProperty, Expression<Func<TOut, ICollection<TOutProperty>>> toProperty, bool useTargetIfExisting = false) where TInProperty : class where TOutProperty : class;

        ITranslationDefinitions<TIn, TOut> AddCollection<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty) where TInProperty : class where TOutProperty : class;

        ITranslationDefinitions<TIn, TOut> AddCollection<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty, TranslationPolicy translationPolicy) where TInProperty : class where TOutProperty : class;

        ITranslationDefinitions<TIn, TOut> AddCollection<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty, bool keepUntouchedAndMerge) where TInProperty : class where TOutProperty : class;


        ITranslationDefinitions<TIn, TOut> AddCollectionWithKeep<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty, Func<TOutProperty, bool> mergeAndKeepTheseSelector) where TInProperty : class where TOutProperty : class;

        ITranslationDefinitions<TIn, TOut> AddCollectionWithKeep<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty, TranslationPolicy translationPolicy, Func<TOutProperty, bool> mergeAndKeepTheseSelector) where TInProperty : class where TOutProperty : class;

        ITranslationDefinitions<TIn, TOut> AddCollectionWithRemove<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty, Func<TOutProperty, bool> whatRemoveSelector) where TInProperty : class where TOutProperty : class;

        ITranslationDefinitions<TIn, TOut> AddCollectionWithRemove<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty, TranslationPolicy translationPolicy, Func<TOutProperty, bool> whatRemoveSelector) where TInProperty : class where TOutProperty : class;

        ITranslationDefinitions<TIn, TOut> AddDictionary<TInProperty, TOutProperty, TKeyProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty, Expression<Func<TOut, Dictionary<TKeyProperty, TOutProperty>>> toProperty, Func<TInProperty, TKeyProperty> keyProperty) where TInProperty : class where TOutProperty : class;

        ITranslationDefinitions<TIn, TOut> AddDictionaryList<TInProperty, TOutProperty, TKeyProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty, Expression<Func<TOut, Dictionary<TKeyProperty, List<TOutProperty>>>> toProperty, Func<TInProperty, TKeyProperty> keyProperty) where TInProperty : class where TOutProperty : class;
        ITranslationDefinitions<TIn, TOut> AddDictionary<TInProperty, TOutProperty, TKeyProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty, Expression<Func<TOut, Dictionary<TKeyProperty, TOutProperty>>> toProperty, Func<TInProperty, TKeyProperty> keyProperty, bool keepUntouchedAndMerge) where TInProperty : class where TOutProperty : class;
        ITranslationDefinitions<TIn, TOut> AddLocalizable<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty, Expression<Func<TOut, TOutProperty>> toProperty, TOutProperty defaultValueIfNotFound = null) where TInProperty : class, ILocalizable where TOutProperty : class;

        ITranslationDefinitions<TIn, TOut> AddLocalizable<TInProperty, TOutProperty>(Func<TIn, TInProperty> fromProperty, Expression<Func<TOut, ICollection<TOutProperty>>> toProperty, Func<TIn, Func<TOutProperty, bool>> entitySelector = null) where TInProperty : class where TOutProperty : class, ILocalizable;

        ITranslationDefinitions<TIn, TOut> AddRequestLanguage<TOutLoc>(Expression<Func<TOut, TOutLoc>> localizable) where TOutLoc : class, TOut, ILocalizable;

        ITranslationDefinitions<TIn, TOut> AddLanguageAvailability<TOutLang>(Func<TOut, IMultilanguagedEntity<TOutLang>> languageAvailable) where TOutLang : class,ILanguageAvailability,new();
        ITranslationDefinitions<TIn, TOut> AddLanguageAvailability<TInLang, TOutLang>(Func<TIn, TInLang> fromProperty, Func<TOut, IMultilanguagedEntity<TOutLang>> languageAvailable) where TOutLang : class,ILanguageAvailability,new() where TInLang : class, ILanguagesAvailabilities;

        TOut GetFinal(TOut defaultValue = default(TOut));

        ITranslationDefinitions<TIn, TOut> DisableAutoTranslation();

        ITranslationDefinitions<TIn, TOut> AddSimple<TInProperty, TOutProperty>(Func<TIn, TInProperty> fromProperty, Expression<Func<TOut, TOutProperty>> toProperty)
            where TInProperty : struct where TOutProperty : struct;

        ITranslationDefinitions<TIn, TOut> AddSimple<TInProperty, TOutProperty>(Func<TIn, TInProperty?> fromProperty, Expression<Func<TOut, TOutProperty?>> toProperty)
            where TInProperty : struct where TOutProperty : struct;

        ITranslationDefinitions<TIn, TOut> AddSimple<TInProperty, TOutProperty>(Func<TIn, TInProperty> fromProperty, Expression<Func<TOut, TOutProperty?>> toProperty)
            where TInProperty : struct where TOutProperty : struct;

        ITranslationDefinitions<TIn, TOut> AddSimpleList<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty, Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty)
           where TInProperty : struct where TOutProperty : struct;
        

        ITranslationDefinitions<TIn, TOut> Propagation(Action<TIn, TOut> propagationAction);

        ITranslationDefinitions<TIn, TOut> Propagation(Action<TIn, TOut, TOut> propagationAction);

        ITranslationDefinitions<TIn, TOut> ApplyMarkAsKept<TProcess>(Func<TProcess, bool> markingCondition) where TProcess : class;
        
        ITranslationDefinitions<TIn, TOut> AddCollectionWithKeep<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty, Action<DefinitionDeepClause<TOutProperty>> deepConditions)
            where TInProperty : class where TOutProperty : class;

        ITranslationDefinitions<TIn, TOut> AddCollectionWithRemove<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty, Action<DefinitionDeepClause<TOutProperty>> deepConditions)
            where TInProperty : class where TOutProperty : class;
    }

    internal interface ITranslationDefinitionsEntityToVModel<TIn, TOut> : ITranslationDefinitions<TIn, TOut>
    {}

    internal interface ITranslationDefinitionsVModelToEntity<TIn, TOut> : ITranslationDefinitions<TIn, TOut>
    {
       
    }

    internal interface ITranslationDefinitionsForContextUsage<TIn, TOut> : ITranslationDefinitionsVModelToEntity<TIn, TOut>
    {
        new ITranslationDefinitionsForContextUsage<TIn, TOut> DisableAutoTranslation();
        ITranslationDefinitionsForContextUsage<TIn, TOut> DefineEntitySubTree(Func<IQueryable<TOut>, IQueryable<TOut>> subTreeIncludes);

        ITranslationDefinitionsForVersioning<TIn, TOut> UseDataContextCreate<TKeyProperty>(Func<TIn, bool> callCondition, Expression<Func<TOut, TKeyProperty>> outKey,
            Func<TIn, TKeyProperty> valueForNew);

        ITranslationDefinitionsForVersioning<TIn, TOut> UseDataContextCreate(Func<TIn, bool> callCondition = null);

        ITranslationDefinitionsForVersioning<TIn, TOut> UseDataContextLocalizedUpdate(Func<TIn, bool> callCondition, Func<TIn, Expression<Func<TOut, bool>>> entitySelector, Action<ITranslationDefinitionsForVersioning<TIn, TOut>> actionIfNotFound = null, bool allowOverridingExisting = false);
        ITranslationDefinitionsForVersioning<TIn, TOut> UseDataContextUpdate(Func<TIn, bool> callCondition, Func<TIn, Expression<Func<TOut, bool>>> entitySelector, Action<ITranslationDefinitionsForVersioning<TIn, TOut>> actionIfNotFound = null, bool allowOverridingExisting = false);


    }

    internal interface ITranslationDefinitionsForVersioning<TIn, TOut> : ITranslationDefinitionsForContextUsage<TIn, TOut>
    {
        ITranslationDefinitions<TIn, TOut> UseVersioning<TOutVersioned, TOutRoot>(Expression<Func<TOut, TOutVersioned>> versioned, VersioningMode versioningMode = VersioningMode.Standard,
            Func<TIn, bool> versioningCondition = null) where TOutRoot : class, IVersionedRoot, new()
            where TOutVersioned : class, TOut, IVersionedVolume<TOutRoot>, new();
    }
}
