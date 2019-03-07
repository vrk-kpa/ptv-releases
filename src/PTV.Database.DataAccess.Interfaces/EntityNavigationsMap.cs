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
using PTV.Framework;

namespace PTV.Database.DataAccess.Interfaces
{
    [RegisterService(typeof(EntityNavigationsMap), RegisterType.Singleton)]
    [RegisterService(typeof(IEntityNavigationsMap), RegisterType.Singleton)]
    internal class EntityNavigationsMap : IEntityNavigationsMap
    {
        public Dictionary<Type, EntityPropertiesDefinition> NavigationsMap { get; set; }

        public void SetForeignKey<TType>(TType entity, string navigation, object value)
        {
            var type = typeof(TType);
            EntityPropertiesDefinition definitionPairs;
            if (!NavigationsMap.TryGetValue(type, out definitionPairs)) return;
            var foreignKey = definitionPairs.NavigationsMap.FirstOrDefault(i => i.Navigation.Name.Contains(navigation))?.ForeignKeys?.FirstOrDefault()?.Name;
            type.GetProperty(foreignKey).SetValue(entity, value);
        }

        public void SetForeignKey<TType, TProperty>(TType entity, Expression<Func<TType, TProperty>> toProperty, object value)
        {
            var memberSelectorExpression = toProperty.Body as MemberExpression;
            var property = memberSelectorExpression?.Member as PropertyInfo;
            if (property == null) return;
            SetForeignKey(entity, property.Name, value);
        }


        public void NullNavigation<TType, TProperty>(TType instance, Expression<Func<TType, TProperty>> toProperty)
        {
//            var type = typeof(TType);
//            List<EntityFrameworkEntityTools.ForeignKeyNavigationPair> definitionPairs;
//            if (!NavigationsMap.TryGetValue(type, out definitionPairs)) return;
//            var memberSelectorExpression = toProperty.Body as MemberExpression;
//            var property = memberSelectorExpression?.Member as PropertyInfo;
//            if (property == null) return;
//            var navigationsToNull = definitionPairs.Where(i => i.ForeignKeys.Contains(property.Name));
//            foreach (var navigationToNull in navigationsToNull)
//            {
//                type.GetProperty(navigationToNull.Navigation).SetValue(instance, null);
//            }
        }
    }
}
