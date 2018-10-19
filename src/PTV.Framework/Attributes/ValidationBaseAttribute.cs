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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PTV.Framework.ServiceManager;
using System.Linq;

namespace PTV.Framework.Attributes
{
    /// <summary>
    /// Base class for validating REST api parameters.
    /// </summary>
    public class ValidationBaseAttribute : ActionFilterAttribute
    {
        private string keyName;
        private ValidationAttribute validator;
        private bool decode;
        private bool isRequired;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationBaseAttribute"/> class.
        /// </summary>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="validator">The validator.</param>
        public ValidationBaseAttribute(string keyName, ValidationAttribute validator, bool decode = false, bool isRequired = true)
        {
            if (string.IsNullOrEmpty(keyName) || string.IsNullOrWhiteSpace(keyName))
            {
                throw new PtvArgumentException("keyName cannot be null, empty string or whitespaces.");
            }

            this.keyName = keyName;
            this.validator = validator ?? throw new PtvArgumentException("validator cannot be null.");
            this.decode = decode;
            this.isRequired = isRequired;
        }

        /// <summary>
        /// Validation of request when action is executing
        /// </summary>
        /// <param name="context"></param>
        /// <inheritdoc />
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var value = context.ActionArguments[keyName];

                if (value != null)
                {
                    if (decode)
                    {
                        value = WebUtility.UrlDecode(value as string);
                    }
                    
                    var attributes = new List<ValidationAttribute>() { validator };
                    var results = new List<ValidationResult>();

                    bool isValid = Validator.TryValidateValue(value,
                                         new ValidationContext(value),
                                         results,
                                         attributes);

                    if (!isValid)
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        context.ModelState.AddModelError(keyName, results.First()?.ErrorMessage);
                        context.Result = new BadRequestObjectResult(context.ModelState);
                    }
                }
            }
            catch (KeyNotFoundException)
            {
                if (!isRequired) return;
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.ModelState.AddModelError(keyName, validator.FormatErrorMessage(keyName));
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
            base.OnActionExecuting(context);
        }
    }
}
