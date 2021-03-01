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
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Domain.Model.Enums;
using PTV.Framework;
using PTV.Framework.Extensions;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for area code.
    /// </summary>
    public class AreaValidator : BaseValidator<string>
    {
        private readonly string type;
        private readonly ICodeService codeService;

        /// <summary>
        /// Ctor - area code validator.
        /// </summary>
        /// <param name="model">Area code</param>
        /// <param name="type">Type of the area</param>
        /// <param name="codeService">Code service</param>
        /// <param name="propertyName">Property name</param>
        public AreaValidator(string model, string type, ICodeService codeService, string propertyName = "Area") : base(model, propertyName)
        {
            this.type = type;
            this.codeService = codeService;
        }

        /// <summary>
        /// Validates area code.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (string.IsNullOrEmpty(Model))
            {
                return;
            }

            var area = codeService.GetAreaByCodeAndType(Model, type.GetEnumValueByOpenApiEnumValue<AreaTypeEnum>());
            if (area?.Code == null)
            {
                modelState.AddModelError(PropertyName, string.Format(CoreMessages.OpenApi.CodeNotFound, Model));
            }
        }
    }
}
