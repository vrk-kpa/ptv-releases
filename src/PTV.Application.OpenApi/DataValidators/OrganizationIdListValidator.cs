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
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for organization id list.
    /// </summary>
    public class OrganizationIdListValidator : BaseValidator<IList<string>>
    {
        private readonly ICommonService commonService;

        /// <summary>
        /// Ctor - organization id list validator.
        /// </summary>
        /// <param name="model">organization id list</param>
        /// <param name="commonService">Common service</param>
        /// <param name="propertyName">Property name</param>
        public OrganizationIdListValidator(IList<string> model, ICommonService commonService, string propertyName = "Organizations") : base(model, propertyName)
        {
            this.commonService = commonService;
        }

        /// <summary>
        /// Checks if organization id list is valid or not.
        /// </summary>
        /// <returns></returns>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;

            var i = 0;
            Model.ForEach(id =>
            {
                var organizationId = new OrganizationIdValidator(id, commonService, $"{PropertyName}[{ i++ }].OrganizationId");
                organizationId.Validate(modelState);
            });
        }
    }
}
