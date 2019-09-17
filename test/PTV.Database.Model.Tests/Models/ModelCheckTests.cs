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
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using PTV.Database.Model.Models;
using Xunit;
using System.Reflection;
using Castle.Core.Internal;
using FluentAssertions.Common;
using PTV.Database.Model.Models.Attributes;
using PTV.Framework;
using PTV.Framework.Formatters.Attributes;
using PTV.Framework.Interfaces;

namespace PTV.Database.Model.Tests.Models
{
    public class ModelCheckTests
    {
        Dictionary<Type, IDataTypeValueCreator> typesGenerator = new Dictionary<Type, IDataTypeValueCreator>
        {
            { typeof(string), new StringValueCreator() },
//            { typeof(Guid), new GuidValueCreator() },
//            { typeof(Guid?), new GuidValueCreator() }
        };

        [Fact]
        public void ModelCollectionInitializationCheckTest()
        {

//            var type = typeof (PTV.Database.Model.Models.Base.EntityBase);
            var type = typeof (ServiceVersioned);
            var types = type.GetTypeInfo().Assembly.DefinedTypes.Where(x => x.IsClass && x.Namespace == type.Namespace).Where(i => !i.IsGenericType);
            StringBuilder errors = new StringBuilder();
            foreach (var typeInfo in types)
            {
                var entity = typeInfo.GetConstructor(new Type[] {}).Invoke(null);
                foreach (var property in typeInfo.GetProperties().Where(x => x.PropertyType.Name.Contains("ICollection")))
                {
                    if (property.GetValue(entity) == null)
                    {
                        var error = $"Property {property.Name} of {typeInfo.Name} has to be initialized in constructor.";
                        errors.AppendLine(error);
                    }
                }
            }
            Assert.True(errors.Length == 0, errors.ToString());
        }

        [Fact]
        public void ModelIntegrityCheckTest()
        {
            var type = typeof (ServiceVersioned);
            var types = type.GetTypeInfo().Assembly.DefinedTypes.Where(x => x.IsClass && x.Namespace == type.Namespace).Where(i => !i.IsGenericType);
            StringBuilder errors = new StringBuilder();
            foreach (var typeInfo in types)
            {
                CheckForeignKeyAttributes(typeInfo, errors);
                CheckValueFormatterAttributes(typeInfo, errors);
            }
            Assert.True(errors.Length == 0, errors.ToString());
        }

        private void CheckForeignKeyAttributes(TypeInfo typeInfo, StringBuilder errors)
        {
            foreach (var property in typeInfo.GetProperties().Where(x => x.Name != "Id" && (x.PropertyType == typeof(Guid) || x.PropertyType == typeof(Guid?))))
            {
                if (property.GetAttributes<NotMappedAttribute>().Any())
                {
                    continue;
                }
                bool notFk = property.GetAttributes<NotForeignKeyAttribute>().Any();
                if (!notFk)
                {
                    if (property.Name.EndsWith("Id") && typeInfo.GetProperties().All(x => (x.Name + "Id") != property.Name))
                    {
                        var error =
                            $"Property {property.Name} of {typeInfo.Name} has not valid navigation property.";
                        errors.AppendLine(error);
                    }
                }
                else
                {
                    if (typeInfo.GetProperties().Any(x => (x.Name + "Id") == property.Name))
                    {
                        if (property.GetAttributes<NotForeignKeyAttribute>().Any())
                        {
                            var error =
                                $"Property {property.Name} in entity {typeInfo.Name} of type {GetTypeName(property.PropertyType)} has navigation property, but is marked with {typeof(NotForeignKeyAttribute).Name}.";
                            errors.AppendLine(error);
                        }
                    }
                }
            }
        }

        private void CheckValueFormatterAttributes(TypeInfo typeInfo, StringBuilder errors)
        {
            var entity = typeInfo.GetConstructor(new Type[] {}).Invoke(new object[] {});
            var propertiesToFormat = typeInfo.GetProperties().Where(prop => prop.IsDefined(typeof(ValueFormatterAttribute), true));
            propertiesToFormat.ForEach(
                prop =>
                    prop.GetCustomAttributes<ValueFormatterAttribute>(true)
                        .OrderBy(attr => attr.Order)
                        .ForEach(attr =>
                        {
                            var formatter = attr.ValueFormatterType.GetConstructor(new Type[] { })?.Invoke(new object[] {  }) as IValueFormatter;

                            if (formatter != null)
                            {
                                prop.SetValue(entity, CreateValue(prop.PropertyType));
                                try
                                {
                                    formatter.Format(prop.GetValue(entity), entity.GetType().Name);
                                }
                                catch (ArgumentNullException)
                                {
                                    errors.AppendLine($"Property {prop.Name} in entity {typeInfo.Name} of type {GetTypeName(prop.PropertyType)} has invalid data formatter {attr.ValueFormatterType.Name}.");
                                }
                            }
                        }));
        }

        private object CreateValue(Type type)
        {
            IDataTypeValueCreator creator = typesGenerator.TryGet(type) ?? new DefaultValueCreator(type);
            return creator.Create();
        }

        private string GetTypeName(Type type)
        {
            string genericTypeInfo = type.GenericTypeArguments.Length > 0
                ? $"<{string.Join(", ", type.GenericTypeArguments.Select(x => x.Name))}>"
                : string.Empty;

            return $"{type.Name}{genericTypeInfo}";
        }
    }

    internal interface IDataTypeValueCreator
    {
        object Create();
    }

    internal abstract class DataTypeValueCreator<T> : IDataTypeValueCreator
    {
        protected DataTypeValueCreator()
        {
        }

        public Type DataType => typeof(T);

        public abstract T Create();

        object IDataTypeValueCreator.Create()
        {
            return Create();
        }
    }

    internal class StringValueCreator : DataTypeValueCreator<string>
    {
        public override string Create()
        {
            return "asdbhklfhdkl";
        }
    }
    internal class GuidValueCreator : DataTypeValueCreator<Guid>
    {
        public override Guid Create()
        {
            return Guid.NewGuid();
        }
    }

    internal class DefaultValueCreator : IDataTypeValueCreator
    {
        private Type type;
        public DefaultValueCreator(Type type)
        {
            this.type = type;
        }

        public object Create()
        {
            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsClass && typeInfo.GenericTypeArguments.Length > 0)
            {
                return Activator.CreateInstance(typeInfo.GenericTypeArguments.First());
            }
            return Activator.CreateInstance(type);
        }
    }
}
