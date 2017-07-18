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

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for ontology term list
    /// </summary>
    public class OntologyTermListValidator : BaseValidator<IList<string>>
    {
        private IFintoService fintoService;

        /// <summary>
        /// Ctor - ontology term list validator.
        /// </summary>
        /// <param name="model">Ontology term list</param>
        /// <param name="fintoService">Finto item service</param>
        /// <param name="propertyName">Property name</param>
        public OntologyTermListValidator(IList<string> model, IFintoService fintoService, string propertyName = "OntologyTerms") : base(model, propertyName)
        {
            this.fintoService = fintoService;
        }

        /// <summary>
        /// Checks if ontology term list is valid or not.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            var i = 0;
            Model.ForEach(uri =>
            {
                var item = fintoService.GetOntologyTermByUri(uri);
                if (item == null || !item.Id.IsAssigned())
                {
                    modelState.AddModelError($"{PropertyName}[{ i++ }]", string.Format(CoreMessages.OpenApi.CodeNotFound, uri));
                }
            });
        }
    }
}
