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
using Newtonsoft.Json;
using PTV.Framework;
using PTV.Framework.Attributes;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Models;

namespace PTV.Application.OpenApi.Swagger
{
    /// <summary>
    /// Swagger schema filter for maxlength descriptions
    /// </summary>
    public class SwaggerSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// Restrict schema operations to Models under OpenApi namespace
        /// </summary>
        private static readonly string NamespaceRestriction = $"{nameof(Domain.Model.Models)}.{nameof(Domain.Model.Models.OpenApi)}";

        /// <summary>
        /// Apply schema operations for types in PTV.Domain.Model.Models.OpenApi namespace
        /// </summary>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var typeInfo = context.GetType().GetTypeInfo();

            if (typeInfo.Namespace != null && typeInfo.Namespace.Contains(NamespaceRestriction))
            {
                // Get all properties with validation attributes, exclude JsonIgnored, order by Type hierarchy
                var validatedProperties = GetOrderedProperties(context.GetType());

                // For each property add attribute types which are meant to override base type definitions
                // such as MaxLengthAttribute (ie. declared overrides inherited)
                var nonAdditiveAttributesAdded = new HashSet<string>();

                foreach (var property in validatedProperties)
                {
                    // Get all property attributes (also inherited) for:
                    // MaxLength, ListPropertyMaxLength, ListRequired and ListRegularExpression
                    var attributes = property.GetCustomAttributes(true)
                        .Where(a =>
                            a.GetType() == typeof(MaxLengthAttribute) ||
                            a.GetType() == typeof(ListPropertyMaxLengthAttribute) ||
                            a.GetType() == typeof(ListRequiredAttribute) ||
                            a.GetType() == typeof(ListRegularExpressionAttribute)
                        ).ToList();

                    if (attributes.Count == 0)
                    {
                        continue;
                    }

                    // Get Json PropertyName if defined, otherwise use property name
                    var propertyName = ((JsonPropertyAttribute)property.GetCustomAttribute(typeof(JsonPropertyAttribute)))?.PropertyName ?? property.Name;
                    var definition = schema.Properties.FirstOrDefault(i => i.Key.ToLower() == propertyName.ToLower());

                    var nonAdditiveAttributes = new HashSet<string>();

                    foreach (var attribute in attributes)
                    {
                        var attributeTypeName = attribute.GetType().Name;
                        var nonAdditiveMemberKey = propertyName + attributeTypeName;

                        switch (attributeTypeName)
                        {
                            // Add attribute lengths to Swagger definition
                            // Treat both MaxLengthAttributes similarly
                            case nameof(ListPropertyMaxLengthAttribute):
                            case nameof(MaxLengthAttribute):
                                {
                                    if (nonAdditiveAttributesAdded.Contains(nonAdditiveMemberKey))
                                    {
                                        continue;
                                    }

                                    var listAttribute = attribute as ListPropertyMaxLengthAttribute;

                                    // Get ListPropertyMaxLength OR MaxLength
                                    var length = listAttribute?.Length ?? (attribute as MaxLengthAttribute)?.Length;
                                    var listTypeValue = listAttribute?.TypeValue?.Trim();

                                    if (definition.Value != null)
                                    {
                                        if (!nonAdditiveAttributes.Contains(nonAdditiveMemberKey))
                                        {
                                            nonAdditiveAttributes.Add(nonAdditiveMemberKey);
                                        }
                                        definition.Value.Description +=
                                            $" (Max.Length: {length}{(!listTypeValue.IsNullOrEmpty() ? $" {listTypeValue}" : null)}).";
                                    }
                                }
                                break;
                            case nameof(ListRequiredAttribute):
                                {
                                    // Add the key to schema required collection
                                    if (schema.Required == null)
                                    {
                                        schema.Required = new HashSet<string>();
                                    }
                                    schema.Required.Add(definition.Key);
                                }
                                break;
                            case nameof(ListRegularExpressionAttribute):
                                {
                                    // Add the regular expression pattern to definition
                                    if (definition.Value != null)
                                    {
                                        definition.Value.Pattern = (attribute as ListRegularExpressionAttribute)?.Pattern;
                                    }
                                }
                                break;
                        }
                    }

                    nonAdditiveAttributes.ForEach(x => nonAdditiveAttributesAdded.Add(x));
                }
            }
        }

        /// <summary>
        /// Get properties with validation attributes ordered (declared properties before inherited).
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private IEnumerable<PropertyInfo> GetOrderedProperties(Type type)
        {
            var properties = type.GetProperties()
                .Where(p =>
                    p.GetCustomAttributes(true).Any(c => c is ValidationAttribute) &&
                    !p.GetCustomAttributes(true).Any(c => c is JsonIgnoreAttribute)
                ).ToArray();

            // Sort declared before inherited properties
            Array.Sort(properties, (pi1, pi2) =>
            {
                if (pi1?.DeclaringType == null || pi2?.DeclaringType == null)
                {
                    return 0;
                }

                if (pi1.DeclaringType.GetTypeInfo().IsSubclassOf(pi2.DeclaringType))
                    return -1;
                if (pi2.DeclaringType.GetTypeInfo().IsSubclassOf(pi1.DeclaringType))
                    return 1;
                return 0;
            });

            return properties;
        }
    }
}
