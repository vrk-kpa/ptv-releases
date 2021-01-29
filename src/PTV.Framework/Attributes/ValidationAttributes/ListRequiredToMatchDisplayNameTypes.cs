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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Linq;

namespace PTV.Framework.Attributes
{
    /// <summary>
    /// Validates that the list contains an item with values matching to required property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ListRequiredToMatchDisplayNameTypes : ValidationAttribute
    {
        private string linkedListPropertyName; // List property to match against

        public ListRequiredToMatchDisplayNameTypes(string linkedListPropertyName) : base()
        {
            this.linkedListPropertyName = linkedListPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && !(value is IList))
            {
                throw new InvalidOperationException("Attribute used on a type that doesn't implement IList.");
            }

            var instance = validationContext.ObjectInstance;
            var list = value as IList;
            var type = instance.GetType();

            var linkedListProperty = type.GetProperty(linkedListPropertyName);
            var linkedProperty = linkedListProperty.GetValue(instance);

            if (linkedProperty != null && !(linkedProperty is IList))
            {
                throw new InvalidOperationException("Attribute configured to target a linked list property type that doesn't implement IList.");
            }

            var linkedList = linkedProperty as IList;

            // Both types must be IList
            if (list == null || linkedList == null)
            {
                return null;
            }

            var linkedListLanguages = new List<string>();
            var listLanguages = new List<string>();

            // Verify that each linked list property value pair exists in current list
            // Additionally verify that all languages present in list exist in linked list
            foreach (var linkedListItem in linkedList)
            {
                if (linkedListItem == null)
                {
                    throw new InvalidOperationException("There is a null item in items list.");
                }

                var linkedListItemLanguage = linkedListItem.GetItemValue("Language") as string;
                var linkedListItemType = linkedListItem.GetItemValue("Type") as string;

                // Collect distinct languages from linked list
                if (!linkedListLanguages.Contains(linkedListItemLanguage))
                {
                    linkedListLanguages.Add(linkedListItemLanguage);
                }

                bool found = false;
                foreach (var listItem in list)
                {
                    if (listItem == null)
                    {
                        throw new InvalidOperationException("There is a null item in items list.");
                    }

                    var listItemLanguage = listItem.GetItemValue("Language") as string;
                    var listItemType = listItem.GetItemValue("Type") as string;

                    // Collect distinct languages from list
                    if (!listLanguages.Contains(listItemLanguage))
                    {
                        listLanguages.Add(listItemLanguage);
                    }

                    if (!found)
                    {
                        // Both lists contain a matching item
                        found = linkedListItemLanguage == listItemLanguage && linkedListItemType == listItemType;
                    }
                }

                if (!found)
                {
                    return new ValidationResult(string.Format(CoreMessages.OpenApi.ListRequiredToMatchDisplayNameTypesMissingNames, linkedListPropertyName, linkedListItemLanguage, linkedListItemType));
                }
            }

            // Get missing languages
            var missingLanguages = listLanguages.Except(linkedListLanguages);

            if (missingLanguages.Any())
            {
                return new ValidationResult(string.Format(CoreMessages.OpenApi.ListRequiredToMatchDisplayNameTypesMissingTypes, linkedListPropertyName, string.Join(", ", missingLanguages)));
            }

            return null;
        }
    }
}
