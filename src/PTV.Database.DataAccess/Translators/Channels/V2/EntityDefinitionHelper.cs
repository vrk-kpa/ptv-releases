/**
* The MIT License
* Copyright (c) 2020 Finnish Digital Agency (DVV)
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
using PTV.Database.DataAccess.Caches;
using PTV.Database.DataAccess.Interfaces.Caches;
using PTV.Database.DataAccess.Interfaces.Translators;
using PTV.Database.Model.Interfaces;
using PTV.Database.Model.Models.Base;
using PTV.Domain.Model.Models;
using PTV.Domain.Model.Models.Interfaces;
using PTV.Domain.Model.Models.Interfaces.V2;
using PTV.Framework;

namespace PTV.Database.DataAccess.Translators.Channels.V2
{
    [RegisterService(typeof(EntityDefinitionHelper), RegisterType.Scope)]
    internal class EntityDefinitionHelper
    {
        public ICacheManager CacheManager { get; private set; }

        public EntityDefinitionHelper(ICacheManager cacheManager)
        {
            CacheManager = cacheManager;
        }
        public ITranslationDefinitionsForContextUsage<TIn, TOut> GetDefinitionWithCreateOrUpdate<TIn, TOut>(ITranslationDefinitionsForContextUsage<TIn, TOut> definition) where TIn : VmEntityBase where TOut : IEntityIdentifier
        {
            return definition
                .DisableAutoTranslation()
                .UseDataContextCreate(input => !input.Id.IsAssigned(), output => output.Id, input => Guid.NewGuid())
                .UseDataContextUpdate(input => input.Id.IsAssigned(), input => output => input.Id == output.Id);

        }

        public ITranslationDefinitions<TIn, TOut> AddLanguageAvailabilitiesDefinition<TIn, TOut, TLanguagesAvailability>(ITranslationDefinitions<TIn, TOut> definition, ILanguageOrderCache orderCache) where TIn : IMultilanguagedEntity<TLanguagesAvailability> where TOut : ILanguagesAvailabilities where TLanguagesAvailability : LanguageAvailability
        {
            return definition.AddCollection<ILanguageAvailability, VmLanguageAvailabilityInfo>(
                i => i.LanguageAvailabilities.OrderBy(x => orderCache.Get(x.LanguageId)),
                o => o.LanguagesAvailabilities);
        }

        public EntityDefinitionHelper AddOrderedCollectionWithRemove<TIn, TOut, TInProperty, TOutProperty>(
            ITranslationDefinitions<TIn, TOut> definition,
            Func<TIn, IDictionary<string, List<TInProperty>>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty,
            Func<TOutProperty, bool> whatRemoveSelector,
            Action<TIn, TInProperty, string> updateItem = null
        ) where TInProperty : class, IVmOrderable
          where TOutProperty : class, IOrderable
        {
            definition.AddCollectionWithRemove(
                input => fromProperty(input)?.SelectMany(pair =>
                {
                    return pair.Value.Select((value, index) =>
                    {
                        value.OrderNumber = index;
                        updateItem?.Invoke(input, value, pair.Key);
                        return value;
                    });
                }),
                toProperty,
                whatRemoveSelector
            );
            return this;
        }
        public EntityDefinitionHelper AddOrderedCollectionWithRemove<TIn, TOut, TInProperty, TOutProperty>(
            ITranslationDefinitions<TIn, TOut> definition,
            Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty,
            Func<TOutProperty, bool> whatRemoveSelector,
            Action<TIn, TInProperty> updateItem = null
        ) where TInProperty : class, IVmOrderable
          where TOutProperty : class, IOrderable
        {
            definition.AddCollectionWithRemove(
                input => fromProperty(input)?.Select((item, index) => {
                    item.OrderNumber = index;
                    updateItem?.Invoke(input, item);
                    return item;
                }),
                toProperty,
                whatRemoveSelector
            );
            return this;
        }
        public EntityDefinitionHelper AddOrderedCollection<TIn, TOut, TInProperty, TOutProperty>(
            ITranslationDefinitions<TIn, TOut> definition,
            Func<TIn, IDictionary<string, List<TInProperty>>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty,
            bool keepUntouchedAndMerge,
            Action<TIn, TInProperty, string> updateItem = null
        ) where TInProperty : class, IVmOrderable
          where TOutProperty : class, IOrderable
        {
            definition.AddCollection(
                input => fromProperty(input)?.SelectMany(pair =>
                {
                    return pair.Value.Select((value, index) =>
                    {
                        value.OrderNumber = index;
                        updateItem?.Invoke(input, value, pair.Key);
                        return value;
                    });
                }),
                toProperty,
                keepUntouchedAndMerge
            );
            return this;
        }
        public EntityDefinitionHelper AddOrderedCollection<TIn, TOut, TInProperty, TOutProperty>(
            ITranslationDefinitions<TIn, TOut> definition,
            Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty,
            bool keepUntouchedAndMerge,
            Action<TIn, TInProperty> updateItem = null
        ) where TInProperty : class, IVmOrderable
          where TOutProperty : class, IOrderable
        {
            definition.AddCollection(
                input => fromProperty(input)?.Select((item, index) => {
                    item.OrderNumber = index;
                    updateItem?.Invoke(input, item);
                    return item;
                }),
                toProperty,
                keepUntouchedAndMerge
            );
            return this;
        }

        public EntityDefinitionHelper AddOrderedDictionaryList<TIn, TOut, TInProperty, TOutProperty, TKeyProperty>(
            ITranslationDefinitions<TIn, TOut> definition,
            Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, Dictionary<TKeyProperty, List<TOutProperty>>>> toProperty,
            Func<TInProperty, TKeyProperty> keyProperty
        ) where TInProperty : class, IOrderable
          where TOutProperty : class, IVmOrderable
        {
            definition.AddDictionaryList(
                input => fromProperty(input).OrderBy(x => x.OrderNumber),
                toProperty,
                keyProperty
            );
            return this;
        }
        public EntityDefinitionHelper AddOrderedCollection<TIn, TOut, TInProperty, TOutProperty>(
            ITranslationDefinitions<TIn, TOut> definition,
            Func<TIn, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty
        ) where TInProperty : class, IOrderable
          where TOutProperty : class, IVmOrderable
        {
            definition.AddCollection(
                input => fromProperty(input).OrderBy(x => x.OrderNumber),
                toProperty
            );
            return this;
        }
        public EntityDefinitionHelper AddOrderedSimpleList<TIn, TOut, TInProperty, TOutProperty, TOrderable>(
            ITranslationDefinitions<TIn, TOut> definition,
            Func<TIn, IEnumerable<TOrderable>> fromOrderable,
            Func<IEnumerable<TOrderable>, IEnumerable<TInProperty>> fromProperty,
            Expression<Func<TOut, IEnumerable<TOutProperty>>> toProperty
        ) where TInProperty : struct
          where TOutProperty : struct
          where TOrderable : IOrderable
        {
            definition.AddSimpleList(
                input => fromProperty(fromOrderable(input).OrderBy(x => x.OrderNumber)),
                toProperty
            );
            return this;
        }
    }
}
