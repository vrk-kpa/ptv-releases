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
using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PTV.Framework.Attributes
{
    /// <summary>
    /// Checks the query string parameter uri that it is in right service class format.
    /// </summary>
    public class ValidateServiceClassAttribute : ActionFilterAttribute
    {
        private string key = "uri";
        private RegularExpressionAttribute validator;

        public ValidateServiceClassAttribute()
        {
            validator = new RegularExpressionAttribute(@"^http://urn.fi/URN:NBN:fi:au:ptvl:v[0-9]+$");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var queryString = context.HttpContext.Request.Query;
                var pair = queryString.FirstOrDefault(q => q.Key == key);
                if (pair.Key != null && pair.Value.Count > 0)
                {
                    var value = pair.Value.First();
                    var uri = WebUtility.UrlDecode(value as string);

                    var attributes = new List<ValidationAttribute>() { validator };
                    var results = new List<ValidationResult>();
                    bool isValid = Validator.TryValidateValue(uri, new ValidationContext(value), results, attributes);

                    if (!isValid)
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        context.ModelState.AddModelError(key, results.First()?.ErrorMessage);
                        context.Result = new BadRequestObjectResult(context.ModelState);
                    }
                }
            }
            catch (Exception)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.ModelState.AddModelError(key, validator.FormatErrorMessage(key));
                context.Result = new BadRequestObjectResult(context.ModelState);
            }

            base.OnActionExecuting(context);
        }
    }
}
