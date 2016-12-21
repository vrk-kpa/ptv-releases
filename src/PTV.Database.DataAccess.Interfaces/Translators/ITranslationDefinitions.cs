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
using System.Linq.Expressions;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models;

namespace PTV.Database.DataAccess.Interfaces.Translators
{
    internal interface ITranslationDefinitions<TIn, TOut>
    {void SetTnit(TIn source);

        ITranslationDefinitions<TIn, TOut> AddPartial<TInProperty>(Func<TIn, TInProperty> fromProperty) where TInProperty : class;

        ITranslationDefinitions<TIn, TOut> AddNavigation<TInProperty, TOutProperty>(Func<TIn, TInProperty> fromProperty, Expression<Func<TOut, TOutProperty>> toProperty, bool useTargetIfExisting = false) where TInProperty : class where TOutProperty : class;

        ITranslationDefinitions<TIn, TOut> AddNavigationOneMany<TInProperty, TOutProperty>(Func<TIn, TInProperty> fromProperty, Expression<Func<TOut, ICollection<TOutProperty>>> toProperty, bool useTargetIfExisting = false) where TInProperty : class where TOutProperty : class;

        ITranslationDefinitions<TIn, TOut> AddCollection<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty, Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty) where TInProperty : class where TOutProperty : class;

       // ITranslationDefinitions<TIn, TOut> AddLocalizable<TInProperty, TOutProperty>(Func<TIn, ICollection<TInProperty>> fromProperty, Expression<Func<TOut, TOutProperty>> toProperty, TOutProperty defaultValueIfNotFound = null) where TInProperty : class, ILocalizable where TOutProperty : class;

        ITranslationDefinitions<TIn, TOut> AddLocalizable<TInProperty, TOutProperty>(Func<TIn, IEnumerable<TInProperty>> fromProperty, Expression<Func<TOut, TOutProperty>> toProperty, TOutProperty defaultValueIfNotFound = null) where TInProperty : class, ILocalizable where TOutProperty : class;

        ITranslationDefinitions<TIn, TOut> AddLocalizable<TInProperty, TOutProperty>(Func<TIn, TInProperty> fromProperty, Expression<Func<TOut, ICollection<TOutProperty>>> toProperty, Func<TIn, Func<TOutProperty, bool>> entitySelector = null) where TInProperty : class where TOutProperty : class, ILocalizable;
        //ITranslationDefinitions<TIn, TOut> AddRequestLanguage<TOutProperty>(Expression<Func<TOut, TOutProperty>> toProperty) where TOutProperty : Language;

        ITranslationDefinitions<TIn, TOut> AddRequestLanguage<TOutLoc>(Expression<Func<TOut, TOutLoc>> localizable) where TOutLoc : class, TOut, ILocalizable;

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

        ITranslationDefinitions<TIn, TOut> UseDataContextCreate<TKeyProperty>(Func<TIn, bool> callCondition, Expression<Func<TOut, TKeyProperty>> outKey,
            Func<TIn, TKeyProperty> valueForNew);

        ITranslationDefinitions<TIn, TOut> UseDataContextCreate(Func<TIn, bool> callCondition);

        ITranslationDefinitions<TIn, TOut> UseDataContextLocalizedUpdate(Func<TIn, bool> callCondition, Func<TIn, Expression<Func<TOut, bool>>> entitySelector, Action<ITranslationDefinitions<TIn, TOut>> actionIfNotFound = null);
        ITranslationDefinitions<TIn, TOut> UseDataContextUpdate(Func<TIn, bool> callCondition, Func<TIn, Expression<Func<TOut, bool>>> entitySelector, Action<ITranslationDefinitions<TIn, TOut>> actionIfNotFound = null);
    }

    internal interface ITranslationDefinitionsEntityToVModel<TIn, TOut> : ITranslationDefinitions<TIn, TOut>
    {}

    internal interface ITranslationDefinitionsVModelToEntity<TIn, TOut> : ITranslationDefinitions<TIn, TOut>
    {}
}
