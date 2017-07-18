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
using System.Threading.Tasks;

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
        private static readonly string namespaceRestriction = $"{nameof(PTV.Domain.Model.Models)}.{nameof(PTV.Domain.Model.Models.OpenApi)}";

        /// <summary>
        /// Apply schema operations for types in PTV.Domain.Model.Models.OpenApi namespace
        /// </summary>
        public void Apply(Schema schema, SchemaFilterContext context)
        {
            var typeInfo = context.SystemType.GetTypeInfo();

            if (typeInfo.Namespace.Contains(namespaceRestriction))
            {
                // Get all type properties (also inherited), exclude JsonIgnored
                foreach (var property in typeInfo.GetProperties().Where(t => !t.CustomAttributes.Any(c => c.AttributeType == typeof(JsonIgnoreAttribute))))
                {
                    // Get all MaxLength property attributes (also inherited)
                    var lengthAttributes = property.GetCustomAttributes(true)
                        .Where(a => 
                            a.GetType() == typeof(MaxLengthAttribute) || 
                            a.GetType() == typeof(ListPropertyMaxLengthAttribute)
                        ).ToList();

                    // Add attribute lengths to Swagger definition
                    foreach (var attribute in lengthAttributes)
                    {
                        var listAttribute = attribute as ListPropertyMaxLengthAttribute;

                        // Get ListPropertyMaxLength OR MaxLength
                        var length = listAttribute?.Length ?? (attribute as MaxLengthAttribute)?.Length;
                        var listTypeValue = listAttribute?.TypeValue?.Trim();

                        // Get Json PropertyName if defined, otherwise use property name
                        var attributeName = ((JsonPropertyAttribute)property.GetCustomAttribute(typeof(JsonPropertyAttribute)))?.PropertyName ?? property.Name;
                        var definition = schema.Properties.Where(i => i.Key.ToLower() == attributeName.ToLower()).FirstOrDefault();
                        if (definition.Value != null)
                        {
                            definition.Value.Description += string.Format(" (Max.Length: {0}{1}).", length, !listTypeValue.IsNullOrEmpty() ? $" {listTypeValue}" : null);
                        }
                    }
                }
            }
        }
    }
}
