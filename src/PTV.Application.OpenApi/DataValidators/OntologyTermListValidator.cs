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

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Caches.Finto;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for ontology term list
    /// </summary>
    public class OntologyTermListValidator : BaseValidator<IList<string>>
    {
        private readonly int validCount;
        private readonly IOntologyTermDataCache ontologyTermDataCache;

        /// <summary>
        /// Ctor - ontology term list validator.
        /// </summary>
        /// <param name="model">Ontology term list</param>
        /// <param name="ontologyTermDataCache">Finto item service</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="validCount">Indicates what is the valid number of items within the list. If set to zero item count is ignored.</param>
        public OntologyTermListValidator(IList<string> model, IOntologyTermDataCache ontologyTermDataCache, string propertyName = "OntologyTerms", int validCount = 0) : base(model, propertyName)
        {
            this.ontologyTermDataCache = ontologyTermDataCache;
            this.validCount = validCount;
        }

        /// <summary>
        /// Amount of service classes that are attached with related general description.
        /// </summary>
        public int GeneralDescriptionOntologyTermCount { get; set; }


        /// <summary>
        /// Checks if ontology term list is valid or not.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null || Model?.Count == 0) return;

            // Validate the item count (PTV-4395)
            if (validCount > 0)
            {
                var amount = Model?.Count;
                if (GeneralDescriptionOntologyTermCount > 0)
                {
                    amount += GeneralDescriptionOntologyTermCount;
                }
                if (amount > validCount)
                {
                    modelState.AddModelError($"{PropertyName}", $"Only {validCount} items allowed!");
                    if (GeneralDescriptionOntologyTermCount > 0)
                    {
                        modelState.AddModelError($"{PropertyName}", $"Attached general description already includes {GeneralDescriptionOntologyTermCount} ontology terms!");
                    }
                }
            }

            var notExistingUris = Model.Except(ontologyTermDataCache.HasUris(Model)).ToList();
            if (notExistingUris.Any())
            {
                modelState.AddModelError(PropertyName, $"Some of the uris were not found: '{string.Join(", ", notExistingUris)}'");
            }
        }
    }
}
