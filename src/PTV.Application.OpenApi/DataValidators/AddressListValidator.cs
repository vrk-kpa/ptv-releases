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

using PTV.Domain.Model.Models.OpenApi.V4;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PTV.Database.DataAccess.Interfaces.Services;
using PTV.Framework;

namespace PTV.Application.OpenApi.DataValidators
{
    /// <summary>
    /// Validator for address list.
    /// </summary>
    public class AddressListValidator : BaseValidator<IList<V4VmOpenApiAddressWithTypeIn>>
    {
        private ICodeService codeService;

        /// <summary>
        /// Ctor - address list validator.
        /// </summary>
        /// <param name="model">Address list</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="codeService">Code service</param>
        public AddressListValidator(IList<V4VmOpenApiAddressWithTypeIn> model, ICodeService codeService, string propertyName = "Addresses") : base(model, propertyName)
        {
            this.codeService = codeService;
        }

        /// <summary>
        /// Checks if address list is valid or not.
        /// </summary>
        /// <param name="modelState"></param>
        public override void Validate(ModelStateDictionary modelState)
        {
            if (Model == null) return;


            var i = 0;
            Model.ForEach(a =>
            {
                var address = new AddressValidator(a, $"{PropertyName}[{ i++ }]", codeService);
                address.Validate(modelState);
            });
        }
    }
}
