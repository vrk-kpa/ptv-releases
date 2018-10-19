﻿/**
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

using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;
using System.Collections.Generic;
using System.Linq;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for language list
    /// </summary>
    public class LanguageListValidator : BaseValidator<IList<string>>
    {
        private ICodeService codeService;

        /// <summary>
        /// Ctor - language code list validator.
        /// </summary>
        /// <param name="model">Language code list</param>
        /// <param name="codeService">Code service</param>
        /// <param name="propertyName">Property name</param>
        public LanguageListValidator(IList<string> model, ICodeService codeService, string propertyName = "Languages") : base(model, propertyName)
        {
            this.codeService = codeService;
        }

        /// <summary>
        /// Checks if language code list is valid or not.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            var notExistingCodes = codeService.CheckLanguages(Model);
            if (notExistingCodes?.Count > 0)
            {
                modelState.AddModelError(PropertyName, $"Some of the codes were not found: '{string.Join(", ", notExistingCodes)}'");
            }
        }
    }
}
