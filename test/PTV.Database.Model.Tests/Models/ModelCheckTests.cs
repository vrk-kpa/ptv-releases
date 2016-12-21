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
using System.Text;
using PTV.Database.Model.Models;
using Xunit;
using System.Reflection;
using Castle.Core.Internal;
using PTV.Database.Model.Models.Attributes;

namespace PTV.Database.Model.Tests.Models
{
    public class ModelCheckTests
    {
        [Fact]
        public void ModelCollectionInitializationCheckTest()
        {

//            var type = typeof (PTV.Database.Model.Models.Base.EntityBase);
            var type = typeof (Service);
            var types = type.GetTypeInfo().Assembly.DefinedTypes.Where(x => x.IsClass && x.Namespace == type.Namespace);
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
        public void ModelForeignKeyCheckTest()
        {
            var type = typeof (Service);
            var types = type.GetTypeInfo().Assembly.DefinedTypes.Where(x => x.IsClass && x.Namespace == type.Namespace);
            StringBuilder errors = new StringBuilder();
            foreach (var typeInfo in types)
            {
                // User id check temporary for UserOrganization table - needs to be changed
                foreach (var property in typeInfo.GetProperties().Where(x => x.Name != "Id" && (x.PropertyType == typeof(Guid) || x.PropertyType == typeof(Guid?))))
                {
                    bool notFk = property.GetAttributes<NotForeignKeyAttribute>().Any();
                    if (!notFk)
                    {
                        if (typeInfo.GetProperties().All(x => (x.Name + "Id") != property.Name))
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
                                var error = $"Property {property.Name} of {typeInfo.Name} has navigation property, but is marked with {typeof(NotForeignKeyAttribute).Name}.";
                                errors.AppendLine(error);
                            }
                        }
                    }
                }
            }
            Assert.True(errors.Length == 0, errors.ToString());
        }
    }
}
