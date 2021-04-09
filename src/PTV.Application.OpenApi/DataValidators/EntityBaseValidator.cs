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

using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Domain.Model.Enums;
using PTV.Domain.Model.Models.Interfaces.OpenApi;
using PTV.Domain.Model.Models.OpenApi;
using PTV.Domain.Model.Models.OpenApi.Extensions;
using PTV.Domain.Model.Models.OpenApi.V4;
using PTV.Domain.Model.Models.OpenApi.V9;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Base validator for entity.
    /// </summary>
    public abstract class EntityBaseValidator<TModel> : BaseValidator<TModel> where TModel : IOpenApiEntity
    {
        private readonly IList<string> newAddedLanguages;
        private readonly bool isRemovingLanguageVersions;

        /// <summary>
        /// Required languages
        /// </summary>
        protected IList<string> RequiredLanguages;

        /// <summary>
        /// Ctor - base for timed publishing item validator
        /// </summary>
        public EntityBaseValidator(TModel model, IList<string> currentVersionAvailableLanguages, string propertyName) : base(model, propertyName)
        {
            if (model == null)
            {
                throw new ArgumentNullException(PropertyName, $"{PropertyName} must be defined.");
            }

            // SFIPTV-1913 - The required property lists needs to contain required languages.
            // If there is no items within required property lists (no changes made) the newly added languages should exist in required property lists.
            // If there are items within required property lists, all the required property lists should include all the available languages.
            // If user has not added/removed any new language versions the required property lists can be also empty.
            newAddedLanguages = currentVersionAvailableLanguages == null ? model.AvailableLanguages : model.AvailableLanguages.Where(i => !currentVersionAvailableLanguages.Contains(i)).ToList();
            isRemovingLanguageVersions = currentVersionAvailableLanguages != null && currentVersionAvailableLanguages.Except(model.AvailableLanguages).Count() > 0;
            var isAddingNewLanguages = newAddedLanguages?.Count() > 0;
            RequiredLanguages = model.RequiredPropertiesAvailableLanguages?.Count == 0 ? newAddedLanguages
                : !isRemovingLanguageVersions && !isAddingNewLanguages ? null
                : model.AvailableLanguages;

        }

        /// <summary>
        /// Checks if service model is valid or not.
        /// </summary>
        public override void Validate(ModelStateDictionary modelState)
        {

            // SFIPTV-1913
            // If there are more languages in available languages list it means that some of the optional arrays contains language(s) that is not
            // within required property language lists. User either needs to add the language versions into required language lists or
            // user needs to remove the additional language.
            // If updating items and there are no changes within required property lists they can be left empty!!!
            if (Model.RequiredPropertiesAvailableLanguages?.Count < Model.AvailableLanguages?.Count)
            {
                // Has the user added new languages that only exist within optional parameter lists? Show an error.
                if (newAddedLanguages?.Count > 0)
                {
                    var listLang = string.Join(", ", newAddedLanguages);
                    modelState.AddModelError("Model", $"You have added new language version(s): {listLang}. Either add required values in '{listLang}' or remove all items in language '{listLang}'.");
                    return;
                }
            }
            // SFIPTV-1913
            // If all the required lists do not contain all the available languages show error.
            // If user is removing some of the language versions all required fields needs to be set.
            var missingLanguagesLists = GetPropertyListsWhereMissingLanguages();
            if (missingLanguagesLists?.Count > 0)
            {
                var strMissing = string.Join(", ", missingLanguagesLists);
                var strRequiredLanguages = string.Join(", ", RequiredLanguages);
                if (isRemovingLanguageVersions)
                {
                    modelState.AddModelError("Model", $"You are removing language versions so all the required lists need to include required languages. Check lists '{strMissing} for languages '{strRequiredLanguages}'.");
                }
                else
                {
                    modelState.AddModelError("Model", $"Some of the required lists are missing the languages you have added. Check lists '{strMissing} for languages '{strRequiredLanguages}'.");
                }
                return;
            }
        }

        /// <summary>
        /// Get the required property list names where languages do not exist.
        /// </summary>
        /// <returns></returns>
        protected abstract IList<string> GetPropertyListsWhereMissingLanguages();

        /// <summary>
        /// Return true if some of the required languages are missing from the list.
        /// </summary>
        /// <param name="propertyList"></param>
        /// <returns></returns>
        protected bool IsLanguagesMissing(IList<VmOpenApiLocalizedListItem> propertyList)
        {
            if (RequiredLanguages == null || RequiredLanguages.Count == 0)
            {
                return false;
            }
            var list = new HashSet<string>();
            list.GetAvailableLanguages(propertyList);
            var notExistingLanguages = RequiredLanguages.Except(list);
            if (notExistingLanguages.Count() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return true if some of the required languages are missing from the list.
        /// </summary>
        /// <param name="propertyList"></param>
        /// <returns></returns>
        protected bool IsLanguagesMissing(IList<VmOpenApiLanguageItem> propertyList)
        {
            if (RequiredLanguages == null || RequiredLanguages.Count == 0)
            {
                return false;
            }
            var list = new HashSet<string>();
            list.GetAvailableLanguages(propertyList);
            var notExistingLanguages = RequiredLanguages.Except(list);
            if (notExistingLanguages.Count() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return true if some of the required languages are missing from the list.
        /// </summary>
        /// <param name="propertyList"></param>
        /// <returns></returns>
        protected bool IsLanguagesMissing(IList<V9VmOpenApiAddressLocationIn> propertyList)
        {
            if (RequiredLanguages == null || RequiredLanguages.Count == 0)
            {
                return false;
            }
            var list = new HashSet<string>();
            list.GetAvailableLanguages(propertyList);
            var notExistingLanguages = RequiredLanguages.Except(list);
            if (notExistingLanguages.Count() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return true if some of the required languages are missing from the list.
        /// </summary>
        /// <param name="propertyList"></param>
        /// <returns></returns>
        protected bool IsLanguagesMissing(IList<V4VmOpenApiPhoneWithType> propertyList)
        {
            if (RequiredLanguages == null || RequiredLanguages.Count == 0)
            {
                return false;
            }
            var list = new HashSet<string>();
            list.GetPhoneAvailableLanguages(propertyList);
            var notExistingLanguages = RequiredLanguages.Except(list);
            if (notExistingLanguages.Count() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return true if some of the required languages are missing from the list.
        /// </summary>
        /// <param name="propertyList"></param>
        /// <returns></returns>
        protected bool IsLanguagesMissing(IList<VmOpenApiAccessibilityClassification> propertyList)
        {
            if (RequiredLanguages == null || RequiredLanguages.Count == 0)
            {
                return false;
            }
            var list = new HashSet<string>();
            list.GetAvailableLanguages(propertyList);
            var notExistingLanguages = RequiredLanguages.Except(list);
            if (notExistingLanguages.Count() > 0)
            {
                return true;
            }
            return false;
        }
    }
}
